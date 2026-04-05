using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Api.Services;

public sealed class ImageCaptureService : IImageCaptureService
{
    private readonly IImageSaveSettingsService _saveSettings;
    private readonly FilenameGenerator _filenameGenerator;
    private readonly FrameSaveTracker _frameSaveTracker;
    private readonly ICapturedImageRepository _imageRepository;
    private readonly IThumbnailService _thumbnailService;
    private readonly ICaptureStatService _captureStatService;
    private readonly string _contentRootPath;

    public ImageCaptureService(
        IImageSaveSettingsService saveSettings,
        FilenameGenerator filenameGenerator,
        FrameSaveTracker frameSaveTracker,
        ICapturedImageRepository imageRepository,
        IThumbnailService thumbnailService,
        ICaptureStatService captureStatService,
        IWebHostEnvironment environment)
    {
        _saveSettings = saveSettings;
        _filenameGenerator = filenameGenerator;
        _frameSaveTracker = frameSaveTracker;
        _imageRepository = imageRepository;
        _thumbnailService = thumbnailService;
        _captureStatService = captureStatService;
        _contentRootPath = environment.ContentRootPath;
    }

    public async Task<string> SaveAndRecordAsync(ImageData image, string? profileId)
    {
        var settings = _saveSettings.GetSettings();
        var filePath = _filenameGenerator.Generate(settings, _contentRootPath, profileId);
        new ImageWriter().Save(image, filePath);

        var thumbPath = await _thumbnailService.GenerateAsync(filePath);
        var fileInfo = new FileInfo(filePath);
        var capturedAt = DateTime.UtcNow;

        await _imageRepository.AddAsync(new CapturedImage
        {
            Id = Guid.NewGuid(),
            FilePath = filePath,
            ThumbnailPath = thumbPath,
            Width = image.Width,
            Height = image.Height,
            FileSizeBytes = fileInfo.Exists ? fileInfo.Length : 0,
            Format = settings.Format.ToString().ToLower(),
            CapturedAt = capturedAt,
        });

        await _captureStatService.RecordCaptureAsync(capturedAt);

        return filePath;
    }
}
