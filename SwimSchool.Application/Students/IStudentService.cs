using SwimSchool.Application.Common.Models;
using SwimSchool.Application.Students.Dtos;

namespace SwimSchool.Application.Students;

public interface IStudentService
{
    Task<PagedResult<StudentListItemDto>> GetAsync(StudentFilter filter, CancellationToken ct = default);

    Task<StudentDetailsDto> GetByIdAsync(int id, CancellationToken ct = default);

    Task<StudentDetailsDto> CreateAsync(CreateStudentRequest request, CancellationToken ct = default);

    Task<StudentDetailsDto> UpdateAsync(int id, UpdateStudentRequest request, CancellationToken ct = default);

    Task TransferAsync(int id, TransferStudentRequest request, CancellationToken ct = default);
}
