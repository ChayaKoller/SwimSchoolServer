using System.ComponentModel.DataAnnotations;

namespace SwimSchool.Application.Common.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class NoFutureDatesAttribute : ValidationAttribute
{
    public NoFutureDatesAttribute() : base("{0} cannot contain future dates.")
    {
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
            return ValidationResult.Success;

        var today = IsraelToday.From(validationContext);

        return value is IEnumerable<DateOnly> dates && dates.All(d => d <= today)
            ? ValidationResult.Success
            : new ValidationResult(FormatErrorMessage(validationContext.DisplayName), MemberNames(validationContext));
    }

    private static string[]? MemberNames(ValidationContext context) =>
        context.MemberName is null ? null : [context.MemberName];
}
