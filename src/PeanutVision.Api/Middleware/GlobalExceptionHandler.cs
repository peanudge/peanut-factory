using Microsoft.AspNetCore.Diagnostics;
using PeanutVision.Api.Exceptions;
using PeanutVision.Api.Services;
using PeanutVision.MultiCamDriver;

namespace PeanutVision.Api.Middleware;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        int status;
        string errorCode;
        string message;

        switch (exception)
        {
            case BusinessException biz:
                status = biz.StatusCode;
                errorCode = biz.ErrorCode;
                message = biz.Message;
                logger.LogWarning(exception,
                    "[{ErrorCode}] {Method} {Path}: {Message}",
                    errorCode, httpContext.Request.Method, httpContext.Request.Path, message);
                break;

            case MultiCamException hw:
                status = 502;
                errorCode = "HARDWARE_ERROR";
                message = ErrorMessageSanitizer.Sanitize(hw);
                logger.LogError(exception,
                    "[{ErrorCode}] Hardware error on {Method} {Path}",
                    errorCode, httpContext.Request.Method, httpContext.Request.Path);
                break;

            case TimeoutException:
                status = 504;
                errorCode = "TIMEOUT";
                message = "The operation timed out.";
                logger.LogError(exception,
                    "[{ErrorCode}] Timeout on {Method} {Path}",
                    errorCode, httpContext.Request.Method, httpContext.Request.Path);
                break;

            default:
                status = 500;
                errorCode = "INTERNAL_SERVER_ERROR";
                message = exception.ToString();
                logger.LogError(exception,
                    "[{ErrorCode}] Unhandled exception on {Method} {Path}",
                    errorCode, httpContext.Request.Method, httpContext.Request.Path);
                break;
        }

        httpContext.Response.StatusCode = status;
        httpContext.Response.ContentType = "application/json";
        await httpContext.Response.WriteAsJsonAsync(
            new { error = message, errorCode },
            cancellationToken);
        return true;
    }
}
