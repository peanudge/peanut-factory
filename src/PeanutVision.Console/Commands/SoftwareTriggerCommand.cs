using PeanutVision.MultiCamDriver;

namespace PeanutVision.Console.Commands;

/// <summary>
/// Demonstrates software-triggered frame capture with polling.
/// </summary>
public sealed class SoftwareTriggerCommand : CommandBase
{
    public override string Name => "Software Trigger";
    public override string Description => "Manual frame capture";
    public override char Key => '3';

    public override void Execute(CommandContext context)
    {
        if (!RequireHardware(context, "run acquisition"))
            return;

        PrintHeader("SOFTWARE TRIGGER MODE");

        try
        {
            var camPath = CamFileResource.GetCamFilePath(CamFileResource.KnownCamFiles.TC_A160K_FreeRun_RGB8);

            using var channel = context.Service.CreateChannel(new GrabChannelOptions
            {
                DriverIndex = 0,
                Connector = "M",
                CamFilePath = camPath,
                SurfaceCount = 2,
                UseCallback = false,
                TriggerMode = McTrigMode.MC_TrigMode_SOFT
            });

            Print($"\n  Image Size: {channel.ImageWidth} x {channel.ImageHeight}");
            Print("  Trigger Mode: SOFTWARE");
            Print("\n  Press SPACE to trigger, any other key to exit...\n");

            channel.StartAcquisition();

            int frameNum = 0;
            while (true)
            {
                var key = ReadKey(true);
                if (key.Key != ConsoleKey.Spacebar)
                    break;

                channel.SendSoftwareTrigger();
                var surface = channel.WaitForFrame(2000);

                if (surface.HasValue)
                {
                    frameNum++;
                    Print($"  Frame {frameNum,3}: {surface.Value.Width}x{surface.Value.Height}, " +
                         $"Size: {surface.Value.Size:N0} bytes, Addr: 0x{surface.Value.Address:X8}");

                    channel.ReleaseSurface(surface.Value);
                }
                else
                {
                    Print($"  Frame {frameNum + 1,3}: TIMEOUT - No frame received");
                }
            }

            channel.StopAcquisition();
            Print($"\n  Captured {frameNum} frames.");
        }
        catch (Exception ex)
        {
            PrintError(ex.Message);
        }

        PrintFooter();
        WaitForKey();
    }
}
