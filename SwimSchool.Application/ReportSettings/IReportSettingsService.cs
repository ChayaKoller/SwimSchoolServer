using SwimSchool.Application.ReportSettings.Dtos;

namespace SwimSchool.Application.ReportSettings;

public interface IReportSettingsService
{
    Task<ReportSettingsDto> GetAsync(CancellationToken ct = default);
    Task<ReportSettingsDto> UpdateAsync(UpdateReportSettingsRequest request, CancellationToken ct = default);
}
