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
    public ActionResult<List<CamFileInfoDto>> GetCameras()
    {
        var cameras = _camFileService.CamFiles
            .Select(c => new CamFileInfoDto
            {
                FileName = c.FileName,
                Manufacturer = c.Manufacturer,
                CameraModel = c.CameraModel,
                Width = c.Width,
                Height = c.Height,
                Spectrum = c.Spectrum,
                ColorFormat = c.ColorFormat,
                TrigMode = c.TrigMode,
                AcquisitionMode = c.AcquisitionMode,
                TapConfiguration = c.TapConfiguration,
            })
            .ToList();
        return cameras;
    }
}

public class CamFileInfoDto
{
    public required string FileName { get; set; }
    public required string Manufacturer { get; set; }
    public required string CameraModel { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public required string Spectrum { get; set; }
    public required string ColorFormat { get; set; }
    public required string TrigMode { get; set; }
    public required string AcquisitionMode { get; set; }
    public required string TapConfiguration { get; set; }
}
