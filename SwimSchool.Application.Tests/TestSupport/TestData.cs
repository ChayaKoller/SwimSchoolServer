using SwimSchool.Domain.Entities;
using SwimSchool.Domain.Enums;

namespace SwimSchool.Application.Tests.TestSupport;

// Small, valid entities. Each test changes only what it is about.
public static class TestData
{
    public const int TeacherId = 1;
    public const int OtherTeacherId = 2;

    public static Group Group(int maxStudents = 10, int enrolledCount = 0, decimal price = 1200m, bool isActive = true) => new()
    {
        Id = 10,
        Name = "מתחילות א",
        Year = 2026,
        Season = Season.Autumn,
        LessonsCount = 12,
        IsActive = isActive,
        Price = price,
        MaxStudents = maxStudents,
        EnrolledCount = enrolledCount,
        Version = 1,
        TeacherId = TeacherId
    };

    public static Student Student(int id = 20, int teacherId = TeacherId) => new()
    {
        Id = id,
        IdentityNumber = "123456782",
        FullName = "נועה כהן",
        Phone = "0501234567",
        BirthDate = new DateOnly(2015, 3, 1),
        HealthFund = HealthFund.Maccabi,
        ResponsibleTeacherId = teacherId
    };

    public static Enrollment Enrollment(Group group, Student student, decimal amountDue = 1200m, bool isActive = true) => new()
    {
        Id = 30,
        Group = group,
        GroupId = group.Id,
        Student = student,
        StudentId = student.Id,
        AmountDue = amountDue,
        IsActive = isActive
    };

    public static User Teacher(int id = TeacherId, bool isActive = true) => new()
    {
        Id = id,
        FullName = "רחל לוי",
        Email = "rachel@example.com",
        Phone = "0507654321",
        IdentityNumber = "111111118",
        Role = UserRole.Teacher,
        IsActive = isActive,
        PasswordHash = "HASH"
    };

    public static User Secretary(User teacher, bool isActive = true) => new()
    {
        Id = 50,
        FullName = "דינה מזכירה",
        Email = "dina@example.com",
        Phone = "0521111111",
        IdentityNumber = "222222226",
        Role = UserRole.Secretary,
        IsActive = isActive,
        TeacherId = teacher.Id,
        Teacher = teacher,
        PasswordHash = "HASH"
    };
}
