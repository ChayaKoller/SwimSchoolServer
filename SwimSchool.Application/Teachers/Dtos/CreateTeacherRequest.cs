using System.ComponentModel.DataAnnotations;
using SwimSchool.Application.Common.Validation;

namespace SwimSchool.Application.Teachers.Dtos;

public sealed class CreateTeacherRequest
{
    [Required, RegularExpression(@"^\d{9}$")]
    public string IdentityNumber { get; init; } = string.Empty;

    [Required, MaxLength(100)]
    public string FullName { get; init; } = string.Empty;

    [Required, EmailAddress, MaxLength(254)]
    public string Email { get; init; } = string.Empty;

    [Required, MaxLength(20), Phone]
    public string Phone { get; init; } = string.Empty;

    [Required, StrongPassword]
    public string Password { get; init; } = string.Empty;
}
