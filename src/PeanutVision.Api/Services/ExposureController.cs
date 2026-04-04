namespace PeanutVision.Api.Services;

/// <summary>
/// Manages exposure settings, caching the desired value when no channel is active.
/// </summary>
internal sealed class ExposureController : IExposureController
{
    private const double DEFAULT_EXPOSURE_US = 10_000.0;

    private readonly IExposureSource _source;
    private readonly object _lock = new();
    private double _desiredExposureUs = DEFAULT_EXPOSURE_US;

    public ExposureController(IExposureSource source)
    {
        _source = source ?? throw new ArgumentNullException(nameof(source));
    }

    public ExposureInfo GetExposure()
    {
        lock (_lock)
        {
            if (_source.HasActiveChannel)
            {
                _desiredExposureUs = _source.GetExposureUs();
                var (min, max) = _source.GetExposureRange();
                return new ExposureInfo
                {
                    ExposureUs = _desiredExposureUs,
                    ExposureRange = new ExposureRangeInfo { Min = min, Max = max },
                };
            }

            return new ExposureInfo { ExposureUs = _desiredExposureUs };
        }
    }

    public ExposureInfo SetExposure(double? exposureUs)
    {
        lock (_lock)
        {
            if (exposureUs.HasValue)
                _desiredExposureUs = exposureUs.Value;

            if (_source.HasActiveChannel)
            {
                _source.SetExposureUs(_desiredExposureUs);
                var (min, max) = _source.GetExposureRange();
                return new ExposureInfo
                {
                    ExposureUs = _source.GetExposureUs(),
                    ExposureRange = new ExposureRangeInfo { Min = min, Max = max },
                };
            }

            return new ExposureInfo { ExposureUs = _desiredExposureUs };
        }
    }
}
