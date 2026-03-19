using PeanutVision.MultiCamDriver.Camera;

namespace PeanutVision.Api.Tests.Infrastructure;

/// <summary>
/// Creates a temporary directory with dummy .cam files for testing.
/// Returns a CamFileService backed by those files.
/// </summary>
public static class TestCamFileHelper
{
    private static readonly object Lock = new();
    private static ICamFileService? _instance;
    private static string? _testDir;

    public static readonly string[] DefaultCamFileNames =
    [
        "crevis-tc-a160k-freerun-rgb8.cam",
        "crevis-tc-a160k-freerun-1tap-rgb8.cam",
        "crevis-tc-a160k-softtrig-rgb8.cam",
    ];

    public static ICamFileService GetOrCreate()
    {
        if (_instance != null) return _instance;

        lock (Lock)
        {
            if (_instance != null) return _instance;

            _testDir = Path.Combine(Path.GetTempPath(), $"PeanutVision.CamFileTests.{Guid.NewGuid():N}");
            Directory.CreateDirectory(_testDir);

            var camFileContents = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["crevis-tc-a160k-freerun-rgb8.cam"] = "TrigMode = IMMEDIATE\n",
                ["crevis-tc-a160k-freerun-1tap-rgb8.cam"] = "TrigMode = IMMEDIATE\n",
                ["crevis-tc-a160k-softtrig-rgb8.cam"] = "TrigMode = SOFT\n",
            };

            foreach (var name in DefaultCamFileNames)
            {
                var path = Path.Combine(_testDir, name);
                if (!File.Exists(path))
                {
                    var content = camFileContents.TryGetValue(name, out var c) ? c : "";
                    File.WriteAllText(path, content);
                }
            }

            _instance = new CamFileService(_testDir);
            return _instance;
        }
    }
}
