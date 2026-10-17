using AutoMapper;
using SwimSchool.Application.Common.Exceptions;
using SwimSchool.Application.Common.Interfaces;
using SwimSchool.Application.Common.Users;
using SwimSchool.Application.Secretaries.Dtos;
using SwimSchool.Application.Teachers;
using SwimSchool.Domain.Entities;
using SwimSchool.Domain.Enums;

namespace SwimSchool.Application.Secretaries;

public class SecretaryService(
    IUserRepository users,
    UserAccountGuard guard,
    IPasswordHasher hasher,
    IUnitOfWork uow,
    ICurrentUser me,
    IMapper mapper) : ISecretaryService
{
    public async Task<IReadOnlyList<SecretaryDto>> GetAllAsync(CancellationToken ct = default)
    {
        EnsureTeacher();

        var secretaries = await users.GetSecretariesAsync(me.UserId, ct);

        return mapper.Map<List<SecretaryDto>>(secretaries);
    }

    public async Task<SecretaryDto> CreateAsync(CreateSecretaryRequest request, CancellationToken ct = default)
    {
        EnsureTeacher();

        var email = await guard.EnsureUniqueAsync(request.Email, request.IdentityNumber, ct);

        var secretary = new User
        {
            IdentityNumber = request.IdentityNumber,
            FullName = request.FullName.Trim(),
            Email = email,
            Phone = request.Phone.Trim(),
            Role = UserRole.Secretary,
            TeacherId = me.UserId,
            PasswordHash = hasher.Hash(request.Password)
        };

        users.Add(secretary);
        await uow.SaveChangesAsync(ct);

        return mapper.Map<SecretaryDto>(secretary);
    }

    public async Task DeactivateAsync(int id, CancellationToken ct = default)
    {
        EnsureTeacher();

        var secretary = await FindSecretaryAsync(id, ct);

        secretary.IsActive = false;
        await uow.SaveChangesAsync(ct);
    }

    public async Task SetPasswordAsync(int id, SetPasswordRequest request, CancellationToken ct = default)
    {
        EnsureTeacher();

        var secretary = await FindSecretaryAsync(id, ct);

        secretary.SetPassword(hasher.Hash(request.NewPassword));
        await uow.SaveChangesAsync(ct);
    }

    private void EnsureTeacher()
    {
        if (!me.IsTeacher)
            throw new ForbiddenException(ErrorCodes.Forbidden, ErrorCodes.ForbiddenMessage);
    }

    private async Task<User> FindSecretaryAsync(int id, CancellationToken ct) =>
        await users.FindSecretaryAsync(id, me.UserId, ct)
            ?? throw new NotFoundException(ErrorCodes.SecretaryNotFound, ErrorCodes.SecretaryNotFoundMessage);
}
