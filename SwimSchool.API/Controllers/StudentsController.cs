using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwimSchool.Application.Common.Models;
using SwimSchool.Application.Students;
using SwimSchool.Application.Students.Dtos;
using SwimSchool.Domain.Enums;

namespace SwimSchool.API.Controllers;

[ApiController]
[Route("api/students")]
[Produces("application/json")]
[ProducesResponseType<ErrorResponse>(StatusCodes.Status401Unauthorized)]
public class StudentsController(IStudentService students) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<StudentListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<StudentListItemDto>>> Get(
        [FromQuery] StudentFilter filter, CancellationToken ct) =>
        Ok(await students.GetAsync(filter, ct));

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(StudentDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StudentDetailsDto>> GetById([FromRoute] int id, CancellationToken ct) =>
        Ok(await students.GetByIdAsync(id, ct));

    [HttpPost]
    [ProducesResponseType(typeof(StudentDetailsDto), StatusCodes.Status201Created)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<StudentDetailsDto>> Create(
        [FromBody] CreateStudentRequest request, CancellationToken ct)
    {
        var student = await students.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = student.Id }, student);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(StudentDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StudentDetailsDto>> Update(
        [FromRoute] int id, [FromBody] UpdateStudentRequest request, CancellationToken ct) =>
        Ok(await students.UpdateAsync(id, request, ct));

    [Authorize(Roles = nameof(UserRole.Teacher))]
    [HttpPost("{id:int}/transfer")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Transfer(
        [FromRoute] int id, [FromBody] TransferStudentRequest request, CancellationToken ct)
    {
        await students.TransferAsync(id, request, ct);
        return NoContent();
    }

    [Authorize(Roles = nameof(UserRole.Teacher))]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> Delete([FromRoute] int id, CancellationToken ct)
    {
        await students.DeleteAsync(id, ct);
        return NoContent();
    }
}
