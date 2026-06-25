using System.Net;
using System.Text.Json;
using PeanutVision.Api.Tests.Infrastructure;

namespace PeanutVision.Api.Tests.Tests.System;

public class SystemCamerasTests : IClassFixture<PeanutVisionApiFactory>, IAsyncLifetime
{
    private readonly HttpClient _client;

    public SystemCamerasTests(PeanutVisionApiFactory factory)
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
    public async Task GetCameras_each_entry_is_a_filename_string()
    {
        var response = await _client.GetAsync("/api/system/cameras");

        using var doc = await response.ReadJsonDocumentAsync();
        foreach (var entry in doc.RootElement.EnumerateArray())
        {
            Assert.Equal(JsonValueKind.String, entry.ValueKind);
            Assert.EndsWith(".cam", entry.GetString(), StringComparison.OrdinalIgnoreCase);
        }
    }

    [Fact]
    public async Task GetCameras_includes_freerun_rgb8()
    {
        var response = await _client.GetAsync("/api/system/cameras");

        using var doc = await response.ReadJsonDocumentAsync();
        var fileNames = doc.RootElement.EnumerateArray()
            .Select(e => e.GetString())
            .ToList();

        Assert.Contains("crevis-tc-a160k-freerun-rgb8.cam", fileNames);
    }
}
