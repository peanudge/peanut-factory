using Microsoft.AspNetCore.Mvc;
using PeanutVision.Api.Services;
using PeanutVision.MultiCamDriver;

namespace PeanutVision.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CalibrationController : ControllerBase
{
    private readonly AcquisitionManager _manager;

    public CalibrationController(AcquisitionManager manager)
    {
        _manager = manager;
    }

    [HttpPost("black")]
    public ActionResult PerformBlackCalibration()
    {
        var channel = GetActiveChannel();
        if (channel == null)
            return Conflict(new { error = "No active acquisition channel." });

        channel.PerformBlackCalibration();
        return Ok(new { message = "Black calibration executed. Ensure lens was covered." });
    }

    [HttpPost("white")]
    public ActionResult PerformWhiteCalibration()
    {
        var channel = GetActiveChannel();
        if (channel == null)
            return Conflict(new { error = "No active acquisition channel." });

        channel.PerformWhiteCalibration();
        return Ok(new { message = "White calibration executed. Ensure uniform ~200DN illumination." });
    }

    [HttpPost("white-balance")]
    public ActionResult PerformWhiteBalance()
    {
        var channel = GetActiveChannel();
        if (channel == null)
            return Conflict(new { error = "No active acquisition channel." });

        channel.PerformWhiteBalanceOnce();
        return Ok(new { message = "White balance (ONCE) executed." });
    }

    [HttpPost("ffc")]
    public ActionResult SetFlatFieldCorrection([FromBody] FfcRequest request)
    {
        var channel = GetActiveChannel();
        if (channel == null)
            return Conflict(new { error = "No active acquisition channel." });

        channel.SetFlatFieldCorrection(request.Enable);
        return Ok(new { message = $"Flat field correction {(request.Enable ? "enabled" : "disabled")}." });
    }

    [HttpGet("exposure")]
    public ActionResult GetExposure()
    {
        var channel = GetActiveChannel();
        if (channel == null)
            return Conflict(new { error = "No active acquisition channel." });

        return Ok(new
        {
            exposureUs = channel.GetExposureUs(),
            exposureRange = new
            {
                min = channel.GetExposureRange().Min,
                max = channel.GetExposureRange().Max,
            },
            gainDb = channel.GetGainDb(),
        });
    }

    [HttpPut("exposure")]
    public ActionResult SetExposure([FromBody] ExposureRequest request)
    {
        var channel = GetActiveChannel();
        if (channel == null)
            return Conflict(new { error = "No active acquisition channel." });

        if (request.ExposureUs.HasValue)
            channel.SetExposureUs(request.ExposureUs.Value);

        if (request.GainDb.HasValue)
            channel.SetGainDb(request.GainDb.Value);

        return Ok(new
        {
            message = "Exposure settings updated.",
            exposureUs = channel.GetExposureUs(),
            gainDb = channel.GetGainDb(),
        });
    }

    private GrabChannel? GetActiveChannel() => _manager.Channel;
}

public class FfcRequest
{
    public bool Enable { get; set; }
}

public class ExposureRequest
{
    public double? ExposureUs { get; set; }
    public double? GainDb { get; set; }
}
