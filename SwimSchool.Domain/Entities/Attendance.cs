using SwimSchool.Domain.Common;

namespace SwimSchool.Domain.Entities;

public class Attendance : BaseEntity
{
    public int LessonId { get; set; }
    public Lesson Lesson { get; set; } = null!;

    public int EnrollmentId { get; set; }
    public Enrollment Enrollment { get; set; } = null!;

    public bool WasPresent { get; set; }
}
