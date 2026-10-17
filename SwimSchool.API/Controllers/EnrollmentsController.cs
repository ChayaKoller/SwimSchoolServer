using Microsoft.AspNetCore.Mvc;
using SwimSchool.Application.Common.Models;
using SwimSchool.Application.Enrollments;
using SwimSchool.Application.Enrollments.Dtos;

namespace SwimSchool.API.Controllers;

[ApiController]
[Route("api/enrollments")]
[Produces("application/json")]
public class EnrollmentsController(IEnrollmentService enrollments) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(EnrollmentDto), StatusCodes.Status201Created)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<EnrollmentDto>> Enroll([FromBody] EnrollRequest request, CancellationToken ct)
    {
        var enrollment = await enrollments.EnrollAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = enrollment.Id }, enrollment);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(EnrollmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EnrollmentDto>> GetById([FromRoute] int id, CancellationToken ct) =>
        Ok(await enrollments.GetByIdAsync(id, ct));

    [HttpPut("{id:int}/payment")]
    [ProducesResponseType(typeof(EnrollmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EnrollmentDto>> UpdatePayment(
        [FromRoute] int id, [FromBody] UpdatePaymentRequest request, CancellationToken ct) =>
        Ok(await enrollments.UpdatePaymentAsync(id, request, ct));

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> Cancel([FromRoute] int id, CancellationToken ct)
    {
        await enrollments.CancelAsync(id, ct);
        return NoContent();
    }
}
