using Microsoft.AspNetCore.Mvc;
using PeanutVision.Api.Services;

namespace PeanutVision.Api.Controllers;

[ApiController]
[Route("api/settings")]
public class SettingsController : ControllerBase
{
    private readonly IImageSaveSettingsService _service;
    private readonly IWebHostEnvironment _env;

    public SettingsController(IImageSaveSettingsService service, IWebHostEnvironment env)
    {
        _service = service;
        _env = env;
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

    /// <summary>
    /// Partially updates OutputDirectory and/or FilenamePrefix.
    /// Changes are immediately reflected in memory and applied to subsequent saves.
    /// </summary>
    [HttpPatch("image-save")]
    public async Task<ActionResult<ImageSaveSettings>> PatchImageSaveSettings(
        [FromBody] ImageSaveSettingsPatch patch)
    {
        var errors = ValidatePatch(patch);
        if (errors.Count > 0)
            return BadRequest(new { errors });

        var current = _service.GetSettings();
        var updated = current with
        {
            OutputDirectory = patch.OutputDirectory ?? current.OutputDirectory,
            FilenamePrefix = patch.FilenamePrefix ?? current.FilenamePrefix,
        };

        await _service.SaveSettingsAsync(updated);
        return Ok(updated);
    }

    /// <summary>
    /// Returns disk usage information for the configured output directory's drive.
    /// </summary>
    [HttpGet("disk-usage")]
    public ActionResult<DiskUsageDto> GetDiskUsage()
    {
        var settings = _service.GetSettings();
        var outputDir = settings.OutputDirectory;

        // Resolve relative paths against the content root
        var resolvedPath = Path.IsPathRooted(outputDir)
            ? outputDir
            : Path.Combine(_env.ContentRootPath, outputDir);

        // Walk up to find an existing ancestor directory for DriveInfo
        var probe = resolvedPath;
        while (!string.IsNullOrEmpty(probe) && !Directory.Exists(probe))
            probe = Path.GetDirectoryName(probe) ?? string.Empty;

        if (string.IsNullOrEmpty(probe))
            probe = _env.ContentRootPath;

        try
        {
            var drive = new DriveInfo(probe);
            var totalBytes = drive.TotalSize;
            var freeBytes = drive.TotalFreeSpace;
            var usedBytes = totalBytes - freeBytes;
            var usagePercent = totalBytes > 0
                ? Math.Round((double)usedBytes / totalBytes * 100, 2)
                : 0.0;

            return Ok(new DiskUsageDto(totalBytes, usedBytes, freeBytes, usagePercent));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = $"Failed to read disk info: {ex.Message}" });
        }
    }

    private static List<string> ValidatePatch(ImageSaveSettingsPatch patch)
    {
        var errors = new List<string>();

        if (patch.OutputDirectory is not null)
        {
            if (string.IsNullOrWhiteSpace(patch.OutputDirectory))
                errors.Add("OutputDirectory cannot be empty");
            else if (patch.OutputDirectory.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
                errors.Add("OutputDirectory contains invalid path characters");
        }

        if (patch.FilenamePrefix is not null)
        {
            if (string.IsNullOrWhiteSpace(patch.FilenamePrefix))
                errors.Add("FilenamePrefix cannot be empty");
            else if (patch.FilenamePrefix.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                errors.Add("FilenamePrefix contains invalid filename characters");
        }

        return errors;
    }

    private static List<string> Validate(ImageSaveSettings settings)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(settings.OutputDirectory))
            errors.Add("OutputDirectory is required");
        else if (settings.OutputDirectory.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
            errors.Add("OutputDirectory contains invalid path characters");

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

/// <summary>
/// Patch payload for quick-editing OutputDirectory and FilenamePrefix from the main screen.
/// Null fields are left unchanged.
/// </summary>
public sealed record ImageSaveSettingsPatch
{
    public string? OutputDirectory { get; init; }
    public string? FilenamePrefix { get; init; }
}

/// <summary>
/// Disk usage information for the output directory's drive.
/// </summary>
public sealed record DiskUsageDto(
    long TotalBytes,
    long UsedBytes,
    long FreeBytes,
    double UsagePercent);
