using System.ComponentModel.DataAnnotations;

namespace SwimSchool.Application.Common.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class PastDateAttribute : ValidationAttribute
{
    public PastDateAttribute() : base("{0} must be a date in the past.")
    {
    }

    public override bool IsValid(object? value)
    {
        if (value is null)
            return true;

        return value is DateOnly date && date < DateOnly.FromDateTime(DateTime.UtcNow);
    }
}
