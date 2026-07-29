using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RemsAPI.Data;
using RemsAPI.DTOs;
using RemsAPI.Entities;
using RemsAPI.Exceptions;
using RemsAPI.Interfaces;

namespace RemsAPI.Services;

public class NeighborhoodService(RemsDbContext context, IMapper mapper) : INeighborhoodService
{
  public async Task<NeighborhoodDto> CreateNeighborhoodAsync(CreateNeighborhoodDto createNeighborhoodDto)
  {
    if (await NeighborhoodExistsAsync(createNeighborhoodDto.DistrictId, createNeighborhoodDto.Name))
    {
      throw new ConflictException("This neighborhood already exists.");
    }

    var newNeighborhood = new Neighborhood
    {
      Name = createNeighborhoodDto.Name,
      DistrictId = createNeighborhoodDto.DistrictId
    };
    try
    {
      context.Neighborhoods.Add(newNeighborhood);
      await context.SaveChangesAsync();
    }
    catch (Exception)
    {
      throw new BadRequestException("CreateNeighborhood failed due to a database error.");
    }

    return mapper.Map<NeighborhoodDto>(newNeighborhood);
  }

  public async Task<bool> DeleteNeighborhoodAsync(string neighborhoodId)
  {
    var neighborhood = await context.Neighborhoods.FindAsync(neighborhoodId);
    if (neighborhood != null)
    {
      try
      {
        context.Neighborhoods.Remove(neighborhood);
        await context.SaveChangesAsync();
      }
      catch (Exception)
      {
        throw new BadRequestException("DeleteNeighborhood failed due to a database error.");
      }
      return true;
    }
    throw new NotFoundException("The neighborhood you are trying to delete does not exist.");
  }

  public async Task<List<NeighborhoodDto>> GetNeighborhoodByDistrictIdAsync(string districtId)
  {
    List<Neighborhood> neighborhoodsByDistrictId = await context.Neighborhoods.Where(n => n.DistrictId == districtId).AsNoTracking().ToListAsync();
    return mapper.Map<List<NeighborhoodDto>>(neighborhoodsByDistrictId);
  }

  public async Task<NeighborhoodDto> UpdateNeighborhoodAsync(UpdateNeighborhoodDto updateNeighborhoodDto)
  {
    var neighborhood = await context.Neighborhoods.FirstOrDefaultAsync(n => n.Id == updateNeighborhoodDto.Id);

    if (neighborhood != null)
    {
      neighborhood.Name = updateNeighborhoodDto.Name;
      neighborhood.DistrictId = updateNeighborhoodDto.DistrictId;
      try
      {
        await context.SaveChangesAsync();
      }
      catch (Exception)
      {
        throw new BadRequestException("UpdateNeighborhood failed due to a database error.");
      }
      return mapper.Map<NeighborhoodDto>(neighborhood);
    }

    throw new NotFoundException("This neighborhood does not exist.");
  }

  private async Task<bool> NeighborhoodExistsAsync(string neighborhoodDistrictId, string neighborhoodName)
  {
    return await context.Neighborhoods.AnyAsync(n => n.DistrictId == neighborhoodDistrictId && n.Name.ToLower() == neighborhoodName.ToLower());
  }
}
