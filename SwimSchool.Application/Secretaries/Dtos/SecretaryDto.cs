namespace SwimSchool.Application.Secretaries.Dtos;

public sealed record SecretaryDto(int Id, string FullName, string Email, string Phone, bool IsActive, DateTime? LastLoginAt);
