using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwimSchool.Application.Common.Models;
using SwimSchool.Application.ReportSettings;
using SwimSchool.Application.ReportSettings.Dtos;
using SwimSchool.Domain.Enums;

namespace SwimSchool.API.Controllers;

[ApiController]
[Route("api/report-settings")]
[Produces("application/json")]
[ProducesResponseType<ErrorResponse>(StatusCodes.Status401Unauthorized)]
public class ReportSettingsController(IReportSettingsService settings) : ControllerBase
{
    // Teacher and secretary
    [HttpGet]
    [ProducesResponseType(typeof(ReportSettingsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ReportSettingsDto>> Get(CancellationToken ct) =>
        Ok(await settings.GetAsync(ct));

    [HttpPut]
    [Authorize(Roles = nameof(UserRole.Teacher))]
    [ProducesResponseType(typeof(ReportSettingsDto), StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ReportSettingsDto>> Update(
        [FromBody] UpdateReportSettingsRequest request, CancellationToken ct) =>
        Ok(await settings.UpdateAsync(request, ct));
}
