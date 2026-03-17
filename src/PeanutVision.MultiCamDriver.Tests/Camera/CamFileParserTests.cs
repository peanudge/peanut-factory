using PeanutVision.MultiCamDriver.Camera;

namespace PeanutVision.MultiCamDriver.Tests.Camera;

public class CamFileParserTests
{
    private static readonly string[] SampleCamFile =
    [
        "; +-------------------+",
        "; | Original CAM File |",
        "; +-------------------+",
        ";*********************************************************************************************",
        "; Camera Manufacturer: Crevis",
        "; Camera Model: TC-A160K",
        ";*********************************************************************************************",
        "    Camera =            MyCameraLink;",
        "    Spectrum =          BW;                 <== BW COLOR ...",
        "    TapConfiguration =  BASE_1T8;           <== BASE_1T8 BASE_1T10 BASE_1T24 ...",
        "    Hactive_Px =        640;                <==",
        "    Vactive_Ln =        480;                <==",
        "    AcquisitionMode =   SNAPSHOT;           <== SNAPSHOT HFR VIDEO",
        "    TrigMode =          IMMEDIATE;          <== IMMEDIATE HARD SOFT COMBINED",
        "    ColorFormat =       Y8;                 <== Y8 Y10 RGB24 ...",
        "",
        "; +------------------------------------------+",
        "; | User Settings (Thu May 21 17:10:51 2020) |",
        "; +------------------------------------------+",
        "",
        "Spectrum = COLOR",
        "Hactive_Px = 1456",
        "Vactive_Ln = 1088",
        "TapConfiguration = BASE_1T24",
        "ColorFormat = RGB24",
        "TrigMode = SOFT",
    ];

    [Fact]
    public void ParseLines_ExtractsManufacturer()
    {
        var info = CamFileParser.ParseLines(SampleCamFile, "/test/sample.cam");

        Assert.Equal("Crevis", info.Manufacturer);
    }

    [Fact]
    public void ParseLines_ExtractsCameraModel()
    {
        var info = CamFileParser.ParseLines(SampleCamFile, "/test/sample.cam");

        Assert.Equal("TC-A160K", info.CameraModel);
    }

    [Fact]
    public void ParseLines_UserSettingsOverrideOriginal()
    {
        var info = CamFileParser.ParseLines(SampleCamFile, "/test/sample.cam");

        Assert.Equal(1456, info.Width);
        Assert.Equal(1088, info.Height);
        Assert.Equal("COLOR", info.Spectrum);
        Assert.Equal("RGB24", info.ColorFormat);
        Assert.Equal("BASE_1T24", info.TapConfiguration);
        Assert.Equal("SOFT", info.TrigMode);
    }

    [Fact]
    public void ParseLines_SetsFileName()
    {
        var info = CamFileParser.ParseLines(SampleCamFile, "/test/sample.cam");

        Assert.Equal("sample.cam", info.FileName);
    }

    [Fact]
    public void ParseLines_KeepsAcquisitionModeFromOriginal()
    {
        var info = CamFileParser.ParseLines(SampleCamFile, "/test/sample.cam");

        Assert.Equal("SNAPSHOT", info.AcquisitionMode);
    }

    [Fact]
    public void ParseLines_EmptyFile_ReturnsDefaults()
    {
        var info = CamFileParser.ParseLines([], "/test/empty.cam");

        Assert.Equal("empty.cam", info.FileName);
        Assert.Equal("Unknown", info.Manufacturer);
        Assert.Equal("Unknown", info.CameraModel);
        Assert.Equal(0, info.Width);
        Assert.Equal(0, info.Height);
        Assert.Equal("BW", info.Spectrum);
        Assert.Equal("Y8", info.ColorFormat);
        Assert.Equal("IMMEDIATE", info.TrigMode);
        Assert.Equal("SNAPSHOT", info.AcquisitionMode);
    }

    [Fact]
    public void ParseLines_TrimsInlineArrowComments()
    {
        string[] lines = ["    Hactive_Px =        4160;                <== 640 1280 4160"];
        var info = CamFileParser.ParseLines(lines, "/test/test.cam");

        Assert.Equal(4160, info.Width);
    }

    [Fact]
    public void ParseLines_IgnoresCommentedOutLines()
    {
        string[] lines =
        [
            ";   TrigMode = HARD;",
            "    TrigMode = SOFT;",
        ];
        var info = CamFileParser.ParseLines(lines, "/test/test.cam");

        Assert.Equal("SOFT", info.TrigMode);
    }

    [Fact]
    public void Parse_ThrowsForMissingFile()
    {
        Assert.Throws<FileNotFoundException>(() => CamFileParser.Parse("/nonexistent/file.cam"));
    }

    [Fact]
    public void Parse_RealFile_ReturnsValidInfo()
    {
        var testDir = Path.Combine(Path.GetTempPath(), $"CamParserTest.{Guid.NewGuid():N}");
        Directory.CreateDirectory(testDir);

        try
        {
            var filePath = Path.Combine(testDir, "test.cam");
            File.WriteAllLines(filePath, SampleCamFile);

            var info = CamFileParser.Parse(filePath);

            Assert.Equal("test.cam", info.FileName);
            Assert.Equal(Path.GetFullPath(filePath), info.FilePath);
            Assert.Equal("Crevis", info.Manufacturer);
            Assert.Equal(1456, info.Width);
        }
        finally
        {
            Directory.Delete(testDir, recursive: true);
        }
    }
}
