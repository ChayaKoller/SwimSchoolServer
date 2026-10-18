using Riok.Mapperly.Abstractions;
using SwimSchool.Application.Reports.Dtos;
using SwimSchool.Domain.Entities;

namespace SwimSchool.Application.Reports;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class ReportMapper
{
    [MapPropertyFromSource(nameof(ReportDto.StudentName), Use = nameof(StudentName))]
    [MapPropertyFromSource(nameof(ReportDto.CreatedByName), Use = nameof(CreatedByName))]
    [MapPropertyFromSource(nameof(ReportDto.TotalAmount), Use = nameof(TotalAmount))]
    public static partial ReportDto ToDto(Report report);

    public static partial List<ReportDto> ToDtos(IEnumerable<Report> reports);

    private static decimal TotalAmount(Report report) => report.PricePerTreatment * report.LessonDates.Count;

    // A navigation that was not loaded gives an empty value, not a crash (as AutoMapper did)
    private static string StudentName(Report report) => report.Student?.FullName ?? string.Empty;

    private static string CreatedByName(Report report) => report.CreatedBy?.FullName ?? string.Empty;
}
