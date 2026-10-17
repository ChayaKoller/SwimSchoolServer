using Microsoft.AspNetCore.Diagnostics;
using SwimSchool.Application.Common.Exceptions;
using SwimSchool.Application.Common.Models;
using SwimSchool.Domain.Exceptions;

namespace SwimSchool.API.Middleware;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    private const int ClientClosedRequest = 499;

    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        if (exception is OperationCanceledException && context.RequestAborted.IsCancellationRequested)
        {
            context.Response.StatusCode = ClientClosedRequest;
            return true;
        }

        var (status, code, message) = exception switch
        {
            NotFoundException e => (StatusCodes.Status404NotFound, e.Code, e.Message),
            ForbiddenException e => (StatusCodes.Status403Forbidden, e.Code, e.Message),
            ConflictException e => (StatusCodes.Status409Conflict, e.Code, e.Message),
            BusinessRuleException e => (StatusCodes.Status400BadRequest, e.Code, e.Message),
            DomainException => (StatusCodes.Status409Conflict, ErrorCodes.DomainRule, ErrorCodes.DomainRuleMessage),
            _ => (StatusCodes.Status500InternalServerError, ErrorCodes.ServerError, ErrorCodes.ServerErrorMessage)
        };

        if (status == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Unhandled exception");
        }
        else if (exception is DomainException)
        {
            logger.LogWarning(exception, "Domain rule violated: {Reason}", exception.Message);
        }
        else
        {
            logger.LogInformation("Business error {Code}", code);
        }

        context.Response.StatusCode = status;
        await context.Response.WriteAsJsonAsync(
            new ErrorResponse(status, code, message, null, context.TraceIdentifier), ct);
        return true;
    }
}
