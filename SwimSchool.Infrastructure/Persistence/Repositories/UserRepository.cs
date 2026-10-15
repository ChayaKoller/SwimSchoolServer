using Microsoft.EntityFrameworkCore;
using SwimSchool.Application.Teachers;
using SwimSchool.Domain.Entities;
using SwimSchool.Domain.Enums;

namespace SwimSchool.Infrastructure.Persistence.Repositories;

public class UserRepository(SwimSchoolDbContext db) : IUserRepository
{
    public Task<List<User>> GetActiveTeachersAsync(CancellationToken ct = default) =>
        ActiveTeachers()
            .AsNoTracking()
            .OrderBy(u => u.FullName)
            .ToListAsync(ct);

    public Task<bool> IsActiveTeacherAsync(int userId, CancellationToken ct = default) =>
        ActiveTeachers().AnyAsync(u => u.Id == userId, ct);

    private IQueryable<User> ActiveTeachers() =>
        db.Users.Where(u => u.Role == UserRole.Teacher && u.IsActive);
}
