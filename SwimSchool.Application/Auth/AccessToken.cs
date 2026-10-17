namespace SwimSchool.Application.Auth;

public sealed record AccessToken(string Token, DateTime ExpiresAt);
