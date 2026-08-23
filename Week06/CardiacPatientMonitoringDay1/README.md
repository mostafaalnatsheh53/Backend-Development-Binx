# Week 06 Day 1: Sprint 1 Planning and Database Design

## Overview

This day starts Sprint 1 for the Cardiac Patient Monitoring capstone. The work focuses on defining what done means for the sprint, inspecting the existing EF Core database, documenting the complete professional schema, and preparing an ERD that can remain aligned with the real database throughout the project.

**Duration:** 8 hours  
**Project:** Cardiac Patient Monitoring API  
**Technology:** ASP.NET Core, Entity Framework Core, SQL Server, ASP.NET Core Identity, dbdiagram.io DBML

## Learning Objectives

By the end of this day, the developer should be able to:

- Run Sprint Planning and write a single measurable sprint goal.
- Turn the sprint goal into a realistic backlog with tasks sized from half a day to one day.
- Inspect entity classes, `DbContext`, Identity configuration, migrations, and model snapshots.
- Design a normalized relational schema using 1NF, 2NF, and 3NF principles.
- Create and maintain an ERD that represents the actual database rather than an assumed design.
- Identify differences between the current implementation and the future professional baseline.

## Day Schedule

| Time | Activity | Output |
|---|---|---|
| 1 hour | Sprint Planning | One-sentence Sprint 1 goal and definition of done |
| 1.5 hours | Project and database inspection | Inventory of models, Identity tables, migrations, and configuration |
| 2 hours | Full schema design | Complete entity and relationship design |
| 1.5 hours | Normalization review | 1NF, 2NF, and 3NF decisions and constraint review |
| 1 hour | ERD finalization | dbdiagram.io-compatible DBML |
| 1 hour | Backlog sizing and handoff | Sized backlog, risks, assumptions, and next steps |

## Sprint 1 Goal

> By the end of Sprint 1, the Cardiac Patient Monitoring API will have a complete, documented database schema for patients, vital signs, medications, appointments, and users, with the relationships and constraints required to support the core monitoring workflow.

For this day, "done" means that the current database has been inspected, the schema is documented, the ERD matches the migration, and the remaining Sprint 1 implementation work is broken into small, checkable tasks.

## What Was Inspected

The database design was verified against:

- Entity classes in `CardiacPatientMonitoring.Api/Models/Entities.cs`.
- `ApplicationDbContext` and its Identity base class.
- `ApplicationDbContextFactory` and SQL Server configuration.
- Fluent API configuration in `OnModelCreating`.
- Data annotations on the entity classes.
- The `InitialCreate` migration.
- The generated migration designer and model snapshot.
- `Program.cs`, including Identity registration and the test database provider.
- `appsettings.json` and the SQL Server connection string.

## Actual Current Database

The current migration creates four application tables:

- `Patients`
- `VitalSigns`
- `Medications`
- `Appointments`

Because the context inherits from `IdentityDbContext<ApplicationUser>`, it also creates these Identity tables:

- `AspNetUsers`
- `AspNetRoles`
- `AspNetRoleClaims`
- `AspNetUserClaims`
- `AspNetUserLogins`
- `AspNetUserRoles`
- `AspNetUserTokens`

EF Core also creates `__EFMigrationsHistory` as infrastructure metadata. The current application entities use integer identity primary keys. The Identity entities use string keys, with composite keys for user logins, user roles, and user tokens.

The current patient relationships are:

- One patient to many vital-sign records.
- One patient to many medication records.
- One patient to many appointment records.
- All three relationships use required foreign keys and cascade delete.
- There is currently no foreign-key relationship between `AspNetUsers` and `Patients`.

## Normalization Decisions

The schema review applies the principles from Week 3:

- **1NF:** Each column stores one atomic value. Repeated measurements and medications are represented as rows, not comma-separated values.
- **2NF:** Non-key fields depend on the complete key. Identity junction tables use composite primary keys for their relationships.
- **3NF:** Descriptive data is stored in the table identified by its key. Patient data is not duplicated into vital-sign, medication, or appointment rows.

The ERD documents the current schema exactly. Future entities such as clinicians, monitoring devices, alerts, risk assessments, care plans, and clinical notes are listed separately in the lab as future professional-baseline scope; they are not added to the current DBML until they exist in the code and migrations.

## ERD and Lab Deliverables

The complete lab deliverables are in [Hands-On Lab.md](Hands-On%20Lab.md), including:

- Full professional-baseline entity inventory.
- Normalized schema notes.
- Complete dbdiagram.io DBML for the current database.
- Primary keys, foreign keys, indexes, nullability, and delete behavior.
- Relationship summary.
- Issues and inconsistencies found during inspection.
- Sprint 1 backlog tasks sized between half a day and one day.

To create the visual ERD:

1. Open [dbdiagram.io](https://dbdiagram.io/).
2. Copy the DBML from the "Actual Current Database Schema and ERD" section of `Hands-On Lab.md`.
3. Paste it into a new diagram.
4. Confirm that the generated diagram contains the four domain tables, seven Identity tables, and the migration-history table.
5. Keep the DBML updated whenever a migration changes the database.

## Sprint 1 Backlog

The backlog is maintained in `Hands-On Lab.md`. It contains twelve tasks, each estimated at `0.5 day` or `1 day`, covering:

- Database artifact and Identity inspection.
- Documentation of each current application table.
- Normalization and constraint review.
- ERD creation and migration comparison.
- Risk documentation and Sprint 1 handoff.

## Issues Found

The inspection identified these follow-up items:

1. `Appointments.ClinicianName` is free text and is not a foreign key to a clinician entity.
2. Identity role tables exist, but runtime registration uses `AddIdentityCore<ApplicationUser>()` without registering role services.
3. EF Core InMemory tests do not validate SQL Server relational constraints, indexes, precision, cascade deletes, or migrations.
4. `DateTime` maps to `datetime2`, so timezone information is not stored by SQL Server; UTC handling must be consistent.
5. No domain-specific unique constraints exist beyond the Identity indexes.
6. The current model, migration, designer, and snapshot are aligned, and no pending model changes were found.

## Useful Commands

Run the API project:

```powershell
dotnet run --project CardiacPatientMonitoring.Api/CardiacPatientMonitoring.Api.csproj
```

Run the test project:

```powershell
dotnet test CardiacPatientMonitoring.Tests/CardiacPatientMonitoring.Tests.csproj --nologo
```

Check for pending EF Core model changes:

```powershell
dotnet ef migrations has-pending-model-changes --project CardiacPatientMonitoring.Api/CardiacPatientMonitoring.Api.csproj
```

## Definition of Done

Day 1 is complete when:

- The Sprint 1 goal is written.
- The full current database schema is documented.
- The future professional baseline is identified without confusing it with the current implementation.
- The schema follows normalization principles.
- A complete DBML ERD is available for dbdiagram.io.
- Relationships, keys, indexes, nullability, and delete behavior are recorded.
- Sprint 1 work is divided into tasks sized from half a day to one day.
- Known risks and inconsistencies are documented for the next sprint.
