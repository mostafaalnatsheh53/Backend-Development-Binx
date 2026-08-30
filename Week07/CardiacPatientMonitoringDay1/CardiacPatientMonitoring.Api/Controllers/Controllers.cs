using CardiacPatientMonitoring.Api.DTOs;
using CardiacPatientMonitoring.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CardiacPatientMonitoring.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService s) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType<AuthResponseDto>(201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto d)
    {
        return StatusCode(201, await s.RegisterAsync(d));
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType<AuthResponseDto>(200)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto d)
    {
        return Ok(await s.LoginAsync(d));
    }
}

[ApiController]
[Authorize]
[Route("api/patients")]
public class PatientsController(IPatientService s) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IEnumerable<PatientResponseDto>>(200)]
    public Task<IEnumerable<PatientResponseDto>> GetAll(
        [FromQuery] string? search)
    {
        return s.GetAllAsync(search);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<PatientResponseDto>(200)]
    [ProducesResponseType(404)]
    public Task<PatientResponseDto> Get(int id)
    {
        return s.GetAsync(id);
    }

    [HttpPost]
    [ProducesResponseType<PatientResponseDto>(201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<PatientResponseDto>> Create(
        PatientRequestDto d)
    {
        var x = await s.CreateAsync(d);

        return CreatedAtAction(
            nameof(Get),
            new { id = x.Id },
            x);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(
        int id,
        PatientRequestDto d)
    {
        await s.UpdateAsync(id, d);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int id)
    {
        await s.DeleteAsync(id);

        return NoContent();
    }
}

[ApiController]
[Authorize]
[Route("api/patients/{patientId:int}/vital-signs")]
public class VitalSignsController(IVitalSignService s) : ControllerBase
{
    [HttpGet]
    public Task<IEnumerable<VitalSignResponseDto>> All(int patientId)
    {
        return s.GetAllAsync(patientId);
    }

    [HttpGet("/api/vital-signs/{id:int}")]
    public Task<VitalSignResponseDto> Get(int id)
    {
        return s.GetAsync(id);
    }

    [HttpPost]
    [ProducesResponseType<VitalSignResponseDto>(201)]
    public async Task<ActionResult<VitalSignResponseDto>> Create(
        int patientId,
        VitalSignRequestDto d)
    {
        var x = await s.CreateAsync(patientId, d);

        return CreatedAtAction(
            nameof(Get),
            new { id = x.Id },
            x);
    }

    [HttpPut("/api/vital-signs/{id:int}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Update(
        int id,
        VitalSignRequestDto d)
    {
        await s.UpdateAsync(id, d);

        return NoContent();
    }

    [HttpDelete("/api/vital-signs/{id:int}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Delete(int id)
    {
        await s.DeleteAsync(id);

        return NoContent();
    }
}

[ApiController]
[Authorize]
[Route("api/patients/{patientId:int}/medications")]
public class MedicationsController(IMedicationService s) : ControllerBase
{
    [HttpGet]
    public Task<IEnumerable<MedicationResponseDto>> All(
        int patientId,
        [FromQuery] string? search)
    {
        return s.GetAllAsync(patientId, search);
    }

    [HttpGet("/api/medications/{id:int}")]
    public Task<MedicationResponseDto> Get(int id)
    {
        return s.GetAsync(id);
    }

    [HttpPost]
    [ProducesResponseType<MedicationResponseDto>(201)]
    public async Task<ActionResult<MedicationResponseDto>> Create(
        int patientId,
        MedicationRequestDto d)
    {
        var x = await s.CreateAsync(patientId, d);

        return CreatedAtAction(
            nameof(Get),
            new { id = x.Id },
            x);
    }

    [HttpPut("/api/medications/{id:int}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Update(
        int id,
        MedicationRequestDto d)
    {
        await s.UpdateAsync(id, d);

        return NoContent();
    }

    [HttpDelete("/api/medications/{id:int}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Delete(int id)
    {
        await s.DeleteAsync(id);

        return NoContent();
    }
}

[ApiController]
[Authorize]
[Route("api/patients/{patientId:int}/appointments")]
public class AppointmentsController(IAppointmentService s) : ControllerBase
{
    [HttpGet]
    public Task<IEnumerable<AppointmentResponseDto>> All(
        int patientId,
        [FromQuery] string? status)
    {
        return s.GetAllAsync(patientId, status);
    }

    [HttpGet("/api/appointments/{id:int}")]
    public Task<AppointmentResponseDto> Get(int id)
    {
        return s.GetAsync(id);
    }

    [HttpPost]
    [ProducesResponseType<AppointmentResponseDto>(201)]
    public async Task<ActionResult<AppointmentResponseDto>> Create(
        int patientId,
        AppointmentRequestDto d)
    {
        var x = await s.CreateAsync(patientId, d);

        return CreatedAtAction(
            nameof(Get),
            new { id = x.Id },
            x);
    }

    [HttpPut("/api/appointments/{id:int}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Update(
        int id,
        AppointmentRequestDto d)
    {
        await s.UpdateAsync(id, d);

        return NoContent();
    }

    [HttpDelete("/api/appointments/{id:int}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Delete(int id)
    {
        await s.DeleteAsync(id);

        return NoContent();
    }
}