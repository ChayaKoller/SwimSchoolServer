using Microsoft.AspNetCore.Mvc;
using SwimSchool.Application.Teachers;
using SwimSchool.Application.Teachers.Dtos;

namespace SwimSchool.API.Controllers;

[ApiController]
[Route("api/teachers")]
[Produces("application/json")]
public class TeachersController(ITeacherService teachers) : ControllerBase
{
    [HttpGet("options")]
    [ProducesResponseType(typeof(List<TeacherOptionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TeacherOptionDto>>> GetOptions(CancellationToken ct) =>
        Ok(await teachers.GetOptionsAsync(ct));
}
