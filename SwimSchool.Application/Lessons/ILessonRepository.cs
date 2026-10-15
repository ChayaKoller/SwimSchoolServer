using SwimSchool.Domain.Entities;

namespace SwimSchool.Application.Lessons;

public interface ILessonRepository
{
    // No tracking; includes Group, ordered by StartTime
    Task<List<Lesson>> GetForTeacherAsync(int teacherId, DateTime fromUtc, DateTime toUtc, CancellationToken ct = default);

    // Tracked; includes Group
    Task<Lesson?> FindForUpdateAsync(int lessonId, int teacherId, CancellationToken ct = default);

    Task<bool> HasAttendanceAsync(int lessonId, CancellationToken ct = default);

    void AddRange(IEnumerable<Lesson> lessons);

    void Remove(Lesson lesson);
}
