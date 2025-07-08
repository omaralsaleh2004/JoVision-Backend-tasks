using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FirstApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UploadController : ControllerBase
    {
        public class Upload
        {
            public IFormFile? File { get; set; }
            public string? Name { get; set; }
        }

        public class ImageMetadata
        {
            public string Name { get; set; } = "";
            public DateTime CreatedAt { get; set; }
            public DateTime LastModifiedAt { get; set; }
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Post([FromForm] Upload uploadFile)
        {
            var file = uploadFile.File;
            var name = string.IsNullOrWhiteSpace(uploadFile.Name) ? "Anonymous" : uploadFile.Name;

            if (file == null || file.Length == 0)
                return BadRequest("No file selected.");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(extension) ||
                (extension != ".pdf" && extension != ".png" && extension != ".jpg" && extension != ".jpeg"))
                return BadRequest("Invalid file type. Only PDF, PNG, JPG, and JPEG are allowed.");

            var newFileName = Guid.NewGuid().ToString() + extension;

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(),"uploads");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, newFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

           
            var metadata = new ImageMetadata
            {
                Name = name,
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow
            };

            var metadataPath = Path.Combine(folderPath, Path.GetFileNameWithoutExtension(newFileName) + ".json");
            await System.IO.File.WriteAllTextAsync(metadataPath, JsonSerializer.Serialize(metadata, new JsonSerializerOptions { WriteIndented = true }));

            var fileUrl = Url.Content($"~/uploads/{newFileName}");
            return Ok(new
            {
                url = fileUrl,
                uploader = name
            });
        }
    }
}
