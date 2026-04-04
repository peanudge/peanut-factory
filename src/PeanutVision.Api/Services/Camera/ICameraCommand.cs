using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Api.Services.Camera;

public interface ICameraCommand { }

public sealed record StartCmd(
    ProfileId ProfileId,
    TriggerMode? TriggerMode,
    int? FrameCount,
    int? IntervalMs,
    TaskCompletionSource<bool> Tcs) : ICameraCommand;

public sealed record StopCmd(
    TaskCompletionSource<bool> Tcs) : ICameraCommand;

public sealed record TriggerCmd(
    TaskCompletionSource<ImageData> Tcs,
    CancellationToken Ct) : ICameraCommand;

public sealed record CancelTriggerCmd(
    TaskCompletionSource<ImageData> Tcs) : ICameraCommand;

public sealed record GetLatestFrameCmd(
    TaskCompletionSource<LatestFrameResult> Tcs) : ICameraCommand;

public sealed record FrameArrivedCmd(ImageData Image) : ICameraCommand;

public sealed record AcquisitionErrorCmd(string Message, McSignal Signal) : ICameraCommand;

public sealed record GetStatusCmd(
    TaskCompletionSource<CameraActorStatus> Tcs) : ICameraCommand;

public sealed record GetExposureCmd(
    TaskCompletionSource<ExposureInfo> Tcs) : ICameraCommand;

public sealed record SetExposureCmd(
    double? ExposureUs,
    TaskCompletionSource<ExposureInfo> Tcs) : ICameraCommand;
