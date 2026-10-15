using SwimSchool.Domain.Common;
using SwimSchool.Domain.Enums;

namespace SwimSchool.Domain.Entities;

public class User : BaseEntity
{
    public string IdentityNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;

    // Secretary's teacher
    public int? TeacherId { get; set; }
    public User? Teacher { get; set; }

    public ICollection<User> Secretaries { get; set; } = new List<User>();
    public ICollection<Group> Groups { get; set; } = new List<Group>();
    public ICollection<Student> Students { get; set; } = new List<Student>();

    public int OwningTeacherId => Role == UserRole.Teacher ? Id : TeacherId!.Value;
}
