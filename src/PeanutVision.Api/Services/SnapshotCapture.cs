using PeanutVision.Capture;
using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Camera;
using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Api.Services;

/// <summary>Orchestrates a single-frame snapshot: channel setup, trigger, save, and DB record.</summary>
public sealed class SnapshotCapture : ISnapshotCapture
{
    private readonly IGrabService _grabService;
    private readonly ICamFileService _camFileService;
    private readonly IFrameWriter _frameWriter;
    private readonly IImageSaveSettingsService _saveSettings;
    private readonly ILatencyService _latencyService;
    private readonly ICapturedImageRepository _imageRepository;
    private readonly IThumbnailService _thumbnailService;
    private readonly string _contentRootPath;

    public SnapshotCapture(
        IGrabService grabService,
        ICamFileService camFileService,
        IFrameWriter frameWriter,
        IImageSaveSettingsService saveSettings,
        ILatencyService latencyService,
        ICapturedImageRepository imageRepository,
        IThumbnailService thumbnailService,
        IWebHostEnvironment environment)
    {
        _grabService = grabService;
        _camFileService = camFileService;
        _frameWriter = frameWriter;
        _saveSettings = saveSettings;
        _latencyService = latencyService;
        _imageRepository = imageRepository;
        _thumbnailService = thumbnailService;
        _contentRootPath = environment.ContentRootPath;
    }

    public async Task<string> CaptureAsync(ProfileId profileId, TriggerMode? triggerMode = null)
    {
        var camFile = _camFileService.GetByFileName(profileId.Value);
        var options = triggerMode.HasValue
            ? camFile.ToChannelOptions(triggerMode.Value.Mode, useCallback: false)
            : camFile.ToChannelOptions(useCallback: false);

        // Snapshot requires a software-triggerable mode
        if (options.TriggerMode != McTrigMode.MC_TrigMode_SOFT &&
            options.TriggerMode != McTrigMode.MC_TrigMode_COMBINED)
        {
            options.TriggerMode = McTrigMode.MC_TrigMode_SOFT;
        }

        var channel = _grabService.CreateChannel(options);
        try
        {
            channel.StartAcquisition(1);
            var triggerAt = DateTimeOffset.UtcNow;
            channel.SendSoftwareTrigger();

            var surface = channel.WaitForFrame(5000)
                ?? throw new TimeoutException("Snapshot timed out waiting for frame.");

            var frameAt = DateTimeOffset.UtcNow;
            _latencyService.Record(triggerAt, frameAt, 1, profileId.Value);

            ImageData image;
            try { image = ImageData.FromSurface(surface); }
            finally { channel.ReleaseSurface(surface); }

            var settings = _saveSettings.GetSettings();
            var writerOptions = settings.ToWriterOptions(_contentRootPath);
            var filePath = _frameWriter.Write(image, writerOptions);

            var thumbnailPath = await _thumbnailService.GenerateAsync(filePath);
            var fileInfo = new FileInfo(filePath);

            await _imageRepository.AddAsync(new CapturedImage
            {
                Id = Guid.NewGuid(),
                FilePath = filePath,
                ThumbnailPath = thumbnailPath,
                Width = image.Width,
                Height = image.Height,
                FileSizeBytes = fileInfo.Exists ? fileInfo.Length : 0,
                Format = "png",
                CapturedAt = DateTime.UtcNow,
            });

            return filePath;
        }
        finally
        {
            channel.StopAcquisition();
            _grabService.ReleaseChannel(channel);
        }
    }
}
