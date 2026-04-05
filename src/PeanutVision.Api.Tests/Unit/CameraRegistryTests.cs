using PeanutVision.Api.Services;
using PeanutVision.Api.Services.Camera;
using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Api.Tests.Unit;

public class CameraRegistryTests
{
    [Fact]
    public void Register_then_TryGet_returns_same_actor()
    {
        var registry = new CameraRegistry();
        var actor = new FakeCameraActor("cam-1");

        registry.Register(actor);
        var found = registry.TryGet("cam-1");

        Assert.Same(actor, found);
    }

    [Fact]
    public void Register_duplicate_id_throws_InvalidOperationException()
    {
        var registry = new CameraRegistry();
        registry.Register(new FakeCameraActor("cam-1"));

        var ex = Assert.Throws<InvalidOperationException>(
            () => registry.Register(new FakeCameraActor("cam-1")));

        Assert.Contains("cam-1", ex.Message);
    }

    [Fact]
    public void Register_is_case_insensitive_for_duplicate_detection()
    {
        var registry = new CameraRegistry();
        registry.Register(new FakeCameraActor("cam-1"));

        Assert.Throws<InvalidOperationException>(
            () => registry.Register(new FakeCameraActor("CAM-1")));
    }

    [Fact]
    public void TryGet_returns_null_for_unknown_id()
    {
        var registry = new CameraRegistry();

        var result = registry.TryGet("nonexistent");

        Assert.Null(result);
    }

    [Fact]
    public void TryGet_is_case_insensitive()
    {
        var registry = new CameraRegistry();
        var actor = new FakeCameraActor("cam-1");
        registry.Register(actor);

        Assert.Same(actor, registry.TryGet("CAM-1"));
        Assert.Same(actor, registry.TryGet("Cam-1"));
    }

    [Fact]
    public void GetAllIds_returns_empty_for_new_registry()
    {
        var registry = new CameraRegistry();
        Assert.Empty(registry.GetAllIds());
    }

    [Fact]
    public void GetAllIds_returns_all_registered_ids()
    {
        var registry = new CameraRegistry();
        registry.Register(new FakeCameraActor("cam-1"));
        registry.Register(new FakeCameraActor("cam-2"));
        registry.Register(new FakeCameraActor("cam-3"));

        var ids = registry.GetAllIds();

        Assert.Equal(3, ids.Count);
        Assert.Contains("cam-1", ids);
        Assert.Contains("cam-2", ids);
        Assert.Contains("cam-3", ids);
    }

    [Fact]
    public void GetAllIds_returns_snapshot_not_live_collection()
    {
        var registry = new CameraRegistry();
        registry.Register(new FakeCameraActor("cam-1"));

        var ids = registry.GetAllIds();
        registry.Register(new FakeCameraActor("cam-2"));

        // The snapshot taken before cam-2 was added should still have only 1
        Assert.Single(ids);
    }

    // Minimal fake actor for registry tests -- no behavior needed
    private sealed class FakeCameraActor : ICameraActor
    {
        public string CameraId { get; }
        public FakeCameraActor(string id) => CameraId = id;
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
        public Task StartAsync(ProfileId profileId, TriggerMode? triggerMode = null, int? frameCount = null, int? intervalMs = null, CancellationToken ct = default) => Task.CompletedTask;
        public Task StopAsync(CancellationToken ct = default) => Task.CompletedTask;
        public Task<ImageData> TriggerAsync(int timeoutMs = 5000, CancellationToken ct = default) => throw new NotImplementedException();
        public Task<LatestFrameResult> GetLatestFrameAsync(CancellationToken ct = default) => throw new NotImplementedException();
        public Task<CameraActorStatus> GetStatusAsync(CancellationToken ct = default) => throw new NotImplementedException();
        public Task<ExposureInfo> GetExposureAsync(CancellationToken ct = default) => throw new NotImplementedException();
        public Task<ExposureInfo> SetExposureAsync(double? exposureUs, CancellationToken ct = default) => throw new NotImplementedException();
    }
}
