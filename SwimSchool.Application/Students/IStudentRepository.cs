using SwimSchool.Application.Common.Models;
using SwimSchool.Domain.Entities;

namespace SwimSchool.Application.Students;

public interface IStudentRepository
{
    Task<PagedResult<Student>> GetAsync(int teacherId, StudentFilter filter, CancellationToken ct = default);

    // No tracking; includes active Enrollments with their Group
    Task<Student?> GetByIdAsync(int id, int teacherId, CancellationToken ct = default);

    // Tracked
    Task<Student?> FindForUpdateAsync(int id, int teacherId, CancellationToken ct = default);

    Task<bool> IdentityNumberExistsAsync(string identityNumber, CancellationToken ct = default);

    void Add(Student student);
}
