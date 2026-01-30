namespace PeanutVision.Console;

/// <summary>
/// Entry point for PeanutVision MultiCam Driver Demo Console.
/// </summary>
public static class Program
{
    public static void Main(string[] args)
    {
        PrintBanner();

        bool useMockHal = args.Contains("--mock") || args.Contains("-m");

        if (useMockHal)
        {
            WriteLine("[INFO] Running in MOCK mode (no hardware required)");
            WriteLine();
        }

        try
        {
            using var context = CommandContext.Create(useMockHal);
            var runner = new CommandRunner(context);
            runner.Run();
        }
        catch (Exception ex)
        {
            WriteLine($"\n[ERROR] {ex.Message}");
            PrintUsageHint();
        }
    }

    private static void PrintBanner()
    {
        WriteLine("╔══════════════════════════════════════════════════════════════╗");
        WriteLine("║       PeanutVision MultiCam Driver - Demo Console            ║");
        WriteLine("║       Euresys Grablink Full (PC1622) + Crevis TC-A160K       ║");
        WriteLine("╚══════════════════════════════════════════════════════════════╝");
        WriteLine();
    }

    private static void PrintUsageHint()
    {
        WriteLine("\nTip: Run with --mock flag to test without hardware:");
        WriteLine("  dotnet run -- --mock");
    }
}
