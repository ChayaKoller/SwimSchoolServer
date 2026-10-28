using SwimSchool.Domain.Enums;

namespace SwimSchool.Application.Students.Dtos;

public sealed record StudentGroupDto
{
    public int EnrollmentId { get; init; }
    public int GroupId { get; init; }
    public string GroupName { get; init; } = string.Empty;
    public Season Season { get; init; }
    public int Year { get; init; }
    public decimal AmountDue { get; init; }
    public decimal AmountPaid { get; init; }
    public int AbsencesCount { get; init; }
}
