using System.ComponentModel.DataAnnotations;

namespace SwimSchool.Application.Common.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class StrongPasswordAttribute : ValidationAttribute
{
    public const string Message = "הסיסמה חייבת להכיל 8 עד 128 תווים, ולפחות אות אחת וספרה אחת";

    public StrongPasswordAttribute() : base(Message)
    {
    }

    public static bool IsStrong(string? p) =>
        p is { Length: >= 8 and <= 128 } && p.Any(char.IsLetter) && p.Any(char.IsDigit);

    // [Required] handles null
    public override bool IsValid(object? value) => value is null || IsStrong(value as string);
}
