namespace PeanutVision.Api.Services;

/// <summary>
/// Thread-safe gate that coordinates mutual exclusion between
/// <see cref="AcquisitionService"/> and <see cref="CaptureOnceService"/>.
/// CaptureOnceService acquires the gate while a single-shot capture is in progress;
/// AcquisitionService checks <see cref="IsInProgress"/> before allowing Start/CreateChannel.
/// </summary>
public sealed class AcquisitionOperationGate
{
    private int _inProgress; // 0 = idle, 1 = in-progress

    public bool TryEnter() => Interlocked.CompareExchange(ref _inProgress, 1, 0) == 0;
    public void Exit()     => Interlocked.Exchange(ref _inProgress, 0);
    public bool IsInProgress => Volatile.Read(ref _inProgress) == 1;
}
