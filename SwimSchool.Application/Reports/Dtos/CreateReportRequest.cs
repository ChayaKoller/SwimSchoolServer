using System.ComponentModel.DataAnnotations;

namespace SwimSchool.Application.Reports.Dtos;

// Any health fund; taken from the student
public sealed record CreateReportRequest : ReportDetailsRequest
{
    [Required, Range(1, int.MaxValue)]
    public int? StudentId { get; init; }
}
