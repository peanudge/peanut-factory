using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Camera;
using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Api.Services;

/// <summary>
/// Performs a single-shot synchronous image capture using an independent temporary channel.
/// Does not interact with the persistent channel managed by <see cref="AcquisitionService"/>.
/// </summary>
public sealed class CaptureOnceService : ICaptureOnceService
{
    private readonly IAcquisitionChannelManager _channelManager;
    private readonly ICamFileService _camFileService;
    private readonly ILatencyService _latencyService;
    private readonly AcquisitionOperationGate _gate;
    private readonly IAcquisitionService _acquisition;

    public CaptureOnceService(
        IAcquisitionChannelManager channelManager,
        ICamFileService camFileService,
        ILatencyService latencyService,
        AcquisitionOperationGate gate,
        IAcquisitionService acquisition)
    {
        _channelManager = channelManager;
        _camFileService = camFileService;
        _latencyService = latencyService;
        _gate = gate;
        _acquisition = acquisition;
    }

    public ImageData CaptureOnce(ProfileId profileId, TriggerMode? triggerMode = null)
    {
        if (_acquisition.IsActive)
            throw new InvalidOperationException("Acquisition is already active. Stop it first.");

        if (!_gate.TryEnter())
            throw new InvalidOperationException("Another capture operation is already in progress. Wait for it to complete.");

        try
        {
            var camFile = _camFileService.GetByFileName(profileId.Value);
            var options = triggerMode.HasValue
                ? camFile.ToChannelOptions(triggerMode.Value.Mode, useCallback: false)
                : camFile.ToChannelOptions(useCallback: false);

            // CaptureOnce requires a software-triggerable mode.
            // Throw explicitly rather than silently overriding the caller's intent.
            if (triggerMode.HasValue &&
                options.TriggerMode != McTrigMode.MC_TrigMode_SOFT &&
                options.TriggerMode != McTrigMode.MC_TrigMode_COMBINED)
            {
                throw new ArgumentException(
                    $"CaptureOnce requires SOFT or COMBINED trigger mode. " +
                    $"The requested mode '{options.TriggerMode}' is not software-triggerable. " +
                    "Use a cam file or trigger mode that supports software triggering.",
                    nameof(triggerMode));
            }

            // If no explicit trigger mode was requested, force SOFT for reliable single-shot capture.
            if (!triggerMode.HasValue)
            {
                options.TriggerMode = McTrigMode.MC_TrigMode_SOFT;
            }

            var channel = _channelManager.CreateChannel(options);
            try
            {
                channel.StartAcquisition(1);
                var triggerAt = DateTimeOffset.UtcNow;
                channel.SendSoftwareTrigger();

                var surface = channel.WaitForFrame(5000)
                    ?? throw new TimeoutException("CaptureOnce timed out waiting for frame.");

                var frameAt = DateTimeOffset.UtcNow;
                _latencyService.Record(triggerAt, frameAt, 1, profileId.Value);

                try
                {
                    return ImageData.FromSurface(surface);
                }
                finally
                {
                    channel.ReleaseSurface(surface);
                }
            }
            finally
            {
                channel.StopAcquisition();
                _channelManager.ReleaseChannel(channel);
            }
        }
        finally
        {
            _gate.Exit();
        }
    }
}
