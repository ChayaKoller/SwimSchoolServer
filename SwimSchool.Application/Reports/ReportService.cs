using AutoMapper;
using SwimSchool.Application.Common.Exceptions;
using SwimSchool.Application.Common.Interfaces;
using SwimSchool.Application.Common.Models;
using SwimSchool.Application.Reports.Dtos;
using SwimSchool.Application.Students;
using SwimSchool.Domain.Entities;

namespace SwimSchool.Application.Reports;

// Teachers and secretaries
public class ReportService(
    IStudentRepository students,
    IReportRepository reports,
    IUnitOfWork unitOfWork,
    ICurrentUser me,
    IDateTimeProvider clock,
    IMapper mapper) : IReportService
{
    // Reports follow the student: ownership is the student's current responsible teacher
    public async Task<PagedResult<ReportDto>> GetAsync(ReportFilter filter, CancellationToken ct = default)
    {
        DateTime? fromUtc = filter.FromDate is DateOnly from
            ? clock.IsraelToUtc(from.ToDateTime(TimeOnly.MinValue))
            : null;

        // Exclusive: start of the day after ToDate, so the whole ToDate is included
        DateTime? toUtc = filter.ToDate is DateOnly to
            ? clock.IsraelToUtc(to.AddDays(1).ToDateTime(TimeOnly.MinValue))
            : null;

        var page = await reports.GetAsync(me.OwningTeacherId, filter, fromUtc, toUtc, ct);

        return new PagedResult<ReportDto>
        {
            Items = mapper.Map<List<ReportDto>>(page.Items),
            TotalCount = page.TotalCount,
            Page = page.Page,
            PageSize = page.PageSize
        };
    }

    public async Task<ReportDto> CreateAsync(CreateReportRequest request, CancellationToken ct = default)
    {
        var student = await students.GetByIdAsync(request.StudentId!.Value, me.OwningTeacherId, ct)
            ?? throw new NotFoundException(ErrorCodes.StudentNotFound, ErrorCodes.StudentNotFoundMessage);

        // Any health fund; the PDF is generated in Stage 10
        var report = new Report
        {
            StudentId = student.Id,
            HealthFund = student.HealthFund,
            LessonDates = request.LessonDates.Order().ToList(),
            IssuedAt = clock.UtcNow,
            CreatedById = me.UserId
        };

        reports.Add(report);
        await unitOfWork.SaveChangesAsync(ct);

        return await GetByIdAsync(report.Id, ct);
    }

    // Unknown or foreign student returns an empty list
    public async Task<List<ReportDto>> GetForStudentAsync(int studentId, CancellationToken ct = default)
    {
        var list = await reports.GetForStudentAsync(studentId, me.OwningTeacherId, ct);

        return mapper.Map<List<ReportDto>>(list);
    }

    public async Task<ReportDto> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var report = await reports.GetByIdAsync(id, me.OwningTeacherId, ct)
            ?? throw new NotFoundException(ErrorCodes.ReportNotFound, ErrorCodes.ReportNotFoundMessage);

        return mapper.Map<ReportDto>(report);
    }
}
