using System.Reflection;

namespace PeanutVision.MultiCamDriver;

/// <summary>
/// Provides access to embedded camera configuration (.cam) files.
/// Extracts them to a temporary directory for use with MultiCam API.
/// </summary>
public static class CamFileResource
{
    private static readonly Assembly Assembly = typeof(CamFileResource).Assembly;
    private static string _directory = Path.Combine(Path.GetTempPath(), "PeanutVision.CamFiles");
    private static readonly object Lock = new();
    private static bool _initialized;

    /// <summary>
    /// Sets the directory where cam files are stored/extracted.
    /// Must be called before any cam file access. If the directory contains
    /// .cam files already, they will be used directly without extraction.
    /// </summary>
    public static void SetDirectory(string path)
    {
        lock (Lock)
        {
            _directory = Path.GetFullPath(path);
            _initialized = false;
        }
    }

    /// <summary>
    /// Known embedded camera configurations.
    /// </summary>
    public static class KnownCamFiles
    {
        /// <summary>Crevis TC-A160K, Free-run mode, 1 TAP, RGB8</summary>
        public const string TC_A160K_FreeRun_1TAP_RGB8 = "CREVIS_TC-A160K-SEM_FreeRun_1TAP_RGB8.cam";

        /// <summary>TC-A160K, Free-run mode, RGB8</summary>
        public const string TC_A160K_FreeRun_RGB8 = "TC-A160K-SEM_freerun_RGB8.cam";
    }

    /// <summary>
    /// Gets the file path to an embedded camera configuration file.
    /// Extracts the file to a temporary directory if not already extracted.
    /// </summary>
    /// <param name="camFileName">Name of the .cam file (e.g., "TC-A160K-SEM_freerun_RGB8.cam")</param>
    /// <returns>Full path to the extracted .cam file</returns>
    /// <exception cref="FileNotFoundException">If the specified cam file is not embedded</exception>
    public static string GetCamFilePath(string camFileName)
    {
        EnsureInitialized();

        string targetPath = Path.Combine(_directory, camFileName);

        if (File.Exists(targetPath))
        {
            return targetPath;
        }

        // Extract from embedded resource
        string resourceName = $"CamFiles.{camFileName}";
        using var stream = Assembly.GetManifestResourceStream(resourceName);

        if (stream == null)
        {
            throw new FileNotFoundException(
                $"Embedded camera file '{camFileName}' not found. Available: {string.Join(", ", GetAvailableCamFiles())}",
                camFileName);
        }

        using var fileStream = File.Create(targetPath);
        stream.CopyTo(fileStream);

        return targetPath;
    }

    /// <summary>
    /// Gets the file path for the TC-A160K FreeRun RGB8 configuration.
    /// </summary>
    public static string GetTC_A160K_FreeRunPath()
    {
        return GetCamFilePath(KnownCamFiles.TC_A160K_FreeRun_RGB8);
    }

    /// <summary>
    /// Lists all available embedded camera configuration files.
    /// </summary>
    public static IEnumerable<string> GetAvailableCamFiles()
    {
        const string prefix = "CamFiles.";

        return Assembly.GetManifestResourceNames()
            .Where(name => name.StartsWith(prefix) && name.EndsWith(".cam"))
            .Select(name => name.Substring(prefix.Length));
    }

    /// <summary>
    /// Checks if a camera configuration file is available as an embedded resource.
    /// </summary>
    public static bool IsCamFileAvailable(string camFileName)
    {
        string resourceName = $"CamFiles.{camFileName}";
        return Assembly.GetManifestResourceStream(resourceName) != null;
    }

    /// <summary>
    /// Extracts all embedded camera files to the temporary directory.
    /// </summary>
    public static void ExtractAllCamFiles()
    {
        EnsureInitialized();

        foreach (var camFile in GetAvailableCamFiles())
        {
            GetCamFilePath(camFile);
        }
    }

    /// <summary>
    /// Gets the directory where cam files are stored.
    /// </summary>
    public static string GetDirectory()
    {
        EnsureInitialized();
        return _directory;
    }

    /// <summary>
    /// Cleans up extracted cam files from the temporary directory.
    /// </summary>
    public static void Cleanup()
    {
        lock (Lock)
        {
            if (Directory.Exists(_directory))
            {
                try
                {
                    Directory.Delete(_directory, recursive: true);
                }
                catch
                {
                    // Ignore cleanup errors
                }
            }
            _initialized = false;
        }
    }

    private static void EnsureInitialized()
    {
        if (_initialized) return;

        lock (Lock)
        {
            if (_initialized) return;

            if (!Directory.Exists(_directory))
            {
                Directory.CreateDirectory(_directory);
            }

            _initialized = true;
        }
    }
}
