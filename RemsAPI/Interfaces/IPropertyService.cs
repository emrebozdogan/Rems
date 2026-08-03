using RemsAPI.DTOs;

namespace RemsAPI.Interfaces;

public interface IPropertyService
{
  Task<PaginatedResults<PropertyDto>> GetFilteredPropertiesAsync(string userId, string userRole, PropertyFilterDto propertyFilterDto);
  Task<List<PropertyDto>> GetAllFilteredPropertiesAsync(string userId, string userRole, PropertyFilterDto propertyFilterDto);
  Task<PropertyDto> GetPropertyAsync(string propertyId, string userId);
  Task<PropertyDto> CreatePropertyAsync(CreatePropertyDto createPropertyDto, string userId);
  Task<PropertyDto> UpdatePropertyAsync(UpdatePropertyDto updatePropertyDto, string userId);
  Task<bool> DeletePropertyAsync(string propertyId, string userId);
  Task<string> UploadImageAsync(string propertyId, string userId, IFormFile file);
  Task<byte[]> GetPropertyImage(string propertyId);
  Task<bool> ImportPropertiesFromExcelAsync(string userId, IFormFile file);
}