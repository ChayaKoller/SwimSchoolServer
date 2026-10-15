namespace SwimSchool.Application.Attendance.Dtos;

public sealed record GridLessonDto
{
    public int LessonId { get; init; }
    public DateTime StartTime { get; init; }
    public bool TookPlace { get; init; }
    public bool IsMakeup { get; init; }
}
