using SwimSchool.Domain.Common;
using SwimSchool.Domain.Enums;
using SwimSchool.Domain.Exceptions;

namespace SwimSchool.Domain.Entities;

public class Student : BaseEntity
{
    public string IdentityNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public DateOnly BirthDate { get; set; }
    public HealthFund HealthFund { get; set; }
    public string? PlacementRequest { get; set; }
    public bool IsActive { get; set; } = true;

    public int ResponsibleTeacherId { get; set; }
    public User ResponsibleTeacher { get; set; } = null!;

    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public ICollection<Report> Reports { get; set; } = new List<Report>();

    public void TransferTo(int newTeacherId)
    {
        if (newTeacherId == ResponsibleTeacherId)
            throw new DomainException("The student is already assigned to this teacher.");

        ResponsibleTeacherId = newTeacherId;
    }
}
