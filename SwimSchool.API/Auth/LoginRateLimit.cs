using System.Threading.RateLimiting;
using SwimSchool.Application.Common.Exceptions;
using SwimSchool.Application.Common.Models;

namespace SwimSchool.API.Auth;

public static class LoginRateLimit
{
    public const string PolicyName = "login";

    public static IServiceCollection AddLoginRateLimit(this IServiceCollection services) =>
        services.AddRateLimiter(o =>
        {
            o.AddPolicy(PolicyName, http => RateLimitPartition.GetFixedWindowLimiter(
                http.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 5,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 0
                }));

            o.OnRejected = (context, ct) =>
            {
                var http = context.HttpContext;
                http.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                return new ValueTask(http.Response.WriteAsJsonAsync(
                    new ErrorResponse(StatusCodes.Status429TooManyRequests, ErrorCodes.TooManyAttempts,
                        ErrorCodes.TooManyAttemptsMessage, null, http.TraceIdentifier), ct));
            };
        });
}
