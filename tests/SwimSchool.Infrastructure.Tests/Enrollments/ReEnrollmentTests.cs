using Microsoft.EntityFrameworkCore;
using SwimSchool.Application.Common.Exceptions;
using SwimSchool.Application.Enrollments.Dtos;
using SwimSchool.Domain.Entities;
using SwimSchool.Infrastructure.Tests.TestSupport;

namespace SwimSchool.Infrastructure.Tests.Enrollments;

// A cancelled enrollment stays as history (attendance, payments), so the unique index
// on (StudentId, GroupId) must only cover ACTIVE enrollments.
[Collection(PostgresCollection.Name)]
public class ReEnrollmentTests(PostgresDatabase database)
{
    [Fact]
    public async Task EnrollCancelEnrollAgain_SameStudentSameGroup_Succeeds_AndKeepsTheOldRow()
    {
        var (teacher, group, students) = await TestSeed.TeacherWithGroupAsync(database, maxStudents: 10, enrolledCount: 0, studentsCount: 1);
        var me = FakeCurrentUser.Teacher(teacher.Id);
        var request = new EnrollRequest { StudentId = students[0].Id, GroupId = group.Id };

        // Every step is a separate request with its own DbContext, like in the API
        int firstId;
        await using (var db = database.CreateContext())
            firstId = (await AppServices.Enrollments(db, me).EnrollAsync(request)).Id;

        await using (var db = database.CreateContext())
            await AppServices.Enrollments(db, me).CancelAsync(firstId);

        int secondId;
        await using (var db = database.CreateContext())
            secondId = (await AppServices.Enrollments(db, me).EnrollAsync(request)).Id;

        Assert.NotEqual(firstId, secondId);

        await using var check = database.CreateContext();
        var rows = await check.Enrollments.Where(e => e.GroupId == group.Id).ToListAsync();
        Assert.Equal(2, rows.Count);                        // the cancelled one is kept as history
        Assert.Single(rows, e => e.IsActive);
        Assert.Equal(1, (await check.Groups.SingleAsync(g => g.Id == group.Id)).EnrolledCount);
    }

    [Fact]
    public async Task EnrollTwiceWhileActive_SecondGetsAlreadyEnrolled()
    {
        var (teacher, group, students) = await TestSeed.TeacherWithGroupAsync(database, maxStudents: 10, enrolledCount: 0, studentsCount: 1);
        var me = FakeCurrentUser.Teacher(teacher.Id);
        var request = new EnrollRequest { StudentId = students[0].Id, GroupId = group.Id };

        await using (var db = database.CreateContext())
            await AppServices.Enrollments(db, me).EnrollAsync(request);

        await using var again = database.CreateContext();
        var ex = await Assert.ThrowsAsync<ConflictException>(() => AppServices.Enrollments(again, me).EnrollAsync(request));
        Assert.Equal(ErrorCodes.AlreadyEnrolled, ex.Code);
    }

    // The database itself still blocks two ACTIVE enrollments, even if the service check is skipped
    [Fact]
    public async Task Database_RejectsTwoActiveEnrollmentsOfSameStudentInSameGroup()
    {
        var (_, group, students) = await TestSeed.TeacherWithGroupAsync(database, maxStudents: 10, enrolledCount: 0, studentsCount: 1);

        await using var db = database.CreateContext();
        db.Enrollments.AddRange(NewActive(students[0].Id, group.Id), NewActive(students[0].Id, group.Id));

        await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
    }

    private static Enrollment NewActive(int studentId, int groupId) => new()
    {
        StudentId = studentId,
        GroupId = groupId,
        EnrolledAt = DateTime.UtcNow,
        AmountDue = 1200m,
        IsActive = true
    };
}
