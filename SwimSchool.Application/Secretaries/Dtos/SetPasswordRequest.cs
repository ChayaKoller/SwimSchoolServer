using System.ComponentModel.DataAnnotations;
using SwimSchool.Application.Common.Validation;

namespace SwimSchool.Application.Secretaries.Dtos;

public sealed class SetPasswordRequest
{
    [Required, StrongPassword]
    public string NewPassword { get; init; } = string.Empty;
}
