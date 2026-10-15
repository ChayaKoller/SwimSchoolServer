using Microsoft.EntityFrameworkCore;
using SwimSchool.Application.Enrollments;
using SwimSchool.Domain.Entities;

namespace SwimSchool.Infrastructure.Persistence.Repositories;

public class EnrollmentRepository(SwimSchoolDbContext db) : IEnrollmentRepository
{
    public Task<Enrollment?> FindForUpdateAsync(int id, int teacherId, CancellationToken ct = default) =>
        ForTeacher(teacherId)
            .Include(e => e.Group)
            .FirstOrDefaultAsync(e => e.Id == id, ct);

    public Task<Enrollment?> GetByIdAsync(int id, int teacherId, CancellationToken ct = default) =>
        ForTeacher(teacherId)
            .AsNoTracking()
            .Include(e => e.Student)
            .Include(e => e.Group)
            .FirstOrDefaultAsync(e => e.Id == id, ct);

    public Task<bool> IsActiveAsync(int studentId, int groupId, CancellationToken ct = default) =>
        db.Enrollments.AnyAsync(e => e.StudentId == studentId && e.GroupId == groupId && e.IsActive, ct);

    public void Add(Enrollment enrollment) => db.Enrollments.Add(enrollment);

    private IQueryable<Enrollment> ForTeacher(int teacherId) =>
        db.Enrollments.Where(e => e.Group.TeacherId == teacherId);
}
