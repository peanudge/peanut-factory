namespace PeanutVision.Capture;

/// <summary>Fires Tick at a fixed interval. Single responsibility: timing only.</summary>
public sealed class CaptureScheduler : IDisposable
{
    private readonly object _lock = new();
    private Timer? _timer;
    private volatile bool _disposed;

    public event Action? Tick;

    public void Start(TimeSpan interval)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        lock (_lock)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            _timer?.Dispose();
            _timer = new Timer(_ => Tick?.Invoke(), null, TimeSpan.Zero, interval);
        }
    }

    public void Stop()
    {
        lock (_lock)
        {
            _timer?.Dispose();
            _timer = null;
        }
    }

    public void Dispose()
    {
        _disposed = true;
        Stop();
    }
}
