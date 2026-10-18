using SwimSchool.Application.Teachers.Dtos;

namespace SwimSchool.Application.Teachers;

public class TeacherService(IUserRepository users) : ITeacherService
{
    public async Task<List<TeacherOptionDto>> GetOptionsAsync(CancellationToken ct = default)
    {
        var teachers = await users.GetActiveTeachersAsync(ct);

        return TeacherMapper.ToOptionDtos(teachers);
    }
}
