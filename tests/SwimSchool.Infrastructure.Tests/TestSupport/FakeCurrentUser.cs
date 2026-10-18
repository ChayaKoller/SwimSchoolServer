using SwimSchool.Application.Common.Interfaces;
using SwimSchool.Domain.Enums;

namespace SwimSchool.Infrastructure.Tests.TestSupport;

public sealed class FakeCurrentUser(int userId, UserRole role, int owningTeacherId) : ICurrentUser
{
    public int UserId { get; } = userId;
    public UserRole Role { get; } = role;
    public int OwningTeacherId { get; } = owningTeacherId;

    public static FakeCurrentUser Teacher(int teacherId) => new(teacherId, UserRole.Teacher, teacherId);

    // The secretary's own id does not matter here; she works on her teacher's data
    public static FakeCurrentUser SecretaryOf(int teacherId) => new(-1, UserRole.Secretary, teacherId);
}
