using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RemsAPI.DTOs;
using RemsAPI.Interfaces;

namespace RemsAPI.Controllers
{
    [Authorize]
    public class PropertyController(IPropertyService propertyService) : BaseController
    {
        [Authorize(Roles = "Regular")]
        [HttpPost]
        public async Task<IActionResult> CreatePropertyAsync([FromBody] CreatePropertyDto createPropertyDto)
        {
            var result = await propertyService.CreatePropertyAsync(createPropertyDto, UserId);
            return Created($"/api/Property/{result.Id}", result);
        }
        [Authorize(Roles = "Regular")]
        [HttpPut]
        public async Task<IActionResult> UpdatePropertyAsync([FromBody] UpdatePropertyDto updatePropertyDto)
        {
            var result = await propertyService.UpdatePropertyAsync(updatePropertyDto, UserId);
            return Ok(result);
        }
        [Authorize(Roles = "Regular")]
        [HttpDelete("{propertyId}")]
        public async Task<IActionResult> DeletePropertyAsync(string propertyId)
        {
            await propertyService.DeletePropertyAsync(propertyId, UserId);
            return NoContent();
        }
        [HttpGet]
        public async Task<IActionResult> GetFilteredPropertiesAsync([FromQuery] PropertyFilterDto propertyFilterDto)
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role)!;
            var result = await propertyService.GetFilteredPropertiesAsync(UserId, userRole, propertyFilterDto);
            return Ok(result);
        }
        [HttpGet("{propertyId}")]
        public async Task<IActionResult> GetPropertyByIdAsync(string propertyId)
        {
            var result = await propertyService.GetPropertyAsync(propertyId, UserId);
            return Ok(result);
        }
        [Authorize(Roles = "Regular")]
        [HttpPatch("{propertyId}/image")]
        public async Task<IActionResult> UploadImageAsync(string propertyId, IFormFile file)
        {
            var result = await propertyService.UploadImageAsync(propertyId, UserId, file);
            return Ok(new { Message = "Image uploaded successfully.", Data = result });
        }
        [Authorize(Roles = "Regular")]
        [HttpPost("import")]
        public async Task<IActionResult> ImportPropertiesFromExcelAsync(IFormFile file)
        {
            await propertyService.ImportPropertiesFromExcelAsync(UserId, file);
            return Ok(new { Message = "Properties imported successfully." });
        }
        [HttpGet("{propertyId}/image")]
        public async Task<IActionResult> GetPropertyImage(string propertyId)
        {
            var result = await propertyService.GetPropertyImage(propertyId);
            return File(result, "image/jpeg");
        }
    }
}
