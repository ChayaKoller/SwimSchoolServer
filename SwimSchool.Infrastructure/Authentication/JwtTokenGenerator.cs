using System.Globalization;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SwimSchool.Application.Auth;
using SwimSchool.Application.Common.Interfaces;
using SwimSchool.Domain.Entities;

namespace SwimSchool.Infrastructure.Authentication;

public class JwtTokenGenerator(IOptions<JwtOptions> options, IDateTimeProvider clock) : IJwtTokenGenerator
{
    private readonly JsonWebTokenHandler _handler = new();

    public AccessToken Generate(User user)
    {
        var jwt = options.Value;
        var now = clock.UtcNow;
        var expires = now.AddMinutes(jwt.ExpiryMinutes);

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = jwt.Issuer,
            Audience = jwt.Audience,
            IssuedAt = now,
            NotBefore = now,
            Expires = expires,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
                SecurityAlgorithms.HmacSha256),
            Claims = new Dictionary<string, object>
            {
                [AuthClaims.UserId] = user.Id.ToString(CultureInfo.InvariantCulture),
                [AuthClaims.Role] = user.Role.ToString(),
                [AuthClaims.OwningTeacherId] = user.OwningTeacherId.ToString(CultureInfo.InvariantCulture),
                [AuthClaims.Name] = user.FullName,
                [AuthClaims.SecurityStamp] = user.SecurityStamp
            }
        };

        return new AccessToken(_handler.CreateToken(descriptor), expires);
    }
}
