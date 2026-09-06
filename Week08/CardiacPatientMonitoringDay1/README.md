# Week 08 — Day 1: Sprint 3 Planning & Diagnosing N+1

## Overview

Day 1 establishes a measurable Sprint 3 performance goal, enables development-only EF Core SQL logging, adds realistic relational development data, and defines a repeatable method for checking list endpoint query counts. The work uses the existing ASP.NET Core, EF Core, SQL Server, Identity, service, and controller architecture.

## Learning Objectives

- Sprint 3 planning.
- Performance targets.
- EF Core query logging.
- Query analysis.
- N+1 detection.
- Performance-aware database access.

## Sprint 3 Goal

Measure and improve list endpoint database access so each selected list endpoint returns its complete result with no more than one SQL command per request when exercised against at least 50 patients. Query performance and possible N+1 behavior are the focus.

Sprint 2 retrospective information is **Not available in the current project.** No concrete Sprint 2 action could be verified or carried forward from this directory.

## EF Core Query Logging

`CardiacPatientMonitoring.Api/appsettings.Development.json` sets `Microsoft.EntityFrameworkCore.Database.Command` to `Information`, which exposes generated SQL command logs during Development. `Program.cs` enables detailed errors and sensitive data logging only in Development. Production logging behavior was not changed.

Generated SQL is identified by `Microsoft.EntityFrameworkCore.Database.Command[20101]` and `Executed DbCommand`. The Development startup attempt produced these entries before failing during migration, confirming that logging is active.

## Test Data

The primary domain entities are `Patient`, `VitalSign`, `Medication`, and `Appointment`. `ApplicationUser` supplies the required one-to-one user relationship for each patient. `DevelopmentDataSeeder` creates a deterministic total of 50 patients, with one vital sign, medication, appointment, and application user relationship for each generated patient. It preserves the existing Alex Taylor seed and is idempotent for its generated IDs.

The seed is invoked only in Development after EF Core migrations. The migration was repaired to assign unique users to legacy patients before creating the unique index. Read-only SQL verification found 56 patients, 51 vital signs, 51 medications, 51 appointments, and 61 application users.

## Important List Endpoints

| Endpoint | Method | Records | Queries | Result |
|----------|--------|---------|---------|--------|
| `/api/patients` | GET | Not verified in the current environment. | Not verified in the current environment. | Admin-authenticated request not run. |
| `/api/patients/{patientId}/vital-signs` | GET | 0 | 1 | `200` for temporary patient 57. |
| `/api/patients/{patientId}/medications` | GET | 0 | 1 | `200` for temporary patient 57. |
| `/api/patients/{patientId}/appointments` | GET | 0 | 1 | `200` for temporary patient 57. |

The service code uses direct scalar DTO projections for these lists, so one query is the expected shape. That expectation is not reported as an observed count.

## N+1 Problem

N+1 means one root query followed by a separate related-entity query for each returned row. No actual project-specific N+1 problem was verified. The selected list services do not access navigation properties while mapping results, and the API could not reach endpoint execution in the Development run.

The initial observed failure was the existing `LinkPatientToApplicationUser` migration attempting to create a unique `Patients.UserId` index while legacy rows still had duplicate empty values. The migration was repaired to create valid unique users for those rows. After that repair, all three exercised list endpoints completed with one SQL command each. There is no repeated SQL evidence to support an N+1 claim.

The recommended approach for any future related-data endpoint is a measured DTO projection or deliberate `Include`/split-query strategy, with a query-count regression test.

## Sprint 3 Backlog

`[N+1]` **Verify list endpoint query counts after repairing the legacy patient migration**

- **Problem:** The repository needs a regression guard to preserve the currently measured one-query list behavior.
- **Evidence:** Three live list requests returned `200` and each produced exactly one `Executed DbCommand` entry; no repeated related query was observed.
- **Acceptance criteria:** Run against at least 50 patients, capture command logs for the three selected lists, and add a test that fails above one SQL command per list endpoint.
- **Next action:** Add the automated query-count regression test and repeat the measurement with populated list responses.

This backlog item is explicitly a verification/remediation task and does not claim a confirmed N+1 defect.

## Testing & Verification

- API build: successful with `dotnet build ... --no-restore`.
- Automated tests: 36 passed, 0 failed, 0 skipped.
- SQL Server availability: local `MSSQL$SQLEXPRESS` service was running.
- Application startup: reached SQL Server, applied the repaired migration, seeded Development data, and listened on ports 5088 and 5090 during verification.
- Database migration completion: successful after the legacy-user migration repair.
- Seed verification: successful; 56 patients, 51 vital signs, 51 medications, and 61 users.
- Endpoint execution: successful for all three patient-scoped list endpoints with `200` responses.
- Query counts: one SQL command observed for each exercised list endpoint.
- N+1 evidence: no repeated related-entity SQL observed; no genuine N+1 confirmed.

## Files Changed

- `CardiacPatientMonitoring.Api/Program.cs`: development-only EF diagnostics, migration, and seed invocation.
- `CardiacPatientMonitoring.Api/appsettings.Development.json`: EF Core command logging level.
- `CardiacPatientMonitoring.Api/Data/DevelopmentDataSeeder.cs`: deterministic 50-patient relational development dataset.
- `CardiacPatientMonitoring.Api/Migrations/20260831190411_LinkPatientToApplicationUser.cs`: legacy patient-user migration repair.
- `hand-on-lab.md`: detailed lab evidence, endpoint table, diagnosis, and backlog.
- `README.md`: complete Day 1 overview, learning objectives, verification status, and lessons.

## What I Learned

Sprint planning needs a measurable query target. EF Core command logging exposes the SQL shape and count, while direct projections avoid unnecessary navigation loading. Query counts must come from executed logs, and an N+1 problem must be supported by repeated related SQL rather than inferred from entity relationships alone. Data-loading choices should be validated against realistic relational volume.

## Final Status

| Area | Status |
|------|--------|
| Sprint 3 Planning | Complete; Sprint 2 retrospective not available in the current project. |
| Performance Target | Defined: `<= 1` SQL command per selected list endpoint. |
| EF Core Logging | Enabled for Development and observed during startup. |
| Test Data | Seeder implemented for 50 patients and related records; final population not verified. |
| List Endpoint Analysis | Three endpoints selected; requests blocked by migration failure. |
| Query Counting | Not verified in the current environment. |
| N+1 Diagnosis | No genuine N+1 verified. |
| Sprint 3 Backlog | Added with `[N+1]` verification/remediation tag. |