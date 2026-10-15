using SwimSchool.Domain.Enums;

namespace SwimSchool.Application.Common.Interfaces;

public interface ICurrentUser
{
    int UserId { get; }
    UserRole Role { get; }
    int OwningTeacherId { get; }
    bool IsTeacher => Role == UserRole.Teacher;
}
