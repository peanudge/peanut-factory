namespace PeanutVision.MultiCamDriver.Tests;

public class CamFileResourceTests
{
    [Fact]
    public void GetAvailableCamFiles_ReturnsEmbeddedFiles()
    {
        var camFiles = CamFileResource.GetAvailableCamFiles().ToList();

        Assert.NotEmpty(camFiles);
        Assert.All(camFiles, f => Assert.EndsWith(".cam", f));
    }

    [Fact]
    public void GetAvailableCamFiles_ContainsKnownFiles()
    {
        var camFiles = CamFileResource.GetAvailableCamFiles().ToList();

        // At least one of the known files should be present
        Assert.True(
            camFiles.Contains(CamFileResource.KnownCamFiles.TC_A160K_FreeRun_RGB8) ||
            camFiles.Contains(CamFileResource.KnownCamFiles.TC_A160K_FreeRun_1TAP_RGB8),
            $"Expected known cam files. Found: {string.Join(", ", camFiles)}");
    }

    [Fact]
    public void IsCamFileAvailable_ReturnsTrueForExistingFile()
    {
        var camFiles = CamFileResource.GetAvailableCamFiles().ToList();

        if (camFiles.Count > 0)
        {
            bool isAvailable = CamFileResource.IsCamFileAvailable(camFiles[0]);
            Assert.True(isAvailable);
        }
    }

    [Fact]
    public void IsCamFileAvailable_ReturnsFalseForNonExistingFile()
    {
        bool isAvailable = CamFileResource.IsCamFileAvailable("nonexistent_camera.cam");

        Assert.False(isAvailable);
    }

    [Fact]
    public void GetCamFilePath_ExtractsFileToTempDirectory()
    {
        var camFiles = CamFileResource.GetAvailableCamFiles().ToList();

        if (camFiles.Count == 0)
        {
            // Skip if no cam files are embedded
            return;
        }

        string camFileName = camFiles[0];
        string extractedPath = CamFileResource.GetCamFilePath(camFileName);

        Assert.True(File.Exists(extractedPath), $"File should exist at: {extractedPath}");
        Assert.EndsWith(camFileName, extractedPath);
    }

    [Fact]
    public void GetCamFilePath_ReturnsSamePathOnMultipleCalls()
    {
        var camFiles = CamFileResource.GetAvailableCamFiles().ToList();

        if (camFiles.Count == 0)
        {
            return;
        }

        string camFileName = camFiles[0];
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
    public void ExtractAllCamFiles_ExtractsAllFiles()
    {
        CamFileResource.ExtractAllCamFiles();

        var camFiles = CamFileResource.GetAvailableCamFiles().ToList();
        string tempDir = CamFileResource.GetDirectory();

        foreach (var camFile in camFiles)
        {
            string expectedPath = Path.Combine(tempDir, camFile);
            Assert.True(File.Exists(expectedPath), $"Expected file at: {expectedPath}");
        }
    }

    [Fact]
    public void KnownCamFiles_Constants_HaveCorrectExtension()
    {
        Assert.EndsWith(".cam", CamFileResource.KnownCamFiles.TC_A160K_FreeRun_RGB8);
        Assert.EndsWith(".cam", CamFileResource.KnownCamFiles.TC_A160K_FreeRun_1TAP_RGB8);
    }
}
