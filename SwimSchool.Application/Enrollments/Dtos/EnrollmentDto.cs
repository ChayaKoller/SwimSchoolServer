namespace SwimSchool.Application.Enrollments.Dtos;

public sealed record EnrollmentDto
{
    public int Id { get; init; }
    public int StudentId { get; init; }
    public string StudentName { get; init; } = string.Empty;
    public int GroupId { get; init; }
    public string GroupName { get; init; } = string.Empty;
    public DateTime EnrolledAt { get; init; }
    public decimal AmountDue { get; init; }
    public decimal AmountPaid { get; init; }
    public bool IsActive { get; init; }
}
