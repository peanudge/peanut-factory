using System.Threading.Channels;
using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Api.Services;

public sealed class FrameSaveService : IHostedService
{
    private readonly IAcquisitionSession _acquisition;
    private readonly FilenameGenerator _filenameGenerator;
    private readonly FrameSaveTracker _frameSaveTracker;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IWebHostEnvironment _environment;
    private readonly IImageWriter _imageWriter;

    // Bounded queue: decouples the acquisition loop from disk I/O.
    // DropWrite when full so a slow disk never blocks or OOM-s the acquisition pipeline.
    private readonly Channel<(ImageData Frame, AcquisitionConfig Config)> _saveQueue;
    private Task? _saveWorker;

    public FrameSaveService(
        IAcquisitionSession acquisition,
        FilenameGenerator filenameGenerator,
        FrameSaveTracker frameSaveTracker,
        IServiceScopeFactory scopeFactory,
        IWebHostEnvironment environment,
        IImageWriter imageWriter)
    {
        _acquisition = acquisition;
        _filenameGenerator = filenameGenerator;
        _frameSaveTracker = frameSaveTracker;
        _scopeFactory = scopeFactory;
        _environment = environment;
        _imageWriter = imageWriter;
        _saveQueue = Channel.CreateBounded<(ImageData, AcquisitionConfig)>(
            new BoundedChannelOptions(8)
            {
                AllowSynchronousContinuations = false,
                FullMode = BoundedChannelFullMode.DropWrite,
                SingleReader = true,
                SingleWriter = false,
            });
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _acquisition.FrameAcquired += OnFrameAcquired;
        _saveWorker = Task.Run(RunSaveWorkerAsync);
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _acquisition.FrameAcquired -= OnFrameAcquired;
        _saveQueue.Writer.TryComplete();
        if (_saveWorker != null)
        {
            try { await _saveWorker.WaitAsync(cancellationToken); }
            catch (OperationCanceledException) { }
        }
    }

    private void OnFrameAcquired(object? sender, EventArgs e)
    {
        var config = _acquisition.GetStatus().ActiveConfig;
        if (config == null)
            return;

        var frame = _acquisition.GetLatestFrame();
        if (frame == null || !_frameSaveTracker.ShouldSave(frame))
            return;

        _saveQueue.Writer.TryWrite((frame, config));
    }

    private async Task RunSaveWorkerAsync()
    {
        await foreach (var (image, config) in _saveQueue.Reader.ReadAllAsync())
        {
            await SaveAsync(image, config);
        }
    }

    private async Task SaveAsync(ImageData image, AcquisitionConfig config)
    {
        try
        {
            var filePath = _filenameGenerator.Generate(config, _environment.ContentRootPath);
            _imageWriter.Save(image, filePath);

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
            // Save failures must not kill the worker
        }
    }
}
