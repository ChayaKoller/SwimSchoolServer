using Riok.Mapperly.Abstractions;
using SwimSchool.Application.Attendance.Dtos;
using SwimSchool.Domain.Entities;

namespace SwimSchool.Application.Attendance;

// GroupAttendanceGridDto is built in the service, not here
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class AttendanceMapper
{
    [MapProperty(nameof(Lesson.Id), nameof(LessonAttendanceDto.LessonId))]
    [MapPropertyFromSource(nameof(LessonAttendanceDto.GroupName), Use = nameof(GroupName))]
    [MapPropertyFromSource(nameof(LessonAttendanceDto.Students), Use = nameof(Students))]
    public static partial LessonAttendanceDto ToDto(Lesson lesson);

    private static List<LessonAttendanceStudentDto> Students(Lesson lesson) => (lesson.Group?.Enrollments ?? [])
        .Where(e => e.IsActive)
        .Select(e => new LessonAttendanceStudentDto
        {
            EnrollmentId = e.Id,
            StudentId = e.StudentId,
            FullName = e.Student?.FullName ?? string.Empty,
            WasPresent = e.Attendances
                .Where(a => a.LessonId == lesson.Id)
                .Select(a => (bool?)a.WasPresent)
                .FirstOrDefault()
        })
        .ToList();

    // A navigation that was not loaded gives an empty value, not a crash (as AutoMapper did)
    private static string GroupName(Lesson lesson) => lesson.Group?.Name ?? string.Empty;
}
