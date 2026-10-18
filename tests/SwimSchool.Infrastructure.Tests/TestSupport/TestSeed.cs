using SwimSchool.Domain.Entities;
using SwimSchool.Domain.Enums;
using SwimSchool.Infrastructure.Persistence;

namespace SwimSchool.Infrastructure.Tests.TestSupport;

// Every test creates its own teacher, group and students, so tests never depend on each other or on demo data
public static class TestSeed
{
    private static int _next = 100_000_000;

    // Unique 9-digit numbers for the unique indexes (identity number, email)
    private static string NextNumber() => Interlocked.Increment(ref _next).ToString();

    public static async Task<(User teacher, Group group, List<Student> students)> TeacherWithGroupAsync(
        PostgresDatabase database, int maxStudents, int enrolledCount, int studentsCount)
    {
        await using var db = database.CreateContext();

        var number = NextNumber();
        var teacher = new User
        {
            IdentityNumber = number,
            FullName = "מורה בדיקה",
            Email = $"teacher{number}@test.local",
            Phone = "0500000000",
            Role = UserRole.Teacher,
            PasswordHash = "not-a-real-hash"
        };

        var group = new Group
        {
            Name = "קבוצת בדיקה",
            Year = 2026,
            Season = Season.Autumn,
            LessonsCount = 12,
            Price = 1200m,
            MaxStudents = maxStudents,
            EnrolledCount = enrolledCount,
            Teacher = teacher
        };

        var students = Enumerable.Range(0, studentsCount).Select(_ => new Student
        {
            IdentityNumber = NextNumber(),
            FullName = "תלמידת בדיקה",
            Phone = "0500000000",
            BirthDate = new DateOnly(2015, 1, 1),
            HealthFund = HealthFund.Maccabi,
            ResponsibleTeacher = teacher
        }).ToList();

        db.AddRange(teacher, group);
        db.AddRange(students);
        await db.SaveChangesAsync();

        return (teacher, group, students);
    }
}
