namespace PeanutVision.Api.Services;

/// <summary>
/// Internal interface exposing channel exposure access to ExposureController.
/// Not visible outside PeanutVision.Api.
/// </summary>
internal interface IExposureSource
{
    bool HasActiveChannel { get; }
    double GetExposureUs();
    (double Min, double Max) GetExposureRange();
    void SetExposureUs(double us);
}
