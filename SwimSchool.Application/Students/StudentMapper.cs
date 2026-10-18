using Riok.Mapperly.Abstractions;
using SwimSchool.Application.Students.Dtos;
using SwimSchool.Domain.Entities;
using SwimSchool.Domain.Enums;

namespace SwimSchool.Application.Students;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class StudentMapper
{
    [MapPropertyFromSource(nameof(StudentListItemDto.AmountOwed), Use = nameof(AmountOwed))]
    [MapPropertyFromSource(nameof(StudentListItemDto.AbsencesCount), Use = nameof(AbsencesCount))]
    public static partial StudentListItemDto ToListItemDto(Student student);

    public static partial List<StudentListItemDto> ToListItemDtos(IEnumerable<Student> students);

    [MapPropertyFromSource(nameof(StudentDetailsDto.ResponsibleTeacherName), Use = nameof(ResponsibleTeacherName))]
    [MapProperty(nameof(Student.Enrollments), nameof(StudentDetailsDto.Groups), Use = nameof(ActiveGroups))]
    public static partial StudentDetailsDto ToDetailsDto(Student student);

    [MapperIgnoreTarget(nameof(Student.Id))]
    [MapperIgnoreTarget(nameof(Student.CreatedAt))]
    [MapperIgnoreTarget(nameof(Student.UpdatedAt))]
    [MapperIgnoreTarget(nameof(Student.IsActive))]
    [MapperIgnoreTarget(nameof(Student.ResponsibleTeacherId))]
    [MapperIgnoreTarget(nameof(Student.ResponsibleTeacher))]
    [MapperIgnoreTarget(nameof(Student.Enrollments))]
    [MapperIgnoreTarget(nameof(Student.Reports))]
    public static partial Student ToEntity(CreateStudentRequest request);

    [MapperIgnoreTarget(nameof(Student.Id))]
    [MapperIgnoreTarget(nameof(Student.CreatedAt))]
    [MapperIgnoreTarget(nameof(Student.UpdatedAt))]
    [MapperIgnoreTarget(nameof(Student.IdentityNumber))]
    [MapperIgnoreTarget(nameof(Student.BirthDate))]
    [MapperIgnoreTarget(nameof(Student.ResponsibleTeacherId))]
    [MapperIgnoreTarget(nameof(Student.ResponsibleTeacher))]
    [MapperIgnoreTarget(nameof(Student.Enrollments))]
    [MapperIgnoreTarget(nameof(Student.Reports))]
    public static partial void UpdateEntity(UpdateStudentRequest request, Student student);

    [MapProperty(nameof(Enrollment.Id), nameof(StudentGroupDto.EnrollmentId))]
    [MapPropertyFromSource(nameof(StudentGroupDto.GroupName), Use = nameof(GroupName))]
    [MapPropertyFromSource(nameof(StudentGroupDto.Season), Use = nameof(GroupSeason))]
    [MapPropertyFromSource(nameof(StudentGroupDto.Year), Use = nameof(GroupYear))]
    private static partial StudentGroupDto ToGroupDto(Enrollment enrollment);

    private static decimal AmountOwed(Student student) => student.Enrollments
        .Where(e => e.IsActive)
        .Sum(e => e.AmountDue - e.AmountPaid);

    private static int AbsencesCount(Student student) => student.Enrollments
        .Where(e => e.IsActive)
        .SelectMany(e => e.Attendances)
        .Count(a => !a.WasPresent);

    private static List<StudentGroupDto> ActiveGroups(ICollection<Enrollment> enrollments) =>
        enrollments.Where(e => e.IsActive).Select(ToGroupDto).ToList();

    // A navigation that was not loaded gives an empty value, not a crash (as AutoMapper did)
    private static string ResponsibleTeacherName(Student student) => student.ResponsibleTeacher?.FullName ?? string.Empty;

    private static string GroupName(Enrollment enrollment) => enrollment.Group?.Name ?? string.Empty;

    private static Season GroupSeason(Enrollment enrollment) => enrollment.Group?.Season ?? default;

    private static int GroupYear(Enrollment enrollment) => enrollment.Group?.Year ?? default;
}
