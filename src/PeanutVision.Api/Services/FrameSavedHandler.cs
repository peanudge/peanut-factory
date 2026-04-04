using PeanutVision.Capture;

namespace PeanutVision.Api.Services;

/// <summary>Single responsibility: DB record + thumbnail after a frame is written to disk.</summary>
public sealed class FrameSavedHandler
{
    private readonly ICapturedImageRepository _imageRepository;
    private readonly IThumbnailService _thumbnailService;

    public FrameSavedHandler(
        ICapturedImageRepository imageRepository,
        IThumbnailService thumbnailService)
    {
        _imageRepository = imageRepository;
        _thumbnailService = thumbnailService;
    }

    public async Task HandleAsync(FrameSavedEvent e)
    {
        var thumbPath = await _thumbnailService.GenerateAsync(e.FilePath);

        await _imageRepository.AddAsync(new CapturedImage
        {
            Id            = Guid.NewGuid(),
            FilePath      = e.FilePath,
            ThumbnailPath = thumbPath,
            Width         = e.Width,
            Height        = e.Height,
            FileSizeBytes = e.FileSizeBytes,
            Format        = e.Format,
            CapturedAt    = e.CapturedAt.UtcDateTime,
        });
    }
}
