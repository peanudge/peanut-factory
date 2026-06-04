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
        var errors = Validate(settings);
        if (errors.Count > 0)
            return BadRequest(new { errors });

        await _service.SaveSettingsAsync(settings);
        return Ok(settings);
    }

    private static List<string> Validate(ImageSaveSettings settings)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(settings.OutputDirectory))
            errors.Add("OutputDirectory is required");

        return errors;
    }
}
