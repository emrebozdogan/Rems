using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RemsAPI.Data;
using RemsAPI.DTOs;
using RemsAPI.Entities;
using RemsAPI.Exceptions;
using RemsAPI.Interfaces;

namespace RemsAPI.Services;

public class DistrictService(RemsDbContext context, IMapper mapper) : IDistrictService
{
  public async Task<DistrictDto> CreateDistrictAsync(CreateDistrictDto createDistrictDto)
  {
    if (await DistrictExistsAsync(createDistrictDto.CityId, createDistrictDto.Name))
    {
      throw new ConflictException("This district already exists.");
    }

    var newDistrict = new District
    {
      Name = createDistrictDto.Name,
      CityId = createDistrictDto.CityId
    };
    try
    {
      context.Districts.Add(newDistrict);
      await context.SaveChangesAsync();
    }
    catch (Exception)
    {
      throw new BadRequestException("CreateDistrict failed due to a database error.");
    }

    return mapper.Map<DistrictDto>(newDistrict);
  }

  public async Task<bool> DeleteDistrictAsync(string districtId)
  {
    var district = await context.Districts.FindAsync(districtId);

    if (district != null)
    {
      try
      {
        context.Districts.Remove(district);
        await context.SaveChangesAsync();
      }
      catch (Exception)
      {
        throw new BadRequestException("DeleteDistrict failed due to a database error.");
      }
      return true;
    }

    throw new NotFoundException("The district you are trying to delete does not exist.");
  }

  public async Task<List<DistrictDto>> GetDistrictsByCityIdAsync(string cityId)
  {
    List<District> districtsByCityId = await context.Districts.Where(d => d.CityId == cityId).AsNoTracking().ToListAsync();
    return mapper.Map<List<DistrictDto>>(districtsByCityId);
  }

  public async Task<DistrictDto> UpdateDistrictAsync(UpdateDistrictDto updateDistrictDto)
  {
    var district = await context.Districts.FirstOrDefaultAsync(d => d.Id == updateDistrictDto.Id);

    if (district != null)
    {
      district.Name = updateDistrictDto.Name;
      district.CityId = updateDistrictDto.CityId;
      try
      {
        await context.SaveChangesAsync();
        return mapper.Map<DistrictDto>(district);
      }
      catch (Exception)
      {

        throw new BadRequestException("UpdateDistrict failed due to a database error.");
      }
    }

    throw new NotFoundException("This district does not exist.");
  }

  private async Task<bool> DistrictExistsAsync(string districtCityId, string districtName)
  {
    return await context.Districts.AnyAsync(d => d.CityId == districtCityId && d.Name.ToLower() == districtName.ToLower());
  }
}
