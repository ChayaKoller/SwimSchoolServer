using AutoMapper;
using SwimSchool.Application.Teachers.Dtos;

namespace SwimSchool.Application.Teachers;

public class TeacherService(IUserRepository users, IMapper mapper) : ITeacherService
{
    public async Task<List<TeacherOptionDto>> GetOptionsAsync(CancellationToken ct = default)
    {
        var teachers = await users.GetActiveTeachersAsync(ct);

        return mapper.Map<List<TeacherOptionDto>>(teachers);
    }
}
