using Riok.Mapperly.Abstractions;
using SwimSchool.Application.Teachers.Dtos;
using SwimSchool.Domain.Entities;

namespace SwimSchool.Application.Teachers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class TeacherMapper
{
    public static partial TeacherOptionDto ToOptionDto(User teacher);

    public static partial List<TeacherOptionDto> ToOptionDtos(IEnumerable<User> teachers);
}
