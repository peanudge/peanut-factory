using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.MultiCamDriver.Camera;

/// <summary>
/// Built-in camera profiles for Crevis cameras.
/// </summary>
public static class CrevisProfiles
{
    /// <summary>
    /// Crevis TC-A160K area-scan camera, Free-run mode, RGB8 output.
    /// Standard configuration for continuous acquisition.
    /// </summary>
    public static CameraProfile TC_A160K_FreeRun_RGB8 { get; } = new CameraProfile.Builder()
        .WithId("crevis-tc-a160k-freerun-rgb8")
        .WithDisplayName("Crevis TC-A160K (FreeRun RGB8)")
        .WithManufacturer("Crevis")
        .WithModel("TC-A160K")
        .WithCamFile(CamFileResource.KnownCamFiles.TC_A160K_FreeRun_RGB8)
        .WithConnector("M")
        .WithTriggerMode(McTrigMode.MC_TrigMode_IMMEDIATE)
        .WithSurfaceCount(4)
        .WithPixelFormat(PixelFormat.Rgb24)
        .WithResolution(4096, 3072)
        .WithDescription("Area-scan camera for high-resolution color imaging. 12MP resolution.")
        .Build();

    /// <summary>
    /// Crevis TC-A160K area-scan camera, Free-run mode, 1 TAP, RGB8 output.
    /// Single-tap configuration for specific timing requirements.
    /// </summary>
    public static CameraProfile TC_A160K_FreeRun_1TAP_RGB8 { get; } = new CameraProfile.Builder()
        .WithId("crevis-tc-a160k-freerun-1tap-rgb8")
        .WithDisplayName("Crevis TC-A160K (FreeRun 1TAP RGB8)")
        .WithManufacturer("Crevis")
        .WithModel("TC-A160K")
        .WithCamFile(CamFileResource.KnownCamFiles.TC_A160K_FreeRun_1TAP_RGB8)
        .WithConnector("M")
        .WithTriggerMode(McTrigMode.MC_TrigMode_IMMEDIATE)
        .WithSurfaceCount(4)
        .WithPixelFormat(PixelFormat.Rgb24)
        .WithResolution(4096, 3072)
        .WithDescription("Single-tap configuration for specific timing requirements.")
        .Build();

    /// <summary>
    /// Crevis TC-A160K configured for software triggering.
    /// Use for precise frame-by-frame capture control.
    /// </summary>
    public static CameraProfile TC_A160K_SoftwareTrigger_RGB8 { get; } = new CameraProfile.Builder()
        .WithId("crevis-tc-a160k-softtrig-rgb8")
        .WithDisplayName("Crevis TC-A160K (Software Trigger)")
        .WithManufacturer("Crevis")
        .WithModel("TC-A160K")
        .WithCamFile(CamFileResource.KnownCamFiles.TC_A160K_FreeRun_RGB8)
        .WithConnector("M")
        .WithTriggerMode(McTrigMode.MC_TrigMode_SOFT)
        .WithSurfaceCount(2)
        .WithPixelFormat(PixelFormat.Rgb24)
        .WithResolution(4096, 3072)
        .WithDescription("Software-triggered acquisition for precise capture timing.")
        .Build();

    /// <summary>
    /// All built-in Crevis camera profiles.
    /// </summary>
    public static IReadOnlyList<CameraProfile> All { get; } = new[]
    {
        TC_A160K_FreeRun_RGB8,
        TC_A160K_FreeRun_1TAP_RGB8,
        TC_A160K_SoftwareTrigger_RGB8
    };
}
