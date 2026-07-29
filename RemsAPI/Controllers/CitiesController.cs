using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RemsAPI.DTOs;
using RemsAPI.Interfaces;

namespace RemsAPI.Controllers
{
    [Authorize]
    public class CitiesController(ICityService cityService) : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> GetCitiesAsync()
        {
            var result = await cityService.GetCitiesAsync();
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateCityAsync(CreateCityDto createCityDto)
        {
            var result = await cityService.CreateCityAsync(createCityDto);
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> UpdateCityAsync(UpdateCityDto updateCityDto)
        {
            var result = await cityService.UpdateCityAsync(updateCityDto);
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{cityId}")]
        public async Task<IActionResult> DeleteCityAsync(string cityId)
        {
            var result = await cityService.DeleteCityAsync(cityId);
            return Ok(result);
        }
    }
}