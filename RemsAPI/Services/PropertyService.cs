using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RemsAPI.Data;
using RemsAPI.DTOs;
using RemsAPI.Exceptions;
using RemsAPI.Interfaces;
using RemsAPI.Entities;
using ClosedXML.Excel;
using NetTopologySuite.IO;
namespace RemsAPI.Services;

public class PropertyService(RemsDbContext context, IMapper mapper, WKTReader reader) : IPropertyService
{
  public async Task<PropertyDto> CreatePropertyAsync(CreatePropertyDto createPropertyDto, string userId)
  {
    if (await PropertyExists(createPropertyDto.Geometry, createPropertyDto.NeighborhoodId))
    {
      throw new ConflictException("This property is already exists.");
    }

    var newProperty = mapper.Map<Property>(createPropertyDto);
    newProperty.UserId = userId;
    newProperty.Geometry = reader.Read(createPropertyDto.Geometry);

    try
    {
      context.Properties.Add(newProperty);
      await context.SaveChangesAsync();
    }
    catch (Exception)
    {
      throw new BadRequestException("CreatePropertyAsync failed due to a database error.");
    }

    return mapper.Map<PropertyDto>(newProperty);

  }

  public async Task<bool> DeletePropertyAsync(string propertyId, string userId)
  {
    var property = await context.Properties.FindAsync(propertyId);
    if (property != null)
    {
      if (property.UserId == userId)
      {
        try
        {
          context.Properties.Remove(property);
          await context.SaveChangesAsync();
        }
        catch (Exception)
        {
          throw new BadRequestException("DeletePropertyAsync failed due to a database error.");
        }
        return true;
      }
      throw new UnauthorizedException("You don't own the property you are trying to delete");
    }
    throw new NotFoundException("The property you are trying to delete does not exist.");
  }

  private IQueryable<Property> BuildFilteredPropertyQuery(string userId, string userRole, PropertyFilterDto propertyFilterDto)
  {
    var query = context.Properties.AsNoTracking().AsQueryable();

    if (userRole.ToLower() != "admin")
    {
      query = query.Where(p => p.UserId == userId);
    }
    else if (userRole.ToLower() == "admin" && !string.IsNullOrEmpty(propertyFilterDto.OwnerId))
    {
      query = query.Where(p => p.UserId == propertyFilterDto.OwnerId);
    }

    if (!string.IsNullOrEmpty(propertyFilterDto.ParcelNumber))
    {
      query = query.Where(p => EF.Functions.ILike(p.ParcelNumber, $"%{propertyFilterDto.ParcelNumber}%"));
    }
    if (!string.IsNullOrEmpty(propertyFilterDto.LotNumber))
    {
      query = query.Where(p => EF.Functions.ILike(p.LotNumber, $"%{propertyFilterDto.LotNumber}%"));
    }
    if (!string.IsNullOrEmpty(propertyFilterDto.Address))
    {
      query = query.Where(p => p.Address.ToLower().Contains(propertyFilterDto.Address.ToLower()));
    }
    if (!string.IsNullOrEmpty(propertyFilterDto.PropertyType))
    {
      query = query.Where(p => p.PropertyType.ToLower().Contains(propertyFilterDto.PropertyType.ToLower()));
    }
    if (!string.IsNullOrEmpty(propertyFilterDto.NeighborhoodName))
    {
      query = query.Where(p => p.Neighborhood!.Name.ToLower().Contains(propertyFilterDto.NeighborhoodName.ToLower()));
    }
    if (!string.IsNullOrEmpty(propertyFilterDto.DistrictName))
    {
      query = query.Where(p => p.Neighborhood!.District!.Name.ToLower().Contains(propertyFilterDto.DistrictName.ToLower()));
    }
    if (!string.IsNullOrEmpty(propertyFilterDto.CityName))
    {
      query = query.Where(p => p.Neighborhood!.District!.City!.Name.ToLower().Contains(propertyFilterDto.CityName.ToLower()));
    }
    
    if (propertyFilterDto.SelectedIds != null && propertyFilterDto.SelectedIds.Any())
    {
      query = query.Where(p => propertyFilterDto.SelectedIds.Contains(p.Id));
    }
    
    return query;
  }

  public async Task<PaginatedResults<PropertyDto>> GetFilteredPropertiesAsync(string userId, string userRole, PropertyFilterDto propertyFilterDto)
  {
    var query = BuildFilteredPropertyQuery(userId, userRole, propertyFilterDto);

    var totalCount = await query.CountAsync();
    var items = await query
      .Include(p => p.Neighborhood)
      .ThenInclude(n => n!.District)
      .ThenInclude(d => d!.City)
      .Skip((propertyFilterDto.PageNumber - 1) * propertyFilterDto.PageSize)
      .Take(propertyFilterDto.PageSize)
      .ToListAsync();

    var properties = new PaginatedResults<PropertyDto>
    {
      Data = mapper.Map<List<PropertyDto>>(items),
      TotalCount = totalCount,
      PageSize = propertyFilterDto.PageSize,
      PageNumber = propertyFilterDto.PageNumber
    };

    return properties;
  }

  public async Task<List<PropertyDto>> GetAllFilteredPropertiesAsync(string userId, string userRole, PropertyFilterDto propertyFilterDto)
  {
    var query = BuildFilteredPropertyQuery(userId, userRole, propertyFilterDto);
    
    var properties = await query
      .Include(p => p.User)
      .Include(p => p.Neighborhood!)
        .ThenInclude(n => n.District!)
          .ThenInclude(d => d.City)
      .OrderBy(p => p.Id)
      .ToListAsync();

    return mapper.Map<List<PropertyDto>>(properties);
  }

  public async Task<PropertyDto> GetPropertyAsync(string propertyId, string userId)
  {
    var property = await context.Properties
      .Include(p => p.Neighborhood)
      .ThenInclude(n => n!.District)
      .ThenInclude(d => d!.City)
      .FirstOrDefaultAsync(p => p.Id == propertyId && p.UserId == userId);
    if (property != null)
    {
      return mapper.Map<PropertyDto>(property);
    }
    throw new NotFoundException("The property that you searching for does not exist.");
  }

  public async Task<byte[]> GetPropertyImage(string propertyId)
  {
    var property = await context.Properties.FindAsync(propertyId);
    if (property == null || string.IsNullOrEmpty(property.ImagePath))
    {
      throw new NotFoundException("Image not found.");
    }

    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", property.ImagePath);
    if (!File.Exists(filePath))
    {
      throw new NotFoundException("File is not exists.");
    }

    return await File.ReadAllBytesAsync(filePath);
  }

  public async Task<bool> ImportPropertiesFromExcelAsync(string userId, IFormFile file)
  {
    var extension = Path.GetExtension(file.FileName);

    if (extension != ".xlsx")
    {
      throw new BadRequestException("Import failed. Please check the file format and data.");
    }
    using var stream = file.OpenReadStream();
    using var workbook = new XLWorkbook(stream);
    var worksheet = workbook.Worksheet(1);
    var rows = worksheet.RangeUsed()!.RowsUsed();

    var propertiesToImport = new List<Property>();

    foreach (var row in rows)
    {
      if (row.RowNumber() == 1 || row.IsEmpty())
      {
        continue;
      }

      var parcelNumber = row.Cell(1).GetValue<string?>();
      if (string.IsNullOrWhiteSpace(parcelNumber))
      {
        throw new BadRequestException("Import failed. Please check the file format and data.");
      }
      var lotNumber = row.Cell(2).GetValue<string?>();
      if (string.IsNullOrWhiteSpace(lotNumber))
      {
        throw new BadRequestException("Import failed. Please check the file format and data.");
      }
      var address = row.Cell(3).GetValue<string?>();
      if (string.IsNullOrEmpty(address) || string.IsNullOrWhiteSpace(address))
      {
        throw new BadRequestException("Import failed. Please check the file format and data.");
      }
      var coordinate = row.Cell(4).GetValue<string?>();
      if (string.IsNullOrWhiteSpace(coordinate))
      {
        throw new BadRequestException("Import failed. Please check the file format and data.");
      }
      var propertyType = row.Cell(5).GetValue<string?>();
      if (string.IsNullOrWhiteSpace(propertyType))
      {
        throw new BadRequestException("Import failed. Please check the file format and data.");
      }
      var neighborhoodName = row.Cell(6).GetValue<string?>();
      if (string.IsNullOrWhiteSpace(neighborhoodName))
      {
        throw new BadRequestException("Import failed. Please check the file format and data.");
      }
      var districtName = row.Cell(7).GetValue<string?>();
      if (string.IsNullOrWhiteSpace(districtName))
      {
        throw new BadRequestException("Import failed. Please check the file format and data.");
      }
      var cityName = row.Cell(8).GetValue<string?>();
      if (string.IsNullOrWhiteSpace(cityName))
      {
        throw new BadRequestException("Import failed. Please check the file format and data.");
      }

      var neighborhood = await context.Neighborhoods.FirstOrDefaultAsync(n => n.Name.ToLower() == neighborhoodName.ToLower().Trim() && n.District!.Name.ToLower() == districtName.ToLower().Trim() && n.District.City!.Name.ToLower() == cityName.ToLower().Trim()) ?? throw new BadRequestException("Import failed. Please check the file format and data.");

      var parsedGeometry = reader.Read(coordinate);
      var propertyToAdd = new Property
      {
        ParcelNumber = parcelNumber,
        LotNumber = lotNumber,
        Address = address,
        Geometry = parsedGeometry,
        PropertyType = propertyType,
        NeighborhoodId = neighborhood.Id,
        UserId = userId
      };

      propertiesToImport.Add(propertyToAdd);
    }

    try
    {
      await context.Properties.AddRangeAsync(propertiesToImport);
      await context.SaveChangesAsync();
    }
    catch (Exception)
    {
      throw new BadRequestException("Importing properties failed due to a database error.");
    }

    return true;
  }

  public async Task<PropertyDto> UpdatePropertyAsync(UpdatePropertyDto updatePropertyDto, string userId)
  {
    var property = await context.Properties.FindAsync(updatePropertyDto.Id);
    if (property != null)
    {
      if (property.UserId == userId)
      {
        try
        {
          mapper.Map(updatePropertyDto, property);
          property.Geometry = reader.Read(updatePropertyDto.Geometry);
          context.Properties.Update(property);
          await context.SaveChangesAsync();
        }
        catch (Exception)
        {
          throw new BadRequestException("UpdatePropertyAsync failed due to a database error.");
        }
        return mapper.Map<PropertyDto>(property);
      }
      throw new UnauthorizedException("You don't own the property you are trying to update");
    }
    throw new NotFoundException("The property you are trying to update does not exist.");
  }

  public async Task<string> UploadImageAsync(string propertyId, string userId, IFormFile file)
  {
    var property = await context.Properties.FindAsync(propertyId);
    var allowedExtensions = new List<string>
    {
      ".jpg", ".jpeg", ".png"
    };
    long maxBytes = 100L * 1024 * 1024;

    if (property != null)
    {
      if (property.UserId == userId)
      {
        var extension = Path.GetExtension(file.FileName);
        if (allowedExtensions.Contains(extension))
        {
          var fileSizeBytes = file.Length;
          if (fileSizeBytes <= maxBytes)
          {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            if (!Directory.Exists(folderPath))
            {
              Directory.CreateDirectory(folderPath);
            }
            var fileName = Guid.NewGuid();

            var fullPath = Path.Combine(folderPath, fileName.ToString() + extension);

            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            property.ImagePath = fileName + extension;

            try
            {
              await context.SaveChangesAsync();
            }
            catch (Exception)
            {
              throw new BadRequestException("Saving file failed due to a database error.");
            }
            return property.ImagePath;
          }
          else
          {
            throw new BadRequestException("File size needs to be less then 100MB");
          }
        }
        else
        {
          throw new BadRequestException("File extension is not supported. (Available extensions: '.jpg', '.jpeg', '.png')");
        }
      }
      else
      {
        throw new UnauthorizedException("You have no access to this property");
      }
    }
    else
    {
      throw new BadRequestException("This property does not exists.");
    }
  }

  private async Task<bool> PropertyExists(string propertyCoordinate, string neighborhoodId)
  {
    var parsedGeometry = reader.Read(propertyCoordinate);
    return await context.Properties.AnyAsync(p => p.NeighborhoodId == neighborhoodId && p.Geometry.Equals(parsedGeometry));
  }
}
