using SwimSchool.Application.Teachers.Dtos;

namespace SwimSchool.Application.Teachers;

public interface ITeacherService
{
    Task<List<TeacherOptionDto>> GetOptionsAsync(CancellationToken ct = default);
}
