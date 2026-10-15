using System.ComponentModel.DataAnnotations;
using SwimSchool.Application.Common.Validation;

namespace SwimSchool.Application.Reports.Dtos;

// Any health fund; taken from the student
public sealed record CreateReportRequest
{
    [Required, Range(1, int.MaxValue)]
    public int? StudentId { get; init; }

    [MinLength(1), NoDuplicates, NoFutureDates]
    public List<DateOnly> LessonDates { get; init; } = new List<DateOnly>();
}
