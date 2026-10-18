using System.ComponentModel.DataAnnotations;
using SwimSchool.Application.Common.Validation;
using SwimSchool.Domain.Enums;

namespace SwimSchool.Application.Reports.Dtos;

// Shared by create and update
public abstract record ReportDetailsRequest
{
    [Required, EnumDataType(typeof(TreatmentType))]
    public TreatmentType? TreatmentType { get; init; }

    [Required, Range(typeof(decimal), "1", "10000", ErrorMessage = "המחיר לטיפול חייב להיות בין 1 ל-10,000")]
    public decimal? PricePerTreatment { get; init; }

    [Required, MaxLength(100)]
    public string ServiceDescription { get; init; } = string.Empty;

    // Empty = the student's health fund
    [MaxLength(100)]
    public string? InsuranceCompany { get; init; }

    [MinLength(1), MaxLength(100), NoDuplicates, NoFutureDates]
    public List<DateOnly> LessonDates { get; init; } = new List<DateOnly>();
}
