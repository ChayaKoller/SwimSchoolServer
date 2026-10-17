using SwimSchool.Application.Common.Exceptions;
using SwimSchool.Application.Teachers;

namespace SwimSchool.Application.Common.Users;

public class UserAccountGuard(IUserRepository users)
{
    public static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();

    // Returns the normalized email
    public async Task<string> EnsureUniqueAsync(string email, string identityNumber, CancellationToken ct = default)
    {
        var normalizedEmail = NormalizeEmail(email);

        if (await users.EmailExistsAsync(normalizedEmail, ct))
            throw new ConflictException(ErrorCodes.DuplicateEmail, ErrorCodes.DuplicateEmailMessage);

        if (await users.IdentityNumberExistsAsync(identityNumber, ct))
            throw new ConflictException(ErrorCodes.DuplicateUserIdentity, ErrorCodes.DuplicateUserIdentityMessage);

        return normalizedEmail;
    }
}
