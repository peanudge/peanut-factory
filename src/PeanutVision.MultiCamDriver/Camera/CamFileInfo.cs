namespace PeanutVision.MultiCamDriver.Camera;

/// <summary>
/// Parsed metadata from a .cam configuration file.
/// FileName is the natural key (case-insensitive).
/// </summary>
public sealed record CamFileInfo(
    string FileName,
    string FilePath,
    string Manufacturer,
    string CameraModel,
    int Width,
    int Height,
    string Spectrum,
    string ColorFormat,
    string TrigMode,
    string AcquisitionMode,
    string TapConfiguration)
{
    /// <summary>
    /// Creates GrabChannelOptions from this cam file info.
    /// </summary>
    public GrabChannelOptions ToChannelOptions(int driverIndex = 0, string connector = "M",
        int surfaceCount = 4, bool useCallback = true)
    {
        return new GrabChannelOptions
        {
            DriverIndex = driverIndex,
            Connector = connector,
            CamFilePath = FilePath,
            SurfaceCount = surfaceCount,
            TriggerMode = ParseTrigMode(TrigMode),
            UseCallback = useCallback
        };
    }

    /// <summary>
    /// Creates GrabChannelOptions with a custom trigger mode override.
    /// </summary>
    public GrabChannelOptions ToChannelOptions(McTrigMode triggerMode, int driverIndex = 0,
        string connector = "M", int surfaceCount = 4, bool useCallback = true)
    {
        var options = ToChannelOptions(driverIndex, connector, surfaceCount, useCallback);
        options.TriggerMode = triggerMode;
        return options;
    }

    private static McTrigMode ParseTrigMode(string trigMode)
    {
        return trigMode.ToUpperInvariant() switch
        {
            "IMMEDIATE" => McTrigMode.MC_TrigMode_IMMEDIATE,
            "SOFT" => McTrigMode.MC_TrigMode_SOFT,
            "HARD" => McTrigMode.MC_TrigMode_HARD,
            "COMBINED" => McTrigMode.MC_TrigMode_COMBINED,
            _ => McTrigMode.MC_TrigMode_IMMEDIATE
        };
    }
}
