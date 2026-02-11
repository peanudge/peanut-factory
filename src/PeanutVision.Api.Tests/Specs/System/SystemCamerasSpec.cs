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
    public async Task GetCameras_returns_three_crevis_profiles()
    {
        var response = await _client.GetAsync("/api/system/cameras");

        using var doc = await response.ReadJsonDocumentAsync();
        Assert.Equal(3, doc.RootElement.GetArrayLength());
    }

    [Fact]
    public async Task GetCameras_each_profile_has_required_fields()
    {
        var response = await _client.GetAsync("/api/system/cameras");

        using var doc = await response.ReadJsonDocumentAsync();
        foreach (var profile in doc.RootElement.EnumerateArray())
        {
            Assert.True(profile.TryGetProperty("id", out _));
            Assert.True(profile.TryGetProperty("displayName", out _));
            Assert.True(profile.TryGetProperty("manufacturer", out _));
            Assert.True(profile.TryGetProperty("model", out _));
            Assert.True(profile.TryGetProperty("connector", out _));
            Assert.True(profile.TryGetProperty("triggerMode", out _));
            Assert.True(profile.TryGetProperty("pixelFormat", out _));
        }
    }

    [Fact]
    public async Task GetCameras_includes_freerun_rgb8_profile()
    {
        var response = await _client.GetAsync("/api/system/cameras");

        using var doc = await response.ReadJsonDocumentAsync();
        var ids = doc.RootElement.EnumerateArray()
            .Select(e => e.GetProperty("id").GetString())
            .ToList();

        Assert.Contains("crevis-tc-a160k-freerun-rgb8", ids);
    }
}
