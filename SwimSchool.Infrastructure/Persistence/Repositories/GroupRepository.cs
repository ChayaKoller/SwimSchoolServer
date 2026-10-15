using Microsoft.EntityFrameworkCore;
using SwimSchool.Application.Common.Models;
using SwimSchool.Application.Groups;
using SwimSchool.Domain.Entities;
using SwimSchool.Infrastructure.Persistence.Extensions;

namespace SwimSchool.Infrastructure.Persistence.Repositories;

public class GroupRepository(SwimSchoolDbContext db) : IGroupRepository
{
    public async Task<PagedResult<Group>> GetAsync(int teacherId, GroupFilter filter, CancellationToken ct = default)
    {
        var query = ForTeacher(teacherId).AsNoTracking();

        if (filter.IsActive is bool isActive)
            query = query.Where(g => g.IsActive == isActive);

        if (filter.Season is { } season)
            query = query.Where(g => g.Season == season);

        if (filter.Year is int year)
            query = query.Where(g => g.Year == year);

        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            var pattern = "%" + filter.Name.Trim().EscapeLikePattern() + "%";
            query = query.Where(g => EF.Functions.ILike(g.Name, pattern, "\\"));
        }

        return await query
            .OrderByDescending(g => g.Year)
            .ThenBy(g => g.Season)
            .ThenBy(g => g.Name)
            .ToPagedAsync(filter.Page, filter.PageSize, ct);
    }

    public Task<Group?> GetByIdAsync(int id, int teacherId, CancellationToken ct = default) =>
        ForTeacher(teacherId).AsNoTracking().FirstOrDefaultAsync(g => g.Id == id, ct);

    public Task<Group?> GetDetailsAsync(int id, int teacherId, CancellationToken ct = default) =>
        ForTeacher(teacherId)
            .AsNoTracking()
            .Include(g => g.Enrollments.Where(e => e.IsActive))
                .ThenInclude(e => e.Student)
            .Include(g => g.Lessons)
            .AsSplitQuery()
            .FirstOrDefaultAsync(g => g.Id == id, ct);

    public Task<Group?> FindForUpdateAsync(int id, int teacherId, CancellationToken ct = default) =>
        ForTeacher(teacherId).FirstOrDefaultAsync(g => g.Id == id, ct);

    public Task<Group?> GetForAttendanceAsync(int groupId, int teacherId, CancellationToken ct = default) =>
        ForTeacher(teacherId)
            .Include(g => g.Lessons)
            .Include(g => g.Enrollments.Where(e => e.IsActive))
                .ThenInclude(e => e.Student)
            .Include(g => g.Enrollments.Where(e => e.IsActive))
                .ThenInclude(e => e.Attendances)
            .AsSplitQuery()
            .FirstOrDefaultAsync(g => g.Id == groupId, ct);

    public Task<Group?> FindForDeleteAsync(int id, int teacherId, CancellationToken ct = default) =>
        ForTeacher(teacherId)
            .Include(g => g.Lessons)
            .Include(g => g.Enrollments)
            .AsSplitQuery()
            .FirstOrDefaultAsync(g => g.Id == id, ct);

    public void Add(Group group) => db.Groups.Add(group);

    public void Remove(Group group)
    {
        db.Lessons.RemoveRange(group.Lessons);
        db.Groups.Remove(group);
    }

    private IQueryable<Group> ForTeacher(int teacherId) =>
        db.Groups.Where(g => g.TeacherId == teacherId);
}
