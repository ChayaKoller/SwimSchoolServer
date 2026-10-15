using SwimSchool.Application.Common.Models;
using SwimSchool.Application.Groups.Dtos;

namespace SwimSchool.Application.Groups;

public interface IGroupService
{
    Task<PagedResult<GroupDto>> GetAsync(GroupFilter filter, CancellationToken ct = default);

    Task<GroupDetailsDto> GetDetailsAsync(int id, CancellationToken ct = default);

    Task<GroupDetailsDto> CreateAsync(CreateGroupRequest request, CancellationToken ct = default);

    Task<GroupDto> UpdateAsync(int id, UpdateGroupRequest request, CancellationToken ct = default);
}
