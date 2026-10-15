using SwimSchool.Domain.Entities;

namespace SwimSchool.Application.Enrollments;

public interface IEnrollmentRepository
{
    // Tracked; includes Group
    Task<Enrollment?> FindForUpdateAsync(int id, int teacherId, CancellationToken ct = default);

    // No tracking; includes Student and Group
    Task<Enrollment?> GetByIdAsync(int id, int teacherId, CancellationToken ct = default);

    // No teacherId: called only after ownership of both student and group was checked
    Task<bool> IsActiveAsync(int studentId, int groupId, CancellationToken ct = default);

    void Add(Enrollment enrollment);
}
