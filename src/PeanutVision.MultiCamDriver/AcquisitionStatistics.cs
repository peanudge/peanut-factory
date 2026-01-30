using System.Diagnostics;

namespace PeanutVision.MultiCamDriver;

/// <summary>
/// Thread-safe statistics tracking for frame acquisition.
/// Provides FPS, frame counts, and timing metrics.
/// </summary>
public sealed class AcquisitionStatistics
{
    private readonly object _lock = new();
    private readonly Stopwatch _stopwatch = new();

    private long _frameCount;
    private long _droppedFrameCount;
    private long _errorCount;
    private double _minFrameIntervalMs = double.MaxValue;
    private double _maxFrameIntervalMs = double.MinValue;
    private double _totalFrameIntervalMs;
    private long _lastFrameTimestampMs;
    private bool _firstFrame = true;

    /// <summary>Total number of frames acquired since start or reset.</summary>
    public long FrameCount
    {
        get { lock (_lock) return _frameCount; }
    }

    /// <summary>Number of dropped frames (when surface cluster is exhausted).</summary>
    public long DroppedFrameCount
    {
        get { lock (_lock) return _droppedFrameCount; }
    }

    /// <summary>Number of acquisition errors.</summary>
    public long ErrorCount
    {
        get { lock (_lock) return _errorCount; }
    }

    /// <summary>Elapsed time since acquisition started.</summary>
    public TimeSpan ElapsedTime => _stopwatch.Elapsed;

    /// <summary>Average frames per second.</summary>
    public double AverageFps
    {
        get
        {
            lock (_lock)
            {
                var seconds = _stopwatch.Elapsed.TotalSeconds;
                return seconds > 0 ? _frameCount / seconds : 0;
            }
        }
    }

    /// <summary>Minimum frame interval in milliseconds.</summary>
    public double MinFrameIntervalMs
    {
        get
        {
            lock (_lock)
            {
                return _minFrameIntervalMs == double.MaxValue ? 0 : _minFrameIntervalMs;
            }
        }
    }

    /// <summary>Maximum frame interval in milliseconds.</summary>
    public double MaxFrameIntervalMs
    {
        get
        {
            lock (_lock)
            {
                return _maxFrameIntervalMs == double.MinValue ? 0 : _maxFrameIntervalMs;
            }
        }
    }

    /// <summary>Average frame interval in milliseconds.</summary>
    public double AverageFrameIntervalMs
    {
        get
        {
            lock (_lock)
            {
                return _frameCount > 1 ? _totalFrameIntervalMs / (_frameCount - 1) : 0;
            }
        }
    }

    /// <summary>Instantaneous FPS based on last frame interval.</summary>
    public double InstantaneousFps
    {
        get
        {
            lock (_lock)
            {
                if (_frameCount < 2) return 0;
                var lastInterval = _stopwatch.ElapsedMilliseconds - _lastFrameTimestampMs;
                // Use the last recorded interval
                var interval = _maxFrameIntervalMs != double.MinValue ? _maxFrameIntervalMs : 0;
                return interval > 0 ? 1000.0 / interval : 0;
            }
        }
    }

    /// <summary>
    /// Starts or restarts the statistics timer.
    /// </summary>
    public void Start()
    {
        lock (_lock)
        {
            _stopwatch.Start();
        }
    }

    /// <summary>
    /// Stops the statistics timer.
    /// </summary>
    public void Stop()
    {
        lock (_lock)
        {
            _stopwatch.Stop();
        }
    }

    /// <summary>
    /// Records a frame acquisition.
    /// Call this from the FrameAcquired event handler.
    /// </summary>
    public void RecordFrame()
    {
        lock (_lock)
        {
            var currentTimestamp = _stopwatch.ElapsedMilliseconds;

            if (!_firstFrame && _lastFrameTimestampMs > 0)
            {
                var interval = currentTimestamp - _lastFrameTimestampMs;
                _totalFrameIntervalMs += interval;

                if (interval < _minFrameIntervalMs)
                    _minFrameIntervalMs = interval;
                if (interval > _maxFrameIntervalMs)
                    _maxFrameIntervalMs = interval;
            }

            _lastFrameTimestampMs = currentTimestamp;
            _firstFrame = false;
            _frameCount++;
        }
    }

    /// <summary>
    /// Records a dropped frame.
    /// </summary>
    public void RecordDroppedFrame()
    {
        lock (_lock)
        {
            _droppedFrameCount++;
        }
    }

    /// <summary>
    /// Records an acquisition error.
    /// </summary>
    public void RecordError()
    {
        lock (_lock)
        {
            _errorCount++;
        }
    }

    /// <summary>
    /// Resets all statistics to zero.
    /// </summary>
    public void Reset()
    {
        lock (_lock)
        {
            _frameCount = 0;
            _droppedFrameCount = 0;
            _errorCount = 0;
            _minFrameIntervalMs = double.MaxValue;
            _maxFrameIntervalMs = double.MinValue;
            _totalFrameIntervalMs = 0;
            _lastFrameTimestampMs = 0;
            _firstFrame = true;
            _stopwatch.Reset();
        }
    }

    /// <summary>
    /// Creates a snapshot of current statistics.
    /// </summary>
    public AcquisitionStatisticsSnapshot GetSnapshot()
    {
        lock (_lock)
        {
            return new AcquisitionStatisticsSnapshot
            {
                FrameCount = _frameCount,
                DroppedFrameCount = _droppedFrameCount,
                ErrorCount = _errorCount,
                ElapsedTime = _stopwatch.Elapsed,
                AverageFps = _stopwatch.Elapsed.TotalSeconds > 0
                    ? _frameCount / _stopwatch.Elapsed.TotalSeconds
                    : 0,
                MinFrameIntervalMs = _minFrameIntervalMs == double.MaxValue ? 0 : _minFrameIntervalMs,
                MaxFrameIntervalMs = _maxFrameIntervalMs == double.MinValue ? 0 : _maxFrameIntervalMs,
                AverageFrameIntervalMs = _frameCount > 1
                    ? _totalFrameIntervalMs / (_frameCount - 1)
                    : 0
            };
        }
    }

    public override string ToString()
    {
        var snapshot = GetSnapshot();
        return $"Frames: {snapshot.FrameCount}, " +
               $"FPS: {snapshot.AverageFps:F1}, " +
               $"Dropped: {snapshot.DroppedFrameCount}, " +
               $"Errors: {snapshot.ErrorCount}, " +
               $"Interval: {snapshot.MinFrameIntervalMs:F1}-{snapshot.MaxFrameIntervalMs:F1}ms";
    }
}

/// <summary>
/// Immutable snapshot of acquisition statistics at a point in time.
/// </summary>
public readonly struct AcquisitionStatisticsSnapshot
{
    public long FrameCount { get; init; }
    public long DroppedFrameCount { get; init; }
    public long ErrorCount { get; init; }
    public TimeSpan ElapsedTime { get; init; }
    public double AverageFps { get; init; }
    public double MinFrameIntervalMs { get; init; }
    public double MaxFrameIntervalMs { get; init; }
    public double AverageFrameIntervalMs { get; init; }

    public override string ToString()
    {
        return $"Frames: {FrameCount}, " +
               $"FPS: {AverageFps:F1}, " +
               $"Dropped: {DroppedFrameCount}, " +
               $"Errors: {ErrorCount}";
    }
}
