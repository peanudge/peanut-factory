using System.Net;
using System.Text.Json;
using PeanutVision.Api.Tests.Infrastructure;

namespace PeanutVision.Api.Tests.Specs.System;

public class SystemBoardsSpec : IClassFixture<PeanutVisionApiFactory>, IAsyncLifetime
{
    private readonly PeanutVisionApiFactory _factory;
    private readonly HttpClient _client;

    public SystemBoardsSpec(PeanutVisionApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await _client.PostAsync("/api/acquisition/stop", null);
    }

    [Fact]
    public async Task GetBoards_returns_ok_with_board_list()
    {
        var response = await _client.GetAsync("/api/system/boards");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = await response.ReadJsonDocumentAsync();
        Assert.Equal(JsonValueKind.Array, doc.RootElement.ValueKind);
    }

    [Fact]
    public async Task GetBoards_returns_one_board_from_mock()
    {
        var response = await _client.GetAsync("/api/system/boards");

        using var doc = await response.ReadJsonDocumentAsync();
        Assert.Equal(1, doc.RootElement.GetArrayLength());
    }

    [Fact]
    public async Task GetBoards_board_has_expected_fields()
    {
        var response = await _client.GetAsync("/api/system/boards");

        using var doc = await response.ReadJsonDocumentAsync();
        var board = doc.RootElement[0];

        Assert.True(board.TryGetProperty("index", out _));
        Assert.True(board.TryGetProperty("boardType", out _));
        Assert.True(board.TryGetProperty("boardName", out _));
        Assert.True(board.TryGetProperty("serialNumber", out _));
        Assert.True(board.TryGetProperty("pciPosition", out _));
    }

    [Fact]
    public async Task GetBoardStatus_returns_ok_for_valid_index()
    {
        var response = await _client.GetAsync("/api/system/boards/0/status");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var doc = await response.ReadJsonDocumentAsync();
        Assert.True(doc.RootElement.TryGetProperty("boardName", out _));
        Assert.True(doc.RootElement.TryGetProperty("inputState", out _));
        Assert.True(doc.RootElement.TryGetProperty("pcIeLinkInfo", out _));
    }

    [Fact]
    public async Task GetBoardStatus_returns_404_for_invalid_index()
    {
        var response = await _client.GetAsync("/api/system/boards/99/status");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
