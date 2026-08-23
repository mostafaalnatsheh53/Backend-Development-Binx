# Hands-On Lab: Sprint 1 Planning & Full Schema Design

## 1. Sprint 1 Goal

**By the end of Sprint 1, the Cardiac Patient Monitoring API will have a complete, documented database schema for patients, vital signs, medications, appointments, and users, with the relationships and constraints required to support the core monitoring workflow.**

## 2. Sprint 1 Backlog Board

This Markdown board replaces the Notion/Trello board for Sprint 1 planning.

| Backlog Item | Description | Estimate | Priority | Status | Definition of Done |
|---|---|---:|---:|---|---|
| Inventory current database artifacts | Inspect models, `DbContext`, Identity setup, migrations, snapshot, and connection configuration. | 0.5 day | High | To Do | Every current table and database source is listed. |
| Document Identity schema | Record `AspNetUsers`, roles, claims, logins, user roles, and tokens with their keys and relationships. | 0.5 day | High | To Do | Identity tables and indexes are documented accurately. |
| Document patient schema | Record `Patients` columns, SQL types, nullability, and primary-key definition. | 0.5 day | High | To Do | The patient table matches the migration and model snapshot. |
| Document vital-sign schema | Record `VitalSigns` columns, decimal precision, timestamp, and patient relationship. | 0.5 day | High | To Do | Vital-sign fields, precision, index, and cascade rule are documented. |
| Document medication schema | Record `Medications` columns, date fields, length constraints, and patient relationship. | 0.5 day | Medium | To Do | Medication fields, nullability, index, and cascade rule are documented. |
| Document appointment schema | Record `Appointments` columns, status length, clinician text field, and patient relationship. | 0.5 day | Medium | To Do | Appointment fields and current limitation are documented. |
| Apply normalization review | Check atomic values, key dependencies, duplicated data, and any required junction tables. | 0.5 day | High | To Do | A written 1NF, 2NF, and 3NF review is complete. |
| Define keys and indexes | Consolidate all PKs, FKs, Identity indexes, unique constraints, and delete behaviors. | 0.5 day | High | To Do | The constraint matrix matches EF Core configuration and migrations. |
| Create ERD in DBML | Convert the verified current schema into valid dbdiagram.io DBML. | 1 day | High | To Do | DBML contains every current table, column, index, and relationship. |
| Verify ERD against migration | Compare DBML with `InitialCreate` and the model snapshot, then correct discrepancies. | 0.5 day | High | To Do | No unexplained differences remain between the ERD and migration. |
| Record schema risks | Document free-text clinician names, Identity role registration, InMemory testing, and UTC handling. | 0.5 day | Medium | To Do | Each risk has an impact and a recommended follow-up. |
| Prepare Sprint 1 handoff | Summarize decisions, assumptions, open issues, and the next-sprint implementation tasks. | 0.5 day | Medium | To Do | Documentation is complete and ready for review. |

## 3. Planned Professional Baseline Entities (Future Scope)

The complete Cardiac Patient Monitoring platform needs the following entities across its professional baseline. The first five are the current Sprint 1 core; the remaining entities support production-ready monitoring, clinical workflows, security, and reporting.

| Entity | Responsibility | Sprint 1 |
|---|---|---|
| `ApplicationUser` | Stores authenticated users, Identity credentials, roles, and account security data. | Yes |
| `Patient` | Stores patient demographics, contact details, and links to clinical records. | Yes |
| `Clinician` | Stores care-provider profile, specialty, credentials, and availability. | No |
| `PatientClinicianAssignment` | Connects patients and clinicians and defines the care or access relationship. | No |
| `VitalSign` | Stores timestamped measurements such as heart rate, blood pressure, temperature, and oxygen saturation. | Yes |
| `MonitoringDevice` | Stores registered wearable or medical-device details, serial number, type, status, and ownership. | No |
| `DeviceReading` | Stores readings received from a device before or alongside clinical normalization. | No |
| `Medication` | Stores prescriptions and medication history, including dosage, frequency, and active dates. | Yes |
| `Appointment` | Stores scheduled consultations or follow-ups between patients and clinicians. | Yes |
| `MedicalCondition` | Stores diagnosed or historical cardiac and general medical conditions. | No |
| `Allergy` | Stores patient allergies and intolerances to support medication safety. | No |
| `RiskAssessment` | Stores calculated risk scores, contributing measurements, algorithm version, and assessment time. | No |
| `AlertRule` | Stores configurable thresholds and conditions used to detect abnormal monitoring results. | No |
| `Alert` | Stores abnormal measurements or risk events that require clinical review or action. | No |
| `Notification` | Stores delivery records for alerts, reminders, and care updates through different channels. | No |
| `CarePlan` | Stores patient-specific monitoring goals, interventions, review cadence, and status. | No |
| `ClinicalNote` | Stores clinician observations, assessments, and follow-up notes. | No |
| `AuditLog` | Stores an immutable history of security-sensitive and clinical-data access or modification activity. | No |

### Baseline Design Decisions

- `Appointment` should reference `Clinician` with a foreign key instead of storing only a free-text clinician name.
- `PatientClinicianAssignment` resolves the many-to-many relationship between patients and clinicians.
- `RiskAssessment` persists the result of the risk calculator so scores can be reviewed over time.
- `MonitoringDevice` and `DeviceReading` separate device ownership from the readings produced by that device.
- `AlertRule`, `Alert`, and `Notification` separate detection, clinical review, and message delivery responsibilities.
- `AuditLog` is kept separate from business entities so compliance and security history cannot be confused with clinical data.

## 4. Planned Normalized Schema (Future Scope)

The schema follows the normalization principles studied in Week 3:

- **1NF:** Every column stores one atomic value. Repeating values such as medications, conditions, readings, and roles are stored in related rows rather than comma-separated text.
- **2NF:** Every non-key attribute depends on the whole primary key. Junction tables contain only relationship-specific attributes in addition to their composite key or surrogate key.
- **3NF:** Non-key attributes depend only on their table's key. For example, clinician details are stored once in `Clinician`, and appointment records reference the clinician by foreign key.
- Lookup-like values such as status, role, device type, and notification channel use controlled enums or validated values instead of duplicated descriptive text.

### Identity and Access Tables

| Table | Columns | Key and constraints |
|---|---|---|
| `ApplicationUser` | `UserId`, `Email`, `PasswordHash`, `Role`, `IsActive`, `CreatedAt` | PK `UserId`; unique, required `Email`; required `Role`; password data is never stored as plain text. |
| `Clinician` | `ClinicianId`, `UserId`, `LicenseNumber`, `Specialty`, `FirstName`, `LastName`, `Phone` | PK `ClinicianId`; FK `UserId` to `ApplicationUser`; unique `UserId` and `LicenseNumber`; required license and name fields. |
| `PatientClinicianAssignment` | `PatientId`, `ClinicianId`, `AssignedAt`, `UnassignedAt`, `IsPrimary` | Composite PK (`PatientId`, `ClinicianId`, `AssignedAt`); FKs to `Patient` and `Clinician`; `UnassignedAt` must not precede `AssignedAt`. |

### Patient and Clinical History Tables

| Table | Columns | Key and constraints |
|---|---|---|
| `Patient` | `PatientId`, `UserId`, `MedicalRecordNumber`, `FirstName`, `LastName`, `DateOfBirth`, `Gender`, `Phone`, `Email`, `Address`, `CreatedAt` | PK `PatientId`; optional FK `UserId` to `ApplicationUser`; unique medical record number; required name and date of birth; date of birth cannot be in the future. |
| `MedicalCondition` | `ConditionId`, `PatientId`, `Name`, `Code`, `DiagnosedAt`, `Notes`, `IsActive` | PK `ConditionId`; FK `PatientId`; required condition name; index (`PatientId`, `IsActive`). |
| `Allergy` | `AllergyId`, `PatientId`, `Substance`, `Reaction`, `Severity`, `RecordedAt` | PK `AllergyId`; FK `PatientId`; required substance and reaction; index `PatientId`. |
| `Medication` | `MedicationId`, `PatientId`, `Name`, `DosageAmount`, `DosageUnit`, `Frequency`, `StartDate`, `EndDate`, `PrescribedByClinicianId`, `IsActive` | PK `MedicationId`; FKs to `Patient` and optional `Clinician`; required name, dosage, and start date; `EndDate` cannot precede `StartDate`. |
| `CarePlan` | `CarePlanId`, `PatientId`, `CreatedByClinicianId`, `Title`, `GoalStatement`, `ReviewCadence`, `StartDate`, `EndDate`, `Status` | PK `CarePlanId`; FKs to `Patient` and `Clinician`; required title, goal statement, dates, and status; end date must not precede start date. |
| `ClinicalNote` | `ClinicalNoteId`, `PatientId`, `ClinicianId`, `AppointmentId`, `NoteText`, `CreatedAt`, `UpdatedAt` | PK `ClinicalNoteId`; FKs to `Patient`, `Clinician`, and optional `Appointment`; required note text and author; index (`PatientId`, `CreatedAt`). |

### Monitoring Tables

| Table | Columns | Key and constraints |
|---|---|---|
| `MonitoringDevice` | `DeviceId`, `PatientId`, `SerialNumber`, `DeviceType`, `Manufacturer`, `Model`, `RegisteredAt`, `Status` | PK `DeviceId`; FK `PatientId`; unique serial number; required type and status. |
| `DeviceReading` | `ReadingId`, `DeviceId`, `PatientId`, `RecordedAt`, `HeartRate`, `SystolicPressure`, `DiastolicPressure`, `OxygenSaturation`, `Temperature`, `ReceivedAt` | PK `ReadingId`; FKs to `MonitoringDevice` and `Patient`; required recorded time; numeric values must be within clinically configured ranges; index (`PatientId`, `RecordedAt`). |
| `VitalSign` | `VitalSignId`, `PatientId`, `DeviceReadingId`, `RecordedByUserId`, `RecordedAt`, `HeartRate`, `SystolicPressure`, `DiastolicPressure`, `OxygenSaturation`, `Temperature` | PK `VitalSignId`; FKs to `Patient`, optional `DeviceReading`, and optional `ApplicationUser`; each measurement is one row; index (`PatientId`, `RecordedAt`). |
| `RiskAssessment` | `RiskAssessmentId`, `PatientId`, `VitalSignId`, `Score`, `RiskLevel`, `AlgorithmVersion`, `AssessedAt` | PK `RiskAssessmentId`; FKs to `Patient` and `VitalSign`; required score, level, version, and timestamp; score cannot be negative. |
| `AlertRule` | `AlertRuleId`, `Name`, `Metric`, `Operator`, `Threshold`, `Severity`, `IsActive`, `CreatedByUserId` | PK `AlertRuleId`; FK to `ApplicationUser`; required metric, operator, threshold, and severity; one rule represents one condition. |
| `Alert` | `AlertId`, `PatientId`, `VitalSignId`, `RiskAssessmentId`, `AlertRuleId`, `Severity`, `Message`, `CreatedAt`, `AcknowledgedAt`, `AcknowledgedByUserId`, `Status` | PK `AlertId`; FKs to patient, optional source records, rule, and acknowledging user; required status and creation time; acknowledgement fields are required only when status is acknowledged or resolved. |

### Scheduling and Communication Tables

| Table | Columns | Key and constraints |
|---|---|---|
| `Appointment` | `AppointmentId`, `PatientId`, `ClinicianId`, `ScheduledAt`, `DurationMinutes`, `Purpose`, `Status`, `Notes` | PK `AppointmentId`; FKs to `Patient` and `Clinician`; required date, duration, purpose, and status; duration must be positive. |
| `Notification` | `NotificationId`, `UserId`, `AlertId`, `AppointmentId`, `Channel`, `Subject`, `Body`, `SentAt`, `DeliveredAt`, `Status` | PK `NotificationId`; FKs to `ApplicationUser` and optional `Alert` or `Appointment`; required channel, body, and status; delivery timestamps cannot precede send time. |
| `AuditLog` | `AuditLogId`, `UserId`, `Action`, `EntityName`, `EntityId`, `OccurredAt`, `IpAddress`, `Details` | PK `AuditLogId`; optional FK to `ApplicationUser`; required action, entity name, entity id, and timestamp; append-only and never updated by normal business operations. |

### Relationship Summary

- One `Patient` has many `VitalSign`, `DeviceReading`, `Medication`, `Appointment`, `MedicalCondition`, `Allergy`, `CarePlan`, `ClinicalNote`, `RiskAssessment`, and `Alert` records.
- One `Clinician` has many `Appointment`, `ClinicalNote`, and `Medication` records, while `PatientClinicianAssignment` manages the patient-clinician many-to-many relationship.
- One `MonitoringDevice` produces many `DeviceReading` records; a normalized `VitalSign` may reference one source reading.
- One `VitalSign` can produce multiple `RiskAssessment` or `Alert` records as rules and algorithm versions change.
- One `Alert` can produce many `Notification` records, allowing retries across different delivery channels without duplicating alert data.

## 5. Actual Current Database Schema and ERD

The following schema is the actual current database design, based on the entity classes, `ApplicationDbContext`, Identity configuration, `InitialCreate` migration, and model snapshot. It intentionally excludes the future-scope entities listed above because they do not currently exist in the project.

### A. Complete dbdiagram.io DBML

```dbml
Table Patients {
	Id int [pk, increment, not null]
	FirstName nvarchar(80) [not null]
	LastName nvarchar(80) [not null]
	DateOfBirth date [not null]
	Gender nvarchar [not null, note: 'SQL Server: nvarchar(max)']
	PhoneNumber nvarchar [note: 'SQL Server: nvarchar(max), nullable']
}

Table VitalSigns {
	Id int [pk, increment, not null]
	PatientId int [not null]
	HeartRate decimal(5,2) [not null]
	SystolicBloodPressure decimal(5,2) [not null]
	DiastolicBloodPressure decimal(5,2) [not null]
	TemperatureCelsius decimal(4,1)
	RecordedAt datetime2 [not null]

	indexes {
		PatientId [name: 'IX_VitalSigns_PatientId']
	}
}

Table Medications {
	Id int [pk, increment, not null]
	PatientId int [not null]
	Name nvarchar(120) [not null]
	Dosage nvarchar [not null, note: 'SQL Server: nvarchar(max)']
	Frequency nvarchar [not null, note: 'SQL Server: nvarchar(max)']
	StartDate date [not null]
	EndDate date

	indexes {
		PatientId [name: 'IX_Medications_PatientId']
	}
}

Table Appointments {
	Id int [pk, increment, not null]
	PatientId int [not null]
	ScheduledAt datetime2 [not null]
	ClinicianName nvarchar [not null, note: 'SQL Server: nvarchar(max)']
	Reason nvarchar [not null, note: 'SQL Server: nvarchar(max)']
	Status nvarchar(30) [not null]

	indexes {
		PatientId [name: 'IX_Appointments_PatientId']
	}
}

Table AspNetUsers {
	Id nvarchar(450) [pk, not null]
	UserName nvarchar(256)
	NormalizedUserName nvarchar(256)
	Email nvarchar(256)
	NormalizedEmail nvarchar(256)
	EmailConfirmed bit [not null]
	PasswordHash nvarchar [note: 'SQL Server: nvarchar(max)']
	SecurityStamp nvarchar [note: 'SQL Server: nvarchar(max)']
	ConcurrencyStamp nvarchar [note: 'SQL Server: nvarchar(max)']
	PhoneNumber nvarchar [note: 'SQL Server: nvarchar(max)']
	PhoneNumberConfirmed bit [not null]
	TwoFactorEnabled bit [not null]
	LockoutEnd datetimeoffset
	LockoutEnabled bit [not null]
	AccessFailedCount int [not null]

	indexes {
		NormalizedEmail [name: 'EmailIndex']
		NormalizedUserName [name: 'UserNameIndex', unique]
	}
}

Table AspNetRoles {
	Id nvarchar(450) [pk, not null]
	Name nvarchar(256)
	NormalizedName nvarchar(256)
	ConcurrencyStamp nvarchar [note: 'SQL Server: nvarchar(max)']

	indexes {
		NormalizedName [name: 'RoleNameIndex', unique]
	}
}

Table AspNetRoleClaims {
	Id int [pk, increment, not null]
	RoleId nvarchar(450) [not null]
	ClaimType nvarchar [note: 'SQL Server: nvarchar(max)']
	ClaimValue nvarchar [note: 'SQL Server: nvarchar(max)']

	indexes {
		RoleId [name: 'IX_AspNetRoleClaims_RoleId']
	}
}

Table AspNetUserClaims {
	Id int [pk, increment, not null]
	UserId nvarchar(450) [not null]
	ClaimType nvarchar [note: 'SQL Server: nvarchar(max)']
	ClaimValue nvarchar [note: 'SQL Server: nvarchar(max)']

	indexes {
		UserId [name: 'IX_AspNetUserClaims_UserId']
	}
}

Table AspNetUserLogins {
	LoginProvider nvarchar(450) [pk, not null]
	ProviderKey nvarchar(450) [pk, not null]
	ProviderDisplayName nvarchar [note: 'SQL Server: nvarchar(max)']
	UserId nvarchar(450) [not null]

	indexes {
		UserId [name: 'IX_AspNetUserLogins_UserId']
	}
}

Table AspNetUserRoles {
	UserId nvarchar(450) [pk, not null]
	RoleId nvarchar(450) [pk, not null]

	indexes {
		RoleId [name: 'IX_AspNetUserRoles_RoleId']
	}
}

Table AspNetUserTokens {
	UserId nvarchar(450) [pk, not null]
	LoginProvider nvarchar(450) [pk, not null]
	Name nvarchar(450) [pk, not null]
	Value nvarchar [note: 'SQL Server: nvarchar(max)']
}

Table __EFMigrationsHistory {
	MigrationId nvarchar(150) [pk, not null]
	ProductVersion nvarchar(32) [not null]
}

Ref: VitalSigns.PatientId > Patients.Id [delete: cascade]
Ref: Medications.PatientId > Patients.Id [delete: cascade]
Ref: Appointments.PatientId > Patients.Id [delete: cascade]
Ref: AspNetRoleClaims.RoleId > AspNetRoles.Id [delete: cascade]
Ref: AspNetUserClaims.UserId > AspNetUsers.Id [delete: cascade]
Ref: AspNetUserLogins.UserId > AspNetUsers.Id [delete: cascade]
Ref: AspNetUserRoles.UserId > AspNetUsers.Id [delete: cascade]
Ref: AspNetUserRoles.RoleId > AspNetRoles.Id [delete: cascade]
Ref: AspNetUserTokens.UserId > AspNetUsers.Id [delete: cascade]
```

### B. Actual Entities and Relationships

- `Patients`, `VitalSigns`, `Medications`, and `Appointments` are the four application entities.
- `AspNetUsers`, `AspNetRoles`, `AspNetRoleClaims`, `AspNetUserClaims`, `AspNetUserLogins`, `AspNetUserRoles`, and `AspNetUserTokens` are created by ASP.NET Core Identity.
- `__EFMigrationsHistory` is EF Core infrastructure metadata, not a domain entity.
- `Patients` has one-to-many relationships with `VitalSigns`, `Medications`, and `Appointments`; all three foreign keys are required and cascade on delete.
- Identity has one-to-many relationships from users to claims, logins, roles, and tokens, plus many-to-many users-to-roles through `AspNetUserRoles`.
- There is currently no relationship between `AspNetUsers` and `Patients`; the application patient table does not contain a user foreign key.

### C. Issues and Inconsistencies

1. `Appointments.ClinicianName` is free text, so clinicians are not represented as a related entity or foreign key.
2. `AddIdentityCore<ApplicationUser>()` does not register role services even though the Identity role tables exist in the database model.
3. The tests use EF Core InMemory, so they do not validate SQL Server foreign keys, indexes, decimal precision, cascade deletes, or migrations.
4. `DateTime` values map to `datetime2`, which does not retain timezone information; UTC handling must be consistent in the application.
5. No domain-specific unique constraints exist beyond the Identity indexes shown in the DBML.
6. The committed model, migration, designer, and snapshot are aligned; no pending model changes were found.

## 6. Board Columns

- **To Do:** Work that has not started yet.
- **In Progress:** The current item being implemented or reviewed.
- **Review:** Completed work waiting for validation or feedback.
- **Done:** Work that meets its Definition of Done.

## 7. Sprint 1 Completion Criteria

Sprint 1 is complete when:

- The goal has been achieved.
- All core entities are defined.
- Relationships between entities are documented.
- Primary keys, foreign keys, required fields, and important constraints are identified.
- The schema supports patient monitoring, medication tracking, and appointment management.
- The documentation is ready to guide database model and migration implementation in the next sprint.
