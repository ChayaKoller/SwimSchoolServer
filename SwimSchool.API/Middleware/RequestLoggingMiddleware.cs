using System.Diagnostics;

namespace SwimSchool.API.Middleware;

public sealed class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            await next(context);
        }
        finally
        {
            stopwatch.Stop();
            var statusCode = context.Response.StatusCode;
            var level = context.Request.Path.StartsWithSegments("/health") ? LogLevel.Debug
                : statusCode >= 500 ? LogLevel.Error
                : statusCode >= 400 ? LogLevel.Warning
                : LogLevel.Information;

            logger.Log(level, "HTTP {Method} {Path} -> {StatusCode} in {ElapsedMs} ms",
                context.Request.Method, context.Request.Path.Value, statusCode, stopwatch.ElapsedMilliseconds);
        }
    }
}
