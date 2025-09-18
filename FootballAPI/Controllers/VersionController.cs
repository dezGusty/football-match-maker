using Microsoft.AspNetCore.Mvc;

namespace FootballAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VersionController : ControllerBase
    {
        private readonly ILogger<VersionController> _logger;

        public VersionController(ILogger<VersionController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<string>> GetVersion()
        {
            try
            {
                // Get the path to the VERSION file in the root directory
                var versionFilePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "VERSION");
                
                if (!System.IO.File.Exists(versionFilePath))
                {
                    _logger.LogWarning("VERSION file not found at path: {VersionFilePath}", versionFilePath);
                    return NotFound("Version file not found");
                }

                var version = await System.IO.File.ReadAllTextAsync(versionFilePath);
                version = version.Trim(); // Remove any whitespace/newlines

                _logger.LogInformation("Retrieved version: {Version}", version);
                return Ok(new { version = version });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading version file");
                return StatusCode(500, "Error reading version information");
            }
        }
    }
}