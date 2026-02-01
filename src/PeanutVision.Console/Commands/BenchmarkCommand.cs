using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Camera;

namespace PeanutVision.Console.Commands;

/// <summary>
/// Performance benchmark measuring acquisition speed and statistics.
/// </summary>
public sealed class BenchmarkCommand : CommandBase
{
    public override string Name => "Performance Benchmark";
    public override string Description => "Measure acquisition speed";
    public override char Key => '5';

    public override void Execute(CommandContext context)
    {
        if (!RequireHardware(context, "run benchmark"))
            return;

        PrintHeader("PERFORMANCE BENCHMARK");

        int targetFrames = GetTargetFrameCount();
        Print($"\n  Benchmarking {targetFrames} frames...");

        try
        {
            using var channel = context.Service.CreateChannel(CrevisProfiles.TC_A160K_FreeRun_RGB8.ToChannelOptions());

            var stats = new AcquisitionStatistics();
            int receivedFrames = 0;

            channel.FrameAcquired += (sender, args) =>
            {
                stats.RecordFrame();
                receivedFrames++;

                if (receivedFrames % 100 == 0)
                {
                    PrintInline($"\r  Progress: {receivedFrames,5}/{targetFrames} ({100.0 * receivedFrames / targetFrames:F0}%)    ");
                }
            };

            stats.Start();
            channel.StartAcquisition(targetFrames);

            WaitForFrames(context, channel, targetFrames, ref receivedFrames);

            channel.StopAcquisition();
            stats.Stop();

            PrintBenchmarkResults(stats, targetFrames, channel);
        }
        catch (Exception ex)
        {
            PrintError(ex.Message);
        }

        PrintFooter();
        WaitForKey();
    }

    private int GetTargetFrameCount()
    {
        PrintInline("\n  Enter number of frames to capture (default 1000): ");
        var input = ReadLine();

        if (!string.IsNullOrWhiteSpace(input) && int.TryParse(input, out int parsed) && parsed > 0)
        {
            return parsed;
        }
        return 1000;
    }

    private void WaitForFrames(CommandContext context, GrabChannel channel, int targetFrames, ref int receivedFrames)
    {
        var mockHal = context.GetMockHal();

        if (mockHal != null)
        {
            while (receivedFrames < targetFrames)
            {
                mockHal.SimulateFrameAcquisition(channel.Handle);
                Thread.Sleep(1);
            }
        }
        else
        {
            while (channel.IsActive && receivedFrames < targetFrames)
            {
                Thread.Sleep(100);
            }
        }
    }

    private void PrintBenchmarkResults(AcquisitionStatistics stats, int targetFrames, GrabChannel channel)
    {
        var finalStats = stats.GetSnapshot();

        Print("\n");
        Print("  ╔═══════════════════════════════════════════════════════════╗");
        Print("  ║                 BENCHMARK RESULTS                         ║");
        Print("  ╠═══════════════════════════════════════════════════════════╣");
        Print($"  ║  Target Frames    : {targetFrames,10}                        ║");
        Print($"  ║  Captured Frames  : {finalStats.FrameCount,10}                        ║");
        Print($"  ║  Elapsed Time     : {finalStats.ElapsedTime.TotalSeconds,10:F3} sec                   ║");
        Print($"  ║  Average FPS      : {finalStats.AverageFps,10:F1}                        ║");
        Print($"  ║  Dropped Frames   : {finalStats.DroppedFrameCount,10}                        ║");
        Print($"  ║  Errors           : {finalStats.ErrorCount,10}                        ║");
        Print("  ╠═══════════════════════════════════════════════════════════╣");
        Print("  ║  Frame Interval Statistics:                               ║");
        Print($"  ║    Minimum        : {finalStats.MinFrameIntervalMs,10:F3} ms                   ║");
        Print($"  ║    Maximum        : {finalStats.MaxFrameIntervalMs,10:F3} ms                   ║");
        Print($"  ║    Average        : {finalStats.AverageFrameIntervalMs,10:F3} ms                   ║");
        Print("  ╚═══════════════════════════════════════════════════════════╝");

        // Data rate calculation
        long bytesPerFrame = channel.ImageWidth * channel.ImageHeight * 3; // Assuming RGB8
        double mbps = (bytesPerFrame * finalStats.AverageFps) / (1024 * 1024);
        Print($"\n  Data Rate: ~{mbps:F1} MB/s ({channel.ImageWidth}x{channel.ImageHeight} RGB8)");
    }
}
