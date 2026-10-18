using SwimSchool.Application.Attendance.Dtos;
using SwimSchool.Application.Common.Exceptions;
using SwimSchool.Application.Common.Interfaces;
using SwimSchool.Application.Groups;
using SwimSchool.Application.Lessons;
using SwimSchool.Domain.Entities;

namespace SwimSchool.Application.Attendance;

// Teacher only
public class AttendanceService(
    ILessonRepository lessons,
    IGroupRepository groups,
    IUnitOfWork unitOfWork,
    ICurrentUser me,
    IDateTimeProvider clock) : IAttendanceService
{
    public async Task<LessonAttendanceDto> GetLessonAsync(int lessonId, CancellationToken ct = default)
    {
        EnsureTeacher();

        var lesson = await FindLessonAsync(lessonId, ct);

        // Loads active enrollments and attendances into the same tracked graph as the lesson
        _ = await groups.GetForAttendanceAsync(lesson.GroupId, me.OwningTeacherId, ct)
            ?? throw new NotFoundException(ErrorCodes.LessonNotFound, ErrorCodes.LessonNotFoundMessage);

        return AttendanceMapper.ToDto(lesson);
    }

    public async Task UpdateLessonAsync(UpdateAttendanceRequest request, CancellationToken ct = default)
    {
        EnsureTeacher();

        var lesson = await FindLessonAsync(request.LessonId!.Value, ct);
        EnsureNotFuture(lesson);

        var group = await groups.GetForAttendanceAsync(lesson.GroupId, me.OwningTeacherId, ct)
            ?? throw new NotFoundException(ErrorCodes.LessonNotFound, ErrorCodes.LessonNotFoundMessage);

        var activeEnrollments = group.Enrollments.ToDictionary(e => e.Id);

        foreach (var item in request.Items)
        {
            var enrollment = FindActiveEnrollment(activeEnrollments, item.EnrollmentId!.Value);
            UpsertAttendance(enrollment, lesson.Id, item.WasPresent!.Value);
        }

        lesson.TookPlace = request.TookPlace!.Value;
        await unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<GroupAttendanceGridDto> GetGridAsync(int groupId, CancellationToken ct = default)
    {
        EnsureTeacher();

        var group = await FindGroupAsync(groupId, ct);

        var orderedLessons = group.Lessons
            .OrderBy(l => l.StartTime)
            .ThenBy(l => l.Id)
            .ToList();

        var rows = group.Enrollments
            .OrderBy(e => e.Student.FullName)
            .ThenBy(e => e.Id)
            .Select(e =>
            {
                var byLesson = e.Attendances.ToDictionary(a => a.LessonId, a => a.WasPresent);

                return new GridRowDto
                {
                    EnrollmentId = e.Id,
                    StudentId = e.StudentId,
                    FullName = e.Student.FullName,
                    AbsencesCount = e.Attendances.Count(a => !a.WasPresent),
                    Cells = orderedLessons
                        .Select(l => new GridCellDto
                        {
                            LessonId = l.Id,
                            WasPresent = byLesson.TryGetValue(l.Id, out var wasPresent) ? wasPresent : null
                        })
                        .ToList()
                };
            })
            .ToList();

        return new GroupAttendanceGridDto
        {
            GroupId = group.Id,
            GroupName = group.Name,
            Lessons = orderedLessons
                .Select(l => new GridLessonDto
                {
                    LessonId = l.Id,
                    StartTime = l.StartTime,
                    TookPlace = l.TookPlace,
                    IsMakeup = l.IsMakeup
                })
                .ToList(),
            Rows = rows
        };
    }

    public async Task UpdateGridAsync(int groupId, UpdateGroupAttendanceRequest request, CancellationToken ct = default)
    {
        EnsureTeacher();

        var group = await FindGroupAsync(groupId, ct);

        var groupLessons = group.Lessons.ToDictionary(l => l.Id);
        var activeEnrollments = group.Enrollments.ToDictionary(e => e.Id);

        foreach (var cell in request.Items)
        {
            if (!groupLessons.TryGetValue(cell.LessonId!.Value, out var lesson))
                throw new NotFoundException(ErrorCodes.LessonNotFound, ErrorCodes.LessonNotFoundMessage);

            EnsureNotFuture(lesson);

            var enrollment = FindActiveEnrollment(activeEnrollments, cell.EnrollmentId!.Value);
            UpsertAttendance(enrollment, lesson.Id, cell.WasPresent!.Value);
            lesson.TookPlace = true;
        }

        await unitOfWork.SaveChangesAsync(ct);
    }

    // Updating already-marked attendance is always allowed
    private static void UpsertAttendance(Enrollment enrollment, int lessonId, bool wasPresent)
    {
        var existing = enrollment.Attendances.FirstOrDefault(a => a.LessonId == lessonId);

        if (existing is null)
            enrollment.Attendances.Add(new() { LessonId = lessonId, WasPresent = wasPresent });
        else
            existing.WasPresent = wasPresent;
    }

    private static Enrollment FindActiveEnrollment(Dictionary<int, Enrollment> activeEnrollments, int enrollmentId) =>
        activeEnrollments.TryGetValue(enrollmentId, out var enrollment)
            ? enrollment
            : throw new BusinessRuleException(ErrorCodes.StudentNotInGroup, ErrorCodes.StudentNotInGroupMessage);

    private void EnsureNotFuture(Lesson lesson)
    {
        if (lesson.StartTime > clock.UtcNow)
            throw new BusinessRuleException(ErrorCodes.FutureLesson, ErrorCodes.FutureLessonMessage);
    }

    private void EnsureTeacher()
    {
        if (!me.IsTeacher)
            throw new ForbiddenException(ErrorCodes.Forbidden, ErrorCodes.ForbiddenMessage);
    }

    private async Task<Lesson> FindLessonAsync(int lessonId, CancellationToken ct) =>
        await lessons.FindForUpdateAsync(lessonId, me.OwningTeacherId, ct)
            ?? throw new NotFoundException(ErrorCodes.LessonNotFound, ErrorCodes.LessonNotFoundMessage);

    private async Task<Group> FindGroupAsync(int groupId, CancellationToken ct) =>
        await groups.GetForAttendanceAsync(groupId, me.OwningTeacherId, ct)
            ?? throw new NotFoundException(ErrorCodes.GroupNotFound, ErrorCodes.GroupNotFoundMessage);
}
