using SwimSchool.Domain.Entities;

namespace SwimSchool.Application.Teachers;

public interface IUserRepository
{
    // No tracking; ordered by FullName
    Task<List<User>> GetActiveTeachersAsync(CancellationToken ct = default);

    Task<bool> IsActiveTeacherAsync(int userId, CancellationToken ct = default);
}
