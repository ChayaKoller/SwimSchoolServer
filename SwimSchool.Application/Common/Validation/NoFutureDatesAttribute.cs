using System.ComponentModel.DataAnnotations;

namespace SwimSchool.Application.Common.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class NoFutureDatesAttribute : ValidationAttribute
{
    public NoFutureDatesAttribute() : base("{0} cannot contain future dates.")
    {
    }

    public override bool IsValid(object? value)
    {
        if (value is null)
            return true;

        if (value is not IEnumerable<DateOnly> dates)
            return false;

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        return dates.All(d => d <= today);
    }
}
