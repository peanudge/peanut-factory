using PeanutVision.FakeCamDriver.FrameGenerators;
using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Hal;

namespace PeanutVision.FakeCamDriver;

/// <summary>
/// Fake camera HAL that generates dynamic test patterns on each trigger.
/// Extends MockMultiCamHAL with real pixel data generation and native surface memory.
/// </summary>
public sealed class FakeMultiCamHAL : MockMultiCamHAL, IDisposable
{
    private readonly IFrameGenerator _frameGenerator;
    private readonly SurfaceMemoryManager _surfaceMemory;
    private int _frameCounter;

    public FakeHalConfiguration FakeConfig { get; } = new();

    public FakeMultiCamHAL(IFrameGenerator? frameGenerator = null)
    {
        _frameGenerator = frameGenerator ?? new TestPatternGenerator();

        var w = Configuration.DefaultImageWidth;
        var h = Configuration.DefaultImageHeight;
        _surfaceMemory = new SurfaceMemoryManager(w, h);
        Configuration.SimulatedSurfaceAddress = _surfaceMemory.Address;
        Configuration.AutoSimulateFrameOnTrigger = true;
    }

    public override int SetParamStr(uint instance, string paramName, string value)
    {
        if (paramName == MultiCamApi.PN_ForceTrig)
        {
            var w = Configuration.DefaultImageWidth;
            var h = Configuration.DefaultImageHeight;
            int pitch = w * 3;
            var buffer = new byte[h * pitch];

            int frame = Interlocked.Increment(ref _frameCounter) - 1;
            _frameGenerator.Generate(buffer, w, h, pitch, frame);
            _surfaceMemory.WriteFrame(buffer);

            if (FakeConfig.FrameDelayMs > 0)
                Thread.Sleep(FakeConfig.FrameDelayMs);
        }

        return base.SetParamStr(instance, paramName, value);
    }

    public void Dispose()
    {
        _surfaceMemory.Dispose();
    }
}
