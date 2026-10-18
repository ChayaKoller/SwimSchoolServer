using System.ComponentModel.DataAnnotations;

namespace SwimSchool.Application.Reports;

public class AttendedDatesFilter : IValidatableObject
{
    [Range(1, int.MaxValue)]
    public int? GroupId { get; set; }

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
