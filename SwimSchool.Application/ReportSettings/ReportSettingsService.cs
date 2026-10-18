using SwimSchool.Application.Common.Exceptions;
using SwimSchool.Application.Common.Interfaces;
using SwimSchool.Application.ReportSettings.Dtos;
using SwimSchool.Application.Teachers;

namespace SwimSchool.Application.ReportSettings;

public class ReportSettingsService(
    IUserRepository users,
    IUnitOfWork unitOfWork,
    ICurrentUser me) : IReportSettingsService
{
    // Teacher and secretary: a secretary needs the defaults to pre-fill the form
    public async Task<ReportSettingsDto> GetAsync(CancellationToken ct = default)
    {
        var teacher = await users.GetByIdAsync(me.OwningTeacherId, ct)
            ?? throw new InvalidOperationException("Owning teacher not found");

        return ReportSettingsMapper.ToDto(teacher);
    }

    // Teacher only
    public async Task<ReportSettingsDto> UpdateAsync(UpdateReportSettingsRequest request, CancellationToken ct = default)
    {
        if (!me.IsTeacher)
            throw new ForbiddenException(ErrorCodes.Forbidden, ErrorCodes.ForbiddenMessage);

        var teacher = await users.FindByIdForUpdateAsync(me.UserId, ct)
            ?? throw new InvalidOperationException("Authenticated user not found");

        teacher.ReportSignature = string.IsNullOrWhiteSpace(request.ReportSignature)
            ? null
            : request.ReportSignature.Trim();
        teacher.DefaultGroupPrice = request.DefaultGroupPrice!.Value;
        teacher.DefaultIndividualPrice = request.DefaultIndividualPrice;
        teacher.DefaultTreatmentsCount = request.DefaultTreatmentsCount!.Value;

        await unitOfWork.SaveChangesAsync(ct);

        return ReportSettingsMapper.ToDto(teacher);
    }
}
