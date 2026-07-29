using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RemsAPI.Interfaces;

namespace RemsAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ExcelController(IExcelService excelService) : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> DownloadExcelAsync()
        {
            var excelFile = await excelService.ExportLogsToExcelAsync();
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            string fileName = "RemsLogs.xlsx";

            return File(excelFile, contentType, fileName);
        }
    }
}
