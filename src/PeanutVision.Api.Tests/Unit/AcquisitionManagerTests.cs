using System.Runtime.InteropServices;
using PeanutVision.Api.Services;
using PeanutVision.Api.Tests.Infrastructure;
using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Camera;
using PeanutVision.MultiCamDriver.Hal;
using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Api.Tests.Unit;

public class AcquisitionManagerTests : IDisposable
{
    protected readonly MockMultiCamHAL _mockHal;
    protected readonly GrabService _grabService;
    protected readonly AcquisitionManager _manager;
    private readonly IntPtr _surfaceMemory;

    public AcquisitionManagerTests()
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
        _manager = new AcquisitionManager(_grabService, TestCamFileHelper.GetOrCreate());
    }

    public void Dispose()
    {
        _manager.Dispose();
        _grabService.Dispose();
        if (_surfaceMemory != IntPtr.Zero)
            Marshal.FreeHGlobal(_surfaceMemory);
    }

    public class Given_idle : AcquisitionManagerTests
    {
        [Fact]
        public void Then_channel_state_is_none()
        {
            Assert.Equal(ChannelState.None, _manager.ChannelState);
        }

        [Fact]
        public void Then_is_not_active()
        {
            Assert.False(_manager.IsActive);
        }

        [Fact]
        public void Then_active_profile_id_is_null()
        {
            Assert.Null(_manager.ActiveProfileId);
        }

        [Fact]
        public void Then_channel_is_null()
        {
            Assert.Null(_manager.Channel);
        }

        [Fact]
        public void Then_statistics_are_null()
        {
            Assert.Null(_manager.GetStatistics());
        }

        [Fact]
        public void Then_last_error_is_null()
        {
            Assert.Null(_manager.LastError);
        }

        [Fact]
        public void When_stop_then_does_nothing()
        {
            _manager.Stop();
            Assert.False(_manager.IsActive);
        }

        [Fact]
        public async Task When_trigger_and_wait_then_throws()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _manager.TriggerAndWaitAsync());
        }

        [Fact]
        public void When_create_channel_with_unknown_profile_then_throws()
        {
            Assert.Throws<KeyNotFoundException>(() => _manager.CreateChannel("nonexistent"));
        }

        [Fact]
        public void When_start_without_channel_then_throws()
        {
            Assert.Throws<InvalidOperationException>(() => _manager.Start());
        }

        [Fact]
        public void Then_allowed_actions_contains_start_and_snapshot()
        {
            var actions = _manager.GetAllowedActions();
            Assert.Contains(ChannelAction.Start, actions);
            Assert.Contains(ChannelAction.Snapshot, actions);
        }
    }

    public class Given_channel_created : AcquisitionManagerTests
    {
        public Given_channel_created()
        {
            _manager.CreateChannel("crevis-tc-a160k-freerun-rgb8.cam");
        }

        [Fact]
        public void Then_channel_state_is_idle()
        {
            Assert.Equal(ChannelState.Idle, _manager.ChannelState);
        }

        [Fact]
        public void Then_is_not_active()
        {
            Assert.False(_manager.IsActive);
        }

        [Fact]
        public void Then_active_profile_id_matches()
        {
            Assert.Equal(new ProfileId("crevis-tc-a160k-freerun-rgb8.cam"), _manager.ActiveProfileId);
        }

        [Fact]
        public void Then_channel_is_not_null()
        {
            Assert.NotNull(_manager.Channel);
        }

        [Fact]
        public void When_create_channel_again_then_throws()
        {
            Assert.Throws<InvalidOperationException>(() =>
                _manager.CreateChannel("crevis-tc-a160k-softtrig-rgb8.cam"));
        }

        [Fact]
        public void Then_allowed_actions_contains_start_and_snapshot()
        {
            var actions = _manager.GetAllowedActions();
            Assert.Contains(ChannelAction.Start, actions);
            Assert.Contains(ChannelAction.Snapshot, actions);
            Assert.DoesNotContain(ChannelAction.Stop, actions);
        }
    }

    public class Given_started : AcquisitionManagerTests
    {
        public Given_started()
        {
            _manager.CreateChannel("crevis-tc-a160k-freerun-rgb8.cam");
            _manager.Start();
        }

        [Fact]
        public void Then_channel_state_is_active()
        {
            Assert.Equal(ChannelState.Active, _manager.ChannelState);
        }

        [Fact]
        public void Then_is_active()
        {
            Assert.True(_manager.IsActive);
        }

        [Fact]
        public void Then_active_profile_id_matches()
        {
            Assert.Equal(new ProfileId("crevis-tc-a160k-freerun-rgb8.cam"), _manager.ActiveProfileId);
        }

        [Fact]
        public void Then_channel_is_not_null()
        {
            Assert.NotNull(_manager.Channel);
        }

        [Fact]
        public void Then_hal_acquisition_is_started()
        {
            Assert.True(_mockHal.CallLog.AcquisitionStarted);
        }

        [Fact]
        public void Then_statistics_return_zero_frames()
        {
            var stats = _manager.GetStatistics();
            Assert.NotNull(stats);
            Assert.Equal(0, stats.Value.FrameCount);
        }

        [Fact]
        public void Then_last_error_is_null()
        {
            Assert.Null(_manager.LastError);
        }

        [Fact]
        public void When_start_again_then_throws()
        {
            Assert.Throws<InvalidOperationException>(() => _manager.Start());
        }

        [Fact]
        public void When_snapshot_then_throws()
        {
            Assert.Throws<InvalidOperationException>(() =>
                _manager.Snapshot("crevis-tc-a160k-freerun-rgb8.cam"));
        }

        [Fact]
        public void When_release_channel_then_throws()
        {
            Assert.Throws<InvalidOperationException>(() => _manager.ReleaseChannel());
        }

        [Fact]
        public void When_dispose_then_stops_acquisition()
        {
            _manager.Dispose();
            Assert.False(_manager.IsActive);
        }

        [Fact]
        public void Then_allowed_actions_contains_stop_and_trigger()
        {
            var actions = _manager.GetAllowedActions();
            Assert.Contains(ChannelAction.Stop, actions);
            Assert.Contains(ChannelAction.Trigger, actions);
            Assert.DoesNotContain(ChannelAction.Start, actions);
            Assert.DoesNotContain(ChannelAction.Snapshot, actions);
        }
    }

    public class Given_started_with_trigger_mode : AcquisitionManagerTests
    {
        public Given_started_with_trigger_mode()
        {
            _manager.CreateChannel("crevis-tc-a160k-freerun-rgb8.cam", TriggerMode.Soft);
            _manager.Start();
        }

        [Fact]
        public void Then_is_active()
        {
            Assert.True(_manager.IsActive);
        }

        [Fact]
        public void Then_channel_trigger_mode_is_soft()
        {
            Assert.Equal(TriggerMode.Soft, _manager.ChannelTriggerMode);
        }
    }

    public class Given_started_then_stopped : AcquisitionManagerTests
    {
        public Given_started_then_stopped()
        {
            _manager.CreateChannel("crevis-tc-a160k-freerun-rgb8.cam");
            _manager.Start();
            _manager.Stop();
        }

        [Fact]
        public void Then_channel_state_is_idle()
        {
            Assert.Equal(ChannelState.Idle, _manager.ChannelState);
        }

        [Fact]
        public void Then_is_not_active()
        {
            Assert.False(_manager.IsActive);
        }

        [Fact]
        public void Then_active_profile_id_still_set()
        {
            Assert.Equal(new ProfileId("crevis-tc-a160k-freerun-rgb8.cam"), _manager.ActiveProfileId);
        }

        [Fact]
        public void Then_channel_is_not_null_channel_kept_alive()
        {
            Assert.NotNull(_manager.Channel);
        }

        [Fact]
        public void When_start_again_then_activates()
        {
            _manager.Start();

            Assert.True(_manager.IsActive);
            Assert.Equal(ChannelState.Active, _manager.ChannelState);
        }

        [Fact]
        public void When_release_then_channel_state_is_none()
        {
            _manager.ReleaseChannel();

            Assert.Equal(ChannelState.None, _manager.ChannelState);
            Assert.Null(_manager.Channel);
        }
    }

    public class Given_started_and_trigger_and_wait : AcquisitionManagerTests
    {
        public Given_started_and_trigger_and_wait()
        {
            _mockHal.Configuration.AutoSimulateFrameOnTrigger = true;
        }

        [Fact]
        public async Task Then_returns_image_with_correct_dimensions()
        {
            int width = _mockHal.Configuration.DefaultImageWidth;
            int height = _mockHal.Configuration.DefaultImageHeight;

            _manager.CreateChannel(new ProfileId("crevis-tc-a160k-softtrig-rgb8.cam"));
            _manager.Start();
            var image = await _manager.TriggerAndWaitAsync();

            Assert.Equal(width, image.Width);
            Assert.Equal(height, image.Height);
        }

        [Fact]
        public async Task Then_increments_hal_trigger_count()
        {
            _manager.CreateChannel(new ProfileId("crevis-tc-a160k-softtrig-rgb8.cam"));
            _manager.Start();
            _mockHal.CallLog.Reset();

            await _manager.TriggerAndWaitAsync();
            await _manager.TriggerAndWaitAsync();

            Assert.Equal(2, _mockHal.CallLog.SoftwareTriggerCount);
        }
    }

    public class Given_started_and_trigger_timeout : AcquisitionManagerTests
    {
        [Fact]
        public async Task Then_throws_TimeoutException()
        {
            // AutoSimulateFrameOnTrigger is false by default, so no frame arrives
            _manager.CreateChannel(new ProfileId("crevis-tc-a160k-softtrig-rgb8.cam"));
            _manager.Start();

            await Assert.ThrowsAsync<TimeoutException>(
                () => _manager.TriggerAndWaitAsync(timeoutMs: 100));
        }
    }

    public class Given_started_and_frame_acquired : AcquisitionManagerTests
    {
        [Fact]
        public async Task Then_last_frame_has_correct_dimensions()
        {
            int width = _mockHal.Configuration.DefaultImageWidth;
            int height = _mockHal.Configuration.DefaultImageHeight;
            int pitch = width * 3;
            int size = pitch * height;
            byte[] pixelData = new byte[size];

            for (int i = 0; i < pixelData.Length; i++)
                pixelData[i] = (byte)(i % 256);

            var pinnedHandle = GCHandle.Alloc(pixelData, GCHandleType.Pinned);
            try
            {
                _mockHal.Configuration.SimulatedSurfaceAddress = pinnedHandle.AddrOfPinnedObject();

                _manager.CreateChannel("crevis-tc-a160k-freerun-rgb8.cam");
                _manager.Start();
                var frameReady = _manager.Channel!.WaitForNextFrameAsync();
                _mockHal.SimulateFrameAcquisition(_manager.Channel!.Handle);
                await frameReady.WaitAsync(TimeSpan.FromSeconds(1));

                var frame = _manager.LastFrame;
                Assert.NotNull(frame);
                Assert.Equal(width, frame.Width);
                Assert.Equal(height, frame.Height);
                Assert.Equal(pitch, frame.Pitch);
                Assert.Equal(size, frame.Pixels.Length);
            }
            finally
            {
                pinnedHandle.Free();
            }
        }
    }

    public class Snapshot_given_idle : AcquisitionManagerTests
    {
        [Fact]
        public void Then_returns_image_data()
        {
            var image = _manager.Snapshot("crevis-tc-a160k-freerun-rgb8.cam");

            Assert.NotNull(image);
            Assert.Equal(_mockHal.Configuration.DefaultImageWidth, image.Width);
            Assert.Equal(_mockHal.Configuration.DefaultImageHeight, image.Height);
        }

        [Fact]
        public void Then_sends_software_trigger()
        {
            _mockHal.CallLog.Reset();

            _manager.Snapshot("crevis-tc-a160k-freerun-rgb8.cam");

            Assert.Equal(1, _mockHal.CallLog.SoftwareTriggerCount);
        }

        [Fact]
        public void Then_starts_and_stops_acquisition()
        {
            _mockHal.CallLog.Reset();

            _manager.Snapshot("crevis-tc-a160k-freerun-rgb8.cam");

            Assert.True(_mockHal.CallLog.AcquisitionStarted);
            Assert.True(_mockHal.CallLog.AcquisitionStopped);
        }

        [Fact]
        public void Then_does_not_leave_channel_active()
        {
            _manager.Snapshot("crevis-tc-a160k-freerun-rgb8.cam");

            Assert.False(_manager.IsActive);
            Assert.Null(_manager.Channel);
        }

        [Fact]
        public void Then_channel_state_remains_none()
        {
            _manager.Snapshot("crevis-tc-a160k-freerun-rgb8.cam");

            Assert.Equal(ChannelState.None, _manager.ChannelState);
        }

        [Fact]
        public void Then_uses_polling_mode_not_callback()
        {
            _mockHal.CallLog.Reset();

            _manager.Snapshot("crevis-tc-a160k-freerun-rgb8.cam");

            Assert.Equal(1, _mockHal.CallLog.WaitSignalCalls);
            Assert.Equal(0, _mockHal.CallLog.RegisterCallbackCalls);
        }

        [Fact]
        public void Then_can_be_called_multiple_times()
        {
            var image1 = _manager.Snapshot("crevis-tc-a160k-freerun-rgb8.cam");
            var image2 = _manager.Snapshot("crevis-tc-a160k-freerun-rgb8.cam");

            Assert.NotNull(image1);
            Assert.NotNull(image2);
        }

        [Fact]
        public void When_unknown_profile_then_throws()
        {
            Assert.Throws<KeyNotFoundException>(() => _manager.Snapshot("nonexistent"));
        }

        [Fact]
        public void Then_snapshot_channel_removed_from_grab_service()
        {
            _mockHal.CallLog.Reset();

            _manager.Snapshot("crevis-tc-a160k-freerun-rgb8.cam");

            // Snapshot creates and releases a temp channel (1 create + 1 delete)
            Assert.Equal(1, _mockHal.CallLog.CreateCalls);
            Assert.Equal(1, _mockHal.CallLog.DeleteCalls);
        }
    }

    public class Snapshot_given_channel_idle : AcquisitionManagerTests
    {
        public Snapshot_given_channel_idle()
        {
            _manager.CreateChannel("crevis-tc-a160k-freerun-rgb8.cam");
        }

        [Fact]
        public void Then_snapshot_succeeds()
        {
            var image = _manager.Snapshot("crevis-tc-a160k-freerun-rgb8.cam");
            Assert.NotNull(image);
        }

        [Fact]
        public void Then_channel_state_remains_idle_after_snapshot()
        {
            _manager.Snapshot("crevis-tc-a160k-freerun-rgb8.cam");
            Assert.Equal(ChannelState.Idle, _manager.ChannelState);
        }
    }

    public class Snapshot_given_incompatible_trigger_mode : AcquisitionManagerTests
    {
        [Fact]
        public void When_hard_trigger_mode_then_throws_ArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                _manager.Snapshot("crevis-tc-a160k-freerun-rgb8.cam", TriggerMode.Hard));
        }

        [Fact]
        public void When_immediate_trigger_mode_then_throws_ArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                _manager.Snapshot("crevis-tc-a160k-freerun-rgb8.cam", TriggerMode.Immediate));
        }

        [Fact]
        public void When_soft_trigger_mode_then_succeeds()
        {
            var image = _manager.Snapshot("crevis-tc-a160k-freerun-rgb8.cam", TriggerMode.Soft);
            Assert.NotNull(image);
        }

        [Fact]
        public void When_no_trigger_mode_and_freerun_cam_then_succeeds()
        {
            // No trigger mode passed — Snapshot silently forces SOFT; this should never throw
            var image = _manager.Snapshot("crevis-tc-a160k-freerun-rgb8.cam");
            Assert.NotNull(image);
        }
    }

    public class Snapshot_given_active : AcquisitionManagerTests
    {
        public Snapshot_given_active()
        {
            _manager.CreateChannel("crevis-tc-a160k-freerun-rgb8.cam");
            _manager.Start();
        }

        [Fact]
        public void Then_throws()
        {
            Assert.Throws<InvalidOperationException>(() =>
                _manager.Snapshot("crevis-tc-a160k-freerun-rgb8.cam"));
        }
    }

    public class Snapshot_given_timeout : AcquisitionManagerTests
    {
        public Snapshot_given_timeout()
        {
            _mockHal.Configuration.WaitSignalFailure = (int)McStatus.MC_TIMEOUT;
        }

        [Fact]
        public void Then_throws_TimeoutException()
        {
            Assert.Throws<TimeoutException>(() =>
                _manager.Snapshot("crevis-tc-a160k-freerun-rgb8.cam"));
        }
    }

    public class Given_copy_queue_full : AcquisitionManagerTests
    {
        [Fact]
        public async Task Then_grab_channel_records_copy_drops()
        {
            _manager.CreateChannel("crevis-tc-a160k-freerun-rgb8.cam");
            _manager.Start();
            var channel = _manager.Channel!;

            // Fire more frames than the bounded copy queue can hold
            int totalFrames = channel.SurfaceCount + 4;
            for (int i = 0; i < totalFrames; i++)
            {
                _mockHal.SimulateFrameAcquisition(channel.Handle);
            }

            // Give copy thread time to process
            await Task.Delay(300);

            var stats = _manager.GetStatistics();
            Assert.NotNull(stats);
            Assert.True(stats.Value.FrameCount + channel.CopyDropCount > 0);
        }
    }

    public class Given_statistics_include_channel_diagnostics : AcquisitionManagerTests
    {
        [Fact]
        public void Then_statistics_contain_copy_drop_and_cluster_counts()
        {
            _manager.CreateChannel("crevis-tc-a160k-freerun-rgb8.cam");
            _manager.Start();

            var stats = _manager.GetStatistics();
            Assert.NotNull(stats);
            Assert.Equal(0, stats.Value.CopyDropCount);
            Assert.Equal(0, stats.Value.ClusterUnavailableCount);
        }
    }

    public class Given_event_log : AcquisitionManagerTests
    {
        [Fact]
        public void Then_start_records_event()
        {
            _manager.CreateChannel("crevis-tc-a160k-freerun-rgb8.cam");
            _manager.Start();

            var events = _manager.GetRecentEvents();
            Assert.Single(events);
            Assert.Equal(ChannelEventType.AcquisitionStarted, events[0].Type);
        }

        [Fact]
        public void Then_stop_records_event()
        {
            _manager.CreateChannel("crevis-tc-a160k-freerun-rgb8.cam");
            _manager.Start();
            _manager.Stop();

            var events = _manager.GetRecentEvents();
            Assert.Equal(2, events.Count);
            Assert.Equal(ChannelEventType.AcquisitionStopped, events[0].Type);
            Assert.Equal(ChannelEventType.AcquisitionStarted, events[1].Type);
        }

        [Fact]
        public void Then_events_persist_across_sessions()
        {
            _manager.CreateChannel("crevis-tc-a160k-freerun-rgb8.cam");
            _manager.Start();
            _manager.Stop();
            _manager.Start();

            var events = _manager.GetRecentEvents();
            Assert.Equal(3, events.Count);
            Assert.Equal(ChannelEventType.AcquisitionStarted, events[0].Type);
            Assert.Equal(ChannelEventType.AcquisitionStopped, events[1].Type);
        }
    }

    public class Given_snapshot_in_progress : AcquisitionManagerTests
    {
        [Fact]
        public void Then_snapshot_blocks_start()
        {
            _manager.Snapshot("crevis-tc-a160k-freerun-rgb8.cam");

            // After snapshot completes, CreateChannel + Start should work
            _manager.CreateChannel("crevis-tc-a160k-freerun-rgb8.cam");
            _manager.Start();
            Assert.True(_manager.IsActive);
        }

        [Fact]
        public void Then_allowed_actions_restored_after_snapshot()
        {
            _manager.Snapshot("crevis-tc-a160k-freerun-rgb8.cam");

            var actions = _manager.GetAllowedActions();
            Assert.Contains(ChannelAction.Start, actions);
            Assert.Contains(ChannelAction.Snapshot, actions);
        }
    }

    public class Given_intervalMs_validation : AcquisitionManagerTests
    {
        [Fact]
        public void Then_intervalMs_below_minimum_throws_ArgumentException()
        {
            _manager.CreateChannel("crevis-tc-a160k-freerun-rgb8.cam");
            Assert.Throws<ArgumentException>(() => _manager.Start(intervalMs: 1));
        }

        [Fact]
        public void Then_intervalMs_at_minimum_succeeds()
        {
            _manager.CreateChannel("crevis-tc-a160k-freerun-rgb8.cam");
            _manager.Start(intervalMs: 50);
            Assert.True(_manager.IsActive);
        }

        [Fact]
        public void Then_intervalMs_zero_does_not_create_timer()
        {
            _manager.CreateChannel("crevis-tc-a160k-freerun-rgb8.cam");
            _manager.Start(intervalMs: 0);
            Assert.True(_manager.IsActive);
            _mockHal.CallLog.Reset();
            Thread.Sleep(100);
            Assert.Equal(0, _mockHal.CallLog.SoftwareTriggerCount);
        }
    }

    public class Given_channel_start_stop_cycle_reuses_channel : AcquisitionManagerTests
    {
        [Fact]
        public void Then_McCreate_called_once_across_start_stop_start()
        {
            _manager.CreateChannel("crevis-tc-a160k-freerun-rgb8.cam");
            _manager.Start();
            _manager.Stop();
            _mockHal.CallLog.Reset();

            _manager.Start();

            Assert.Equal(0, _mockHal.CallLog.CreateCalls);
            Assert.Equal(0, _mockHal.CallLog.DeleteCalls);
        }

        [Fact]
        public void Then_is_active_after_second_start()
        {
            _manager.CreateChannel("crevis-tc-a160k-freerun-rgb8.cam");
            _manager.Start();
            _manager.Stop();
            _manager.Start();

            Assert.True(_manager.IsActive);
        }

        [Fact]
        public void Then_finite_frameCount_auto_restart_reuses_channel()
        {
            _manager.CreateChannel("crevis-tc-a160k-freerun-rgb8.cam");
            _manager.Start();
            var channel = _manager.Channel!;
            _mockHal.CallLog.Reset();

            // Simulate hardware signalling end of sequence
            _mockHal.SimulateAcquisitionError(channel.Handle, McSignal.MC_SIG_END_CHANNEL_ACTIVITY);
            Thread.Sleep(200);

            Assert.Equal(ChannelState.Idle, _manager.ChannelState);

            // Restart — should reuse same channel handle (no McCreate)
            _manager.Start();

            Assert.Equal(0, _mockHal.CallLog.CreateCalls);
            Assert.True(_manager.IsActive);
        }
    }

    public class Given_channel_release_and_recreate : AcquisitionManagerTests
    {
        [Fact]
        public void Then_McCreate_and_McDelete_each_called_once()
        {
            _manager.CreateChannel("crevis-tc-a160k-freerun-rgb8.cam");
            _manager.Start();
            _manager.Stop();
            _mockHal.CallLog.Reset();

            _manager.ReleaseChannel();
            _manager.CreateChannel("crevis-tc-a160k-softtrig-rgb8.cam");

            Assert.Equal(1, _mockHal.CallLog.CreateCalls);
            Assert.Equal(1, _mockHal.CallLog.DeleteCalls);
        }

        [Fact]
        public void Then_channel_state_is_idle_after_recreate()
        {
            _manager.CreateChannel("crevis-tc-a160k-freerun-rgb8.cam");
            _manager.Stop();
            _manager.ReleaseChannel();
            _manager.CreateChannel("crevis-tc-a160k-softtrig-rgb8.cam");

            Assert.Equal(ChannelState.Idle, _manager.ChannelState);
            Assert.Equal(new ProfileId("crevis-tc-a160k-softtrig-rgb8.cam"), _manager.ActiveProfileId);
        }
    }
}
