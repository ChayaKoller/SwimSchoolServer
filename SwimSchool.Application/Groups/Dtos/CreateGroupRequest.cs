using System.ComponentModel.DataAnnotations;
using SwimSchool.Application.Common.Validation;
using SwimSchool.Domain.Enums;

namespace SwimSchool.Application.Groups.Dtos;

public sealed record CreateGroupRequest : IValidatableObject
{
    [Required, MaxLength(100)]
    public string Name { get; init; } = string.Empty;

    [Required, Range(2020, 2100)]
    public int? Year { get; init; }

    [Required, EnumDataType(typeof(Season))]
    public Season? Season { get; init; }

    [Range(1, 200)]
    public int? LessonsCount { get; init; }

    [Required, Range(0, double.MaxValue)]
    public decimal? Price { get; init; }

    [Required, Range(1, 500)]
    public int? MaxStudents { get; init; }

    // Israel local time; converted to UTC in the service
    public DateTime? FirstLessonAt { get; init; }

    // Israel local time; converted to UTC in the service
    [NoDuplicates]
    public List<DateTime>? LessonTimes { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var hasSchedule = FirstLessonAt is not null && LessonsCount is not null;
        var hasLessonTimes = LessonTimes is { Count: > 0 };
        var usesSchedule = FirstLessonAt is not null || LessonsCount is not null;

        var valid = (hasSchedule && LessonTimes is null) || (hasLessonTimes && !usesSchedule);
        if (!valid)
        {
            yield return new ValidationResult(
                "Provide either FirstLessonAt and LessonsCount, or LessonTimes with at least one item, but not both.",
                new[] { nameof(FirstLessonAt), nameof(LessonsCount), nameof(LessonTimes) });
        }
    }
}
