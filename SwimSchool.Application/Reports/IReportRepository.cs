using SwimSchool.Domain.Entities;

namespace SwimSchool.Application.Reports;

public interface IReportRepository
{
    // No tracking; includes Student and CreatedBy, newest first
    Task<List<Report>> GetForStudentAsync(int studentId, int teacherId, CancellationToken ct = default);

    // No tracking; includes Student and CreatedBy
    Task<Report?> GetByIdAsync(int id, int teacherId, CancellationToken ct = default);

    void Add(Report report);
}
