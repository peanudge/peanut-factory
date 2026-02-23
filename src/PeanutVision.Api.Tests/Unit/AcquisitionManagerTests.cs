using System.Runtime.InteropServices;
using PeanutVision.Api.Services;
using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Camera;
using PeanutVision.MultiCamDriver.Hal;
using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Api.Tests.Unit;

public class AcquisitionManagerTests : IDisposable
{
    private static readonly object _initLock = new();
    private static bool _initialized;

    protected readonly MockMultiCamHAL _mockHal;
    protected readonly GrabService _grabService;
    protected readonly AcquisitionManager _manager;
    private readonly IntPtr _surfaceMemory;

    public AcquisitionManagerTests()
    {
        lock (_initLock)
        {
            if (!_initialized)
            {
                var camDir = CamFileResource.GetDirectory();
                foreach (var name in new[]
                {
                    CamFileResource.KnownCamFiles.TC_A160K_FreeRun_RGB8,
                    CamFileResource.KnownCamFiles.TC_A160K_FreeRun_1TAP_RGB8,
                })
                {
                    var path = Path.Combine(camDir, name);
                    if (!File.Exists(path)) File.WriteAllText(path, "");
                }

                foreach (var profile in CrevisProfiles.All)
                    CameraRegistry.Default.Register(profile);

                _initialized = true;
            }
        }

        _mockHal = new MockMultiCamHAL();

        var bufferSize = _mockHal.Configuration.DefaultImageWidth
            * _mockHal.Configuration.DefaultImageHeight * 3;
        _surfaceMemory = Marshal.AllocHGlobal(bufferSize);
        var zeros = new byte[bufferSize];
        Marshal.Copy(zeros, 0, _surfaceMemory, bufferSize);
        _mockHal.Configuration.SimulatedSurfaceAddress = _surfaceMemory;

        _grabService = new GrabService(_mockHal);
        _grabService.Initialize();
        _manager = new AcquisitionManager(_grabService);
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
        public void Then_capture_frame_returns_null()
        {
            Assert.Null(_manager.CaptureFrame());
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
        public void When_send_trigger_then_throws()
        {
            Assert.Throws<InvalidOperationException>(() => _manager.SendTrigger());
        }

        [Fact]
        public void When_start_with_unknown_profile_then_throws()
        {
            Assert.Throws<KeyNotFoundException>(() => _manager.Start("nonexistent"));
        }
    }

    public class Given_started : AcquisitionManagerTests
    {
        public Given_started()
        {
            _manager.Start("crevis-tc-a160k-freerun-rgb8");
        }

        [Fact]
        public void Then_is_active()
        {
            Assert.True(_manager.IsActive);
        }

        [Fact]
        public void Then_active_profile_id_matches()
        {
            Assert.Equal(new ProfileId("crevis-tc-a160k-freerun-rgb8"), _manager.ActiveProfileId);
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
        public void Then_capture_frame_returns_null_before_callback()
        {
            Assert.Null(_manager.CaptureFrame());
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
            Assert.Throws<InvalidOperationException>(() =>
                _manager.Start("crevis-tc-a160k-freerun-rgb8"));
        }

        [Fact]
        public void When_snapshot_then_throws()
        {
            Assert.Throws<InvalidOperationException>(() =>
                _manager.Snapshot("crevis-tc-a160k-freerun-rgb8"));
        }

        [Fact]
        public void When_dispose_then_stops_acquisition()
        {
            _manager.Dispose();
            Assert.False(_manager.IsActive);
        }
    }

    public class Given_started_with_trigger_mode : AcquisitionManagerTests
    {
        public Given_started_with_trigger_mode()
        {
            _manager.Start("crevis-tc-a160k-freerun-rgb8", TriggerMode.Soft);
        }

        [Fact]
        public void Then_is_active()
        {
            Assert.True(_manager.IsActive);
        }
    }

    public class Given_started_then_stopped : AcquisitionManagerTests
    {
        public Given_started_then_stopped()
        {
            _manager.Start("crevis-tc-a160k-freerun-rgb8");
            _manager.Stop();
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
        public void When_restart_with_different_profile_then_activates()
        {
            _manager.Start("crevis-tc-a160k-softtrig-rgb8");

            Assert.True(_manager.IsActive);
            Assert.Equal(new ProfileId("crevis-tc-a160k-softtrig-rgb8"), _manager.ActiveProfileId);
        }
    }

    public class Given_started_and_trigger_sent : AcquisitionManagerTests
    {
        public Given_started_and_trigger_sent()
        {
            _manager.Start("crevis-tc-a160k-freerun-rgb8");
            _mockHal.CallLog.Reset();

            _manager.SendTrigger();
            _manager.SendTrigger();
        }

        [Fact]
        public void Then_hal_trigger_count_increments()
        {
            Assert.Equal(2, _mockHal.CallLog.SoftwareTriggerCount);
        }
    }

    public class Given_started_and_frame_acquired : AcquisitionManagerTests
    {
        [Fact]
        public void Then_capture_frame_returns_image_with_correct_dimensions()
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

                _manager.Start("crevis-tc-a160k-freerun-rgb8");
                _mockHal.SimulateFrameAcquisition(_manager.Channel!.Handle);

                var frame = _manager.CaptureFrame();
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
            var image = _manager.Snapshot("crevis-tc-a160k-freerun-rgb8");

            Assert.NotNull(image);
            Assert.Equal(_mockHal.Configuration.DefaultImageWidth, image.Width);
            Assert.Equal(_mockHal.Configuration.DefaultImageHeight, image.Height);
        }

        [Fact]
        public void Then_sends_software_trigger()
        {
            _mockHal.CallLog.Reset();

            _manager.Snapshot("crevis-tc-a160k-freerun-rgb8");

            Assert.Equal(1, _mockHal.CallLog.SoftwareTriggerCount);
        }

        [Fact]
        public void Then_starts_and_stops_acquisition()
        {
            _mockHal.CallLog.Reset();

            _manager.Snapshot("crevis-tc-a160k-freerun-rgb8");

            Assert.True(_mockHal.CallLog.AcquisitionStarted);
            Assert.True(_mockHal.CallLog.AcquisitionStopped);
        }

        [Fact]
        public void Then_does_not_leave_channel_active()
        {
            _manager.Snapshot("crevis-tc-a160k-freerun-rgb8");

            Assert.False(_manager.IsActive);
            Assert.Null(_manager.Channel);
        }

        [Fact]
        public void Then_uses_polling_mode_not_callback()
        {
            _mockHal.CallLog.Reset();

            _manager.Snapshot("crevis-tc-a160k-freerun-rgb8");

            Assert.Equal(1, _mockHal.CallLog.WaitSignalCalls);
            Assert.Equal(0, _mockHal.CallLog.RegisterCallbackCalls);
        }

        [Fact]
        public void Then_can_be_called_multiple_times()
        {
            var image1 = _manager.Snapshot("crevis-tc-a160k-freerun-rgb8");
            var image2 = _manager.Snapshot("crevis-tc-a160k-freerun-rgb8");

            Assert.NotNull(image1);
            Assert.NotNull(image2);
        }

        [Fact]
        public void When_unknown_profile_then_throws()
        {
            Assert.Throws<KeyNotFoundException>(() => _manager.Snapshot("nonexistent"));
        }
    }

    public class Snapshot_given_active : AcquisitionManagerTests
    {
        public Snapshot_given_active()
        {
            _manager.Start("crevis-tc-a160k-freerun-rgb8");
        }

        [Fact]
        public void Then_throws()
        {
            Assert.Throws<InvalidOperationException>(() =>
                _manager.Snapshot("crevis-tc-a160k-freerun-rgb8"));
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
                _manager.Snapshot("crevis-tc-a160k-freerun-rgb8"));
        }
    }
}
