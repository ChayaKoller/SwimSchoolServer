namespace SwimSchool.Application.Common.Exceptions;

// Mapped to 409 in the API
public class ConflictException(string message) : Exception(message);
