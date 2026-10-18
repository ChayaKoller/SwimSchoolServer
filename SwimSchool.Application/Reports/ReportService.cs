using AutoMapper;
using SwimSchool.Application.Common.Exceptions;
using SwimSchool.Application.Common.Interfaces;
using SwimSchool.Application.Common.Models;
using SwimSchool.Application.Reports.Dtos;
using SwimSchool.Application.Reports.Pdf;
using SwimSchool.Application.Students;
using SwimSchool.Domain.Entities;
using SwimSchool.Domain.Enums;

namespace SwimSchool.Application.Reports;

// Teachers and secretaries
public class ReportService(
    IStudentRepository students,
    IReportRepository reports,
    IUnitOfWork unitOfWork,
    ICurrentUser me,
    IDateTimeProvider clock,
    IMapper mapper,
    IEnumerable<IReportPdfGenerator> pdfGenerators) : IReportService
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
            IssuedAt = clock.UtcNow,
            CreatedById = me.UserId
        };
        ApplyDetails(report, request);

        reports.Add(report);
        await unitOfWork.SaveChangesAsync(ct);

        return await GetByIdAsync(report.Id, ct);
    }

    // Student, HealthFund, IssuedAt and CreatedById never change
    public async Task<ReportDto> UpdateAsync(int id, UpdateReportRequest request, CancellationToken ct = default)
    {
        var report = await reports.FindForUpdateAsync(id, me.OwningTeacherId, ct)
            ?? throw new NotFoundException(ErrorCodes.ReportNotFound, ErrorCodes.ReportNotFoundMessage);

        ApplyDetails(report, request);
        await unitOfWork.SaveChangesAsync(ct);

        return await GetByIdAsync(id, ct);
    }

    // Suggestion only: the report stores whatever dates the client sends.
    public async Task<IReadOnlyList<DateOnly>> GetAttendedDatesAsync(
        int studentId, AttendedDatesFilter filter, CancellationToken ct = default)
    {
        var student = await students.GetByIdAsync(studentId, me.OwningTeacherId, ct)
            ?? throw new NotFoundException(ErrorCodes.StudentNotFound, ErrorCodes.StudentNotFoundMessage);

        DateTime? fromUtc = filter.FromDate is DateOnly from
            ? clock.IsraelToUtc(from.ToDateTime(TimeOnly.MinValue))
            : null;

        // Exclusive: start of the day after ToDate, so the whole ToDate is included
        DateTime? toUtcExclusive = filter.ToDate is DateOnly to
            ? clock.IsraelToUtc(to.AddDays(1).ToDateTime(TimeOnly.MinValue))
            : null;

        var times = await reports.GetAttendedLessonTimesAsync(
            student.Id, me.OwningTeacherId, filter.GroupId, fromUtc, toUtcExclusive, ct);

        return times
            .Select(t => DateOnly.FromDateTime(clock.UtcToIsrael(t)))
            .Distinct()
            .Order()
            .ToList();
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

    public async Task<ReportPdfFile> GetPdfAsync(int id, CancellationToken ct = default)
    {
        var report = await reports.GetForPdfAsync(id, me.OwningTeacherId, ct)
            ?? throw new NotFoundException(ErrorCodes.ReportNotFound, ErrorCodes.ReportNotFoundMessage);

        // Signed by the teacher the report was created under (herself, or the creating secretary's teacher),
        // so an old report keeps its signature even after the student moves to another teacher.
        var teacher = report.CreatedBy.Role == UserRole.Teacher ? report.CreatedBy : report.CreatedBy.Teacher!;
        var data = new ReportPdfData(
            TeacherId: teacher.Id,
            IssuedOn: DateOnly.FromDateTime(clock.UtcToIsrael(report.IssuedAt)),
            ServiceDescription: report.ServiceDescription,
            StudentName: report.Student.FullName,
            StudentIdentityNumber: report.Student.IdentityNumber,
            TreatmentType: report.TreatmentType,
            PricePerTreatment: report.PricePerTreatment,
            TotalAmount: report.PricePerTreatment * report.LessonDates.Count,
            TreatmentDates: report.LessonDates,
            Signature: string.IsNullOrWhiteSpace(teacher.ReportSignature) ? teacher.FullName : teacher.ReportSignature);

        return new ReportPdfFile(PickGenerator(report).Generate(data), $"report-{report.Id}.pdf");
    }

    // A fund's own form if one exists; private insurance always uses the general form.
    private IReportPdfGenerator PickGenerator(Report report) =>
        (report.InsuranceCompany is null ? pdfGenerators.FirstOrDefault(g => g.Fund == report.HealthFund) : null)
        ?? pdfGenerators.Single(g => g.Fund is null);

    private static void ApplyDetails(Report report, ReportDetailsRequest request)
    {
        report.TreatmentType = request.TreatmentType!.Value;
        report.PricePerTreatment = request.PricePerTreatment!.Value;
        report.ServiceDescription = request.ServiceDescription.Trim();
        report.InsuranceCompany = string.IsNullOrWhiteSpace(request.InsuranceCompany)
            ? null
            : request.InsuranceCompany.Trim();
        report.LessonDates = request.LessonDates.Order().ToList();
    }
}
