using SwimSchool.Application.Common.Exceptions;
using SwimSchool.Application.Common.Interfaces;
using SwimSchool.Application.Groups;
using SwimSchool.Application.Lessons.Dtos;
using SwimSchool.Domain.Entities;

namespace SwimSchool.Application.Lessons;

public class LessonService(
    ILessonRepository lessons,
    IGroupRepository groups,
    IUnitOfWork unitOfWork,
    ICurrentUser me,
    IDateTimeProvider clock) : ILessonService
{
    public async Task<List<LessonDto>> GetWeekAsync(CancellationToken ct = default)
    {
        var today = clock.TodayIsrael;
        var fromUtc = clock.IsraelToUtc(today.ToDateTime(TimeOnly.MinValue));
        var toUtc = clock.IsraelToUtc(today.AddDays(7).ToDateTime(TimeOnly.MinValue));

        var week = await lessons.GetForTeacherAsync(me.OwningTeacherId, fromUtc, toUtc, ct);

        return LessonMapper.ToDtos(week);
    }

    public async Task<List<LessonDto>> AddAsync(int groupId, AddLessonsRequest request, CancellationToken ct = default)
    {
        var group = await groups.FindForUpdateAsync(groupId, me.OwningTeacherId, ct)
            ?? throw new NotFoundException(ErrorCodes.GroupNotFound, ErrorCodes.GroupNotFoundMessage);

        var newLessons = request.LessonTimes
            .OrderBy(t => t)
            .Select(t => new Lesson
            {
                Group = group,
                StartTime = clock.IsraelToUtc(t),
                IsMakeup = request.IsMakeup
            })
            .ToList();

        lessons.AddRange(newLessons);
        group.LessonsCount += newLessons.Count;
        await unitOfWork.SaveChangesAsync(ct);

        return LessonMapper.ToDtos(newLessons);
    }

    public async Task<LessonDto> UpdateAsync(int lessonId, UpdateLessonRequest request, CancellationToken ct = default)
    {
        var lesson = await FindEditableAsync(lessonId, ct);

        lesson.StartTime = clock.IsraelToUtc(request.StartTime!.Value);
        await unitOfWork.SaveChangesAsync(ct);

        return LessonMapper.ToDto(lesson);
    }

    public async Task DeleteAsync(int lessonId, CancellationToken ct = default)
    {
        var lesson = await FindEditableAsync(lessonId, ct);

        lessons.Remove(lesson);
        lesson.Group.LessonsCount--;
        await unitOfWork.SaveChangesAsync(ct);
    }

    private async Task<Lesson> FindEditableAsync(int lessonId, CancellationToken ct)
    {
        var lesson = await lessons.FindForUpdateAsync(lessonId, me.OwningTeacherId, ct)
            ?? throw new NotFoundException(ErrorCodes.LessonNotFound, ErrorCodes.LessonNotFoundMessage);

        if (await lessons.HasAttendanceAsync(lessonId, ct))
            throw new BusinessRuleException(ErrorCodes.LessonHasAttendance, ErrorCodes.LessonHasAttendanceMessage);

        return lesson;
    }
}
