namespace SwimSchool.Application.ReportSettings.Dtos;

public sealed record ReportSettingsDto
{
    public string? ReportSignature { get; init; }
    public decimal DefaultGroupPrice { get; init; }
    public decimal? DefaultIndividualPrice { get; init; }
    public int DefaultTreatmentsCount { get; init; }
}
