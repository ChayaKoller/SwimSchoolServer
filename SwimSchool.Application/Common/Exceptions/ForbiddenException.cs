namespace SwimSchool.Application.Common.Exceptions;

// Mapped to 403 in the API
public class ForbiddenException(string code, string message) : AppException(code, message);
