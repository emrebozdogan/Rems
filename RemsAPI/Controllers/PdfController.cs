using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RemsAPI.DTOs;
using RemsAPI.Interfaces;

namespace RemsAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PdfController(IPdfService pdfService) : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> DownloadPdfAsync([FromServices] ILogService logService, [FromQuery] LogFilterDto logFilterDto)
        {
            var logs = await logService.GetAllFilteredLogsAsync(logFilterDto);
            var pdfFile = await pdfService.ExportLogsToPdfAsync(logs);
            string contentType = "application/pdf";
            string fileName = "RemsLogs.pdf";

            return File(pdfFile, contentType, fileName);
        }
    }
}
