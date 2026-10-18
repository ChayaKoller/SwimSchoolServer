using Moq;
using SwimSchool.Application.Common.Exceptions;
using SwimSchool.Application.Common.Interfaces;
using SwimSchool.Application.Groups;
using SwimSchool.Application.Lessons;
using SwimSchool.Application.Lessons.Dtos;
using SwimSchool.Application.Tests.TestSupport;
using SwimSchool.Domain.Entities;

namespace SwimSchool.Application.Tests.Lessons;

public class LessonServiceTests
{
    private readonly Mock<ILessonRepository> _lessons = new();
    private readonly Mock<IGroupRepository> _groups = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();

    private LessonService CreateService() => new(
        _lessons.Object, _groups.Object, _unitOfWork.Object,
        FakeCurrentUser.Teacher(TestData.TeacherId), new FakeClock());

    private Lesson ArrangeLesson(bool hasAttendance)
    {
        var group = TestData.Group();
        var lesson = new Lesson { Id = 40, Group = group, GroupId = group.Id, StartTime = FakeClock.Now };
        _lessons.Setup(r => r.FindForUpdateAsync(lesson.Id, TestData.TeacherId, It.IsAny<CancellationToken>())).ReturnsAsync(lesson);
        _lessons.Setup(r => r.HasAttendanceAsync(lesson.Id, It.IsAny<CancellationToken>())).ReturnsAsync(hasAttendance);
        return lesson;
    }

    [Fact]
    public async Task Update_LessonWithAttendance_ThrowsLessonHasAttendance()
    {
        var lesson = ArrangeLesson(hasAttendance: true);

        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            CreateService().UpdateAsync(lesson.Id, new UpdateLessonRequest { StartTime = new DateTime(2026, 11, 3, 17, 0, 0) }));

        Assert.Equal(ErrorCodes.LessonHasAttendance, ex.Code);
        Assert.Equal(FakeClock.Now, lesson.StartTime);
    }

    [Fact]
    public async Task Delete_LessonWithAttendance_ThrowsLessonHasAttendance()
    {
        var lesson = ArrangeLesson(hasAttendance: true);

        var ex = await Assert.ThrowsAsync<BusinessRuleException>(() => CreateService().DeleteAsync(lesson.Id));

        Assert.Equal(ErrorCodes.LessonHasAttendance, ex.Code);
        _lessons.Verify(r => r.Remove(It.IsAny<Lesson>()), Times.Never);
    }

    [Fact]
    public async Task Delete_LessonWithoutAttendance_RemovesItAndUpdatesCount()
    {
        var lesson = ArrangeLesson(hasAttendance: false);

        await CreateService().DeleteAsync(lesson.Id);

        _lessons.Verify(r => r.Remove(lesson), Times.Once);
        Assert.Equal(11, lesson.Group.LessonsCount);
    }
}
