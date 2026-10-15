using Microsoft.EntityFrameworkCore;
using SwimSchool.Application.Common.Models;
using SwimSchool.Application.Reports;
using SwimSchool.Domain.Entities;
using SwimSchool.Infrastructure.Persistence.Extensions;

namespace SwimSchool.Infrastructure.Persistence.Repositories;

public class ReportRepository(SwimSchoolDbContext db) : IReportRepository
{
    public async Task<PagedResult<Report>> GetAsync(
        int teacherId, ReportFilter filter, DateTime? fromUtc, DateTime? toUtc, CancellationToken ct = default)
    {
        var query = ForTeacher(teacherId).AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.Trim();
            var pattern = "%" + search.EscapeLikePattern() + "%";
            query = query.Where(r =>
                EF.Functions.ILike(r.Student.FullName, pattern, "\\")
                || r.Student.IdentityNumber.StartsWith(search));
        }

        if (filter.HealthFund is { } healthFund)
            query = query.Where(r => r.HealthFund == healthFund);

        if (fromUtc is DateTime from)
            query = query.Where(r => r.IssuedAt >= from);

        if (toUtc is DateTime to)
            query = query.Where(r => r.IssuedAt < to);

        return await query
            .Include(r => r.Student)
            .Include(r => r.CreatedBy)
            .OrderByDescending(r => r.IssuedAt)
            .ThenByDescending(r => r.Id)
            .ToPagedAsync(filter.Page, filter.PageSize, ct);
    }

    public Task<List<Report>> GetForStudentAsync(int studentId, int teacherId, CancellationToken ct = default) =>
        ForTeacher(teacherId)
            .AsNoTracking()
            .Include(r => r.Student)
            .Include(r => r.CreatedBy)
            .Where(r => r.StudentId == studentId)
            .OrderByDescending(r => r.IssuedAt)
            .ThenByDescending(r => r.Id)
            .ToListAsync(ct);

    public Task<Report?> GetByIdAsync(int id, int teacherId, CancellationToken ct = default) =>
        ForTeacher(teacherId)
            .AsNoTracking()
            .Include(r => r.Student)
            .Include(r => r.CreatedBy)
            .FirstOrDefaultAsync(r => r.Id == id, ct);

    public void Add(Report report) => db.Reports.Add(report);

    private IQueryable<Report> ForTeacher(int teacherId) =>
        db.Reports.Where(r => r.Student.ResponsibleTeacherId == teacherId);
}
