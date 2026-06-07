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
        _manager = new AcquisitionManager(_grabService, TestCamFileHelper.GetOrCreate(), new NullLatencyService());
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
            Assert.Equal(ChannelState.None, _manager.GetStatus().ChannelState);
        }

        [Fact]
        public void Then_is_not_active()
        {
            Assert.False(_manager.GetStatus().IsActive);
        }

        [Fact]
        public void Then_active_profile_id_is_null()
        {
            Assert.Null(_manager.GetStatus().ActiveConfig?.ProfileId);
        }

        [Fact]
        public void Then_statistics_are_null()
        {
            Assert.Null(_manager.GetStatus().Statistics);
        }

        [Fact]
        public void Then_last_error_is_null()
        {
            Assert.Null(_manager.GetStatus().LastError);
        }

        [Fact]
        public void When_stop_then_does_nothing()
        {
            _manager.Stop();
            Assert.False(_manager.GetStatus().IsActive);
        }

        [Fact]
        public async Task When_trigger_and_wait_then_throws()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _manager.TriggerAsync());
        }

        [Fact]
        public void When_start_with_unknown_profile_then_throws()
        {
            Assert.Throws<KeyNotFoundException>(
                () => _manager.Start(new AcquisitionConfig(new ProfileId("nonexistent"))));
        }

        [Fact]
        public void Then_allowed_actions_contains_start()
        {
            var actions = _manager.GetStatus().AllowedActions;
            Assert.Contains(ChannelAction.Start, actions);
        }
    }

    public class Given_started : AcquisitionManagerTests
    {
        public Given_started()
        {
            _manager.Start(new AcquisitionConfig(new ProfileId("crevis-tc-a160k-freerun-rgb8.cam")));
        }

        [Fact]
        public void Then_channel_state_is_active()
        {
            Assert.Equal(ChannelState.Active, _manager.GetStatus().ChannelState);
        }

        [Fact]
        public void Then_is_active()
        {
            Assert.True(_manager.GetStatus().IsActive);
        }

        [Fact]
        public void Then_active_profile_id_matches()
        {
            Assert.Equal(new ProfileId("crevis-tc-a160k-freerun-rgb8.cam"), _manager.GetStatus().ActiveConfig?.ProfileId);
        }

        [Fact]
        public void Then_hal_acquisition_is_started()
        {
            Assert.True(_mockHal.CallLog.AcquisitionStarted);
        }

        [Fact]
        public void Then_statistics_return_zero_frames()
        {
            var stats = _manager.GetStatus().Statistics;
            Assert.NotNull(stats);
            Assert.Equal(0, stats.Value.FrameCount);
        }

        [Fact]
        public void Then_last_error_is_null()
        {
            Assert.Null(_manager.GetStatus().LastError);
        }

        [Fact]
        public void When_start_again_then_throws()
        {
            Assert.Throws<InvalidOperationException>(
                () => _manager.Start(new AcquisitionConfig(new ProfileId("crevis-tc-a160k-freerun-rgb8.cam"))));
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
            Assert.False(_manager.GetStatus().IsActive);
        }

        [Fact]
        public void Then_allowed_actions_contains_stop_and_trigger()
        {
            var actions = _manager.GetStatus().AllowedActions;
            Assert.Contains(ChannelAction.Stop, actions);
            Assert.Contains(ChannelAction.Trigger, actions);
            Assert.DoesNotContain(ChannelAction.Start, actions);
        }
    }

    public class Given_started_then_stopped : AcquisitionManagerTests
    {
        public Given_started_then_stopped()
        {
            _manager.Start(new AcquisitionConfig(new ProfileId("crevis-tc-a160k-freerun-rgb8.cam")));
            _manager.Stop();
        }

        [Fact]
        public void Then_channel_state_is_idle()
        {
            Assert.Equal(ChannelState.Idle, _manager.GetStatus().ChannelState);
        }

        [Fact]
        public void Then_is_not_active()
        {
            Assert.False(_manager.GetStatus().IsActive);
        }

        [Fact]
        public void Then_active_config_is_null_after_stop()
        {
            // ActiveConfig is only set when state == Active
            Assert.Null(_manager.GetStatus().ActiveConfig);
        }

        [Fact]
        public void When_start_again_then_activates()
        {
            _manager.Start(new AcquisitionConfig(new ProfileId("crevis-tc-a160k-freerun-rgb8.cam")));

            Assert.True(_manager.GetStatus().IsActive);
            Assert.Equal(ChannelState.Active, _manager.GetStatus().ChannelState);
        }

        [Fact]
        public void When_release_then_channel_state_is_none()
        {
            _manager.ReleaseChannel();

            Assert.Equal(ChannelState.None, _manager.GetStatus().ChannelState);
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

            _manager.Start(new AcquisitionConfig(new ProfileId("crevis-tc-a160k-softtrig-rgb8.cam")));
            var image = await _manager.TriggerAsync();

            Assert.Equal(width, image.Width);
            Assert.Equal(height, image.Height);
        }

        [Fact]
        public async Task Then_increments_hal_trigger_count()
        {
            _manager.Start(new AcquisitionConfig(new ProfileId("crevis-tc-a160k-softtrig-rgb8.cam")));
            _mockHal.CallLog.Reset();

            await _manager.TriggerAsync();
            await _manager.TriggerAsync();

            Assert.Equal(2, _mockHal.CallLog.SoftwareTriggerCount);
        }
    }

    public class Given_started_and_trigger_timeout : AcquisitionManagerTests
    {
        [Fact]
        public async Task Then_throws_TimeoutException()
        {
            // AutoSimulateFrameOnTrigger is false by default, so no frame arrives
            _manager.Start(new AcquisitionConfig(new ProfileId("crevis-tc-a160k-softtrig-rgb8.cam")));

            await Assert.ThrowsAsync<TimeoutException>(
                () => _manager.TriggerAsync(timeoutMs: 100));
        }
    }

    public class Given_statistics_include_channel_diagnostics : AcquisitionManagerTests
    {
        [Fact]
        public void Then_statistics_contain_copy_drop_and_cluster_counts()
        {
            _manager.Start(new AcquisitionConfig(new ProfileId("crevis-tc-a160k-freerun-rgb8.cam")));

            var stats = _manager.GetStatus().Statistics;
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
            _manager.Start(new AcquisitionConfig(new ProfileId("crevis-tc-a160k-freerun-rgb8.cam")));

            var events = _manager.GetStatus().RecentEvents;
            Assert.Single(events);
            Assert.Equal(ChannelEventType.AcquisitionStarted, events[0].Type);
        }

        [Fact]
        public void Then_stop_records_event()
        {
            _manager.Start(new AcquisitionConfig(new ProfileId("crevis-tc-a160k-freerun-rgb8.cam")));
            _manager.Stop();

            var events = _manager.GetStatus().RecentEvents;
            Assert.Equal(2, events.Count);
            Assert.Equal(ChannelEventType.AcquisitionStopped, events[0].Type);
            Assert.Equal(ChannelEventType.AcquisitionStarted, events[1].Type);
        }

        [Fact]
        public void Then_events_persist_across_sessions()
        {
            _manager.Start(new AcquisitionConfig(new ProfileId("crevis-tc-a160k-freerun-rgb8.cam")));
            _manager.Stop();
            _manager.Start(new AcquisitionConfig(new ProfileId("crevis-tc-a160k-freerun-rgb8.cam")));

            var events = _manager.GetStatus().RecentEvents;
            Assert.Equal(3, events.Count);
            Assert.Equal(ChannelEventType.AcquisitionStarted, events[0].Type);
            Assert.Equal(ChannelEventType.AcquisitionStopped, events[1].Type);
        }
    }

    // intervalMs validation moved to AcquisitionConfigValidator (controller layer)

    public class Given_channel_start_stop_cycle : AcquisitionManagerTests
    {
        [Fact]
        public void Then_Start_after_Stop_releases_old_and_creates_new_channel()
        {
            // Start(config) always creates a fresh channel; when Idle it releases the old one first
            _manager.Start(new AcquisitionConfig(new ProfileId("crevis-tc-a160k-freerun-rgb8.cam")));
            _manager.Stop();
            _mockHal.CallLog.Reset();

            _manager.Start(new AcquisitionConfig(new ProfileId("crevis-tc-a160k-freerun-rgb8.cam")));

            Assert.Equal(1, _mockHal.CallLog.CreateCalls);
            Assert.Equal(1, _mockHal.CallLog.DeleteCalls);
        }

        [Fact]
        public void Then_is_active_after_second_start()
        {
            _manager.Start(new AcquisitionConfig(new ProfileId("crevis-tc-a160k-freerun-rgb8.cam")));
            _manager.Stop();
            _manager.Start(new AcquisitionConfig(new ProfileId("crevis-tc-a160k-freerun-rgb8.cam")));

            Assert.True(_manager.GetStatus().IsActive);
        }
    }

    public class Given_channel_release_and_recreate : AcquisitionManagerTests
    {
        [Fact]
        public void Then_McCreate_and_McDelete_each_called_once()
        {
            _manager.Start(new AcquisitionConfig(new ProfileId("crevis-tc-a160k-freerun-rgb8.cam")));
            _manager.Stop();
            _manager.ReleaseChannel();
            _mockHal.CallLog.Reset();

            _manager.Start(new AcquisitionConfig(new ProfileId("crevis-tc-a160k-softtrig-rgb8.cam")));

            Assert.Equal(1, _mockHal.CallLog.CreateCalls);
            Assert.Equal(0, _mockHal.CallLog.DeleteCalls);
        }

        [Fact]
        public void Then_channel_state_is_active_after_recreate()
        {
            _manager.Start(new AcquisitionConfig(new ProfileId("crevis-tc-a160k-freerun-rgb8.cam")));
            _manager.Stop();
            _manager.ReleaseChannel();
            _manager.Start(new AcquisitionConfig(new ProfileId("crevis-tc-a160k-softtrig-rgb8.cam")));

            Assert.Equal(ChannelState.Active, _manager.GetStatus().ChannelState);
            Assert.Equal(new ProfileId("crevis-tc-a160k-softtrig-rgb8.cam"), _manager.GetStatus().ActiveConfig?.ProfileId);
        }
    }

    public class Given_active_frameCount_and_intervalMs : AcquisitionManagerTests
    {
        [Fact]
        public void Then_ActiveFrameCount_reflects_configured_value()
        {
            _manager.Start(new AcquisitionConfig(new ProfileId("crevis-tc-a160k-freerun-rgb8.cam"), FrameCount: 10));

            Assert.Equal(10, _manager.GetStatus().ActiveConfig?.FrameCount);
        }

        [Fact]
        public void Then_ActiveFrameCount_is_null_when_not_specified()
        {
            _manager.Start(new AcquisitionConfig(new ProfileId("crevis-tc-a160k-freerun-rgb8.cam")));

            Assert.Null(_manager.GetStatus().ActiveConfig?.FrameCount);
        }

        [Fact]
        public void Then_ActiveIntervalMs_reflects_configured_value()
        {
            _manager.Start(new AcquisitionConfig(new ProfileId("crevis-tc-a160k-freerun-rgb8.cam"), IntervalMs: 200));

            Assert.Equal(200, _manager.GetStatus().ActiveConfig?.IntervalMs);
        }

        [Fact]
        public void Then_ActiveIntervalMs_is_null_when_not_specified()
        {
            _manager.Start(new AcquisitionConfig(new ProfileId("crevis-tc-a160k-freerun-rgb8.cam")));

            Assert.Null(_manager.GetStatus().ActiveConfig?.IntervalMs);
        }

        [Fact]
        public void Then_ActiveFrameCount_is_null_after_stop()
        {
            _manager.Start(new AcquisitionConfig(new ProfileId("crevis-tc-a160k-freerun-rgb8.cam"), FrameCount: 5));
            _manager.Stop();

            // ActiveConfig is null when not active
            Assert.Null(_manager.GetStatus().ActiveConfig?.FrameCount);
        }

        [Fact]
        public void Then_ActiveIntervalMs_is_null_after_stop()
        {
            _manager.Start(new AcquisitionConfig(new ProfileId("crevis-tc-a160k-freerun-rgb8.cam"), IntervalMs: 100));
            _manager.Stop();

            // ActiveConfig is null when not active
            Assert.Null(_manager.GetStatus().ActiveConfig?.IntervalMs);
        }

        [Fact]
        public void Then_ActiveFrameCount_is_null_when_not_started()
        {
            Assert.Null(_manager.GetStatus().ActiveConfig?.FrameCount);
        }

        [Fact]
        public void Then_ActiveIntervalMs_is_null_when_not_started()
        {
            Assert.Null(_manager.GetStatus().ActiveConfig?.IntervalMs);
        }
    }

    public class Given_AcquisitionConfig : AcquisitionManagerTests
    {
        [Fact]
        public void Config_holds_all_fields()
        {
            var config = new AcquisitionConfig(
                new ProfileId("cam.cam"),
                FrameCount: 5,
                IntervalMs: 100);

            Assert.Equal("cam.cam", config.ProfileId.Value);
            Assert.Equal(5, config.FrameCount);
            Assert.Equal(100, config.IntervalMs);
        }

        [Fact]
        public void Config_defaults_optional_fields_to_null()
        {
            var config = new AcquisitionConfig(new ProfileId("cam.cam"));

            Assert.Null(config.FrameCount);
            Assert.Null(config.IntervalMs);
        }
    }

    public class Given_GetStatus : AcquisitionManagerTests
    {
        [Fact]
        public void When_idle_returns_None_state_and_no_config()
        {
            var status = _manager.GetStatus();

            Assert.Equal(ChannelState.None, status.ChannelState);
            Assert.False(status.IsActive);
            Assert.Null(status.ActiveConfig);
            Assert.False(status.HasFrame);
            Assert.Null(status.LastError);
            Assert.Null(status.Statistics);
        }

        [Fact]
        public void When_active_returns_Active_state_with_config()
        {
            var config = new AcquisitionConfig(
                new ProfileId("crevis-tc-a160k-freerun-rgb8.cam"),
                FrameCount: 3,
                IntervalMs: 100);
            _manager.Start(config);

            var status = _manager.GetStatus();

            Assert.Equal(ChannelState.Active, status.ChannelState);
            Assert.True(status.IsActive);
            Assert.NotNull(status.ActiveConfig);
            Assert.Equal("crevis-tc-a160k-freerun-rgb8.cam", status.ActiveConfig!.ProfileId.Value);
            Assert.Equal(3, status.ActiveConfig.FrameCount);
            Assert.Equal(100, status.ActiveConfig.IntervalMs);
        }

        [Fact]
        public void When_active_AllowedActions_contains_Stop_and_Trigger()
        {
            _manager.Start(new AcquisitionConfig(new ProfileId("crevis-tc-a160k-freerun-rgb8.cam")));

            var status = _manager.GetStatus();

            Assert.Contains(ChannelAction.Stop, status.AllowedActions);
            Assert.Contains(ChannelAction.Trigger, status.AllowedActions);
            Assert.DoesNotContain(ChannelAction.Start, status.AllowedActions);
        }

        [Fact]
        public void When_idle_AllowedActions_contains_Start()
        {
            var status = _manager.GetStatus();

            Assert.Contains(ChannelAction.Start, status.AllowedActions);
        }

        [Fact]
        public void After_stop_returns_Idle_state_and_null_config()
        {
            _manager.Start(new AcquisitionConfig(new ProfileId("crevis-tc-a160k-freerun-rgb8.cam")));
            _manager.Stop();

            var status = _manager.GetStatus();

            Assert.Equal(ChannelState.Idle, status.ChannelState);
            Assert.False(status.IsActive);
            Assert.Null(status.ActiveConfig);
        }
    }

    public class Given_Start_with_config : AcquisitionManagerTests
    {
        [Fact]
        public void Then_channel_is_active()
        {
            var config = new AcquisitionConfig(new ProfileId("crevis-tc-a160k-freerun-rgb8.cam"));
            _manager.Start(config);

            Assert.True(_manager.GetStatus().IsActive);
            Assert.Equal(ChannelState.Active, _manager.GetStatus().ChannelState);
        }

        [Fact]
        public void Then_active_profile_id_matches_config()
        {
            var config = new AcquisitionConfig(new ProfileId("crevis-tc-a160k-freerun-rgb8.cam"));
            _manager.Start(config);

            Assert.Equal("crevis-tc-a160k-freerun-rgb8.cam", _manager.GetStatus().ActiveConfig?.ProfileId.Value);
        }

        [Fact]
        public void Then_auto_releases_idle_channel_before_starting()
        {
            // Put manager into Idle state first by starting and stopping
            _manager.Start(new AcquisitionConfig(new ProfileId("crevis-tc-a160k-freerun-rgb8.cam")));
            _manager.Stop();
            _mockHal.CallLog.Reset();

            var config = new AcquisitionConfig(new ProfileId("crevis-tc-a160k-softtrig-rgb8.cam"));
            _manager.Start(config);

            // Old channel released (1 Delete), new one created (1 Create)
            Assert.Equal(1, _mockHal.CallLog.DeleteCalls);
            Assert.Equal(1, _mockHal.CallLog.CreateCalls);
            Assert.Equal("crevis-tc-a160k-softtrig-rgb8.cam", _manager.GetStatus().ActiveConfig?.ProfileId.Value);
        }

        [Fact]
        public void When_already_active_then_throws()
        {
            var config = new AcquisitionConfig(new ProfileId("crevis-tc-a160k-freerun-rgb8.cam"));
            _manager.Start(config);

            Assert.Throws<InvalidOperationException>(() =>
                _manager.Start(new AcquisitionConfig(new ProfileId("crevis-tc-a160k-freerun-rgb8.cam"))));
        }

        [Fact]
        public void Then_frameCount_stored_in_ActiveConfig()
        {
            var config = new AcquisitionConfig(new ProfileId("crevis-tc-a160k-freerun-rgb8.cam"), FrameCount: 7);
            _manager.Start(config);

            Assert.Equal(7, _manager.GetStatus().ActiveConfig?.FrameCount);
        }

        [Fact]
        public void Then_intervalMs_stored_in_ActiveConfig()
        {
            var config = new AcquisitionConfig(new ProfileId("crevis-tc-a160k-freerun-rgb8.cam"), IntervalMs: 200);
            _manager.Start(config);

            Assert.Equal(200, _manager.GetStatus().ActiveConfig?.IntervalMs);
        }
    }
}
