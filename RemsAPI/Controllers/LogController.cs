using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RemsAPI.DTOs;
using RemsAPI.Interfaces;

namespace RemsAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class LogController(ILogService logService) : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> GetLogsAsync([FromQuery] LogFilterDto logFilterDto)
        {
            var result = await logService.GetLogsAsync(logFilterDto);
            return Ok(result);
        }
    }
}
