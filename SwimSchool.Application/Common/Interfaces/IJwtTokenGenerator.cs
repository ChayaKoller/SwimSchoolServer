using SwimSchool.Application.Auth;
using SwimSchool.Domain.Entities;

namespace SwimSchool.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    AccessToken Generate(User user);
}
