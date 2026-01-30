namespace PeanutVision.MultiCamDriver.Tests;

public class AcquisitionStatisticsTests
{
    [Fact]
    public void NewInstance_HasZeroCounts()
    {
        var stats = new AcquisitionStatistics();

        Assert.Equal(0, stats.FrameCount);
        Assert.Equal(0, stats.DroppedFrameCount);
        Assert.Equal(0, stats.ErrorCount);
        Assert.Equal(0, stats.AverageFps);
    }

    [Fact]
    public void RecordFrame_IncrementsFrameCount()
    {
        var stats = new AcquisitionStatistics();
        stats.Start();

        stats.RecordFrame();
        stats.RecordFrame();
        stats.RecordFrame();

        Assert.Equal(3, stats.FrameCount);
    }

    [Fact]
    public void RecordDroppedFrame_IncrementsDroppedCount()
    {
        var stats = new AcquisitionStatistics();

        stats.RecordDroppedFrame();
        stats.RecordDroppedFrame();

        Assert.Equal(2, stats.DroppedFrameCount);
    }

    [Fact]
    public void RecordError_IncrementsErrorCount()
    {
        var stats = new AcquisitionStatistics();

        stats.RecordError();

        Assert.Equal(1, stats.ErrorCount);
    }

    [Fact]
    public void Reset_ClearsAllCounters()
    {
        var stats = new AcquisitionStatistics();
        stats.Start();
        stats.RecordFrame();
        stats.RecordFrame();
        stats.RecordDroppedFrame();
        stats.RecordError();
        stats.Stop();

        stats.Reset();

        Assert.Equal(0, stats.FrameCount);
        Assert.Equal(0, stats.DroppedFrameCount);
        Assert.Equal(0, stats.ErrorCount);
        Assert.Equal(TimeSpan.Zero, stats.ElapsedTime);
    }

    [Fact]
    public void AverageFps_CalculatesCorrectly()
    {
        var stats = new AcquisitionStatistics();
        stats.Start();

        // Record 10 frames
        for (int i = 0; i < 10; i++)
        {
            stats.RecordFrame();
            Thread.Sleep(10); // ~100 FPS
        }

        stats.Stop();

        // Should be approximately 100 FPS (10 frames in ~100ms)
        Assert.True(stats.AverageFps > 50, $"Expected FPS > 50, got {stats.AverageFps}");
        Assert.True(stats.AverageFps < 200, $"Expected FPS < 200, got {stats.AverageFps}");
    }

    [Fact]
    public void FrameIntervals_AfterMultipleFrames_TracksNonZeroValues()
    {
        var stats = new AcquisitionStatistics();
        stats.Start();

        // Record frames with some delay
        stats.RecordFrame();
        Thread.Sleep(50);
        stats.RecordFrame();
        Thread.Sleep(50);
        stats.RecordFrame();

        stats.Stop();

        // After 3 frames, we should have 2 intervals recorded
        // All interval metrics should be non-zero
        Assert.True(stats.MinFrameIntervalMs > 0, $"Min interval should be > 0, got {stats.MinFrameIntervalMs}");
        Assert.True(stats.MaxFrameIntervalMs > 0, $"Max interval should be > 0, got {stats.MaxFrameIntervalMs}");
        Assert.True(stats.AverageFrameIntervalMs > 0, $"Avg interval should be > 0, got {stats.AverageFrameIntervalMs}");

        // Max should be >= Min (basic mathematical invariant)
        Assert.True(stats.MaxFrameIntervalMs >= stats.MinFrameIntervalMs,
            $"Max ({stats.MaxFrameIntervalMs}) should be >= Min ({stats.MinFrameIntervalMs})");
    }

    [Fact]
    public void GetSnapshot_ReturnsImmutableCopy()
    {
        var stats = new AcquisitionStatistics();
        stats.Start();
        stats.RecordFrame();
        stats.RecordFrame();

        var snapshot1 = stats.GetSnapshot();

        stats.RecordFrame();
        stats.RecordFrame();

        var snapshot2 = stats.GetSnapshot();

        Assert.Equal(2, snapshot1.FrameCount);
        Assert.Equal(4, snapshot2.FrameCount);
    }

    [Fact]
    public void ElapsedTime_TracksCorrectly()
    {
        var stats = new AcquisitionStatistics();

        stats.Start();
        Thread.Sleep(50);
        stats.Stop();

        Assert.True(stats.ElapsedTime.TotalMilliseconds >= 40,
            $"Expected >= 40ms, got {stats.ElapsedTime.TotalMilliseconds}ms");
    }

    [Fact]
    public void ToString_ReturnsFormattedString()
    {
        var stats = new AcquisitionStatistics();
        stats.Start();
        stats.RecordFrame();
        stats.RecordError();
        stats.Stop();

        var str = stats.ToString();

        Assert.Contains("Frames:", str);
        Assert.Contains("FPS:", str);
        Assert.Contains("Errors:", str);
    }

    [Fact]
    public void Snapshot_ToString_ReturnsFormattedString()
    {
        var stats = new AcquisitionStatistics();
        stats.Start();
        stats.RecordFrame();
        stats.Stop();

        var snapshot = stats.GetSnapshot();
        var str = snapshot.ToString();

        Assert.Contains("Frames:", str);
        Assert.Contains("FPS:", str);
    }

    [Fact]
    public void MinMaxFrameInterval_NoFrames_ReturnsZero()
    {
        var stats = new AcquisitionStatistics();

        Assert.Equal(0, stats.MinFrameIntervalMs);
        Assert.Equal(0, stats.MaxFrameIntervalMs);
    }

    [Fact]
    public void AverageFrameInterval_SingleFrame_ReturnsZero()
    {
        var stats = new AcquisitionStatistics();
        stats.Start();
        stats.RecordFrame();
        stats.Stop();

        Assert.Equal(0, stats.AverageFrameIntervalMs);
    }

    [Fact]
    public async Task ThreadSafety_ConcurrentRecordFrame()
    {
        var stats = new AcquisitionStatistics();
        stats.Start();

        var tasks = new List<Task>();
        for (int i = 0; i < 10; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                for (int j = 0; j < 100; j++)
                {
                    stats.RecordFrame();
                }
            }));
        }

        await Task.WhenAll(tasks);
        stats.Stop();

        Assert.Equal(1000, stats.FrameCount);
    }
}
