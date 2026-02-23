using PeanutVision.MultiCamDriver;

namespace PeanutVision.Api.Services;

public class CalibrationManager : ICalibrationService
{
    private readonly AcquisitionManager _acquisitionManager;

    public CalibrationManager(AcquisitionManager acquisitionManager)
    {
        _acquisitionManager = acquisitionManager;
    }

    public bool IsAvailable => _acquisitionManager.Channel != null;

    public void PerformBlackCalibration()
    {
        GetRequiredChannel().PerformBlackCalibration();
    }

    public void PerformWhiteCalibration()
    {
        GetRequiredChannel().PerformWhiteCalibration();
    }

    public void PerformWhiteBalanceOnce()
    {
        GetRequiredChannel().PerformWhiteBalanceOnce();
    }

    public void SetFlatFieldCorrection(bool enable)
    {
        GetRequiredChannel().SetFlatFieldCorrection(enable);
    }

    public ExposureInfo GetExposure()
    {
        var channel = GetRequiredChannel();
        var range = channel.GetExposureRange();

        return new ExposureInfo
        {
            ExposureUs = channel.GetExposureUs(),
            GainDb = channel.GetGainDb(),
            ExposureRange = new ExposureRangeInfo
            {
                Min = range.Min,
                Max = range.Max,
            },
        };
    }

    public ExposureInfo SetExposure(double? exposureUs, double? gainDb)
    {
        var channel = GetRequiredChannel();

        if (exposureUs.HasValue)
            channel.SetExposureUs(exposureUs.Value);

        if (gainDb.HasValue)
            channel.SetGainDb(gainDb.Value);

        var range = channel.GetExposureRange();

        return new ExposureInfo
        {
            ExposureUs = channel.GetExposureUs(),
            GainDb = channel.GetGainDb(),
            ExposureRange = new ExposureRangeInfo
            {
                Min = range.Min,
                Max = range.Max,
            },
        };
    }

    private GrabChannel GetRequiredChannel()
    {
        return _acquisitionManager.Channel
            ?? throw new InvalidOperationException("No active acquisition channel.");
    }
}
