namespace PeanutVision.MultiCamDriver.Tests;

[Collection("CamFileResource")]
public class CamFileResourceTests : IDisposable
{
    private readonly string _testDir;

    public CamFileResourceTests()
    {
        _testDir = Path.Combine(Path.GetTempPath(), $"PeanutVision.CamFileTests.{Guid.NewGuid():N}");
        Directory.CreateDirectory(_testDir);
        CamFileResource.SetDirectory(_testDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDir))
            Directory.Delete(_testDir, recursive: true);
    }

    [Fact]
    public void GetAvailableCamFiles_ReturnsCamFilesFromDirectory()
    {
        File.WriteAllText(Path.Combine(_testDir, "test1.cam"), "");
        File.WriteAllText(Path.Combine(_testDir, "test2.cam"), "");

        var camFiles = CamFileResource.GetAvailableCamFiles().ToList();

        Assert.Equal(2, camFiles.Count);
        Assert.All(camFiles, f => Assert.EndsWith(".cam", f));
    }

    [Fact]
    public void GetAvailableCamFiles_ReturnsEmptyWhenNoFiles()
    {
        var camFiles = CamFileResource.GetAvailableCamFiles().ToList();

        Assert.Empty(camFiles);
    }

    [Fact]
    public void IsCamFileAvailable_ReturnsTrueForExistingFile()
    {
        File.WriteAllText(Path.Combine(_testDir, "test.cam"), "");

        Assert.True(CamFileResource.IsCamFileAvailable("test.cam"));
    }

    [Fact]
    public void IsCamFileAvailable_ReturnsFalseForNonExistingFile()
    {
        Assert.False(CamFileResource.IsCamFileAvailable("nonexistent_camera.cam"));
    }

    [Fact]
    public void GetCamFilePath_ReturnsPathForExistingFile()
    {
        string camFileName = "test.cam";
        File.WriteAllText(Path.Combine(_testDir, camFileName), "");

        string path = CamFileResource.GetCamFilePath(camFileName);

        Assert.True(File.Exists(path));
        Assert.EndsWith(camFileName, path);
    }

    [Fact]
    public void GetCamFilePath_ReturnsSamePathOnMultipleCalls()
    {
        string camFileName = "test.cam";
        File.WriteAllText(Path.Combine(_testDir, camFileName), "");

        string path1 = CamFileResource.GetCamFilePath(camFileName);
        string path2 = CamFileResource.GetCamFilePath(camFileName);

        Assert.Equal(path1, path2);
    }

    [Fact]
    public void GetCamFilePath_ThrowsForNonExistingFile()
    {
        Assert.Throws<FileNotFoundException>(() =>
            CamFileResource.GetCamFilePath("nonexistent_camera.cam"));
    }

    [Fact]
    public void GetDirectory_ReturnsValidPath()
    {
        string dir = CamFileResource.GetDirectory();

        Assert.NotEmpty(dir);
        Assert.True(Directory.Exists(dir));
    }

    [Fact]
    public void SetDirectory_ChangesDirectory()
    {
        string newDir = Path.Combine(Path.GetTempPath(), $"PeanutVision.CamFileTests2.{Guid.NewGuid():N}");
        Directory.CreateDirectory(newDir);

        try
        {
            CamFileResource.SetDirectory(newDir);

            Assert.Equal(newDir, CamFileResource.GetDirectory());
        }
        finally
        {
            Directory.Delete(newDir, recursive: true);
            CamFileResource.SetDirectory(_testDir);
        }
    }

}
