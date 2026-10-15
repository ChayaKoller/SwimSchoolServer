using AutoMapper;
using SwimSchool.Application.Common.Exceptions;
using SwimSchool.Application.Common.Interfaces;
using SwimSchool.Application.Common.Models;
using SwimSchool.Application.Students.Dtos;
using SwimSchool.Application.Teachers;
using SwimSchool.Domain.Entities;

namespace SwimSchool.Application.Students;

public class StudentService(
    IStudentRepository students,
    IUserRepository users,
    IUnitOfWork unitOfWork,
    ICurrentUser me,
    IMapper mapper) : IStudentService
{
    public async Task<PagedResult<StudentListItemDto>> GetAsync(StudentFilter filter, CancellationToken ct = default)
    {
        var page = await students.GetAsync(me.OwningTeacherId, filter, ct);

        return new PagedResult<StudentListItemDto>
        {
            Items = mapper.Map<List<StudentListItemDto>>(page.Items),
            TotalCount = page.TotalCount,
            Page = page.Page,
            PageSize = page.PageSize
        };
    }

    public async Task<StudentDetailsDto> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var student = await students.GetByIdAsync(id, me.OwningTeacherId, ct)
            ?? throw NotFound();

        return mapper.Map<StudentDetailsDto>(student);
    }

    public async Task<StudentDetailsDto> CreateAsync(CreateStudentRequest request, CancellationToken ct = default)
    {
        if (await students.IdentityNumberExistsAsync(request.IdentityNumber, ct))
            throw new ConflictException(ErrorCodes.DuplicateIdentity, ErrorCodes.DuplicateIdentityMessage);

        var student = mapper.Map<Student>(request);
        student.ResponsibleTeacherId = me.OwningTeacherId;

        students.Add(student);
        await unitOfWork.SaveChangesAsync(ct);

        return await GetByIdAsync(student.Id, ct);
    }

    public async Task<StudentDetailsDto> UpdateAsync(int id, UpdateStudentRequest request, CancellationToken ct = default)
    {
        var student = await students.FindForUpdateAsync(id, me.OwningTeacherId, ct)
            ?? throw NotFound();

        mapper.Map(request, student);
        await unitOfWork.SaveChangesAsync(ct);

        return await GetByIdAsync(id, ct);
    }

    public async Task TransferAsync(int id, TransferStudentRequest request, CancellationToken ct = default)
    {
        if (!me.IsTeacher)
            throw new ForbiddenException(ErrorCodes.Forbidden, ErrorCodes.ForbiddenMessage);

        var student = await students.FindForUpdateAsync(id, me.OwningTeacherId, ct)
            ?? throw NotFound();

        var newTeacherId = request.NewTeacherId!.Value;

        if (newTeacherId == student.ResponsibleTeacherId)
            throw new BusinessRuleException(ErrorCodes.SameTeacher, ErrorCodes.SameTeacherMessage);

        if (!await users.IsActiveTeacherAsync(newTeacherId, ct))
            throw new NotFoundException(ErrorCodes.TeacherNotFound, ErrorCodes.TeacherNotFoundMessage);

        student.TransferTo(newTeacherId);
        await unitOfWork.SaveChangesAsync(ct);
    }

    private static NotFoundException NotFound() =>
        new(ErrorCodes.StudentNotFound, ErrorCodes.StudentNotFoundMessage);
}
