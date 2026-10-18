using SwimSchool.Application.Common.Models;
using SwimSchool.Domain.Entities;

namespace SwimSchool.Application.Reports;

public interface IReportRepository
{
    // No tracking; includes Student and CreatedBy, newest first. toUtc is exclusive
    Task<PagedResult<Report>> GetAsync(
        int teacherId, ReportFilter filter, DateTime? fromUtc, DateTime? toUtc, CancellationToken ct = default);

    // No tracking; includes Student and CreatedBy, newest first
    Task<List<Report>> GetForStudentAsync(int studentId, int teacherId, CancellationToken ct = default);

    // No tracking; includes Student and CreatedBy
    Task<Report?> GetByIdAsync(int id, int teacherId, CancellationToken ct = default);

    // Tracked; same ownership rule as GetByIdAsync
    Task<Report?> FindForUpdateAsync(int id, int teacherId, CancellationToken ct = default);

    // No tracking; includes Student and CreatedBy with its Teacher. Same ownership rule as GetByIdAsync
    Task<Report?> GetForPdfAsync(int id, int teacherId, CancellationToken ct = default);

    // Start times (UTC) of lessons where the student was marked present, only in this teacher's groups. toUtcExclusive is exclusive
    Task<IReadOnlyList<DateTime>> GetAttendedLessonTimesAsync(
        int studentId, int teacherId, int? groupId, DateTime? fromUtc, DateTime? toUtcExclusive, CancellationToken ct = default);

    void Add(Report report);
}
