namespace SwimSchool.Application.Teachers.Dtos;

public sealed record TeacherOptionDto
{
    public int Id { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
}
