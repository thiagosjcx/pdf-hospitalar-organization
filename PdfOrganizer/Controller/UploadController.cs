using Microsoft.AspNetCore.Mvc;
using PdfOrganizer.Services;

namespace PdfOrganizer.Controllers;

[ApiController]
[Route("upload")]
public class UploadController : ControllerBase
{
    private readonly PdfService _pdfService;

    public UploadController(PdfService pdfService)
    {
        _pdfService = pdfService;
    }

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("Arquivo inválido");
        }

        var inputPath = Path.Combine(
            Path.GetTempPath(),
            file.FileName
        );

        using (var stream = new FileStream(inputPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var resultadoPdf = _pdfService.ProcessPdf(inputPath);

        var bytes = await System.IO.File.ReadAllBytesAsync(resultadoPdf);

        return File(
            bytes,
            "application/pdf",
            "resultado.pdf"
        );
    }
}