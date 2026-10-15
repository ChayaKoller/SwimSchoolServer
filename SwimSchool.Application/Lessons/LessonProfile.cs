using AutoMapper;
using SwimSchool.Application.Lessons.Dtos;
using SwimSchool.Domain.Entities;

namespace SwimSchool.Application.Lessons;

public class LessonProfile : Profile
{
    public LessonProfile()
    {
        CreateMap<Lesson, LessonDto>()
            .ForMember(d => d.LessonId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.GroupName, o => o.MapFrom(s => s.Group.Name));
    }
}
