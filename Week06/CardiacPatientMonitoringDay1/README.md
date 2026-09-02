# Week 06 Day 1: Sprint 1 Planning and Database Design

## Overview

Day 1 starts Sprint 1 for the Cardiac Patient Monitoring API. The focus was on defining the Sprint 1 goal, inspecting the existing EF Core database, reviewing the current schema and relationships, applying normalization principles, and preparing an ERD for the project.

## Project

**Cardiac Patient Monitoring API**

## Technology

* ASP.NET Core
* Entity Framework Core
* SQL Server
* ASP.NET Core Identity
* dbdiagram.io / DBML

## Learning Objectives

By the end of this day, the developer should be able to:

* Define a measurable Sprint goal and Definition of Done.
* Break the Sprint scope into small, realistic backlog tasks.
* Inspect EF Core entities, DbContext, Identity configuration, migrations, and model snapshots.
* Apply 1NF, 2NF, and 3NF principles to a relational schema.
* Create an ERD that matches the actual database.
* Identify gaps between the current implementation and the future professional baseline.

## Sprint 1 Goal

> By the end of Sprint 1, the Cardiac Patient Monitoring API will have a complete, documented database schema for patients, vital signs, medications, appointments, and users, with the relationships and constraints required to support the core monitoring workflow.

## Work Completed

### 1. Sprint Planning

Defined the Sprint 1 goal and broke the work into tasks sized between approximately half a day and one day.

### 2. Database Inspection

Reviewed:

* Entity classes
* `ApplicationDbContext`
* Identity configuration
* Fluent API configuration
* Data annotations
* Initial migration
* Migration designer
* Model snapshot
* SQL Server configuration

### 3. Current Database Schema

The current database contains four application tables:

* `Patients`
* `VitalSigns`
* `Medications`
* `Appointments`

ASP.NET Core Identity also creates the required Identity tables:

* `AspNetUsers`
* `AspNetRoles`
* `AspNetRoleClaims`
* `AspNetUserClaims`
* `AspNetUserLogins`
* `AspNetUserRoles`
* `AspNetUserTokens`

EF Core also maintains `__EFMigrationsHistory`.

### 4. Relationships

The current domain relationships are:

* One `Patient` has many `VitalSigns`.
* One `Patient` has many `Medications`.
* One `Patient` has many `Appointments`.
* The three relationships use required foreign keys with cascade delete.
* `Patients` is currently not linked to `AspNetUsers`.

### 5. Normalization Review

The current schema was reviewed against:

* **1NF:** Columns contain atomic values and repeating data is represented through related rows.
* **2NF:** Non-key attributes depend on the complete key.
* **3NF:** Data is stored in the entity it describes without unnecessary duplication.

### 6. ERD

The current database schema was documented using DBML for dbdiagram.io.

The ERD represents the actual implemented schema rather than the future professional baseline.

The DBML and detailed schema documentation are available in:

**`Hands-On Lab.md`**

### 7. Future Professional Baseline

The future baseline was documented separately from the current implementation.

Potential future entities include:

* Clinician
* PatientClinicianAssignment
* MonitoringDevice
* DeviceReading
* MedicalCondition
* Allergy
* RiskAssessment
* AlertRule
* Alert
* Notification
* CarePlan
* ClinicalNote
* AuditLog

These entities are not included in the current ERD because they do not yet exist in the implemented code and migrations.

## Issues Identified

The inspection identified several follow-up items:

1. `Appointments.ClinicianName` is currently free text rather than a foreign key to a clinician entity.
2. Identity role tables exist, but role services are not currently registered through `AddIdentityCore<ApplicationUser>()`.
3. EF Core InMemory tests do not validate SQL Server-specific relational behavior.
4. `DateTime` values are mapped to `datetime2`, so consistent UTC handling is required.
5. There are no domain-specific unique constraints beyond the current Identity indexes.
6. The current entity model, migration, designer, and snapshot are aligned with no pending model changes.

## Useful Commands

Run the API:

```powershell
dotnet run --project CardiacPatientMonitoring.Api/CardiacPatientMonitoring.Api.csproj
```

Run tests:

```powershell
dotnet test CardiacPatientMonitoring.Tests/CardiacPatientMonitoring.Tests.csproj --nologo
```

Check for pending model changes:

```powershell
dotnet ef migrations has-pending-model-changes --project CardiacPatientMonitoring.Api/CardiacPatientMonitoring.Api.csproj
```

## Definition of Done

Day 1 is complete when:

* The Sprint 1 goal is defined.
* The current database schema is documented.
* The future professional baseline is identified separately.
* Normalization has been reviewed.
* The ERD matches the implemented schema.
* Keys, relationships, indexes, and delete behavior are documented.
* Sprint 1 work is divided into appropriately sized tasks.
* Known risks and inconsistencies are recorded for follow-up.

## Deliverables

* `README.md`
* `Hands-On Lab.md`
* Sprint 1 backlog
* Normalization review
* Current database schema
* dbdiagram.io DBML
* ERD
* Identified risks and follow-up items
