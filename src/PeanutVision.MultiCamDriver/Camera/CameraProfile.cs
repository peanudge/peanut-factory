using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.MultiCamDriver.Camera;

/// <summary>
/// Represents a camera configuration profile with all settings needed for acquisition.
/// Immutable value object that encapsulates camera-specific details.
/// </summary>
public sealed class CameraProfile
{
    /// <summary>Unique identifier for this profile (e.g., "crevis-tc-a160k-freerun-rgb8")</summary>
    public string Id { get; }

    /// <summary>Human-readable display name</summary>
    public string DisplayName { get; }

    /// <summary>Camera manufacturer</summary>
    public string Manufacturer { get; }

    /// <summary>Camera model</summary>
    public string Model { get; }

    /// <summary>Name of the embedded .cam configuration file</summary>
    public string CamFileName { get; }

    /// <summary>Default connector on the frame grabber</summary>
    public string Connector { get; }

    /// <summary>Default trigger mode</summary>
    public McTrigMode TriggerMode { get; }

    /// <summary>Default number of frame buffers</summary>
    public int SurfaceCount { get; }

    /// <summary>Pixel format of captured images</summary>
    public PixelFormat PixelFormat { get; }

    /// <summary>Expected image width (0 if variable)</summary>
    public int ExpectedWidth { get; }

    /// <summary>Expected image height (0 if variable)</summary>
    public int ExpectedHeight { get; }

    /// <summary>Optional description or notes</summary>
    public string? Description { get; }

    private CameraProfile(Builder builder)
    {
        Id = builder.Id ?? throw new ArgumentNullException(nameof(builder.Id));
        DisplayName = builder.DisplayName ?? builder.Model ?? Id;
        Manufacturer = builder.Manufacturer ?? "Unknown";
        Model = builder.Model ?? "Unknown";
        CamFileName = builder.CamFileName ?? throw new ArgumentNullException(nameof(builder.CamFileName));
        Connector = builder.Connector ?? "M";
        TriggerMode = builder.TriggerMode;
        SurfaceCount = builder.SurfaceCount > 0 ? builder.SurfaceCount : 4;
        PixelFormat = builder.PixelFormat ?? PixelFormat.Rgb24;
        ExpectedWidth = builder.ExpectedWidth;
        ExpectedHeight = builder.ExpectedHeight;
        Description = builder.Description;
    }

    /// <summary>
    /// Creates GrabChannelOptions from this profile.
    /// </summary>
    public GrabChannelOptions ToChannelOptions(int driverIndex = 0, bool useCallback = true)
    {
        return new GrabChannelOptions
        {
            DriverIndex = driverIndex,
            Connector = Connector,
            CamFilePath = CamFileResource.GetCamFilePath(CamFileName),
            SurfaceCount = SurfaceCount,
            TriggerMode = TriggerMode,
            UseCallback = useCallback
        };
    }

    /// <summary>
    /// Creates GrabChannelOptions with custom trigger mode.
    /// </summary>
    public GrabChannelOptions ToChannelOptions(McTrigMode triggerMode, int driverIndex = 0, bool useCallback = true)
    {
        var options = ToChannelOptions(driverIndex, useCallback);
        options.TriggerMode = triggerMode;
        return options;
    }

    /// <summary>
    /// Creates a profile from a cam file name with default settings.
    /// </summary>
    public static CameraProfile FromCamFile(string camFileName)
    {
        var name = Path.GetFileNameWithoutExtension(camFileName);
        return new Builder()
            .WithId(name)
            .WithDisplayName(name)
            .WithCamFile(camFileName)
            .Build();
    }

    public override string ToString() => $"{DisplayName} ({Id})";

    /// <summary>
    /// Builder for creating CameraProfile instances.
    /// </summary>
    public sealed class Builder
    {
        internal string? Id { get; private set; }
        internal string? DisplayName { get; private set; }
        internal string? Manufacturer { get; private set; }
        internal string? Model { get; private set; }
        internal string? CamFileName { get; private set; }
        internal string? Connector { get; private set; }
        internal McTrigMode TriggerMode { get; private set; } = McTrigMode.MC_TrigMode_IMMEDIATE;
        internal int SurfaceCount { get; private set; } = 4;
        internal PixelFormat? PixelFormat { get; private set; }
        internal int ExpectedWidth { get; private set; }
        internal int ExpectedHeight { get; private set; }
        internal string? Description { get; private set; }

        public Builder WithId(string id) { Id = id; return this; }
        public Builder WithDisplayName(string name) { DisplayName = name; return this; }
        public Builder WithManufacturer(string manufacturer) { Manufacturer = manufacturer; return this; }
        public Builder WithModel(string model) { Model = model; return this; }
        public Builder WithCamFile(string camFileName) { CamFileName = camFileName; return this; }
        public Builder WithConnector(string connector) { Connector = connector; return this; }
        public Builder WithTriggerMode(McTrigMode mode) { TriggerMode = mode; return this; }
        public Builder WithSurfaceCount(int count) { SurfaceCount = count; return this; }
        public Builder WithPixelFormat(PixelFormat format) { PixelFormat = format; return this; }
        public Builder WithResolution(int width, int height) { ExpectedWidth = width; ExpectedHeight = height; return this; }
        public Builder WithDescription(string description) { Description = description; return this; }

        public CameraProfile Build() => new(this);
    }
}
