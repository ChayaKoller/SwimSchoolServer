using Moq;
using SwimSchool.Application.Common.Exceptions;
using SwimSchool.Application.Common.Interfaces;
using SwimSchool.Application.Students;
using SwimSchool.Application.Students.Dtos;
using SwimSchool.Application.Teachers;
using SwimSchool.Application.Tests.TestSupport;
using SwimSchool.Domain.Entities;
using SwimSchool.Domain.Enums;

namespace SwimSchool.Application.Tests.Students;

public class StudentServiceTests
{
    private readonly Mock<IStudentRepository> _students = new();
    private readonly Mock<IUserRepository> _users = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private StudentService CreateService(ICurrentUser me) =>
        new(_students.Object, _users.Object, _unitOfWork.Object, me);

    private static CreateStudentRequest NewStudentRequest() => new()
    {
        IdentityNumber = "123456782",
        FullName = "נועה כהן",
        Phone = "0501234567",
        BirthDate = new DateOnly(2015, 3, 1),
        HealthFund = HealthFund.Maccabi
    };

    // ---- Create ----

    [Theory]
    [InlineData(UserRole.Teacher)]
    [InlineData(UserRole.Secretary)]
    public async Task Create_SetsResponsibleTeacherToTheOwningTeacher(UserRole role)
    {
        var me = role == UserRole.Teacher
            ? FakeCurrentUser.Teacher(TestData.TeacherId)
            : FakeCurrentUser.SecretaryOf(TestData.TeacherId);
        Student? added = null;
        _students.Setup(r => r.Add(It.IsAny<Student>())).Callback<Student>(s => added = s);
        _students.Setup(r => r.GetByIdAsync(It.IsAny<int>(), TestData.TeacherId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => added);

        await CreateService(me).CreateAsync(NewStudentRequest());

        Assert.NotNull(added);
        Assert.Equal(TestData.TeacherId, added.ResponsibleTeacherId);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Create_WhenIdentityNumberExists_ThrowsDuplicateIdentity()
    {
        _students.Setup(r => r.IdentityNumberExistsAsync("123456782", It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var ex = await Assert.ThrowsAsync<ConflictException>(() =>
            CreateService(FakeCurrentUser.Teacher(TestData.TeacherId)).CreateAsync(NewStudentRequest()));

        Assert.Equal(ErrorCodes.DuplicateIdentity, ex.Code);
        _students.Verify(r => r.Add(It.IsAny<Student>()), Times.Never);
    }

    // ---- Transfer ----

    [Fact]
    public async Task Transfer_ToAnotherActiveTeacher_ChangesResponsibleTeacher()
    {
        var student = TestData.Student();
        _students.Setup(r => r.FindForUpdateAsync(student.Id, TestData.TeacherId, It.IsAny<CancellationToken>())).ReturnsAsync(student);
        _users.Setup(r => r.IsActiveTeacherAsync(TestData.OtherTeacherId, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        await CreateService(FakeCurrentUser.Teacher(TestData.TeacherId))
            .TransferAsync(student.Id, new TransferStudentRequest { NewTeacherId = TestData.OtherTeacherId });

        Assert.Equal(TestData.OtherTeacherId, student.ResponsibleTeacherId);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Transfer_BySecretary_ThrowsForbidden()
    {
        var ex = await Assert.ThrowsAsync<ForbiddenException>(() =>
            CreateService(FakeCurrentUser.SecretaryOf(TestData.TeacherId))
                .TransferAsync(20, new TransferStudentRequest { NewTeacherId = TestData.OtherTeacherId }));

        Assert.Equal(ErrorCodes.Forbidden, ex.Code);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Transfer_ToTheSameTeacher_ThrowsSameTeacher()
    {
        var student = TestData.Student();
        _students.Setup(r => r.FindForUpdateAsync(student.Id, TestData.TeacherId, It.IsAny<CancellationToken>())).ReturnsAsync(student);

        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            CreateService(FakeCurrentUser.Teacher(TestData.TeacherId))
                .TransferAsync(student.Id, new TransferStudentRequest { NewTeacherId = TestData.TeacherId }));

        Assert.Equal(ErrorCodes.SameTeacher, ex.Code);
    }

    [Fact]
    public async Task Transfer_ToUnknownOrInactiveTeacher_ThrowsTeacherNotFound()
    {
        var student = TestData.Student();
        _students.Setup(r => r.FindForUpdateAsync(student.Id, TestData.TeacherId, It.IsAny<CancellationToken>())).ReturnsAsync(student);
        _users.Setup(r => r.IsActiveTeacherAsync(TestData.OtherTeacherId, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var ex = await Assert.ThrowsAsync<NotFoundException>(() =>
            CreateService(FakeCurrentUser.Teacher(TestData.TeacherId))
                .TransferAsync(student.Id, new TransferStudentRequest { NewTeacherId = TestData.OtherTeacherId }));

        Assert.Equal(ErrorCodes.TeacherNotFound, ex.Code);
        Assert.Equal(TestData.TeacherId, student.ResponsibleTeacherId);
    }

    [Fact]
    public async Task Transfer_OfAnotherTeachersStudent_ThrowsStudentNotFound()
    {
        var ex = await Assert.ThrowsAsync<NotFoundException>(() =>
            CreateService(FakeCurrentUser.Teacher(TestData.TeacherId))
                .TransferAsync(99, new TransferStudentRequest { NewTeacherId = TestData.OtherTeacherId }));

        Assert.Equal(ErrorCodes.StudentNotFound, ex.Code);
    }

    // ---- Delete ----

    [Fact]
    public async Task Delete_StudentWithHistory_ArchivesAndReleasesHerSpots()
    {
        var group = TestData.Group(maxStudents: 10, enrolledCount: 10);
        var student = TestData.Student();
        var enrollment = TestData.Enrollment(group, student);
        student.Enrollments.Add(enrollment);
        _students.Setup(r => r.FindForDeleteAsync(student.Id, TestData.TeacherId, It.IsAny<CancellationToken>())).ReturnsAsync(student);

        await CreateService(FakeCurrentUser.Teacher(TestData.TeacherId)).DeleteAsync(student.Id);

        Assert.False(student.IsActive);
        Assert.False(enrollment.IsActive);
        Assert.Equal(9, group.EnrolledCount);
        _students.Verify(r => r.Remove(It.IsAny<Student>()), Times.Never);
    }
}
