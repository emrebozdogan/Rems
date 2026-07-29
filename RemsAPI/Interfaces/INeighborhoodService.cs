using RemsAPI.DTOs;

namespace RemsAPI.Interfaces;

public interface INeighborhoodService
{
  Task<List<NeighborhoodDto>> GetNeighborhoodByDistrictIdAsync(string districtId);
  Task<NeighborhoodDto> CreateNeighborhoodAsync(CreateNeighborhoodDto createNeighborhoodDto);
  Task<NeighborhoodDto> UpdateNeighborhoodAsync(UpdateNeighborhoodDto updateNeighborhoodDto);
  Task<bool> DeleteNeighborhoodAsync(string neighborhoodId);
}
