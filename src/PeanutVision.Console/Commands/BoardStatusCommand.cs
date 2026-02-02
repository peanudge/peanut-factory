using PeanutVision.MultiCamDriver;

namespace PeanutVision.Console.Commands;

/// <summary>
/// Displays detailed board status including diagnostics.
/// </summary>
public sealed class BoardStatusCommand : CommandBase
{
    public override string Name => "Board Status";
    public override string Description => "Detailed board diagnostics";
    public override char Key => '8';

    public override void Execute(CommandContext context)
    {
        if (!RequireHardware(context, "query board status"))
            return;

        PrintHeader("BOARD STATUS");

        for (int i = 0; i < context.Service.BoardCount; i++)
        {
            try
            {
                var status = context.Service.GetBoardStatus(i);

                PrintSection($"Board [{i}]: {status.BoardName}");

                // Identification
                Print($"    Type        : {status.BoardType}");
                Print($"    Serial      : {status.SerialNumber}");
                Print($"    PCI Position: {status.PCIPosition}");

                // Input Status
                Print("\n    INPUT STATUS:");
                Print($"      Connector : {status.InputConnector}");
                Print($"      State     : {status.InputState}");
                Print($"      Signal    : {status.SignalStrength}");

                // Output Status
                Print("\n    OUTPUT STATUS:");
                Print($"      State     : {status.OutputState}");

                // Link Status
                Print("\n    LINK STATUS:");
                Print($"      Camera Link: {status.CameraLinkStatus}");
                Print($"      Sync Errors: {status.SyncErrors}");
                Print($"      Clock Errors: {status.ClockErrors}");

                // Diagnostics
                Print("\n    DIAGNOSTICS:");
                Print($"      Grabber Errors      : {status.GrabberErrors}");
                Print($"      Frame Trig Violations: {status.FrameTriggerViolations}");
                Print($"      Line Trig Violations : {status.LineTriggerViolations}");

                // PCIe
                Print($"\n    PCIe Link   : {status.PCIeLinkInfo}");
            }
            catch (Exception ex)
            {
                Print($"\n  [ERROR] Failed to get status for board {i}: {ex.Message}");
            }
        }

        PrintFooter();
        WaitForKey();
    }
}
