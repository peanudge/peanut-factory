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

        // Step 5: Enable signals for surface processing
        // MC_SIG_SURFACE_PROCESSING = 0x0003
        // MC_SIG_ACQUISITION_FAILURE = 0x0013
        // MC_SIG_END_CHANNEL_ACTIVITY = 0x0011
        int signals = (int)McSignal.MC_SIG_SURFACE_PROCESSING |
                     (int)McSignal.MC_SIG_ACQUISITION_FAILURE |
                     (int)McSignal.MC_SIG_END_CHANNEL_ACTIVITY;

        status = _hal.SetParamInt(_channelHandle, MultiCamApi.PN_SignalEnable, signals);

        Assert.Equal(MultiCamApi.MC_OK, status);
    }

    #endregion

    #region Step 6: Channel Activation

    [Fact]
    public void Step6_ActivateChannel_Succeeds()
    {
        SkipIfNoHardware();

        int status = _hal.Create(MultiCamApi.MC_CHANNEL, out _channelHandle);
        Assert.Equal(MultiCamApi.MC_OK, status);

        // Configure channel through Step 5
        status = _hal.SetParamInt(_channelHandle, MultiCamApi.PN_DriverIndex, MultiCamApi.DefaultBoardIndex);
        Assert.Equal(MultiCamApi.MC_OK, status);

        status = _hal.SetParamStr(_channelHandle, MultiCamApi.PN_Connector, "M");
        Assert.Equal(MultiCamApi.MC_OK, status);

        var camPath = CamFileResource.GetCamFilePath(CamFileResource.KnownCamFiles.TC_A160K_FreeRun_RGB8);
        status = _hal.SetParamStr(_channelHandle, MultiCamApi.PN_CamFile, camPath);
        Assert.Equal(MultiCamApi.MC_OK, status);

        status = _hal.SetParamInt(_channelHandle, MultiCamApi.PN_SurfaceCount, 4);
        Assert.Equal(MultiCamApi.MC_OK, status);

        int signals = (int)McSignal.MC_SIG_SURFACE_PROCESSING |
                     (int)McSignal.MC_SIG_ACQUISITION_FAILURE |
                     (int)McSignal.MC_SIG_END_CHANNEL_ACTIVITY;
        status = _hal.SetParamInt(_channelHandle, MultiCamApi.PN_SignalEnable, signals);
        Assert.Equal(MultiCamApi.MC_OK, status);

        // Step 6: Activate the channel
        status = _hal.SetParamStr(_channelHandle, MultiCamApi.PN_ChannelState, MultiCamApi.MC_ChannelState_ACTIVE_STR);

        Assert.Equal(MultiCamApi.MC_OK, status);

        // Verify channel is active
        status = _hal.GetParamStr(_channelHandle, MultiCamApi.PN_ChannelState, out string state);
        Assert.Equal(MultiCamApi.MC_OK, status);
        Assert.Equal(MultiCamApi.MC_ChannelState_ACTIVE_STR, state);

        // Stop the channel
        status = _hal.SetParamStr(_channelHandle, MultiCamApi.PN_ChannelState, MultiCamApi.MC_ChannelState_IDLE_STR);
        Assert.Equal(MultiCamApi.MC_OK, status);
    }

    [Fact]
    public void Step6_ActivateWithoutCamFile_ReturnsError()
    {
        SkipIfNoHardware();

        int status = _hal.Create(MultiCamApi.MC_CHANNEL, out _channelHandle);
        Assert.Equal(MultiCamApi.MC_OK, status);

        // Only set driver index and connector, skip CamFile
        status = _hal.SetParamInt(_channelHandle, MultiCamApi.PN_DriverIndex, MultiCamApi.DefaultBoardIndex);
        Assert.Equal(MultiCamApi.MC_OK, status);

        status = _hal.SetParamStr(_channelHandle, MultiCamApi.PN_Connector, "M");
        Assert.Equal(MultiCamApi.MC_OK, status);

        // Try to activate without loading CamFile
        status = _hal.SetParamStr(_channelHandle, MultiCamApi.PN_ChannelState, MultiCamApi.MC_ChannelState_ACTIVE_STR);

        // Should fail because camera configuration is incomplete
        Assert.NotEqual(MultiCamApi.MC_OK, status);
    }

    #endregion

    #region Full Chain Test

    [Fact]
    public void FullChain_ConfigureAndAcquireFrame_Succeeds()
    {
        SkipIfNoHardware();

        // Step 0: Create channel
        int status = _hal.Create(MultiCamApi.MC_CHANNEL, out _channelHandle);
        Assert.Equal(MultiCamApi.MC_OK, status);

        // Step 1: Board linkage
        status = _hal.SetParamInt(_channelHandle, MultiCamApi.PN_DriverIndex, MultiCamApi.DefaultBoardIndex);
        Assert.Equal(MultiCamApi.MC_OK, status);

        // Step 2: Connector selection
        status = _hal.SetParamStr(_channelHandle, MultiCamApi.PN_Connector, "M");
        Assert.Equal(MultiCamApi.MC_OK, status);

        // Step 3: Load camera configuration
        var camPath = CamFileResource.GetCamFilePath(CamFileResource.KnownCamFiles.TC_A160K_FreeRun_RGB8);
        status = _hal.SetParamStr(_channelHandle, MultiCamApi.PN_CamFile, camPath);
        Assert.Equal(MultiCamApi.MC_OK, status);

        // Step 4: Configure surfaces
        status = _hal.SetParamInt(_channelHandle, MultiCamApi.PN_SurfaceCount, 4);
        Assert.Equal(MultiCamApi.MC_OK, status);

        // Step 5: Enable signals
        int signals = (int)McSignal.MC_SIG_SURFACE_PROCESSING |
                     (int)McSignal.MC_SIG_ACQUISITION_FAILURE |
                     (int)McSignal.MC_SIG_END_CHANNEL_ACTIVITY;
        status = _hal.SetParamInt(_channelHandle, MultiCamApi.PN_SignalEnable, signals);
        Assert.Equal(MultiCamApi.MC_OK, status);

        // Set trigger mode to immediate (free-run)
        status = _hal.SetParamStr(_channelHandle, MultiCamApi.PN_TrigMode, MultiCamApi.MC_TrigMode_IMMEDIATE_STR);
        Assert.Equal(MultiCamApi.MC_OK, status);

        // Set sequence length for finite acquisition (10 frames)
        status = _hal.SetParamInt(_channelHandle, MultiCamApi.PN_SeqLength_Fr, 10);
        Assert.Equal(MultiCamApi.MC_OK, status);

        // Step 6: Activate channel
        status = _hal.SetParamStr(_channelHandle, MultiCamApi.PN_ChannelState, MultiCamApi.MC_ChannelState_ACTIVE_STR);
        Assert.Equal(MultiCamApi.MC_OK, status);

        // Wait for a frame using WaitSignal
        status = _hal.WaitSignal(_channelHandle, (int)McSignal.MC_SIG_SURFACE_PROCESSING, 5000, out var signalInfo);

        // Should receive a surface processing signal
        Assert.Equal(MultiCamApi.MC_OK, status);
        Assert.Equal((int)McSignal.MC_SIG_SURFACE_PROCESSING, signalInfo.Signal);

        // Query the surface address from the channel
        status = _hal.GetParamPtr(_channelHandle, MultiCamApi.PN_SurfaceAddr, out IntPtr surfaceAddr);
        Assert.Equal(MultiCamApi.MC_OK, status);
        Assert.True(surfaceAddr != IntPtr.Zero, "Surface address should not be null");

        // Stop the channel
        status = _hal.SetParamStr(_channelHandle, MultiCamApi.PN_ChannelState, MultiCamApi.MC_ChannelState_IDLE_STR);
        Assert.Equal(MultiCamApi.MC_OK, status);
    }

    #endregion
}
