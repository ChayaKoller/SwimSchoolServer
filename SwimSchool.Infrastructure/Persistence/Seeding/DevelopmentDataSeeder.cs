using Microsoft.EntityFrameworkCore;
using SwimSchool.Domain.Entities;
using SwimSchool.Domain.Enums;

namespace SwimSchool.Infrastructure.Persistence.Seeding;

public static class DevelopmentDataSeeder
{
    // Replaced with a real hash in Stage 9
    private const string TempPasswordHash = "TEMP";

    private static readonly int[] LessonDayOffsets = [1, 4, 7, 10, 13];

    public static async Task SeedAsync(SwimSchoolDbContext db, CancellationToken cancellationToken = default)
    {
        if (await db.Users.AnyAsync(cancellationToken))
            return;

        var now = DateTime.UtcNow;
        var today = now.Date;

        var teacher1 = CreateUser("100000001", "Dana Levi", "dana.levi@example.com", "050-1000001", UserRole.Teacher);
        var teacher2 = CreateUser("100000002", "Noa Cohen", "noa.cohen@example.com", "050-1000002", UserRole.Teacher);

        var secretary1 = CreateUser("100000003", "Yael Mizrahi", "yael.mizrahi@example.com", "050-1000003", UserRole.Secretary);
        secretary1.Teacher = teacher1;

        var secretary2 = CreateUser("100000004", "Tamar Peretz", "tamar.peretz@example.com", "050-1000004", UserRole.Secretary);
        secretary2.Teacher = teacher2;

        var beginnersA = CreateGroup("Beginners A", teacher1, maxStudents: 2, price: 450m, today.Year);
        var advanced = CreateGroup("Advanced", teacher1, maxStudents: 10, price: 600m, today.Year);
        var intermediate = CreateGroup("Intermediate", teacher2, maxStudents: 8, price: 520m, today.Year);

        var student1 = CreateStudent("200000001", "Avi Shalom", HealthFund.Maccabi, teacher1, new DateOnly(2015, 3, 12));
        var student2 = CreateStudent("200000002", "Maya Golan", HealthFund.Clalit, teacher1, new DateOnly(2016, 7, 5));
        var student3 = CreateStudent("200000003", "Eitan Bar", HealthFund.Maccabi, teacher1, new DateOnly(2012, 11, 20));
        var student4 = CreateStudent("200000004", "Shira Katz", HealthFund.Meuhedet, teacher1, new DateOnly(2013, 1, 30));
        var student5 = CreateStudent("200000005", "Omer Azulay", HealthFund.Maccabi, teacher2, new DateOnly(2014, 5, 18));
        var student6 = CreateStudent("200000006", "Lia Friedman", HealthFund.Maccabi, teacher2, new DateOnly(2014, 9, 9));

        var enrollments = new List<Enrollment>
        {
            Enroll(student1, beginnersA, amountPaid: 450m, now),
            Enroll(student2, beginnersA, amountPaid: 200m, now),
            Enroll(student3, advanced, amountPaid: 600m, now),
            Enroll(student4, advanced, amountPaid: 300m, now),
            Enroll(student5, intermediate, amountPaid: 520m, now),
            Enroll(student6, intermediate, amountPaid: 0m, now)
        };

        foreach (var group in new[] { beginnersA, advanced, intermediate })
        {
            foreach (var offset in LessonDayOffsets)
            {
                group.Lessons.Add(new Lesson
                {
                    StartTime = today.AddDays(offset).AddHours(15),
                    TookPlace = false,
                    IsMakeup = false
                });
            }
        }

        // student3 is absent from the first 3 lessons of "Advanced"
        foreach (var enrollment in enrollments)
        {
            var lessonIndex = 0;
            foreach (var lesson in enrollment.Group.Lessons)
            {
                var wasPresent = !(enrollment.Student == student3 && lessonIndex < 3);
                enrollment.Attendances.Add(new Attendance { Lesson = lesson, WasPresent = wasPresent });
                lessonIndex++;
            }
        }

        db.Users.AddRange(teacher1, teacher2, secretary1, secretary2);
        db.Groups.AddRange(beginnersA, advanced, intermediate);
        db.Students.AddRange(student1, student2, student3, student4, student5, student6);
        db.Enrollments.AddRange(enrollments);

        await db.SaveChangesAsync(cancellationToken);
    }

    private static User CreateUser(string identityNumber, string fullName, string email, string phone, UserRole role) => new()
    {
        IdentityNumber = identityNumber,
        FullName = fullName,
        Email = email,
        Phone = phone,
        PasswordHash = TempPasswordHash,
        Role = role
    };

    private static Group CreateGroup(string name, User teacher, int maxStudents, decimal price, int year) => new()
    {
        Name = name,
        Teacher = teacher,
        MaxStudents = maxStudents,
        Price = price,
        Year = year,
        Season = Season.Autumn,
        LessonsCount = LessonDayOffsets.Length
    };

    private static Student CreateStudent(string identityNumber, string fullName, HealthFund healthFund, User teacher, DateOnly birthDate) => new()
    {
        IdentityNumber = identityNumber,
        FullName = fullName,
        Phone = $"052-{identityNumber[^7..]}",
        HealthFund = healthFund,
        BirthDate = birthDate,
        ResponsibleTeacher = teacher
    };

    private static Enrollment Enroll(Student student, Group group, decimal amountPaid, DateTime enrolledAt)
    {
        if (student.ResponsibleTeacher != group.Teacher)
            throw new InvalidOperationException("A student can only be enrolled in a group of their own teacher.");

        group.ReserveSpot();

        return new Enrollment
        {
            Student = student,
            Group = group,
            EnrolledAt = enrolledAt,
            AmountDue = group.Price,
            AmountPaid = amountPaid
        };
    }
}
