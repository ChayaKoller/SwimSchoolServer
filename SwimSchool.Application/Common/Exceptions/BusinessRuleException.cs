namespace SwimSchool.Application.Common.Exceptions;

// Mapped to 400 in the API
public class BusinessRuleException(string code, string message) : AppException(code, message);
