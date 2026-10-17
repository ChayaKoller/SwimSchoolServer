using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwimSchool.Application.Common.Models;
using SwimSchool.Application.Secretaries;
using SwimSchool.Application.Secretaries.Dtos;
using SwimSchool.Domain.Enums;

namespace SwimSchool.API.Controllers;

[ApiController]
[Route("api/secretaries")]
[Authorize(Roles = nameof(UserRole.Teacher))]
[Produces("application/json")]
[ProducesResponseType<ErrorResponse>(StatusCodes.Status401Unauthorized)]
[ProducesResponseType<ErrorResponse>(StatusCodes.Status403Forbidden)]
public class SecretariesController(ISecretaryService secretaries) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<SecretaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<SecretaryDto>>> Get(CancellationToken ct) =>
        Ok(await secretaries.GetAllAsync(ct));

    [HttpPost]
    [ProducesResponseType(typeof(SecretaryDto), StatusCodes.Status201Created)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<SecretaryDto>> Create([FromBody] CreateSecretaryRequest request, CancellationToken ct) =>
        StatusCode(StatusCodes.Status201Created, await secretaries.CreateAsync(request, ct));

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Deactivate([FromRoute] int id, CancellationToken ct)
    {
        await secretaries.DeactivateAsync(id, ct);
        return NoContent();
    }

    [HttpPut("{id:int}/password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> SetPassword(
        [FromRoute] int id, [FromBody] SetPasswordRequest request, CancellationToken ct)
    {
        await secretaries.SetPasswordAsync(id, request, ct);
        return NoContent();
    }
}
