using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SwimSchool.Application.Auth;
using SwimSchool.Application.Common.Exceptions;
using SwimSchool.Application.Common.Models;

namespace SwimSchool.API.Auth;

public static class JwtAuthenticationSetup
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<JwtOptions>>((bearer, jwt) =>
            {
                var options = jwt.Value;

                bearer.MapInboundClaims = false;
                bearer.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = options.Issuer,
                    ValidAudience = options.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Key)),
                    ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
                    ClockSkew = TimeSpan.FromMinutes(1),
                    NameClaimType = AuthClaims.Name,
                    RoleClaimType = AuthClaims.Role
                };

                bearer.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var sub = context.Principal?.FindFirst(AuthClaims.UserId)?.Value;
                        var stamp = context.Principal?.FindFirst(AuthClaims.SecurityStamp)?.Value;

                        var valid = int.TryParse(sub, NumberStyles.None, CultureInfo.InvariantCulture, out var userId)
                            && !string.IsNullOrEmpty(stamp)
                            && await context.HttpContext.RequestServices.GetRequiredService<IAuthService>()
                                .IsSessionValidAsync(userId, stamp, context.HttpContext.RequestAborted);

                        if (!valid)
                            context.Fail("Session is no longer valid");
                    },
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();
                        await WriteErrorAsync(context.HttpContext, StatusCodes.Status401Unauthorized,
                            ErrorCodes.Unauthorized, ErrorCodes.UnauthorizedMessage);
                    },
                    OnForbidden = context =>
                        WriteErrorAsync(context.HttpContext, StatusCodes.Status403Forbidden,
                            ErrorCodes.Forbidden, ErrorCodes.ForbiddenMessage)
                };
            });

        services.AddAuthorization(o => o.FallbackPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build());

        return services;
    }

    private static Task WriteErrorAsync(HttpContext http, int status, string code, string message)
    {
        http.Response.StatusCode = status;
        return http.Response.WriteAsJsonAsync(
            new ErrorResponse(status, code, message, null, http.TraceIdentifier), http.RequestAborted);
    }
}
