using PeanutVision.MultiCamDriver;

namespace PeanutVision.Api.Services;

/// <summary>
/// Maps raw MultiCam error codes and exception messages to human-readable messages
/// suitable for API consumers. Prevents internal driver details from leaking to the UI.
/// </summary>
public static class ErrorMessageSanitizer
{
    private static readonly Dictionary<McStatus, string> Messages = new()
    {
        [McStatus.MC_OK] = "Operation completed successfully.",
        [McStatus.MC_NO_BOARD_FOUND] = "No frame grabber board was found. Check that the board is installed and powered on.",
        [McStatus.MC_BAD_PARAMETER] = "Invalid parameter name or value.",
        [McStatus.MC_IO_ERROR] = "Hardware I/O error. Check camera cable connections.",
        [McStatus.MC_INTERNAL_ERROR] = "Internal driver error. Please restart the application.",
        [McStatus.MC_NO_MORE_RESOURCES] = "System resources exhausted. Close unused channels and try again.",
        [McStatus.MC_IN_USE] = "The resource is currently in use by another operation.",
        [McStatus.MC_NOT_SUPPORTED] = "This operation is not supported by the current hardware configuration.",
        [McStatus.MC_DATABASE_ERROR] = "Driver configuration database error.",
        [McStatus.MC_OUT_OF_BOUND] = "Parameter value is out of the allowed range.",
        [McStatus.MC_INSTANCE_NOT_FOUND] = "The requested resource was not found.",
        [McStatus.MC_INVALID_HANDLE] = "Invalid resource handle. The resource may have been released.",
        [McStatus.MC_TIMEOUT] = "Operation timed out. The camera may not be responding.",
        [McStatus.MC_INVALID_VALUE] = "The provided value is not valid for this parameter.",
        [McStatus.MC_RANGE_ERROR] = "Value is outside the acceptable range.",
        [McStatus.MC_BAD_HW_CONFIG] = "Hardware configuration error. Check board and camera link settings.",
        [McStatus.MC_NO_EVENT] = "No event available.",
        [McStatus.MC_LICENSE_NOT_GRANTED] = "License not granted. Check your Euresys license.",
        [McStatus.MC_FATAL_ERROR] = "Fatal hardware error. The system must be restarted.",
        [McStatus.MC_HW_EVENT_CONFLICT] = "Hardware event conflict. Multiple operations are competing for the same resource.",
        [McStatus.MC_FILE_NOT_FOUND] = "Camera configuration file not found.",
        [McStatus.MC_OVERFLOW] = "Buffer overflow. The system cannot keep up with the data rate.",
        [McStatus.MC_INVALID_PARAMETER_SETTING] = "Parameter settings are inconsistent. Review your acquisition configuration.",
        [McStatus.MC_PARAMETER_ILLEGAL_ACCESS] = "This parameter cannot be modified in the current state.",
        [McStatus.MC_CLUSTER_BUSY] = "Frame buffer cluster is busy. Increase buffer count or reduce frame rate.",
        [McStatus.MC_SERVICE_ERROR] = "Driver service not ready. The service may still be initializing.",
        [McStatus.MC_INVALID_SURFACE] = "Invalid surface buffer.",
        [McStatus.MC_BAD_GRABBER_CONFIG] = "Invalid grabber configuration. Review the board settings.",
    };

    /// <summary>
    /// Converts a MultiCam status code to a user-friendly message.
    /// </summary>
    public static string Sanitize(McStatus status)
        => Messages.TryGetValue(status, out var message) ? message : $"Unknown error (code: {(int)status}).";

    /// <summary>
    /// Converts a MultiCamException to a user-friendly error message.
    /// </summary>
    public static string Sanitize(MultiCamException ex)
    {
        var status = (McStatus)ex.StatusCode;
        return Messages.TryGetValue(status, out var message)
            ? message
            : $"Hardware error during {ex.Operation ?? "operation"} (code: {ex.StatusCode}).";
    }

    /// <summary>
    /// Sanitizes an arbitrary exception, handling MultiCamException specially.
    /// </summary>
    public static string Sanitize(Exception ex)
        => ex is MultiCamException mcEx ? Sanitize(mcEx) : ex.Message;
}
