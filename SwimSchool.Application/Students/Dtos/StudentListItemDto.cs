using SwimSchool.Domain.Enums;

namespace SwimSchool.Application.Students.Dtos;

public sealed record StudentListItemDto
{
    public int Id { get; init; }
    public string IdentityNumber { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public HealthFund HealthFund { get; init; }
    public bool IsActive { get; init; }
    public decimal AmountOwed { get; init; }
    public int AbsencesCount { get; init; }
}
