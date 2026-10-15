namespace SwimSchool.Application.Common.Exceptions;

// Mapped to 409 in the API
public class ConflictException(string code, string message) : AppException(code, message);
