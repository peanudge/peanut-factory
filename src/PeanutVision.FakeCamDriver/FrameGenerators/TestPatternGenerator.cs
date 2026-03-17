namespace PeanutVision.FakeCamDriver.FrameGenerators;

/// <summary>
/// Generates a 3-region test pattern that changes every frame:
/// - Top 20%: SMPTE color bars (R, G, B, C, M, Y, W)
/// - Middle 60%: Horizontal gradient that scrolls each frame
/// - Bottom 20%: Checkerboard (8x8 blocks) that inverts on odd frames
/// - Top-left: 16-bit binary encoding of frame number
/// </summary>
public sealed class TestPatternGenerator : IFrameGenerator
{
    // BGR24 color bar colors (7 bars)
    private static readonly byte[][] ColorBars =
    [
        [0, 0, 255],       // Red (BGR)
        [0, 255, 0],       // Green
        [255, 0, 0],       // Blue
        [255, 255, 0],     // Cyan
        [255, 0, 255],     // Magenta
        [0, 255, 255],     // Yellow
        [255, 255, 255],   // White
    ];

    public void Generate(byte[] buffer, int width, int height, int pitch, int frameNumber)
    {
        int colorBarH = height / 5;
        int gradientH = height * 3 / 5;
        int checkerH = height - colorBarH - gradientH;

        GenerateColorBars(buffer, width, colorBarH, pitch, 0);
        GenerateMovingGradient(buffer, width, gradientH, pitch, colorBarH, frameNumber);
        GenerateCheckerboard(buffer, width, checkerH, pitch, colorBarH + gradientH, frameNumber);
        EncodeFrameNumber(buffer, width, pitch, frameNumber);
    }

    private static void GenerateColorBars(byte[] buffer, int width, int height, int pitch, int yOffset)
    {
        int barWidth = width / ColorBars.Length;

        for (int y = 0; y < height; y++)
        {
            int rowStart = (yOffset + y) * pitch;
            for (int x = 0; x < width; x++)
            {
                int barIndex = Math.Min(x / barWidth, ColorBars.Length - 1);
                var color = ColorBars[barIndex];
                int offset = rowStart + x * 3;
                buffer[offset] = color[0];     // B
                buffer[offset + 1] = color[1]; // G
                buffer[offset + 2] = color[2]; // R
            }
        }
    }

    private static void GenerateMovingGradient(byte[] buffer, int width, int height, int pitch, int yOffset, int frameNumber)
    {
        for (int y = 0; y < height; y++)
        {
            int rowStart = (yOffset + y) * pitch;
            for (int x = 0; x < width; x++)
            {
                byte val = (byte)((x * 256 / width + frameNumber * 4) % 256);
                int offset = rowStart + x * 3;
                buffer[offset] = val;     // B
                buffer[offset + 1] = val; // G
                buffer[offset + 2] = val; // R
            }
        }
    }

    private static void GenerateCheckerboard(byte[] buffer, int width, int height, int pitch, int yOffset, int frameNumber)
    {
        bool invert = (frameNumber & 1) != 0;
        const int blockSize = 8;

        for (int y = 0; y < height; y++)
        {
            int rowStart = (yOffset + y) * pitch;
            for (int x = 0; x < width; x++)
            {
                bool isWhite = ((x / blockSize) + (y / blockSize)) % 2 == 0;
                if (invert) isWhite = !isWhite;
                byte val = isWhite ? (byte)255 : (byte)0;

                int offset = rowStart + x * 3;
                buffer[offset] = val;
                buffer[offset + 1] = val;
                buffer[offset + 2] = val;
            }
        }
    }

    private static void EncodeFrameNumber(byte[] buffer, int width, int pitch, int frameNumber)
    {
        // Encode lower 16 bits as 16x16 pixel blocks in the top-left corner
        const int blockSize = 16;
        ushort value = (ushort)(frameNumber & 0xFFFF);

        for (int bit = 0; bit < 16; bit++)
        {
            bool isSet = ((value >> (15 - bit)) & 1) != 0;
            byte r = isSet ? (byte)0 : (byte)0;
            byte g = isSet ? (byte)255 : (byte)0;
            byte b = isSet ? (byte)0 : (byte)0;

            int xStart = bit * blockSize;
            if (xStart + blockSize > width) break;

            for (int dy = 0; dy < blockSize && dy < buffer.Length / pitch; dy++)
            {
                int rowStart = dy * pitch;
                for (int dx = 0; dx < blockSize; dx++)
                {
                    int offset = rowStart + (xStart + dx) * 3;
                    if (offset + 2 < buffer.Length)
                    {
                        buffer[offset] = b;
                        buffer[offset + 1] = g;
                        buffer[offset + 2] = r;
                    }
                }
            }
        }
    }
}
