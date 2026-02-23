using PeanutVision.MultiCamDriver;

namespace PeanutVision.Api.Services;

public readonly record struct TriggerMode
{
    internal McTrigMode Mode { get; }

    public TriggerMode(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        var normalized = value.StartsWith("MC_TrigMode_", StringComparison.OrdinalIgnoreCase)
            ? value : $"MC_TrigMode_{value}";
        Mode = Enum.Parse<McTrigMode>(normalized, ignoreCase: true);
    }

    private TriggerMode(McTrigMode mode) => Mode = mode;

    public static TriggerMode Immediate => new(McTrigMode.MC_TrigMode_IMMEDIATE);
    public static TriggerMode Soft => new(McTrigMode.MC_TrigMode_SOFT);
    public static TriggerMode Hard => new(McTrigMode.MC_TrigMode_HARD);
    public static TriggerMode Combined => new(McTrigMode.MC_TrigMode_COMBINED);

    public static TriggerMode Parse(string value) => new(value);
    public override string ToString() => Mode.ToString().Replace("MC_TrigMode_", "");
}
