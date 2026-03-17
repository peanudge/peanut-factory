namespace PeanutVision.FakeCamDriver.FrameGenerators;

/// <summary>
/// Generates frame pixel data for fake camera driver.
/// </summary>
public interface IFrameGenerator
{
    /// <summary>
    /// Generates BGR24 pixel data into the buffer.
    /// </summary>
    /// <param name="buffer">BGR24 pixel buffer (length = height * pitch)</param>
    /// <param name="width">Image width in pixels</param>
    /// <param name="height">Image height in pixels</param>
    /// <param name="pitch">Row stride in bytes (typically width * 3)</param>
    /// <param name="frameNumber">Current frame number (0-based, incrementing)</param>
    void Generate(byte[] buffer, int width, int height, int pitch, int frameNumber);
}
