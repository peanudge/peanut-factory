namespace PeanutVision.MultiCamDriver;

/// <summary>
/// Example utility class for checking MultiCam system status.
/// Demonstrates basic driver initialization and board detection.
/// </summary>
public class MultiCamStatusChecker
{
    /// <summary>
    /// Checks the MultiCam system status and prints diagnostic information.
    /// </summary>
    public void CheckSystemStatus()
    {
        // 1. Driver connection (McOpenDriver requires NULL/null as reserved argument)
        int status = MultiCamNative.McOpenDriver(null);

        if (status != MultiCamNative.MC_OK)
        {
            Console.WriteLine($"MultiCam driver connection failed. Error code: {status}");
            return;
        }

        Console.WriteLine("MultiCam driver connected successfully.");

        try
        {
            // 2. Query installed board count from MC_CONFIGURATION object
            status = MultiCamNative.McGetParamNmInt(
                MultiCamNative.MC_CONFIGURATION,
                MultiCamNative.PN_BoardCount,
                out int boardCount
            );

            if (status == MultiCamNative.MC_OK)
            {
                Console.WriteLine($"Detected MultiCam boards: {boardCount}");

                if (boardCount == 0)
                {
                    Console.WriteLine("Warning: No boards detected in the system.");
                }
                else
                {
                    // Print info for each board
                    for (int i = 0; i < boardCount; i++)
                    {
                        PrintBoardInfo(i);
                    }
                }
            }
            else
            {
                Console.WriteLine($"Failed to get board information. Error code: {status}");
            }
        }
        finally
        {
            // 3. Close driver connection (must be called to release resources)
            MultiCamNative.McCloseDriver();
            Console.WriteLine("MultiCam driver connection closed.");
        }
    }

    private void PrintBoardInfo(int boardIndex)
    {
        uint boardHandle = MultiCamNative.MC_BOARD + (uint)boardIndex;
        byte[] buffer = new byte[256];

        Console.WriteLine($"\n--- Board {boardIndex} ---");

        if (MultiCamNative.McGetParamNmStr(boardHandle, MultiCamNative.PN_BoardName, buffer, (uint)buffer.Length) == MultiCamNative.MC_OK)
        {
            string name = System.Text.Encoding.UTF8.GetString(buffer).TrimEnd('\0');
            Console.WriteLine($"  Name: {name}");
        }

        if (MultiCamNative.McGetParamNmStr(boardHandle, MultiCamNative.PN_BoardType, buffer, (uint)buffer.Length) == MultiCamNative.MC_OK)
        {
            string type = System.Text.Encoding.UTF8.GetString(buffer).TrimEnd('\0');
            Console.WriteLine($"  Type: {type}");
        }

        if (MultiCamNative.McGetParamNmStr(boardHandle, MultiCamNative.PN_SerialNumber, buffer, (uint)buffer.Length) == MultiCamNative.MC_OK)
        {
            string serial = System.Text.Encoding.UTF8.GetString(buffer).TrimEnd('\0');
            Console.WriteLine($"  Serial: {serial}");
        }
    }
}
