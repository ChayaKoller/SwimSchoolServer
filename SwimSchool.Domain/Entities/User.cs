using SwimSchool.Domain.Common;
using SwimSchool.Domain.Enums;

namespace SwimSchool.Domain.Entities;

public class User : BaseEntity
{
    private string _passwordHash = string.Empty;

    public string IdentityNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string PasswordHash { get => _passwordHash; init => _passwordHash = value; }
    public string SecurityStamp { get; private set; } = NewStamp();
    public DateTime? LastLoginAt { get; set; }
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;

    // Teacher only. Printed at the bottom of every report; null = her full name.
    public string? ReportSignature { get; set; }

    // Teacher only. Pre-filled in the report form; the report keeps its own price.
    public decimal DefaultGroupPrice { get; set; } = 90m;
    public decimal? DefaultIndividualPrice { get; set; }

    // Teacher only. How many empty date rows the report form shows at first (the report itself may have 1-34).
    public int DefaultTreatmentsCount { get; set; } = 16;

    // Secretary's teacher
    public int? TeacherId { get; set; }
    public User? Teacher { get; set; }

    public ICollection<User> Secretaries { get; set; } = new List<User>();
    public ICollection<Group> Groups { get; set; } = new List<Group>();
    public ICollection<Student> Students { get; set; } = new List<Student>();

    public int OwningTeacherId => Role == UserRole.Teacher ? Id : TeacherId!.Value;

    // Needs Teacher loaded for a secretary
    public bool CanSignIn => IsActive && (Role == UserRole.Teacher || Teacher?.IsActive == true);

    // The only way to change a password
    public void SetPassword(string passwordHash)
    {
        _passwordHash = passwordHash;
        SecurityStamp = NewStamp();
    }

    private static string NewStamp() => Guid.NewGuid().ToString("N");
}
