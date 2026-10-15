using AutoMapper;
using SwimSchool.Application.Students.Dtos;
using SwimSchool.Domain.Entities;

namespace SwimSchool.Application.Students;

public class StudentProfile : Profile
{
    public StudentProfile()
    {
        CreateMap<Student, StudentListItemDto>()
            .ForMember(d => d.AmountOwed, o => o.MapFrom(s => s.Enrollments
                .Where(e => e.IsActive)
                .Sum(e => e.AmountDue - e.AmountPaid)))
            .ForMember(d => d.AbsencesCount, o => o.MapFrom(s => s.Enrollments
                .Where(e => e.IsActive)
                .SelectMany(e => e.Attendances)
                .Count(a => !a.WasPresent)));

        CreateMap<Student, StudentDetailsDto>()
            .ForMember(d => d.ResponsibleTeacherName, o => o.MapFrom(s => s.ResponsibleTeacher.FullName))
            .ForMember(d => d.Groups, o => o.MapFrom(s => s.Enrollments.Where(e => e.IsActive)));

        CreateMap<Enrollment, StudentGroupDto>()
            .ForMember(d => d.EnrollmentId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.GroupName, o => o.MapFrom(s => s.Group.Name))
            .ForMember(d => d.Season, o => o.MapFrom(s => s.Group.Season))
            .ForMember(d => d.Year, o => o.MapFrom(s => s.Group.Year));

        CreateMap<CreateStudentRequest, Student>()
            .ForMember(d => d.BirthDate, o => o.MapFrom(s => s.BirthDate!.Value))
            .ForMember(d => d.HealthFund, o => o.MapFrom(s => s.HealthFund!.Value))
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.Ignore())
            .ForMember(d => d.IsActive, o => o.Ignore())
            .ForMember(d => d.ResponsibleTeacherId, o => o.Ignore())
            .ForMember(d => d.ResponsibleTeacher, o => o.Ignore())
            .ForMember(d => d.Enrollments, o => o.Ignore())
            .ForMember(d => d.Reports, o => o.Ignore());

        CreateMap<UpdateStudentRequest, Student>()
            .ForMember(d => d.HealthFund, o => o.MapFrom(s => s.HealthFund!.Value))
            .ForMember(d => d.IsActive, o => o.MapFrom(s => s.IsActive!.Value))
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.Ignore())
            .ForMember(d => d.IdentityNumber, o => o.Ignore())
            .ForMember(d => d.BirthDate, o => o.Ignore())
            .ForMember(d => d.ResponsibleTeacherId, o => o.Ignore())
            .ForMember(d => d.ResponsibleTeacher, o => o.Ignore())
            .ForMember(d => d.Enrollments, o => o.Ignore())
            .ForMember(d => d.Reports, o => o.Ignore());
    }
}
