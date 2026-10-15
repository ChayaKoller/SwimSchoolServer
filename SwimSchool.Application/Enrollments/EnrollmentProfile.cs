using AutoMapper;
using SwimSchool.Application.Enrollments.Dtos;
using SwimSchool.Domain.Entities;

namespace SwimSchool.Application.Enrollments;

public class EnrollmentProfile : Profile
{
    public EnrollmentProfile()
    {
        CreateMap<Enrollment, EnrollmentDto>()
            .ForMember(d => d.StudentName, o => o.MapFrom(s => s.Student.FullName))
            .ForMember(d => d.GroupName, o => o.MapFrom(s => s.Group.Name));
    }
}
