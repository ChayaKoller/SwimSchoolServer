using SwimSchool.Domain.Common;

namespace SwimSchool.Domain.Entities;

public class Lesson : BaseEntity
{
    public int GroupId { get; set; }
    public Group Group { get; set; } = null!;

    // UTC
    public DateTime StartTime { get; set; }
    public bool TookPlace { get; set; }
    public bool IsMakeup { get; set; }

    public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
}
