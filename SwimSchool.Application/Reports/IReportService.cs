using SwimSchool.Application.Common.Models;
using SwimSchool.Application.Reports.Dtos;

namespace SwimSchool.Application.Reports;

public interface IReportService
{
    Task<PagedResult<ReportDto>> GetAsync(ReportFilter filter, CancellationToken ct = default);
    Task<ReportDto> CreateAsync(CreateReportRequest request, CancellationToken ct = default);
    Task<List<ReportDto>> GetForStudentAsync(int studentId, CancellationToken ct = default);
    Task<ReportDto> GetByIdAsync(int id, CancellationToken ct = default);
}
