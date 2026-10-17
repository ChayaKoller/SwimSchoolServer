using SwimSchool.Domain.Entities;

namespace SwimSchool.Application.Teachers;

public interface IUserRepository
{
    // No tracking; ordered by FullName
    Task<List<User>> GetActiveTeachersAsync(CancellationToken ct = default);

    Task<bool> IsActiveTeacherAsync(int userId, CancellationToken ct = default);

    // The only user queries without a teacher filter (the user is not signed in yet)
    // Tracked; includes Teacher
    Task<User?> FindByEmailAsync(string normalizedEmail, CancellationToken ct = default);

    // No tracking
    Task<User?> GetByIdAsync(int id, CancellationToken ct = default);

    // Projection only
    Task<UserSessionState?> GetSessionStateAsync(int id, CancellationToken ct = default);

    // All users, no teacher filter: email is the login name
    Task<bool> EmailExistsAsync(string normalizedEmail, CancellationToken ct = default);

    // All users
    Task<bool> IdentityNumberExistsAsync(string identityNumber, CancellationToken ct = default);

    // No tracking; active first, then by FullName
    Task<IReadOnlyList<User>> GetSecretariesAsync(int teacherId, CancellationToken ct = default);

    // Tracked
    Task<User?> FindSecretaryAsync(int id, int teacherId, CancellationToken ct = default);

    // Tracked
    Task<User?> FindByIdForUpdateAsync(int id, CancellationToken ct = default);

    void Add(User user);
}
