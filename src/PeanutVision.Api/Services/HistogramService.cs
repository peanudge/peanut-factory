using PeanutVision.MultiCamDriver.Imaging;

namespace PeanutVision.Api.Services;

public static class HistogramService
{
    public sealed record ChannelHistogram(int[] Red, int[] Green, int[] Blue);

    /// <summary>
    /// Computes per-channel RGB histogram (256 bins each) from an ImageData frame.
    /// Assumes RGB24 format (BGR byte order per MultiCam convention).
    /// </summary>
    public static ChannelHistogram Compute(ImageData image)
    {
        var red = new int[256];
        var green = new int[256];
        var blue = new int[256];

        var pixels = image.Pixels;
        int bpp = image.Format.BytesPerPixel;

        if (bpp == 3) // RGB24 / BGR8
        {
            for (int y = 0; y < image.Height; y++)
            {
                int rowOffset = y * image.Pitch;
                for (int x = 0; x < image.Width; x++)
                {
                    int offset = rowOffset + x * 3;
                    blue[pixels[offset]]++;
                    green[pixels[offset + 1]]++;
                    red[pixels[offset + 2]]++;
                }
            }
        }
        else if (bpp == 1) // Grayscale
        {
            for (int y = 0; y < image.Height; y++)
            {
                int rowOffset = y * image.Pitch;
                for (int x = 0; x < image.Width; x++)
                {
                    byte val = pixels[rowOffset + x];
                    red[val]++;
                    green[val]++;
                    blue[val]++;
                }
            }
        }
        else if (bpp == 4) // RGBA32 / BGRa8
        {
            for (int y = 0; y < image.Height; y++)
            {
                int rowOffset = y * image.Pitch;
                for (int x = 0; x < image.Width; x++)
                {
                    int offset = rowOffset + x * 4;
                    blue[pixels[offset]]++;
                    green[pixels[offset + 1]]++;
                    red[pixels[offset + 2]]++;
                }
            }
        }

        return new ChannelHistogram(red, green, blue);
    }
}
