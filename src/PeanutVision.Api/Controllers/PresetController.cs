using Microsoft.AspNetCore.Mvc;
using PeanutVision.Api.Services;

namespace PeanutVision.Api.Controllers;

[ApiController]
[Route("api/presets")]
public class PresetController : ControllerBase
{
    private readonly IAcquisitionConfigPresetService _presets;

    public PresetController(IAcquisitionConfigPresetService presets) => _presets = presets;

    [HttpGet]
    public ActionResult<IReadOnlyList<AcquisitionConfigPreset>> GetAll()
        => Ok(_presets.GetAll());

    [HttpGet("{name}")]
    public ActionResult<AcquisitionConfigPreset> GetByName(string name)
    {
        var preset = _presets.GetByName(name);
        return preset is null ? NotFound(new { error = $"Preset '{name}' not found" }) : Ok(preset);
    }

    [HttpPut]
    public async Task<ActionResult<AcquisitionConfigPreset>> Save([FromBody] AcquisitionConfigPreset preset)
    {
        if (string.IsNullOrWhiteSpace(preset.Name))
            return BadRequest(new { error = "Preset name is required" });
        if (string.IsNullOrWhiteSpace(preset.ProfileId))
            return BadRequest(new { error = "ProfileId is required" });

        await _presets.SaveAsync(preset);
        return Ok(preset);
    }

    [HttpDelete("{name}")]
    public async Task<ActionResult> Delete(string name)
    {
        await _presets.DeleteAsync(name);
        return NoContent();
    }
}
