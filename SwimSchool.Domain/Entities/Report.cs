using SwimSchool.Domain.Common;
using SwimSchool.Domain.Enums;

namespace SwimSchool.Domain.Entities;

public class Report : BaseEntity
{
    public int StudentId { get; set; }
    public Student Student { get; set; } = null!;

    public int CreatedById { get; set; }
    public User CreatedBy { get; set; } = null!;

    public HealthFund HealthFund { get; set; }

    // UTC
    public DateTime IssuedAt { get; set; }

    // Entered manually
    public List<DateOnly> LessonDates { get; set; } = new List<DateOnly>();
}
