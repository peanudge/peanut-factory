using PeanutVision.Capture;
using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Capture.Tests;

public class BoundedFrameQueueTests
{
    private static ImageData MakeFrame() => new(new byte[12], 2, 2, 6);

    [Fact]
    public void TryEnqueue_BelowCapacity_ReturnsTrue()
    {
        using var queue = new BoundedFrameQueue(capacity: 4);
        Assert.True(queue.TryEnqueue(MakeFrame()));
    }

    [Fact]
    public void TryEnqueue_AtCapacity_ReturnsFalse()
    {
        using var queue = new BoundedFrameQueue(capacity: 1);
        queue.TryEnqueue(MakeFrame()); // fill
        Assert.False(queue.TryEnqueue(MakeFrame())); // overflow → drop
    }

    [Fact]
    public async Task ReadAllAsync_YieldsAllEnqueuedFrames()
    {
        using var queue = new BoundedFrameQueue(capacity: 4);
        var f1 = MakeFrame();
        var f2 = MakeFrame();
        queue.TryEnqueue(f1);
        queue.TryEnqueue(f2);
        queue.Dispose(); // complete the channel so ReadAllAsync terminates

        var results = new List<ImageData>();
        await foreach (var f in queue.ReadAllAsync(CancellationToken.None))
            results.Add(f);

        Assert.Equal(2, results.Count);
    }

    [Fact]
    public async Task ReadAllAsync_CompletesOnDispose()
    {
        var queue = new BoundedFrameQueue(capacity: 4);
        var cts = new CancellationTokenSource();

        var readerTask = Task.Run(async () =>
        {
            var count = 0;
            await foreach (var _ in queue.ReadAllAsync(cts.Token))
                count++;
            return count;
        });

        queue.TryEnqueue(MakeFrame());
        await Task.Delay(20);
        queue.Dispose(); // signals completion

        var received = await readerTask.WaitAsync(TimeSpan.FromSeconds(2));
        Assert.Equal(1, received);
    }
}
