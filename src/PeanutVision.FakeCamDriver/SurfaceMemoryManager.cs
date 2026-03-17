using System.Runtime.InteropServices;

namespace PeanutVision.FakeCamDriver;

/// <summary>
/// Manages native memory allocation for simulated surface buffers.
/// </summary>
public sealed class SurfaceMemoryManager : IDisposable
{
    private IntPtr _memory;

    public IntPtr Address => _memory;
    public int Size { get; }

    public SurfaceMemoryManager(int width, int height, int bytesPerPixel = 3)
    {
        Size = width * height * bytesPerPixel;
        _memory = Marshal.AllocHGlobal(Size);
        unsafe { new Span<byte>((void*)_memory, Size).Clear(); }
    }

    public void WriteFrame(byte[] frameData)
    {
        Marshal.Copy(frameData, 0, _memory, Math.Min(frameData.Length, Size));
    }

    public void Dispose()
    {
        if (_memory != IntPtr.Zero)
        {
            Marshal.FreeHGlobal(_memory);
            _memory = IntPtr.Zero;
        }
    }
}
