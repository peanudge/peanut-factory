using PeanutVision.MultiCamDriver.Camera;

namespace PeanutVision.MultiCamDriver.Tests.Camera;

[Collection("CamFileResource")]
public class CameraRegistryTests
{
    private static CameraRegistry CreateTestRegistry()
    {
        var registry = new CameraRegistry();
        registry.Register(new CameraProfile.Builder()
            .WithId("test-freerun")
            .WithDisplayName("Test FreeRun")
            .WithManufacturer("TestCorp")
            .WithModel("CAM-100")
            .WithCamFile("freerun.cam")
            .Build());
        registry.Register(new CameraProfile.Builder()
            .WithId("test-trigger")
            .WithDisplayName("Test Trigger")
            .WithManufacturer("TestCorp")
            .WithModel("CAM-100")
            .WithCamFile("trigger.cam")
            .Build());
        return registry;
    }

    [Fact]
    public void GetProfile_ReturnsCorrectProfile()
    {
        var registry = CreateTestRegistry();

        var profile = registry.GetProfile("test-freerun");

        Assert.Equal("TestCorp", profile.Manufacturer);
        Assert.Equal("CAM-100", profile.Model);
    }

    [Fact]
    public void GetProfile_CaseInsensitive()
    {
        var registry = CreateTestRegistry();

        var profile = registry.GetProfile("TEST-FREERUN");

        Assert.NotNull(profile);
    }

    [Fact]
    public void GetProfile_NotFound_Throws()
    {
        var registry = CreateTestRegistry();

        var ex = Assert.Throws<KeyNotFoundException>(() =>
            registry.GetProfile("nonexistent-camera"));

        Assert.Contains("nonexistent-camera", ex.Message);
    }

    [Fact]
    public void TryGetProfile_Found_ReturnsTrue()
    {
        var registry = CreateTestRegistry();

        var found = registry.TryGetProfile("test-freerun", out var profile);

        Assert.True(found);
        Assert.NotNull(profile);
    }

    [Fact]
    public void TryGetProfile_NotFound_ReturnsFalse()
    {
        var registry = CreateTestRegistry();

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
        var registry = CreateTestRegistry();

        var matched = registry.GetByManufacturer("TestCorp").ToList();

        Assert.Equal(2, matched.Count);
        Assert.All(matched, p => Assert.Equal("TestCorp", p.Manufacturer));
    }

    [Fact]
    public void GetByModel_ReturnsMatchingProfiles()
    {
        var registry = CreateTestRegistry();

        var matched = registry.GetByModel("CAM-100").ToList();

        Assert.Equal(2, matched.Count);
        Assert.All(matched, p => Assert.Equal("CAM-100", p.Model));
    }

    [Fact]
    public void ProfileIds_ReturnsAllIds()
    {
        var registry = CreateTestRegistry();

        var ids = registry.ProfileIds;

        Assert.Contains("test-freerun", ids, StringComparer.OrdinalIgnoreCase);
        Assert.Contains("test-trigger", ids, StringComparer.OrdinalIgnoreCase);
    }

    [Fact]
    public void DefaultProfile_ReturnsFirstProfile()
    {
        var registry = CreateTestRegistry();

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

    [Fact]
    public void LoadFromDirectory_CreatesProfilesFromCamFiles()
    {
        var testDir = Path.Combine(Path.GetTempPath(), $"PeanutVision.RegistryTest.{Guid.NewGuid():N}");
        Directory.CreateDirectory(testDir);
        var previousDir = CamFileResource.GetDirectory();

        try
        {
            File.WriteAllText(Path.Combine(testDir, "camera1.cam"), "");
            File.WriteAllText(Path.Combine(testDir, "camera2.cam"), "");
            CamFileResource.SetDirectory(testDir);

            var registry = new CameraRegistry().LoadFromDirectory();

            Assert.Equal(2, registry.Profiles.Count);
            Assert.True(registry.HasProfile("camera1"));
            Assert.True(registry.HasProfile("camera2"));
        }
        finally
        {
            CamFileResource.SetDirectory(previousDir);
            Directory.Delete(testDir, recursive: true);
        }
    }
}
