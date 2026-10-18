using Microsoft.AspNetCore.Mvc;
using SwimSchool.Application.Common.Models;
using SwimSchool.Application.Reports;
using SwimSchool.Application.Reports.Dtos;

namespace SwimSchool.API.Controllers;

[ApiController]
[Produces("application/json")]
[ProducesResponseType<ErrorResponse>(StatusCodes.Status401Unauthorized)]
public class ReportsController(IReportService reports) : ControllerBase
{
    [HttpGet("api/reports")]
    [ProducesResponseType(typeof(PagedResult<ReportDto>), StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<ReportDto>>> Get([FromQuery] ReportFilter filter, CancellationToken ct) =>
        Ok(await reports.GetAsync(filter, ct));

    [HttpPost("api/reports")]
    [ProducesResponseType(typeof(ReportDto), StatusCodes.Status201Created)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReportDto>> Create([FromBody] CreateReportRequest request, CancellationToken ct)
    {
        var report = await reports.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = report.Id }, report);
    }

    [HttpPut("api/reports/{id:int}")]
    [ProducesResponseType(typeof(ReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReportDto>> Update(
        [FromRoute] int id, [FromBody] UpdateReportRequest request, CancellationToken ct) =>
        Ok(await reports.UpdateAsync(id, request, ct));

    [HttpGet("api/students/{studentId:int}/attended-dates")]
    [ProducesResponseType(typeof(IReadOnlyList<DateOnly>), StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<DateOnly>>> GetAttendedDates(
        [FromRoute] int studentId, [FromQuery] AttendedDatesFilter filter, CancellationToken ct) =>
        Ok(await reports.GetAttendedDatesAsync(studentId, filter, ct));

    [HttpGet("api/students/{studentId:int}/reports")]
    [ProducesResponseType(typeof(List<ReportDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ReportDto>>> GetForStudent([FromRoute] int studentId, CancellationToken ct) =>
        Ok(await reports.GetForStudentAsync(studentId, ct));

    [HttpGet("api/reports/{id:int}")]
    [ProducesResponseType(typeof(ReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReportDto>> GetById([FromRoute] int id, CancellationToken ct) =>
        Ok(await reports.GetByIdAsync(id, ct));
}
