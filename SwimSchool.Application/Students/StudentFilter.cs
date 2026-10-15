using SwimSchool.Application.Common.Models;

namespace SwimSchool.Application.Students;

public class StudentFilter : PagingFilter
{
    public string? Name { get; set; }
    public int? GroupId { get; set; }
    public bool OnlyUnpaid { get; set; }
    public int? AbsentMoreThan { get; set; }
    public bool IncludeInactive { get; set; }
}
