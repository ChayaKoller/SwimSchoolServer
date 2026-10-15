using SwimSchool.Domain.Enums;

namespace SwimSchool.API.Development;

// TEMPORARY: removed in Stage 9
public class DevUserOptions
{
    public int UserId { get; set; }
    public UserRole Role { get; set; }
    public int OwningTeacherId { get; set; }
}
