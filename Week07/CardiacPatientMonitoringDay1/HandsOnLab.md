# Sprint 2, Day 1: Identity and Role Foundation

## 1. Sprint 2 planning

### Goal

Build on the Sprint 1 Cardiac Patient Monitoring API by establishing identity, role, and authorization foundations without disrupting its patient-monitoring workflows.

### Backlog

1. Configure Identity users, persistence, and domain roles.
2. Connect a patient record to its owning application user.
3. Issue role claims in JWTs and assign an appropriate default role at registration.
4. Apply the documented role and ownership rules to existing patient, vital-sign, medication, and appointment endpoints.
5. Add authorization and ownership tests for those existing endpoints.

### Sprint 1 improvement carried forward

Sprint 2 carries forward the need to protect the already-working CRUD API with incremental, testable changes. Identity wiring and the authorization design are completed first; endpoint restrictions and ownership enforcement follow only after a patient-to-user relationship exists.

### Day 1 objectives

- Inspect the current data model, migrations, seed data, and endpoints.
- Confirm and complete Identity role registration.
- Seed the `Admin` and `Patient` domain roles in a non-destructive migration.
- Record the authorization plan for the existing API.

## 2. Existing application inspection

`ApplicationDbContext` is the EF Core context. It uses SQL Server through `DefaultConnection` in `CardiacPatientMonitoring.Api/appsettings.json` and inherits from `IdentityDbContext<ApplicationUser>`.

Existing domain entities are `Patient`, `VitalSign`, `Medication`, and `Appointment`. A patient has many vital signs, medications, and appointments; each related record has a required `PatientId`. Seed data provides Alex Taylor and one record of each related entity.

`20260814130519_InitialCreate` is the existing migration. It creates both the Sprint 1 application tables and the standard Identity tables (`AspNetUsers`, `AspNetRoles`, claims, logins, roles, and tokens). It preserves the application seed data.

The API provides anonymous registration/login endpoints, authenticated patient and monitoring CRUD endpoints, and an anonymous test exception endpoint.

## 3. Identity integration

The project already referenced `Microsoft.AspNetCore.Identity.EntityFrameworkCore`, used `ApplicationUser : IdentityUser`, configured `IdentityDbContext<ApplicationUser>`, and registered Identity with the EF store. Day 1 adds `.AddRoles<IdentityRole>()` to the existing Identity registration so role services are available. No packages were added because the required package was already present.

## 4. Migration creation and review

Migration: `20260830100859_SeedIdentityRoles`

The migration inserts `Admin` and `Patient` into `AspNetRoles`. Its `Up` method contains only `InsertData`; it has no `DropTable`, `DropColumn`, schema alteration, or application-data change. The `Down` method removes only those two seeded role records if explicitly rolled back.

Because the previous initial migration already creates all Identity tables, creating another table-creation migration would be empty and inaccurate. This follow-up migration is the safe Day 1 Identity migration on top of the existing history.

## 5. Domain roles

| Role | Planned permissions |
| --- | --- |
| Patient | Register and sign in; access only the monitoring information that belongs to their linked patient record. Patient write permissions will be finalized after the ownership model is added. |
| Admin | View and manage patients and all existing vital-sign, medication, and appointment records. |

There is currently no relationship between `ApplicationUser` and `Patient`, so Day 1 deliberately does not claim that patient ownership is enforced yet.

## 6. Endpoint authorization plan

| Endpoint | Method | Planned role | Reason |
| --- | --- | --- | --- |
| `/api/auth/register` | POST | Anonymous | Creates an account. A default role assignment is a later backlog item. |
| `/api/auth/login` | POST | Anonymous | Obtains a JWT for an existing account. |
| `/api/patients` | GET | Admin | Lists system-wide patient records. |
| `/api/patients/{id}` | GET | Admin; Patient (own linked record) | A patient may view only their record. |
| `/api/patients` | POST | Admin | Creates a patient record. |
| `/api/patients/{id}` | PUT, DELETE | Admin | Modifies or removes a patient record. |
| `/api/patients/{patientId}/vital-signs` | GET | Admin; Patient (own linked record) | Reads monitoring history. |
| `/api/vital-signs/{id}` | GET | Admin; Patient (own linked record) | Reads one vital-sign record. |
| `/api/patients/{patientId}/vital-signs` | POST | Admin | Adds a clinical measurement. |
| `/api/vital-signs/{id}` | PUT, DELETE | Admin | Corrects or removes a measurement. |
| `/api/patients/{patientId}/medications` | GET | Admin; Patient (own linked record) | Reads medication information. |
| `/api/medications/{id}` | GET | Admin; Patient (own linked record) | Reads one medication. |
| `/api/patients/{patientId}/medications` | POST | Admin | Adds medication information. |
| `/api/medications/{id}` | PUT, DELETE | Admin | Updates or removes medication information. |
| `/api/patients/{patientId}/appointments` | GET | Admin; Patient (own linked record) | Reads appointment history. |
| `/api/appointments/{id}` | GET | Admin; Patient (own linked record) | Reads one appointment. |
| `/api/patients/{patientId}/appointments` | POST | Admin | Schedules an appointment. |
| `/api/appointments/{id}` | PUT, DELETE | Admin | Updates or cancels an appointment. |
| `/api/test/exception` | GET | Anonymous (development/test diagnostic) | Exercises exception middleware; it is not a domain operation. |

The current controllers retain their existing `[Authorize]` attributes. Role-specific attributes and ownership checks are intentionally deferred to the later Sprint 2 backlog.

## 7. Validation performed

- `dotnet ef migrations add SeedIdentityRoles` completed successfully.
- The generated migration was inspected and contains only role inserts.
- `dotnet ef migrations has-pending-model-changes` reported no pending model changes.
- Database migration-status lookup and update could not connect to the configured local SQL Server Express instance: it requires encryption that this machine does not support. Migration application and live table/data verification remain pending until the local SQL Server encryption/connection configuration is corrected.
