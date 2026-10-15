using System.ComponentModel.DataAnnotations;

namespace SwimSchool.Application.Lessons.Dtos;

public sealed record UpdateLessonRequest
{
    // Israel local time; converted to UTC in the service
    [Required]
    public DateTime? StartTime { get; init; }
}
