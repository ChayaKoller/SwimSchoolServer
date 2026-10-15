namespace SwimSchool.Application.Attendance.Dtos;

public sealed record LessonAttendanceDto
{
    public int LessonId { get; init; }
    public string GroupName { get; init; } = string.Empty;
    public DateTime StartTime { get; init; }
    public bool TookPlace { get; init; }
    public List<LessonAttendanceStudentDto> Students { get; init; } = new List<LessonAttendanceStudentDto>();
}
