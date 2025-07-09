using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

[ApiController]
[Route("[controller]")]
public class TransferOwnershipController : ControllerBase
{
    private readonly string metadataDirectory = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

    public class ImageMetadata
    {
        public string? Name { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ModificationDate { get; set; }
    }

    [HttpGet]
    public IActionResult TransferOwnership([FromQuery] string oldOwner, [FromQuery] string newOwner)
    {
        if (string.IsNullOrWhiteSpace(oldOwner) || string.IsNullOrWhiteSpace(newOwner))
            return BadRequest("OldOwner and NewOwner are required.");

        try
        {
            var jsonFiles = Directory.GetFiles(metadataDirectory, "*.json");
       
            foreach (var file in jsonFiles)
            {
                Console.WriteLine(file);
                var json = System.IO.File.ReadAllText(file);
                var metadata = JsonSerializer.Deserialize<ImageMetadata>(json);
                if (metadata == null) continue;

                if (string.Equals(metadata.Name, oldOwner, StringComparison.OrdinalIgnoreCase))
                {
                    metadata.Name = newOwner;
                    var updatedJson = JsonSerializer.Serialize(metadata);
                    System.IO.File.WriteAllText(file, updatedJson);
                }
            }

            var result = jsonFiles
                .Select(file =>
                {
                    var json = System.IO.File.ReadAllText(file);
                    var metadata = JsonSerializer.Deserialize<ImageMetadata>(json);
                    if (metadata == null || !string.Equals(metadata.Name, newOwner, StringComparison.OrdinalIgnoreCase))
                        return null;

                    return new
                    {
                        FileName = Path.GetFileNameWithoutExtension(file),
                        Owner = metadata.Name
                    };
                })
                .Where(x => x != null)
                .ToList();

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Server Error: " + ex.Message);
        }
    }
}
