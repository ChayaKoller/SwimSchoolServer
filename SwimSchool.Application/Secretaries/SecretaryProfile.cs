using AutoMapper;
using SwimSchool.Application.Secretaries.Dtos;
using SwimSchool.Domain.Entities;

namespace SwimSchool.Application.Secretaries;

public class SecretaryProfile : Profile
{
    public SecretaryProfile()
    {
        CreateMap<User, SecretaryDto>().MaxDepth(64);
    }
}
