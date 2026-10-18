using Riok.Mapperly.Abstractions;
using SwimSchool.Application.Enrollments.Dtos;
using SwimSchool.Domain.Entities;

namespace SwimSchool.Application.Enrollments;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class EnrollmentMapper
{
    [MapPropertyFromSource(nameof(EnrollmentDto.StudentName), Use = nameof(StudentName))]
    [MapPropertyFromSource(nameof(EnrollmentDto.GroupName), Use = nameof(GroupName))]
    public static partial EnrollmentDto ToDto(Enrollment enrollment);

    // A navigation that was not loaded gives an empty value, not a crash (as AutoMapper did)
    private static string StudentName(Enrollment enrollment) => enrollment.Student?.FullName ?? string.Empty;

    private static string GroupName(Enrollment enrollment) => enrollment.Group?.Name ?? string.Empty;
}
