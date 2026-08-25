# Day 3 - Cardiac Patient Monitoring API

## Paginated patient catalog

The authenticated `GET /api/patients` endpoint returns a paginated patient catalog. It projects database rows into `PatientResponseDto` values and supports optional search, filtering, and sorting parameters.

### Query parameters

- `page`: 1-based page number; defaults to `1`
- `pageSize`: number of results per page from `1` to `100`; defaults to `10`
- `search`: searches first and last name
- `gender`: exact gender filter
- `bornBefore`: date-of-birth upper bound in `yyyy-MM-dd` format
- `sort`: `nameAsc`, `nameDesc`, `oldest`, or `newest`; defaults to `nameAsc`

### Postman checks

After calling `Login` and setting the collection `token` variable, run these requests:

```text
GET {{baseUrl}}/api/patients?page=1&pageSize=2
GET {{baseUrl}}/api/patients?page=1&pageSize=5&gender=Female&sort=nameDesc
GET {{baseUrl}}/api/patients?page=2&pageSize=2&search=an&bornBefore=1995-01-01&sort=oldest
```

Each response contains `items`, `page`, `pageSize`, `totalCount`, and `totalPages`. These requests cover plain pagination, two filters with descending sorting, and combined filters with ascending date sorting.

## Overview
This project continues the Cardiac Patient Monitoring API work by implementing the EF Core data model, explicit relationship configuration, reference-table seeding, and migration validation.

The main focus of Day 2 is to build the actual database model that matches the ERD created in Day 1 and ensure the schema is consistent with the application code.

---

## Objectives
By the end of this day, the project should:
- implement entity classes for all core tables
- include navigation properties for related entities
- configure relationships explicitly with Fluent API
- define delete behavior for one-to-many relationships
- seed at least one reference table using `HasData`
- generate and review a migration
- apply the migration and verify the database schema

---

## Project Structure

```text
CardiacPatientMonitoringDay2/
├── CardiacPatientMonitoring.Api/
│   ├── Data/
│   │   └── ApplicationDbContext.cs
│   ├── Models/
│   │   └── Entities.cs
│   ├── Migrations/
│   ├── Program.cs
│   ├── appsettings.json
│   └── CardiacPatientMonitoring.Api.csproj
├── CardiacPatientMonitoring.Tests/
├── CardiacPatientMonitoring.postman_collection.json
├── CardiacPatientMonitoring.slnx
├── ERD/
├── Hands-On Lab.md
├── README.md
└── CardiacPatientMonitoring_Report.pdf
```

---

## Core Domain Model
The application includes the main entities:

- `ApplicationUser`
- `Patient`
- `VitalSign`
- `Medication`
- `Appointment`
- `CareCategory`

### Example entity setup

```csharp
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
```

Each child entity has a foreign key pointing to `Patient` and a navigation property back to the parent.

---

## Relationship Configuration
The relationships are configured inside `ApplicationDbContext.OnModelCreating` using Fluent API.

### Example

```csharp
b.Entity<Patient>()
    .HasMany(p => p.VitalSigns)
    .WithOne(v => v.Patient)
    .HasForeignKey(v => v.PatientId)
    .OnDelete(DeleteBehavior.Cascade);

b.Entity<Patient>()
    .HasMany(p => p.Appointments)
    .WithOne(a => a.Patient)
    .HasForeignKey(a => a.PatientId)
    .OnDelete(DeleteBehavior.Restrict);
```

### Delete behavior decision
- `Cascade` was chosen for dependent child records such as `VitalSigns` and `Medications`.
- `Restrict` was chosen for `Appointments` to avoid accidental deletion of medical schedule history.

---

## Seed Data
A reference table called `CareCategory` was added and populated using `HasData`.

### Example

```csharp
b.Entity<CareCategory>().HasData(
    new CareCategory { Id = 1, Name = "Low Risk", Description = "Stable patient with no immediate concern." },
    new CareCategory { Id = 2, Name = "Moderate Risk", Description = "Requires monitoring and periodic review." },
    new CareCategory { Id = 3, Name = "High Risk", Description = "Urgent follow-up or escalation is recommended." }
);
```

This creates default categories immediately when the database is created or updated.

---

## Migration Workflow
The project includes EF Core migrations for the model changes.

### Generate migration

```powershell
dotnet ef migrations add Day2EntitySetup --project CardiacPatientMonitoring.Api --startup-project CardiacPatientMonitoring.Api
```

### Review migration file
Before applying, check the generated migration and confirm that:
- the new `CareCategories` table exists
- the `InsertData` seed records are present
- the foreign keys are correct
- the delete behavior matches the expected design

### Apply migration

```powershell
dotnet ef database update --project CardiacPatientMonitoring.Api --startup-project CardiacPatientMonitoring.Api
```

---

## Database Verification
After applying the migration, the schema can be checked in SQL Server or in a database GUI client.

### Confirm these items
- `Patients` table exists
- `VitalSigns` table exists
- `Medications` table exists
- `Appointments` table exists
- `CareCategories` table exists
- `PatientId` foreign keys point to the `Patients` table
- seed values are present in `CareCategories`

### Example metadata query

```sql
SELECT TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE';

SELECT TABLE_NAME, COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME IN ('Patients', 'VitalSigns', 'Medications', 'Appointments', 'CareCategories');
```

---

## Verification Status
The project was validated with:

```powershell
dotnet build
```

Result:
- Build succeeded
- all project files compiled successfully

---

## Learning Outcomes
After completing Day 2, the developer should understand:
- how to model tables as EF Core entities
- how to use navigation properties
- how to configure relationships through Fluent API
- how to define delete behavior intentionally
- how to seed reference-table data with `HasData`
- how to create and review migrations
- how to verify the final schema in SQL Server

---

## Summary
Day 2 focuses on turning the ERD into a working EF Core data model. The final result is a project where the database structure matches the intended design and is ready for future API development, business logic, and service layers.
