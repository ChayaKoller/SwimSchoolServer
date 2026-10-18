using Moq;
using SwimSchool.Application.Common.Exceptions;
using SwimSchool.Application.Common.Interfaces;
using SwimSchool.Application.Groups;
using SwimSchool.Application.Groups.Dtos;
using SwimSchool.Application.Tests.TestSupport;
using SwimSchool.Domain.Entities;
using SwimSchool.Domain.Enums;

namespace SwimSchool.Application.Tests.Groups;

public class GroupServiceTests
{
    private readonly Mock<IGroupRepository> _groups = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private GroupService CreateService(ICurrentUser? me = null) => new(
        _groups.Object, _unitOfWork.Object, me ?? FakeCurrentUser.Teacher(TestData.TeacherId), new FakeClock(), TestMapper.Instance);

    private static UpdateGroupRequest UpdateRequest(int maxStudents, uint version = 1) => new()
    {
        Name = "מתחילות א",
        LessonsCount = 12,
        Price = 1200m,
        MaxStudents = maxStudents,
        IsActive = true,
        Version = version
    };

    [Fact]
    public async Task Create_WithFirstLessonAndCount_GeneratesWeeklyLessonsInUtc()
    {
        Group? added = null;
        _groups.Setup(r => r.Add(It.IsAny<Group>())).Callback<Group>(g => added = g);
        _groups.Setup(r => r.GetDetailsAsync(It.IsAny<int>(), TestData.TeacherId, It.IsAny<CancellationToken>())).ReturnsAsync(() => added);
        var firstLesson = new DateTime(2026, 11, 2, 17, 0, 0); // Israel local time

        await CreateService().CreateAsync(new CreateGroupRequest
        {
            Name = "מתחילות א",
            Year = 2026,
            Season = Season.Autumn,
            Price = 1200m,
            MaxStudents = 8,
            FirstLessonAt = firstLesson,
            LessonsCount = 3
        });

        Assert.NotNull(added);
        Assert.Equal(TestData.TeacherId, added.TeacherId);
        Assert.Equal(3, added.LessonsCount);
        Assert.Equal(
            new[] { firstLesson, firstLesson.AddDays(7), firstLesson.AddDays(14) }.Select(t => t.AddHours(-3)),
            added.Lessons.Select(l => l.StartTime));
    }

    [Fact]
    public async Task Update_WhenMaxStudentsBelowEnrolled_ThrowsMaxBelowEnrolled()
    {
        var group = TestData.Group(maxStudents: 10, enrolledCount: 6);
        _groups.Setup(r => r.FindForUpdateAsync(group.Id, TestData.TeacherId, It.IsAny<CancellationToken>())).ReturnsAsync(group);

        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() => CreateService().UpdateAsync(group.Id, UpdateRequest(maxStudents: 5)));

        Assert.Equal(ErrorCodes.MaxBelowEnrolled, ex.Code);
        Assert.Equal(10, group.MaxStudents);
    }

    [Fact]
    public async Task Update_WithOldVersion_ThrowsDataChanged()
    {
        var group = TestData.Group();
        group.Version = 7;
        _groups.Setup(r => r.FindForUpdateAsync(group.Id, TestData.TeacherId, It.IsAny<CancellationToken>())).ReturnsAsync(group);

        var ex = await Assert.ThrowsAsync<ConflictException>(() => CreateService().UpdateAsync(group.Id, UpdateRequest(maxStudents: 10, version: 6)));

        Assert.Equal(ErrorCodes.DataChanged, ex.Code);
    }

    [Fact]
    public async Task Delete_GroupWithActiveStudents_ThrowsGroupHasStudents()
    {
        var group = TestData.Group(enrolledCount: 1);
        group.Enrollments.Add(TestData.Enrollment(group, TestData.Student()));
        _groups.Setup(r => r.FindForDeleteAsync(group.Id, TestData.TeacherId, It.IsAny<CancellationToken>())).ReturnsAsync(group);

        var ex = await Assert.ThrowsAsync<ConflictException>(() => CreateService().DeleteAsync(group.Id, group.Version));

        Assert.Equal(ErrorCodes.GroupHasStudents, ex.Code);
        _groups.Verify(r => r.Remove(It.IsAny<Group>()), Times.Never);
    }

    [Fact]
    public async Task Delete_BySecretary_ThrowsForbidden()
    {
        var ex = await Assert.ThrowsAsync<ForbiddenException>(() =>
            CreateService(FakeCurrentUser.SecretaryOf(TestData.TeacherId)).DeleteAsync(10, 1));

        Assert.Equal(ErrorCodes.Forbidden, ex.Code);
    }
}
