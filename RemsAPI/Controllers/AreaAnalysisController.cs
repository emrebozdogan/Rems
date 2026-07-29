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
    }
}
