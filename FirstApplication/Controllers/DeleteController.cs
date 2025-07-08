using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FirstApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DeleteController : ControllerBase
    {
        public class ImageMetadata
        {
            public string Name { get; set; } = "";
            public DateTime CreatedAt { get; set; }
            public DateTime LastModifiedAt { get; set; }
        }

        private readonly string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

        [HttpDelete]
        public IActionResult DeleteFile([FromQuery] string? fileName, [FromQuery] string? fileOwner)
        {
            try
            {
                // Validate parameters
                if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(fileOwner))
                {
                    return BadRequest("File name and owner are required.");
                }

                var imagePath = Path.Combine(uploadPath, fileName);
                var metadataPath = Path.Combine(uploadPath, Path.GetFileNameWithoutExtension(fileName) + ".json");

                // Check if files exist
                if (!System.IO.File.Exists(imagePath) || !System.IO.File.Exists(metadataPath))
                {
                    return BadRequest("File or metadata not found.");
                }

                // Read and deserialize metadata
                var metadataJson = System.IO.File.ReadAllText(metadataPath);
                var metadata = JsonSerializer.Deserialize<ImageMetadata>(metadataJson);
                Console.WriteLine($"Looking for image at: {imagePath}");
                Console.WriteLine($"Looking for metadata at: {metadataPath}");


                if (metadata == null)
                {
                    return StatusCode(500, "Metadata corrupted or unreadable.");
                }

                // Check ownership
                if (!string.Equals(metadata.Name, fileOwner, StringComparison.OrdinalIgnoreCase))
                {
                    return Forbid("You are not authorized to delete this file.");
                }

                // Delete files
                System.IO.File.Delete(imagePath);
                System.IO.File.Delete(metadataPath);

                return Ok("File and metadata deleted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
