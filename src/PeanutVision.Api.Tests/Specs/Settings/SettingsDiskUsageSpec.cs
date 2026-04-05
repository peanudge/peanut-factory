using System.Net;
using System.Text.Json;
using PeanutVision.Api.Tests.Infrastructure;

namespace PeanutVision.Api.Tests.Specs.Settings;

public class SettingsDiskUsageSpec : IClassFixture<PeanutVisionApiFactory>
{
    private readonly HttpClient _client;

    public SettingsDiskUsageSpec(PeanutVisionApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetDiskUsage_returns_ok()
    {
        var response = await _client.GetAsync("/api/settings/disk-usage");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetDiskUsage_returns_expected_fields()
    {
        var response = await _client.GetAsync("/api/settings/disk-usage");

        using var doc = await response.ReadJsonDocumentAsync();
        var root = doc.RootElement;

        Assert.True(root.TryGetProperty("totalBytes", out var total));
        Assert.True(root.TryGetProperty("usedBytes", out var used));
        Assert.True(root.TryGetProperty("freeBytes", out var free));
        Assert.True(root.TryGetProperty("usagePercent", out _));

        Assert.Equal(JsonValueKind.Number, total.ValueKind);
        Assert.Equal(JsonValueKind.Number, used.ValueKind);
        Assert.Equal(JsonValueKind.Number, free.ValueKind);
    }

    [Fact]
    public async Task GetDiskUsage_totalBytes_is_positive()
    {
        var response = await _client.GetAsync("/api/settings/disk-usage");

        using var doc = await response.ReadJsonDocumentAsync();
        var totalBytes = doc.RootElement.GetProperty("totalBytes").GetInt64();

        Assert.True(totalBytes > 0, $"totalBytes should be > 0, got {totalBytes}");
    }

    [Fact]
    public async Task GetDiskUsage_usedBytes_plus_freeBytes_equals_totalBytes()
    {
        var response = await _client.GetAsync("/api/settings/disk-usage");

        using var doc = await response.ReadJsonDocumentAsync();
        var root = doc.RootElement;

        var total = root.GetProperty("totalBytes").GetInt64();
        var used = root.GetProperty("usedBytes").GetInt64();
        var free = root.GetProperty("freeBytes").GetInt64();

        Assert.Equal(total, used + free);
    }

    [Fact]
    public async Task GetDiskUsage_usagePercent_is_between_0_and_100()
    {
        var response = await _client.GetAsync("/api/settings/disk-usage");

        using var doc = await response.ReadJsonDocumentAsync();
        var usagePercent = doc.RootElement.GetProperty("usagePercent").GetDouble();

        Assert.InRange(usagePercent, 0.0, 100.0);
    }
}
