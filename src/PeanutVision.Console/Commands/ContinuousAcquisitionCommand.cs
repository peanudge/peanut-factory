using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Camera;

namespace PeanutVision.Console.Commands;

/// <summary>
/// Demonstrates callback-based continuous frame acquisition.
/// </summary>
public sealed class ContinuousAcquisitionCommand : CommandBase
{
    public override string Name => "Continuous Acquisition";
    public override string Description => "Callback-based acquisition";
    public override char Key => '2';

    public override void Execute(CommandContext context)
    {
        if (!RequireHardware(context, "run acquisition"))
            return;

        PrintHeader("CONTINUOUS ACQUISITION");

        try
        {
            using var channel = context.Service.CreateChannel(CrevisProfiles.TC_A160K_FreeRun_RGB8.ToChannelOptions());

            Print($"\n  Image Size: {channel.ImageWidth} x {channel.ImageHeight}");
            Print($"  Active    : {channel.IsActive}");

            var stats = new AcquisitionStatistics();
            int displayInterval = context.UseMockHal ? 10 : 100;

            channel.FrameAcquired += (sender, args) =>
            {
                stats.RecordFrame();

                if (stats.FrameCount % displayInterval == 0)
                {
                    var snapshot = stats.GetSnapshot();
                    PrintInline($"\r  Frame {snapshot.FrameCount,6}: {snapshot.AverageFps,6:F1} FPS | " +
                          $"Interval: {snapshot.MinFrameIntervalMs,5:F1} - {snapshot.MaxFrameIntervalMs,5:F1} ms    ");
                }
            };

            channel.AcquisitionError += (sender, args) =>
            {
                stats.RecordError();
                Print($"\n  [ERROR] {args.Message}");
            };

            Print("\n  Press any key to start acquisition...");
            ReadKey(true);

            stats.Start();
            channel.StartAcquisition();

            Print("  Acquisition started. Press any key to stop...\n");

            RunAcquisitionLoop(context, channel);

            channel.StopAcquisition();
            stats.Stop();

            PrintAcquisitionSummary(stats);
        }
        catch (Exception ex)
        {
            PrintError(ex.Message);
        }

        PrintFooter();
        WaitForKey();
    }

    private void RunAcquisitionLoop(CommandContext context, GrabChannel channel)
    {
        var mockHal = context.GetMockHal();

        if (mockHal != null)
        {
            var cts = new CancellationTokenSource();
            var simulationTask = Task.Run(async () =>
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    mockHal.SimulateFrameAcquisition(channel.Handle);
                    await Task.Delay(10, cts.Token);
                }
            });

            ReadKey(true);
            cts.Cancel();
            try { simulationTask.Wait(); } catch { }
        }
        else
        {
            ReadKey(true);
        }
    }

    private void PrintAcquisitionSummary(AcquisitionStatistics stats)
    {
        Print("\n");
        Print("  ─────────────────────────────────────────────────────────");
        Print("                    ACQUISITION SUMMARY                    ");
        Print("  ─────────────────────────────────────────────────────────");

        var finalStats = stats.GetSnapshot();
        Print($"  Total Frames : {finalStats.FrameCount}");
        Print($"  Elapsed Time : {finalStats.ElapsedTime.TotalSeconds:F2} seconds");
        Print($"  Average FPS  : {finalStats.AverageFps:F1}");
        Print($"  Dropped      : {finalStats.DroppedFrameCount}");
        Print($"  Errors       : {finalStats.ErrorCount}");
        Print($"  Frame Interval:");
        Print($"    Min: {finalStats.MinFrameIntervalMs:F2} ms");
        Print($"    Max: {finalStats.MaxFrameIntervalMs:F2} ms");
        Print($"    Avg: {finalStats.AverageFrameIntervalMs:F2} ms");
    }
}
