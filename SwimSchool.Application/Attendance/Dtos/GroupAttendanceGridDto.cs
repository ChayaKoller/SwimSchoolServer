namespace SwimSchool.Application.Attendance.Dtos;

public sealed record GroupAttendanceGridDto
{
    public int GroupId { get; init; }
    public string GroupName { get; init; } = string.Empty;

    // Ordered by StartTime
    public List<GridLessonDto> Lessons { get; init; } = new List<GridLessonDto>();

    public List<GridRowDto> Rows { get; init; } = new List<GridRowDto>();
}
