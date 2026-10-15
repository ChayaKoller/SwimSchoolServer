namespace SwimSchool.Application.Lessons.Dtos;

public sealed record LessonDto
{
    public int LessonId { get; init; }
    public int GroupId { get; init; }
    public string GroupName { get; init; } = string.Empty;
    public DateTime StartTime { get; init; }
    public bool TookPlace { get; init; }
    public bool IsMakeup { get; init; }
}
