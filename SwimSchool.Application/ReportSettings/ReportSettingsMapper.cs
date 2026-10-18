using Riok.Mapperly.Abstractions;
using SwimSchool.Application.ReportSettings.Dtos;
using SwimSchool.Domain.Entities;

namespace SwimSchool.Application.ReportSettings;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class ReportSettingsMapper
{
    public static partial ReportSettingsDto ToDto(User teacher);
}
