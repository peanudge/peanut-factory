namespace PeanutVision.MultiCamDriver.Camera;

/// <summary>
/// Scans a directory for .cam files, parses them, and provides read-only access.
/// </summary>
public sealed class CamFileService : ICamFileService
{
    private readonly Dictionary<string, CamFileInfo> _byFileName;
    private readonly List<CamFileInfo> _camFiles;

    public string Directory { get; }
    public IReadOnlyList<CamFileInfo> CamFiles => _camFiles;

    public CamFileService(string directory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directory);

        Directory = Path.GetFullPath(directory);

        if (!System.IO.Directory.Exists(Directory))
        {
            System.IO.Directory.CreateDirectory(Directory);
        }

        _byFileName = new Dictionary<string, CamFileInfo>(StringComparer.OrdinalIgnoreCase);
        _camFiles = new List<CamFileInfo>();

        foreach (var filePath in System.IO.Directory.GetFiles(Directory, "*.cam"))
        {
            var info = CamFileParser.Parse(filePath);
            _byFileName[info.FileName] = info;
            _camFiles.Add(info);
        }
    }

    public CamFileInfo GetByFileName(string fileName)
    {
        if (_byFileName.TryGetValue(fileName, out var camFile))
            return camFile;

        var available = string.Join(", ", _byFileName.Keys);
        throw new KeyNotFoundException(
            $"Cam file '{fileName}' not found. Available: {available}");
    }

    public bool TryGetByFileName(string fileName, out CamFileInfo? camFile)
    {
        var found = _byFileName.TryGetValue(fileName, out var result);
        camFile = result;
        return found;
    }
}
