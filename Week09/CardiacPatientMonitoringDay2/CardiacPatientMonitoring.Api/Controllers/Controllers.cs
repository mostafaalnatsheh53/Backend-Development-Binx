using CardiacPatientMonitoring.Api.DTOs;
using CardiacPatientMonitoring.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CardiacPatientMonitoring.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService s) : ControllerBase
{
    /// <summary>
    /// Creates a patient account and returns a JWT for the new patient.
    /// </summary>
    /// <param name="d">The email, password, identity, and contact details for the patient account.</param>
    /// <returns>A bearer token, its expiration time, and the created patient's identifier.</returns>
    /// <remarks>
    /// Example request: { "email": "patient@example.com", "password": "Secure123", "firstName": "Maya", "lastName": "Patel", "dateOfBirth": "1987-04-12", "gender": "Female", "phoneNumber": "555-0102" }
    /// Example response: { "token": "YOUR_JWT_TOKEN", "expiresAt": "2026-09-14T12:00:00Z", "patientId": 42 }
    /// </remarks>
    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType<AuthResponseDto>(201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto d)
    {
        return StatusCode(201, await s.RegisterAsync(d));
    }

    /// <summary>
    /// Authenticates an existing account and returns a JWT containing its role claims.
    /// </summary>
    /// <param name="d">The account email and password.</param>
    /// <returns>A bearer token, expiration time, and patient identifier when the account is linked to a patient.</returns>
    /// <remarks>
    /// Example request: { "email": "patient@example.com", "password": "Secure123" }
    /// Example response: { "token": "YOUR_JWT_TOKEN", "expiresAt": "2026-09-14T12:00:00Z", "patientId": 42 }
    /// </remarks>
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
[Route("api/patients")]
public class PatientsController(IPatientService s) : ControllerBase
{
    /// <summary>
    /// Returns the patient catalog for administrators, optionally filtered by name or contact details.
    /// </summary>
    /// <param name="search">Optional text used to filter patient records.</param>
    /// <returns>The matching patient records.</returns>
    [Authorize(Roles = "Admin")]
    [HttpGet]
    [ProducesResponseType<IEnumerable<PatientResponseDto>>(200)]
    public Task<IEnumerable<PatientResponseDto>> GetAll(
        [FromQuery] string? search)
    {
        return s.GetAllAsync(search);
    }

    [Authorize]
    [HttpGet("{id:int}")]
    [ProducesResponseType<PatientResponseDto>(200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<PatientResponseDto>> Get(int id)
    {
        if (User is not null && !User.IsInRole("Admin"))
        {
            var currentPatientId = GetCurrentPatientId();
            if (currentPatientId is null || currentPatientId != id)
                return Forbid();
        }

        return Ok(await s.GetAsync(id));
    }

    /// <summary>
    /// Adds a patient record for an administrator to manage in the monitoring catalog.
    /// </summary>
    /// <param name="d">The patient's identity, date of birth, gender, and optional phone number.</param>
    /// <returns>The created patient record, including its generated identifier.</returns>
    [Authorize(Roles = "Admin")]
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

    [Authorize(Roles = "Admin")]
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

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int id)
    {
        await s.DeleteAsync(id);

        return NoContent();
    }

    private int? GetCurrentPatientId()
    {
        if (User is null)
            return null;

        return int.TryParse(User.FindFirst("patient_id")?.Value, out var patientId)
            ? patientId
            : null;
    }
}

[ApiController]
[Authorize]
[Route("api/patients/{patientId:int}/vital-signs")]
public class VitalSignsController(IVitalSignService s) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VitalSignResponseDto>>> All(int patientId)
    {
        var forbidden = EnsureAllowed(patientId);
        if (forbidden is not null)
            return forbidden;

        return Ok(await s.GetAllAsync(patientId));
    }

    [HttpGet("/api/vital-signs/{id:int}")]
    public async Task<ActionResult<VitalSignResponseDto>> Get(int id)
    {
        var resource = await s.GetAsync(id);
        var forbidden = EnsureAllowed(resource.PatientId);
        if (forbidden is not null)
            return forbidden;

        return Ok(resource);
    }

    /// <summary>
    /// Records a heart rate, blood pressure, and optional temperature reading for an authorized patient.
    /// </summary>
    /// <param name="patientId">The patient whose vital signs are being recorded.</param>
    /// <param name="d">The measured values and the UTC time at which they were recorded.</param>
    /// <returns>The persisted vital-sign record with its generated identifier.</returns>
    /// <remarks>
    /// Example request: { "heartRate": 72, "systolicBloodPressure": 120, "diastolicBloodPressure": 80, "temperatureCelsius": 36.8, "recordedAt": "2026-09-14T10:00:00Z" }
    /// Example response: { "id": 18, "patientId": 42, "heartRate": 72, "systolicBloodPressure": 120, "diastolicBloodPressure": 80, "temperatureCelsius": 36.8, "recordedAt": "2026-09-14T10:00:00Z" }
    /// </remarks>
    [HttpPost]
    [ProducesResponseType<VitalSignResponseDto>(201)]
    public async Task<ActionResult<VitalSignResponseDto>> Create(
        int patientId,
        VitalSignRequestDto d)
    {
        var forbidden = EnsureAllowed(patientId);
        if (forbidden is not null)
            return forbidden;

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
        var resource = await s.GetAsync(id);
        var forbidden = EnsureAllowed(resource.PatientId);
        if (forbidden is not null)
            return forbidden;

        await s.UpdateAsync(id, d);

        return NoContent();
    }

    [HttpDelete("/api/vital-signs/{id:int}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Delete(int id)
    {
        var resource = await s.GetAsync(id);
        var forbidden = EnsureAllowed(resource.PatientId);
        if (forbidden is not null)
            return forbidden;

        await s.DeleteAsync(id);

        return NoContent();
    }

    private ActionResult? EnsureAllowed(int patientId)
    {
        if (User is null || User.IsInRole("Admin"))
            return null;

        var currentPatientId = GetCurrentPatientId();
        if (currentPatientId is null || currentPatientId != patientId)
            return Forbid();

        return null;
    }

    private int? GetCurrentPatientId()
    {
        if (User is null)
            return null;

        return int.TryParse(User.FindFirst("patient_id")?.Value, out var patientId)
            ? patientId
            : null;
    }
}

[ApiController]
[Authorize]
[Route("api/patients/{patientId:int}/medications")]
public class MedicationsController(IMedicationService s) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MedicationResponseDto>>> All(
        int patientId,
        [FromQuery] string? search)
    {
        var forbidden = EnsureAllowed(patientId);
        if (forbidden is not null)
            return forbidden;

        return Ok(await s.GetAllAsync(patientId, search));
    }

    [HttpGet("/api/medications/{id:int}")]
    public async Task<ActionResult<MedicationResponseDto>> Get(int id)
    {
        var resource = await s.GetAsync(id);
        var forbidden = EnsureAllowed(resource.PatientId);
        if (forbidden is not null)
            return forbidden;

        return Ok(resource);
    }

    [HttpPost]
    [ProducesResponseType<MedicationResponseDto>(201)]
    public async Task<ActionResult<MedicationResponseDto>> Create(
        int patientId,
        MedicationRequestDto d)
    {
        var forbidden = EnsureAllowed(patientId);
        if (forbidden is not null)
            return forbidden;

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
        var resource = await s.GetAsync(id);
        var forbidden = EnsureAllowed(resource.PatientId);
        if (forbidden is not null)
            return forbidden;

        await s.UpdateAsync(id, d);

        return NoContent();
    }

    [HttpDelete("/api/medications/{id:int}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Delete(int id)
    {
        if (User is not null && !User.IsInRole("Admin"))
        {
            var resource = await s.GetAsync(id);
            var forbidden = EnsureAllowed(resource.PatientId);
            if (forbidden is not null)
                return forbidden;
        }

        await s.DeleteAsync(id);

        return NoContent();
    }

    private ActionResult? EnsureAllowed(int patientId)
    {
        if (User is null || User.IsInRole("Admin"))
            return null;

        var currentPatientId = GetCurrentPatientId();
        if (currentPatientId is null || currentPatientId != patientId)
            return Forbid();

        return null;
    }

    private int? GetCurrentPatientId()
    {
        if (User is null)
            return null;

        return int.TryParse(User.FindFirst("patient_id")?.Value, out var patientId)
            ? patientId
            : null;
    }
}

[ApiController]
[Authorize]
[Route("api/patients/{patientId:int}/appointments")]
public class AppointmentsController(IAppointmentService s) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppointmentResponseDto>>> All(
        int patientId,
        [FromQuery] string? status)
    {
        var forbidden = EnsureAllowed(patientId);
        if (forbidden is not null)
            return forbidden;

        return Ok(await s.GetAllAsync(patientId, status));
    }

    [HttpGet("/api/appointments/{id:int}")]
    public async Task<ActionResult<AppointmentResponseDto>> Get(int id)
    {
        var resource = await s.GetAsync(id);
        var forbidden = EnsureAllowed(resource.PatientId);
        if (forbidden is not null)
            return forbidden;

        return Ok(resource);
    }

    [HttpPost]
    [ProducesResponseType<AppointmentResponseDto>(201)]
    public async Task<ActionResult<AppointmentResponseDto>> Create(
        int patientId,
        AppointmentRequestDto d)
    {
        var forbidden = EnsureAllowed(patientId);
        if (forbidden is not null)
            return forbidden;

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
        var resource = await s.GetAsync(id);
        var forbidden = EnsureAllowed(resource.PatientId);
        if (forbidden is not null)
            return forbidden;

        await s.UpdateAsync(id, d);

        return NoContent();
    }

    [HttpDelete("/api/appointments/{id:int}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Delete(int id)
    {
        var resource = await s.GetAsync(id);
        var forbidden = EnsureAllowed(resource.PatientId);
        if (forbidden is not null)
            return forbidden;

        await s.DeleteAsync(id);

        return NoContent();
    }

    private ActionResult? EnsureAllowed(int patientId)
    {
        if (User is null || User.IsInRole("Admin"))
            return null;

        var currentPatientId = GetCurrentPatientId();
        if (currentPatientId is null || currentPatientId != patientId)
            return Forbid();

        return null;
    }

    private int? GetCurrentPatientId()
    {
        if (User is null)
            return null;

        return int.TryParse(User.FindFirst("patient_id")?.Value, out var patientId)
            ? patientId
            : null;
    }
}
