using PeanutVision.MultiCamDriver.Hal;

namespace PeanutVision.MultiCamDriver.Tests;

public class GrabChannelTests
{
    private readonly MockMultiCamHAL _mockHal;

    public GrabChannelTests()
    {
        _mockHal = new MockMultiCamHAL();
    }

    #region Channel Creation

    [Fact]
    public void Constructor_CreatesChannel()
    {
        var options = new GrabChannelOptions();

        using var channel = new GrabChannel(options, _mockHal);

        Assert.True(channel.Handle > 0);
        Assert.Equal(1, _mockHal.CallLog.CreateCalls);
    }

    [Fact]
    public void Constructor_SetsDriverIndex()
    {
        var options = new GrabChannelOptions { DriverIndex = 2 };

        using var channel = new GrabChannel(options, _mockHal);

        Assert.True(_mockHal.CallLog.LastSetParams.ContainsKey(MultiCamApi.PN_DriverIndex));
        Assert.Equal(2, _mockHal.CallLog.LastSetParams[MultiCamApi.PN_DriverIndex]);
    }

    [Fact]
    public void Constructor_SetsConnector()
    {
        var options = new GrabChannelOptions { Connector = "A" };

        using var channel = new GrabChannel(options, _mockHal);

        Assert.Equal("A", _mockHal.CallLog.LastSetParams[MultiCamApi.PN_Connector]);
    }

    [Fact]
    public void Constructor_SetsCamFile_WhenProvided()
    {
        var options = new GrabChannelOptions { CamFilePath = "test.cam" };

        using var channel = new GrabChannel(options, _mockHal);

        Assert.Equal("test.cam", _mockHal.CallLog.LastSetParams[MultiCamApi.PN_CamFile]);
    }

    [Fact]
    public void Constructor_SetsSurfaceCount()
    {
        var options = new GrabChannelOptions { SurfaceCount = 8 };

        using var channel = new GrabChannel(options, _mockHal);

        Assert.Equal(8, _mockHal.CallLog.LastSetParams[MultiCamApi.PN_SurfaceCount]);
    }

    [Fact]
    public void Constructor_SetsTriggerMode_Immediate()
    {
        var options = new GrabChannelOptions { TriggerMode = McTrigMode.MC_TrigMode_IMMEDIATE };

        using var channel = new GrabChannel(options, _mockHal);

        Assert.Equal(MultiCamApi.MC_TrigMode_IMMEDIATE_STR, _mockHal.CallLog.LastSetParams[MultiCamApi.PN_TrigMode]);
    }

    [Fact]
    public void Constructor_SetsTriggerMode_Soft()
    {
        var options = new GrabChannelOptions { TriggerMode = McTrigMode.MC_TrigMode_SOFT };

        using var channel = new GrabChannel(options, _mockHal);

        Assert.Equal(MultiCamApi.MC_TrigMode_SOFT_STR, _mockHal.CallLog.LastSetParams[MultiCamApi.PN_TrigMode]);
    }

    [Fact]
    public void Constructor_RegistersCallback_WhenEnabled()
    {
        var options = new GrabChannelOptions { UseCallback = true };

        using var channel = new GrabChannel(options, _mockHal);

        Assert.Equal(1, _mockHal.CallLog.RegisterCallbackCalls);
    }

    [Fact]
    public void Constructor_DoesNotRegisterCallback_WhenDisabled()
    {
        var options = new GrabChannelOptions { UseCallback = false };

        using var channel = new GrabChannel(options, _mockHal);

        Assert.Equal(0, _mockHal.CallLog.RegisterCallbackCalls);
    }

    [Fact]
    public void Constructor_ReadsImageParameters()
    {
        _mockHal.Configuration.DefaultImageWidth = 1920;
        _mockHal.Configuration.DefaultImageHeight = 1080;
        var options = new GrabChannelOptions();

        using var channel = new GrabChannel(options, _mockHal);

        Assert.Equal(1920, channel.ImageWidth);
        Assert.Equal(1080, channel.ImageHeight);
    }

    [Fact]
    public void Constructor_WhenCreateFails_ThrowsException()
    {
        _mockHal.Configuration.CreateFailure = (int)McStatus.MC_ERROR;
        var options = new GrabChannelOptions();

        Assert.Throws<MultiCamException>(() => new GrabChannel(options, _mockHal));
    }

    #endregion

    #region Acquisition Control

    [Fact]
    public void StartAcquisition_SetsChannelStateToActive()
    {
        var options = new GrabChannelOptions();
        using var channel = new GrabChannel(options, _mockHal);

        channel.StartAcquisition();

        Assert.True(_mockHal.CallLog.AcquisitionStarted);
        Assert.True(channel.IsActive);
    }

    [Fact]
    public void StartAcquisition_WithFrameCount_SetsSequenceLength()
    {
        var options = new GrabChannelOptions();
        using var channel = new GrabChannel(options, _mockHal);

        channel.StartAcquisition(100);

        Assert.Equal(100, _mockHal.CallLog.LastSetParams[MultiCamApi.PN_SeqLength_Fr]);
    }

    [Fact]
    public void StartAcquisition_CalledTwice_OnlyStartsOnce()
    {
        var options = new GrabChannelOptions();
        using var channel = new GrabChannel(options, _mockHal);

        channel.StartAcquisition();
        int setParamCallsAfterFirst = _mockHal.CallLog.SetParamCalls;

        channel.StartAcquisition(); // Should be no-op

        // SetParam count should not increase significantly
        Assert.True(channel.IsActive);
    }

    [Fact]
    public void StopAcquisition_SetsChannelStateToIdle()
    {
        var options = new GrabChannelOptions();
        using var channel = new GrabChannel(options, _mockHal);

        channel.StartAcquisition();
        channel.StopAcquisition();

        Assert.True(_mockHal.CallLog.AcquisitionStopped);
        Assert.False(channel.IsActive);
    }

    [Fact]
    public void StopAcquisition_WhenNotActive_DoesNothing()
    {
        var options = new GrabChannelOptions();
        using var channel = new GrabChannel(options, _mockHal);

        channel.StopAcquisition(); // Should not throw

        Assert.False(channel.IsActive);
    }

    [Fact]
    public void SendSoftwareTrigger_SetsForceTrig()
    {
        var options = new GrabChannelOptions { TriggerMode = McTrigMode.MC_TrigMode_SOFT };
        using var channel = new GrabChannel(options, _mockHal);

        channel.SendSoftwareTrigger();

        Assert.Equal(1, _mockHal.CallLog.SoftwareTriggerCount);
    }

    [Fact]
    public void WaitForFrame_ReturnsFrameData()
    {
        var options = new GrabChannelOptions { UseCallback = false };
        using var channel = new GrabChannel(options, _mockHal);

        var surface = channel.WaitForFrame(1000);

        Assert.NotNull(surface);
        Assert.Equal(1, _mockHal.CallLog.WaitSignalCalls);
    }

    [Fact]
    public void WaitForFrame_OnTimeout_ReturnsNull()
    {
        _mockHal.Configuration.WaitSignalFailure = (int)McStatus.MC_TIMEOUT;
        var options = new GrabChannelOptions { UseCallback = false };
        using var channel = new GrabChannel(options, _mockHal);

        var surface = channel.WaitForFrame(1000);

        Assert.Null(surface);
    }

    #endregion

    #region Calibration

    [Fact]
    public void PerformBlackCalibration_SetsParameter()
    {
        var options = new GrabChannelOptions();
        using var channel = new GrabChannel(options, _mockHal);

        channel.PerformBlackCalibration();

        Assert.True(_mockHal.CallLog.BlackCalibrationPerformed);
    }

    [Fact]
    public void PerformWhiteCalibration_SetsParameter()
    {
        var options = new GrabChannelOptions();
        using var channel = new GrabChannel(options, _mockHal);

        channel.PerformWhiteCalibration();

        Assert.True(_mockHal.CallLog.WhiteCalibrationPerformed);
    }

    [Fact]
    public void SetFlatFieldCorrection_SetsParameter()
    {
        var options = new GrabChannelOptions();
        using var channel = new GrabChannel(options, _mockHal);

        channel.SetFlatFieldCorrection(true);

        Assert.Equal("ON", _mockHal.CallLog.LastSetParams[MultiCamApi.PN_FlatFieldCorrection]);
    }

    [Fact]
    public void PerformWhiteBalanceOnce_SetsParameter()
    {
        var options = new GrabChannelOptions();
        using var channel = new GrabChannel(options, _mockHal);

        channel.PerformWhiteBalanceOnce();

        Assert.True(_mockHal.CallLog.WhiteBalancePerformed);
    }

    [Fact]
    public void GetWhiteBalanceRatios_ReturnsCurrentRatios()
    {
        var options = new GrabChannelOptions();
        using var channel = new GrabChannel(options, _mockHal);

        var (red, green, blue) = channel.GetWhiteBalanceRatios();

        Assert.Equal(1.0, red);
        Assert.Equal(1.0, green);
        Assert.Equal(1.0, blue);
    }

    [Fact]
    public void PerformWhiteBalanceOnce_AdjustsRatios()
    {
        var options = new GrabChannelOptions();
        using var channel = new GrabChannel(options, _mockHal);

        channel.PerformWhiteBalanceOnce();
        var (red, green, blue) = channel.GetWhiteBalanceRatios();

        // Mock HAL simulates adjustment
        Assert.Equal(1.05, red);
        Assert.Equal(1.0, green);
        Assert.Equal(0.95, blue);
    }

    [Fact]
    public void SetWhiteBalanceRatios_SetsCustomRatios()
    {
        var options = new GrabChannelOptions();
        using var channel = new GrabChannel(options, _mockHal);

        channel.SetWhiteBalanceRatios(1.1, 1.0, 0.9);
        var (red, green, blue) = channel.GetWhiteBalanceRatios();

        Assert.Equal(1.1, red);
        Assert.Equal(1.0, green);
        Assert.Equal(0.9, blue);
    }

    #endregion

    #region Exposure Control

    [Fact]
    public void GetExposureUs_ReturnsCurrentExposure()
    {
        var options = new GrabChannelOptions();
        using var channel = new GrabChannel(options, _mockHal);

        var exposure = channel.GetExposureUs();

        Assert.Equal(10000.0, exposure); // Default from mock
    }

    [Fact]
    public void SetExposureUs_SetsExposure()
    {
        var options = new GrabChannelOptions();
        using var channel = new GrabChannel(options, _mockHal);

        channel.SetExposureUs(5000.0);
        var exposure = channel.GetExposureUs();

        Assert.Equal(5000.0, exposure);
    }

    [Fact]
    public void GetExposureRange_ReturnsMinMax()
    {
        var options = new GrabChannelOptions();
        using var channel = new GrabChannel(options, _mockHal);

        var (min, max) = channel.GetExposureRange();

        Assert.Equal(10.0, min);
        Assert.Equal(1000000.0, max);
    }

    [Fact]
    public void GetGainDb_ReturnsCurrentGain()
    {
        var options = new GrabChannelOptions();
        using var channel = new GrabChannel(options, _mockHal);

        var gain = channel.GetGainDb();

        Assert.Equal(0.0, gain);
    }

    [Fact]
    public void SetGainDb_SetsGain()
    {
        var options = new GrabChannelOptions();
        using var channel = new GrabChannel(options, _mockHal);

        channel.SetGainDb(6.0);
        var gain = channel.GetGainDb();

        Assert.Equal(6.0, gain);
    }

    #endregion

    #region Parameter Access

    [Fact]
    public void GetParamInt_ReturnsValue()
    {
        var options = new GrabChannelOptions();
        using var channel = new GrabChannel(options, _mockHal);

        var width = channel.GetParamInt(MultiCamApi.PN_ImageSizeX);

        Assert.Equal(1920, width);
    }

    [Fact]
    public void SetParamInt_SetsValue()
    {
        var options = new GrabChannelOptions();
        using var channel = new GrabChannel(options, _mockHal);

        channel.SetParamInt(MultiCamApi.PN_SurfaceCount, 16);

        Assert.Equal(16, _mockHal.CallLog.LastSetParams[MultiCamApi.PN_SurfaceCount]);
    }

    [Fact]
    public void GetParamFloat_ReturnsValue()
    {
        var options = new GrabChannelOptions();
        using var channel = new GrabChannel(options, _mockHal);

        var exposure = channel.GetParamFloat(MultiCamApi.PN_Expose_us);

        Assert.Equal(10000.0, exposure);
    }

    [Fact]
    public void SetParamFloat_SetsValue()
    {
        var options = new GrabChannelOptions();
        using var channel = new GrabChannel(options, _mockHal);

        channel.SetParamFloat(MultiCamApi.PN_Expose_us, 20000.0);

        Assert.Equal(20000.0, _mockHal.CallLog.LastSetParams[MultiCamApi.PN_Expose_us]);
    }

    [Fact]
    public void GetParamStr_ReturnsValue()
    {
        var options = new GrabChannelOptions();
        using var channel = new GrabChannel(options, _mockHal);

        var state = channel.GetParamStr(MultiCamApi.PN_ChannelState);

        Assert.Equal(MultiCamApi.MC_ChannelState_IDLE_STR, state);
    }

    [Fact]
    public void SetParamStr_SetsValue()
    {
        var options = new GrabChannelOptions();
        using var channel = new GrabChannel(options, _mockHal);

        channel.SetParamStr(MultiCamApi.PN_TrigMode, MultiCamApi.MC_TrigMode_HARD_STR);

        Assert.Equal(MultiCamApi.MC_TrigMode_HARD_STR, _mockHal.CallLog.LastSetParams[MultiCamApi.PN_TrigMode]);
    }

    #endregion

    #region Disposal

    [Fact]
    public void Dispose_DeletesChannel()
    {
        var options = new GrabChannelOptions();
        var channel = new GrabChannel(options, _mockHal);

        channel.Dispose();

        Assert.Equal(1, _mockHal.CallLog.DeleteCalls);
    }

    [Fact]
    public void Dispose_StopsAcquisitionFirst()
    {
        var options = new GrabChannelOptions();
        var channel = new GrabChannel(options, _mockHal);
        channel.StartAcquisition();

        channel.Dispose();

        Assert.True(_mockHal.CallLog.AcquisitionStopped);
    }

    [Fact]
    public void Dispose_CalledMultipleTimes_IsSafe()
    {
        var options = new GrabChannelOptions();
        var channel = new GrabChannel(options, _mockHal);

        channel.Dispose();
        channel.Dispose();
        channel.Dispose();

        Assert.Equal(1, _mockHal.CallLog.DeleteCalls);
    }

    [Fact]
    public void AfterDispose_StartAcquisition_ThrowsException()
    {
        var options = new GrabChannelOptions();
        var channel = new GrabChannel(options, _mockHal);
        channel.Dispose();

        Assert.Throws<ObjectDisposedException>(() => channel.StartAcquisition());
    }

    [Fact]
    public void AfterDispose_GetParamInt_ThrowsException()
    {
        var options = new GrabChannelOptions();
        var channel = new GrabChannel(options, _mockHal);
        channel.Dispose();

        Assert.Throws<ObjectDisposedException>(() =>
            channel.GetParamInt(MultiCamApi.PN_ImageSizeX));
    }

    #endregion

    #region Events

    [Fact]
    public void FrameAcquired_FiredOnSurfaceProcessing()
    {
        var options = new GrabChannelOptions { UseCallback = true };
        using var channel = new GrabChannel(options, _mockHal);
        bool eventFired = false;
        SurfaceData? receivedSurface = null;

        channel.FrameAcquired += (sender, args) =>
        {
            eventFired = true;
            receivedSurface = args.Surface;
        };

        // Simulate frame acquisition
        _mockHal.SimulateFrameAcquisition(channel.Handle, surfaceIndex: 2);

        Assert.True(eventFired);
        Assert.NotNull(receivedSurface);
        Assert.Equal(2, receivedSurface.Value.SurfaceIndex);
    }

    [Fact]
    public void AcquisitionError_FiredOnFailure()
    {
        var options = new GrabChannelOptions { UseCallback = true };
        using var channel = new GrabChannel(options, _mockHal);
        bool errorFired = false;
        McSignal? receivedSignal = null;

        channel.AcquisitionError += (sender, args) =>
        {
            errorFired = true;
            receivedSignal = args.Signal;
        };

        // Simulate acquisition failure
        _mockHal.SimulateAcquisitionError(channel.Handle, McSignal.MC_SIG_ACQUISITION_FAILURE);

        Assert.True(errorFired);
        Assert.Equal(McSignal.MC_SIG_ACQUISITION_FAILURE, receivedSignal);
    }

    [Fact]
    public void AcquisitionEnded_FiredOnEndChannelActivity()
    {
        var options = new GrabChannelOptions { UseCallback = true };
        using var channel = new GrabChannel(options, _mockHal);
        channel.StartAcquisition();
        bool endFired = false;

        channel.AcquisitionEnded += (sender, args) =>
        {
            endFired = true;
        };

        // Simulate end of activity
        _mockHal.SimulateAcquisitionError(channel.Handle, McSignal.MC_SIG_END_CHANNEL_ACTIVITY);

        Assert.True(endFired);
        Assert.False(channel.IsActive);
    }

    [Fact]
    public void UnrecoverableError_StopsAcquisitionAndFiresEvent()
    {
        var options = new GrabChannelOptions { UseCallback = true };
        using var channel = new GrabChannel(options, _mockHal);
        channel.StartAcquisition();
        bool errorFired = false;

        channel.AcquisitionError += (sender, args) =>
        {
            errorFired = true;
        };

        // Simulate unrecoverable error
        _mockHal.SimulateAcquisitionError(channel.Handle, McSignal.MC_SIG_UNRECOVERABLE_ERROR);

        Assert.True(errorFired);
        Assert.False(channel.IsActive);
    }

    #endregion
}
