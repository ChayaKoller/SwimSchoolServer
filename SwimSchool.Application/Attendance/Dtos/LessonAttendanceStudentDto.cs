namespace SwimSchool.Application.Attendance.Dtos;

public sealed record LessonAttendanceStudentDto
{
    public int EnrollmentId { get; init; }
    public int StudentId { get; init; }
    public string FullName { get; init; } = string.Empty;
    public bool? WasPresent { get; init; }
}
