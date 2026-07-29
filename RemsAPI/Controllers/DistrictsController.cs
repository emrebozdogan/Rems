using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RemsAPI.DTOs;
using RemsAPI.Interfaces;

namespace RemsAPI.Controllers
{
    [Authorize]
    public class DistrictsController(IDistrictService districtService) : BaseController
    {
        [HttpGet("by-city/{cityId}")]
        public async Task<IActionResult> GetDistrictsByCityIdAsync(string cityId)
        {
            var result = await districtService.GetDistrictsByCityIdAsync(cityId);
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateDistrictAsync(CreateDistrictDto createDistrictDto)
        {
            var result = await districtService.CreateDistrictAsync(createDistrictDto);
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> UpdateDistrictAsync(UpdateDistrictDto updateDistrictDto)
        {
            var result = await districtService.UpdateDistrictAsync(updateDistrictDto);
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{districtId}")]
        public async Task<IActionResult> DeleteDistrictAsync(string districtId)
        {
            var result = await districtService.DeleteDistrictAsync(districtId);
            return Ok(result);
        }
    }
}
