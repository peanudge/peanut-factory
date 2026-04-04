using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PeanutVision.Api.Exceptions;
using PeanutVision.Capture;
using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Camera;
using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Api.Services.Camera;

/// <summary>
/// Encapsulates all state and behavior for a single camera.
/// External access is serialized through a System.Threading.Channels mailbox,
/// eliminating the need for locks.
/// </summary>
public sealed class CameraActor : ICameraActor
{
    // --- identity & external services ---
    private readonly string _cameraId;
    private readonly IGrabService _grabService;
    private readonly ICamFileService _camFileService;
    private readonly ILatencyService _latencyService;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IFrameWriter _frameWriter;
    private readonly IImageSaveSettingsService _saveSettings;
    private readonly string _contentRootPath;
    private readonly ILogger<CameraActor> _logger;

    // --- mailbox & loop ---
    private readonly Channel<ICameraCommand> _mailbox =
        Channel.CreateUnbounded<ICameraCommand>(new UnboundedChannelOptions { SingleReader = true });
    private readonly Task _loopTask;
    private readonly CancellationTokenSource _cts = new();

    // --- state (only accessed inside RunLoopAsync) ---
    private GrabChannel? _channel;
    private ImageData? _lastFrame;
    private ImageData? _lastSavedFrame;
    private string? _lastError;
    private AcquisitionStatistics? _statistics;
    private TaskCompletionSource<ImageData>? _pendingTriggerTcs;
    private DateTimeOffset? _triggerTimestamp;
    private ChannelState _state = ChannelState.None;
    private ProfileId? _activeProfileId;
    private readonly ChannelEventLog _eventLog = new();
    private double _desiredExposureUs = 10_000.0;
    private readonly SemaphoreSlim _saveConcurrency = new(initialCount: 8, maxCount: 8);

    public string CameraId => _cameraId;

    public CameraActor(
        string cameraId,
        IGrabService grabService,
        ICamFileService camFileService,
        ILatencyService latencyService,
        IServiceScopeFactory scopeFactory,
        IFrameWriter frameWriter,
        IImageSaveSettingsService saveSettings,
        string contentRootPath,
        ILogger<CameraActor> logger)
    {
        _cameraId        = cameraId;
        _grabService     = grabService;
        _camFileService  = camFileService;
        _latencyService  = latencyService;
        _scopeFactory    = scopeFactory;
        _frameWriter     = frameWriter;
        _saveSettings    = saveSettings;
        _contentRootPath = contentRootPath;
        _logger          = logger;
        _loopTask        = Task.Run(() => RunLoopAsync(_cts.Token));
    }

    // ---- public API: send commands to mailbox ----

    public Task StartAsync(ProfileId profileId, TriggerMode? triggerMode = null,
                           int? frameCount = null, int? intervalMs = null,
                           CancellationToken ct = default)
    {
        var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        _mailbox.Writer.TryWrite(new StartCmd(profileId, triggerMode, frameCount, intervalMs, tcs));
        return tcs.Task;
    }

    public Task StopAsync(CancellationToken ct = default)
    {
        var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        _mailbox.Writer.TryWrite(new StopCmd(tcs));
        return tcs.Task;
    }

    public async Task<ImageData> TriggerAsync(int timeoutMs = 5000, CancellationToken ct = default)
    {
        var tcs = new TaskCompletionSource<ImageData>(TaskCreationOptions.RunContinuationsAsynchronously);
        _mailbox.Writer.TryWrite(new TriggerCmd(tcs, ct));

        var timeout = Task.Delay(timeoutMs, ct);
        var completed = await Task.WhenAny(tcs.Task, timeout).ConfigureAwait(false);
        if (completed == timeout)
        {
            _mailbox.Writer.TryWrite(new CancelTriggerCmd(tcs));
            ct.ThrowIfCancellationRequested();
            throw new TimeoutException("Trigger timed out waiting for frame.");
        }
        return await tcs.Task.ConfigureAwait(false);
    }

    public Task<LatestFrameResult> GetLatestFrameAsync(CancellationToken ct = default)
    {
        var tcs = new TaskCompletionSource<LatestFrameResult>(TaskCreationOptions.RunContinuationsAsynchronously);
        _mailbox.Writer.TryWrite(new GetLatestFrameCmd(tcs));
        return tcs.Task;
    }

    public Task<CameraActorStatus> GetStatusAsync(CancellationToken ct = default)
    {
        var tcs = new TaskCompletionSource<CameraActorStatus>(TaskCreationOptions.RunContinuationsAsynchronously);
        _mailbox.Writer.TryWrite(new GetStatusCmd(tcs));
        return tcs.Task;
    }

    public Task<ExposureInfo> GetExposureAsync(CancellationToken ct = default)
    {
        var tcs = new TaskCompletionSource<ExposureInfo>(TaskCreationOptions.RunContinuationsAsynchronously);
        _mailbox.Writer.TryWrite(new GetExposureCmd(tcs));
        return tcs.Task;
    }

    public Task<ExposureInfo> SetExposureAsync(double? exposureUs, CancellationToken ct = default)
    {
        var tcs = new TaskCompletionSource<ExposureInfo>(TaskCreationOptions.RunContinuationsAsynchronously);
        _mailbox.Writer.TryWrite(new SetExposureCmd(exposureUs, tcs));
        return tcs.Task;
    }

    // ---- actor loop ----

    private async Task RunLoopAsync(CancellationToken ct)
    {
        try
        {
            await foreach (var cmd in _mailbox.Reader.ReadAllAsync(ct).ConfigureAwait(false))
            {
                try
                {
                    switch (cmd)
                    {
                        case StartCmd c:            HandleStart(c);            break;
                        case StopCmd c:             HandleStop(c);             break;
                        case TriggerCmd c:          HandleTrigger(c);          break;
                        case CancelTriggerCmd c:    HandleCancelTrigger(c);    break;
                        case GetLatestFrameCmd c:   HandleGetLatestFrame(c);   break;
                        case FrameArrivedCmd c:     HandleFrameArrived(c);     break;
                        case AcquisitionErrorCmd c: HandleAcquisitionError(c); break;
                        case GetStatusCmd c:        c.Tcs.TrySetResult(BuildStatus()); break;
                        case GetExposureCmd c:      c.Tcs.TrySetResult(BuildExposureInfo()); break;
                        case SetExposureCmd c:      HandleSetExposure(c);      break;
                        case AcquisitionEndedCmd:   HandleStop(new StopCmd(new TaskCompletionSource<bool>())); break;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unhandled exception in CameraActor loop for {CameraId}", _cameraId);
                    PropagateExceptionToCommand(cmd, ex);
                }
            }
        }
        catch (OperationCanceledException) { /* expected on shutdown */ }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CameraActor loop crashed for {CameraId}", _cameraId);
        }
    }

    private static void PropagateExceptionToCommand(ICameraCommand cmd, Exception ex)
    {
        switch (cmd)
        {
            case StartCmd c:          c.Tcs.TrySetException(ex); break;
            case StopCmd c:           c.Tcs.TrySetException(ex); break;
            case TriggerCmd c:        c.Tcs.TrySetException(ex); break;
            case GetStatusCmd c:      c.Tcs.TrySetException(ex); break;
            case GetExposureCmd c:    c.Tcs.TrySetException(ex); break;
            case SetExposureCmd c:    c.Tcs.TrySetException(ex); break;
            case GetLatestFrameCmd c: c.Tcs.TrySetException(ex); break;
        }
    }

    // ---- command handlers ----

    private void HandleStart(StartCmd command)
    {
        const int minimumIntervalMs = 50;
        if (command.IntervalMs.HasValue && command.IntervalMs.Value > 0 && command.IntervalMs.Value < minimumIntervalMs)
        {
            command.Tcs.TrySetException(new InvalidParameterException(
                $"intervalMs must be >= {minimumIntervalMs}ms, got {command.IntervalMs.Value}ms."));
            return;
        }

        if (_state == ChannelState.Active)
        {
            command.Tcs.TrySetException(new AcquisitionConflictException(
                "Acquisition is already active. Stop it first."));
            return;
        }

        ReleaseCurrentChannel();

        var camFile = _camFileService.GetByFileName(command.ProfileId.Value);
        var options = command.TriggerMode.HasValue
            ? camFile.ToChannelOptions(command.TriggerMode.Value.Mode)
            : camFile.ToChannelOptions();

        _channel = _grabService.CreateChannel(options);
        _activeProfileId = command.ProfileId;
        _lastFrame = null;
        _lastError = null;
        _statistics = new AcquisitionStatistics();

        _channel.FrameAcquired    += OnFrameAcquired;
        _channel.AcquisitionError += OnAcquisitionError;
        _channel.AcquisitionEnded += OnAcquisitionEnded;

        _statistics.Start();
        _channel.StartAcquisition(command.FrameCount ?? -1);
        _state = ChannelState.Active;

        _eventLog.Add(new ChannelEvent(DateTime.UtcNow, ChannelEventType.AcquisitionStarted,
            $"Started with profile '{command.ProfileId.Value}'" +
            (command.FrameCount.HasValue ? $", frameCount={command.FrameCount}" : "") +
            (command.IntervalMs.HasValue ? $", intervalMs={command.IntervalMs}" : "")));

        command.Tcs.TrySetResult(true);
    }

    private void HandleStop(StopCmd command)
    {
        if (_state != ChannelState.Active)
        {
            command.Tcs.TrySetResult(true);
            return;
        }
        _state = ChannelState.Idle;

        var pendingTcs = _pendingTriggerTcs;
        _pendingTriggerTcs = null;
        pendingTcs?.TrySetCanceled();

        _statistics?.Stop();
        _channel!.StopAcquisition();
        _channel.FrameAcquired    -= OnFrameAcquired;
        _channel.AcquisitionError -= OnAcquisitionError;
        _channel.AcquisitionEnded -= OnAcquisitionEnded;
        _grabService.ReleaseChannel(_channel);
        _channel = null;
        _activeProfileId = null;
        _state = ChannelState.None;

        _eventLog.Add(new ChannelEvent(DateTime.UtcNow, ChannelEventType.AcquisitionStopped, "Stopped"));
        command.Tcs.TrySetResult(true);
    }

    private void HandleTrigger(TriggerCmd command)
    {
        if (_channel == null || !_channel.IsActive)
        {
            command.Tcs.TrySetException(new ChannelNotAvailableException());
            return;
        }
        if (_pendingTriggerTcs != null)
        {
            command.Tcs.TrySetException(new AcquisitionConflictException(
                "A trigger is already pending. Wait for it to complete."));
            return;
        }
        if (!_channel.SupportsSoftwareTrigger)
        {
            command.Tcs.TrySetException(new InvalidParameterException(
                $"TriggerAsync requires SOFT or COMBINED trigger mode, but channel is {_channel.TriggerMode}."));
            return;
        }

        _pendingTriggerTcs = command.Tcs;
        _triggerTimestamp  = DateTimeOffset.UtcNow;
        _channel.SendSoftwareTrigger();
    }

    private void HandleCancelTrigger(CancelTriggerCmd command)
    {
        if (ReferenceEquals(_pendingTriggerTcs, command.Tcs))
            _pendingTriggerTcs = null;
    }

    private void HandleGetLatestFrame(GetLatestFrameCmd command)
    {
        var isNew = _lastFrame is not null && !ReferenceEquals(_lastFrame, _lastSavedFrame);
        if (isNew) _lastSavedFrame = _lastFrame;
        command.Tcs.TrySetResult(new LatestFrameResult(_lastFrame, isNew));
    }

    private void HandleFrameArrived(FrameArrivedCmd command)
    {
        var frameAt = DateTimeOffset.UtcNow;
        _lastFrame = command.Image;
        _statistics?.RecordFrame();
        var frameIndex = _statistics?.FrameCount ?? 0;

        if (_triggerTimestamp.HasValue)
        {
            _latencyService.Record(_triggerTimestamp.Value, frameAt, frameIndex, _activeProfileId?.Value);
            _triggerTimestamp = null;
        }

        if (_pendingTriggerTcs is not null)
        {
            var tcs = _pendingTriggerTcs;
            _pendingTriggerTcs = null;
            tcs.TrySetResult(command.Image);
            // Triggered frames are saved by the controller — do NOT save here
            return;
        }

        // Stream frame — save asynchronously (fire-and-forget)
        _ = SaveStreamFrameAsync(command.Image);
    }

    private void HandleAcquisitionError(AcquisitionErrorCmd command)
    {
        _lastError = command.Message;
        _statistics?.RecordError();
        if (command.Signal == McSignal.MC_SIG_CLUSTER_UNAVAILABLE) _statistics?.RecordDroppedFrame();

        var pendingTcs = _pendingTriggerTcs;
        _pendingTriggerTcs = null;
        pendingTcs?.TrySetException(new InvalidOperationException($"Acquisition error: {command.Message}"));

        var eventType = command.Signal == McSignal.MC_SIG_CLUSTER_UNAVAILABLE
            ? ChannelEventType.BufferUnavailable
            : ChannelEventType.AcquisitionError;
        _eventLog.Add(new ChannelEvent(DateTime.UtcNow, eventType, command.Message));

        if (command.Signal == McSignal.MC_SIG_UNRECOVERABLE_OVERRUN)
            HandleStop(new StopCmd(new TaskCompletionSource<bool>()));
    }

    private void HandleSetExposure(SetExposureCmd command)
    {
        if (command.ExposureUs.HasValue)
            _desiredExposureUs = command.ExposureUs.Value;

        if (_channel?.IsActive == true)
        {
            _channel.SetExposureUs(_desiredExposureUs);
            var (min, max) = _channel.GetExposureRange();
            command.Tcs.TrySetResult(new ExposureInfo
            {
                ExposureUs    = _channel.GetExposureUs(),
                ExposureRange = new ExposureRangeInfo { Min = min, Max = max },
            });
        }
        else
        {
            command.Tcs.TrySetResult(new ExposureInfo { ExposureUs = _desiredExposureUs });
        }
    }

    // ---- hardware callbacks (called from driver thread — write to mailbox only) ----

    private void OnFrameAcquired(object? sender, FrameAcquiredEventArgs e)
        => _mailbox.Writer.TryWrite(new FrameArrivedCmd(e.Image));

    private void OnAcquisitionError(object? sender, AcquisitionErrorEventArgs e)
        => _mailbox.Writer.TryWrite(new AcquisitionErrorCmd(e.Message, e.Signal));

    private void OnAcquisitionEnded(object? sender, EventArgs e)
        => _mailbox.Writer.TryWrite(new AcquisitionEndedCmd());

    // ---- helpers ----

    private void ReleaseCurrentChannel()
    {
        if (_channel != null)
        {
            _grabService.ReleaseChannel(_channel);
            _channel = null;
            _state = ChannelState.None;
        }
    }

    private ExposureInfo BuildExposureInfo()
    {
        if (_channel?.IsActive == true)
        {
            _desiredExposureUs = _channel.GetExposureUs();
            var (min, max) = _channel.GetExposureRange();
            return new ExposureInfo
            {
                ExposureUs    = _desiredExposureUs,
                ExposureRange = new ExposureRangeInfo { Min = min, Max = max },
            };
        }
        return new ExposureInfo { ExposureUs = _desiredExposureUs };
    }

    private CameraActorStatus BuildStatus()
    {
        var snapshot = _statistics?.GetSnapshot();
        AcquisitionStatisticsSnapshot? statisticsWithDriver = null;
        if (snapshot.HasValue && _channel != null)
            statisticsWithDriver = snapshot.Value with
            {
                CopyDropCount           = _channel.CopyDropCount,
                ClusterUnavailableCount = _channel.ClusterUnavailableCount,
            };
        else if (snapshot.HasValue)
            statisticsWithDriver = snapshot;

        var allowedActions = _state switch
        {
            ChannelState.None   => (IReadOnlySet<ChannelAction>)new HashSet<ChannelAction> { ChannelAction.Start, ChannelAction.Snapshot },
            ChannelState.Idle   => new HashSet<ChannelAction> { ChannelAction.Start, ChannelAction.Snapshot },
            ChannelState.Active => new HashSet<ChannelAction> { ChannelAction.Stop, ChannelAction.Trigger },
            _                   => new HashSet<ChannelAction>(),
        };

        return new CameraActorStatus(
            IsActive:        _state == ChannelState.Active,
            HasFrame:        _lastFrame is not null,
            LastError:       _lastError,
            ChannelState:    _state,
            ActiveProfileId: _activeProfileId?.Value,
            AllowedActions:  allowedActions,
            Statistics:      statisticsWithDriver,
            RecentEvents:    _eventLog.GetRecent(50));
    }

    private async Task SaveStreamFrameAsync(ImageData image)
    {
        var settings = _saveSettings.GetSettings();
        if (!settings.AutoSave) return;

        if (!await _saveConcurrency.WaitAsync(0).ConfigureAwait(false))
        {
            _logger.LogWarning("Stream frame dropped for {CameraId}: save concurrency limit reached", _cameraId);
            _statistics?.RecordDroppedFrame();
            return;
        }

        try
        {
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

            await using var scope   = _scopeFactory.CreateAsyncScope();
            var handler = scope.ServiceProvider.GetRequiredService<FrameSavedHandler>();
            await handler.HandleAsync(evt).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save stream frame for camera {CameraId}", _cameraId);
        }
        finally
        {
            _saveConcurrency.Release();
        }
    }

    // ---- dispose ----

    public async ValueTask DisposeAsync()
    {
        // Always send StopCmd — HandleStop is a no-op if not active.
        // Avoids reading _state from outside the actor loop (data race).
        await StopAsync().ConfigureAwait(false);

        _mailbox.Writer.TryComplete();
        _cts.Cancel();
        try { await _loopTask.ConfigureAwait(false); } catch { /* loop already exited */ }
        _cts.Dispose();
        _saveConcurrency.Dispose();
    }
}
