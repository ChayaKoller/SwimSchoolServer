using AutoMapper;
using SwimSchool.Application.ReportSettings.Dtos;
using SwimSchool.Domain.Entities;

namespace SwimSchool.Application.ReportSettings;

public class ReportSettingsProfile : Profile
{
    public ReportSettingsProfile()
    {
        CreateMap<User, ReportSettingsDto>().MaxDepth(64);
    }
}
