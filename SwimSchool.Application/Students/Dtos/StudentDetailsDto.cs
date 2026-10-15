using SwimSchool.Domain.Enums;

namespace SwimSchool.Application.Students.Dtos;

public sealed record StudentDetailsDto
{
    public int Id { get; init; }
    public string IdentityNumber { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string? Email { get; init; }
    public DateOnly BirthDate { get; init; }
    public HealthFund HealthFund { get; init; }
    public string? PlacementRequest { get; init; }
    public bool IsActive { get; init; }
    public string ResponsibleTeacherName { get; init; } = string.Empty;
    public List<StudentGroupDto> Groups { get; init; } = new List<StudentGroupDto>();
}
