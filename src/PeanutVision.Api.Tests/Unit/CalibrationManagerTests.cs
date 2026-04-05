using System.Runtime.InteropServices;
using PeanutVision.Api.Services;
using PeanutVision.Api.Tests.Infrastructure;
using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Hal;

namespace PeanutVision.Api.Tests.Unit;

public class CalibrationManagerTests : IDisposable
{
    protected readonly MockMultiCamHAL _mockHal;
    protected readonly AcquisitionChannelManager _channelManager;
    protected readonly AcquisitionService _acquisitionManager;
    protected readonly CalibrationManager _calibrationManager;
    private readonly IntPtr _surfaceMemory;

    public CalibrationManagerTests()
    {
        _mockHal = new MockMultiCamHAL();

        var bufferSize = _mockHal.Configuration.DefaultImageWidth
            * _mockHal.Configuration.DefaultImageHeight * 3;
        _surfaceMemory = Marshal.AllocHGlobal(bufferSize);
        var zeros = new byte[bufferSize];
        Marshal.Copy(zeros, 0, _surfaceMemory, bufferSize);
        _mockHal.Configuration.SimulatedSurfaceAddress = _surfaceMemory;

        _channelManager = new AcquisitionChannelManager(_mockHal);
        _channelManager.Initialize();
        _acquisitionManager = new AcquisitionService(_channelManager, TestCamFileHelper.GetOrCreate(), new NullLatencyService(), new AcquisitionOperationGate());
        _calibrationManager = new CalibrationManager(_acquisitionManager, _acquisitionManager);
    }

    public void Dispose()
    {
        _acquisitionManager.Dispose();
        _channelManager.Dispose();
        if (_surfaceMemory != IntPtr.Zero)
            Marshal.FreeHGlobal(_surfaceMemory);
    }

    public class Given_no_acquisition : CalibrationManagerTests
    {
        [Fact]
        public void Then_is_not_available()
        {
            Assert.False(_calibrationManager.IsAvailable);
        }

        [Fact]
        public void When_black_calibration_then_throws()
        {
            Assert.Throws<InvalidOperationException>(() => _calibrationManager.PerformBlackCalibration());
        }

        [Fact]
        public void When_white_calibration_then_throws()
        {
            Assert.Throws<InvalidOperationException>(() => _calibrationManager.PerformWhiteCalibration());
        }

        [Fact]
        public void When_white_balance_then_throws()
        {
            Assert.Throws<InvalidOperationException>(() => _calibrationManager.PerformWhiteBalanceOnce());
        }

        [Fact]
        public void When_set_flat_field_correction_then_throws()
        {
            Assert.Throws<InvalidOperationException>(() => _calibrationManager.SetFlatFieldCorrection(true));
        }

        [Fact]
        public void When_get_exposure_returns_desired_default()
        {
            var info = _calibrationManager.GetExposure();

            Assert.True(info.ExposureUs > 0);
            Assert.Null(info.ExposureRange); // no range without active channel
        }

        [Fact]
        public void When_set_exposure_stores_desired_value()
        {
            var info = _calibrationManager.SetExposure(5000);

            Assert.Equal(5000.0, info.ExposureUs);
            Assert.Null(info.ExposureRange); // no range without active channel
        }
    }

    public class Given_active_acquisition : CalibrationManagerTests
    {
        public Given_active_acquisition()
        {
            _acquisitionManager.CreateChannel("crevis-tc-a160k-freerun-rgb8.cam");
            _acquisitionManager.Start();
        }

        [Fact]
        public void Then_is_available()
        {
            Assert.True(_calibrationManager.IsAvailable);
        }

        [Fact]
        public void When_black_calibration_then_calls_hal()
        {
            _mockHal.CallLog.Reset();

            _calibrationManager.PerformBlackCalibration();

            Assert.True(_mockHal.CallLog.BlackCalibrationPerformed);
        }

        [Fact]
        public void When_white_calibration_then_calls_hal()
        {
            _mockHal.CallLog.Reset();

            _calibrationManager.PerformWhiteCalibration();

            Assert.True(_mockHal.CallLog.WhiteCalibrationPerformed);
        }

        [Fact]
        public void When_white_balance_then_calls_hal()
        {
            _mockHal.CallLog.Reset();

            _calibrationManager.PerformWhiteBalanceOnce();

            Assert.True(_mockHal.CallLog.WhiteBalancePerformed);
        }

        [Fact]
        public void When_set_flat_field_correction_then_calls_hal()
        {
            _mockHal.CallLog.Reset();

            _calibrationManager.SetFlatFieldCorrection(true);

            Assert.True(_mockHal.CallLog.LastSetParams.ContainsKey(MultiCamApi.PN_FlatFieldCorrection));
        }

        [Fact]
        public void When_get_exposure_then_returns_correct_values()
        {
            var info = _calibrationManager.GetExposure();

            Assert.Equal(10000.0, info.ExposureUs);
            Assert.NotNull(info.ExposureRange);
            Assert.Equal(10.0, info.ExposureRange.Min);
            Assert.Equal(1000000.0, info.ExposureRange.Max);
        }

        [Fact]
        public void When_set_exposure_then_returns_updated_values()
        {
            var info = _calibrationManager.SetExposure(5000.0);

            Assert.Equal(5000.0, info.ExposureUs);
            Assert.NotNull(info.ExposureRange);
        }
    }

    public class Given_idle_channel : CalibrationManagerTests
    {
        public Given_idle_channel()
        {
            _acquisitionManager.CreateChannel("crevis-tc-a160k-freerun-rgb8.cam");
        }

        [Fact]
        public void Then_is_available()
        {
            Assert.True(_calibrationManager.IsAvailable);
        }

        [Fact]
        public void When_black_calibration_then_calls_hal()
        {
            _mockHal.CallLog.Reset();

            _calibrationManager.PerformBlackCalibration();

            Assert.True(_mockHal.CallLog.BlackCalibrationPerformed);
        }

        [Fact]
        public void When_white_calibration_then_calls_hal()
        {
            _mockHal.CallLog.Reset();

            _calibrationManager.PerformWhiteCalibration();

            Assert.True(_mockHal.CallLog.WhiteCalibrationPerformed);
        }

        [Fact]
        public void When_white_balance_then_calls_hal()
        {
            _mockHal.CallLog.Reset();

            _calibrationManager.PerformWhiteBalanceOnce();

            Assert.True(_mockHal.CallLog.WhiteBalancePerformed);
        }

        [Fact]
        public void When_get_exposure_then_returns_values()
        {
            var info = _calibrationManager.GetExposure();

            Assert.Equal(10000.0, info.ExposureUs);
        }

        [Fact]
        public void When_set_exposure_then_returns_updated_values()
        {
            var info = _calibrationManager.SetExposure(5000.0);

            Assert.Equal(5000.0, info.ExposureUs);
        }
    }

    public class Given_active_then_stopped : CalibrationManagerTests
    {
        public Given_active_then_stopped()
        {
            _acquisitionManager.CreateChannel("crevis-tc-a160k-freerun-rgb8.cam");
            _acquisitionManager.Start();
            _acquisitionManager.Stop();
        }

        [Fact]
        public void Then_is_still_available()
        {
            // Channel remains alive in Idle state after Stop — calibration is still available.
            Assert.True(_calibrationManager.IsAvailable);
        }

        [Fact]
        public void When_black_calibration_then_calls_hal()
        {
            _mockHal.CallLog.Reset();

            _calibrationManager.PerformBlackCalibration();

            Assert.True(_mockHal.CallLog.BlackCalibrationPerformed);
        }
    }

    public class Given_channel_released : CalibrationManagerTests
    {
        public Given_channel_released()
        {
            _acquisitionManager.CreateChannel("crevis-tc-a160k-freerun-rgb8.cam");
            _acquisitionManager.ReleaseChannel();
        }

        [Fact]
        public void Then_is_not_available()
        {
            Assert.False(_calibrationManager.IsAvailable);
        }

        [Fact]
        public void When_black_calibration_then_throws()
        {
            Assert.Throws<InvalidOperationException>(() => _calibrationManager.PerformBlackCalibration());
        }
    }
}
