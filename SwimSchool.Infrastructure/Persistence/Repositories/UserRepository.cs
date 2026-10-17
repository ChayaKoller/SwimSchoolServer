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

    public Task<User?> FindByEmailAsync(string normalizedEmail, CancellationToken ct = default) =>
        db.Users
            .Include(u => u.Teacher)
            .SingleOrDefaultAsync(u => u.Email == normalizedEmail, ct);

    public Task<User?> GetByIdAsync(int id, CancellationToken ct = default) =>
        db.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Id == id, ct);

    public Task<UserSessionState?> GetSessionStateAsync(int id, CancellationToken ct = default) =>
        db.Users
            .AsNoTracking()
            .Where(u => u.Id == id)
            .Select(u => new UserSessionState(u.IsActive, u.TeacherId == null || u.Teacher!.IsActive, u.SecurityStamp))
            .SingleOrDefaultAsync(ct);

    public Task<bool> EmailExistsAsync(string normalizedEmail, CancellationToken ct = default) =>
        db.Users.AnyAsync(u => u.Email == normalizedEmail, ct);

    public Task<bool> IdentityNumberExistsAsync(string identityNumber, CancellationToken ct = default) =>
        db.Users.AnyAsync(u => u.IdentityNumber == identityNumber, ct);

    public async Task<IReadOnlyList<User>> GetSecretariesAsync(int teacherId, CancellationToken ct = default) =>
        await db.Users
            .AsNoTracking()
            .Where(u => u.Role == UserRole.Secretary && u.TeacherId == teacherId)
            .OrderByDescending(u => u.IsActive)
            .ThenBy(u => u.FullName)
            .ToListAsync(ct);

    public Task<User?> FindSecretaryAsync(int id, int teacherId, CancellationToken ct = default) =>
        db.Users.SingleOrDefaultAsync(
            u => u.Id == id && u.Role == UserRole.Secretary && u.TeacherId == teacherId, ct);

    public Task<User?> FindByIdForUpdateAsync(int id, CancellationToken ct = default) =>
        db.Users.SingleOrDefaultAsync(u => u.Id == id, ct);

    public void Add(User user) => db.Users.Add(user);

    private IQueryable<User> ActiveTeachers() =>
        db.Users.Where(u => u.Role == UserRole.Teacher && u.IsActive);
}
