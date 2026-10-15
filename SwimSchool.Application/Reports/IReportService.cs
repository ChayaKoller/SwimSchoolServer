using SwimSchool.Application.Reports.Dtos;

namespace SwimSchool.Application.Reports;

public interface IReportService
{
    Task<ReportDto> CreateAsync(CreateReportRequest request, CancellationToken ct = default);
    Task<List<ReportDto>> GetForStudentAsync(int studentId, CancellationToken ct = default);
    Task<ReportDto> GetByIdAsync(int id, CancellationToken ct = default);
}
