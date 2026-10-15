using AutoMapper;
using SwimSchool.Application.Teachers.Dtos;
using SwimSchool.Domain.Entities;

namespace SwimSchool.Application.Teachers;

public class TeacherProfile : Profile
{
    public TeacherProfile()
    {
        CreateMap<User, TeacherOptionDto>();
    }
}
