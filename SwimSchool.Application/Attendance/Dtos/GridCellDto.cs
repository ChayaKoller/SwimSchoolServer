namespace SwimSchool.Application.Attendance.Dtos;

public sealed record GridCellDto
{
    public int LessonId { get; init; }
    public bool? WasPresent { get; init; }
}
