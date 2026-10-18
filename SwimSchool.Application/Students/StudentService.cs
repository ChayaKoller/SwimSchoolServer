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
    ICurrentUser me) : IStudentService
{
    public async Task<PagedResult<StudentListItemDto>> GetAsync(StudentFilter filter, CancellationToken ct = default)
    {
        var page = await students.GetAsync(me.OwningTeacherId, filter, ct);

        return new PagedResult<StudentListItemDto>
        {
            Items = StudentMapper.ToListItemDtos(page.Items),
            TotalCount = page.TotalCount,
            Page = page.Page,
            PageSize = page.PageSize
        };
    }

    public async Task<StudentDetailsDto> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var student = await students.GetByIdAsync(id, me.OwningTeacherId, ct)
            ?? throw NotFound();

        return StudentMapper.ToDetailsDto(student);
    }

    public async Task<StudentDetailsDto> CreateAsync(CreateStudentRequest request, CancellationToken ct = default)
    {
        if (await students.IdentityNumberExistsAsync(request.IdentityNumber, ct))
            throw new ConflictException(ErrorCodes.DuplicateIdentity, ErrorCodes.DuplicateIdentityMessage);

        var student = StudentMapper.ToEntity(request);
        student.ResponsibleTeacherId = me.OwningTeacherId;

        students.Add(student);
        await unitOfWork.SaveChangesAsync(ct);

        return await GetByIdAsync(student.Id, ct);
    }

    public async Task<StudentDetailsDto> UpdateAsync(int id, UpdateStudentRequest request, CancellationToken ct = default)
    {
        var student = await students.FindForUpdateAsync(id, me.OwningTeacherId, ct)
            ?? throw NotFound();

        StudentMapper.UpdateEntity(request, student);
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

    // No history: real delete. Otherwise archive and release active spots; history is kept
    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        if (!me.IsTeacher)
            throw new ForbiddenException(ErrorCodes.Forbidden, ErrorCodes.ForbiddenMessage);

        var student = await students.FindForDeleteAsync(id, me.OwningTeacherId, ct)
            ?? throw NotFound();

        if (student.Enrollments.Count == 0 && !await students.HasReportsAsync(student.Id, ct))
        {
            students.Remove(student);
        }
        else
        {
            student.IsActive = false;

            foreach (var enrollment in student.Enrollments.Where(e => e.IsActive))
            {
                enrollment.IsActive = false;
                enrollment.Group.ReleaseSpot();
            }
        }

        await unitOfWork.SaveChangesAsync(ct);
    }

    private static NotFoundException NotFound() =>
        new(ErrorCodes.StudentNotFound, ErrorCodes.StudentNotFoundMessage);
}
