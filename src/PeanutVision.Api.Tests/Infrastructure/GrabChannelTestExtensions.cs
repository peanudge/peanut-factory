using PeanutVision.MultiCamDriver;

namespace PeanutVision.Api.Tests.Infrastructure;

internal static class GrabChannelTestExtensions
{
    /// <summary>
    /// Returns a task that completes after the next frame has been fully processed
    /// by any previously-registered FrameAcquired handlers (e.g. AcquisitionManager).
    /// Subscribe BEFORE triggering the frame to avoid missing it.
    /// </summary>
    internal static Task WaitForNextFrameAsync(this GrabChannel channel)
    {
        var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        channel.FrameAcquired += Handler;
        return tcs.Task;

        void Handler(object? sender, FrameAcquiredEventArgs e)
        {
            channel.FrameAcquired -= Handler;
            tcs.TrySetResult();
        }
    }
}
