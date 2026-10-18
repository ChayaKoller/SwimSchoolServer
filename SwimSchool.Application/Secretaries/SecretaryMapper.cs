using Riok.Mapperly.Abstractions;
using SwimSchool.Application.Secretaries.Dtos;
using SwimSchool.Domain.Entities;

namespace SwimSchool.Application.Secretaries;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class SecretaryMapper
{
    public static partial SecretaryDto ToDto(User secretary);

    public static partial List<SecretaryDto> ToDtos(IEnumerable<User> secretaries);
}
