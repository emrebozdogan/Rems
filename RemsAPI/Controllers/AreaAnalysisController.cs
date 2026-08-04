using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RemsAPI.DTOs;
using RemsAPI.Interfaces;

namespace RemsAPI.Controllers
{
    [Authorize(Roles = "Regular")]
    public class AreaAnalysisController(IAreaAnalysisService areaAnalysisService) : BaseController
    {
        [HttpPost]
        public async Task<IActionResult> PerformAnalysisAsync([FromBody] AreaAnalysisRequestDto areaAnalysisRequestDto)
        {
            var result = await areaAnalysisService.PerformAnalysisAsync(areaAnalysisRequestDto, UserId);
            return Ok(result);
        }

        [HttpPost("save-geometries")]
        public async Task<IActionResult> SaveGeometriesAsync([FromBody] SaveGeometriesRequestDto request)
        {
            await areaAnalysisService.SaveGeometriesAsync(request, UserId);
            return Ok(new { message = "Geometries saved successfully." });
        }

        [HttpGet("saved-geometries")]
        public async Task<IActionResult> GetSavedGeometriesAsync()
        {
            var result = await areaAnalysisService.GetSavedGeometriesAsync(UserId);
            if (result == null) return NotFound(new { message = "No saved geometries found." });
            return Ok(result);
        }
    }
}
