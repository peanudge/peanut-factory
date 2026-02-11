using System.Net;
using PeanutVision.Api.Tests.Infrastructure;

namespace PeanutVision.Api.Tests.Specs.Calibration;

public class CalibrationOperationsSpec : IClassFixture<PeanutVisionApiFactory>, IAsyncLifetime
{
    private readonly PeanutVisionApiFactory _factory;
    private readonly HttpClient _client;

    public CalibrationOperationsSpec(PeanutVisionApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await _client.PostAsync("/api/acquisition/stop", null);
    }

    // --- No active channel (409 Conflict) ---

    [Fact]
    public async Task BlackCalibration_without_active_channel_returns_409()
    {
        var response = await _client.PostAsync("/api/calibration/black", null);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task WhiteCalibration_without_active_channel_returns_409()
    {
        var response = await _client.PostAsync("/api/calibration/white", null);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task WhiteBalance_without_active_channel_returns_409()
    {
        var response = await _client.PostAsync("/api/calibration/white-balance", null);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Ffc_without_active_channel_returns_409()
    {
        var response = await _client.PostJsonAsync("/api/calibration/ffc",
            new { enable = true });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    // --- With active channel ---

    [Fact]
    public async Task BlackCalibration_with_active_channel_returns_ok()
    {
        await _client.PostJsonAsync("/api/acquisition/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8" });
        _factory.ResetMockState();

        var response = await _client.PostAsync("/api/calibration/black", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(_factory.MockHal.CallLog.BlackCalibrationPerformed);
    }

    [Fact]
    public async Task WhiteCalibration_with_active_channel_returns_ok()
    {
        await _client.PostJsonAsync("/api/acquisition/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8" });
        _factory.ResetMockState();

        var response = await _client.PostAsync("/api/calibration/white", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(_factory.MockHal.CallLog.WhiteCalibrationPerformed);
    }

    [Fact]
    public async Task WhiteBalance_with_active_channel_returns_ok()
    {
        await _client.PostJsonAsync("/api/acquisition/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8" });
        _factory.ResetMockState();

        var response = await _client.PostAsync("/api/calibration/white-balance", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(_factory.MockHal.CallLog.WhiteBalancePerformed);
    }

    [Fact]
    public async Task Ffc_enable_with_active_channel_returns_ok()
    {
        await _client.PostJsonAsync("/api/acquisition/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8" });

        var response = await _client.PostJsonAsync("/api/calibration/ffc",
            new { enable = true });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = await response.ReadJsonDocumentAsync();
        Assert.Contains("enabled", doc.RootElement.GetProperty("message").GetString());
    }

    [Fact]
    public async Task Ffc_disable_with_active_channel_returns_ok()
    {
        await _client.PostJsonAsync("/api/acquisition/start",
            new { profileId = "crevis-tc-a160k-freerun-rgb8" });

        var response = await _client.PostJsonAsync("/api/calibration/ffc",
            new { enable = false });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = await response.ReadJsonDocumentAsync();
        Assert.Contains("disabled", doc.RootElement.GetProperty("message").GetString());
    }
}
