using RemsAPI.DTOs;

namespace RemsAPI.Interfaces;

public interface ICityService
{
  Task<List<CityDto>> GetCitiesAsync();
  Task<CityDto> CreateCityAsync(CreateCityDto createCityDto);
  Task<CityDto> UpdateCityAsync(UpdateCityDto updateCityDto);
  Task<bool> DeleteCityAsync(string cityId);
}
