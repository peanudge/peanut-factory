namespace PeanutVision.MultiCamDriver.Tests;

public class MultiCamDriverTests
{
    [Fact]
    public void MultiCamNative_Constants_HaveExpectedValues()
    {
        // System object handles
        Assert.Equal(0x10000000u, MultiCamApi.MC_CONFIGURATION);
        Assert.Equal(0x20000000u, MultiCamApi.MC_BOARD);
        Assert.Equal(0x00000001u, MultiCamApi.MC_CHANNEL);
        Assert.Equal(0xFFFFFFFFu, MultiCamApi.MC_DEFAULT_SURFACE_HANDLE);
        Assert.Equal(0, MultiCamApi.MC_OK);
    }

    [Fact]
    public void MultiCamNative_ParameterNames_AreDefined()
    {
        // Configuration parameters
        Assert.Equal("BoardCount", MultiCamApi.PN_BoardCount);
        Assert.Equal("DriverVersion", MultiCamApi.PN_DriverVersion);

        // Channel parameters
        Assert.Equal("DriverIndex", MultiCamApi.PN_DriverIndex);
        Assert.Equal("CamFile", MultiCamApi.PN_CamFile);
        Assert.Equal("ChannelState", MultiCamApi.PN_ChannelState);
        Assert.Equal("TrigMode", MultiCamApi.PN_TrigMode);

        // Calibration parameters
        Assert.Equal("BlackCalibration", MultiCamApi.PN_BlackCalibration);
        Assert.Equal("WhiteCalibration", MultiCamApi.PN_WhiteCalibration);
        Assert.Equal("BalanceWhiteAuto", MultiCamApi.PN_BalanceWhiteAuto);
    }

    [Fact]
    public void McSignalInfo_HasCorrectLayout()
    {
        var info = new McSignalInfo
        {
            Context = IntPtr.Zero,
            Instance = 123,
            Signal = (int)McSignal.MC_SIG_SURFACE_PROCESSING,
            SignalInfo = 456,
            Reserved = 789
        };

        Assert.Equal(123u, info.Instance);
        Assert.Equal((int)McSignal.MC_SIG_SURFACE_PROCESSING, info.Signal);
        Assert.Equal(456u, info.SignalInfo);
    }

    [Fact]
    public void McStatus_HasExpectedValues()
    {
        Assert.Equal(0, (int)McStatus.MC_OK);
        Assert.Equal(-1, (int)McStatus.MC_ERROR);
        Assert.Equal(-4, (int)McStatus.MC_TIMEOUT);
        Assert.Equal(-8, (int)McStatus.MC_BUSY);
    }

    [Fact]
    public void McSignal_HasExpectedValues()
    {
        Assert.Equal(1, (int)McSignal.MC_SIG_SURFACE_PROCESSING);
        Assert.Equal(2, (int)McSignal.MC_SIG_SURFACE_FILLED);
        Assert.Equal(3, (int)McSignal.MC_SIG_ACQUISITION_FAILURE);
        Assert.Equal(-1, (int)McSignal.MC_SIG_ANY);
    }

    [Fact]
    public void McTrigMode_HasExpectedValues()
    {
        Assert.Equal(0, (int)McTrigMode.MC_TrigMode_IMMEDIATE);
        Assert.Equal(1, (int)McTrigMode.MC_TrigMode_HARD);
        Assert.Equal(2, (int)McTrigMode.MC_TrigMode_SOFT);
        Assert.Equal(3, (int)McTrigMode.MC_TrigMode_COMBINED);
    }

    [Fact]
    public void McChannelState_HasExpectedValues()
    {
        Assert.Equal(0, (int)McChannelState.MC_ChannelState_ORPHAN);
        Assert.Equal(1, (int)McChannelState.MC_ChannelState_IDLE);
        Assert.Equal(2, (int)McChannelState.MC_ChannelState_ACTIVE);
    }

    [Fact]
    public void MultiCamException_ContainsStatusCodeAndOperation()
    {
        var ex = new MultiCamException(-4, "McWaitSignal");

        Assert.Equal(-4, ex.StatusCode);
        Assert.Equal("McWaitSignal", ex.Operation);
        Assert.Contains("MC_TIMEOUT", ex.Message);
    }

    [Fact]
    public void GrabService_CanBeInstantiated()
    {
        using var service = new GrabService();
        Assert.False(service.IsInitialized);
    }

    [Fact]
    public void GrabService_ThrowsWhenNotInitialized()
    {
        using var service = new GrabService();

        Assert.Throws<InvalidOperationException>(() =>
            service.CreateChannel("test.cam"));
    }

    [Fact]
    public void GrabService_ThrowsAfterDispose()
    {
        var service = new GrabService();
        service.Dispose();

        Assert.Throws<ObjectDisposedException>(() => service.Initialize());
    }

    [Fact]
    public void GrabChannelOptions_HasDefaultValues()
    {
        var options = new GrabChannelOptions();

        Assert.Equal(0, options.DriverIndex);
        Assert.Equal("M", options.Connector);
        Assert.Null(options.CamFilePath);
        Assert.Equal(4, options.SurfaceCount);
        Assert.True(options.UseCallback);
        Assert.Equal(McTrigMode.MC_TrigMode_IMMEDIATE, options.TriggerMode);
    }

    [Fact]
    public void SurfaceData_ToArray_ReturnsEmptyForZeroAddress()
    {
        var surface = new SurfaceData
        {
            Address = IntPtr.Zero,
            Size = 100
        };

        var result = surface.ToArray();

        Assert.Empty(result);
    }

    [Fact]
    public void SurfaceData_AsSpan_ReturnsEmptyForZeroAddress()
    {
        var surface = new SurfaceData
        {
            Address = IntPtr.Zero,
            Size = 100
        };

        var span = surface.AsSpan();

        Assert.True(span.IsEmpty);
    }

    [Fact]
    public void BoardInfo_CanBeCreated()
    {
        var info = new BoardInfo
        {
            Index = 0,
            BoardType = "Grablink Full",
            BoardName = "PC1622",
            SerialNumber = "12345",
            PCIPosition = "0:1:0"
        };

        Assert.Equal(0, info.Index);
        Assert.Equal("Grablink Full", info.BoardType);
        Assert.Equal("PC1622", info.BoardName);
    }

    [Fact]
    public void FrameAcquiredEventArgs_ContainsSurfaceData()
    {
        var surface = new SurfaceData
        {
            SurfaceIndex = 2,
            Width = 1920,
            Height = 1080
        };

        var args = new FrameAcquiredEventArgs(surface, 123, McSignal.MC_SIG_SURFACE_PROCESSING);

        Assert.Equal(2, args.Surface.SurfaceIndex);
        Assert.Equal(1920, args.Surface.Width);
        Assert.Equal(123u, args.ChannelHandle);
        Assert.Equal(McSignal.MC_SIG_SURFACE_PROCESSING, args.Signal);
    }

    [Fact]
    public void AcquisitionErrorEventArgs_ContainsErrorInfo()
    {
        var args = new AcquisitionErrorEventArgs(
            McSignal.MC_SIG_UNRECOVERABLE_ERROR,
            channelHandle: 456,
            signalInfo: 789,
            message: "Test error"
        );

        Assert.Equal(McSignal.MC_SIG_UNRECOVERABLE_ERROR, args.Signal);
        Assert.Equal(456u, args.ChannelHandle);
        Assert.Equal("Test error", args.Message);
    }
}
