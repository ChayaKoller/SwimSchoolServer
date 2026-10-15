using SwimSchool.Application.Lessons.Dtos;

namespace SwimSchool.Application.Lessons;

public interface ILessonService
{
    Task<List<LessonDto>> GetWeekAsync(CancellationToken ct = default);

    Task<List<LessonDto>> AddAsync(int groupId, AddLessonsRequest request, CancellationToken ct = default);

    Task<LessonDto> UpdateAsync(int lessonId, UpdateLessonRequest request, CancellationToken ct = default);

    Task DeleteAsync(int lessonId, CancellationToken ct = default);
}
