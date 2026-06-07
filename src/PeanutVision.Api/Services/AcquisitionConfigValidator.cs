using PeanutVision.MultiCamDriver.Camera;

namespace PeanutVision.Api.Services;

public sealed class AcquisitionConfigValidator
{
    private readonly ICamFileService _camFileService;

    public AcquisitionConfigValidator(ICamFileService camFileService)
    {
        _camFileService = camFileService;
    }

    public ValidationResult Validate(AcquisitionConfig config)
    {
        var result = new ValidationResult();

        // ProfileId
        if (string.IsNullOrWhiteSpace(config.ProfileId.Value))
        {
            result.Add("profileId", "Profile ID is required.");
        }
        else if (!_camFileService.TryGetByFileName(config.ProfileId.Value, out _))
        {
            result.Add("profileId", $"Camera profile '{config.ProfileId.Value}' not found.");
        }

        // IntervalMs
        if (config.IntervalMs.HasValue && config.IntervalMs.Value <= 0)
            result.Add("intervalMs", "Interval must be a positive value.");
        else if (config.IntervalMs.HasValue && config.IntervalMs.Value < 50)
            result.Add("intervalMs", "Interval must be at least 50ms.");

        // FrameCount
        if (config.FrameCount is <= 0)
            result.Add("frameCount", "Frame count must be greater than 0.");

        // OutputDirectory
        if (string.IsNullOrWhiteSpace(config.OutputDirectory))
            result.Add("outputDirectory", "Output directory is required.");

        return result;
    }
}
