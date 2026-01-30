using PeanutVision.MultiCamDriver;

namespace PeanutVision.Console.Commands;

/// <summary>
/// Camera calibration workflow: FFC, white balance, and exposure control.
/// </summary>
public sealed class CalibrationCommand : CommandBase
{
    public override string Name => "Calibration";
    public override string Description => "FFC and white balance";
    public override char Key => '4';

    public override void Execute(CommandContext context)
    {
        if (!RequireHardware(context, "run calibration"))
            return;

        PrintHeader("CAMERA CALIBRATION");

        try
        {
            using var channel = context.Service.CreateChannelForTC_A160K(driverIndex: 0);

            RunFlatFieldCalibration(channel);
            RunWhiteBalanceCalibration(channel);
            RunExposureSettings(channel);

            Print("\n  Calibration sequence complete.");
        }
        catch (Exception ex)
        {
            PrintError(ex.Message);
        }

        PrintFooter();
        WaitForKey();
    }

    private void RunFlatFieldCalibration(GrabChannel channel)
    {
        PrintSection("FLAT FIELD CORRECTION (FFC)");

        // Black Calibration
        Print("\n  1. BLACK CALIBRATION");
        Print("     - Cover the lens completely");
        Print("     - Ensure NO light reaches the sensor");
        PrintInline("\n     Press ENTER when ready (or 'S' to skip): ");

        var key = ReadKey();
        Print();

        if (key.Key != ConsoleKey.S)
        {
            try
            {
                channel.PerformBlackCalibration();
                PrintSuccess("Black calibration completed.");
            }
            catch (MultiCamException ex)
            {
                Print($"     [FAILED] {ex.Message}");
            }
        }
        else
        {
            PrintSkipped();
        }

        // White Calibration
        Print("\n  2. WHITE CALIBRATION");
        Print("     - Point camera at uniform white surface");
        Print("     - Target ~200 DN (Digital Numbers) brightness");
        PrintInline("\n     Press ENTER when ready (or 'S' to skip): ");

        key = ReadKey();
        Print();

        if (key.Key != ConsoleKey.S)
        {
            try
            {
                channel.PerformWhiteCalibration();
                PrintSuccess("White calibration completed.");

                channel.SetFlatFieldCorrection(true);
                PrintSuccess("Flat field correction ENABLED.");
            }
            catch (MultiCamException ex)
            {
                Print($"     [FAILED] {ex.Message}");
            }
        }
        else
        {
            PrintSkipped();
        }
    }

    private void RunWhiteBalanceCalibration(GrabChannel channel)
    {
        PrintSection("WHITE BALANCE");

        try
        {
            var (r, g, b) = channel.GetWhiteBalanceRatios();
            Print($"\n     Current Ratios - R: {r:F3}, G: {g:F3}, B: {b:F3}");

            Print("\n     - Point camera at neutral gray/white reference");
            PrintInline("     Press ENTER to auto-adjust (or 'S' to skip): ");

            var key = ReadKey();
            Print();

            if (key.Key != ConsoleKey.S)
            {
                channel.PerformWhiteBalanceOnce();
                (r, g, b) = channel.GetWhiteBalanceRatios();
                PrintSuccess($"New Ratios - R: {r:F3}, G: {g:F3}, B: {b:F3}");
            }
            else
            {
                PrintSkipped();
            }
        }
        catch (MultiCamException ex)
        {
            Print($"     [ERROR] {ex.Message}");
        }
    }

    private void RunExposureSettings(GrabChannel channel)
    {
        PrintSection("EXPOSURE SETTINGS");

        try
        {
            var (minExp, maxExp) = channel.GetExposureRange();
            var currentExp = channel.GetExposureUs();

            Print($"\n     Range  : {minExp:F0} - {maxExp:F0} µs");
            Print($"     Current: {currentExp:F0} µs");

            PrintInline("\n     Enter new exposure in µs (or press ENTER to skip): ");
            var input = ReadLine();

            if (!string.IsNullOrWhiteSpace(input) && double.TryParse(input, out double newExp))
            {
                channel.SetExposureUs(newExp);
                currentExp = channel.GetExposureUs();
                PrintSuccess($"Exposure set to {currentExp:F0} µs");
            }
        }
        catch (MultiCamException ex)
        {
            PrintInfo($"Exposure control: {ex.Message}");
        }
    }
}
