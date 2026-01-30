# Camera Capture Mode Guide

This guide explains the two capture modes available in PeanutVision and helps you choose the right one for your application.

## Overview

| Mode | Description | Best For |
|------|-------------|----------|
| **Continuous** | Camera streams frames automatically | Live preview, video, real-time inspection |
| **Software Trigger** | Capture on-demand when you trigger | ML datasets, precise timing, event-driven |

---

## How They Work

```
┌─────────────────────────────────────────────────────────────────────┐
│                      CONTINUOUS MODE                                │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  Camera: ──●──●──●──●──●──●──●──●──●──●──▶  (frames flow non-stop) │
│            │  │  │  │  │  │  │  │  │  │                            │
│  Callback: ▼  ▼  ▼  ▼  ▼  ▼  ▼  ▼  ▼  ▼   (called for each frame) │
│                                                                     │
│  You decide: "Which frames do I want to keep?"                      │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────┐
│                    SOFTWARE TRIGGER MODE                            │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  Camera: ──────────────────────────────────▶  (waiting...)         │
│                    │              │                                 │
│  Your trigger:     ▼              ▼                                 │
│                    ●              ●            (frames on demand)   │
│                                                                     │
│  You decide: "When do I want to capture?"                           │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

---

## Trade-off Comparison

| Aspect | Continuous Mode | Software Trigger Mode |
|--------|-----------------|----------------------|
| **Frame Rate** | Maximum (camera limit) | You control |
| **CPU Usage** | High (processing all frames) | Low (only when triggered) |
| **Memory** | Need queue for backlog | Minimal |
| **Timing Control** | Frames arrive when ready | Exact timing control |
| **Complexity** | Need thread-safe callback | Simple sequential code |
| **Miss Frames?** | Yes, if callback too slow | No, you trigger each one |
| **Latency** | Frames already flowing | Small delay on trigger |

---

## Detailed Trade-offs

### 1. Performance vs Control

```
Continuous Mode:
├── ✅ Maximum throughput (30+ FPS possible)
├── ✅ Lowest latency (frames already in buffer)
├── ❌ Must process fast or drop frames
└── ❌ CPU always busy

Software Trigger:
├── ✅ CPU idle between captures
├── ✅ Never miss a frame you requested
├── ❌ Lower throughput (trigger overhead)
└── ❌ Small delay per capture (~10-50ms)
```

### 2. Code Complexity

**Continuous Mode** - More complex (async, thread-safety needed)

```csharp
using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Camera;
using System.Collections.Concurrent;

// Need thread-safe queue and background processing
ConcurrentQueue<byte[]> frameQueue = new();

channel.FrameAcquired += (s, e) =>
{
    // Callback runs on DRIVER thread - must be fast!
    frameQueue.Enqueue(e.Surface.ToArray());
    // Don't do heavy work here - will drop frames
};

// Separate thread for heavy processing
Task.Run(() => {
    while (running)
    {
        if (frameQueue.TryDequeue(out var frame))
            ProcessAndSave(frame);  // Heavy work here
    }
});
```

**Software Trigger** - Simple (sequential, no threading needed)

```csharp
using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Camera;

// Simple sequential code
for (int i = 0; i < 100; i++)
{
    channel.SendSoftwareTrigger();
    var surface = channel.WaitForFrame(5000);

    if (surface.HasValue)
    {
        var data = surface.Value.ToArray();
        channel.ReleaseSurface(surface.Value);

        // Take your time - no rush
        ProcessAndSave(data);
    }
}
```

### 3. Frame Timing

```
Continuous Mode - Frames arrive at camera's pace:
Time:    0ms    33ms    66ms    99ms    132ms
Frames:   ●──────●──────●──────●──────●
          └─ You can't control when frames arrive

Software Trigger - You control timing:
Time:    0ms         500ms              2000ms
Frames:   ●───────────●──────────────────●
          └─ Trigger    └─ Trigger         └─ Trigger

          Can sync with: button click, sensor, timer, external event
```

---

## When to Use Each Mode

### Use Continuous Mode When:

| Scenario | Why |
|----------|-----|
| Live video display | Need smooth, continuous stream |
| Real-time inspection | Analyze every frame for defects |
| High-speed recording | Capture maximum frames per second |
| Motion detection | Compare consecutive frames |

### Use Software Trigger When:

| Scenario | Why |
|----------|-----|
| ML dataset collection | Precise control, one image at a time |
| User-initiated capture | Button click = one photo |
| Sync with external event | Capture when sensor triggers |
| Barcode/QR scanning | Capture when product arrives |
| Low-power operation | Camera idle between captures |

---

## Decision Flowchart

```
                    ┌─────────────────────┐
                    │ What's your need?   │
                    └──────────┬──────────┘
                               │
              ┌────────────────┴────────────────┐
              │                                 │
              ▼                                 ▼
   ┌──────────────────┐             ┌──────────────────┐
   │ "I need to react │             │ "I decide when   │
   │  to every frame" │             │  to take photo"  │
   └────────┬─────────┘             └────────┬─────────┘
            │                                 │
            ▼                                 ▼
   ┌──────────────────┐             ┌──────────────────┐
   │  CONTINUOUS      │             │  SOFTWARE        │
   │  MODE            │             │  TRIGGER         │
   └──────────────────┘             └──────────────────┘
            │                                 │
            ▼                                 ▼
   • Video streaming              • ML dataset capture
   • Quality inspection           • On-demand photos
   • Motion tracking              • Event-driven capture
   • Frame counting               • Low CPU usage
```

---

## Code Examples

### Continuous Mode - Complete Example

```csharp
using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Camera;

public class ContinuousCapture
{
    public void Run()
    {
        using var service = new GrabService();
        service.Initialize();

        // Use continuous (free-run) profile
        using var channel = service.CreateChannel(CrevisProfiles.TC_A160K_FreeRun_RGB8);

        int frameCount = 0;

        channel.FrameAcquired += (sender, args) =>
        {
            frameCount++;

            // Example: Save every 100th frame
            if (frameCount % 100 == 0)
            {
                var data = args.Surface.ToArray();
                var filename = $"frame_{frameCount:D6}.png";

                // Save in background to avoid blocking callback
                Task.Run(() => ImageSaver.Save(data,
                    args.Surface.Width, args.Surface.Height,
                    args.Surface.Pitch, filename));
            }
        };

        channel.StartAcquisition();

        Console.WriteLine("Acquiring... Press any key to stop.");
        Console.ReadKey();

        channel.StopAcquisition();
        Console.WriteLine($"Total frames: {frameCount}");
    }
}
```

### Software Trigger Mode - Complete Example

```csharp
using PeanutVision.MultiCamDriver;
using PeanutVision.MultiCamDriver.Camera;

public class TriggeredCapture
{
    public void Run(string outputFolder, int count)
    {
        using var service = new GrabService();
        service.Initialize();

        // Use software trigger profile
        var options = CrevisProfiles.TC_A160K_SoftwareTrigger_RGB8
            .ToChannelOptions(McTrigMode.MC_TrigMode_SOFT, useCallback: false);

        using var channel = service.CreateChannel(options);
        channel.StartAcquisition();

        for (int i = 0; i < count; i++)
        {
            Console.WriteLine($"Capturing {i + 1}/{count}...");

            // Trigger capture
            channel.SendSoftwareTrigger();
            var surface = channel.WaitForFrame(5000);

            if (surface.HasValue)
            {
                // Copy data
                var data = surface.Value.ToArray();
                var width = surface.Value.Width;
                var height = surface.Value.Height;
                var pitch = surface.Value.Pitch;

                // Release surface back to driver
                channel.ReleaseSurface(surface.Value);

                // Save image (take your time)
                var filename = Path.Combine(outputFolder, $"image_{i:D4}.png");
                ImageSaver.Save(data, width, height, pitch, filename);

                Console.WriteLine($"  Saved: {filename}");
            }
            else
            {
                Console.WriteLine($"  Timeout - no frame received");
            }
        }

        channel.StopAcquisition();
    }
}
```

### Software Trigger - Button Click Pattern

```csharp
public class CaptureOnClick
{
    private GrabChannel _channel;

    public void Initialize()
    {
        var service = new GrabService();
        service.Initialize();

        var options = CrevisProfiles.TC_A160K_SoftwareTrigger_RGB8
            .ToChannelOptions(McTrigMode.MC_TrigMode_SOFT, useCallback: false);

        _channel = service.CreateChannel(options);
        _channel.StartAcquisition();
    }

    // Call this from UI button click
    public byte[]? CaptureImage()
    {
        _channel.SendSoftwareTrigger();
        var surface = _channel.WaitForFrame(5000);

        if (surface.HasValue)
        {
            var data = surface.Value.ToArray();
            _channel.ReleaseSurface(surface.Value);
            return data;
        }

        return null;
    }
}
```

---

## Saving Images

Both modes can save images using the same API:

```csharp
// After obtaining image data from either mode:
ImageSaver.Save(data, width, height, pitch, "output.png");  // Auto-detect format
ImageSaver.SaveAsPng(data, width, height, pitch, "output.png");  // Explicit PNG
ImageSaver.SaveAsBmp(data, width, height, pitch, "output.bmp");  // BMP format
ImageSaver.SaveAsRaw(data, width, height, pitch, "output.raw");  // Raw binary
```

### Recommended Format for ML

| Format | File Size | Use Case |
|--------|-----------|----------|
| **PNG** | ~15 MB | ML datasets (recommended) |
| BMP | ~38 MB | Windows compatibility |
| RAW | ~38 MB | Custom processing pipelines |

---

## Troubleshooting

| Problem | Mode | Solution |
|---------|------|----------|
| Dropped frames | Continuous | Reduce callback processing time, use queue |
| Timeout on WaitForFrame | Software Trigger | Check camera connection, increase timeout |
| Memory leak | Both | Always call `ReleaseSurface()` |
| High CPU usage | Continuous | Expected; use Software Trigger if not needed |
| Inconsistent timing | Continuous | Use Software Trigger for precise timing |

---

## Summary

| If you need... | Use... |
|----------------|--------|
| Maximum frame rate | Continuous Mode |
| Precise capture timing | Software Trigger Mode |
| Simple code | Software Trigger Mode |
| Live video | Continuous Mode |
| ML dataset collection | Software Trigger Mode |
| Low CPU usage | Software Trigger Mode |
| Real-time inspection | Continuous Mode |

**Simple Rule:**
- **"Camera decides when"** → Continuous Mode
- **"I decide when"** → Software Trigger Mode
