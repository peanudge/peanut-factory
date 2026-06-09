using Microsoft.AspNetCore.Mvc;
using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Camera;

namespace PeanutVision.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SystemController : ControllerBase
{
    private readonly IGrabService _grabService;
    private readonly ICamFileService _camFileService;

    public SystemController(IGrabService grabService, ICamFileService camFileService)
    {
        _grabService = grabService;
        _camFileService = camFileService;
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
    public ActionResult<IEnumerable<string>> GetCameras()
        => Ok(_camFileService.CamFiles.Select(c => c.FileName));
}
