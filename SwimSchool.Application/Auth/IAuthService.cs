using SwimSchool.Application.Auth.Dtos;

namespace SwimSchool.Application.Auth;

public interface IAuthService
{
    Task<AuthResponse?> LoginAsync(LoginRequest request, CancellationToken ct = default);
    Task<CurrentUserDto> GetMeAsync(CancellationToken ct = default);
    Task<AuthResponse> ChangePasswordAsync(ChangePasswordRequest request, CancellationToken ct = default);
    Task<bool> IsSessionValidAsync(int userId, string securityStamp, CancellationToken ct = default);
}
