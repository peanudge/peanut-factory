namespace PeanutVision.Api.Exceptions;

public abstract class BusinessException(string message, int statusCode, string errorCode)
    : Exception(message)
{
    public int StatusCode { get; } = statusCode;
    public string ErrorCode { get; } = errorCode;
}

public sealed class ChannelNotAvailableException()
    : BusinessException("No active acquisition channel.", 409, "CHANNEL_NOT_AVAILABLE");

public sealed class AcquisitionConflictException(string message)
    : BusinessException(message, 409, "ACQUISITION_CONFLICT");

public sealed class ResourceNotFoundException(string message)
    : BusinessException(message, 404, "RESOURCE_NOT_FOUND");

public sealed class InvalidParameterException(string message)
    : BusinessException(message, 400, "INVALID_PARAMETER");
