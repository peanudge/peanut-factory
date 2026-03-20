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

        if (string.IsNullOrWhiteSpace(settings.FilenamePrefix))
            errors.Add("FilenamePrefix is required");
        else if (settings.FilenamePrefix.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            errors.Add("FilenamePrefix contains invalid filename characters");

        if (string.IsNullOrWhiteSpace(settings.TimestampFormat))
            errors.Add("TimestampFormat is required");
        else
        {
            try { _ = DateTime.Now.ToString(settings.TimestampFormat); }
            catch { errors.Add("TimestampFormat is not a valid date/time format string"); }
        }

        return errors;
    }
}
