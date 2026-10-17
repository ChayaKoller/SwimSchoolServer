namespace SwimSchool.Application.Teachers;

public sealed record UserSessionState(bool IsActive, bool TeacherIsActive, string SecurityStamp);
