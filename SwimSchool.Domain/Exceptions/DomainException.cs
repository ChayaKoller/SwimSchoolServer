namespace SwimSchool.Domain.Exceptions;

// Entity rule broken, mapped to 409 in the API
public class DomainException(string message) : Exception(message);
