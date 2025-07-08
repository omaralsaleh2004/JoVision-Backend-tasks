using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FirstApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UpdateController : ControllerBase
    {
        public class ImageMetadata
        {
            public string Name { get; set; } = "";
            public DateTime CreatedAt { get; set; }
            public DateTime LastModifiedAt { get; set; }
        }

        private readonly string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

        [HttpPut] // Change to PUT since it's an update operation
        public IActionResult UpdateMetadata(
            [FromQuery] string? fileName,
            [FromQuery] string? fileOwner,
            [FromQuery] string? newOwner)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(fileOwner) || string.IsNullOrWhiteSpace(newOwner))
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
                    return Forbid("You are not authorized to update this file.");

                // ✅ Update only the name and LastModifiedAt
                metadata.Name = newOwner;
                metadata.LastModifiedAt = DateTime.UtcNow;

                // ✅ Save the updated metadata
                var updatedJson = JsonSerializer.Serialize(metadata, new JsonSerializerOptions { WriteIndented = true });
                System.IO.File.WriteAllText(metadataPath, updatedJson);

                return Ok("Metadata updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
