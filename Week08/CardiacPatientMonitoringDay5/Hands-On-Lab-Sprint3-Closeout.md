# Sprint 3 Close-Out Hands-On Lab

## Scope and Evidence Rules

This close-out records the Sprint 3 work found in the Week 08 Day 1-4 documentation, the Day 5 source tree, Git history, and the verification run completed for this close-out. Values are reported only when they are present in repository evidence or were measured during this run. Missing baselines, live infrastructure results, and retained execution plans are marked `Needs Manual Verification`.

## 1. Demo Before/After Performance Evidence

### N+1 / Query Counts

The Sprint 3 diagnostic investigated the patient and patient-scoped clinical list endpoints. The original N+1 problem was not confirmed in the available project evidence. The earlier lab describes it as a verification/remediation task: the services already use direct DTO projections and do not traverse collection navigations inside a loop.

| Endpoint/evidence | Before | After | Status |
|---|---|---|---|
| Patient and clinical list query count | No Day 1 N+1 baseline was captured. | The Day 1 lab reports one `Executed DbCommand` for each of the three live patient-scoped list requests (vital signs, medications, appointments); the unfiltered patient catalog was not live-counted. | PARTIALLY COMPLETE |

**What changed:** no N+1 code fix was required in the current Day 5 implementation. The list services use `AsNoTracking()` and scalar DTO projection in a single LINQ query. The diagnostic evidence came from Development EF Core command logging (`Microsoft.EntityFrameworkCore.Database.Command` at `Information`) and the Day 1 lab's captured endpoint observations.

**Performance target:** each selected list endpoint should use no more than one SQL command and no repeated related-entity commands with at least 50 patients. The one-command observations meet the query-count portion for the three exercised clinical lists, but the 50-patient run, patient-catalog live count, and a regression interceptor/test were not completed.

### Redis Cache

- **Endpoint:** `GET /api/patients` without a `search` query, the admin-only patient catalog.
- **Strategy:** cache-aside. Read the distributed cache first; on a miss, query the database, serialize the DTO list, store it, and return it. Filtered requests bypass the cache.
- **Cache key:** `catalog:patients:list:v1`.
- **Expiration:** absolute expiration from `CacheSettings:CatalogExpirationMinutes`; the configured/default value is 5 minutes.
- **MISS behavior:** query `Patients` with `AsNoTracking()`, serialize the result, and populate the key.
- **HIT behavior:** deserialize the cached JSON and return it without the catalog database query.
- **Invalidation:** `PatientService.CreateAsync`, `UpdateAsync`, and `DeleteAsync` remove the key after a successful database save.
- **Evidence:** `PatientCatalogCacheTests` passed. The tests verify cached data remains until invalidation and that an update invalidates the catalog. Create/delete invalidation is implemented and visible in source, but is not individually covered by the focused tests.
- **Timing:** cache MISS and HIT response timings were not measured. Live Redis-backed API verification was not completed because the earlier lab records that Docker could not pull `redis:7-alpine`.

Cache status: **PARTIALLY COMPLETE**. Behavior is implemented and testable with the in-memory distributed-cache test substitute; live Redis HIT/MISS logs and timings need manual verification.

### Database Indexes

| Index | Columns | Query pattern | Why it was added |
|---|---|---|---|
| `IX_Medications_PatientId_Name` | `Medications(PatientId, Name)` | Patient-scoped medication listing with optional name search | Keeps the patient equality filter first and supports the secondary name predicate. |
| `IX_Appointments_PatientId_ScheduledAt` | `Appointments(PatientId, ScheduledAt)` | Patient-scoped appointment listing ordered by scheduled time | Keeps the patient filter first and supports the requested chronological order. |

Migration `20260909172310_AddPerformanceIndexes` drops the previous single-column indexes `IX_Medications_PatientId` and `IX_Appointments_PatientId` and creates the two composite indexes above. The EF Core model configuration and migration are present in the Day 5 source tree.

| Area | Before | After | Evidence | Status |
|---|---|---|---|---|
| N+1 query count | No original N+1 baseline captured | One command observed for each of three exercised clinical list requests | Development EF Core command logs documented in the Day 1 lab | PARTIALLY COMPLETE |
| Redis cache | No timing captured | HIT/MISS behavior covered by cache tests; no live timing | `PatientCatalogCacheTests`; live Redis unavailable in the recorded run | PARTIALLY COMPLETE |
| Database indexes | Pre-index timing and plan not captured | Post-migration samples reported: medication 1 ms elapsed/0 ms CPU; appointment 0 ms elapsed/0 ms CPU | Day 4 lab/README; single seeded-dataset smoke measurements | NEEDS MANUAL VERIFICATION |

The post-index values are not an improvement claim. No pre-index execution statistics were retained, and the actual graphical/text execution plans are not stored in the repository. Index existence and column order were reported as verified in the Day 4 documentation; execution-plan comparison remains `Needs Manual Verification`.

## 2. Sprint 3 Backlog Against Performance Targets

The actual Sprint 3 work items are represented by the Sprint 3 planning and close-out labs in Week 08 Days 1-4.

| Task | Performance target | Actual result | Status |
|---|---|---|---|
| Verify list endpoint query counts and diagnose N+1 behavior | At most one SQL command per selected list endpoint, with no per-record related queries at a 50-patient scale | Three patient-scoped list requests were documented at one command each; no N+1 was confirmed. The 50-patient run, `/api/patients` live count, and automated query-count regression check remain absent. | PARTIALLY COMPLETE |
| Add cache-aside behavior to the catalog-like endpoint | Repeated unfiltered catalog reads should avoid the database on a HIT and remain fresh after writes | Key, expiration, MISS/HIT code paths, and create/update/delete invalidation are implemented. Memory-cache tests pass for cached reads and update invalidation; live Redis and timings were not verified. | PARTIALLY COMPLETE |
| Add query-driven composite database indexes and profile them | Indexes should match actual filter/order patterns and demonstrate a measured before/after improvement with execution-plan evidence | Both composite indexes and the migration are present. Post-migration smoke timings are documented, but no pre-index baseline or retained execution plan is available. | PARTIALLY COMPLETE |

All three partially complete items move conceptually to Sprint 4 below. No item is marked `COMPLETE` where a required measurement or infrastructure check is missing.

## 3. Sprint 4 Performance Backlog

### 1. Capture Repeatable Query-Count Evidence at Target Scale

- **Problem:** the 50-patient query-count target and the unfiltered patient catalog count were not captured, and there is no automated command-count regression check.
- **Proposed solution:** run all selected list endpoints against the intended dataset and add a command interceptor or equivalent test assertion that fails above one SQL command per request.
- **Expected performance improvement:** verified protection against N+1 regressions and bounded database round trips per list request.
- **Acceptance criteria:** capture logs for every selected endpoint; record the command count; exercise at least 50 patients; add a passing regression test with a one-command threshold.
- **Priority:** High
- **Tags:** `Sprint 4`, `Performance`

### 2. Measure Live Redis HIT/MISS and Write Invalidation

- **Problem:** cache behavior is covered with the testing memory implementation, but live Redis connectivity, response timings, and create/delete invalidation are not evidenced.
- **Proposed solution:** start the configured Redis service, exercise MISS, HIT, create, update, and delete flows, and capture application/cache logs and timings.
- **Expected performance improvement:** measured reduction in database work and response time for repeated unfiltered catalog reads, with fresh data after writes.
- **Acceptance criteria:** record live MISS and HIT evidence; record timings without fabricating a baseline; verify no catalog DB query on HIT; verify create/update/delete remove the key and the next read is fresh.
- **Priority:** High
- **Tags:** `Sprint 4`, `Performance`

### 3. Capture Pre/Post SQL Server Plans for Composite Indexes

- **Problem:** the post-index smoke timings exist, but the pre-index baseline and actual execution-plan artifacts do not.
- **Proposed solution:** capture comparable pre/post SQL Server execution plans and statistics for the medication and appointment query shapes on a controlled dataset.
- **Expected performance improvement:** evidence-based reduction in scans, logical reads, CPU, or elapsed time where the indexes are selected by the optimizer.
- **Acceptance criteria:** retain both plans and statistics; verify the composite indexes and column order; report measured deltas or explicitly record no improvement.
- **Priority:** Medium
- **Tags:** `Sprint 4`, `Performance`

## 4. Sprint 3 Retrospective

### What Went Well

- Development EF Core command logging was configured at the database-command category and used to inspect query behavior.
- The selected list services use focused DTO projections and no repeated navigation loading was observed in the documented endpoint run.
- The catalog cache has a clear cache-aside implementation, a stable key, configuration-driven expiration, and write invalidation for create, update, and delete.
- The two indexes match real patient-filtered medication and appointment query shapes, and the generated migration replaces redundant single-column indexes rather than duplicating them.
- The current Day 5 build and full test suite passed: 38 tests passed, with 0 failures and 0 skipped.

### What Could Be Improved

- No original N+1 or pre-index performance baseline was retained, so improvement cannot be quantified.
- The 50-patient target was not re-run as a reproducible close-out measurement.
- Live Redis testing was blocked by the recorded Docker image-pull failure, and cache timings were not captured.
- Execution-plan artifacts were not retained; the documented index timings are single-run smoke measurements on a tiny seeded dataset.
- The cache tests do not individually exercise create and delete invalidation.

### One Concrete Action for Sprint 4

Add and run one repeatable performance verification test suite that exercises all selected list endpoints with at least 50 patients, asserts no more than one SQL command per request, and stores the measured query counts and cache HIT/MISS evidence as build artifacts.

## 5. Sprint 3 Summary

# Sprint 3 — Performance Optimization Summary

## Sprint Goal

Establish measured query-performance evidence, prevent N+1 behavior, add cache-aside behavior to the real catalog-like endpoint, and add indexes based on actual query patterns without claiming unverified improvements.

## N+1 Query Optimization

- **Problem:** Sprint 3 investigated a possible N+1 issue in patient-related list endpoints; a genuine N+1 was not confirmed in this project.
- **Root cause:** No verified root cause. The current services project directly to DTOs and do not load related collections per returned row.
- **Solution:** Retain the single-query projection pattern and inspect Development EF Core command logs.
- **Before:** No Day 1 baseline or original N+1 query count was captured.
- **After:** One command was documented for each of the three exercised patient-scoped clinical list requests.
- **Improvement:** Not measurable from the available evidence.
- **Evidence:** Day 1 lab observations and `Microsoft.EntityFrameworkCore.Database.Command` Development logging.

## Redis Caching

- **Endpoint:** `GET /api/patients` without search, admin-only.
- **Strategy:** Distributed cache-aside using Redis outside Testing and distributed memory cache in Testing.
- **Cache key:** `catalog:patients:list:v1`.
- **Expiration:** `CacheSettings:CatalogExpirationMinutes`, default/configured to 5 minutes absolute.
- **HIT/MISS behavior:** HIT deserializes cached DTOs; MISS queries, serializes, stores, and returns.
- **Invalidation:** create, update, and delete remove the catalog key after database save.
- **Before/After evidence:** behavior tests pass; live Redis timings and endpoint timing comparison are not verified.

## Database Indexing

- **Indexes:** `IX_Medications_PatientId_Name` and `IX_Appointments_PatientId_ScheduledAt`.
- **Composite index:** both are composite; `PatientId` is the leading column.
- **Reason:** support patient filtering plus medication name searching or appointment time ordering.
- **Before:** pre-index timings and execution plans were not captured.
- **After:** documented smoke samples report medication 1 ms elapsed/0 ms CPU and appointments 0 ms elapsed/0 ms CPU.
- **Execution-plan evidence:** index metadata and post-migration query execution were documented; actual retained before/after plans require manual verification.

## Performance Results

No overall performance improvement percentage is claimed. The current evidence is 38/38 tests passed, one command observed for three exercised clinical list requests, and the two documented post-index smoke timings above. Cache response timings and a complete before/after query baseline are not verified.

## Sprint 3 Backlog Status

All three actual Sprint 3 performance tasks are partially complete because implementation or diagnosis exists but one or more required measurements remain missing. Details and targets are in the backlog table above.

## Sprint 4 Backlog

1. Capture repeatable query-count evidence at the 50-patient target scale.
2. Measure live Redis HIT/MISS and create/update/delete invalidation.
3. Capture pre/post SQL Server plans for the composite indexes.

## Sprint 3 Retrospective

The implementation work was successful and testable, but the close-out lacks comparable baselines, live Redis timing evidence, and retained execution plans. The one Sprint 4 action is to add repeatable performance verification artifacts and assertions.

## Pull Request

Pull Request: Needs manual insertion

No pull-request URL was found in Git metadata. The configured remote is a repository URL only, and no Notion integration is available in this workspace.
