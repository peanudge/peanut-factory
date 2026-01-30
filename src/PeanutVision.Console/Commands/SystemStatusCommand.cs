namespace PeanutVision.Console.Commands;

/// <summary>
/// Displays system status including driver version and board information.
/// </summary>
public sealed class SystemStatusCommand : CommandBase
{
    public override string Name => "System Status";
    public override string Description => "Check driver and board info";
    public override char Key => '1';

    public override void Execute(CommandContext context)
    {
        PrintHeader("SYSTEM STATUS");

        var service = context.Service;

        Print($"\n  Driver Version : {service.DriverVersion}");
        Print($"  Boards Detected: {service.BoardCount}");
        Print($"  Mode           : {(context.UseMockHal ? "MOCK (Simulation)" : "HARDWARE")}");

        if (service.BoardCount > 0)
        {
            Print("\n  Detected Boards:");
            Print("  ─────────────────────────────────────────────────────────");

            for (int i = 0; i < service.BoardCount; i++)
            {
                try
                {
                    var info = service.GetBoardInfo(i);
                    Print($"  [{i}] {info.BoardName}");
                    Print($"      Type    : {info.BoardType}");
                    Print($"      Serial  : {info.SerialNumber}");
                    Print($"      PCI     : {info.PciPosition}");
                }
                catch (Exception ex)
                {
                    Print($"  [{i}] Error reading board info: {ex.Message}");
                }
            }
        }
        else
        {
            Print("\n  [WARNING] No frame grabber boards detected!");
            Print("  - Check hardware connection");
            Print("  - Verify MultiCam driver installation");
            Print("  - Run with --mock flag for simulation mode");
        }

        PrintFooter();
        WaitForKey();
    }
}
