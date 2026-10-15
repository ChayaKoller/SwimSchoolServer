using System.ComponentModel.DataAnnotations;

namespace SwimSchool.Application.Lessons.Dtos;

public sealed record AddLessonsRequest
{
    // Israel local time; converted to UTC in the service
    [Required, MinLength(1)]
    public List<DateTime> LessonTimes { get; init; } = new List<DateTime>();

    public bool IsMakeup { get; init; }
}
