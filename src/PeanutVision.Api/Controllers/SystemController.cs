using Microsoft.AspNetCore.Mvc;
using PeanutVision.MultiCamDriver;

namespace PeanutVision.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SystemController : ControllerBase
{
    private readonly IGrabService _grabService;

    public SystemController(IGrabService grabService)
    {
        _grabService = grabService;
    }

    [HttpGet("boards")]
    public ActionResult<List<BoardInfo>> GetBoards()
    {
        var boards = new List<BoardInfo>();
        for (int i = 0; i < _grabService.BoardCount; i++)
        {
            boards.Add(_grabService.GetBoardInfo(i));
        }
        return boards;
    }

    [HttpGet("boards/{index}/status")]
    public ActionResult<BoardStatus> GetBoardStatus(int index)
    {
        if (index < 0 || index >= _grabService.BoardCount)
            return NotFound($"Board index {index} not found. {_grabService.BoardCount} board(s) detected.");

        return _grabService.GetBoardStatus(index);
    }

    [HttpGet("cameras")]
    public ActionResult<List<CameraProfileDto>> GetCameras()
    {
        var profiles = _grabService.CameraProfiles.Profiles
            .Select(p => new CameraProfileDto
            {
                Id = p.Id,
                DisplayName = p.DisplayName,
                Manufacturer = p.Manufacturer,
                Model = p.Model,
                Connector = p.Connector,
                TriggerMode = p.TriggerMode.ToString(),
                PixelFormat = p.PixelFormat.Name,
                ExpectedWidth = p.ExpectedWidth,
                ExpectedHeight = p.ExpectedHeight,
                Description = p.Description,
            })
            .ToList();
        return profiles;
    }
}

public class CameraProfileDto
{
    public required string Id { get; set; }
    public required string DisplayName { get; set; }
    public required string Manufacturer { get; set; }
    public required string Model { get; set; }
    public required string Connector { get; set; }
    public required string TriggerMode { get; set; }
    public required string PixelFormat { get; set; }
    public int ExpectedWidth { get; set; }
    public int ExpectedHeight { get; set; }
    public string? Description { get; set; }
}
