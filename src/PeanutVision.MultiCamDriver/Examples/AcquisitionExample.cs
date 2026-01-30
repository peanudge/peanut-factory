namespace PeanutVision.MultiCamDriver.Examples;

/// <summary>
/// Example demonstrating high-speed image acquisition with the
/// Euresys Grablink Full (PC1622) and Crevis TC-A160K camera.
/// </summary>
public static class AcquisitionExample
{
    /// <summary>
    /// Basic callback-based acquisition example.
    /// This is the recommended approach for high-speed acquisition.
    /// </summary>
    public static void RunCallbackAcquisition()
    {
        // Path to your camera configuration file
        const string camFilePath = @"C:\Program Files\Euresys\MultiCam\Cameras\TC-A160K-SEM_freerun_RGB8.cam";

        using var service = new GrabService();
        service.Initialize();

        Console.WriteLine($"Driver Version: {service.DriverVersion}");
        Console.WriteLine($"Boards Detected: {service.BoardCount}");

        if (service.BoardCount == 0)
        {
            Console.WriteLine("No frame grabber boards detected!");
            return;
        }

        // Print board info
        var boardInfo = service.GetBoardInfo(0);
        Console.WriteLine($"Using board: {boardInfo.BoardName} (S/N: {boardInfo.SerialNumber})");

        // Create channel with the TC-A160K camera file
        using var channel = service.CreateChannel(new GrabChannelOptions
        {
            DriverIndex = 0,
            Connector = "M",  // Medium connector for Camera Link Full
            CamFilePath = camFilePath,
            SurfaceCount = 4,  // 4 frame buffers for smooth acquisition
            UseCallback = true,
            TriggerMode = McTrigMode.MC_TrigMode_IMMEDIATE  // Free-run mode
        });

        Console.WriteLine($"Image Size: {channel.ImageWidth} x {channel.ImageHeight}");

        // Frame counter for demonstration
        int frameCount = 0;
        var startTime = DateTime.UtcNow;

        // Subscribe to frame events
        channel.FrameAcquired += (sender, args) =>
        {
            frameCount++;

            // Process frame data here
            // WARNING: This callback runs on MultiCam's thread - keep it fast!

            // Example: Copy data to managed array for later processing
            // byte[] frameData = args.Surface.ToArray();

            // Or use Span for zero-copy access (only valid during callback)
            // ReadOnlySpan<byte> span = args.Surface.AsSpan();

            if (frameCount % 100 == 0)
            {
                var elapsed = (DateTime.UtcNow - startTime).TotalSeconds;
                var fps = frameCount / elapsed;
                Console.WriteLine($"Frame {frameCount}: {fps:F1} FPS");
            }
        };

        channel.AcquisitionError += (sender, args) =>
        {
            Console.WriteLine($"Acquisition Error: {args.Message} (Signal: {args.Signal})");
        };

        channel.AcquisitionEnded += (sender, args) =>
        {
            Console.WriteLine("Acquisition ended.");
        };

        // Start continuous acquisition
        Console.WriteLine("Starting acquisition... Press Enter to stop.");
        channel.StartAcquisition();

        Console.ReadLine();

        // Stop acquisition
        channel.StopAcquisition();

        var totalTime = (DateTime.UtcNow - startTime).TotalSeconds;
        Console.WriteLine($"Captured {frameCount} frames in {totalTime:F2} seconds ({frameCount / totalTime:F1} FPS)");
    }

    /// <summary>
    /// Software trigger acquisition example.
    /// Use this when you need precise control over frame capture timing.
    /// </summary>
    public static void RunSoftwareTriggerAcquisition()
    {
        const string camFilePath = @"C:\Program Files\Euresys\MultiCam\Cameras\TC-A160K-SEM_freerun_RGB8.cam";

        using var service = new GrabService();
        service.Initialize();

        using var channel = service.CreateChannel(new GrabChannelOptions
        {
            DriverIndex = 0,
            Connector = "M",
            CamFilePath = camFilePath,
            SurfaceCount = 2,
            UseCallback = false,  // Use polling instead of callbacks
            TriggerMode = McTrigMode.MC_TrigMode_SOFT  // Software trigger mode
        });

        // Start acquisition (will wait for triggers)
        channel.StartAcquisition();

        Console.WriteLine("Capturing 10 frames with software triggers...");

        for (int i = 0; i < 10; i++)
        {
            // Send software trigger
            channel.SendSoftwareTrigger();

            // Wait for frame (5 second timeout)
            var surface = channel.WaitForFrame(5000);

            if (surface.HasValue)
            {
                Console.WriteLine($"Frame {i + 1}: {surface.Value.Width}x{surface.Value.Height}, " +
                                $"Address: 0x{surface.Value.Address:X}");

                // Process the frame...

                // IMPORTANT: Release the surface back to MultiCam
                channel.ReleaseSurface(surface.Value);
            }
            else
            {
                Console.WriteLine($"Frame {i + 1}: Timeout!");
            }
        }

        channel.StopAcquisition();
    }

    /// <summary>
    /// Calibration example demonstrating Flat Field Correction and White Balance.
    /// </summary>
    public static void RunCalibration()
    {
        const string camFilePath = @"C:\Program Files\Euresys\MultiCam\Cameras\TC-A160K-SEM_freerun_RGB8.cam";

        using var service = new GrabService();
        service.Initialize();

        using var channel = service.CreateChannel(new GrabChannelOptions
        {
            DriverIndex = 0,
            Connector = "M",
            CamFilePath = camFilePath,
            SurfaceCount = 2,
            TriggerMode = McTrigMode.MC_TrigMode_IMMEDIATE
        });

        // === FLAT FIELD CORRECTION (FFC) ===
        Console.WriteLine("\n=== Flat Field Correction Calibration ===");

        // Step 1: Black Calibration
        Console.WriteLine("\n1. BLACK CALIBRATION");
        Console.WriteLine("   - Cover the lens completely or ensure no light reaches the sensor");
        Console.WriteLine("   Press Enter when ready...");
        Console.ReadLine();

        try
        {
            channel.PerformBlackCalibration();
            Console.WriteLine("   Black calibration completed successfully.");
        }
        catch (MultiCamException ex)
        {
            Console.WriteLine($"   Black calibration failed: {ex.Message}");
        }

        // Step 2: White Calibration
        Console.WriteLine("\n2. WHITE CALIBRATION");
        Console.WriteLine("   - Point the camera at a uniform white surface");
        Console.WriteLine("   - Adjust lighting to achieve ~200 DN (digital numbers) average");
        Console.WriteLine("   Press Enter when ready...");
        Console.ReadLine();

        try
        {
            channel.PerformWhiteCalibration();
            Console.WriteLine("   White calibration completed successfully.");

            // Enable flat field correction
            channel.SetFlatFieldCorrection(true);
            Console.WriteLine("   Flat field correction enabled.");
        }
        catch (MultiCamException ex)
        {
            Console.WriteLine($"   White calibration failed: {ex.Message}");
        }

        // === WHITE BALANCE ===
        Console.WriteLine("\n=== White Balance Calibration ===");
        Console.WriteLine("   - Point the camera at a neutral gray/white reference");
        Console.WriteLine("   Press Enter when ready...");
        Console.ReadLine();

        try
        {
            // Get current ratios
            var (r, g, b) = channel.GetWhiteBalanceRatios();
            Console.WriteLine($"   Current ratios - R: {r:F3}, G: {g:F3}, B: {b:F3}");

            // Perform auto white balance once
            channel.PerformWhiteBalanceOnce();
            Console.WriteLine("   White balance auto-adjustment completed.");

            // Get new ratios
            (r, g, b) = channel.GetWhiteBalanceRatios();
            Console.WriteLine($"   New ratios - R: {r:F3}, G: {g:F3}, B: {b:F3}");
        }
        catch (MultiCamException ex)
        {
            Console.WriteLine($"   White balance failed: {ex.Message}");
        }

        // === EXPOSURE CONTROL ===
        Console.WriteLine("\n=== Exposure Settings ===");
        try
        {
            var (minExp, maxExp) = channel.GetExposureRange();
            var currentExp = channel.GetExposureUs();
            Console.WriteLine($"   Exposure range: {minExp:F0} - {maxExp:F0} us");
            Console.WriteLine($"   Current exposure: {currentExp:F0} us");

            // Example: Set exposure to 1000 us (1 ms)
            // channel.SetExposureUs(1000);
        }
        catch (MultiCamException ex)
        {
            Console.WriteLine($"   Exposure control not available: {ex.Message}");
        }

        Console.WriteLine("\nCalibration complete. Press Enter to exit.");
        Console.ReadLine();
    }
}
