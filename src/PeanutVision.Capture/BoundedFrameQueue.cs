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
