# Sprint 4 - Day 1: Test Coverage Audit

## Overview

This day audits every API endpoint in the cardiac patient monitoring capstone, identifies high-risk test gaps, carries the Sprint 3 retrospective action into Sprint 4 planning, and adds focused integration tests for the highest-risk gaps found.

## Sprint 4 Goal

Close the highest-risk authentication, authorization, ownership, and clinical endpoint test gaps while making the Sprint 3 performance verification action repeatable. The Sprint 3 action carried forward is to run a performance verification suite with at least 50 patients, assert query counts, and store cache HIT/MISS evidence as build artifacts.

## Sprint 4 Backlog

The prioritized backlog is documented in [Sprint4-Planning.md](Sprint4-Planning.md). It contains five real items: the Sprint 3 performance verification action, patient administration update/delete coverage, the clinical ownership matrix, remaining clinical CRUD coverage, and cache invalidation endpoint flows.

## Endpoint Test Coverage Audit

The complete audit is in [Endpoint-Test-Coverage-Audit.md](Endpoint-Test-Coverage-Audit.md). It found 23 endpoints:

| Status | Count |
|---|---:|
| BOTH | 5 |
| HAPPY PATH | 2 |
| ERROR PATH | 3 |
| NEITHER | 13 |

The 23 endpoints include authentication, patient administration, vital signs, medications, appointments, and the anonymous controlled exception route. No payment-adjacent endpoint or minimal API route exists in the inspected project.

## Risk Prioritization

Authentication was checked first because invalid credentials and token issuance are security boundaries. The authentication endpoints now have happy and error coverage. Payment-adjacent functionality was searched for but is not present. The remaining highest-risk gaps are role-protected patient mutations and patient-owned clinical resource routes, followed by untested clinical CRUD paths.

## Tests Added

| Endpoint | Risk | Missing coverage | Test added | Expected behavior |
|---|---|---|---|---|
| `POST /api/auth/login` | High | Invalid credentials endpoint path | `Login_WithInvalidPassword_ReturnsUnauthorized` | `401 Unauthorized`. |
| `GET /api/medications/{id}` | High | Cross-patient ownership denial | `PatientCannotReadAnotherPatientsMedication` | `403 Forbidden`. |
| `GET /api/appointments/{id}` | High | Cross-patient ownership denial | `PatientCannotReadAnotherPatientsAppointment` | `403 Forbidden`. |
| `POST /api/patients/{patientId}/vital-signs` | High | Invalid clinical input through HTTP | `PatientCreatingInvalidVitalSignReceivesBadRequest` | `400 Bad Request`. |

These tests are in [AuthEndpointIntegrationTests.cs](CardiacPatientMonitoring.Tests/AuthEndpointIntegrationTests.cs) and [HighRiskEndpointIntegrationTests.cs](CardiacPatientMonitoring.Tests/HighRiskEndpointIntegrationTests.cs). They use the existing `CustomWebApplicationFactory`, Testing environment, in-memory database, and unique email addresses.

## Full Test Suite

Command:

`dotnet test CardiacPatientMonitoring.slnx --nologo --verbosity minimal --logger "console;verbosity=minimal"`

Result: `42 passed, 0 failed, 0 skipped`. The build succeeded and the full suite completed in 3.6 seconds according to the test runner output.

## Files Changed

- `Sprint4-Planning.md`
- `Endpoint-Test-Coverage-Audit.md`
- `README.md`
- `CardiacPatientMonitoring.Tests/AuthEndpointIntegrationTests.cs`
- `CardiacPatientMonitoring.Tests/HighRiskEndpointIntegrationTests.cs`

Existing tests were not removed or disabled. No production application code was changed.

## Verification

- [x] Every controller endpoint audited.
- [x] Authentication gaps prioritized and invalid login coverage added.
- [x] Payment-adjacent functionality searched; no such endpoint exists.
- [x] Role-protected and ownership gaps prioritized.
- [x] Four highest-priority actual gaps covered with new tests.
- [x] Full test suite executed after all changes.
- [x] Full test suite passes: 42 passed, 0 failed, 0 skipped.
- [x] Sprint 4 planning includes the Sprint 3 retrospective action.
- [x] Documentation matches the inspected implementation.
- [x] Final `git status` recorded after all changes.
# Sprint 3 - Close-Out

## Overview

This day closes Sprint 3 performance work for the cardiac patient monitoring API. It consolidates the query-count diagnosis, patient catalog caching, composite indexes, available measurements, Sprint 3 backlog status, Sprint 4 follow-up work, and retrospective into documentation that can be copied into Notion.

## Objectives

- Review actual Sprint 3 performance work and targets.
- Record before/after evidence without inventing measurements.
- Verify the N+1 diagnosis, cache behavior, invalidation, and indexes.
- Move incomplete measurement work into a focused Sprint 4 backlog.
- Prepare the retrospective and merge/PR status.

## Work Completed

1. Reviewed the available before/after performance evidence.
2. Reviewed the N+1 diagnosis and Development EF Core query logging.
3. Documented Redis cache-aside behavior for `GET /api/patients`.
4. Reviewed cache HIT/MISS behavior from the cache tests.
5. Reviewed create/update/delete cache invalidation in `PatientService`; only update invalidation has a focused test.
6. Reviewed the medication and appointment indexes.
7. Confirmed both indexes are composite.
8. Reviewed the available post-index timing and execution-plan evidence; retained plans are not available.
9. Reviewed the actual Sprint 3 backlog in the Week 08 Day 1-4 labs.
10. Prepared the Sprint 4 performance backlog.
11. Completed the Sprint 3 retrospective.
12. Checked Git history and remote metadata; no PR URL or merge metadata was available.
13. Prepared copy-ready Markdown documentation for the Notion summary.

## Performance Evidence

| Area | Before | After | Status |
|---|---|---|---|
| N+1/query count | No original baseline captured | One command documented for each of three exercised clinical list requests | Partially complete |
| Redis cache | No timing measured | Cache behavior tests pass; live HIT/MISS timing not measured | Partially complete |
| Database indexes | No pre-index timing or plan retained | Medication: 1 ms elapsed/0 ms CPU; appointments: 0 ms elapsed/0 ms CPU | Needs manual verification |

The index values are single-run post-migration smoke measurements from the existing Day 4 documentation, not proof of improvement. The full evidence and caveats are in [Hands-On-Lab-Sprint3-Closeout.md](Hands-On-Lab-Sprint3-Closeout.md).

## Caching Strategy

The unfiltered admin patient catalog uses cache-aside with key `catalog:patients:list:v1`. A MISS queries the database and stores serialized DTOs for the configured absolute expiration, defaulting to 5 minutes. A HIT returns the cached DTO list without the catalog query. Search requests bypass the cache. Patient create, update, and delete remove the key after a successful save. Automated tests use the distributed-memory substitute; live Redis verification and timings need manual verification.

## Database Indexing

The migration `20260909172310_AddPerformanceIndexes` replaces the single-column patient indexes with `IX_Medications_PatientId_Name` and `IX_Appointments_PatientId_ScheduledAt`. The leading `PatientId` column supports the common patient filter; `Name` supports the medication search shape and `ScheduledAt` supports appointment ordering. Pre-index plans were not captured and retained execution-plan artifacts are unavailable.

## Sprint 3 Backlog Status

| Task | Status |
|---|---|
| Verify selected list query counts and diagnose N+1 behavior | PARTIALLY COMPLETE |
| Add and verify patient catalog cache-aside behavior | PARTIALLY COMPLETE |
| Add and profile query-driven composite indexes | PARTIALLY COMPLETE |

Implementation exists for each task, but incomplete measurements move to Sprint 4.

## Sprint 4 Backlog

- **Capture repeatable query-count evidence at target scale:** run selected lists with at least 50 patients and add a one-command regression assertion. Tags: `Sprint 4`, `Performance`.
- **Measure live Redis HIT/MISS and write invalidation:** capture live timings, logs, and create/update/delete freshness behavior. Tags: `Sprint 4`, `Performance`.
- **Capture pre/post SQL Server plans for composite indexes:** retain comparable plans/statistics and report measured deltas. Tags: `Sprint 4`, `Performance`.

## Retrospective

### What Went Well

- Query logging and projection-based list queries were reviewed using repository evidence.
- Cache-aside behavior, expiration, and write invalidation are implemented.
- Two query-driven composite indexes and their migration are present.
- The Day 5 solution passed 38/38 automated tests.

### What Could Be Improved

- Comparable N+1 and index baselines were not retained.
- Live Redis timings and execution-plan artifacts are missing.
- Cache create/delete invalidation lacks individual focused tests.

### Sprint 4 Action

Add one repeatable performance verification suite that runs at least 50 patients, asserts query counts, and stores cache HIT/MISS evidence as build artifacts.

## Pull Request

Needs manual insertion

## Files Changed

- `Hands-On-Lab-Sprint3-Closeout.md`
- `README.md`

No application source files were changed during this close-out.

## Verification

- `dotnet test CardiacPatientMonitoring.slnx --nologo --verbosity minimal --logger "console;verbosity=minimal"`
- Result: build succeeded; 38 tests passed, 0 failed, 0 skipped.
- Reviewed Git status, recent commits, migrations, EF Core configuration, Development query logging, Redis registration, cache tests, index migration, and existing Week 08 documentation.
- Confirmed no measured before/after improvement percentage or PR URL is claimed.

## Mentor Check-in

- [ ] Before/after performance evidence reviewed
- [ ] Query counts verified
- [ ] Cache HIT/MISS evidence reviewed
- [ ] Cache invalidation verified
- [ ] Index execution plans reviewed
- [x] Sprint 3 backlog reviewed
- [x] Sprint 4 backlog created
- [x] Retrospective completed
- [ ] Pull request merged
- [x] Notion summary ready in Markdown
- [ ] Mentor verification completed
