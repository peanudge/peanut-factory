using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Camera;

namespace PeanutVision.Console.Commands;

/// <summary>
/// Captures a single frame and saves it to a file (PNG, BMP, or RAW).
/// </summary>
public sealed class SaveImageCommand : CommandBase
{
    public override string Name => "Save Image";
    public override string Description => "Capture & save to file";
    public override char Key => '7';

    public override void Execute(CommandContext context)
    {
        if (!RequireHardware(context, "capture image"))
            return;

        PrintHeader("SAVE IMAGE");

        try
        {
            Print("\n  Supported formats: PNG, BMP, RAW");
            Print("  Example: capture.png, image.bmp, frame.raw\n");
            PrintInline("  Enter filename (or press Enter to cancel): ");

            var filename = ReadLine()?.Trim();
            if (string.IsNullOrEmpty(filename))
            {
                Print("\n  Cancelled.");
                PrintFooter();
                WaitForKey();
                return;
            }

            // Validate extension
            var ext = Path.GetExtension(filename).ToLowerInvariant();
            if (ext != ".png" && ext != ".bmp" && ext != ".raw")
            {
                PrintError($"Unsupported format: {ext}");
                Print("  Supported formats: .png, .bmp, .raw");
                PrintFooter();
                WaitForKey();
                return;
            }

            // Use absolute path if relative
            var filePath = Path.IsPathRooted(filename)
                ? filename
                : Path.Combine(Environment.CurrentDirectory, filename);

            Print($"\n  Output: {filePath}");
            Print("  Initializing channel...");

            var profile = CrevisProfiles.TC_A160K_SoftwareTrigger_RGB8;
            var options = profile.ToChannelOptions(McTrigMode.MC_TrigMode_SOFT, useCallback: false);

            using var channel = context.Service.CreateChannel(options);

            Print($"  Image Size: {channel.ImageWidth} x {channel.ImageHeight}");
            Print("  Starting acquisition...");

            channel.StartAcquisition();

            Print("  Sending software trigger...");
            channel.SendSoftwareTrigger();

            Print("  Waiting for frame...");
            var surface = channel.WaitForFrame(5000);

            if (surface.HasValue)
            {
                Print($"  Frame received: {surface.Value.Width}x{surface.Value.Height}, " +
                     $"{surface.Value.Size:N0} bytes");

                Print($"  Saving as {ext.ToUpperInvariant().TrimStart('.')}...");

                // Copy data before releasing surface
                var data = surface.Value.ToArray();
                channel.ReleaseSurface(surface.Value);

                ImageSaver.Save(data, surface.Value.Width, surface.Value.Height,
                    surface.Value.Pitch, filePath);

                PrintSuccess($"Image saved: {filePath}");

                var fileInfo = new FileInfo(filePath);
                if (fileInfo.Exists)
                {
                    Print($"  File size: {fileInfo.Length:N0} bytes");
                }
            }
            else
            {
                PrintError("Timeout waiting for frame");
            }

            channel.StopAcquisition();
        }
        catch (Exception ex)
        {
            PrintError(ex.Message);
        }

        PrintFooter();
        WaitForKey();
    }
}
