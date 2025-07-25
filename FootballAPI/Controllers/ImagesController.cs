using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ImagesController : ControllerBase
{
    [HttpGet]
    public IActionResult GetImages()
    {
        var imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "Images");
        var files = Directory.GetFiles(imagesPath)
            .Select(f => Path.GetFileName(f))
            .ToList();

        var urls = files.Select(f => $"http://localhost:5145/images/{f}").ToList();
        return Ok(urls);
    }
}