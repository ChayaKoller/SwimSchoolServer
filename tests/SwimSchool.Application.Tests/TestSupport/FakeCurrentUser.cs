using SwimSchool.Application.Common.Interfaces;
using SwimSchool.Domain.Enums;

namespace SwimSchool.Application.Tests.TestSupport;

// A hand-written fake instead of Moq: ICurrentUser.IsTeacher is a default interface member,
// and a Moq mock would not run it.
public sealed class FakeCurrentUser : ICurrentUser
{
    private FakeCurrentUser(int userId, UserRole role, int owningTeacherId)
    {
        UserId = userId;
        Role = role;
        OwningTeacherId = owningTeacherId;
    }

    public int UserId { get; }
    public UserRole Role { get; }
    public int OwningTeacherId { get; }

    public static FakeCurrentUser Teacher(int teacherId) => new(teacherId, UserRole.Teacher, teacherId);

    public static FakeCurrentUser SecretaryOf(int teacherId, int secretaryId = 900) =>
        new(secretaryId, UserRole.Secretary, teacherId);
}
