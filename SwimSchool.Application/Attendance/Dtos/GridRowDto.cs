namespace SwimSchool.Application.Attendance.Dtos;

public sealed record GridRowDto
{
    public int EnrollmentId { get; init; }
    public int StudentId { get; init; }
    public string FullName { get; init; } = string.Empty;
    public int AbsencesCount { get; init; }

    // Cells are in the same order as Lessons, one cell per lesson (null = not marked)
    public List<GridCellDto> Cells { get; init; } = new List<GridCellDto>();
}
