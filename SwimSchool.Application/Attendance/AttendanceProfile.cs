using AutoMapper;
using SwimSchool.Application.Attendance.Dtos;
using SwimSchool.Domain.Entities;

namespace SwimSchool.Application.Attendance;

// GroupAttendanceGridDto is built in the service, not here
public class AttendanceProfile : Profile
{
    public AttendanceProfile()
    {
        CreateMap<Lesson, LessonAttendanceDto>()
            .ForMember(d => d.LessonId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.GroupName, o => o.MapFrom(s => s.Group.Name))
            .ForMember(d => d.Students, o => o.MapFrom(s => s.Group.Enrollments
                .Where(e => e.IsActive)
                .Select(e => new LessonAttendanceStudentDto
                {
                    EnrollmentId = e.Id,
                    StudentId = e.StudentId,
                    FullName = e.Student.FullName,
                    WasPresent = e.Attendances
                        .Where(a => a.LessonId == s.Id)
                        .Select(a => (bool?)a.WasPresent)
                        .FirstOrDefault()
                })
                .ToList()));
    }
}
