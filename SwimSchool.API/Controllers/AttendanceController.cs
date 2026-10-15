using Microsoft.AspNetCore.Mvc;
using SwimSchool.Application.Attendance;
using SwimSchool.Application.Attendance.Dtos;

namespace SwimSchool.API.Controllers;

[ApiController]
[Produces("application/json")]
public class AttendanceController(IAttendanceService attendance) : ControllerBase
{
    [HttpGet("api/lessons/{lessonId:int}/attendance")]
    [ProducesResponseType(typeof(LessonAttendanceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LessonAttendanceDto>> GetLesson([FromRoute] int lessonId, CancellationToken ct) =>
        Ok(await attendance.GetLessonAsync(lessonId, ct));

    [HttpPut("api/attendance")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> UpdateLesson([FromBody] UpdateAttendanceRequest request, CancellationToken ct)
    {
        await attendance.UpdateLessonAsync(request, ct);
        return NoContent();
    }

    [HttpGet("api/groups/{groupId:int}/attendance")]
    [ProducesResponseType(typeof(GroupAttendanceGridDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GroupAttendanceGridDto>> GetGrid([FromRoute] int groupId, CancellationToken ct) =>
        Ok(await attendance.GetGridAsync(groupId, ct));

    [HttpPut("api/groups/{groupId:int}/attendance")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> UpdateGrid(
        [FromRoute] int groupId, [FromBody] UpdateGroupAttendanceRequest request, CancellationToken ct)
    {
        await attendance.UpdateGridAsync(groupId, request, ct);
        return NoContent();
    }
}
