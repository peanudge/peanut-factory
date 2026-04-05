using Microsoft.AspNetCore.Mvc;
using PeanutVision.Api.Services;

namespace PeanutVision.Api.Controllers;

[ApiController]
[Route("api/settings")]
public class SettingsController : ControllerBase
{
    private readonly IImageSaveSettingsService _service;

    public SettingsController(IImageSaveSettingsService service)
    {
        _service = service;
    }

    [HttpGet("image-save")]
    public ActionResult<ImageSaveSettings> GetImageSaveSettings() => Ok(_service.GetSettings());

    [HttpPut("image-save")]
    public async Task<ActionResult<ImageSaveSettings>> UpdateImageSaveSettings(
        [FromBody] ImageSaveSettings settings)
    {
        if (string.IsNullOrWhiteSpace(settings.OutputDirectory))
            return BadRequest(new { errors = new[] { "OutputDirectory is required" } });

        await _service.SaveSettingsAsync(settings);
        return Ok(settings);
    }
}
