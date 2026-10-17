using System.Globalization;
using SwimSchool.Application.Auth;
using SwimSchool.Application.Common.Interfaces;
using SwimSchool.Domain.Enums;

namespace SwimSchool.API.Auth;

public class HttpCurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    public int UserId => int.Parse(GetClaim(AuthClaims.UserId), CultureInfo.InvariantCulture);
    public UserRole Role => Enum.Parse<UserRole>(GetClaim(AuthClaims.Role));
    public int OwningTeacherId => int.Parse(GetClaim(AuthClaims.OwningTeacherId), CultureInfo.InvariantCulture);

    private string GetClaim(string type) =>
        accessor.HttpContext?.User.FindFirst(type)?.Value
        ?? throw new InvalidOperationException($"Missing claim '{type}'");
}
