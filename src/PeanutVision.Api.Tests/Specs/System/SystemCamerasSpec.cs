using System.Net;
using PeanutVision.Api.Tests.Infrastructure;

namespace PeanutVision.Api.Tests.Specs.System;

public class SystemCamerasSpec : IClassFixture<PeanutVisionApiFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;

    public SystemCamerasSpec(PeanutVisionApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await _client.PostAsync("/api/acquisition/stop", null);
    }

    [Fact]
    public async Task GetCameras_returns_ok()
    {
        var response = await _client.GetAsync("/api/system/cameras");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetCameras_returns_cam_files()
    {
        var response = await _client.GetAsync("/api/system/cameras");

        using var doc = await response.ReadJsonDocumentAsync();
        Assert.True(doc.RootElement.GetArrayLength() >= 3);
    }

    [Fact]
    public async Task GetCameras_each_entry_has_required_fields()
    {
        var response = await _client.GetAsync("/api/system/cameras");

        using var doc = await response.ReadJsonDocumentAsync();
        foreach (var entry in doc.RootElement.EnumerateArray())
        {
            Assert.True(entry.TryGetProperty("fileName", out _));
            Assert.True(entry.TryGetProperty("manufacturer", out _));
            Assert.True(entry.TryGetProperty("cameraModel", out _));
            Assert.True(entry.TryGetProperty("width", out _));
            Assert.True(entry.TryGetProperty("height", out _));
            Assert.True(entry.TryGetProperty("spectrum", out _));
            Assert.True(entry.TryGetProperty("colorFormat", out _));
            Assert.True(entry.TryGetProperty("trigMode", out _));
        }
    }

    [Fact]
    public async Task GetCameras_includes_freerun_rgb8()
    {
        var response = await _client.GetAsync("/api/system/cameras");

        using var doc = await response.ReadJsonDocumentAsync();
        var fileNames = doc.RootElement.EnumerateArray()
            .Select(e => e.GetProperty("fileName").GetString())
            .ToList();

        Assert.Contains("crevis-tc-a160k-freerun-rgb8.cam", fileNames);
    }
}
