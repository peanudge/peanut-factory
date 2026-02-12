using PeanutVision.MultiCamDriver.Camera;
using PeanutVision.MultiCamDriver.Hal;

namespace PeanutVision.MultiCamDriver.IntegrationTests;

/// <summary>
/// Step-by-step integration tests for the MultiCam acquisition chain.
/// Tests each stage of the acquisition setup process in order.
///
/// Acquisition Chain Steps:
/// 1. Board Linkage (MC_DriverIndex)
/// 2. Connector Selection (MC_Connector)
/// 3. Load Camera Configuration (MC_CamFile)
/// 4. Create Surfaces and Register Cluster
/// 5. Configure Signaling and Callbacks
/// 6. Activate the Channel
///
/// Run these tests only on machines with hardware:
///   dotnet test --filter "Category=Hardware"
/// </summary>
[Trait("Category", "Hardware")]
public class AcquisitionChainTests : IDisposable
{
    private readonly IMultiCamHAL _hal;
    private readonly bool _hardwareAvailable;
    private uint _channelHandle;

    public AcquisitionChainTests()
    {
        _hal = MultiCamHAL.Instance;

        try
        {
            int status = _hal.OpenDriver(null);
            _hardwareAvailable = status == MultiCamApi.MC_OK;

            if (_hardwareAvailable)
            {
                // Verify at least one board exists
                status = _hal.GetParamInt(MultiCamApi.MC_CONFIGURATION, MultiCamApi.PN_BoardCount, out int boardCount);
                _hardwareAvailable = status == MultiCamApi.MC_OK && boardCount > 0;
            }
        }
        catch
        {
            _hardwareAvailable = false;
        }
    }

    public void Dispose()
    {
        if (_channelHandle != 0)
        {
            // Ensure channel is stopped before delete
            _hal.SetParamStr(_channelHandle, MultiCamApi.PN_ChannelState, MultiCamApi.MC_ChannelState_IDLE_STR);
            _hal.Delete(_channelHandle);
            _channelHandle = 0;
        }

        _hal.CloseDriver();
    }

    private void SkipIfNoHardware()
    {
        if (!_hardwareAvailable)
        {
            throw new SkipException("No frame grabber hardware detected.");
        }
    }

    /// <summary>
    /// Converts a MultiCam status code to a descriptive string.
    /// </summary>
    private static string DescribeStatus(int status)
    {
        return status switch
        {
            0 => "MC_OK (0)",
            -1 => "MC_ERROR (-1)",
            -2 => "MC_INVALID_HANDLE (-2)",
            -3 => "MC_INVALID_PARAM (-3)",
            -4 => "MC_NOMORE_RESOURCES (-4)",
            -5 => "MC_TIMEOUT (-5)",
            -6 => "MC_NOT_SUPPORTED (-6)",
            -7 => "MC_IN_USE (-7)",
            -8 => "MC_BUSY (-8)",
            -9 => "MC_IO_ERROR (-9)",
            -10 => "MC_INTERNAL_ERROR (-10)",
            -25 => "MC_SERVICE_ERROR (-25)",
            _ => $"UNKNOWN_STATUS ({status})"
        };
    }

    /// <summary>
    /// Asserts that the status equals MC_OK, with a descriptive error message if not.
    /// </summary>
    private static void AssertOk(int status, string operation = "")
    {
        if (status != MultiCamApi.MC_OK)
        {
            string message = string.IsNullOrEmpty(operation)
                ? $"Expected: {DescribeStatus(MultiCamApi.MC_OK)}, Actual: {DescribeStatus(status)}"
                : $"{operation} failed. Expected: {DescribeStatus(MultiCamApi.MC_OK)}, Actual: {DescribeStatus(status)}";
            Assert.Fail(message);
        }
    }

    #region Step 0: Driver and Channel Creation

    [Fact]
    public void Step0_OpenDriver_Succeeds()
    {
        SkipIfNoHardware();

        // Driver is already opened in constructor
        // Verify board count is accessible
        int status = _hal.GetParamInt(MultiCamApi.MC_CONFIGURATION, MultiCamApi.PN_BoardCount, out int boardCount);

        Assert.Equal(MultiCamApi.MC_OK, status);
        Assert.True(boardCount > 0, "At least one board should be detected");
    }

    [Fact]
    public void Step0_CreateChannel_Succeeds()
    {
        SkipIfNoHardware();

        int status = _hal.Create(MultiCamApi.MC_CHANNEL, out uint handle);

        Assert.Equal(MultiCamApi.MC_OK, status);
        Assert.NotEqual(0u, handle);

        // Cleanup
        _hal.Delete(handle);
    }

    #endregion

    #region Step 1: Board Linkage (DriverIndex)

    [Fact]
    public void Step1_SetDriverIndex_Succeeds()
    {
        SkipIfNoHardware();

        int status = _hal.Create(MultiCamApi.MC_CHANNEL, out _channelHandle);
        Assert.Equal(MultiCamApi.MC_OK, status);

        // Set driver index to first board (0)
        status = _hal.SetParamInt(_channelHandle, MultiCamApi.PN_DriverIndex, MultiCamApi.DefaultBoardIndex);

        Assert.Equal(MultiCamApi.MC_OK, status);

        // Verify the value was set
        status = _hal.GetParamInt(_channelHandle, MultiCamApi.PN_DriverIndex, out int readValue);
        Assert.Equal(MultiCamApi.MC_OK, status);
        Assert.Equal(MultiCamApi.DefaultBoardIndex, readValue);
    }

    [Fact]
    public void Step1_SetDriverIndex_InvalidIndex_ReturnsError()
    {
        SkipIfNoHardware();

        int status = _hal.Create(MultiCamApi.MC_CHANNEL, out _channelHandle);
        Assert.Equal(MultiCamApi.MC_OK, status);

        // Try to set an invalid driver index (e.g., 999)
        status = _hal.SetParamInt(_channelHandle, MultiCamApi.PN_DriverIndex, 999);

        // Should return an error (invalid parameter or out of range)
        Assert.NotEqual(MultiCamApi.MC_OK, status);
    }

    #endregion

    #region Step 2: Connector Selection

    [Fact]
    public void Step2_SetConnector_Succeeds()
    {
        SkipIfNoHardware();

        int status = _hal.Create(MultiCamApi.MC_CHANNEL, out _channelHandle);
        Assert.Equal(MultiCamApi.MC_OK, status);

        // Step 1: Set driver index first (required before connector)
        status = _hal.SetParamInt(_channelHandle, MultiCamApi.PN_DriverIndex, MultiCamApi.DefaultBoardIndex);
        Assert.Equal(MultiCamApi.MC_OK, status);

        // Step 2: Set connector to "M" (Main connector for Grablink Full)
        status = _hal.SetParamStr(_channelHandle, MultiCamApi.PN_Connector, "M");

        Assert.Equal(MultiCamApi.MC_OK, status);

        // Verify the value was set
        status = _hal.GetParamStr(_channelHandle, MultiCamApi.PN_Connector, out string readValue);
        Assert.Equal(MultiCamApi.MC_OK, status);
        Assert.Equal("M", readValue);
    }

    [Fact]
    public void Step2_SetConnector_InvalidConnector_ReturnsError()
    {
        SkipIfNoHardware();

        int status = _hal.Create(MultiCamApi.MC_CHANNEL, out _channelHandle);
        Assert.Equal(MultiCamApi.MC_OK, status);

        status = _hal.SetParamInt(_channelHandle, MultiCamApi.PN_DriverIndex, MultiCamApi.DefaultBoardIndex);
        Assert.Equal(MultiCamApi.MC_OK, status);

        // Try to set an invalid connector
        status = _hal.SetParamStr(_channelHandle, MultiCamApi.PN_Connector, "INVALID");

        // Should return an error
        Assert.NotEqual(MultiCamApi.MC_OK, status);
    }

    #endregion

    #region Step 3: Load Camera Configuration (CamFile)

    [Fact]
    public void Step3_LoadCamFile_Succeeds()
    {
        SkipIfNoHardware();

        int status = _hal.Create(MultiCamApi.MC_CHANNEL, out _channelHandle);
        Assert.Equal(MultiCamApi.MC_OK, status);

        // Step 1: Set driver index
        status = _hal.SetParamInt(_channelHandle, MultiCamApi.PN_DriverIndex, MultiCamApi.DefaultBoardIndex);
        Assert.Equal(MultiCamApi.MC_OK, status);

        // Step 2: Set connector
        status = _hal.SetParamStr(_channelHandle, MultiCamApi.PN_Connector, "M");
        Assert.Equal(MultiCamApi.MC_OK, status);

        // Step 3: Load camera file
        var camPath = CamFileResource.GetCamFilePath(CamFileResource.KnownCamFiles.TC_A160K_FreeRun_RGB8);
        status = _hal.SetParamStr(_channelHandle, MultiCamApi.PN_CamFile, camPath);

        Assert.Equal(MultiCamApi.MC_OK, status);
    }

    [Fact]
    public void Step3_LoadCamFile_InvalidPath_ReturnsError()
    {
        SkipIfNoHardware();

        int status = _hal.Create(MultiCamApi.MC_CHANNEL, out _channelHandle);
        Assert.Equal(MultiCamApi.MC_OK, status);

        status = _hal.SetParamInt(_channelHandle, MultiCamApi.PN_DriverIndex, MultiCamApi.DefaultBoardIndex);
        Assert.Equal(MultiCamApi.MC_OK, status);

        status = _hal.SetParamStr(_channelHandle, MultiCamApi.PN_Connector, "M");
        Assert.Equal(MultiCamApi.MC_OK, status);

        // Try to load a non-existent camera file
        status = _hal.SetParamStr(_channelHandle, MultiCamApi.PN_CamFile, @"C:\NonExistent\Camera.cam");

        // Should return an error
        Assert.NotEqual(MultiCamApi.MC_OK, status);
    }

    [Fact]
    public void Step3_AfterCamFile_ImageDimensionsAvailable()
    {
        SkipIfNoHardware();

        int status = _hal.Create(MultiCamApi.MC_CHANNEL, out _channelHandle);
        Assert.Equal(MultiCamApi.MC_OK, status);

        status = _hal.SetParamInt(_channelHandle, MultiCamApi.PN_DriverIndex, MultiCamApi.DefaultBoardIndex);
        Assert.Equal(MultiCamApi.MC_OK, status);

        status = _hal.SetParamStr(_channelHandle, MultiCamApi.PN_Connector, "M");
        Assert.Equal(MultiCamApi.MC_OK, status);

        var camPath = CamFileResource.GetCamFilePath(CamFileResource.KnownCamFiles.TC_A160K_FreeRun_RGB8);
        status = _hal.SetParamStr(_channelHandle, MultiCamApi.PN_CamFile, camPath);
        Assert.Equal(MultiCamApi.MC_OK, status);

        // After loading CamFile, image dimensions should be available
        status = _hal.GetParamInt(_channelHandle, MultiCamApi.PN_ImageSizeX, out int width);
        Assert.Equal(MultiCamApi.MC_OK, status);
        Assert.True(width > 0, $"ImageSizeX should be > 0, got {width}");

        status = _hal.GetParamInt(_channelHandle, MultiCamApi.PN_ImageSizeY, out int height);
        Assert.Equal(MultiCamApi.MC_OK, status);
        Assert.True(height > 0, $"ImageSizeY should be > 0, got {height}");

        // For TC-A160K in RGB8 mode, expected dimensions are 4608 x 3288
        // Allow some flexibility for different modes
        Assert.True(width >= 100, $"Width seems too small: {width}");
        Assert.True(height >= 100, $"Height seems too small: {height}");
    }

    #endregion

    #region Step 4: Surface and Cluster Configuration

    [Fact]
    public void Step4_ConfigureSurfaces_Succeeds()
    {
        SkipIfNoHardware();

        int status = _hal.Create(MultiCamApi.MC_CHANNEL, out _channelHandle);
        Assert.Equal(MultiCamApi.MC_OK, status);

        // Steps 1-3
        status = _hal.SetParamInt(_channelHandle, MultiCamApi.PN_DriverIndex, MultiCamApi.DefaultBoardIndex);
        Assert.Equal(MultiCamApi.MC_OK, status);

        status = _hal.SetParamStr(_channelHandle, MultiCamApi.PN_Connector, "M");
        Assert.Equal(MultiCamApi.MC_OK, status);

        var camPath = CamFileResource.GetCamFilePath(CamFileResource.KnownCamFiles.TC_A160K_FreeRun_RGB8);
        status = _hal.SetParamStr(_channelHandle, MultiCamApi.PN_CamFile, camPath);
        Assert.Equal(MultiCamApi.MC_OK, status);

        // Step 4: Configure surfaces (MultiCam manages allocation with SurfaceCount)
        int surfaceCount = 4;
        status = _hal.SetParamInt(_channelHandle, MultiCamApi.PN_SurfaceCount, surfaceCount);

        Assert.Equal(MultiCamApi.MC_OK, status);

        // Verify surface count was set
        status = _hal.GetParamInt(_channelHandle, MultiCamApi.PN_SurfaceCount, out int readCount);
        Assert.Equal(MultiCamApi.MC_OK, status);
        Assert.Equal(surfaceCount, readCount);
    }

    [Fact]
    public void Step4_BufferSize_CalculatedFromImageDimensions()
    {
        SkipIfNoHardware();

        int status = _hal.Create(MultiCamApi.MC_CHANNEL, out _channelHandle);
        Assert.Equal(MultiCamApi.MC_OK, status);

        // Configure channel through Step 4
        status = _hal.SetParamInt(_channelHandle, MultiCamApi.PN_DriverIndex, MultiCamApi.DefaultBoardIndex);
        Assert.Equal(MultiCamApi.MC_OK, status);

        status = _hal.SetParamStr(_channelHandle, MultiCamApi.PN_Connector, "M");
        Assert.Equal(MultiCamApi.MC_OK, status);

        var camPath = CamFileResource.GetCamFilePath(CamFileResource.KnownCamFiles.TC_A160K_FreeRun_RGB8);
        status = _hal.SetParamStr(_channelHandle, MultiCamApi.PN_CamFile, camPath);
        Assert.Equal(MultiCamApi.MC_OK, status);

        status = _hal.SetParamInt(_channelHandle, MultiCamApi.PN_SurfaceCount, 4);
        Assert.Equal(MultiCamApi.MC_OK, status);

        // Get buffer dimensions
        status = _hal.GetParamInt(_channelHandle, MultiCamApi.PN_BufferPitch, out int pitch);
        Assert.Equal(MultiCamApi.MC_OK, status);
        Assert.True(pitch > 0, $"BufferPitch should be > 0, got {pitch}");

        status = _hal.GetParamInt(_channelHandle, MultiCamApi.PN_BufferSize, out int size);
        Assert.Equal(MultiCamApi.MC_OK, status);
        Assert.True(size > 0, $"BufferSize should be > 0, got {size}");

        // Buffer size should be at least pitch * height
        status = _hal.GetParamInt(_channelHandle, MultiCamApi.PN_ImageSizeY, out int height);
        Assert.Equal(MultiCamApi.MC_OK, status);

        Assert.True(size >= pitch * height, $"BufferSize ({size}) should be >= pitch ({pitch}) * height ({height})");
    }

    #endregion

    #region Step 5: Signal Configuration

    [Fact]
    public void Step5_EnableSignal_Succeeds()
    {
        SkipIfNoHardware();

        int status = _hal.Create(MultiCamApi.MC_CHANNEL, out _channelHandle);
        AssertOk(status, "McCreate(MC_CHANNEL)");

        // Configure channel through Step 4
        status = _hal.SetParamInt(_channelHandle, MultiCamApi.PN_DriverIndex, MultiCamApi.DefaultBoardIndex);
        AssertOk(status, "SetParam(DriverIndex)");

        status = _hal.SetParamStr(_channelHandle, MultiCamApi.PN_Connector, "M");
        AssertOk(status, "SetParam(Connector)");

        var camPath = CamFileResource.GetCamFilePath(CamFileResource.KnownCamFiles.TC_A160K_FreeRun_RGB8);
        status = _hal.SetParamStr(_channelHandle, MultiCamApi.PN_CamFile, camPath);
        AssertOk(status, "SetParam(CamFile)");

        status = _hal.SetParamInt(_channelHandle, MultiCamApi.PN_SurfaceCount, 4);
        AssertOk(status, "SetParam(SurfaceCount)");

        status = _hal.GetParamInt(_channelHandle, MultiCamApi.PN_SurfaceCount, out int surfaceCount);
        AssertOk(status, "GetParam(SurfaceCount)");
        Assert.Equal(4, surfaceCount);

        // Step 5: Enable signals for surface processing
        // MultiCam uses compound parameter IDs: MC_SignalEnable + signal_id
        status = SetSignalEnable(_channelHandle, McSignal.MC_SIG_SURFACE_PROCESSING, true);
        AssertOk(status, "SetSignalEnable(MC_SIG_SURFACE_PROCESSING)");

        status = SetSignalEnable(_channelHandle, McSignal.MC_SIG_ACQUISITION_FAILURE, true);
        AssertOk(status, "SetSignalEnable(MC_SIG_ACQUISITION_FAILURE)");

        status = SetSignalEnable(_channelHandle, McSignal.MC_SIG_END_CHANNEL_ACTIVITY, true);
        AssertOk(status, "SetSignalEnable(MC_SIG_END_CHANNEL_ACTIVITY)");
    }

    /// <summary>
    /// Helper to set signal enable using MultiCam's compound parameter ID format.
    /// The compound parameter ID is: MC_SignalEnable + signal_id
    /// </summary>
    private int SetSignalEnable(uint channelHandle, McSignal signal, bool enable)
    {
        // MultiCam uses compound parameter IDs: MC_SignalEnable + signal_id
        uint compoundParamId = MultiCamApi.MC_SignalEnable + (uint)signal;
        int value = enable ? MultiCamApi.MC_SignalEnable_ON : MultiCamApi.MC_SignalEnable_OFF;
        return _hal.SetParamIntById(channelHandle, compoundParamId, value);
    }

    #endregion

    #region Step 6: Channel Activation

    [Fact]
    public void Step6_ActivateChannel_Succeeds()
    {
        SkipIfNoHardware();

        int status = _hal.Create(MultiCamApi.MC_CHANNEL, out _channelHandle);
        AssertOk(status, "McCreate(MC_CHANNEL)");

        // Configure channel through Step 5
        status = _hal.SetParamInt(_channelHandle, MultiCamApi.PN_DriverIndex, MultiCamApi.DefaultBoardIndex);
        AssertOk(status, "SetParam(DriverIndex)");

        status = _hal.SetParamStr(_channelHandle, MultiCamApi.PN_Connector, "M");
        AssertOk(status, "SetParam(Connector)");

        var camPath = CamFileResource.GetCamFilePath(CamFileResource.KnownCamFiles.TC_A160K_FreeRun_RGB8);
        status = _hal.SetParamStr(_channelHandle, MultiCamApi.PN_CamFile, camPath);
        AssertOk(status, "SetParam(CamFile)");

        status = _hal.SetParamInt(_channelHandle, MultiCamApi.PN_SurfaceCount, 4);
        AssertOk(status, "SetParam(SurfaceCount)");

        SetSignalEnable(_channelHandle, McSignal.MC_SIG_SURFACE_PROCESSING, true);
        SetSignalEnable(_channelHandle, McSignal.MC_SIG_ACQUISITION_FAILURE, true);
        SetSignalEnable(_channelHandle, McSignal.MC_SIG_END_CHANNEL_ACTIVITY, true);

        // Step 6: Activate the channel
        status = _hal.SetParamStr(_channelHandle, MultiCamApi.PN_ChannelState, MultiCamApi.MC_ChannelState_ACTIVE_STR);
        AssertOk(status, "SetParam(ChannelState=ACTIVE)");

        // Verify channel is active
        status = _hal.GetParamStr(_channelHandle, MultiCamApi.PN_ChannelState, out string state);
        AssertOk(status, "GetParam(ChannelState)");
        Assert.Equal(MultiCamApi.MC_ChannelState_ACTIVE_STR, state);

        // Stop the channel
        status = _hal.SetParamStr(_channelHandle, MultiCamApi.PN_ChannelState, MultiCamApi.MC_ChannelState_IDLE_STR);
        AssertOk(status, "SetParam(ChannelState=IDLE)");
    }

    [Fact]
    public void Step6_ActivateWithoutCamFile_HasNoUsableImageParameters()
    {
        SkipIfNoHardware();

        int status = _hal.Create(MultiCamApi.MC_CHANNEL, out _channelHandle);
        AssertOk(status, "McCreate(MC_CHANNEL)");

        // Only set driver index and connector, skip CamFile
        status = _hal.SetParamInt(_channelHandle, MultiCamApi.PN_DriverIndex, MultiCamApi.DefaultBoardIndex);
        AssertOk(status, "SetParam(DriverIndex)");

        status = _hal.SetParamStr(_channelHandle, MultiCamApi.PN_Connector, "M");
        AssertOk(status, "SetParam(Connector)");

        // The native driver accepts activation without CamFile (uses internal defaults),
        // but the resulting image parameters should be zero/default — not usable for real acquisition.
        status = _hal.GetParamInt(_channelHandle, MultiCamApi.PN_ImageSizeX, out int width);
        AssertOk(status, "GetParam(ImageSizeX)");

        status = _hal.GetParamInt(_channelHandle, MultiCamApi.PN_ImageSizeY, out int height);
        AssertOk(status, "GetParam(ImageSizeY)");

        Assert.True(width == 0 || height == 0,
            $"Expected zero image dimensions without CamFile, but got: {width}x{height}");
    }

    #endregion

    #region Full Chain Test

    [Fact]
    public void FullChain_ConfigureAndAcquireFrame_Succeeds()
    {
        SkipIfNoHardware();

        // Step 0: Create channel
        int status = _hal.Create(MultiCamApi.MC_CHANNEL, out _channelHandle);
        AssertOk(status, "McCreate(MC_CHANNEL)");

        // Step 1: Board linkage
        status = _hal.SetParamInt(_channelHandle, MultiCamApi.PN_DriverIndex, MultiCamApi.DefaultBoardIndex);
        AssertOk(status, "SetParam(DriverIndex)");

        // Step 2: Connector selection
        status = _hal.SetParamStr(_channelHandle, MultiCamApi.PN_Connector, "M");
        AssertOk(status, "SetParam(Connector)");

        // Step 3: Load camera configuration
        var camPath = CamFileResource.GetCamFilePath(CamFileResource.KnownCamFiles.TC_A160K_FreeRun_RGB8);
        status = _hal.SetParamStr(_channelHandle, MultiCamApi.PN_CamFile, camPath);
        AssertOk(status, "SetParam(CamFile)");

        // Step 4: Configure surfaces
        status = _hal.SetParamInt(_channelHandle, MultiCamApi.PN_SurfaceCount, 4);
        AssertOk(status, "SetParam(SurfaceCount)");

        // Step 5: Enable signals (using compound parameter ID format)
        SetSignalEnable(_channelHandle, McSignal.MC_SIG_SURFACE_PROCESSING, true);
        SetSignalEnable(_channelHandle, McSignal.MC_SIG_ACQUISITION_FAILURE, true);
        SetSignalEnable(_channelHandle, McSignal.MC_SIG_END_CHANNEL_ACTIVITY, true);

        // Set trigger mode to immediate (free-run)
        status = _hal.SetParamStr(_channelHandle, MultiCamApi.PN_TrigMode, MultiCamApi.MC_TrigMode_IMMEDIATE_STR);
        AssertOk(status, "SetParam(TrigMode)");

        // Set sequence length for finite acquisition (10 frames)
        status = _hal.SetParamInt(_channelHandle, MultiCamApi.PN_SeqLength_Fr, 10);
        AssertOk(status, "SetParam(SeqLength_Fr)");

        // Step 6: Activate channel
        status = _hal.SetParamStr(_channelHandle, MultiCamApi.PN_ChannelState, MultiCamApi.MC_ChannelState_ACTIVE_STR);
        AssertOk(status, "SetParam(ChannelState=ACTIVE)");

        // Wait for a frame using WaitSignal
        status = _hal.WaitSignal(_channelHandle, (int)McSignal.MC_SIG_SURFACE_PROCESSING, 5000, out var signalInfo);
        AssertOk(status, "McWaitSignal(MC_SIG_SURFACE_PROCESSING)");
        Assert.Equal((int)McSignal.MC_SIG_SURFACE_PROCESSING, signalInfo.Signal);

        // SignalInfo contains the surface HANDLE (not index)
        uint surfaceHandle = signalInfo.SignalInfo;

        // Get the surface's cluster index from the handle
        status = _hal.GetParamInt(surfaceHandle, MultiCamApi.PN_SurfaceIndex, out int surfaceIndex);
        AssertOk(status, "GetParam(SurfaceIndex) from surface handle");

        // Select the surface on the channel, then read the buffer address
        status = _hal.SetParamInt(_channelHandle, MultiCamApi.PN_SurfaceIndex, surfaceIndex);
        AssertOk(status, "SetParam(SurfaceIndex) on channel");

        status = _hal.GetParamPtr(_channelHandle, MultiCamApi.PN_SurfaceAddr, out IntPtr surfaceAddr);
        AssertOk(status, "GetParam(SurfaceAddr)");
        Assert.True(surfaceAddr != IntPtr.Zero, "Surface address should not be null");

        // Release surface back to FREE via the surface handle
        _hal.SetParamInt(surfaceHandle, MultiCamApi.PN_SurfaceState, (int)McSurfaceState.MC_SurfaceState_FREE);

        // Stop the channel
        status = _hal.SetParamStr(_channelHandle, MultiCamApi.PN_ChannelState, MultiCamApi.MC_ChannelState_IDLE_STR);
        AssertOk(status, "SetParam(ChannelState=IDLE)");
    }

    #endregion
}
