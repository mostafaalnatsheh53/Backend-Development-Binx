# Hands-On Lab — Enable Logging & Diagnose N+1

## Objective

Enable development-only EF Core SQL visibility, prepare realistic relational data, and use observed SQL to diagnose query performance in the cardiac patient monitoring API.

## 1. Sprint 3 Planning

- **Sprint 3 goal:** Establish a measured query-performance baseline for the patient, vital-sign, medication, and appointment list endpoints, and keep each selected list endpoint at no more than one SQL query per request after the database is available.
- **Performance target:** Each selected list endpoint must return its complete list with `<= 1` SQL command and no repeated related-entity commands when exercised with at least 50 patients.
- **Sprint 2 retrospective action carried forward:** Not available in the current project. No Sprint 2 retrospective or concrete carried-forward action was found in this directory.
- **Why query performance is being addressed:** The API has patient-related collections and list endpoints. SQL logging is needed to distinguish a single projection query from repeated relationship loading before scaling the dataset.

## 2. EF Core Query Logging

Development logging was enabled in `CardiacPatientMonitoring.Api/appsettings.Development.json` by setting `Microsoft.EntityFrameworkCore.Database.Command` to `Information`. `Program.cs` also enables detailed errors and sensitive data logging only when the environment is Development. Production configuration was not changed.

Observed log entries use the `Microsoft.EntityFrameworkCore.Database.Command[20101]` category and include `Executed DbCommand`, command type, parameters, and generated SQL. For example, the attempted Development startup emitted `SELECT 1`, migration SQL, and an `INSERT` command. The SQL can be identified by filtering the application output for `Microsoft.EntityFrameworkCore.Database.Command` or `Executed DbCommand`.

## 3. Test Data Seeding

The primary entities are `Patient`, `VitalSign`, `Medication`, and `Appointment`, with `ApplicationUser` supporting the required one-to-one patient relationship. `DevelopmentDataSeeder` creates a deterministic development dataset up to 50 patients. Each generated patient has one related vital sign, one medication, and one appointment, and each patient references a generated application user. Existing valid seed data is preserved.

The seed runs only in Development after `Database.MigrateAsync()` and is idempotent for the generated user relationships. It can be recreated by starting the API with `ASPNETCORE_ENVIRONMENT=Development`. The migration was repaired to assign unique users to legacy patients before the unique index is created. Verified SQL Server counts were 56 patients, 51 vital signs, 51 medications, 51 appointments, and 61 application users.

## 4. List Endpoint Analysis

The selected endpoints are the patient list and the three patient-scoped clinical lists. The service implementations project directly to DTOs and do not access navigation properties. This is a code-level expectation, not an observed query count.

| Endpoint | Method | Records | Query Count | Result |
|----------|--------|---------|-------------|--------|
| `/api/patients` | GET | Not verified in the current environment. | Not verified in the current environment. | Admin-authenticated request not run. |
| `/api/patients/{patientId}/vital-signs` | GET | 0 | 1 | Normal; observed `200` for temporary patient 57. |
| `/api/patients/{patientId}/medications` | GET | 0 | 1 | Normal; observed `200` for temporary patient 57. |
| `/api/patients/{patientId}/appointments` | GET | 0 | 1 | Normal; observed `200` for temporary patient 57. |

## 5. N+1 Diagnosis

No genuine N+1 problem was verified in the current project run. The relevant root queries are the `Patients`, `VitalSigns`, and `Medications` projections in `Services.cs`. They select scalar DTO fields in the same LINQ query and do not loop over entities to load `Patient`, `VitalSigns`, `Medications`, or `Appointments` navigations.

The initial Development run exposed the existing `LinkPatientToApplicationUser` migration failure: legacy rows contained duplicate empty `UserId` values. After the migration was repaired, the API started, the seed completed, and each selected list request emitted exactly one `Executed DbCommand` entry. No repeated related-entity SQL was observed. It would be incorrect to label this repository with a confirmed N+1 based on the available evidence.

At scale, an N+1 pattern would issue one root query plus one related query per returned row. The recommended prevention for a future confirmed case is a single projection or an explicit `Include`/split-query choice backed by measured SQL and an automated query-count test.

## 6. Sprint 3 Backlog

`[N+1]` **Verify list endpoint query counts after repairing the legacy patient migration**

- **Endpoint:** `/api/patients` and the patient-scoped clinical list endpoints.
- **Current behavior:** Direct DTO projections execute one SQL command for each selected list request.
- **Evidence:** Three live requests returned `200` with zero records and each produced exactly one `Executed DbCommand` entry. No repeated related-entity command was observed.
- **Expected behavior:** Each selected list endpoint completes with one SQL command and does not issue per-record relationship queries.
- **Acceptance criteria:**
  - Keep the repaired legacy-data migration passing on an existing database.
  - Run the API against at least 50 patients.
  - Capture and count `Microsoft.EntityFrameworkCore.Database.Command[20101]` entries for each endpoint.
  - Add a regression test that fails if a selected list endpoint exceeds one query.
- **Suggested technical solution:** Keep the existing scalar DTO projections, then verify them with a command interceptor or test logger. Use an explicit projection for any future endpoint that needs related data.

This is a verification/remediation task, not a claim that an N+1 defect currently exists.

## 7. Final Status

- **Sprint 3 planning:** Complete from verifiable project information; Sprint 2 retrospective is not available in the current project.
- **Performance target:** Defined as `<= 1` SQL command per selected list endpoint.
- **EF Core logging:** Enabled for Development and observed during startup and endpoint execution.
- **Seed data:** Verified in SQL Server: 56 patients, 51 vital signs, 51 medications, 51 appointments, and 61 users.
- **Endpoint analysis:** Three selected endpoints executed successfully for patient 57.
- **Query counts:** Verified as one SQL command per selected endpoint request.
- **N+1 diagnosis:** No genuine N+1 verified; no repeated related query was observed.
- **Sprint 3 backlog:** Added as a tagged verification/remediation task.