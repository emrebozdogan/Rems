using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RemsAPI.DTOs;
using RemsAPI.Interfaces;

namespace RemsAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ExcelController(IExcelService excelService) : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> DownloadExcelAsync([FromServices] ILogService logService, [FromQuery] LogFilterDto logFilterDto)
        {
            var logs = await logService.GetAllFilteredLogsAsync(logFilterDto);
            var excelFile = await excelService.ExportLogsToExcelAsync(logs);
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string fileName = "RemsLogs.xlsx";

            return File(excelFile, contentType, fileName);
        }
    }
}
