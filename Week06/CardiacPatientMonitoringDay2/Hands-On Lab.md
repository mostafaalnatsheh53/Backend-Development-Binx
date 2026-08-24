# Hands-On Lab: Build the Full EF Core Model

## Goal
Create the full EF Core model for the Cardiac Patient Monitoring ERD, configure explicit relationships, seed a reference table, generate a migration, and verify the database schema.

---

## 1. Implement entity classes for every table in the Day 1 ERD

Create model classes for the core tables in the application.

### Example

```csharp
using Microsoft.AspNetCore.Identity;

namespace CardiacPatientMonitoring.Api.Models;

public class ApplicationUser : IdentityUser
{
}

public class Patient
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }

    public ICollection<VitalSign> VitalSigns { get; set; } = new List<VitalSign>();
    public ICollection<Medication> Medications { get; set; } = new List<Medication>();
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}

public class VitalSign
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public decimal HeartRate { get; set; }
    public decimal SystolicBloodPressure { get; set; }
    public decimal DiastolicBloodPressure { get; set; }
    public decimal? TemperatureCelsius { get; set; }
    public DateTime RecordedAt { get; set; }

    public Patient Patient { get; set; } = null!;
}

public class Medication
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }

    public Patient Patient { get; set; } = null!;
}

public class Appointment
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public DateTime ScheduledAt { get; set; }
    public string ClinicianName { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = "Scheduled";

    public Patient Patient { get; set; } = null!;
}
```

### Important points
- each ERD table should have an entity class
- navigation properties should exist on both sides of the relationship
- `ApplicationUser` is included because the project uses ASP.NET Core Identity

---

## 2. Configure at least 2 relationships explicitly via the Fluent API

Add relationship configuration inside `ApplicationDbContext.OnModelCreating`.

### Example

```csharp
protected override void OnModelCreating(ModelBuilder b)
{
    base.OnModelCreating(b);

    b.Entity<Patient>()
        .HasMany(p => p.VitalSigns)
        .WithOne(v => v.Patient)
        .HasForeignKey(v => v.PatientId)
        .OnDelete(DeleteBehavior.Cascade);

    b.Entity<Patient>()
        .HasMany(p => p.Medications)
        .WithOne(m => m.Patient)
        .HasForeignKey(m => m.PatientId)
        .OnDelete(DeleteBehavior.Cascade);

    b.Entity<Patient>()
        .HasMany(p => p.Appointments)
        .WithOne(a => a.Patient)
        .HasForeignKey(a => a.PatientId)
        .OnDelete(DeleteBehavior.Restrict);
}
```

### Delete behavior decision
- `Cascade` is used for `VitalSigns` and `Medications` because these are dependent child records.
- `Restrict` is used for `Appointments` to prevent accidental deletion of clinical schedule history.

---

## 3. Add seed data for at least one reference table using HasData

Create a lookup/reference table and populate it with default values.

### Example

```csharp
public class CareCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
```

```csharp
public DbSet<CareCategory> CareCategories => Set<CareCategory>();

b.Entity<CareCategory>()
    .Property(c => c.Name)
    .HasMaxLength(80)
    .IsRequired();

b.Entity<CareCategory>().HasData(
    new CareCategory { Id = 1, Name = "Low Risk", Description = "Stable patient with no immediate concern." },
    new CareCategory { Id = 2, Name = "Moderate Risk", Description = "Requires monitoring and periodic review." },
    new CareCategory { Id = 3, Name = "High Risk", Description = "Urgent follow-up or escalation is recommended." }
);
```

---

## 4. Generate the initial migration and review the generated file before applying it

Run the EF Core command:

```powershell
dotnet ef migrations add Day2EntitySetup --project CardiacPatientMonitoring.Api --startup-project CardiacPatientMonitoring.Api
```

### Review checklist
Open the generated migration file and confirm:
- `CareCategories` table exists
- seed values are inserted correctly
- foreign keys are created with the intended behavior
- appointment delete behavior is `Restrict`

### Example migration result

```csharp
migrationBuilder.CreateTable(
    name: "CareCategories",
    columns: table => new
    {
        Id = table.Column<int>(type: "int", nullable: false)
            .Annotation("SqlServer:Identity", "1, 1"),
        Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
        Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
    },
    constraints: table =>
    {
        table.PrimaryKey("PK_CareCategories", x => x.Id);
    });
```

---

## 5. Apply the migration and confirm the schema in a database GUI client matches your ERD

Apply the migration:

```powershell
dotnet ef database update --project CardiacPatientMonitoring.Api --startup-project CardiacPatientMonitoring.Api
```

### Verify using database metadata or a GUI tool
Use SQL Server Management Studio, DBeaver, Azure Data Studio, or a similar client.

Check that the following tables exist:
- Patients
- VitalSigns
- Medications
- Appointments
- CareCategories
- ASP.NET Identity tables

Check that the foreign key mappings are:
- `VitalSigns.PatientId -> Patients.Id`
- `Medications.PatientId -> Patients.Id`
- `Appointments.PatientId -> Patients.Id`

Check that seed values are inside `CareCategories`:
- Low Risk
- Moderate Risk
- High Risk

### Example validation query

```sql
SELECT TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE';

SELECT TABLE_NAME, COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME IN ('Patients', 'VitalSigns', 'Medications', 'Appointments', 'CareCategories');

SELECT fk.TABLE_NAME AS ChildTable,
       ccu.COLUMN_NAME AS ChildColumn,
       pk.TABLE_NAME AS ParentTable,
       pcu.COLUMN_NAME AS ParentColumn
FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS rc
JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS fk ON rc.CONSTRAINT_NAME = fk.CONSTRAINT_NAME
JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS pk ON rc.UNIQUE_CONSTRAINT_NAME = pk.CONSTRAINT_NAME
JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE ccu ON rc.CONSTRAINT_NAME = ccu.CONSTRAINT_NAME
JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE pcu ON rc.UNIQUE_CONSTRAINT_NAME = pcu.CONSTRAINT_NAME;
```

---

## Final result
The project now includes:
- entity classes with navigation properties
- explicit Fluent API relationships
- seeded reference data
- a generated migration
- schema validation against the ERD design

The solution builds successfully and the database schema is aligned with the intended model.
