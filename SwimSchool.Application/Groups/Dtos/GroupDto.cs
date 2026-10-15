using SwimSchool.Domain.Enums;

namespace SwimSchool.Application.Groups.Dtos;

public sealed record GroupDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int Year { get; init; }
    public Season Season { get; init; }
    public int LessonsCount { get; init; }
    public decimal Price { get; init; }
    public bool IsActive { get; init; }
    public int MaxStudents { get; init; }
    public int EnrolledCount { get; init; }
    public int FreeSpots { get; init; }
    public uint Version { get; init; }
}
