namespace SwimSchool.Application.Groups.Dtos;

public sealed record GroupStudentDto
{
    public int Id { get; init; }
    public string FullName { get; init; } = string.Empty;
    public decimal AmountOwed { get; init; }
}
