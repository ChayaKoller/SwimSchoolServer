using Microsoft.EntityFrameworkCore;
using SwimSchool.Application.Common.Models;
using SwimSchool.Application.Students;
using SwimSchool.Domain.Entities;
using SwimSchool.Infrastructure.Persistence.Extensions;

namespace SwimSchool.Infrastructure.Persistence.Repositories;

public class StudentRepository(SwimSchoolDbContext db) : IStudentRepository
{
    public async Task<PagedResult<Student>> GetAsync(int teacherId, StudentFilter filter, CancellationToken ct = default)
    {
        var query = ForTeacher(teacherId).AsNoTracking();

        if (!filter.IncludeInactive)
            query = query.Where(s => s.IsActive);

        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            var pattern = "%" + filter.Name.Trim().EscapeLikePattern() + "%";
            query = query.Where(s => EF.Functions.ILike(s.FullName, pattern, "\\"));
        }

        if (filter.GroupId is int groupId)
            query = query.Where(s => s.Enrollments.Any(e => e.IsActive && e.GroupId == groupId));

        if (filter.OnlyUnpaid)
            query = query.Where(s => s.Enrollments.Any(e => e.IsActive && e.AmountPaid < e.AmountDue));

        if (filter.AbsentMoreThan is int absentMoreThan)
            query = query.Where(s => s.Enrollments
                .Where(e => e.IsActive)
                .SelectMany(e => e.Attendances)
                .Count(a => !a.WasPresent) > absentMoreThan);

        return await query
            .Include(s => s.Enrollments.Where(e => e.IsActive))
                .ThenInclude(e => e.Attendances)
            .AsSplitQuery()
            .OrderBy(s => s.FullName)
            .ThenBy(s => s.Id)
            .ToPagedAsync(filter.Page, filter.PageSize, ct);
    }

    public Task<Student?> GetByIdAsync(int id, int teacherId, CancellationToken ct = default) =>
        ForTeacher(teacherId)
            .AsNoTracking()
            .Include(s => s.ResponsibleTeacher)
            .Include(s => s.Enrollments.Where(e => e.IsActive))
                .ThenInclude(e => e.Group)
            .FirstOrDefaultAsync(s => s.Id == id, ct);

    public Task<Student?> FindForUpdateAsync(int id, int teacherId, CancellationToken ct = default) =>
        ForTeacher(teacherId).FirstOrDefaultAsync(s => s.Id == id, ct);

    public Task<bool> IdentityNumberExistsAsync(string identityNumber, CancellationToken ct = default) =>
        db.Students.AnyAsync(s => s.IdentityNumber == identityNumber, ct);

    public void Add(Student student) => db.Students.Add(student);

    private IQueryable<Student> ForTeacher(int teacherId) =>
        db.Students.Where(s => s.ResponsibleTeacherId == teacherId);
}
