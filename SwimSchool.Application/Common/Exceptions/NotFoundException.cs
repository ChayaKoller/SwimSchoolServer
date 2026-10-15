namespace SwimSchool.Application.Common.Exceptions;

// Mapped to 404 in the API
public class NotFoundException(string code, string message) : AppException(code, message);
