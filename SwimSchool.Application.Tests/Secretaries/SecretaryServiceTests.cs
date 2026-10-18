using Moq;
using SwimSchool.Application.Common.Exceptions;
using SwimSchool.Application.Common.Interfaces;
using SwimSchool.Application.Common.Users;
using SwimSchool.Application.Secretaries;
using SwimSchool.Application.Secretaries.Dtos;
using SwimSchool.Application.Teachers;
using SwimSchool.Application.Tests.TestSupport;
using SwimSchool.Domain.Entities;
using SwimSchool.Domain.Enums;

namespace SwimSchool.Application.Tests.Secretaries;

public class SecretaryServiceTests
{
    private readonly Mock<IUserRepository> _users = new();
    private readonly Mock<IPasswordHasher> _hasher = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private SecretaryService CreateService(ICurrentUser me) => new(
        _users.Object, new UserAccountGuard(_users.Object), _hasher.Object, _unitOfWork.Object, me, TestMapper.Instance);

    private static CreateSecretaryRequest Request() => new()
    {
        IdentityNumber = "222222226",
        FullName = "דינה מזכירה",
        Email = " Dina@Example.com ",
        Phone = "0521111111",
        Password = "Office2026"
    };

    [Fact]
    public async Task Create_ByTeacher_LinksSecretaryToHer_AndStoresOnlyTheHash()
    {
        User? added = null;
        _users.Setup(r => r.Add(It.IsAny<User>())).Callback<User>(u => added = u);
        _hasher.Setup(h => h.Hash("Office2026")).Returns("HASHED");

        await CreateService(FakeCurrentUser.Teacher(TestData.TeacherId)).CreateAsync(Request());

        Assert.NotNull(added);
        Assert.Equal(UserRole.Secretary, added.Role);
        Assert.Equal(TestData.TeacherId, added.TeacherId);
        Assert.Equal("dina@example.com", added.Email);
        Assert.Equal("HASHED", added.PasswordHash);
    }

    [Fact]
    public async Task Create_BySecretary_ThrowsForbidden()
    {
        var ex = await Assert.ThrowsAsync<ForbiddenException>(() =>
            CreateService(FakeCurrentUser.SecretaryOf(TestData.TeacherId)).CreateAsync(Request()));

        Assert.Equal(ErrorCodes.Forbidden, ex.Code);
        _users.Verify(r => r.Add(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Create_WithExistingEmail_ThrowsDuplicateEmail()
    {
        _users.Setup(r => r.EmailExistsAsync("dina@example.com", It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var ex = await Assert.ThrowsAsync<ConflictException>(() =>
            CreateService(FakeCurrentUser.Teacher(TestData.TeacherId)).CreateAsync(Request()));

        Assert.Equal(ErrorCodes.DuplicateEmail, ex.Code);
    }

    [Fact]
    public async Task Deactivate_AnotherTeachersSecretary_ThrowsSecretaryNotFound()
    {
        // The repository looks only among this teacher's secretaries
        var ex = await Assert.ThrowsAsync<NotFoundException>(() =>
            CreateService(FakeCurrentUser.Teacher(TestData.TeacherId)).DeactivateAsync(77));

        Assert.Equal(ErrorCodes.SecretaryNotFound, ex.Code);
    }
}
