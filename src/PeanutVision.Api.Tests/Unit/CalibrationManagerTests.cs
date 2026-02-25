using System.Runtime.InteropServices;
using PeanutVision.Api.Services;
using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Camera;
using PeanutVision.MultiCamDriver.Hal;

namespace PeanutVision.Api.Tests.Unit;

public class CalibrationManagerTests : IDisposable
{
    private static readonly object _initLock = new();
    private static bool _initialized;

    protected readonly MockMultiCamHAL _mockHal;
    protected readonly GrabService _grabService;
    protected readonly AcquisitionManager _acquisitionManager;
    protected readonly CalibrationManager _calibrationManager;
    private readonly IntPtr _surfaceMemory;

    public CalibrationManagerTests()
    {
        lock (_initLock)
        {
            if (!_initialized)
            {
                var camDir = CamFileResource.GetDirectory();
                foreach (var name in new[]
                {
                    "crevis-tc-a160k-freerun-rgb8.cam",
                    "crevis-tc-a160k-freerun-1tap-rgb8.cam",
                    "crevis-tc-a160k-softtrig-rgb8.cam",
                })
                {
                    var path = Path.Combine(camDir, name);
                    if (!File.Exists(path)) File.WriteAllText(path, "");
                    CameraRegistry.Default.Register(CameraProfile.FromCamFile(name));
                }

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
        _acquisitionManager = new AcquisitionManager(_grabService);
        _calibrationManager = new CalibrationManager(_acquisitionManager);
    }

    public void Dispose()
    {
        _acquisitionManager.Dispose();
        _grabService.Dispose();
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
        public void When_get_exposure_then_throws()
        {
            Assert.Throws<InvalidOperationException>(() => _calibrationManager.GetExposure());
        }

        [Fact]
        public void When_set_exposure_then_throws()
        {
            Assert.Throws<InvalidOperationException>(() => _calibrationManager.SetExposure(5000, null));
        }
    }

    public class Given_active_acquisition : CalibrationManagerTests
    {
        public Given_active_acquisition()
        {
            _acquisitionManager.Start("crevis-tc-a160k-freerun-rgb8");
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
            Assert.Equal(0.0, info.GainDb);
            Assert.NotNull(info.ExposureRange);
            Assert.Equal(10.0, info.ExposureRange.Min);
            Assert.Equal(1000000.0, info.ExposureRange.Max);
        }

        [Fact]
        public void When_set_exposure_and_gain_then_returns_updated_values()
        {
            var info = _calibrationManager.SetExposure(5000.0, 2.5);

            Assert.Equal(5000.0, info.ExposureUs);
            Assert.Equal(2.5, info.GainDb);
            Assert.NotNull(info.ExposureRange);
        }

        [Fact]
        public void When_set_only_exposure_then_gain_unchanged()
        {
            var info = _calibrationManager.SetExposure(7500.0, null);

            Assert.Equal(7500.0, info.ExposureUs);
            Assert.Equal(0.0, info.GainDb);
        }

        [Fact]
        public void When_set_only_gain_then_exposure_unchanged()
        {
            var info = _calibrationManager.SetExposure(null, 3.0);

            Assert.Equal(10000.0, info.ExposureUs);
            Assert.Equal(3.0, info.GainDb);
        }
    }

    public class Given_active_then_stopped : CalibrationManagerTests
    {
        public Given_active_then_stopped()
        {
            _acquisitionManager.Start("crevis-tc-a160k-freerun-rgb8");
            _acquisitionManager.Stop();
        }

        [Fact]
        public void Then_is_not_available()
        {
            Assert.False(_calibrationManager.IsAvailable);
        }
    }
}
