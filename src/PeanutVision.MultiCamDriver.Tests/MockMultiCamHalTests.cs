using PeanutVision.MultiCamDriver.Hal;

namespace PeanutVision.MultiCamDriver.Tests;

public class MockMultiCamHalTests
{
    private readonly MockMultiCamHAL _mockHal;

    public MockMultiCamHalTests()
    {
        _mockHal = new MockMultiCamHAL();
    }

    #region Configuration

    [Fact]
    public void Configuration_BoardCount_DefaultsToOne()
    {
        Assert.Equal(1, _mockHal.Configuration.BoardCount);
    }

    [Fact]
    public void Configuration_DriverVersion_DefaultsToMock()
    {
        Assert.Contains("Mock", _mockHal.Configuration.DriverVersion);
    }

    [Fact]
    public void Configuration_ImageSize_DefaultsTo1920x1080()
    {
        Assert.Equal(1920, _mockHal.Configuration.DefaultImageWidth);
        Assert.Equal(1080, _mockHal.Configuration.DefaultImageHeight);
    }

    [Fact]
    public void Configuration_CanBeModified()
    {
        _mockHal.Configuration.BoardCount = 3;
        _mockHal.Configuration.DriverVersion = "Test Version";
        _mockHal.Configuration.DefaultImageWidth = 640;
        _mockHal.Configuration.DefaultImageHeight = 480;

        Assert.Equal(3, _mockHal.Configuration.BoardCount);
        Assert.Equal("Test Version", _mockHal.Configuration.DriverVersion);
        Assert.Equal(640, _mockHal.Configuration.DefaultImageWidth);
        Assert.Equal(480, _mockHal.Configuration.DefaultImageHeight);
    }

    #endregion

    #region Driver Lifecycle

    [Fact]
    public void OpenDriver_ReturnsOK()
    {
        int result = _mockHal.OpenDriver(null);

        Assert.Equal(MultiCamNative.MC_OK, result);
        Assert.Equal(1, _mockHal.CallLog.OpenDriverCalls);
    }

    [Fact]
    public void OpenDriver_WhenConfiguredToFail_ReturnsError()
    {
        _mockHal.Configuration.OpenDriverFailure = (int)McStatus.MC_ERROR;

        int result = _mockHal.OpenDriver(null);

        Assert.Equal((int)McStatus.MC_ERROR, result);
    }

    [Fact]
    public void CloseDriver_ReturnsOK()
    {
        int result = _mockHal.CloseDriver();

        Assert.Equal(MultiCamNative.MC_OK, result);
        Assert.Equal(1, _mockHal.CallLog.CloseDriverCalls);
    }

    #endregion

    #region Instance Management

    [Fact]
    public void Create_ReturnsUniqueInstanceHandle()
    {
        int result1 = _mockHal.Create(MultiCamNative.MC_CHANNEL, out uint handle1);
        int result2 = _mockHal.Create(MultiCamNative.MC_CHANNEL, out uint handle2);

        Assert.Equal(MultiCamNative.MC_OK, result1);
        Assert.Equal(MultiCamNative.MC_OK, result2);
        Assert.NotEqual(handle1, handle2);
    }

    [Fact]
    public void Create_WhenConfiguredToFail_ReturnsError()
    {
        _mockHal.Configuration.CreateFailure = (int)McStatus.MC_NO_MORE_RESOURCES;

        int result = _mockHal.Create(MultiCamNative.MC_CHANNEL, out uint handle);

        Assert.Equal((int)McStatus.MC_NO_MORE_RESOURCES, result);
        Assert.Equal(0u, handle);
    }

    [Fact]
    public void Delete_ExistingInstance_ReturnsOK()
    {
        _mockHal.Create(MultiCamNative.MC_CHANNEL, out uint handle);

        int result = _mockHal.Delete(handle);

        Assert.Equal(MultiCamNative.MC_OK, result);
    }

    [Fact]
    public void Delete_NonExistentInstance_ReturnsInvalidHandle()
    {
        int result = _mockHal.Delete(9999);

        Assert.Equal((int)McStatus.MC_INVALID_HANDLE, result);
    }

    #endregion

    #region Parameter Access

    [Fact]
    public void GetParamInt_Configuration_BoardCount()
    {
        _mockHal.Configuration.BoardCount = 2;

        int result = _mockHal.GetParamInt(
            MultiCamNative.MC_CONFIGURATION,
            MultiCamNative.PN_BoardCount,
            out int value);

        Assert.Equal(MultiCamNative.MC_OK, result);
        Assert.Equal(2, value);
    }

    [Fact]
    public void GetParamStr_Configuration_DriverVersion()
    {
        _mockHal.Configuration.DriverVersion = "Test 1.0";

        int result = _mockHal.GetParamStr(
            MultiCamNative.MC_CONFIGURATION,
            MultiCamNative.PN_DriverVersion,
            out string value);

        Assert.Equal(MultiCamNative.MC_OK, result);
        Assert.Equal("Test 1.0", value);
    }

    [Fact]
    public void GetParamInt_Instance_ReturnsDefaultImageWidth()
    {
        _mockHal.Configuration.DefaultImageWidth = 1280;
        _mockHal.Create(MultiCamNative.MC_CHANNEL, out uint handle);

        int result = _mockHal.GetParamInt(handle, MultiCamNative.PN_ImageSizeX, out int value);

        Assert.Equal(MultiCamNative.MC_OK, result);
        Assert.Equal(1280, value);
    }

    [Fact]
    public void GetParamFloat_Instance_ReturnsDefaultExposure()
    {
        _mockHal.Create(MultiCamNative.MC_CHANNEL, out uint handle);

        int result = _mockHal.GetParamFloat(handle, MultiCamNative.PN_Expose_us, out double value);

        Assert.Equal(MultiCamNative.MC_OK, result);
        Assert.Equal(10000.0, value);
    }

    [Fact]
    public void SetParamInt_Instance_StoresValue()
    {
        _mockHal.Create(MultiCamNative.MC_CHANNEL, out uint handle);

        _mockHal.SetParamInt(handle, MultiCamNative.PN_SurfaceCount, 16);
        _mockHal.GetParamInt(handle, MultiCamNative.PN_SurfaceCount, out int value);

        Assert.Equal(16, value);
    }

    [Fact]
    public void SetParamFloat_Instance_StoresValue()
    {
        _mockHal.Create(MultiCamNative.MC_CHANNEL, out uint handle);

        _mockHal.SetParamFloat(handle, MultiCamNative.PN_Expose_us, 5000.0);
        _mockHal.GetParamFloat(handle, MultiCamNative.PN_Expose_us, out double value);

        Assert.Equal(5000.0, value);
    }

    [Fact]
    public void SetParamStr_Instance_StoresValue()
    {
        _mockHal.Create(MultiCamNative.MC_CHANNEL, out uint handle);

        _mockHal.SetParamStr(handle, MultiCamNative.PN_TrigMode, MultiCamNative.MC_TrigMode_SOFT_STR);
        _mockHal.GetParamStr(handle, MultiCamNative.PN_TrigMode, out string value);

        Assert.Equal(MultiCamNative.MC_TrigMode_SOFT_STR, value);
    }

    [Fact]
    public void GetParamInt_InvalidInstance_ReturnsInvalidHandle()
    {
        int result = _mockHal.GetParamInt(9999, MultiCamNative.PN_ImageSizeX, out _);

        Assert.Equal((int)McStatus.MC_INVALID_HANDLE, result);
    }

    #endregion

    #region Board Parameters

    [Fact]
    public void GetParamStr_Board_BoardName()
    {
        _mockHal.Configuration.BoardNames = new List<string> { "TestBoard" };

        int result = _mockHal.GetParamStr(
            MultiCamNative.MC_BOARD,
            MultiCamNative.PN_BoardName,
            out string value);

        Assert.Equal(MultiCamNative.MC_OK, result);
        Assert.Equal("TestBoard", value);
    }

    [Fact]
    public void GetParamStr_Board_BoardType()
    {
        _mockHal.Configuration.BoardTypes = new List<string> { "PC1622" };

        int result = _mockHal.GetParamStr(
            MultiCamNative.MC_BOARD,
            MultiCamNative.PN_BoardType,
            out string value);

        Assert.Equal(MultiCamNative.MC_OK, result);
        Assert.Equal("PC1622", value);
    }

    [Fact]
    public void GetParamStr_Board_SerialNumber()
    {
        int result = _mockHal.GetParamStr(
            MultiCamNative.MC_BOARD,
            MultiCamNative.PN_SerialNumber,
            out string value);

        Assert.Equal(MultiCamNative.MC_OK, result);
        Assert.StartsWith("MOCK", value);
    }

    #endregion

    #region Signaling

    [Fact]
    public void RegisterCallback_StoresCallbackInfo()
    {
        _mockHal.Create(MultiCamNative.MC_CHANNEL, out uint handle);

        int result = _mockHal.RegisterCallback(handle, new IntPtr(12345), new IntPtr(67890));

        Assert.Equal(MultiCamNative.MC_OK, result);
        Assert.Equal(1, _mockHal.CallLog.RegisterCallbackCalls);
    }

    [Fact]
    public void WaitSignal_ReturnsSignalInfo()
    {
        _mockHal.Create(MultiCamNative.MC_CHANNEL, out uint handle);

        int result = _mockHal.WaitSignal(handle, (int)McSignal.MC_SIG_ANY, 1000, out var info);

        Assert.Equal(MultiCamNative.MC_OK, result);
        Assert.Equal(handle, info.Instance);
    }

    [Fact]
    public void WaitSignal_WhenConfiguredToTimeout_ReturnsTimeout()
    {
        _mockHal.Configuration.WaitSignalFailure = (int)McStatus.MC_TIMEOUT;
        _mockHal.Create(MultiCamNative.MC_CHANNEL, out uint handle);

        int result = _mockHal.WaitSignal(handle, (int)McSignal.MC_SIG_ANY, 1000, out _);

        Assert.Equal((int)McStatus.MC_TIMEOUT, result);
    }

    #endregion

    #region Special Parameter Handling

    [Fact]
    public void SetParamStr_ChannelState_TracksAcquisitionState()
    {
        _mockHal.Create(MultiCamNative.MC_CHANNEL, out uint handle);

        _mockHal.SetParamStr(handle, MultiCamNative.PN_ChannelState, MultiCamNative.MC_ChannelState_ACTIVE_STR);
        Assert.True(_mockHal.CallLog.AcquisitionStarted);

        _mockHal.SetParamStr(handle, MultiCamNative.PN_ChannelState, MultiCamNative.MC_ChannelState_IDLE_STR);
        Assert.True(_mockHal.CallLog.AcquisitionStopped);
    }

    [Fact]
    public void SetParamStr_ForceTrig_IncrementsTriggerCount()
    {
        _mockHal.Create(MultiCamNative.MC_CHANNEL, out uint handle);

        _mockHal.SetParamStr(handle, MultiCamNative.PN_ForceTrig, MultiCamNative.MC_ForceTrig_STR);
        _mockHal.SetParamStr(handle, MultiCamNative.PN_ForceTrig, MultiCamNative.MC_ForceTrig_STR);
        _mockHal.SetParamStr(handle, MultiCamNative.PN_ForceTrig, MultiCamNative.MC_ForceTrig_STR);

        Assert.Equal(3, _mockHal.CallLog.SoftwareTriggerCount);
    }

    [Fact]
    public void SetParamStr_BlackCalibration_TracksCalibration()
    {
        _mockHal.Create(MultiCamNative.MC_CHANNEL, out uint handle);

        _mockHal.SetParamStr(handle, MultiCamNative.PN_BlackCalibration, "ON");

        Assert.True(_mockHal.CallLog.BlackCalibrationPerformed);
    }

    [Fact]
    public void SetParamStr_WhiteCalibration_TracksCalibration()
    {
        _mockHal.Create(MultiCamNative.MC_CHANNEL, out uint handle);

        _mockHal.SetParamStr(handle, MultiCamNative.PN_WhiteCalibration, "ON");

        Assert.True(_mockHal.CallLog.WhiteCalibrationPerformed);
    }

    [Fact]
    public void SetParamStr_BalanceWhiteAuto_AdjustsRatios()
    {
        _mockHal.Create(MultiCamNative.MC_CHANNEL, out uint handle);

        _mockHal.SetParamStr(handle, MultiCamNative.PN_BalanceWhiteAuto, "ONCE");

        Assert.True(_mockHal.CallLog.WhiteBalancePerformed);

        // Verify ratios were adjusted
        _mockHal.GetParamFloat(handle, MultiCamNative.PN_BalanceRatioRed, out double red);
        _mockHal.GetParamFloat(handle, MultiCamNative.PN_BalanceRatioBlue, out double blue);

        Assert.NotEqual(1.0, red);
        Assert.NotEqual(1.0, blue);
    }

    #endregion

    #region Reset

    [Fact]
    public void Reset_ClearsAllState()
    {
        _mockHal.OpenDriver(null);
        _mockHal.Create(MultiCamNative.MC_CHANNEL, out _);
        _mockHal.Create(MultiCamNative.MC_CHANNEL, out _);

        _mockHal.Reset();

        Assert.Equal(0, _mockHal.CallLog.OpenDriverCalls);
        Assert.Equal(0, _mockHal.CallLog.CreateCalls);
    }

    [Fact]
    public void Reset_AllowsNewInstancesStartingFromOne()
    {
        _mockHal.Create(MultiCamNative.MC_CHANNEL, out uint handle1);
        Assert.Equal(1u, handle1);

        _mockHal.Reset();

        _mockHal.Create(MultiCamNative.MC_CHANNEL, out uint handle2);
        Assert.Equal(1u, handle2);
    }

    #endregion
}
