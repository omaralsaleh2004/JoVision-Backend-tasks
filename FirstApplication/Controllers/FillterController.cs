using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

[ApiController]
[Route("[controller]")]
public class FilterController : ControllerBase
{
    private readonly string metadataDirectory = Path.Combine(Directory.GetCurrentDirectory(), "uploads");


    public enum FilterType
    {
        ByModificationDate,
        ByCreationDateDescending,
        ByCreationDateAscending,
        ByOwner
    }

    public class FilterRequest
    {
        public DateTime? CreationDate { get; set; }
        public DateTime? ModificationDate { get; set; }
        public string  ? Owner { get; set; }
        public FilterType? FilterType { get; set; }
    }

   

    public class ImageMetadata
    {
        public string ?Name { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ModificationDate { get; set; }
    }



    [HttpPost]
    public IActionResult Filter([FromForm] FilterRequest request)
    {
        try
        {
            if (request.FilterType == null)
                return BadRequest("FilterType is required.");

            var jsonFiles = Directory.GetFiles(metadataDirectory, "*.json");

            var results = jsonFiles
                .Select(file =>
                {
                    var json = System.IO.File.ReadAllText(file);
                    var metadata = JsonSerializer.Deserialize<ImageMetadata>(json);
                    if (metadata == null) return null;

                    return new
                    {
                        FileName = Path.GetFileNameWithoutExtension(file),
                        Owner = metadata.Name,
                        CreationDate = metadata.CreationDate,
                        ModificationDate = metadata.ModificationDate
                    };
                })
                .Where(x => x != null)
                .ToList();

            switch (request.FilterType)
            {
                case FilterType.ByModificationDate:
                    if (!request.ModificationDate.HasValue)
                        return BadRequest("ModificationDate is required.");
                    results = results
                        .Where(f => f.ModificationDate < request.ModificationDate)
                        .ToList();
                    break;

                case FilterType.ByCreationDateDescending:
                    if (!request.CreationDate.HasValue)
                        return BadRequest("CreationDate is required.");
                    results = results
                        .Where(f => f.CreationDate > request.CreationDate)
                        .OrderByDescending(f => f.CreationDate)
                        .ToList();
                    break;

                case FilterType.ByCreationDateAscending:
                    if (!request.CreationDate.HasValue)
                        return BadRequest("CreationDate is required.");
                    results = results
                        .Where(f => f.CreationDate > request.CreationDate)
                        .OrderBy(f => f.CreationDate)
                        .ToList();
                    break;

                case FilterType.ByOwner:
                    if (string.IsNullOrEmpty(request.Owner))
                        return BadRequest("Owner is required.");
                    results = results
                        .Where(f => string.Equals(f.Owner, request.Owner, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                    break;

                default:
                    return BadRequest("Invalid FilterType.");
            }

            var filtered = results.Select(r => new { r.FileName, r.Owner });
            return Ok(filtered);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Server Error: " + ex.Message);
        }
    }
}
