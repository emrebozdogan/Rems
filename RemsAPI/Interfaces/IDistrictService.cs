using RemsAPI.DTOs;

namespace RemsAPI.Interfaces;

public interface IDistrictService
{
  Task<List<DistrictDto>> GetDistrictsByCityIdAsync(string cityId);
  Task<DistrictDto> CreateDistrictAsync(CreateDistrictDto createDistrictDto);
  Task<DistrictDto> UpdateDistrictAsync(UpdateDistrictDto updateDistrictDto);
  Task<bool> DeleteDistrictAsync(string districtId);
}
