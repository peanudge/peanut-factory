using PeanutVision.MultiCamDriver.Hal;

namespace PeanutVision.MultiCamDriver.Tests;

public class GrabServiceTests
{
    private readonly MockMultiCamHAL _mockHal;

    public GrabServiceTests()
    {
        _mockHal = new MockMultiCamHAL();
    }

    [Fact]
    public void Initialize_OpensDriver()
    {
        using var service = new GrabService(_mockHal);

        service.Initialize();

        Assert.True(service.IsInitialized);
        Assert.Equal(1, _mockHal.CallLog.OpenDriverCalls);
    }

    [Fact]
    public void Initialize_QueriesBoardCount()
    {
        _mockHal.Configuration.BoardCount = 2;
        using var service = new GrabService(_mockHal);

        service.Initialize();

        Assert.Equal(2, service.BoardCount);
    }

    [Fact]
    public void Initialize_CalledMultipleTimes_OnlyOpensOnce()
    {
        using var service = new GrabService(_mockHal);

        service.Initialize();
        service.Initialize();
        service.Initialize();

        Assert.Equal(1, _mockHal.CallLog.OpenDriverCalls);
    }

    [Fact]
    public void Initialize_WhenDriverFails_ThrowsException()
    {
        _mockHal.Configuration.OpenDriverFailure = (int)McStatus.MC_ERROR;
        using var service = new GrabService(_mockHal);

        var ex = Assert.Throws<MultiCamException>(() => service.Initialize());

        Assert.Equal((int)McStatus.MC_ERROR, ex.StatusCode);
    }

    [Fact]
    public void CreateChannel_WhenNotInitialized_ThrowsException()
    {
        using var service = new GrabService(_mockHal);

        Assert.Throws<InvalidOperationException>(() =>
            service.CreateChannel("test.cam"));
    }

    [Fact]
    public void CreateChannel_WhenInitialized_CreatesChannel()
    {
        using var service = new GrabService(_mockHal);
        service.Initialize();

        using var channel = service.CreateChannel("test.cam");

        Assert.NotNull(channel);
        Assert.True(channel.Handle > 0);
    }

    [Fact]
    public void CreateChannel_SetsCorrectParameters()
    {
        using var service = new GrabService(_mockHal);
        service.Initialize();

        using var channel = service.CreateChannel(new GrabChannelOptions
        {
            CamFilePath = "camera.cam",
            DriverIndex = 1,
            Connector = "A",
            SurfaceCount = 8,
            TriggerMode = McTrigMode.MC_TrigMode_SOFT
        });

        Assert.True(_mockHal.CallLog.LastSetParams.ContainsKey(MultiCamApi.PN_CamFile));
        Assert.Equal("camera.cam", _mockHal.CallLog.LastSetParams[MultiCamApi.PN_CamFile]);
    }

    [Fact]
    public void GetBoardInfo_ReturnsBoardInformation()
    {
        _mockHal.Configuration.BoardCount = 1;
        _mockHal.Configuration.BoardNames = new List<string> { "Grablink Full" };
        _mockHal.Configuration.BoardTypes = new List<string> { "PC1622" };

        using var service = new GrabService(_mockHal);
        service.Initialize();

        var info = service.GetBoardInfo(0);

        Assert.Equal(0, info.Index);
        Assert.Equal("Grablink Full", info.BoardName);
        Assert.Equal("PC1622", info.BoardType);
    }

    [Fact]
    public void GetBoardInfo_InvalidIndex_ThrowsException()
    {
        _mockHal.Configuration.BoardCount = 1;
        using var service = new GrabService(_mockHal);
        service.Initialize();

        Assert.Throws<ArgumentOutOfRangeException>(() => service.GetBoardInfo(5));
    }

    [Fact]
    public void Dispose_ClosesDriver()
    {
        var service = new GrabService(_mockHal);
        service.Initialize();

        service.Dispose();

        Assert.Equal(1, _mockHal.CallLog.CloseDriverCalls);
    }

    [Fact]
    public void Dispose_DisposesAllChannels()
    {
        using var service = new GrabService(_mockHal);
        service.Initialize();

        var channel1 = service.CreateChannel("test1.cam");
        var channel2 = service.CreateChannel("test2.cam");

        service.Dispose();

        // Channels should be disposed (Delete called)
        Assert.True(_mockHal.CallLog.DeleteCalls >= 2);
    }

    [Fact]
    public void Dispose_CalledMultipleTimes_IsSafe()
    {
        var service = new GrabService(_mockHal);
        service.Initialize();

        service.Dispose();
        service.Dispose();
        service.Dispose();

        Assert.Equal(1, _mockHal.CallLog.CloseDriverCalls);
    }

    [Fact]
    public void AfterDispose_Initialize_ThrowsException()
    {
        var service = new GrabService(_mockHal);
        service.Dispose();

        Assert.Throws<ObjectDisposedException>(() => service.Initialize());
    }

    [Fact]
    public void AfterDispose_CreateChannel_ThrowsException()
    {
        var service = new GrabService(_mockHal);
        service.Initialize();
        service.Dispose();

        Assert.Throws<ObjectDisposedException>(() => service.CreateChannel("test.cam"));
    }
}
