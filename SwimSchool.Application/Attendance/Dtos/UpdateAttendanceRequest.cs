using System.ComponentModel.DataAnnotations;

namespace SwimSchool.Application.Attendance.Dtos;

public sealed record UpdateAttendanceRequest
{
    [Required]
    public int? LessonId { get; init; }

    [Required]
    public bool? TookPlace { get; init; }

    [MinLength(1)]
    public List<AttendanceItem> Items { get; init; } = new List<AttendanceItem>();
}
