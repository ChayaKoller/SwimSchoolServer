using Microsoft.EntityFrameworkCore;
using SwimSchool.Application.Reports;
using SwimSchool.Domain.Entities;

namespace SwimSchool.Infrastructure.Persistence.Repositories;

public class ReportRepository(SwimSchoolDbContext db) : IReportRepository
{
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
