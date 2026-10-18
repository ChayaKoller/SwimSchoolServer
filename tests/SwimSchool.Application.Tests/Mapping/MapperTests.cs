using SwimSchool.Application.Groups;
using SwimSchool.Application.Groups.Dtos;
using SwimSchool.Application.Reports;
using SwimSchool.Application.Students;
using SwimSchool.Domain.Entities;
using SwimSchool.Domain.Enums;
using AttendanceEntity = SwimSchool.Domain.Entities.Attendance;

namespace SwimSchool.Application.Tests.Mapping;

// Mapperly checks at build time that every DTO field is filled; these check the calculated fields
public class MapperTests
{
    [Fact]
    public void GroupDetails_FreeSpotsAndOnlyActiveStudents()
    {
        var group = new Group { MaxStudents = 8, EnrolledCount = 3 };
        group.Enrollments.Add(Enrollment(1, "Active", amountDue: 500, amountPaid: 200, isActive: true));
        group.Enrollments.Add(Enrollment(2, "Cancelled", amountDue: 500, amountPaid: 0, isActive: false));

        var dto = GroupMapper.ToDetailsDto(group);

        Assert.Equal(5, dto.FreeSpots);
        var student = Assert.Single(dto.Students);
        Assert.Equal(1, student.Id);
        Assert.Equal("Active", student.FullName);
        Assert.Equal(300, student.AmountOwed);
    }

    [Fact]
    public void GroupDetails_LessonsOrderedByTime()
    {
        var group = new Group { Name = "A" };
        group.Lessons.Add(new Lesson { Id = 2, StartTime = new DateTime(2026, 11, 8, 0, 0, 0, DateTimeKind.Utc), Group = group });
        group.Lessons.Add(new Lesson { Id = 1, StartTime = new DateTime(2026, 11, 1, 0, 0, 0, DateTimeKind.Utc), Group = group });

        var dto = GroupMapper.ToDetailsDto(group);

        Assert.Equal([1, 2], dto.Lessons.Select(l => l.LessonId));
        Assert.All(dto.Lessons, l => Assert.Equal("A", l.GroupName));
    }

    [Fact]
    public void UpdateGroup_KeepsYearSeasonAndTeacher()
    {
        var group = new Group { Year = 2026, Season = Season.Winter, TeacherId = 7, EnrolledCount = 4 };
        var request = new UpdateGroupRequest
        {
            Name = "New", LessonsCount = 12, Price = 600, MaxStudents = 10, IsActive = true, Version = 5
        };

        GroupMapper.UpdateEntity(request, group);

        Assert.Equal("New", group.Name);
        Assert.Equal(600, group.Price);
        Assert.Equal(5u, group.Version);
        Assert.Equal(2026, group.Year);
        Assert.Equal(Season.Winter, group.Season);
        Assert.Equal(7, group.TeacherId);
        Assert.Equal(4, group.EnrolledCount);
    }

    [Fact]
    public void Report_TotalAndNames()
    {
        var report = new Report
        {
            Student = new Student { FullName = "Student" },
            CreatedBy = new User { FullName = "Teacher" },
            PricePerTreatment = 90,
            LessonDates = [new DateOnly(2026, 10, 1), new DateOnly(2026, 10, 8), new DateOnly(2026, 10, 15)]
        };

        var dto = ReportMapper.ToDto(report);

        Assert.Equal(270, dto.TotalAmount);
        Assert.Equal("Student", dto.StudentName);
        Assert.Equal("Teacher", dto.CreatedByName);
    }

    [Fact]
    public void StudentListItem_CountsOnlyActiveEnrollments()
    {
        var student = new Student();
        var active = Enrollment(1, "S", amountDue: 400, amountPaid: 100, isActive: true);
        active.Attendances.Add(new AttendanceEntity { WasPresent = false });
        active.Attendances.Add(new AttendanceEntity { WasPresent = true });
        var cancelled = Enrollment(1, "S", amountDue: 400, amountPaid: 0, isActive: false);
        cancelled.Attendances.Add(new AttendanceEntity { WasPresent = false });
        student.Enrollments.Add(active);
        student.Enrollments.Add(cancelled);

        var dto = StudentMapper.ToListItemDto(student);

        Assert.Equal(300, dto.AmountOwed);
        Assert.Equal(1, dto.AbsencesCount);
    }

    private static Enrollment Enrollment(int studentId, string name, decimal amountDue, decimal amountPaid, bool isActive) => new()
    {
        StudentId = studentId,
        Student = new Student { Id = studentId, FullName = name },
        AmountDue = amountDue,
        AmountPaid = amountPaid,
        IsActive = isActive
    };
}
