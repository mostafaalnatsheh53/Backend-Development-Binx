using CardiacPatientMonitoring.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CardiacPatientMonitoring.Api.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    private const string AdminRoleId = "07abcc84-88db-460f-b3ab-8730cd53cc24";
    private const string PatientRoleId = "60f03c42-071e-486e-8d00-9e289462b568";
    private const string SeedPatientUserId = "a3d32497-2d0d-43cb-b1d3-660939cfe1d3";

    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<VitalSign> VitalSigns => Set<VitalSign>();
    public DbSet<Medication> Medications => Set<Medication>();
    public DbSet<Appointment> Appointments => Set<Appointment>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        b.Entity<Patient>()
            .Property(p => p.FirstName)
            .HasMaxLength(80)
            .IsRequired();

        b.Entity<Patient>()
            .Property(p => p.LastName)
            .HasMaxLength(80)
            .IsRequired();

        b.Entity<Patient>()
            .HasOne(p => p.User)
            .WithOne(u => u.Patient)
            .HasForeignKey<Patient>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        b.Entity<VitalSign>()
            .Property(v => v.HeartRate)
            .HasPrecision(5, 2);

        b.Entity<VitalSign>()
            .Property(v => v.SystolicBloodPressure)
            .HasPrecision(5, 2);

        b.Entity<VitalSign>()
            .Property(v => v.DiastolicBloodPressure)
            .HasPrecision(5, 2);

        b.Entity<VitalSign>()
            .Property(v => v.TemperatureCelsius)
            .HasPrecision(4, 1);

        b.Entity<Medication>()
            .Property(m => m.Name)
            .HasMaxLength(120)
            .IsRequired();

        b.Entity<Medication>()
            .HasIndex(m => new { m.PatientId, m.Name });

        b.Entity<Appointment>()
            .Property(a => a.Status)
            .HasMaxLength(30)
            .IsRequired();

        b.Entity<Appointment>()
            .HasIndex(a => new { a.PatientId, a.ScheduledAt });

        b.Entity<IdentityRole>().HasData(
            new IdentityRole
            {
                Id = AdminRoleId,
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = "787bf9f0-a6dc-427a-a9b3-c47ec9d145d3"
            },
            new IdentityRole
            {
                Id = PatientRoleId,
                Name = "Patient",
                NormalizedName = "PATIENT",
                ConcurrencyStamp = "83e95786-f6f2-4404-9f2c-c7a437e3a2eb"
            });

        b.Entity<ApplicationUser>().HasData(new ApplicationUser
        {
            Id = SeedPatientUserId,
            UserName = "alex.taylor@example.invalid",
            NormalizedUserName = "ALEX.TAYLOR@EXAMPLE.INVALID",
            Email = "alex.taylor@example.invalid",
            NormalizedEmail = "ALEX.TAYLOR@EXAMPLE.INVALID",
            EmailConfirmed = true,
            SecurityStamp = "f0525098-66e0-4272-b847-65c6354c868a",
            ConcurrencyStamp = "215efe58-17b6-49f0-a59e-5063c5043d69"
        });

        b.Entity<Patient>().HasData(new Patient
        {
            Id = 1,
            FirstName = "Alex",
            LastName = "Taylor",
            DateOfBirth = new DateOnly(1982, 5, 14),
            Gender = "Other",
            PhoneNumber = "555-0101",
            UserId = SeedPatientUserId
        });

        b.Entity<VitalSign>().HasData(new VitalSign
        {
            Id = 1,
            PatientId = 1,
            HeartRate = 72,
            SystolicBloodPressure = 120,
            DiastolicBloodPressure = 80,
            TemperatureCelsius = 36.8m,
            RecordedAt = new DateTime(
                2026, 1, 15, 9, 0, 0, DateTimeKind.Utc)
        });

        b.Entity<Medication>().HasData(new Medication
        {
            Id = 1,
            PatientId = 1,
            Name = "Sample Medication",
            Dosage = "10 mg",
            Frequency = "Once daily",
            StartDate = new DateOnly(2026, 1, 1)
        });

        b.Entity<Appointment>().HasData(new Appointment
        {
            Id = 1,
            PatientId = 1,
            ScheduledAt = new DateTime(
                2026, 2, 1, 10, 0, 0, DateTimeKind.Utc),
            ClinicianName = "Dr. Morgan",
            Reason = "Routine review",
            Status = "Scheduled"
        });
    }
}
