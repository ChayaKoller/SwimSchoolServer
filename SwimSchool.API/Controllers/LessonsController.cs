using Microsoft.AspNetCore.Mvc;
using SwimSchool.Application.Lessons;
using SwimSchool.Application.Lessons.Dtos;

namespace SwimSchool.API.Controllers;

[ApiController]
[Produces("application/json")]
public class LessonsController(ILessonService lessons) : ControllerBase
{
    [HttpGet("api/lessons/week")]
    [ProducesResponseType(typeof(List<LessonDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<LessonDto>>> GetWeek(CancellationToken ct) =>
        Ok(await lessons.GetWeekAsync(ct));

    [HttpPost("api/groups/{groupId:int}/lessons")]
    [ProducesResponseType(typeof(List<LessonDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<LessonDto>>> Add(
        [FromRoute] int groupId, [FromBody] AddLessonsRequest request, CancellationToken ct) =>
        Ok(await lessons.AddAsync(groupId, request, ct));

    [HttpPut("api/lessons/{id:int}")]
    [ProducesResponseType(typeof(LessonDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LessonDto>> Update(
        [FromRoute] int id, [FromBody] UpdateLessonRequest request, CancellationToken ct) =>
        Ok(await lessons.UpdateAsync(id, request, ct));

    [HttpDelete("api/lessons/{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete([FromRoute] int id, CancellationToken ct)
    {
        await lessons.DeleteAsync(id, ct);
        return NoContent();
    }
}
