using Microsoft.AspNetCore.Identity;
using SwimSchool.Application.Common.Interfaces;
using SwimSchool.Domain.Entities;

namespace SwimSchool.Infrastructure.Authentication;

public class IdentityPasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<User> inner = new();

    public string Hash(string password) => inner.HashPassword(null!, password);

    public bool Verify(string passwordHash, string password)
    {
        try
        {
            return inner.VerifyHashedPassword(null!, passwordHash, password) != PasswordVerificationResult.Failed;
        }
        catch (FormatException)
        {
            // Not a real hash (e.g. "TEMP")
            return false;
        }
    }
}
