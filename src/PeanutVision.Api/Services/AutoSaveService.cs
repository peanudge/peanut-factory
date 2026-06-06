using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Api.Services;

public sealed class AutoSaveService : IHostedService
{
    private readonly IAcquisitionSession _acquisition;
    private readonly IImageSaveSettingsService _saveSettings;
    private readonly FilenameGenerator _filenameGenerator;
    private readonly FrameSaveTracker _frameSaveTracker;
    private readonly IThumbnailService _thumbnailService;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IWebHostEnvironment _environment;

    public AutoSaveService(
        IAcquisitionSession acquisition,
        IImageSaveSettingsService saveSettings,
        FilenameGenerator filenameGenerator,
        FrameSaveTracker frameSaveTracker,
        IThumbnailService thumbnailService,
        IServiceScopeFactory scopeFactory,
        IWebHostEnvironment environment)
    {
        _acquisition = acquisition;
        _saveSettings = saveSettings;
        _filenameGenerator = filenameGenerator;
        _frameSaveTracker = frameSaveTracker;
        _thumbnailService = thumbnailService;
        _scopeFactory = scopeFactory;
        _environment = environment;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _acquisition.FrameAcquired += OnFrameAcquired;
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _acquisition.FrameAcquired -= OnFrameAcquired;
        return Task.CompletedTask;
    }

    private void OnFrameAcquired(object? sender, EventArgs e)
    {
        var settings = _saveSettings.GetSettings();
        if (!settings.AutoSave)
            return;

        var frame = _acquisition.GetLatestFrame();
        if (frame == null || !_frameSaveTracker.ShouldSave(frame))
            return;

        var profileId = _acquisition.GetStatus().ActiveConfig?.ProfileId.Value;
        _ = SaveAsync(frame, settings, profileId);
    }

    private async Task SaveAsync(ImageData image, ImageSaveSettings settings, string? profileId)
    {
        try
        {
            var filePath = _filenameGenerator.Generate(settings, _environment.ContentRootPath, profileId);
            new ImageWriter().Save(image, filePath);

            var thumbPath = await _thumbnailService.GenerateAsync(filePath);
            var fileInfo = new FileInfo(filePath);

            using var scope = _scopeFactory.CreateScope();
            var imageRepo = scope.ServiceProvider.GetRequiredService<ICapturedImageRepository>();

            await imageRepo.AddAsync(new CapturedImage
            {
                Id = Guid.NewGuid(),
                FilePath = filePath,
                ThumbnailPath = thumbPath,
                Width = image.Width,
                Height = image.Height,
                FileSizeBytes = fileInfo.Exists ? fileInfo.Length : 0,
                Format = settings.Format.ToString().ToLower(),
                CapturedAt = DateTime.UtcNow,
            });
        }
        catch
        {
            // Save failures must not propagate to the acquisition pipeline
        }
    }
}
