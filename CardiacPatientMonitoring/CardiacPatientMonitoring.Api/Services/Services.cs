using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CardiacPatientMonitoring.Api.Data;
using CardiacPatientMonitoring.Api.DTOs;
using CardiacPatientMonitoring.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CardiacPatientMonitoring.Api.Services;

public class NotFoundException(string message) : Exception(message)
{
}

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
}

public class AuthService(
    UserManager<ApplicationUser> users,
    IConfiguration config) : IAuthService
{
    public async Task<AuthResponseDto> RegisterAsync(RegisterDto d)
    {
        var u = new ApplicationUser
        {
            UserName = d.Email,
            Email = d.Email
        };

        var r = await users.CreateAsync(u, d.Password);

        if (!r.Succeeded)
            throw new ArgumentException(
                string.Join(" ", r.Errors.Select(e => e.Description)));

        return Token(u);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto d)
    {
        var u = await users.FindByEmailAsync(d.Email);

        if (u is null || !await users.CheckPasswordAsync(u, d.Password))
            throw new UnauthorizedAccessException("Invalid email or password.");

        return Token(u);
    }

    private AuthResponseDto Token(ApplicationUser u)
    {
        var j = config.GetSection("Jwt");
        var expires = DateTime.UtcNow.AddHours(2);

        var t = new JwtSecurityToken(
            j["Issuer"],
            j["Audience"],
            [
                new Claim(ClaimTypes.NameIdentifier, u.Id),
                new Claim(ClaimTypes.Email, u.Email!)
            ],
            expires: expires,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(j["Key"]!)),
                SecurityAlgorithms.HmacSha256));

        return new(
            new JwtSecurityTokenHandler().WriteToken(t),
            expires);
    }
}

public interface IPatientService
{
    Task<IEnumerable<PatientResponseDto>> GetAllAsync(string? search);
    Task<PatientResponseDto> GetAsync(int id);
    Task<PatientResponseDto> CreateAsync(PatientRequestDto dto);
    Task UpdateAsync(int id, PatientRequestDto dto);
    Task DeleteAsync(int id);
}

public class PatientService(ApplicationDbContext db) : IPatientService
{
    private static PatientResponseDto Map(Patient p) =>
        new(
            p.Id,
            p.FirstName,
            p.LastName,
            p.DateOfBirth,
            p.Gender,
            p.PhoneNumber);

    public async Task<IEnumerable<PatientResponseDto>> GetAllAsync(string? s) =>
        await db.Patients
            .AsNoTracking()
            .Where(p =>
                string.IsNullOrWhiteSpace(s) ||
                p.FirstName.Contains(s) ||
                p.LastName.Contains(s))
            .Select(p => Map(p))
            .ToListAsync();

    public async Task<PatientResponseDto> GetAsync(int id) =>
        Map(await Find(id));

    public async Task<PatientResponseDto> CreateAsync(PatientRequestDto d)
    {
        Validate(d);

        var p = new Patient();
        Copy(p, d);

        db.Patients.Add(p);
        await db.SaveChangesAsync();

        return Map(p);
    }

    public async Task UpdateAsync(int id, PatientRequestDto d)
    {
        Validate(d);

        Copy(await Find(id), d);
        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        db.Patients.Remove(await Find(id));
        await db.SaveChangesAsync();
    }

    private async Task<Patient> Find(int id) =>
        await db.Patients.FindAsync(id)
        ?? throw new NotFoundException("Patient not found.");

    private static void Validate(PatientRequestDto d)
    {
        if (d.DateOfBirth == default ||
            d.DateOfBirth > DateOnly.FromDateTime(DateTime.UtcNow))
        {
            throw new ArgumentException(
                "Date of birth must be a date in the past.");
        }
    }

    private static void Copy(Patient p, PatientRequestDto d)
    {
        p.FirstName = d.FirstName.Trim();
        p.LastName = d.LastName.Trim();
        p.DateOfBirth = d.DateOfBirth;
        p.Gender = d.Gender.Trim();
        p.PhoneNumber = d.PhoneNumber;
    }
}

public interface IVitalSignService
{
    Task<IEnumerable<VitalSignResponseDto>> GetAllAsync(int patientId);
    Task<VitalSignResponseDto> GetAsync(int id);
    Task<VitalSignResponseDto> CreateAsync(
        int patientId,
        VitalSignRequestDto dto);
    Task UpdateAsync(int id, VitalSignRequestDto dto);
    Task DeleteAsync(int id);
}

public class VitalSignService(ApplicationDbContext db) : IVitalSignService
{
    private static VitalSignResponseDto Map(VitalSign x) =>
        new(
            x.Id,
            x.PatientId,
            x.HeartRate,
            x.SystolicBloodPressure,
            x.DiastolicBloodPressure,
            x.TemperatureCelsius,
            x.RecordedAt);

    public async Task<IEnumerable<VitalSignResponseDto>> GetAllAsync(int p) =>
        await db.VitalSigns
            .AsNoTracking()
            .Where(x => x.PatientId == p)
            .Select(x => Map(x))
            .ToListAsync();

    public async Task<VitalSignResponseDto> GetAsync(int i) =>
        Map(await Find(i));

    public async Task<VitalSignResponseDto> CreateAsync(
        int p,
        VitalSignRequestDto d)
    {
        if (!await db.Patients.AnyAsync(x => x.Id == p))
            throw new NotFoundException("Patient not found.");

        Validate(d);

        var x = new VitalSign
        {
            PatientId = p
        };

        Copy(x, d);

        db.VitalSigns.Add(x);
        await db.SaveChangesAsync();

        return Map(x);
    }

    public async Task UpdateAsync(int i, VitalSignRequestDto d)
    {
        Validate(d);

        Copy(await Find(i), d);
        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int i)
    {
        db.VitalSigns.Remove(await Find(i));
        await db.SaveChangesAsync();
    }

    private async Task<VitalSign> Find(int i) =>
        await db.VitalSigns.FindAsync(i)
        ?? throw new NotFoundException("Vital sign not found.");

    private static void Validate(VitalSignRequestDto d)
    {
        if (d.RecordedAt == default ||
            d.RecordedAt > DateTime.UtcNow.AddMinutes(5))
        {
            throw new ArgumentException(
                "Recorded time must be a valid current or past time.");
        }

        if (d.DiastolicBloodPressure >= d.SystolicBloodPressure)
        {
            throw new ArgumentException(
                "Diastolic blood pressure must be lower than systolic blood pressure.");
        }
    }

    private static void Copy(VitalSign x, VitalSignRequestDto d)
    {
        x.HeartRate = d.HeartRate;
        x.SystolicBloodPressure = d.SystolicBloodPressure;
        x.DiastolicBloodPressure = d.DiastolicBloodPressure;
        x.TemperatureCelsius = d.TemperatureCelsius;
        x.RecordedAt = d.RecordedAt;
    }
}

public interface IMedicationService
{
    Task<IEnumerable<MedicationResponseDto>> GetAllAsync(
        int patientId,
        string? search);

    Task<MedicationResponseDto> GetAsync(int id);

    Task<MedicationResponseDto> CreateAsync(
        int patientId,
        MedicationRequestDto dto);

    Task UpdateAsync(int id, MedicationRequestDto dto);
    Task DeleteAsync(int id);
}

public class MedicationService(ApplicationDbContext db) : IMedicationService
{
    private static MedicationResponseDto Map(Medication x) =>
        new(
            x.Id,
            x.PatientId,
            x.Name,
            x.Dosage,
            x.Frequency,
            x.StartDate,
            x.EndDate);

    public async Task<IEnumerable<MedicationResponseDto>> GetAllAsync(
        int p,
        string? s) =>
        await db.Medications
            .AsNoTracking()
            .Where(x =>
                x.PatientId == p &&
                (string.IsNullOrWhiteSpace(s) || x.Name.Contains(s)))
            .Select(x => Map(x))
            .ToListAsync();

    public async Task<MedicationResponseDto> GetAsync(int i) =>
        Map(await Find(i));

    public async Task<MedicationResponseDto> CreateAsync(
        int p,
        MedicationRequestDto d)
    {
        if (!await db.Patients.AnyAsync(x => x.Id == p))
            throw new NotFoundException("Patient not found.");

        Validate(d);

        var x = new Medication
        {
            PatientId = p
        };

        Copy(x, d);

        db.Medications.Add(x);
        await db.SaveChangesAsync();

        return Map(x);
    }

    public async Task UpdateAsync(int i, MedicationRequestDto d)
    {
        Validate(d);

        Copy(await Find(i), d);
        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int i)
    {
        db.Medications.Remove(await Find(i));
        await db.SaveChangesAsync();
    }

    private async Task<Medication> Find(int i) =>
        await db.Medications.FindAsync(i)
        ?? throw new NotFoundException("Medication not found.");

    private static void Validate(MedicationRequestDto d)
    {
        if (d.StartDate == default)
            throw new ArgumentException("Start date is required.");

        if (d.EndDate is not null && d.EndDate < d.StartDate)
        {
            throw new ArgumentException(
                "End date cannot be before start date.");
        }
    }

    private static void Copy(Medication x, MedicationRequestDto d)
    {
        x.Name = d.Name.Trim();
        x.Dosage = d.Dosage.Trim();
        x.Frequency = d.Frequency.Trim();
        x.StartDate = d.StartDate;
        x.EndDate = d.EndDate;
    }
}

public interface IAppointmentService
{
    Task<IEnumerable<AppointmentResponseDto>> GetAllAsync(
        int patientId,
        string? status);

    Task<AppointmentResponseDto> GetAsync(int id);

    Task<AppointmentResponseDto> CreateAsync(
        int patientId,
        AppointmentRequestDto dto);

    Task UpdateAsync(int id, AppointmentRequestDto dto);
    Task DeleteAsync(int id);
}

public class AppointmentService(ApplicationDbContext db) : IAppointmentService
{
    private static AppointmentResponseDto Map(Appointment x) =>
        new(
            x.Id,
            x.PatientId,
            x.ScheduledAt,
            x.ClinicianName,
            x.Reason,
            x.Status);

    public async Task<IEnumerable<AppointmentResponseDto>> GetAllAsync(
        int p,
        string? s) =>
        await db.Appointments
            .AsNoTracking()
            .Where(x =>
                x.PatientId == p &&
                (string.IsNullOrWhiteSpace(s) || x.Status == s))
            .OrderBy(x => x.ScheduledAt)
            .Select(x => Map(x))
            .ToListAsync();

    public async Task<AppointmentResponseDto> GetAsync(int i) =>
        Map(await Find(i));

    public async Task<AppointmentResponseDto> CreateAsync(
        int p,
        AppointmentRequestDto d)
    {
        if (!await db.Patients.AnyAsync(x => x.Id == p))
            throw new NotFoundException("Patient not found.");

        Validate(d);

        var x = new Appointment
        {
            PatientId = p
        };

        Copy(x, d);

        db.Appointments.Add(x);
        await db.SaveChangesAsync();

        return Map(x);
    }

    public async Task UpdateAsync(int i, AppointmentRequestDto d)
    {
        Validate(d);

        Copy(await Find(i), d);
        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int i)
    {
        db.Appointments.Remove(await Find(i));
        await db.SaveChangesAsync();
    }

    private async Task<Appointment> Find(int i) =>
        await db.Appointments.FindAsync(i)
        ?? throw new NotFoundException("Appointment not found.");

    private static void Validate(AppointmentRequestDto d)
    {
        if (d.ScheduledAt == default)
            throw new ArgumentException("Scheduled time is required.");
    }

    private static void Copy(Appointment x, AppointmentRequestDto d)
    {
        x.ScheduledAt = d.ScheduledAt;
        x.ClinicianName = d.ClinicianName.Trim();
        x.Reason = d.Reason.Trim();
        x.Status = d.Status;
    }
}