using AutoMapper;
using SwimSchool.Application.Groups.Dtos;
using SwimSchool.Domain.Entities;

namespace SwimSchool.Application.Groups;

public class GroupProfile : Profile
{
    public GroupProfile()
    {
        CreateMap<Group, GroupDto>()
            .ForMember(d => d.FreeSpots, o => o.MapFrom(s => s.MaxStudents - s.EnrolledCount));

        CreateMap<Group, GroupDetailsDto>()
            .ForMember(d => d.FreeSpots, o => o.MapFrom(s => s.MaxStudents - s.EnrolledCount))
            .ForMember(d => d.Students, o => o.MapFrom(s => s.Enrollments.Where(e => e.IsActive)))
            .ForMember(d => d.Lessons, o => o.MapFrom(s => s.Lessons.OrderBy(l => l.StartTime)));

        // AmountOwed in this group only
        CreateMap<Enrollment, GroupStudentDto>()
            .ForMember(d => d.Id, o => o.MapFrom(s => s.StudentId))
            .ForMember(d => d.FullName, o => o.MapFrom(s => s.Student.FullName))
            .ForMember(d => d.AmountOwed, o => o.MapFrom(s => s.AmountDue - s.AmountPaid));

        CreateMap<CreateGroupRequest, Group>()
            .ForMember(d => d.Year, o => o.MapFrom(s => s.Year!.Value))
            .ForMember(d => d.Season, o => o.MapFrom(s => s.Season!.Value))
            .ForMember(d => d.LessonsCount, o => o.MapFrom(s => s.LessonTimes != null ? s.LessonTimes.Count : s.LessonsCount!.Value))
            .ForMember(d => d.Price, o => o.MapFrom(s => s.Price!.Value))
            .ForMember(d => d.MaxStudents, o => o.MapFrom(s => s.MaxStudents!.Value))
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.Ignore())
            .ForMember(d => d.IsActive, o => o.Ignore())
            .ForMember(d => d.EnrolledCount, o => o.Ignore())
            .ForMember(d => d.Version, o => o.Ignore())
            .ForMember(d => d.TeacherId, o => o.Ignore())
            .ForMember(d => d.Teacher, o => o.Ignore())
            .ForMember(d => d.Lessons, o => o.Ignore())
            .ForMember(d => d.Enrollments, o => o.Ignore());

        CreateMap<UpdateGroupRequest, Group>()
            .ForMember(d => d.LessonsCount, o => o.MapFrom(s => s.LessonsCount!.Value))
            .ForMember(d => d.Price, o => o.MapFrom(s => s.Price!.Value))
            .ForMember(d => d.MaxStudents, o => o.MapFrom(s => s.MaxStudents!.Value))
            .ForMember(d => d.IsActive, o => o.MapFrom(s => s.IsActive!.Value))
            .ForMember(d => d.Version, o => o.MapFrom(s => s.Version!.Value))
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.Ignore())
            .ForMember(d => d.Year, o => o.Ignore())
            .ForMember(d => d.Season, o => o.Ignore())
            .ForMember(d => d.EnrolledCount, o => o.Ignore())
            .ForMember(d => d.TeacherId, o => o.Ignore())
            .ForMember(d => d.Teacher, o => o.Ignore())
            .ForMember(d => d.Lessons, o => o.Ignore())
            .ForMember(d => d.Enrollments, o => o.Ignore());
    }
}
