using SwimSchool.Domain.Enums;

namespace SwimSchool.Application.Reports.Dtos;

public sealed record ReportDto
{
    public int Id { get; init; }
    public int StudentId { get; init; }
    public string StudentName { get; init; } = string.Empty;
    public HealthFund HealthFund { get; init; }
    public DateTime IssuedAt { get; init; }
    public string CreatedByName { get; init; } = string.Empty;
    public List<DateOnly> LessonDates { get; init; } = new List<DateOnly>();
}
