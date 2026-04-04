using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Capture;

/// <summary>Buffers captured frames for asynchronous consumption by a background writer.</summary>
public interface IFrameQueue : IDisposable
{
    /// <summary>Enqueues a frame for async saving. Returns false and drops the frame if the queue is full.</summary>
    bool TryEnqueue(ImageData frame);

    /// <summary>Async stream of frames for the background writer to consume.</summary>
    IAsyncEnumerable<ImageData> ReadAllAsync(CancellationToken cancellationToken);
}
