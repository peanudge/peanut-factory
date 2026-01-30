using PeanutVision.MultiCamDriver.Camera;

namespace PeanutVision.MultiCamDriver.IntegrationTests;

/// <summary>
/// Hardware integration tests for PeanutVision MultiCam Driver.
/// These tests require actual hardware connected (Euresys Grablink Full + Crevis TC-A160K).
///
/// Run these tests only on machines with hardware:
///   dotnet test --filter "Category=Hardware"
///
/// Skip these tests in CI/CD pipelines:
///   dotnet test --filter "Category!=Hardware"
/// </summary>
[Trait("Category", "Hardware")]
public class HardwareTests : IDisposable
{
    private readonly GrabService _service;
    private bool _hardwareAvailable;

    public HardwareTests()
    {
        _service = new GrabService();

        try
        {
            _service.Initialize();
            _hardwareAvailable = _service.BoardCount > 0;
        }
        catch (MultiCamException)
        {
            _hardwareAvailable = false;
        }
    }

    public void Dispose()
    {
        _service.Dispose();
    }

    private void SkipIfNoHardware()
    {
        if (!_hardwareAvailable)
        {
            throw new SkipException("No frame grabber hardware detected. Connect Grablink Full to run this test.");
        }
    }

    [Fact]
    public void Initialize_WithHardware_ReturnsDriverVersion()
    {
        SkipIfNoHardware();

        Assert.NotEmpty(_service.DriverVersion);
        Assert.True(_service.BoardCount > 0);
    }

    [Fact]
    public void GetBoardInfo_WithHardware_ReturnsValidInfo()
    {
        SkipIfNoHardware();

        var boardInfo = _service.GetBoardInfo(0);

        Assert.NotEmpty(boardInfo.BoardName);
        Assert.NotEmpty(boardInfo.BoardType);
        Assert.NotEmpty(boardInfo.SerialNumber);
    }

    [Fact]
    public void CreateChannel_WithEmbeddedCamFile_Succeeds()
    {
        SkipIfNoHardware();

        using var channel = _service.CreateChannel(CrevisProfiles.TC_A160K_FreeRun_RGB8);

        Assert.True(channel.Handle != 0);
        Assert.True(channel.ImageWidth > 0);
        Assert.True(channel.ImageHeight > 0);
    }

    [Fact]
    public void StartStopAcquisition_WithHardware_NoErrors()
    {
        SkipIfNoHardware();

        using var channel = _service.CreateChannel(CrevisProfiles.TC_A160K_FreeRun_RGB8);

        channel.StartAcquisition();
        Assert.True(channel.IsActive);

        Thread.Sleep(100); // Brief acquisition

        channel.StopAcquisition();
        Assert.False(channel.IsActive);
    }

    [Fact]
    public void FrameAcquisition_Callback_ReceivesFrames()
    {
        SkipIfNoHardware();

        using var channel = _service.CreateChannel(CrevisProfiles.TC_A160K_FreeRun_RGB8);

        var stats = new AcquisitionStatistics();
        int frameCount = 0;
        var frameReceived = new ManualResetEventSlim(false);

        channel.FrameAcquired += (sender, args) =>
        {
            frameCount++;
            stats.RecordFrame();

            if (frameCount >= 10)
            {
                frameReceived.Set();
            }
        };

        stats.Start();
        channel.StartAcquisition();

        // Wait for at least 10 frames (timeout 5 seconds)
        bool received = frameReceived.Wait(5000);

        channel.StopAcquisition();
        stats.Stop();

        Assert.True(received, "Did not receive 10 frames within timeout");
        Assert.True(frameCount >= 10, $"Expected >= 10 frames, got {frameCount}");
        Assert.True(stats.AverageFps > 0, "FPS should be > 0");
    }

    [Fact]
    public void SoftwareTrigger_WithPolling_CapturesFrames()
    {
        SkipIfNoHardware();

        var camPath = CamFileResource.GetCamFilePath(CamFileResource.KnownCamFiles.TC_A160K_FreeRun_RGB8);

        using var channel = _service.CreateChannel(new GrabChannelOptions
        {
            DriverIndex = 0,
            Connector = "M",
            CamFilePath = camPath,
            SurfaceCount = 2,
            UseCallback = false,
            TriggerMode = McTrigMode.MC_TrigMode_SOFT
        });

        channel.StartAcquisition();

        int capturedFrames = 0;
        for (int i = 0; i < 5; i++)
        {
            channel.SendSoftwareTrigger();
            var surface = channel.WaitForFrame(2000);

            if (surface.HasValue)
            {
                capturedFrames++;
                Assert.True(surface.Value.Width > 0);
                Assert.True(surface.Value.Height > 0);
                Assert.True(surface.Value.Address != IntPtr.Zero);
                channel.ReleaseSurface(surface.Value);
            }
        }

        channel.StopAcquisition();

        Assert.True(capturedFrames > 0, "Should have captured at least 1 frame");
    }

    [Fact]
    public void ExposureControl_WithHardware_CanReadWrite()
    {
        SkipIfNoHardware();

        using var channel = _service.CreateChannel(CrevisProfiles.TC_A160K_FreeRun_RGB8);

        try
        {
            var (minExp, maxExp) = channel.GetExposureRange();
            Assert.True(maxExp > minExp, "Max exposure should be > min");

            double currentExp = channel.GetExposureUs();
            Assert.True(currentExp >= minExp && currentExp <= maxExp,
                $"Current exposure {currentExp} should be within range [{minExp}, {maxExp}]");

            // Try setting a new value
            double newExp = (minExp + maxExp) / 2;
            channel.SetExposureUs(newExp);

            double readBack = channel.GetExposureUs();
            Assert.True(Math.Abs(readBack - newExp) < 1.0,
                $"Exposure readback {readBack} should be close to set value {newExp}");
        }
        catch (MultiCamException ex) when (ex.Message.Contains("INVALID_PARAM"))
        {
            // Exposure control may not be available on all cameras
            throw new SkipException("Exposure control not available on this camera");
        }
    }

    [Fact]
    public void WhiteBalanceRatios_WithHardware_CanReadWrite()
    {
        SkipIfNoHardware();

        using var channel = _service.CreateChannel(CrevisProfiles.TC_A160K_FreeRun_RGB8);

        try
        {
            var (r, g, b) = channel.GetWhiteBalanceRatios();

            // Ratios should be reasonable values (typically between 0.5 and 2.0)
            Assert.True(r > 0, $"Red ratio should be > 0, got {r}");
            Assert.True(g > 0, $"Green ratio should be > 0, got {g}");
            Assert.True(b > 0, $"Blue ratio should be > 0, got {b}");
        }
        catch (MultiCamException ex) when (ex.Message.Contains("INVALID_PARAM"))
        {
            throw new SkipException("White balance not available on this camera");
        }
    }

    [Fact]
    public void PerformanceTest_SustainedAcquisition_NoDroppedFrames()
    {
        SkipIfNoHardware();

        using var channel = _service.CreateChannel(CrevisProfiles.TC_A160K_FreeRun_RGB8);

        var stats = new AcquisitionStatistics();
        int targetFrames = 500;
        int receivedFrames = 0;
        var done = new ManualResetEventSlim(false);

        channel.FrameAcquired += (sender, args) =>
        {
            stats.RecordFrame();
            if (++receivedFrames >= targetFrames)
            {
                done.Set();
            }
        };

        channel.AcquisitionError += (sender, args) =>
        {
            stats.RecordError();
        };

        stats.Start();
        channel.StartAcquisition();

        // Wait for 500 frames with 30 second timeout
        bool completed = done.Wait(30000);

        channel.StopAcquisition();
        stats.Stop();

        var snapshot = stats.GetSnapshot();

        Assert.True(completed, $"Did not receive {targetFrames} frames within timeout. Got {receivedFrames}");
        Assert.Equal(0, snapshot.ErrorCount);
        Assert.True(snapshot.AverageFps > 10, $"FPS too low: {snapshot.AverageFps}");
    }
}

/// <summary>
/// Exception type to mark tests as skipped when hardware is not available.
/// </summary>
public class SkipException : Exception
{
    public SkipException(string message) : base(message)
    {
    }
}
