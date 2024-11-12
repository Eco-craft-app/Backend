using Microsoft.AspNetCore.Diagnostics;

namespace HackHeroes.Recycle.ExceptionHandlers;

public class RecycleExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {

        (int statusCode, string? errorMessage) = exception switch
        {
            ArgumentException argumentException => (400, argumentException.Message),
            InvalidOperationException invalidOperationException => (500, invalidOperationException.Message),
            KeyNotFoundException keyNotFoundException => (404, keyNotFoundException.Message),
            IOException ioException => (500, ioException.Message),
            UnauthorizedAccessException unauthorizedAccessException => (403, unauthorizedAccessException.Message),
            TimeoutException timeoutException => (408, timeoutException.Message),
            _ => (500, "Something went wrong")
        };

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsync(errorMessage!);

        return true;

    }
}
