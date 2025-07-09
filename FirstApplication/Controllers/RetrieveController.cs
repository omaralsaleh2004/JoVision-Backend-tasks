using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FirstApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RetrieveController : ControllerBase
    {
        public class ImageMetadata
        {
            public string Name { get; set; } = "";
            public DateTime CreatedAt { get; set; }
            public DateTime LastModifiedAt { get; set; }
        }

        private readonly string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

        [HttpGet]
        public IActionResult UpdateMetadata(
            [FromQuery] string? fileName,
            [FromQuery] string? fileOwner)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(fileOwner))
                    return BadRequest("Missing parameters: fileName, fileOwner, and newOwner are required.");

                var imagePath = Path.Combine(uploadPath, fileName);
                var metadataPath = Path.Combine(uploadPath, Path.GetFileNameWithoutExtension(fileName) + ".json");

                if (!System.IO.File.Exists(imagePath) || !System.IO.File.Exists(metadataPath))
                    return BadRequest("File or metadata not found.");

                var metadataJson = System.IO.File.ReadAllText(metadataPath);
                var metadata = JsonSerializer.Deserialize<ImageMetadata>(metadataJson);

                if (metadata == null)
                    return StatusCode(500, "Metadata corrupted or unreadable.");

                if (!string.Equals(metadata.Name, fileOwner, StringComparison.OrdinalIgnoreCase))
                    return BadRequest("the name you entered is not Equal to the owner name file !");


                return Ok( new
                {
                    name = metadata.Name,
                    fileName = fileName
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
