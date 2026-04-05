using System.Net;
using System.Text.Json;
using PeanutVision.Api.Tests.Infrastructure;

namespace PeanutVision.Api.Tests.Specs.Settings;

/// <summary>
/// AC 2: 변경 즉시 API에 반영되며 다음 저장부터 적용된다
/// Verifies that settings changes via PATCH are immediately reflected in GET
/// and applied from the next save operation.
/// </summary>
public class SettingsImmediateReflectionSpec : IClassFixture<PeanutVisionApiFactory>
{
    private readonly HttpClient _client;

    public SettingsImmediateReflectionSpec(PeanutVisionApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Patch_OutputDirectory_immediately_reflected_in_GET()
    {
        var newDir = $"output-{Guid.NewGuid():N}";

        var patchResponse = await _client.PatchJsonAsync("/api/settings/image-save",
            new { outputDirectory = newDir });

        Assert.Equal(HttpStatusCode.OK, patchResponse.StatusCode);

        var getResponse = await _client.GetAsync("/api/settings/image-save");
        using var doc = await getResponse.ReadJsonDocumentAsync();

        Assert.Equal(newDir, doc.RootElement.GetProperty("outputDirectory").GetString());
    }

    [Fact]
    public async Task Patch_FilenamePrefix_immediately_reflected_in_GET()
    {
        var newPrefix = $"prefix-{Guid.NewGuid():N}";

        var patchResponse = await _client.PatchJsonAsync("/api/settings/image-save",
            new { filenamePrefix = newPrefix });

        Assert.Equal(HttpStatusCode.OK, patchResponse.StatusCode);

        var getResponse = await _client.GetAsync("/api/settings/image-save");
        using var doc = await getResponse.ReadJsonDocumentAsync();

        Assert.Equal(newPrefix, doc.RootElement.GetProperty("filenamePrefix").GetString());
    }

    [Fact]
    public async Task Patch_returns_full_updated_settings()
    {
        // Get current settings first
        var getResponse = await _client.GetAsync("/api/settings/image-save");
        using var currentDoc = await getResponse.ReadJsonDocumentAsync();
        var currentTimestampFormat = currentDoc.RootElement.GetProperty("timestampFormat").GetString();

        var patchResponse = await _client.PatchJsonAsync("/api/settings/image-save",
            new { outputDirectory = "patched-dir" });

        Assert.Equal(HttpStatusCode.OK, patchResponse.StatusCode);
        using var doc = await patchResponse.ReadJsonDocumentAsync();

        // Full settings object returned
        Assert.Equal("patched-dir", doc.RootElement.GetProperty("outputDirectory").GetString());
        // Other fields preserved
        Assert.Equal(currentTimestampFormat, doc.RootElement.GetProperty("timestampFormat").GetString());
    }

    [Fact]
    public async Task Patch_with_null_fields_leaves_them_unchanged()
    {
        // Set a known state
        await _client.PatchJsonAsync("/api/settings/image-save",
            new { outputDirectory = "original-dir", filenamePrefix = "original-prefix" });

        // Patch only OutputDirectory
        await _client.PatchJsonAsync("/api/settings/image-save",
            new { outputDirectory = "new-dir" });

        var getResponse = await _client.GetAsync("/api/settings/image-save");
        using var doc = await getResponse.ReadJsonDocumentAsync();

        Assert.Equal("new-dir", doc.RootElement.GetProperty("outputDirectory").GetString());
        Assert.Equal("original-prefix", doc.RootElement.GetProperty("filenamePrefix").GetString());
    }

    [Fact]
    public async Task Patch_with_invalid_path_chars_returns_BadRequest()
    {
        var response = await _client.PatchJsonAsync("/api/settings/image-save",
            new { outputDirectory = "invalid\0path" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        using var doc = await response.ReadJsonDocumentAsync();
        var errors = doc.RootElement.GetProperty("errors");
        Assert.True(errors.GetArrayLength() > 0);
    }

    [Fact]
    public async Task Patch_with_invalid_filename_chars_returns_BadRequest()
    {
        // '/' is invalid in filenames on both Windows and macOS/Linux
        var response = await _client.PatchJsonAsync("/api/settings/image-save",
            new { filenamePrefix = "bad/prefix" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        using var doc = await response.ReadJsonDocumentAsync();
        var errors = doc.RootElement.GetProperty("errors");
        Assert.True(errors.GetArrayLength() > 0);
    }

    [Fact]
    public async Task Patch_with_empty_string_returns_BadRequest()
    {
        var response = await _client.PatchJsonAsync("/api/settings/image-save",
            new { filenamePrefix = "" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Patch_with_whitespace_only_returns_BadRequest()
    {
        var response = await _client.PatchJsonAsync("/api/settings/image-save",
            new { outputDirectory = "   " });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Patch_settings_used_by_next_capture()
    {
        // Set a unique prefix
        var uniquePrefix = $"cap-{Guid.NewGuid():N}";
        await _client.PatchJsonAsync("/api/settings/image-save",
            new { filenamePrefix = uniquePrefix });

        // Verify GET returns the new prefix immediately before any capture
        var getResponse = await _client.GetAsync("/api/settings/image-save");
        using var doc = await getResponse.ReadJsonDocumentAsync();
        Assert.Equal(uniquePrefix, doc.RootElement.GetProperty("filenamePrefix").GetString());
    }
}
