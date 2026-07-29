using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RemsAPI.Interfaces;

namespace RemsAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PdfController(IPdfService pdfService) : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> DownloadPdfAsync()
        {
            var pdfFile = await pdfService.ExportLogsToPdfAsync();
            string contentType = "application/pdf";
            string fileName = "RemsLogs.pdf";

            return File(pdfFile, contentType, fileName);
        }
    }
}
