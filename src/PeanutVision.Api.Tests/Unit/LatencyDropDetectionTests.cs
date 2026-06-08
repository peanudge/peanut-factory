using Microsoft.Extensions.Logging.Abstractions;
using System.Runtime.InteropServices;
using PeanutVision.Api.Services;
using PeanutVision.Api.Tests.Infrastructure;
using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Camera;
using PeanutVision.MultiCamDriver.Hal;
using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Api.Tests.Unit;

/// <summary>
/// Verifies that AcquisitionManager matches trigger timestamps to frames correctly
/// when frame drops occur (CLUSTER_UNAVAILABLE) or triggers expire (expiry fallback).
/// </summary>
public class LatencyDropDetectionTests : IDisposable
{
    protected readonly MockMultiCamHAL _mockHal;
    protected readonly GrabService _grabService;
    protected readonly SpyLatencyService _spy;
    protected readonly AcquisitionManager _manager;
    private readonly IntPtr _surfaceMemory;

    public LatencyDropDetectionTests()
    {
        _mockHal = new MockMultiCamHAL();

        var bufferSize = _mockHal.Configuration.DefaultImageWidth
            * _mockHal.Configuration.DefaultImageHeight * 3;
        _surfaceMemory = Marshal.AllocHGlobal(bufferSize);
        var zeros = new byte[bufferSize];
        Marshal.Copy(zeros, 0, _surfaceMemory, bufferSize);
        _mockHal.Configuration.SimulatedSurfaceAddress = _surfaceMemory;

        _grabService = new GrabService(_mockHal);
        _grabService.Initialize();
        _spy = new SpyLatencyService();
        _manager = new AcquisitionManager(
            _grabService,
            TestCamFileHelper.GetOrCreate(),
            _spy,
            NullLogger<AcquisitionManager>.Instance);
    }

    public void Dispose()
    {
        _manager.Dispose();
        _grabService.Dispose();
        if (_surfaceMemory != IntPtr.Zero)
        {
            Marshal.FreeHGlobal(_surfaceMemory);
        }
    }

    private Task WaitForFrameAsync()
    {
        var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        _manager.FrameAcquired += Handler;
        return tcs.Task.WaitAsync(TimeSpan.FromSeconds(5));

        void Handler(object? sender, EventArgs e)
        {
            _manager.FrameAcquired -= Handler;
            tcs.TrySetResult();
        }
    }

    // ── Tests ─────────────────────────────────────────────────────────────────

    public class Given_cluster_unavailable_signal_before_frame : LatencyDropDetectionTests
    {
        /// <summary>
        /// Trigger1 fires → CLUSTER_UNAVAILABLE (frame dropped) → Trigger2 fires → frame arrives.
        /// The latency record must use Trigger2's timestamp, not Trigger1's.
        /// </summary>
        [Fact]
        public async Task Then_latency_is_matched_to_second_trigger()
        {
            _manager.Start(new AcquisitionConfig(new ProfileId("crevis-tc-a160k-softtrig-rgb8.cam")));
            var channelHandle = _mockHal.CallLog.LastCreatedHandle;

            // Trigger1 → enqueues t1
            _manager.Trigger();
            var t1 = DateTimeOffset.UtcNow;

            // Frame for trigger1 is dropped
            _mockHal.SimulateAcquisitionError(channelHandle, McSignal.MC_SIG_CLUSTER_UNAVAILABLE);

            // Small gap so t2 is measurably after t1
            await Task.Delay(20);

            // Trigger2 → enqueues t2, then frame arrives
            var frameTask = WaitForFrameAsync();
            var t2Before = DateTimeOffset.UtcNow;
            _manager.Trigger();
            _mockHal.SimulateFrameAcquisition(channelHandle);

            await frameTask;

            Assert.Single(_spy.Recorded);
            Assert.True(_spy.Recorded[0].TriggerSentAt >= t2Before,
                $"Expected triggerSentAt {_spy.Recorded[0].TriggerSentAt} >= t2Before {t2Before}");
        }
    }

    public class Given_normal_trigger_and_frame : LatencyDropDetectionTests
    {
        [Fact]
        public async Task Then_latency_is_recorded()
        {
            _manager.Start(new AcquisitionConfig(new ProfileId("crevis-tc-a160k-softtrig-rgb8.cam")));
            var channelHandle = _mockHal.CallLog.LastCreatedHandle;

            var frameTask = WaitForFrameAsync();
            _manager.Trigger();
            _mockHal.SimulateFrameAcquisition(channelHandle);

            await frameTask;

            Assert.Single(_spy.Recorded);
            Assert.True(_spy.Recorded[0].LatencyMs >= 0);
        }
    }

    public class Given_stale_trigger_and_frame : LatencyDropDetectionTests
    {
        /// <summary>
        /// Trigger fires, but the matching frame arrives after maxExpectedLatency has passed.
        /// The stale trigger should be discarded: no latency record is written.
        /// </summary>
        [Fact]
        public async Task Then_stale_trigger_is_discarded_and_no_latency_recorded()
        {
            // Very short expiry so any real-time delay will exceed it
            var manager = new AcquisitionManager(
                _grabService,
                TestCamFileHelper.GetOrCreate(),
                _spy,
                NullLogger<AcquisitionManager>.Instance,
                maxExpectedLatency: TimeSpan.FromMilliseconds(1));

            manager.Start(new AcquisitionConfig(new ProfileId("crevis-tc-a160k-softtrig-rgb8.cam")));
            var channelHandle = _mockHal.CallLog.LastCreatedHandle;

            manager.Trigger();

            // Wait longer than maxExpectedLatency so the trigger goes stale
            await Task.Delay(50);

            var frameTask = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            manager.FrameAcquired += (_, _) => frameTask.TrySetResult();

            _mockHal.SimulateFrameAcquisition(channelHandle);

            await frameTask.Task.WaitAsync(TimeSpan.FromSeconds(5));
            manager.Dispose();

            Assert.Empty(_spy.Recorded);
        }
    }
}
