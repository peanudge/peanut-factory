using System.Threading.Channels;
using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Capture;

/// <summary>Thread-safe bounded frame queue backed by a System.Threading.Channel.</summary>
public sealed class BoundedFrameQueue : IFrameQueue
{
    private readonly Channel<ImageData> _channel;

    public BoundedFrameQueue(int capacity = 16)
    {
        if (capacity <= 0) throw new ArgumentOutOfRangeException(nameof(capacity));
        // BoundedChannelFullMode.Wait is chosen deliberately:
        // TryWrite returns false when the channel is full (honest "not enqueued" signal).
        // DropWrite would return true even when silently dropping the frame (misleading).
        // We use TryWrite (not WriteAsync) to avoid blocking the acquisition callback thread,
        // so the channel never actually "waits" — it drops and signals false when full.
        _channel = Channel.CreateBounded<ImageData>(new BoundedChannelOptions(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,
            SingleWriter = false,
        });
    }

    public bool TryEnqueue(ImageData frame) => _channel.Writer.TryWrite(frame);

    public IAsyncEnumerable<ImageData> ReadAllAsync(CancellationToken cancellationToken)
        => _channel.Reader.ReadAllAsync(cancellationToken);

    public void Dispose() => _channel.Writer.TryComplete();
}
