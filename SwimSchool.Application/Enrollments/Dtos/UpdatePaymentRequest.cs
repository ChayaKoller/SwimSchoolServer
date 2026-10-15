using System.ComponentModel.DataAnnotations;

namespace SwimSchool.Application.Enrollments.Dtos;

public sealed record UpdatePaymentRequest
{
    [Required, Range(0, double.MaxValue)]
    public decimal? AmountPaid { get; init; }
}
