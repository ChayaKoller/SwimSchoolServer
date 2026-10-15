using Microsoft.AspNetCore.Mvc;
using SwimSchool.Application.Common.Models;
using SwimSchool.Application.Groups;
using SwimSchool.Application.Groups.Dtos;

namespace SwimSchool.API.Controllers;

[ApiController]
[Route("api/groups")]
[Produces("application/json")]
public class GroupsController(IGroupService groups) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<GroupDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<GroupDto>>> Get([FromQuery] GroupFilter filter, CancellationToken ct) =>
        Ok(await groups.GetAsync(filter, ct));

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(GroupDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GroupDetailsDto>> GetById([FromRoute] int id, CancellationToken ct) =>
        Ok(await groups.GetDetailsAsync(id, ct));

    [HttpPost]
    [ProducesResponseType(typeof(GroupDetailsDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<GroupDetailsDto>> Create([FromBody] CreateGroupRequest request, CancellationToken ct)
    {
        var group = await groups.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = group.Id }, group);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(GroupDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<GroupDto>> Update(
        [FromRoute] int id, [FromBody] UpdateGroupRequest request, CancellationToken ct) =>
        Ok(await groups.UpdateAsync(id, request, ct));

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> Delete([FromRoute] int id, [FromQuery] uint version, CancellationToken ct)
    {
        await groups.DeleteAsync(id, version, ct);
        return NoContent();
    }
}
