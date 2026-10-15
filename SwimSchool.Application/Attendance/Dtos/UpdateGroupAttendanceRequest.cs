using System.ComponentModel.DataAnnotations;

namespace SwimSchool.Application.Attendance.Dtos;

public sealed record UpdateGroupAttendanceRequest
{
    [MinLength(1)]
    public List<GridCellUpdate> Items { get; init; } = new List<GridCellUpdate>();
}
