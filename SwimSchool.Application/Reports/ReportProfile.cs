using AutoMapper;
using SwimSchool.Application.Reports.Dtos;
using SwimSchool.Domain.Entities;

namespace SwimSchool.Application.Reports;

public class ReportProfile : Profile
{
    public ReportProfile()
    {
        CreateMap<Report, ReportDto>()
            .ForMember(d => d.StudentName, o => o.MapFrom(s => s.Student.FullName))
            .ForMember(d => d.CreatedByName, o => o.MapFrom(s => s.CreatedBy.FullName))
            .ForMember(d => d.TotalAmount, o => o.MapFrom(s => s.PricePerTreatment * s.LessonDates.Count))
            .MaxDepth(64);
    }
}
