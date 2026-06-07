using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Api.Services;

public sealed class FrameSaveService : IHostedService
{
    private readonly IAcquisitionSession _acquisition;
    private readonly FilenameGenerator _filenameGenerator;
    private readonly FrameSaveTracker _frameSaveTracker;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IWebHostEnvironment _environment;

    public FrameSaveService(
        IAcquisitionSession acquisition,
        FilenameGenerator filenameGenerator,
        FrameSaveTracker frameSaveTracker,
        IServiceScopeFactory scopeFactory,
        IWebHostEnvironment environment)
    {
        _acquisition = acquisition;
        _filenameGenerator = filenameGenerator;
        _frameSaveTracker = frameSaveTracker;
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
        var config = _acquisition.GetStatus().ActiveConfig;
        if (config == null)
            return;

        var frame = _acquisition.GetLatestFrame();
        if (frame == null || !_frameSaveTracker.ShouldSave(frame))
            return;

        _ = SaveAsync(frame, config);
    }

    private async Task SaveAsync(ImageData image, AcquisitionConfig config)
    {
        try
        {
            var filePath = _filenameGenerator.Generate(config, _environment.ContentRootPath);
            new ImageWriter().Save(image, filePath);

            var fileInfo = new FileInfo(filePath);

            using var scope = _scopeFactory.CreateScope();
            var imageRepo = scope.ServiceProvider.GetRequiredService<ICapturedImageRepository>();

            await imageRepo.AddAsync(new CapturedImage
            {
                Id = Guid.NewGuid(),
                FilePath = filePath,
                Width = image.Width,
                Height = image.Height,
                FileSizeBytes = fileInfo.Exists ? fileInfo.Length : 0,
                Format = config.Format.ToString().ToLower(),
                CapturedAt = DateTime.UtcNow,
            });
        }
        catch
        {
            // Save failures must not propagate to the acquisition pipeline
        }
    }
}
