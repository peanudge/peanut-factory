using PeanutVision.Capture;

namespace PeanutVision.Api.Services;

/// <summary>Single responsibility: DB record + thumbnail after a frame is written to disk.</summary>
public sealed class FrameSavedHandler
{
    private readonly ICapturedImageRepository _imageRepository;
    private readonly IThumbnailService _thumbnailService;
    private readonly IImageSaveSettingsService _saveSettings;

    public FrameSavedHandler(
        ICapturedImageRepository imageRepository,
        IThumbnailService thumbnailService,
        IImageSaveSettingsService saveSettings)
    {
        _imageRepository = imageRepository;
        _thumbnailService = thumbnailService;
        _saveSettings = saveSettings;
    }

    public async Task HandleAsync(FrameSavedEvent e)
    {
        var thumbPath = await _thumbnailService.GenerateAsync(e.FilePath);
        var format    = _saveSettings.GetSettings().Format.ToString().ToLower();

        await _imageRepository.AddAsync(new CapturedImage
        {
            Id            = Guid.NewGuid(),
            FilePath      = e.FilePath,
            ThumbnailPath = thumbPath,
            Width         = e.Width,
            Height        = e.Height,
            FileSizeBytes = e.FileSizeBytes,
            Format        = format,
            CapturedAt    = e.CapturedAt.UtcDateTime,
        });
    }
}
