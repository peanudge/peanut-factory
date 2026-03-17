namespace PeanutVision.MultiCamDriver.Camera;

/// <summary>
/// Read-only service for accessing parsed .cam file metadata.
/// </summary>
public interface ICamFileService
{
    /// <summary>Directory where .cam files are located.</summary>
    string Directory { get; }

    /// <summary>All parsed cam file infos.</summary>
    IReadOnlyList<CamFileInfo> CamFiles { get; }

    /// <summary>
    /// Gets a cam file info by file name.
    /// </summary>
    /// <exception cref="KeyNotFoundException">If file name not found</exception>
    CamFileInfo GetByFileName(string fileName);

    /// <summary>
    /// Tries to get a cam file info by file name.
    /// </summary>
    bool TryGetByFileName(string fileName, out CamFileInfo? camFile);
}
