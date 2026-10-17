using SwimSchool.Application.Teachers.Dtos;

namespace SwimSchool.Application.Teachers;

public interface ITeacherAccountService
{
    Task<int> CreateTeacherAsync(CreateTeacherRequest request, CancellationToken ct = default);
}
