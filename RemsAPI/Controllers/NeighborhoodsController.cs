using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RemsAPI.DTOs;
using RemsAPI.Interfaces;

namespace RemsAPI.Controllers
{
    [Authorize]
    public class NeighborhoodsController(INeighborhoodService neighborhoodService) : BaseController
    {
        [HttpGet("by-district/{districtId}")]
        public async Task<IActionResult> GetNeighborhoodsByDistrictIdAsync(string districtId)
        {
            var result = await neighborhoodService.GetNeighborhoodByDistrictIdAsync(districtId);
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateNeighborhoodAsync(CreateNeighborhoodDto createNeighborhoodDto)
        {
            var result = await neighborhoodService.CreateNeighborhoodAsync(createNeighborhoodDto);
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> UpdateNeighborhoodAsync(UpdateNeighborhoodDto updateNeighborhoodDto)
        {
            var result = await neighborhoodService.UpdateNeighborhoodAsync(updateNeighborhoodDto);
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{neighborhoodId}")]
        public async Task<IActionResult> DeleteNeighborhoodAsync(string neighborhoodId)
        {
            var result = await neighborhoodService.DeleteNeighborhoodAsync(neighborhoodId);
            return Ok(result);
        }
    }
}
