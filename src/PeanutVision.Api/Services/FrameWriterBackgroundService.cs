using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PeanutVision.Capture;

namespace PeanutVision.Api.Services;

/// <summary>
/// Drains IFrameQueue -> IFrameWriter -> FrameSavedHandler.
/// Runs for the lifetime of the application.
/// </summary>
public sealed class FrameWriterBackgroundService : BackgroundService
{
    private readonly IFrameQueue _queue;
    private readonly IFrameWriter _frameWriter;
    private readonly IImageSaveSettingsService _saveSettings;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<FrameWriterBackgroundService> _logger;

    public FrameWriterBackgroundService(
        IFrameQueue queue,
        IFrameWriter frameWriter,
        IImageSaveSettingsService saveSettings,
        IServiceScopeFactory scopeFactory,
        IWebHostEnvironment environment,
        ILogger<FrameWriterBackgroundService> logger)
    {
        _queue       = queue;
        _frameWriter = frameWriter;
        _saveSettings = saveSettings;
        _scopeFactory = scopeFactory;
        _environment  = environment;
        _logger       = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await foreach (var frame in _queue.ReadAllAsync(ct))
        {
            var settings = _saveSettings.GetSettings();
            if (!settings.AutoSave) continue;

            try
            {
                var opts     = settings.ToWriterOptions(_environment.ContentRootPath);
                var filePath = _frameWriter.Write(frame, opts);
                var fileInfo = new FileInfo(filePath);

                var evt = new FrameSavedEvent(
                    FilePath:    filePath,
                    CapturedAt:  DateTimeOffset.UtcNow,
                    Width:       frame.Width,
                    Height:      frame.Height,
                    FileSizeBytes: fileInfo.Exists ? fileInfo.Length : 0);

                // FrameSavedHandler uses scoped DbContext — create a scope per frame
                await using var scope   = _scopeFactory.CreateAsyncScope();
                var handler = scope.ServiceProvider.GetRequiredService<FrameSavedHandler>();
                await handler.HandleAsync(evt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save or record frame");
            }
        }
    }
}
