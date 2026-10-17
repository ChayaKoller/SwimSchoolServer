using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SwimSchool.Application.Common.Exceptions;
using SwimSchool.Application.Common.Models;

namespace SwimSchool.API.Validation;

public static class ValidationResponseFactory
{
    private const string InvalidValueMessage = "ערך לא תקין";

    public static IActionResult Create(ActionContext context)
    {
        var errors = context.ModelState
            .Where(entry => entry.Value is { Errors.Count: > 0 })
            .ToDictionary(
                entry => ToKey(entry.Key),
                entry => entry.Value!.Errors
                    .Select(error => error.Exception is not null
                        || string.IsNullOrEmpty(error.ErrorMessage)
                        || entry.Key.StartsWith('$')
                            ? InvalidValueMessage
                            : error.ErrorMessage)
                    .Distinct()
                    .ToArray());

        return new BadRequestObjectResult(new ErrorResponse(
            StatusCodes.Status400BadRequest,
            ErrorCodes.ValidationFailed,
            ErrorCodes.ValidationFailedMessage,
            errors,
            context.HttpContext.TraceIdentifier));
    }

    private static string ToKey(string key)
    {
        var trimmed = key.TrimStart('$', '.');
        return trimmed.Length == 0 ? "request" : JsonNamingPolicy.CamelCase.ConvertName(trimmed);
    }
}
