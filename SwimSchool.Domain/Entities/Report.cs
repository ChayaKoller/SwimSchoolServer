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

    public TreatmentType TreatmentType { get; set; } = TreatmentType.Group;

    // The real price for this report; may differ from the teacher's default
    public decimal PricePerTreatment { get; set; }

    // The "עבור:" line
    public string ServiceDescription { get; set; } = string.Empty;

    // Filled = private insurance; null = the student's health fund
    public string? InsuranceCompany { get; set; }

    // UTC
    public DateTime IssuedAt { get; set; }

    // Entered manually
    public List<DateOnly> LessonDates { get; set; } = new List<DateOnly>();
}
