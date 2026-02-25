namespace PeanutVision.MultiCamDriver;

/// <summary>
/// Provides access to camera configuration (.cam) files from a directory on disk.
/// </summary>
public static class CamFileResource
{
    private static string _directory = Path.Combine(Path.GetTempPath(), "PeanutVision.CamFiles");
    private static readonly object Lock = new();
    private static bool _initialized;

    /// <summary>
    /// Sets the directory where cam files are loaded from.
    /// Must be called before any cam file access.
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
    /// Gets the file path to a camera configuration file in the configured directory.
    /// </summary>
    /// <param name="camFileName">Name of the .cam file (e.g., "TC-A160K-SEM_freerun_RGB8.cam")</param>
    /// <returns>Full path to the .cam file</returns>
    /// <exception cref="FileNotFoundException">If the specified cam file is not found in the directory</exception>
    public static string GetCamFilePath(string camFileName)
    {
        EnsureInitialized();

        string targetPath = Path.Combine(_directory, camFileName);

        if (!File.Exists(targetPath))
        {
            throw new FileNotFoundException(
                $"Camera file '{camFileName}' not found in '{_directory}'. Available: {string.Join(", ", GetAvailableCamFiles())}",
                camFileName);
        }

        return targetPath;
    }

    /// <summary>
    /// Lists all available camera configuration files in the configured directory.
    /// </summary>
    public static IEnumerable<string> GetAvailableCamFiles()
    {
        EnsureInitialized();

        if (!Directory.Exists(_directory))
            return Enumerable.Empty<string>();

        return Directory.GetFiles(_directory, "*.cam")
            .Select(Path.GetFileName)!;
    }

    /// <summary>
    /// Checks if a camera configuration file exists in the configured directory.
    /// </summary>
    public static bool IsCamFileAvailable(string camFileName)
    {
        EnsureInitialized();
        return File.Exists(Path.Combine(_directory, camFileName));
    }

    /// <summary>
    /// Gets the directory where cam files are stored.
    /// </summary>
    public static string GetDirectory()
    {
        EnsureInitialized();
        return _directory;
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
