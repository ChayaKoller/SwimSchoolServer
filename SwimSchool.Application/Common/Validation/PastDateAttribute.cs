using System.ComponentModel.DataAnnotations;

namespace SwimSchool.Application.Common.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class PastDateAttribute : ValidationAttribute
{
    public PastDateAttribute() : base("{0} must be a date in the past.")
    {
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
            return ValidationResult.Success;

        return value is DateOnly date && date < IsraelToday.From(validationContext)
            ? ValidationResult.Success
            : new ValidationResult(FormatErrorMessage(validationContext.DisplayName), MemberNames(validationContext));
    }

    private static string[]? MemberNames(ValidationContext context) =>
        context.MemberName is null ? null : [context.MemberName];
}
