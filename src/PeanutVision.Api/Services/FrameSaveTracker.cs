using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Api.Services;

/// <summary>
/// Tracks which ImageData instance was last saved to disk.
/// Prevents the same frame from being saved multiple times when
/// GET /latest-frame is polled faster than frames arrive.
/// </summary>
public sealed class FrameSaveTracker
{
    private volatile ImageData? _lastSaved;

    /// <summary>
    /// Returns true (and records the frame as saved) only if this frame
    /// has not been saved before. Safe for concurrent callers.
    /// </summary>
    public bool ShouldSave(ImageData frame)
    {
        var previous = Interlocked.Exchange(ref _lastSaved, frame);
        return !ReferenceEquals(previous, frame);
    }
}
