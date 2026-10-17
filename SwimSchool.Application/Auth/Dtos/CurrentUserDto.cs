using SwimSchool.Domain.Enums;

namespace SwimSchool.Application.Auth.Dtos;

public sealed record CurrentUserDto(int Id, string FullName, UserRole Role);
