# Week 08 — Day 2: Fix N+1 Issues & Measure the Improvement

## Overview

This Day 2 exercise was intended to continue the N+1 investigation from Day 1 by verifying whether the Day 2 project still displayed the repeated-query pattern and then measuring the actual EF Core behavior. In this specific project directory, the actual implementation already uses projection-based list queries and does not contain an `Include()`/`ThenInclude()` pattern or a Day 1 N+1 report to fix.

## Learning Objectives

- Fixing N+1 problems.
- Include / ThenInclude.
- Query-count measurement.
- Projection.
- AsSplitQuery.
- Performance comparison.
- Correctness verification.

## Day 1 N+1 Problem

The current Day 2 project directory does not contain a Day 1 artifact, N+1 note, or endpoint-specific report. No Day 1 query count could be verified in the current environment, so the Day 1 N+1 baseline is recorded as:

- Endpoint: Not identified in this project directory.
- Cause: Not applicable; no N+1 pattern was present in the actual implementation.
- Before query count: Not verified in the current environment.

## Include / ThenInclude Fix

- Implementation: No `Include()` or `ThenInclude()` fix was applied because no real N+1 issue exists in the Day 2 code.
- Reason for the fix: Not applicable; the project’s list endpoints already operate on filtered DTO projections rather than on a full entity graph.
- Query count before: Not verified in the current environment.
- Query count after: The live EF Core logs showed a single query for the patient list path and no repeated child-query pattern.
- Improvement: Not applicable; no repeated query pattern was found.
- Correctness verification: The solution test suite passed (36/36) and the DTO mapping remained unchanged.

## Projection

- List endpoint: `/api/patients` is an example of the project’s actual projection-based pattern.
- Include approach: Not present in the code.
- Projection approach: `.Where(...).Select(p => Map(p)).ToListAsync()`
- Query comparison: The project’s actual list query executes as a single SQL statement and does not load entire entity graphs.
- Data-loading comparison: Only the required DTO fields are selected.
- Result comparison: Same contract, less data loaded, and no alternate entity graph fetch required.
- Conclusion: Projection is the correct and already-used approach for this project.

## AsSplitQuery

No suitable endpoint with 2+ collection navigation properties was found in the current project.

- Endpoint: N/A
- 2+ collection navigations: `Patient` has `VitalSigns`, `Medications`, and `Appointments`, but no endpoint loads multiple collection navigations together.
- Why split query was appropriate: Not applicable because no multi-collection graph query exists.
- Before/after behavior: N/A
- Correctness: N/A

## Performance Measurements

| Optimization | Endpoint | Before Queries | After Queries | Result |
|--------------|----------|----------------|---------------|--------|
| Include/ThenInclude | Not identified in current project | Not verified in the current environment. | 1 query observed | No N+1 issue found |
| Projection | /api/patients | N/A | 1 query | DTO-only projection |
| AsSplitQuery | N/A | N/A | N/A | No suitable endpoint |

## Testing & Verification

Actual verification performed in this project directory:

- Build result: `dotnet build "CardiacPatientMonitoring.slnx" --nologo` succeeded.
- Automated tests: `dotnet test "CardiacPatientMonitoring.slnx" --nologo --verbosity minimal --logger "console;verbosity=minimal"` passed with 36 passing tests, 0 failed.
- Endpoint tests: The API started successfully in Development mode on `http://localhost:5075`.
- EF Core logs: `Microsoft.EntityFrameworkCore.Database.Command` logging emitted actual SQL output during startup, including `SELECT COUNT(*) FROM [Patients] AS [p]`.
- Query counts: No N+1 pattern or repeated child query loop was observed in the current project.
- Response correctness: The automated test suite passed and the DTO response contract remains intact.

## Sprint 3 Backlog Update

No Day 1 backlog item for N+1 was present in the current project directory, and no additional performance backlog item was created for this work. The actual state is that the project already reflects the expected projection-based EF Core approach.

## Files Changed

- `README.md` — Documents the actual Day 2 findings and the verification results for this project.
- `hand-on-lab.md` — Captures the hands-on investigation, the actual EF Core observations, and the honest status of N+1 and projection work in this project.

## What I Learned

- N+1 elimination is only relevant when a repeated child-query pattern genuinely exists.
- `Include()` / `ThenInclude()` are useful when a graph is truly needed for a single request.
- Projection with `.Select()` can be a better choice when only DTO fields are needed.
- `AsSplitQuery()` is only appropriate when there is a real multi-collection loading case.
- SQL query generation and logging are the definitive way to measure performance.
- Query counting must be based on actual logs, not assumptions.
- Correctness and performance both matter; if the project already uses an efficient pattern, no unnecessary refactor is justified.

## Final Status

| Area | Status |
|------|--------|
| N+1 Fix | Not applicable; no actual N+1 issue found in this project |
| Include/ThenInclude | Not needed in the current implementation |
| Query Measurement | Partial verification completed; no Day 1 baseline available |
| Projection | Already implemented and efficient |
| AsSplitQuery | Not applicable |
| Testing | Passed (36/36) |
| Documentation | Completed |

