using SwimSchool.Application.Common.Models;
using SwimSchool.Domain.Enums;

namespace SwimSchool.Application.Groups;

public class GroupFilter : PagingFilter
{
    public bool? IsActive { get; set; }
    public Season? Season { get; set; }
    public int? Year { get; set; }
    public string? Name { get; set; }
}
