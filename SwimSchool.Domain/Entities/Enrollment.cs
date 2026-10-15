using SwimSchool.Domain.Common;

namespace SwimSchool.Domain.Entities;

// Student <-> Group many-to-many with payload
public class Enrollment : BaseEntity
{
    public int StudentId { get; set; }
    public Student Student { get; set; } = null!;

    public int GroupId { get; set; }
    public Group Group { get; set; } = null!;

    // UTC
    public DateTime EnrolledAt { get; set; }
    public decimal AmountDue { get; set; }
    public decimal AmountPaid { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
}
