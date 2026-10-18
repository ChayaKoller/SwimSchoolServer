using Riok.Mapperly.Abstractions;
using SwimSchool.Application.Lessons.Dtos;
using SwimSchool.Domain.Entities;

namespace SwimSchool.Application.Lessons;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class LessonMapper
{
    [MapProperty(nameof(Lesson.Id), nameof(LessonDto.LessonId))]
    [MapPropertyFromSource(nameof(LessonDto.GroupName), Use = nameof(GroupName))]
    public static partial LessonDto ToDto(Lesson lesson);

    public static partial List<LessonDto> ToDtos(IEnumerable<Lesson> lessons);

    // A navigation that was not loaded gives an empty value, not a crash (as AutoMapper did)
    private static string GroupName(Lesson lesson) => lesson.Group?.Name ?? string.Empty;
}
