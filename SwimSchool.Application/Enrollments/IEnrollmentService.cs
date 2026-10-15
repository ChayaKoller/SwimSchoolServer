using SwimSchool.Application.Enrollments.Dtos;

namespace SwimSchool.Application.Enrollments;

public interface IEnrollmentService
{
    Task<EnrollmentDto> EnrollAsync(EnrollRequest request, CancellationToken ct = default);
    Task CancelAsync(int id, CancellationToken ct = default);
    Task<EnrollmentDto> UpdatePaymentAsync(int id, UpdatePaymentRequest request, CancellationToken ct = default);
    Task<EnrollmentDto> GetByIdAsync(int id, CancellationToken ct = default);
}
