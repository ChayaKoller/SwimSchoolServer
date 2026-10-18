using SwimSchool.Application.Common.Models;
using SwimSchool.Application.Reports.Dtos;
using SwimSchool.Application.Reports.Pdf;

namespace SwimSchool.Application.Reports;

public interface IReportService
{
    Task<PagedResult<ReportDto>> GetAsync(ReportFilter filter, CancellationToken ct = default);
    Task<ReportDto> CreateAsync(CreateReportRequest request, CancellationToken ct = default);
    Task<ReportDto> UpdateAsync(int id, UpdateReportRequest request, CancellationToken ct = default);
    Task<List<ReportDto>> GetForStudentAsync(int studentId, CancellationToken ct = default);
    Task<ReportDto> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<DateOnly>> GetAttendedDatesAsync(int studentId, AttendedDatesFilter filter, CancellationToken ct = default);
    Task<ReportPdfFile> GetPdfAsync(int id, CancellationToken ct = default);
}
