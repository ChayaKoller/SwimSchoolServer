using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SwimSchool.API.Auth;
using SwimSchool.Application.Auth;
using SwimSchool.Application.Auth.Dtos;
using SwimSchool.Application.Common.Exceptions;
using SwimSchool.Application.Common.Models;

namespace SwimSchool.API.Controllers;

[ApiController]
[Route("api/auth")]
[Produces("application/json")]
[ProducesResponseType<ErrorResponse>(StatusCodes.Status401Unauthorized)]
public class AuthController(IAuthService auth) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    [EnableRateLimiting(LoginRateLimit.PolicyName)]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await auth.LoginAsync(request, ct);
        if (result is null)
        {
            return Unauthorized(new ErrorResponse(StatusCodes.Status401Unauthorized,
                ErrorCodes.InvalidCredentials, ErrorCodes.InvalidCredentialsMessage, null, HttpContext.TraceIdentifier));
        }

        return Ok(result);
    }

    [HttpGet("me")]
    [ProducesResponseType<CurrentUserDto>(StatusCodes.Status200OK)]
    public async Task<ActionResult<CurrentUserDto>> Me(CancellationToken ct) =>
        Ok(await auth.GetMeAsync(ct));

    [HttpPost("change-password")]
    [EnableRateLimiting(LoginRateLimit.PolicyName)]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<AuthResponse>> ChangePassword(
        [FromBody] ChangePasswordRequest request, CancellationToken ct) =>
        Ok(await auth.ChangePasswordAsync(request, ct));
}
