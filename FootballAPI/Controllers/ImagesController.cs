using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ImagesController : ControllerBase
{
    private readonly string _imagesPath;

    public ImagesController()
    {
        _imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Images", "Profile");
        if (!Directory.Exists(_imagesPath))
        {
            Directory.CreateDirectory(_imagesPath);
        }
    }

    [HttpGet]
    public IActionResult GetImages()
    {
        var files = Directory.GetFiles(_imagesPath)
            .Select(f => Path.GetFileName(f))
            .ToList();

        var urls = files.Select(f => $"http://localhost:5145/images/Profile/{f}").ToList();
        return Ok(urls);
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded");
        }

        // Generate a unique filename
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(_imagesPath, fileName);

        try
        {
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return the URL that can be used to access the image
            return Ok(new { imageUrl = $"http://localhost:5145/images/Profile/{fileName}" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}