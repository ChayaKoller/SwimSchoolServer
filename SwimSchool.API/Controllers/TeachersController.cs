using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SwimSchool.Application.Common.Models;
using SwimSchool.Application.Teachers;
using SwimSchool.Application.Teachers.Dtos;
using SwimSchool.Domain.Enums;

namespace SwimSchool.API.Controllers;

[ApiController]
[Route("api/teachers")]
[Produces("application/json")]
[Authorize(Roles = nameof(UserRole.Teacher))]
[ProducesResponseType<ErrorResponse>(StatusCodes.Status401Unauthorized)]
[ProducesResponseType<ErrorResponse>(StatusCodes.Status403Forbidden)]
public class TeachersController(ITeacherService teachers) : ControllerBase
{
    [HttpGet("options")]
    [ProducesResponseType(typeof(List<TeacherOptionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TeacherOptionDto>>> GetOptions(CancellationToken ct) =>
        Ok(await teachers.GetOptionsAsync(ct));
}
