using PeanutVision.MultiCamDriver;

namespace PeanutVision.Console.Commands;

/// <summary>
/// Displays information about embedded camera configuration files.
/// </summary>
public sealed class CamFileInfoCommand : CommandBase
{
    public override string Name => "Cam File Info";
    public override string Description => "List embedded camera files";
    public override char Key => '6';

    public override void Execute(CommandContext context)
    {
        PrintHeader("EMBEDDED CAMERA FILES");

        var camFiles = CamFileResource.GetAvailableCamFiles().ToList();

        if (camFiles.Count == 0)
        {
            Print("\n  No embedded camera files found.");
        }
        else
        {
            PrintCamFileList(camFiles);
            PrintKnownConstants();
        }

        PrintFooter();
        WaitForKey();
    }

    private void PrintCamFileList(List<string> camFiles)
    {
        Print($"\n  Found {camFiles.Count} embedded camera configuration(s):\n");

        for (int i = 0; i < camFiles.Count; i++)
        {
            var file = camFiles[i];
            Print($"  [{i + 1}] {file}");

            PrintCamFileDetails(file);
            Print();
        }

        Print("  Temp Directory: " + CamFileResource.GetTempDirectory());
    }

    private void PrintCamFileDetails(string filename)
    {
        if (filename.Contains("TC-A160K"))
            Print("      Camera: Crevis TC-A160K");
        if (filename.Contains("FreeRun", StringComparison.OrdinalIgnoreCase))
            Print("      Mode  : Free-run (continuous)");
        if (filename.Contains("RGB8"))
            Print("      Format: RGB 8-bit");
        if (filename.Contains("1TAP"))
            Print("      TAP   : Single TAP");
    }

    private void PrintKnownConstants()
    {
        Print("\n  Known File Constants:");
        Print($"    - KnownCamFiles.TC_A160K_FreeRun_RGB8");
        Print($"      = \"{CamFileResource.KnownCamFiles.TC_A160K_FreeRun_RGB8}\"");
        Print($"    - KnownCamFiles.TC_A160K_FreeRun_1TAP_RGB8");
        Print($"      = \"{CamFileResource.KnownCamFiles.TC_A160K_FreeRun_1TAP_RGB8}\"");
    }
}
