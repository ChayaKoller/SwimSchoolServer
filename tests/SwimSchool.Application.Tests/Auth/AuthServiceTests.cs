using Moq;
using SwimSchool.Application.Auth;
using SwimSchool.Application.Auth.Dtos;
using SwimSchool.Application.Common.Interfaces;
using SwimSchool.Application.Teachers;
using SwimSchool.Application.Tests.TestSupport;
using SwimSchool.Domain.Entities;

namespace SwimSchool.Application.Tests.Auth;

public class AuthServiceTests
{
    private const string Password = "Swim2026";

    private readonly Mock<IUserRepository> _users = new();
    private readonly Mock<IPasswordHasher> _hasher = new();
    private readonly Mock<IJwtTokenGenerator> _tokens = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private AuthService CreateService() => new(
        _users.Object, _hasher.Object, _tokens.Object, _unitOfWork.Object,
        new FakeClock(), FakeCurrentUser.Teacher(TestData.TeacherId));

    private void ArrangeUser(User user, bool passwordMatches)
    {
        _users.Setup(r => r.FindByEmailAsync(user.Email, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _hasher.Setup(h => h.Verify(user.PasswordHash, Password)).Returns(passwordMatches);
        _tokens.Setup(t => t.Generate(user)).Returns(new AccessToken("TOKEN", FakeClock.Now.AddHours(8)));
    }

    private static LoginRequest Login(string email) => new() { Email = email, Password = Password };

    [Fact]
    public async Task Login_WithCorrectDetails_ReturnsTokenAndUpdatesLastLogin()
    {
        var teacher = TestData.Teacher();
        ArrangeUser(teacher, passwordMatches: true);

        // Upper case and spaces: the email is compared in lower case
        var result = await CreateService().LoginAsync(Login("  Rachel@Example.com "));

        Assert.NotNull(result);
        Assert.Equal("TOKEN", result.Token);
        Assert.Equal(teacher.Id, result.User.Id);
        Assert.Equal(FakeClock.Now, teacher.LastLoginAt);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Login_WithWrongPassword_ReturnsNull()
    {
        ArrangeUser(TestData.Teacher(), passwordMatches: false);

        Assert.Null(await CreateService().LoginAsync(Login("rachel@example.com")));
        _tokens.Verify(t => t.Generate(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Login_WithUnknownEmail_ReturnsNull_AndStillHashesForEqualTiming()
    {
        Assert.Null(await CreateService().LoginAsync(Login("nobody@example.com")));
        _hasher.Verify(h => h.Hash(Password), Times.Once);
    }

    [Fact]
    public async Task Login_WhenUserIsInactive_ReturnsNull()
    {
        ArrangeUser(TestData.Teacher(isActive: false), passwordMatches: true);

        Assert.Null(await CreateService().LoginAsync(Login("rachel@example.com")));
    }

    [Fact]
    public async Task Login_WhenSecretarysTeacherIsInactive_ReturnsNull()
    {
        var secretary = TestData.Secretary(TestData.Teacher(isActive: false));
        ArrangeUser(secretary, passwordMatches: true);

        Assert.Null(await CreateService().LoginAsync(Login(secretary.Email)));
    }
}
