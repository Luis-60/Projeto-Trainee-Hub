using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

[Route("api/upload")]
[ApiController]
public class UploadController : ControllerBase
{
    private readonly string _baseUploadFolder = "wwwroot/images/upload"; // Pasta raiz dos uploads

    public UploadController()
    {
        if (!Directory.Exists(_baseUploadFolder))
        {
            Directory.CreateDirectory(_baseUploadFolder); // Cria a pasta base se não existir
        }
    }

    [HttpPost("treinamentos")]
    public async Task<IActionResult> UploadTreinamento([FromForm] FileUploadModel model)
    {
        return await UploadFile(model, "treinamentos"); // Especifica a pasta treinamentos
    }

    private async Task<IActionResult> UploadFile(FileUploadModel model, string subFolder)
    {
        if (model.File == null || model.File.Length == 0)
        {
            return BadRequest("Nenhum arquivo foi enviado.");
        }

        // Define o caminho específico da subpasta
        string uploadFolder = Path.Combine(_baseUploadFolder, subFolder);

        if (!Directory.Exists(uploadFolder))
        {
            Directory.CreateDirectory(uploadFolder);
        }

        // Gera um nome único para o arquivo
        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.File.FileName);
        string filePath = Path.Combine(uploadFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await model.File.CopyToAsync(stream);
        }

        return Ok();
    }
}