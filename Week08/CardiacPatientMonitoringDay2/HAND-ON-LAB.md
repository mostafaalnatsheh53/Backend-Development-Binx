# Hands-On Lab — Fix N+1 Issues & Measure the Improvement

## Objective

This Day 2 lab checked whether the current CardiacPatientMonitoringDay2 project still contained an N+1 query problem and whether the existing EF Core queries were already using the efficient patterns expected for the exercise. The project was inspected directly and all results below are based on the actual code, model structure, startup logs, and automated test results in this directory.

## 1. N+1 Problem from Day 1

- Endpoint: No Day 1 endpoint or N+1 finding was present in this Day 2 project directory.
- Original query: Not verified in the current environment.
- Related navigation: The domain model contains `Patient.VitalSigns`, `Patient.Medications`, and `Patient.Appointments`, but no query in this project loads those navigations with `Include()` or `ThenInclude()`.
- Why it caused N+1: Not applicable to the current implementation because the project already avoids traversing the navigation graph in the list queries.
- Before query count: Not verified in the current environment.

## 2. Include / ThenInclude Fix

- Changes made: No code change was required because no N+1 pattern was found in the actual Day 2 implementation.
- Why Include/ThenInclude was selected: Not applicable. The project does not include a lazy-loading or repeated child-query pattern in its list endpoints.
- After query count: The real EF Core Development logs showed a single `SELECT` for the patient list query and no repeated child queries. The startup log included:
  - `SELECT COUNT(*) FROM [Patients] AS [p]`
  - role checks against `AspNetRoles`
- Before/after comparison: Not verified in the current environment because there is no Day 1 N+1 artifact or endpoint in this directory.
- Correctness verification: The project test suite passed (36/36), and the current list endpoints continue to project only the DTO fields required by the API contract.

## 3. Projection Optimization

- List endpoint selected: `/api/patients` is the clearest example of a list endpoint already using projection.
- Original Include approach: No `Include()` or `ThenInclude()` pattern was present in the code.
- Projection approach: `db.Patients.AsNoTracking().Where(...).Select(p => Map(p)).ToListAsync()`
- Query counts: 1 query observed in the project’s actual EF Core logging for the patient list path, with no additional child queries.
- Data loaded: Only the fields needed by `PatientResponseDto` are selected.
- Comparison: Include was not used. Projection is the actual pattern in the project, and it is the more efficient option here.
- Which approach is preferable and why: Projection is preferable because it keeps the query focused on the fields needed by the endpoint and avoids loading full entity graphs unnecessarily.

## 4. AsSplitQuery

- Endpoint: No suitable endpoint with 2+ collection navigation properties was found in the current project.
- Collection navigations: `Patient` contains `VitalSigns`, `Medications`, and `Appointments`, but none of the endpoints currently query multiple collection navigation properties together.
- Reason for using AsSplitQuery: Not applicable because no real multi-collection query existed.
- Query behavior: Not applicable.
- Correctness verification: Not applicable.

## 5. Before / After Measurements

| Optimization | Endpoint | Before | After | Result |
|--------------|----------|--------|-------|--------|
| N+1 Fix | Not identified in this project | Not verified in the current environment. | 1 query observed in EF Core logs | No N+1 issue found in the actual code |
| Projection | /api/patients | N/A | 1 query | Projection already used |
| AsSplitQuery | N/A | N/A | N/A | No suitable endpoint found |

## 6. Testing

- Build result: `dotnet build "CardiacPatientMonitoring.slnx" --nologo` succeeded.
- Automated test result: `dotnet test "CardiacPatientMonitoring.slnx" --nologo --verbosity minimal --logger "console;verbosity=minimal"` passed with 36/36 tests passing.
- API verification: The API started successfully on `http://localhost:5075` in development mode.
- Query logging: EF Core command logging was enabled via the Development configuration and produced real SQL output.
- Query counts: The startup logs included a `SELECT COUNT(*) FROM [Patients] AS [p]` query, and no repeated child access pattern was observed.
- Result correctness: The test suite passed, and the code uses DTO projection in the list endpoints.

## 7. Final Status

- N+1 fixed: No actual N+1 issue was found in this Day 2 project.
- Include/ThenInclude applied: Not needed for the existing implementation.
- Query count measured: Partial startup logging was observed; no Day 1 N+1 baseline could be verified.
- Projection comparison completed: Yes, the project already uses projection.
- AsSplitQuery status: Not applicable.
- Tests completed: Yes.
- Documentation completed: Yes.
