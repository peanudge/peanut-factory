using System.Runtime.InteropServices;

namespace PeanutVision.MultiCamDriver;

/// <summary>
/// Represents acquired image surface data from the frame grabber.
/// This is a reference to memory managed by MultiCam - do NOT use after releasing the surface.
/// </summary>
public readonly struct SurfaceData
{
    /// <summary>Surface index in the cluster (0-based)</summary>
    public int SurfaceIndex { get; init; }

    /// <summary>Pointer to the start of image data in memory</summary>
    public IntPtr Address { get; init; }

    /// <summary>Row stride in bytes (distance between start of consecutive rows)</summary>
    public int Pitch { get; init; }

    /// <summary>Total buffer size in bytes</summary>
    public int Size { get; init; }

    /// <summary>Image width in pixels</summary>
    public int Width { get; init; }

    /// <summary>Image height in pixels</summary>
    public int Height { get; init; }

    /// <summary>Timestamp when the frame was captured (if available)</summary>
    public long Timestamp { get; init; }

    /// <summary>Frame counter from the acquisition</summary>
    public uint FrameCount { get; init; }

    /// <summary>
    /// Copies the surface data to a managed byte array.
    /// Safe to use after the surface is released.
    /// </summary>
    public byte[] ToArray()
    {
        if (Address == IntPtr.Zero || Size <= 0)
            return Array.Empty<byte>();

        byte[] data = new byte[Size];
        Marshal.Copy(Address, data, 0, Size);
        return data;
    }

    /// <summary>
    /// Copies the surface data to an existing buffer.
    /// </summary>
    /// <param name="destination">Destination buffer (must be at least Size bytes)</param>
    /// <returns>Number of bytes copied</returns>
    public int CopyTo(Span<byte> destination)
    {
        if (Address == IntPtr.Zero || Size <= 0)
            return 0;

        int bytesToCopy = Math.Min(destination.Length, Size);
        unsafe
        {
            fixed (byte* destPtr = destination)
            {
                Buffer.MemoryCopy((void*)Address, destPtr, destination.Length, bytesToCopy);
            }
        }
        return bytesToCopy;
    }

    /// <summary>
    /// Gets a read-only span over the surface data.
    /// WARNING: Only valid while the surface is in PROCESSING state.
    /// </summary>
    public unsafe ReadOnlySpan<byte> AsSpan()
    {
        if (Address == IntPtr.Zero || Size <= 0)
            return ReadOnlySpan<byte>.Empty;

        return new ReadOnlySpan<byte>((void*)Address, Size);
    }
}

/// <summary>
/// Event arguments for frame acquired events
/// </summary>
public class FrameAcquiredEventArgs : EventArgs
{
    /// <summary>The acquired surface data</summary>
    public SurfaceData Surface { get; }

    /// <summary>Channel handle that generated the frame</summary>
    public uint ChannelHandle { get; }

    /// <summary>Signal type that triggered this event</summary>
    public McSignal Signal { get; }

    public FrameAcquiredEventArgs(SurfaceData surface, uint channelHandle, McSignal signal)
    {
        Surface = surface;
        ChannelHandle = channelHandle;
        Signal = signal;
    }
}

/// <summary>
/// Event arguments for acquisition error events
/// </summary>
public class AcquisitionErrorEventArgs : EventArgs
{
    /// <summary>Error signal type</summary>
    public McSignal Signal { get; }

    /// <summary>Channel handle where the error occurred</summary>
    public uint ChannelHandle { get; }

    /// <summary>Additional signal-specific information</summary>
    public uint SignalInfo { get; }

    /// <summary>Error message</summary>
    public string Message { get; }

    public AcquisitionErrorEventArgs(McSignal signal, uint channelHandle, uint signalInfo, string message)
    {
        Signal = signal;
        ChannelHandle = channelHandle;
        SignalInfo = signalInfo;
        Message = message;
    }
}
