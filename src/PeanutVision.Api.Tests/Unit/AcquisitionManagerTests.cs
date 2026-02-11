using System.Runtime.InteropServices;
using PeanutVision.Api.Services;
using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Hal;
using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Api.Tests.Unit;

public class AcquisitionManagerTests : IDisposable
{
    private readonly MockMultiCamHAL _mockHal;
    private readonly GrabService _grabService;
    private readonly AcquisitionManager _manager;

    public AcquisitionManagerTests()
    {
        _mockHal = new MockMultiCamHAL();
        _grabService = new GrabService(_mockHal);
        _grabService.Initialize();
        _manager = new AcquisitionManager(_grabService);
    }

    public void Dispose()
    {
        _manager.Dispose();
        _grabService.Dispose();
    }

    // --- Lifecycle ---

    [Fact]
    public void Initially_is_not_active()
    {
        Assert.False(_manager.IsActive);
        Assert.Null(_manager.ActiveProfileId);
        Assert.Null(_manager.Channel);
    }

    [Fact]
    public void Start_activates_acquisition()
    {
        _manager.Start("crevis-tc-a160k-freerun-rgb8");

        Assert.True(_manager.IsActive);
        Assert.Equal("crevis-tc-a160k-freerun-rgb8", _manager.ActiveProfileId);
        Assert.NotNull(_manager.Channel);
    }

    [Fact]
    public void Start_sets_hal_to_active()
    {
        _manager.Start("crevis-tc-a160k-freerun-rgb8");

        Assert.True(_mockHal.CallLog.AcquisitionStarted);
    }

    [Fact]
    public void Start_with_unknown_profile_throws_KeyNotFoundException()
    {
        Assert.Throws<KeyNotFoundException>(() => _manager.Start("nonexistent"));
    }

    [Fact]
    public void Start_when_already_active_throws_InvalidOperationException()
    {
        _manager.Start("crevis-tc-a160k-freerun-rgb8");

        Assert.Throws<InvalidOperationException>(() =>
            _manager.Start("crevis-tc-a160k-freerun-rgb8"));
    }

    [Fact]
    public void Start_with_custom_trigger_mode()
    {
        _manager.Start("crevis-tc-a160k-freerun-rgb8", McTrigMode.MC_TrigMode_SOFT);

        Assert.True(_manager.IsActive);
    }

    [Fact]
    public void Stop_deactivates_acquisition()
    {
        _manager.Start("crevis-tc-a160k-freerun-rgb8");
        _manager.Stop();

        Assert.False(_manager.IsActive);
        Assert.Null(_manager.ActiveProfileId);
        Assert.Null(_manager.Channel);
    }

    [Fact]
    public void Stop_when_idle_does_nothing()
    {
        _manager.Stop(); // Should not throw
        Assert.False(_manager.IsActive);
    }

    [Fact]
    public void Can_restart_after_stop()
    {
        _manager.Start("crevis-tc-a160k-freerun-rgb8");
        _manager.Stop();
        _manager.Start("crevis-tc-a160k-softtrig-rgb8");

        Assert.True(_manager.IsActive);
        Assert.Equal("crevis-tc-a160k-softtrig-rgb8", _manager.ActiveProfileId);
    }

    // --- Trigger ---

    [Fact]
    public void SendTrigger_when_inactive_throws()
    {
        Assert.Throws<InvalidOperationException>(() => _manager.SendTrigger());
    }

    [Fact]
    public void SendTrigger_when_active_increments_hal_count()
    {
        _manager.Start("crevis-tc-a160k-freerun-rgb8");
        _mockHal.CallLog.Reset();

        _manager.SendTrigger();
        _manager.SendTrigger();

        Assert.Equal(2, _mockHal.CallLog.SoftwareTriggerCount);
    }

    // --- Frame storage via pinned memory ---

    [Fact]
    public void CaptureFrame_returns_null_when_no_frame()
    {
        Assert.Null(_manager.CaptureFrame());
    }

    [Fact]
    public void CaptureFrame_returns_null_after_start_with_no_callback()
    {
        _manager.Start("crevis-tc-a160k-freerun-rgb8");
        Assert.Null(_manager.CaptureFrame());
    }

    [Fact]
    public void LastFrame_populated_after_simulated_frame_with_pinned_memory()
    {
        // Allocate real pixel data for the mock surface to read
        int width = _mockHal.Configuration.DefaultImageWidth;
        int height = _mockHal.Configuration.DefaultImageHeight;
        int pitch = width * 3;
        int size = pitch * height;
        byte[] pixelData = new byte[size];

        // Fill with a pattern
        for (int i = 0; i < pixelData.Length; i++)
            pixelData[i] = (byte)(i % 256);

        var pinnedHandle = GCHandle.Alloc(pixelData, GCHandleType.Pinned);
        try
        {
            _mockHal.Configuration.SimulatedSurfaceAddress = pinnedHandle.AddrOfPinnedObject();

            _manager.Start("crevis-tc-a160k-freerun-rgb8");
            var channelHandle = _manager.Channel!.Handle;

            // Simulate frame callback
            _mockHal.SimulateFrameAcquisition(channelHandle);

            var frame = _manager.CaptureFrame();
            Assert.NotNull(frame);
            Assert.Equal(width, frame.Width);
            Assert.Equal(height, frame.Height);
            Assert.Equal(pitch, frame.Pitch);
            Assert.Equal(size, frame.Pixels.Length);
        }
        finally
        {
            pinnedHandle.Free();
        }
    }

    // --- Statistics ---

    [Fact]
    public void GetStatistics_returns_null_when_idle()
    {
        Assert.Null(_manager.GetStatistics());
    }

    [Fact]
    public void GetStatistics_returns_snapshot_when_active()
    {
        _manager.Start("crevis-tc-a160k-freerun-rgb8");

        var stats = _manager.GetStatistics();

        Assert.NotNull(stats);
        Assert.Equal(0, stats.Value.FrameCount);
    }

    // --- Dispose ---

    [Fact]
    public void Dispose_stops_active_acquisition()
    {
        _manager.Start("crevis-tc-a160k-freerun-rgb8");
        _manager.Dispose();

        Assert.False(_manager.IsActive);
    }
}
