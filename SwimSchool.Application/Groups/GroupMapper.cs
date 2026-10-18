using Riok.Mapperly.Abstractions;
using SwimSchool.Application.Groups.Dtos;
using SwimSchool.Application.Lessons;
using SwimSchool.Application.Lessons.Dtos;
using SwimSchool.Domain.Entities;

namespace SwimSchool.Application.Groups;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class GroupMapper
{
    [MapPropertyFromSource(nameof(GroupDto.FreeSpots), Use = nameof(FreeSpots))]
    public static partial GroupDto ToDto(Group group);

    public static partial List<GroupDto> ToDtos(IEnumerable<Group> groups);

    [MapPropertyFromSource(nameof(GroupDetailsDto.FreeSpots), Use = nameof(FreeSpots))]
    [MapProperty(nameof(Group.Enrollments), nameof(GroupDetailsDto.Students), Use = nameof(ActiveStudents))]
    [MapProperty(nameof(Group.Lessons), nameof(GroupDetailsDto.Lessons), Use = nameof(LessonsByTime))]
    public static partial GroupDetailsDto ToDetailsDto(Group group);

    // LessonsCount comes from the service (the real number of lessons created)
    [MapperIgnoreTarget(nameof(Group.Id))]
    [MapperIgnoreTarget(nameof(Group.CreatedAt))]
    [MapperIgnoreTarget(nameof(Group.UpdatedAt))]
    [MapperIgnoreTarget(nameof(Group.LessonsCount))]
    [MapperIgnoreTarget(nameof(Group.IsActive))]
    [MapperIgnoreTarget(nameof(Group.EnrolledCount))]
    [MapperIgnoreTarget(nameof(Group.Version))]
    [MapperIgnoreTarget(nameof(Group.TeacherId))]
    [MapperIgnoreTarget(nameof(Group.Teacher))]
    [MapperIgnoreTarget(nameof(Group.Lessons))]
    [MapperIgnoreTarget(nameof(Group.Enrollments))]
    public static partial Group ToEntity(CreateGroupRequest request);

    [MapperIgnoreTarget(nameof(Group.Id))]
    [MapperIgnoreTarget(nameof(Group.CreatedAt))]
    [MapperIgnoreTarget(nameof(Group.UpdatedAt))]
    [MapperIgnoreTarget(nameof(Group.Year))]
    [MapperIgnoreTarget(nameof(Group.Season))]
    [MapperIgnoreTarget(nameof(Group.EnrolledCount))]
    [MapperIgnoreTarget(nameof(Group.TeacherId))]
    [MapperIgnoreTarget(nameof(Group.Teacher))]
    [MapperIgnoreTarget(nameof(Group.Lessons))]
    [MapperIgnoreTarget(nameof(Group.Enrollments))]
    public static partial void UpdateEntity(UpdateGroupRequest request, Group group);

    // AmountOwed in this group only
    [MapProperty(nameof(Enrollment.StudentId), nameof(GroupStudentDto.Id))]
    [MapPropertyFromSource(nameof(GroupStudentDto.FullName), Use = nameof(StudentName))]
    [MapPropertyFromSource(nameof(GroupStudentDto.AmountOwed), Use = nameof(AmountOwed))]
    private static partial GroupStudentDto ToStudentDto(Enrollment enrollment);

    private static int FreeSpots(Group group) => group.MaxStudents - group.EnrolledCount;

    private static decimal AmountOwed(Enrollment enrollment) => enrollment.AmountDue - enrollment.AmountPaid;

    private static List<GroupStudentDto> ActiveStudents(ICollection<Enrollment> enrollments) =>
        enrollments.Where(e => e.IsActive).Select(ToStudentDto).ToList();

    private static List<LessonDto> LessonsByTime(ICollection<Lesson> lessons) =>
        LessonMapper.ToDtos(lessons.OrderBy(l => l.StartTime));

    // A navigation that was not loaded gives an empty value, not a crash (as AutoMapper did)
    private static string StudentName(Enrollment enrollment) => enrollment.Student?.FullName ?? string.Empty;
}
