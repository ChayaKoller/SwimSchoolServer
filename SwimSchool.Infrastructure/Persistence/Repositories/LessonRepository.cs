using Microsoft.EntityFrameworkCore;
using SwimSchool.Application.Lessons;
using SwimSchool.Domain.Entities;

namespace SwimSchool.Infrastructure.Persistence.Repositories;

public class LessonRepository(SwimSchoolDbContext db) : ILessonRepository
{
    public Task<List<Lesson>> GetForTeacherAsync(int teacherId, DateTime fromUtc, DateTime toUtc, CancellationToken ct = default) =>
        ForTeacher(teacherId)
            .AsNoTracking()
            .Include(l => l.Group)
            .Where(l => l.StartTime >= fromUtc && l.StartTime < toUtc)
            .OrderBy(l => l.StartTime)
            .ToListAsync(ct);

    public Task<Lesson?> FindForUpdateAsync(int lessonId, int teacherId, CancellationToken ct = default) =>
        ForTeacher(teacherId)
            .Include(l => l.Group)
            .FirstOrDefaultAsync(l => l.Id == lessonId, ct);

    public Task<bool> HasAttendanceAsync(int lessonId, CancellationToken ct = default) =>
        db.Attendances.AnyAsync(a => a.LessonId == lessonId, ct);

    public void AddRange(IEnumerable<Lesson> lessons) => db.Lessons.AddRange(lessons);

    public void Remove(Lesson lesson) => db.Lessons.Remove(lesson);

    private IQueryable<Lesson> ForTeacher(int teacherId) =>
        db.Lessons.Where(l => l.Group.TeacherId == teacherId);
}
