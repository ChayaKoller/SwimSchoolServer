using System.ComponentModel.DataAnnotations;

namespace SwimSchool.Application.Attendance.Dtos;

public sealed record AttendanceItem
{
    [Required]
    public int? EnrollmentId { get; init; }

    [Required]
    public bool? WasPresent { get; init; }
}
