using SwimSchool.Application.Common.Models;
using SwimSchool.Domain.Entities;

namespace SwimSchool.Application.Groups;

public interface IGroupRepository
{
    Task<PagedResult<Group>> GetAsync(int teacherId, GroupFilter filter, CancellationToken ct = default);

    // No tracking
    Task<Group?> GetByIdAsync(int id, int teacherId, CancellationToken ct = default);

    // No tracking; includes active Enrollments with their Student, and Lessons
    Task<Group?> GetDetailsAsync(int id, int teacherId, CancellationToken ct = default);

    // Tracked
    Task<Group?> FindForUpdateAsync(int id, int teacherId, CancellationToken ct = default);

    void Add(Group group);
}
