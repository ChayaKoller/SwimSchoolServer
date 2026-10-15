using System.ComponentModel.DataAnnotations;

namespace SwimSchool.Application.Enrollments.Dtos;

public sealed record EnrollRequest
{
    [Required, Range(1, int.MaxValue)]
    public int? StudentId { get; init; }

    [Required, Range(1, int.MaxValue)]
    public int? GroupId { get; init; }

    // null = group price
    [Range(0, double.MaxValue)]
    public decimal? AmountDue { get; init; }
}
