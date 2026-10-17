using System.ComponentModel.DataAnnotations;
using SwimSchool.Application.Common.Validation;

namespace SwimSchool.Application.Auth.Dtos;

public sealed class ChangePasswordRequest
{
    [Required, MaxLength(128)]
    public string CurrentPassword { get; init; } = string.Empty;

    [Required, StrongPassword]
    public string NewPassword { get; init; } = string.Empty;
}
