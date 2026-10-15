using System.ComponentModel.DataAnnotations;
using SwimSchool.Application.Common.Models;
using SwimSchool.Domain.Enums;

namespace SwimSchool.Application.Reports;

public class ReportFilter : PagingFilter, IValidatableObject
{
    // Student name or identity number
    public string? Search { get; set; }
    public HealthFund? HealthFund { get; set; }

    // Israel dates, inclusive
    public DateOnly? FromDate { get; set; }
    public DateOnly? ToDate { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (FromDate is not null && ToDate is not null && FromDate > ToDate)
        {
            yield return new ValidationResult(
                "תאריך ההתחלה מאוחר מתאריך הסיום",
                new[] { nameof(FromDate), nameof(ToDate) });
        }
    }
}
