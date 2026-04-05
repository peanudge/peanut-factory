using PeanutVision.Capture;
using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Api.Services;

/// <summary>
/// Writes an ImageData to disk via IFrameWriter and records it in the catalog via FrameSavedHandler.
/// </summary>
public sealed class AutoSaveService : IAutoSaveService
{
    private readonly IImageSaveSettingsService _saveSettings;
    private readonly IFrameWriter              _frameWriter;
    private readonly FrameSavedHandler         _savedHandler;
    private readonly string                    _contentRootPath;

    public AutoSaveService(
        IImageSaveSettingsService saveSettings,
        IFrameWriter frameWriter,
        FrameSavedHandler savedHandler,
        IWebHostEnvironment environment)
    {
        _saveSettings    = saveSettings;
        _frameWriter     = frameWriter;
        _savedHandler    = savedHandler;
        _contentRootPath = environment.ContentRootPath;
    }

    public async Task<string?> TrySaveAsync(ImageData image)
    {
        var settings = _saveSettings.GetSettings();
        if (!settings.AutoSave) return null;

        var opts     = settings.ToWriterOptions(_contentRootPath);
        var filePath = _frameWriter.Write(image, opts);
        var fileInfo = new FileInfo(filePath);

        var evt = new FrameSavedEvent(
            FilePath:      filePath,
            CapturedAt:    DateTimeOffset.UtcNow,
            Width:         image.Width,
            Height:        image.Height,
            FileSizeBytes: fileInfo.Exists ? fileInfo.Length : 0,
            Format:        settings.Format.ToString().ToLower());

        await _savedHandler.HandleAsync(evt);
        return filePath;
    }
}
