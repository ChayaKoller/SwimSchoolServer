using System.ComponentModel.DataAnnotations;

namespace SwimSchool.Application.Attendance.Dtos;

public sealed record GridCellUpdate
{
    [Required]
    public int? LessonId { get; init; }

    [Required]
    public int? EnrollmentId { get; init; }

    [Required]
    public bool? WasPresent { get; init; }
}
