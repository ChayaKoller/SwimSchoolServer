using SwimSchool.Application.Common.Interfaces;
using SwimSchool.Application.Common.Users;
using SwimSchool.Application.Teachers.Dtos;
using SwimSchool.Domain.Entities;
using SwimSchool.Domain.Enums;

namespace SwimSchool.Application.Teachers;

// Used only by the add-teacher command line. No HTTP endpoint creates teachers.
public class TeacherAccountService(
    IUserRepository users,
    UserAccountGuard guard,
    IPasswordHasher hasher,
    IUnitOfWork uow) : ITeacherAccountService
{
    public async Task<int> CreateTeacherAsync(CreateTeacherRequest request, CancellationToken ct = default)
    {
        var email = await guard.EnsureUniqueAsync(request.Email, request.IdentityNumber, ct);

        var teacher = new User
        {
            IdentityNumber = request.IdentityNumber,
            FullName = request.FullName.Trim(),
            Email = email,
            Phone = request.Phone.Trim(),
            Role = UserRole.Teacher,
            PasswordHash = hasher.Hash(request.Password)
        };

        users.Add(teacher);
        await uow.SaveChangesAsync(ct);

        return teacher.Id;
    }
}
