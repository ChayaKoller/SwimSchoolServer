using System.ComponentModel.DataAnnotations;

namespace SwimSchool.Application.ReportSettings.Dtos;

public sealed record UpdateReportSettingsRequest
{
    // May contain a line break
    [MaxLength(200)]
    public string? ReportSignature { get; init; }

    [Required, Range(typeof(decimal), "1", "10000", ErrorMessage = "המחיר לטיפול חייב להיות בין 1 ל-10,000")]
    public decimal? DefaultGroupPrice { get; init; }

    [Range(typeof(decimal), "1", "10000", ErrorMessage = "המחיר לטיפול חייב להיות בין 1 ל-10,000")]
    public decimal? DefaultIndividualPrice { get; init; }

    [Required, Range(1, 100)]
    public int? DefaultTreatmentsCount { get; init; }
}
