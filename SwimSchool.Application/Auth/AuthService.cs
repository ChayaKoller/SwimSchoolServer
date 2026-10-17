using SwimSchool.Application.Auth.Dtos;
using SwimSchool.Application.Common.Exceptions;
using SwimSchool.Application.Common.Interfaces;
using SwimSchool.Application.Teachers;

namespace SwimSchool.Application.Auth;

public class AuthService(
    IUserRepository users,
    IPasswordHasher hasher,
    IJwtTokenGenerator tokens,
    IUnitOfWork uow,
    IDateTimeProvider clock,
    ICurrentUser me) : IAuthService
{
    public async Task<AuthResponse?> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await users.FindByEmailAsync(email, ct);

        if (user is null)
        {
            // Same work as a real check, so timing does not reveal unknown emails
            _ = hasher.Hash(request.Password);
            return null;
        }

        if (!hasher.Verify(user.PasswordHash, request.Password) || !user.CanSignIn)
            return null;

        user.LastLoginAt = clock.UtcNow;
        await uow.SaveChangesAsync(ct);

        var token = tokens.Generate(user);

        return new AuthResponse(token.Token, token.ExpiresAt, new CurrentUserDto(user.Id, user.FullName, user.Role));
    }

    public async Task<CurrentUserDto> GetMeAsync(CancellationToken ct = default)
    {
        var user = await users.GetByIdAsync(me.UserId, ct)
            ?? throw new InvalidOperationException("Authenticated user not found");

        return new CurrentUserDto(user.Id, user.FullName, user.Role);
    }

    // A new token keeps this session; every other session ends (new security stamp)
    public async Task<AuthResponse> ChangePasswordAsync(ChangePasswordRequest request, CancellationToken ct = default)
    {
        var user = await users.FindByIdForUpdateAsync(me.UserId, ct)
            ?? throw new InvalidOperationException("Authenticated user not found");

        if (!hasher.Verify(user.PasswordHash, request.CurrentPassword))
            throw new BusinessRuleException(ErrorCodes.WrongPassword, ErrorCodes.WrongPasswordMessage);

        user.SetPassword(hasher.Hash(request.NewPassword));
        await uow.SaveChangesAsync(ct);

        var token = tokens.Generate(user);

        return new AuthResponse(token.Token, token.ExpiresAt, new CurrentUserDto(user.Id, user.FullName, user.Role));
    }

    public async Task<bool> IsSessionValidAsync(int userId, string securityStamp, CancellationToken ct = default)
    {
        var state = await users.GetSessionStateAsync(userId, ct);

        return state is { IsActive: true, TeacherIsActive: true } && state.SecurityStamp == securityStamp;
    }
}
