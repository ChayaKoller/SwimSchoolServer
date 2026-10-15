using System.ComponentModel.DataAnnotations;

namespace SwimSchool.Application.Groups.Dtos;

public sealed record UpdateGroupRequest
{
    [Required, MaxLength(100)]
    public string Name { get; init; } = string.Empty;

    [Required, Range(1, 200)]
    public int? LessonsCount { get; init; }

    [Required, Range(0, double.MaxValue)]
    public decimal? Price { get; init; }

    [Required, Range(1, 500)]
    public int? MaxStudents { get; init; }

    [Required]
    public bool? IsActive { get; init; }

    [Required]
    public uint? Version { get; init; }
}
