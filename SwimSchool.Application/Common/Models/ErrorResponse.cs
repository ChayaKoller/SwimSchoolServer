using System.Text.Json.Serialization;

namespace SwimSchool.Application.Common.Models;

public sealed record ErrorResponse(
    int Status,
    string Code,
    string Message,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] IDictionary<string, string[]>? Errors,
    string TraceId);
