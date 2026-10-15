using AutoMapper;
using SwimSchool.Application.Common.Exceptions;
using SwimSchool.Application.Common.Interfaces;
using SwimSchool.Application.Common.Models;
using SwimSchool.Application.Groups.Dtos;
using SwimSchool.Domain.Entities;

namespace SwimSchool.Application.Groups;

public class GroupService(
    IGroupRepository groups,
    IUnitOfWork unitOfWork,
    ICurrentUser me,
    IDateTimeProvider clock,
    IMapper mapper) : IGroupService
{
    public async Task<PagedResult<GroupDto>> GetAsync(GroupFilter filter, CancellationToken ct = default)
    {
        var page = await groups.GetAsync(me.OwningTeacherId, filter, ct);

        return new PagedResult<GroupDto>
        {
            Items = mapper.Map<List<GroupDto>>(page.Items),
            TotalCount = page.TotalCount,
            Page = page.Page,
            PageSize = page.PageSize
        };
    }

    public async Task<GroupDetailsDto> GetDetailsAsync(int id, CancellationToken ct = default)
    {
        var group = await groups.GetDetailsAsync(id, me.OwningTeacherId, ct)
            ?? throw NotFound();

        return mapper.Map<GroupDetailsDto>(group);
    }

    public async Task<GroupDetailsDto> CreateAsync(CreateGroupRequest request, CancellationToken ct = default)
    {
        // Israel local times: manual list, or weekly from the first lesson
        var lessonTimes = request.LessonTimes is { Count: > 0 }
            ? request.LessonTimes.OrderBy(t => t).ToList()
            : Enumerable.Range(0, request.LessonsCount!.Value)
                .Select(i => request.FirstLessonAt!.Value.AddDays(7 * i))
                .ToList();

        var group = mapper.Map<Group>(request);
        group.TeacherId = me.OwningTeacherId;
        group.LessonsCount = lessonTimes.Count;
        group.Lessons = lessonTimes
            .Select(t => new Lesson { StartTime = clock.IsraelToUtc(t) })
            .ToList();

        groups.Add(group);
        await unitOfWork.SaveChangesAsync(ct);

        return await GetDetailsAsync(group.Id, ct);
    }

    public async Task<GroupDto> UpdateAsync(int id, UpdateGroupRequest request, CancellationToken ct = default)
    {
        var group = await groups.FindForUpdateAsync(id, me.OwningTeacherId, ct)
            ?? throw NotFound();

        if (group.Version != request.Version!.Value)
            throw new ConflictException(ErrorCodes.DataChanged, ErrorCodes.DataChangedMessage);

        if (request.MaxStudents!.Value < group.EnrolledCount)
            throw new BusinessRuleException(ErrorCodes.MaxBelowEnrolled, ErrorCodes.MaxBelowEnrolledMessage);

        mapper.Map(request, group);
        await unitOfWork.SaveChangesAsync(ct);

        return mapper.Map<GroupDto>(group);
    }

    private static NotFoundException NotFound() =>
        new(ErrorCodes.GroupNotFound, ErrorCodes.GroupNotFoundMessage);
}
