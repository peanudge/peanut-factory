using PeanutVision.MultiCamDriver;

namespace PeanutVision.Api.Services;

/// <summary>Captures a single frame from the camera and persists it to disk and database.</summary>
public interface ISnapshotCapture
{
    /// <summary>
    /// Creates a temporary channel, captures one frame, saves it to disk, records in DB.
    /// Returns the saved file path.
    /// </summary>
    Task<string> CaptureAsync(ProfileId profileId, TriggerMode? triggerMode = null);
}
