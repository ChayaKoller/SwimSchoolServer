using Moq;
using SwimSchool.Application.Common.Exceptions;
using SwimSchool.Application.Common.Interfaces;
using SwimSchool.Application.Enrollments;
using SwimSchool.Application.Enrollments.Dtos;
using SwimSchool.Application.Groups;
using SwimSchool.Application.Students;
using SwimSchool.Application.Tests.TestSupport;
using SwimSchool.Domain.Entities;

namespace SwimSchool.Application.Tests.Enrollments;

// The limited resource: a spot in a group
public class EnrollmentServiceTests
{
    private readonly Mock<IStudentRepository> _students = new();
    private readonly Mock<IGroupRepository> _groups = new();
    private readonly Mock<IEnrollmentRepository> _enrollments = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private EnrollmentService CreateService(ICurrentUser? me = null) => new(
        _students.Object, _groups.Object, _enrollments.Object, _unitOfWork.Object,
        me ?? FakeCurrentUser.Teacher(TestData.TeacherId), new FakeClock());

    private (Student student, Group group) Arrange(Group group)
    {
        var student = TestData.Student();
        _students.Setup(r => r.FindForUpdateAsync(student.Id, TestData.TeacherId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(student);
        _groups.Setup(r => r.FindForUpdateAsync(group.Id, TestData.TeacherId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(group);
        return (student, group);
    }

    private static EnrollRequest Request(Student student, Group group, decimal? amountDue = null) =>
        new() { StudentId = student.Id, GroupId = group.Id, AmountDue = amountDue };

    // ---- Enroll: the accepted case ----

    [Fact]
    public async Task Enroll_WhenGroupHasFreeSpot_ReservesSpotAndSaves()
    {
        var (student, group) = Arrange(TestData.Group(maxStudents: 10, enrolledCount: 9));
        Enrollment? added = null;
        _enrollments.Setup(r => r.Add(It.IsAny<Enrollment>())).Callback<Enrollment>(e => added = e);

        var result = await CreateService().EnrollAsync(Request(student, group));

        Assert.Equal(10, group.EnrolledCount);
        Assert.False(group.HasFreeSpot());
        Assert.NotNull(added);
        Assert.True(added.IsActive);
        Assert.Equal(FakeClock.Now, added.EnrolledAt);
        Assert.Equal(student.FullName, result.StudentName);
        Assert.Equal(group.Name, result.GroupName);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Enroll_WhenTeacherEntersDiscountedAmount_KeepsEnteredAmountNotGroupPrice()
    {
        var (student, group) = Arrange(TestData.Group(price: 1200m));

        var result = await CreateService().EnrollAsync(Request(student, group, amountDue: 950m));

        Assert.Equal(950m, result.AmountDue);
    }

    [Fact]
    public async Task Enroll_WhenNoAmountEntered_UsesGroupPriceAsDefault()
    {
        var (student, group) = Arrange(TestData.Group(price: 1200m));

        var result = await CreateService().EnrollAsync(Request(student, group, amountDue: null));

        Assert.Equal(1200m, result.AmountDue);
    }

    [Fact]
    public async Task Enroll_AsSecretary_UsesHerTeachersData()
    {
        var (student, group) = Arrange(TestData.Group());

        await CreateService(FakeCurrentUser.SecretaryOf(TestData.TeacherId)).EnrollAsync(Request(student, group));

        _students.Verify(r => r.FindForUpdateAsync(student.Id, TestData.TeacherId, It.IsAny<CancellationToken>()), Times.Once);
        _groups.Verify(r => r.FindForUpdateAsync(group.Id, TestData.TeacherId, It.IsAny<CancellationToken>()), Times.Once);
    }

    // ---- Enroll: the rejected cases ----

    [Fact]
    public async Task Enroll_WhenGroupIsFull_ThrowsGroupFull_AndSavesNothing()
    {
        var (student, group) = Arrange(TestData.Group(maxStudents: 10, enrolledCount: 10));

        var ex = await Assert.ThrowsAsync<ConflictException>(() => CreateService().EnrollAsync(Request(student, group)));

        Assert.Equal(ErrorCodes.GroupFull, ex.Code);
        Assert.Equal(10, group.EnrolledCount);
        _enrollments.Verify(r => r.Add(It.IsAny<Enrollment>()), Times.Never);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Enroll_WhenSomeoneTookTheLastSpotFirst_ThrowsSpotTaken()
    {
        // Simulates the conflict: Infrastructure turns the xmin conflict into DATA_CHANGED.
        // The real conflict against PostgreSQL is proven in SwimSchool.Infrastructure.Tests.
        var (student, group) = Arrange(TestData.Group(maxStudents: 10, enrolledCount: 9));
        _unitOfWork.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ConflictException(ErrorCodes.DataChanged, ErrorCodes.DataChangedMessage));

        var ex = await Assert.ThrowsAsync<ConflictException>(() => CreateService().EnrollAsync(Request(student, group)));

        Assert.Equal(ErrorCodes.SpotTaken, ex.Code);
        Assert.Equal(ErrorCodes.SpotTakenMessage, ex.Message);
    }

    [Fact]
    public async Task Enroll_WhenGroupIsInactive_ThrowsGroupInactive()
    {
        var (student, group) = Arrange(TestData.Group(isActive: false));

        var ex = await Assert.ThrowsAsync<ConflictException>(() => CreateService().EnrollAsync(Request(student, group)));

        Assert.Equal(ErrorCodes.GroupInactive, ex.Code);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Enroll_WhenStudentIsAlreadyEnrolled_ThrowsAlreadyEnrolled()
    {
        var (student, group) = Arrange(TestData.Group());
        _enrollments.Setup(r => r.IsActiveAsync(student.Id, group.Id, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var ex = await Assert.ThrowsAsync<ConflictException>(() => CreateService().EnrollAsync(Request(student, group)));

        Assert.Equal(ErrorCodes.AlreadyEnrolled, ex.Code);
        Assert.Equal(0, group.EnrolledCount);
    }

    [Fact]
    public async Task Enroll_WhenStudentBelongsToAnotherTeacher_ThrowsStudentNotFound()
    {
        // The repository filters by teacher, so another teacher's student comes back as null
        var group = TestData.Group();
        _groups.Setup(r => r.FindForUpdateAsync(group.Id, TestData.TeacherId, It.IsAny<CancellationToken>())).ReturnsAsync(group);
        var request = new EnrollRequest { StudentId = 99, GroupId = group.Id };

        var ex = await Assert.ThrowsAsync<NotFoundException>(() => CreateService().EnrollAsync(request));

        Assert.Equal(ErrorCodes.StudentNotFound, ex.Code);
    }

    [Fact]
    public async Task Enroll_WhenGroupBelongsToAnotherTeacher_ThrowsGroupNotFound()
    {
        var student = TestData.Student();
        _students.Setup(r => r.FindForUpdateAsync(student.Id, TestData.TeacherId, It.IsAny<CancellationToken>())).ReturnsAsync(student);
        var request = new EnrollRequest { StudentId = student.Id, GroupId = 99 };

        var ex = await Assert.ThrowsAsync<NotFoundException>(() => CreateService().EnrollAsync(request));

        Assert.Equal(ErrorCodes.GroupNotFound, ex.Code);
    }

    // ---- Cancel ----

    [Fact]
    public async Task Cancel_ActiveEnrollment_ReleasesSpotAndKeepsTheRecord()
    {
        var group = TestData.Group(maxStudents: 10, enrolledCount: 10);
        var enrollment = TestData.Enrollment(group, TestData.Student());
        _enrollments.Setup(r => r.FindForUpdateAsync(enrollment.Id, TestData.TeacherId, It.IsAny<CancellationToken>())).ReturnsAsync(enrollment);

        await CreateService().CancelAsync(enrollment.Id);

        Assert.False(enrollment.IsActive);
        Assert.Equal(9, group.EnrolledCount);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Cancel_AlreadyCancelledEnrollment_ChangesNothing()
    {
        var group = TestData.Group(maxStudents: 10, enrolledCount: 5);
        var enrollment = TestData.Enrollment(group, TestData.Student(), isActive: false);
        _enrollments.Setup(r => r.FindForUpdateAsync(enrollment.Id, TestData.TeacherId, It.IsAny<CancellationToken>())).ReturnsAsync(enrollment);

        await CreateService().CancelAsync(enrollment.Id);

        Assert.Equal(5, group.EnrolledCount);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    // ---- Payment ----

    [Fact]
    public async Task UpdatePayment_AboveAmountDue_ThrowsPaymentAboveDue()
    {
        var enrollment = TestData.Enrollment(TestData.Group(), TestData.Student(), amountDue: 1000m);
        _enrollments.Setup(r => r.FindForUpdateAsync(enrollment.Id, TestData.TeacherId, It.IsAny<CancellationToken>())).ReturnsAsync(enrollment);

        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            CreateService().UpdatePaymentAsync(enrollment.Id, new UpdatePaymentRequest { AmountPaid = 1001m }));

        Assert.Equal(ErrorCodes.PaymentAboveDue, ex.Code);
        Assert.Equal(0m, enrollment.AmountPaid);
    }

    [Fact]
    public async Task UpdatePayment_WithDiscountAfterEnrollment_ChecksAgainstTheNewAmount()
    {
        var enrollment = TestData.Enrollment(TestData.Group(), TestData.Student(), amountDue: 1000m);
        _enrollments.Setup(r => r.FindForUpdateAsync(enrollment.Id, TestData.TeacherId, It.IsAny<CancellationToken>())).ReturnsAsync(enrollment);

        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            CreateService().UpdatePaymentAsync(enrollment.Id, new UpdatePaymentRequest { AmountPaid = 900m, AmountDue = 800m }));

        Assert.Equal(ErrorCodes.PaymentAboveDue, ex.Code);
    }

    [Fact]
    public async Task UpdatePayment_WithinNewAmount_SavesBothAmounts()
    {
        var enrollment = TestData.Enrollment(TestData.Group(), TestData.Student(), amountDue: 1000m);
        _enrollments.Setup(r => r.FindForUpdateAsync(enrollment.Id, TestData.TeacherId, It.IsAny<CancellationToken>())).ReturnsAsync(enrollment);
        _enrollments.Setup(r => r.GetByIdAsync(enrollment.Id, TestData.TeacherId, It.IsAny<CancellationToken>())).ReturnsAsync(enrollment);

        var result = await CreateService().UpdatePaymentAsync(enrollment.Id, new UpdatePaymentRequest { AmountPaid = 800m, AmountDue = 800m });

        Assert.Equal(800m, result.AmountDue);
        Assert.Equal(800m, result.AmountPaid);
        _unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
