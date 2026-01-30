using PeanutVision.MultiCamDriver.Camera;

namespace PeanutVision.MultiCamDriver.Tests.Camera;

public class CameraRegistryTests
{
    [Fact]
    public void Default_ContainsCrevisProfiles()
    {
        var registry = CameraRegistry.Default;

        Assert.True(registry.HasProfile("crevis-tc-a160k-freerun-rgb8"));
        Assert.True(registry.HasProfile("crevis-tc-a160k-softtrig-rgb8"));
    }

    [Fact]
    public void GetProfile_ReturnsCorrectProfile()
    {
        var registry = CameraRegistry.Default;

        var profile = registry.GetProfile("crevis-tc-a160k-freerun-rgb8");

        Assert.Equal("Crevis", profile.Manufacturer);
        Assert.Equal("TC-A160K", profile.Model);
    }

    [Fact]
    public void GetProfile_CaseInsensitive()
    {
        var registry = CameraRegistry.Default;

        var profile = registry.GetProfile("CREVIS-TC-A160K-FREERUN-RGB8");

        Assert.NotNull(profile);
    }

    [Fact]
    public void GetProfile_NotFound_Throws()
    {
        var registry = CameraRegistry.Default;

        var ex = Assert.Throws<KeyNotFoundException>(() =>
            registry.GetProfile("nonexistent-camera"));

        Assert.Contains("nonexistent-camera", ex.Message);
    }

    [Fact]
    public void TryGetProfile_Found_ReturnsTrue()
    {
        var registry = CameraRegistry.Default;

        var found = registry.TryGetProfile("crevis-tc-a160k-freerun-rgb8", out var profile);

        Assert.True(found);
        Assert.NotNull(profile);
    }

    [Fact]
    public void TryGetProfile_NotFound_ReturnsFalse()
    {
        var registry = CameraRegistry.Default;

        var found = registry.TryGetProfile("nonexistent", out var profile);

        Assert.False(found);
        Assert.Null(profile);
    }

    [Fact]
    public void Register_AddsProfile()
    {
        var registry = new CameraRegistry();
        var profile = new CameraProfile.Builder()
            .WithId("custom-camera")
            .WithCamFile("custom.cam")
            .Build();

        registry.Register(profile);

        Assert.True(registry.HasProfile("custom-camera"));
    }

    [Fact]
    public void Register_OverwritesExisting()
    {
        var registry = new CameraRegistry();
        var profile1 = new CameraProfile.Builder()
            .WithId("camera")
            .WithDisplayName("Version 1")
            .WithCamFile("v1.cam")
            .Build();
        var profile2 = new CameraProfile.Builder()
            .WithId("camera")
            .WithDisplayName("Version 2")
            .WithCamFile("v2.cam")
            .Build();

        registry.Register(profile1);
        registry.Register(profile2);

        var result = registry.GetProfile("camera");
        Assert.Equal("Version 2", result.DisplayName);
    }

    [Fact]
    public void Unregister_RemovesProfile()
    {
        var registry = new CameraRegistry();
        registry.Register(new CameraProfile.Builder()
            .WithId("to-remove")
            .WithCamFile("remove.cam")
            .Build());

        registry.Unregister("to-remove");

        Assert.False(registry.HasProfile("to-remove"));
    }

    [Fact]
    public void GetByManufacturer_ReturnsMatchingProfiles()
    {
        var registry = CameraRegistry.Default;

        var crevisProfiles = registry.GetByManufacturer("Crevis").ToList();

        Assert.NotEmpty(crevisProfiles);
        Assert.All(crevisProfiles, p => Assert.Equal("Crevis", p.Manufacturer));
    }

    [Fact]
    public void GetByModel_ReturnsMatchingProfiles()
    {
        var registry = CameraRegistry.Default;

        var tcA160kProfiles = registry.GetByModel("TC-A160K").ToList();

        Assert.NotEmpty(tcA160kProfiles);
        Assert.All(tcA160kProfiles, p => Assert.Equal("TC-A160K", p.Model));
    }

    [Fact]
    public void ProfileIds_ReturnsAllIds()
    {
        var registry = CameraRegistry.Default;

        var ids = registry.ProfileIds;

        Assert.Contains("crevis-tc-a160k-freerun-rgb8", ids, StringComparer.OrdinalIgnoreCase);
    }

    [Fact]
    public void DefaultProfile_ReturnsFirstProfile()
    {
        var registry = CameraRegistry.Default;

        var defaultProfile = registry.DefaultProfile;

        Assert.NotNull(defaultProfile);
    }

    [Fact]
    public void Register_FluentApi_AllowsChaining()
    {
        var registry = new CameraRegistry()
            .Register(new CameraProfile.Builder().WithId("a").WithCamFile("a.cam").Build())
            .Register(new CameraProfile.Builder().WithId("b").WithCamFile("b.cam").Build());

        Assert.Equal(2, registry.ProfileIds.Count);
    }
}
