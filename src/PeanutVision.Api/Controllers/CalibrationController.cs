using Microsoft.AspNetCore.Mvc;
using PeanutVision.Api.Exceptions;
using PeanutVision.Api.Services;

namespace PeanutVision.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CalibrationController : ControllerBase
{
    private readonly ICalibrationService _calibration;

    public CalibrationController(ICalibrationService calibration)
    {
        _calibration = calibration;
    }

    [HttpPost("black")]
    public ActionResult PerformBlackCalibration()
    {
        if (!_calibration.IsAvailable)
            throw new ChannelNotAvailableException();

        _calibration.PerformBlackCalibration();
        return Ok(new { message = "Black calibration executed. Ensure lens was covered." });
    }

    [HttpPost("white")]
    public ActionResult PerformWhiteCalibration()
    {
        if (!_calibration.IsAvailable)
            throw new ChannelNotAvailableException();

        _calibration.PerformWhiteCalibration();
        return Ok(new { message = "White calibration executed. Ensure uniform ~200DN illumination." });
    }

    [HttpPost("white-balance")]
    public ActionResult PerformWhiteBalance()
    {
        if (!_calibration.IsAvailable)
            throw new ChannelNotAvailableException();

        _calibration.PerformWhiteBalanceOnce();
        return Ok(new { message = "White balance (ONCE) executed." });
    }

    [HttpPost("ffc")]
    public ActionResult SetFlatFieldCorrection([FromBody] FfcRequest request)
    {
        if (!_calibration.IsAvailable)
            throw new ChannelNotAvailableException();

        _calibration.SetFlatFieldCorrection(request.Enable);
        return Ok(new { message = $"Flat field correction {(request.Enable ? "enabled" : "disabled")}." });
    }

    [HttpGet("exposure")]
    public ActionResult GetExposure()
    {
        if (!_calibration.IsAvailable)
            throw new ChannelNotAvailableException();

        var info = _calibration.GetExposure();
        return Ok(new
        {
            exposureUs = info.ExposureUs,
            exposureRange = info.ExposureRange != null
                ? new { min = info.ExposureRange.Min, max = info.ExposureRange.Max }
                : null,
        });
    }

    [HttpPut("exposure")]
    public ActionResult SetExposure([FromBody] ExposureRequest request)
    {
        if (!_calibration.IsAvailable)
            throw new ChannelNotAvailableException();

        var info = _calibration.SetExposure(request.ExposureUs);
        return Ok(new
        {
            message = "Exposure settings updated.",
            exposureUs = info.ExposureUs,
        });
    }
}

public class FfcRequest
{
    public bool Enable { get; set; }
}

public class ExposureRequest
{
    public double? ExposureUs { get; set; }
}
