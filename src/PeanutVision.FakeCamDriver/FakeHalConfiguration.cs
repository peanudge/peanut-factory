namespace PeanutVision.FakeCamDriver;

/// <summary>
/// Configuration for FakeMultiCamHAL behavior.
/// </summary>
public class FakeHalConfiguration
{
    /// <summary>
    /// Delay in milliseconds between trigger and frame delivery.
    /// Simulates camera exposure/transfer time.
    /// </summary>
    public int FrameDelayMs { get; set; }
}
