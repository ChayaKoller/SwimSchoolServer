using SwimSchool.Domain.Enums;

namespace SwimSchool.Application.Reports.Dtos;

public sealed record ReportDto
{
    public int Id { get; init; }
    public int StudentId { get; init; }
    public string StudentName { get; init; } = string.Empty;
    public HealthFund HealthFund { get; init; }
    public string? InsuranceCompany { get; init; }
    public TreatmentType TreatmentType { get; init; }
    public decimal PricePerTreatment { get; init; }

    // Computed, not stored
    public decimal TotalAmount { get; init; }

    public string ServiceDescription { get; init; } = string.Empty;
    public DateTime IssuedAt { get; init; }
    public string CreatedByName { get; init; } = string.Empty;
    public List<DateOnly> LessonDates { get; init; } = new List<DateOnly>();
}
