using Microsoft.Extensions.Options;
using SwimSchool.Application.Common.Interfaces;
using SwimSchool.Domain.Enums;

namespace SwimSchool.API.Development;

// TEMPORARY: removed in Stage 9
public class DevCurrentUser(IOptions<DevUserOptions> options) : ICurrentUser
{
    private readonly DevUserOptions _user = options.Value;

    public int UserId => _user.UserId;
    public UserRole Role => _user.Role;
    public int OwningTeacherId => _user.OwningTeacherId;
}
