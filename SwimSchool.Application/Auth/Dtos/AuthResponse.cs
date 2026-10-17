namespace SwimSchool.Application.Auth.Dtos;

public sealed record AuthResponse(string Token, DateTime ExpiresAt, CurrentUserDto User);
