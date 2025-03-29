using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

public class FileUploadModel
{
    [Required]
    public IFormFile File { get; set; }
}