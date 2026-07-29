using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RemsAPI.Data;
using RemsAPI.DTOs;
using RemsAPI.Entities;
using RemsAPI.Exceptions;
using RemsAPI.Interfaces;

namespace RemsAPI.Services;

public class CityService(RemsDbContext context, IMapper mapper) : ICityService
{
  public async Task<CityDto> CreateCityAsync(CreateCityDto createCityDto)
  {
    if (await CityExistsAsync(createCityDto.Name))
    {
      throw new ConflictException("This city already exists.");
    }

    var newCity = new City
    {
      Name = createCityDto.Name
    };
    try
    {
      context.Cities.Add(newCity);
      await context.SaveChangesAsync();
    }
    catch (Exception)
    {
      throw new BadRequestException("CreateCity failed due to a database error.");
    }


    return mapper.Map<CityDto>(newCity);
  }

  public async Task<bool> DeleteCityAsync(string cityId)
  {
    var city = await context.Cities.FindAsync(cityId);

    if (city != null)
    {
      try
      {
        context.Cities.Remove(city);
        await context.SaveChangesAsync();
      }
      catch (Exception)
      {
        throw new BadRequestException("DeleteCity failed due to a database error.");
      }
      return true;
    }

    throw new NotFoundException("The city you are trying to delete does not exist.");
  }

  public async Task<List<CityDto>> GetCitiesAsync()
  {
    List<City> cities = await context.Cities.AsNoTracking().ToListAsync();
    return mapper.Map<List<CityDto>>(cities);
  }

  public async Task<CityDto> UpdateCityAsync(UpdateCityDto updateCityDto)
  {
    var city = await context.Cities.FirstOrDefaultAsync(c => c.Id == updateCityDto.Id);

    if (city != null)
    {
      city.Name = updateCityDto.Name;
      try
      {
        await context.SaveChangesAsync();
      }
      catch (Exception)
      {
        throw new BadRequestException("UpdateCity failed due to a database error.");
      }

      return mapper.Map<CityDto>(city);
    }

    throw new NotFoundException("This city does not exist.");

  }

  private async Task<bool> CityExistsAsync(string cityName)
  {
    return await context.Cities.AnyAsync(x => x.Name.ToLower() == cityName.ToLower());
  }
}
