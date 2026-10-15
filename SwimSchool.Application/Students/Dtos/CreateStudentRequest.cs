using System.ComponentModel.DataAnnotations;
using SwimSchool.Application.Common.Validation;
using SwimSchool.Domain.Enums;

namespace SwimSchool.Application.Students.Dtos;

public sealed record CreateStudentRequest
{
    [Required, RegularExpression(@"^\d{9}$")]
    public string IdentityNumber { get; init; } = string.Empty;

    [Required, MaxLength(100)]
    public string FullName { get; init; } = string.Empty;

    [Required, MaxLength(20), Phone]
    public string Phone { get; init; } = string.Empty;

    [EmailAddress, MaxLength(254)]
    public string? Email { get; init; }

    [Required, PastDate]
    public DateOnly? BirthDate { get; init; }

    [Required, EnumDataType(typeof(HealthFund))]
    public HealthFund? HealthFund { get; init; }

    [MaxLength(500)]
    public string? PlacementRequest { get; init; }
}
