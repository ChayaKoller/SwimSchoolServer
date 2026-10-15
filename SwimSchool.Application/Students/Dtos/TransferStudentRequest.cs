using System.ComponentModel.DataAnnotations;

namespace SwimSchool.Application.Students.Dtos;

public sealed record TransferStudentRequest
{
    [Required, Range(1, int.MaxValue)]
    public int? NewTeacherId { get; init; }
}
