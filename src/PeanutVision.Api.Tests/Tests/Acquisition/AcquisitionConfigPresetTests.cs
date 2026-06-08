using System.Net;
using System.Text.Json;
using PeanutVision.Api.Tests.Infrastructure;

namespace PeanutVision.Api.Tests.Tests.Acquisition;

public class AcquisitionConfigPresetTests : IClassFixture<PeanutVisionApiFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly string _testPresetName = $"test-preset-{Guid.NewGuid():N}";

    public AcquisitionConfigPresetTests(PeanutVisionApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        // Clean up test preset
        await _client.DeleteAsync($"/api/presets/{Uri.EscapeDataString(_testPresetName)}");
    }

    // ── CRUD ──

    [Fact]
    public async Task Save_preset_returns_ok_with_preset_data()
    {
        var response = await _client.PutJsonAsync("/api/presets", new
        {
            name = _testPresetName,
            profileId = "crevis-tc-a160k-freerun-rgb8.cam",
            frameCount = 10,
            intervalMs = 500,
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(_testPresetName, doc.RootElement.GetProperty("name").GetString());
        Assert.Equal("crevis-tc-a160k-freerun-rgb8.cam", doc.RootElement.GetProperty("profileId").GetString());
        Assert.Equal(10, doc.RootElement.GetProperty("frameCount").GetInt32());
        Assert.Equal(500, doc.RootElement.GetProperty("intervalMs").GetInt32());
    }

    [Fact]
    public async Task Save_preset_without_name_returns_bad_request()
    {
        var response = await _client.PutJsonAsync("/api/presets", new
        {
            name = "",
            profileId = "crevis-tc-a160k-freerun-rgb8.cam",
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Save_preset_without_profileId_returns_bad_request()
    {
        var response = await _client.PutJsonAsync("/api/presets", new
        {
            name = _testPresetName,
            profileId = "",
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Save_preset_with_unknown_profileId_returns_bad_request()
    {
        var response = await _client.PutJsonAsync("/api/presets", new
        {
            name = _testPresetName,
            profileId = "nonexistent-camera.cam",
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_returns_saved_preset()
    {
        await _client.PutJsonAsync("/api/presets", new
        {
            name = _testPresetName,
            profileId = "crevis-tc-a160k-freerun-rgb8.cam",
        });

        var response = await _client.GetAsync("/api/presets");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var presets = doc.RootElement.EnumerateArray().ToList();
        Assert.Contains(presets, p => p.GetProperty("name").GetString() == _testPresetName);
    }

    [Fact]
    public async Task GetByName_returns_saved_preset()
    {
        await _client.PutJsonAsync("/api/presets", new
        {
            name = _testPresetName,
            profileId = "crevis-tc-a160k-freerun-rgb8.cam",
            frameCount = 5,
        });

        var response = await _client.GetAsync($"/api/presets/{Uri.EscapeDataString(_testPresetName)}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(_testPresetName, doc.RootElement.GetProperty("name").GetString());
        Assert.Equal(5, doc.RootElement.GetProperty("frameCount").GetInt32());
    }

    [Fact]
    public async Task GetByName_unknown_returns_not_found()
    {
        var response = await _client.GetAsync("/api/presets/nonexistent-preset-xyz");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Save_preset_overwrites_existing_with_same_name()
    {
        await _client.PutJsonAsync("/api/presets", new
        {
            name = _testPresetName,
            profileId = "crevis-tc-a160k-freerun-rgb8.cam",
            frameCount = 1,
        });

        await _client.PutJsonAsync("/api/presets", new
        {
            name = _testPresetName,
            profileId = "crevis-tc-a160k-freerun-rgb8.cam",
            frameCount = 99,
        });

        var response = await _client.GetAsync($"/api/presets/{Uri.EscapeDataString(_testPresetName)}");
        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(99, doc.RootElement.GetProperty("frameCount").GetInt32());
    }

    [Fact]
    public async Task Delete_preset_removes_it_from_list()
    {
        await _client.PutJsonAsync("/api/presets", new
        {
            name = _testPresetName,
            profileId = "crevis-tc-a160k-freerun-rgb8.cam",
        });

        var deleteResp = await _client.DeleteAsync($"/api/presets/{Uri.EscapeDataString(_testPresetName)}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResp.StatusCode);

        var getResp = await _client.GetAsync($"/api/presets/{Uri.EscapeDataString(_testPresetName)}");
        Assert.Equal(HttpStatusCode.NotFound, getResp.StatusCode);
    }

    // ── TriggerMode 제거 확인 ──

    [Fact]
    public async Task Preset_response_does_not_contain_triggerMode_field()
    {
        // TriggerMode was removed from AcquisitionConfigPreset
        await _client.PutJsonAsync("/api/presets", new
        {
            name = _testPresetName,
            profileId = "crevis-tc-a160k-freerun-rgb8.cam",
        });

        var response = await _client.GetAsync($"/api/presets/{Uri.EscapeDataString(_testPresetName)}");
        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

        Assert.False(doc.RootElement.TryGetProperty("triggerMode", out _),
            "triggerMode must not appear in preset response after removal from AcquisitionConfigPreset");
    }

    // ── Save settings (outputDirectory, format) persisted in preset ──

    [Fact]
    public async Task Preset_saves_and_returns_outputDirectory()
    {
        await _client.PutJsonAsync("/api/presets", new
        {
            name = _testPresetName,
            profileId = "crevis-tc-a160k-freerun-rgb8.cam",
            outputDirectory = "/data/peanut/captures",
        });

        var response = await _client.GetAsync($"/api/presets/{Uri.EscapeDataString(_testPresetName)}");
        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal("/data/peanut/captures", doc.RootElement.GetProperty("outputDirectory").GetString());
    }

    [Fact]
    public async Task Preset_saves_and_returns_format()
    {
        await _client.PutJsonAsync("/api/presets", new
        {
            name = _testPresetName,
            profileId = "crevis-tc-a160k-freerun-rgb8.cam",
            format = "bmp",
        });

        var response = await _client.GetAsync($"/api/presets/{Uri.EscapeDataString(_testPresetName)}");
        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        // SaveImageFormat is serialized as camelCase string via JsonStringEnumConverter
        Assert.Equal("bmp", doc.RootElement.GetProperty("format").GetString());
    }

    [Fact]
    public async Task Preset_start_reflects_outputDirectory_in_status()
    {
        await _client.PutJsonAsync("/api/presets", new
        {
            name = _testPresetName,
            profileId = "crevis-tc-a160k-freerun-rgb8.cam",
            outputDirectory = "/preset/custom/path",
            format = "png",
        });

        var presetResp = await _client.GetAsync($"/api/presets/{Uri.EscapeDataString(_testPresetName)}");
        using var presetDoc = JsonDocument.Parse(await presetResp.Content.ReadAsStringAsync());
        var outputDir = presetDoc.RootElement.GetProperty("outputDirectory").GetString();

        var startResp = await _client.PostJsonAsync("/api/acquisition/start", new
        {
            profileId = "crevis-tc-a160k-softtrig-rgb8.cam",
            outputDirectory = outputDir,
            format = "png",
        });
        Assert.Equal(HttpStatusCode.OK, startResp.StatusCode);

        var statusResp = await _client.GetAsync("/api/acquisition/status");
        using var statusDoc = JsonDocument.Parse(await statusResp.Content.ReadAsStringAsync());
        Assert.Equal("/preset/custom/path", statusDoc.RootElement.GetProperty("outputDirectory").GetString());

        await _client.PostAsync("/api/acquisition/stop", null);
    }
}
