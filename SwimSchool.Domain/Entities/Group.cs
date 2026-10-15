using SwimSchool.Domain.Common;
using SwimSchool.Domain.Enums;
using SwimSchool.Domain.Exceptions;

namespace SwimSchool.Domain.Entities;

public class Group : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public int Year { get; set; }
    public Season Season { get; set; }
    public int LessonsCount { get; set; }
    public bool IsActive { get; set; } = true;
    public decimal Price { get; set; }
    public int MaxStudents { get; set; }
    public int EnrolledCount { get; set; }

    // Concurrency token, mapped to xmin in Infrastructure
    public uint Version { get; set; }

    public int TeacherId { get; set; }
    public User Teacher { get; set; } = null!;

    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public bool HasFreeSpot() => IsActive && EnrolledCount < MaxStudents;

    public void ReserveSpot()
    {
        if (!IsActive)
            throw new DomainException("The group is not active.");

        if (EnrolledCount >= MaxStudents)
            throw new DomainException("The group is full.");

        EnrolledCount++;
    }

    public void ReleaseSpot()
    {
        if (EnrolledCount > 0)
            EnrolledCount--;
    }
}
