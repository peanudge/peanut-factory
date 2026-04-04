using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Api.Services;

/// <summary>
/// Saves an image to disk and records it in the catalog when AutoSave is enabled.
/// </summary>
public interface IAutoSaveService
{
    /// <summary>
    /// Saves the image if AutoSave is enabled. Returns the file path, or null if AutoSave is off.
    /// </summary>
    Task<string?> TrySaveAsync(ImageData image);
}
