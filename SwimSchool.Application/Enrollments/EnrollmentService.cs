using AutoMapper;
using SwimSchool.Application.Common.Exceptions;
using SwimSchool.Application.Common.Interfaces;
using SwimSchool.Application.Enrollments.Dtos;
using SwimSchool.Application.Groups;
using SwimSchool.Application.Students;
using SwimSchool.Domain.Entities;

namespace SwimSchool.Application.Enrollments;

public class EnrollmentService(
    IStudentRepository students,
    IGroupRepository groups,
    IEnrollmentRepository enrollments,
    IUnitOfWork unitOfWork,
    ICurrentUser me,
    IDateTimeProvider clock,
    IMapper mapper) : IEnrollmentService
{
    public async Task<EnrollmentDto> EnrollAsync(EnrollRequest request, CancellationToken ct = default)
    {
        var student = await students.FindForUpdateAsync(request.StudentId!.Value, me.OwningTeacherId, ct)
            ?? throw new NotFoundException(ErrorCodes.StudentNotFound, ErrorCodes.StudentNotFoundMessage);

        var group = await groups.FindForUpdateAsync(request.GroupId!.Value, me.OwningTeacherId, ct)
            ?? throw new NotFoundException(ErrorCodes.GroupNotFound, ErrorCodes.GroupNotFoundMessage);

        if (!group.IsActive)
            throw new ConflictException(ErrorCodes.GroupInactive, ErrorCodes.GroupInactiveMessage);

        if (await enrollments.IsActiveAsync(student.Id, group.Id, ct))
            throw new ConflictException(ErrorCodes.AlreadyEnrolled, ErrorCodes.AlreadyEnrolledMessage);

        if (!group.HasFreeSpot())
            throw new ConflictException(ErrorCodes.GroupFull, ErrorCodes.GroupFullMessage);

        group.ReserveSpot();

        // The entered amount is the real one; the group price is only a default
        var enrollment = new Enrollment
        {
            Student = student,
            Group = group,
            EnrolledAt = clock.UtcNow,
            AmountDue = request.AmountDue ?? group.Price,
            IsActive = true
        };

        enrollments.Add(enrollment);

        try
        {
            await unitOfWork.SaveChangesAsync(ct);
        }
        catch (ConflictException)
        {
            // Group.Version (xmin) changed: someone else took the spot
            throw new ConflictException(ErrorCodes.SpotTaken, ErrorCodes.SpotTakenMessage);
        }

        return mapper.Map<EnrollmentDto>(enrollment);
    }

    public async Task CancelAsync(int id, CancellationToken ct = default)
    {
        var enrollment = await FindForUpdateAsync(id, ct);

        if (!enrollment.IsActive)
            return;

        // Attendance and payments are kept
        enrollment.IsActive = false;
        enrollment.Group.ReleaseSpot();
        await unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<EnrollmentDto> UpdatePaymentAsync(int id, UpdatePaymentRequest request, CancellationToken ct = default)
    {
        var enrollment = await FindForUpdateAsync(id, ct);

        var due = request.AmountDue ?? enrollment.AmountDue;
        var paid = request.AmountPaid!.Value;

        if (paid > due)
            throw new BusinessRuleException(ErrorCodes.PaymentAboveDue, ErrorCodes.PaymentAboveDueMessage);

        enrollment.AmountDue = due;
        enrollment.AmountPaid = paid;
        await unitOfWork.SaveChangesAsync(ct);

        return await GetByIdAsync(id, ct);
    }

    public async Task<EnrollmentDto> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var enrollment = await enrollments.GetByIdAsync(id, me.OwningTeacherId, ct)
            ?? throw new NotFoundException(ErrorCodes.EnrollmentNotFound, ErrorCodes.EnrollmentNotFoundMessage);

        return mapper.Map<EnrollmentDto>(enrollment);
    }

    private async Task<Enrollment> FindForUpdateAsync(int id, CancellationToken ct) =>
        await enrollments.FindForUpdateAsync(id, me.OwningTeacherId, ct)
            ?? throw new NotFoundException(ErrorCodes.EnrollmentNotFound, ErrorCodes.EnrollmentNotFoundMessage);
}
