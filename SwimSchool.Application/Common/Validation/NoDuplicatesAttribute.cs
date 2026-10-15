using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace SwimSchool.Application.Common.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class NoDuplicatesAttribute : ValidationAttribute
{
    public NoDuplicatesAttribute() : base("{0} cannot contain duplicate dates.")
    {
    }

    public override bool IsValid(object? value)
    {
        if (value is null)
            return true;

        if (value is not IEnumerable items)
            return false;

        var list = items.Cast<object>().ToList();
        return list.Distinct().Count() == list.Count;
    }
}
