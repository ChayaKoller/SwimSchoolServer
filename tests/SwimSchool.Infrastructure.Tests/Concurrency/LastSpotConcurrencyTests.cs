using Microsoft.EntityFrameworkCore;
using SwimSchool.Application.Common.Exceptions;
using SwimSchool.Application.Enrollments.Dtos;
using SwimSchool.Infrastructure.Persistence;
using SwimSchool.Infrastructure.Tests.TestSupport;

namespace SwimSchool.Infrastructure.Tests.Concurrency;

// The proof that optimistic concurrency (Group.Version = PostgreSQL xmin) protects the last spot.
// Every "user" has her own DbContext, exactly like two requests from two computers.
[Collection(PostgresCollection.Name)]
public class LastSpotConcurrencyTests(PostgresDatabase database)
{
    // 1. The guidelines' test: two DbContexts read the same group, both change it,
    //    the first save succeeds and the second gets DbUpdateConcurrencyException.
    [Fact]
    public async Task TwoContexts_ChangeSameGroup_FirstSaves_SecondThrowsConcurrencyException()
    {
        var (_, seeded, _) = await TestSeed.TeacherWithGroupAsync(database, maxStudents: 10, enrolledCount: 9, studentsCount: 0);

        await using var teacherDb = database.CreateContext();
        await using var secretaryDb = database.CreateContext();
        var groupForTeacher = await teacherDb.Groups.SingleAsync(g => g.Id == seeded.Id);
        var groupForSecretary = await secretaryDb.Groups.SingleAsync(g => g.Id == seeded.Id);
        Assert.Equal(groupForTeacher.Version, groupForSecretary.Version);   // both read the same version

        groupForTeacher.ReserveSpot();
        groupForSecretary.ReserveSpot();   // still sees a free spot: it does not know about the teacher yet

        await teacherDb.SaveChangesAsync();
        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => secretaryDb.SaveChangesAsync());

        Assert.Equal(10, await EnrolledCountAsync(seeded.Id));
    }

    // 2. The same conflict through UnitOfWork: Infrastructure turns the EF exception into DATA_CHANGED,
    //    so Application never sees EF Core.
    [Fact]
    public async Task TwoContexts_ThroughUnitOfWork_SecondThrowsDataChanged()
    {
        var (_, seeded, _) = await TestSeed.TeacherWithGroupAsync(database, maxStudents: 10, enrolledCount: 9, studentsCount: 0);

        await using var teacherDb = database.CreateContext();
        await using var secretaryDb = database.CreateContext();
        (await teacherDb.Groups.SingleAsync(g => g.Id == seeded.Id)).ReserveSpot();
        (await secretaryDb.Groups.SingleAsync(g => g.Id == seeded.Id)).ReserveSpot();

        await new UnitOfWork(teacherDb).SaveChangesAsync();
        var ex = await Assert.ThrowsAsync<ConflictException>(() => new UnitOfWork(secretaryDb).SaveChangesAsync());

        Assert.Equal(ErrorCodes.DataChanged, ex.Code);
    }

    // 3. The full flow: the teacher and her secretary enroll two different students into the LAST spot.
    //    Both pass the "free spot" check, only one is saved, the other gets SPOT_TAKEN (409).
    [Fact]
    public async Task LastSpot_TeacherAndSecretaryEnrollTogether_OnlyOneSucceeds_OtherGetsSpotTaken()
    {
        var (teacher, group, students) = await TestSeed.TeacherWithGroupAsync(database, maxStudents: 10, enrolledCount: 9, studentsCount: 2);

        await using var teacherDb = database.CreateContext();
        await using var secretaryDb = database.CreateContext();
        var asTeacher = AppServices.Enrollments(teacherDb, FakeCurrentUser.Teacher(teacher.Id));
        var asSecretary = AppServices.Enrollments(secretaryDb, FakeCurrentUser.SecretaryOf(teacher.Id));

        // The secretary loads the group first (it still has a free spot) ...
        var secretaryGroup = await secretaryDb.Groups.SingleAsync(g => g.Id == group.Id);
        Assert.True(secretaryGroup.HasFreeSpot());

        // ... the teacher takes the last spot ...
        await asTeacher.EnrollAsync(new EnrollRequest { StudentId = students[0].Id, GroupId = group.Id });

        // ... and the secretary's enrollment, based on what she loaded, is rejected
        var ex = await Assert.ThrowsAsync<ConflictException>(() =>
            asSecretary.EnrollAsync(new EnrollRequest { StudentId = students[1].Id, GroupId = group.Id }));
        Assert.Equal(ErrorCodes.SpotTaken, ex.Code);

        Assert.Equal(10, await EnrolledCountAsync(group.Id));
        Assert.Equal(1, await ActiveEnrollmentsAsync(group.Id));
    }

    // 4. A real race: 8 users press "enroll" at the same moment for 3 free spots.
    //    There is no automatic retry, so a user who loses the race gets SPOT_TAKEN and tries again herself.
    //    What must ALWAYS hold: the group never goes over its limit, and the counter matches the real enrollments.
    [Fact]
    public async Task ManyUsersAtOnce_GroupNeverGoesOverItsLimit()
    {
        const int maxStudents = 10, alreadyEnrolled = 7, users = 8;
        var (teacher, group, students) = await TestSeed.TeacherWithGroupAsync(
            database, maxStudents, alreadyEnrolled, studentsCount: users);

        var attempts = students.Select(async student =>
        {
            await using var db = database.CreateContext();
            var service = AppServices.Enrollments(db, FakeCurrentUser.Teacher(teacher.Id));
            try
            {
                await service.EnrollAsync(new EnrollRequest { StudentId = student.Id, GroupId = group.Id });
                return "OK";
            }
            catch (ConflictException ex)
            {
                return ex.Code;   // SPOT_TAKEN (lost the race) or GROUP_FULL (came after the group filled up)
            }
        });
        var results = await Task.WhenAll(attempts);
        var succeeded = results.Count(r => r == "OK");

        Assert.InRange(succeeded, 1, maxStudents - alreadyEnrolled);
        Assert.All(results.Where(r => r != "OK"), code => Assert.Contains(code, new[] { ErrorCodes.SpotTaken, ErrorCodes.GroupFull }));
        Assert.Equal(alreadyEnrolled + succeeded, await EnrolledCountAsync(group.Id));
        Assert.Equal(succeeded, await ActiveEnrollmentsAsync(group.Id));
    }

    private async Task<int> EnrolledCountAsync(int groupId)
    {
        await using var db = database.CreateContext();
        return await db.Groups.Where(g => g.Id == groupId).Select(g => g.EnrolledCount).SingleAsync();
    }

    private async Task<int> ActiveEnrollmentsAsync(int groupId)
    {
        await using var db = database.CreateContext();
        return await db.Enrollments.CountAsync(e => e.GroupId == groupId && e.IsActive);
    }
}
