using PeanutVision.Capture;

namespace PeanutVision.Capture.Tests;

public class CaptureSchedulerTests
{
    [Fact]
    public async Task Start_FiresTickMultipleTimes()
    {
        var ticks = 0;
        using var scheduler = new CaptureScheduler();
        scheduler.Tick += () => Interlocked.Increment(ref ticks);

        scheduler.Start(TimeSpan.FromMilliseconds(40));
        await Task.Delay(200);
        scheduler.Stop();

        // At 40ms interval over 200ms: expect at least 3 ticks
        Assert.True(ticks >= 3, $"Expected >= 3 ticks, got {ticks}");
    }

    [Fact]
    public async Task Stop_PreventsFurtherTicks()
    {
        var ticks = 0;
        using var scheduler = new CaptureScheduler();
        scheduler.Tick += () => Interlocked.Increment(ref ticks);

        scheduler.Start(TimeSpan.FromMilliseconds(30));
        await Task.Delay(80);
        scheduler.Stop();
        var countAfterStop = ticks;

        await Task.Delay(100); // wait -- no more ticks should fire

        Assert.Equal(countAfterStop, ticks);
    }

    [Fact]
    public void Dispose_DoesNotThrow()
    {
        var scheduler = new CaptureScheduler();
        scheduler.Start(TimeSpan.FromMilliseconds(50));
        scheduler.Dispose();
        // If we reach here without throwing, the test passes
    }
}
