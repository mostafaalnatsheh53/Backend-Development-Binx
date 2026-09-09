# Hands-On Lab - Add Indexes & Profile Performance

## Objective

Add indexes based on the actual EF Core query patterns, create and apply an EF Core migration, and compare query performance using SQL Server evidence without inventing measurements.

## 1. Index Candidates

| Entity/Table | Column(s) | Query/Endpoint | Reason | Type |
|--------------|-----------|----------------|--------|------|
| Medications | `PatientId`, `Name` | `GET /api/patients/{patientId}/medications?search=...` | Every medication listing is scoped to a patient and an optional name search. The patient column is the selective leading key; `Name` supports the second predicate after the patient scope is narrowed. | Composite |
| Appointments | `PatientId`, `ScheduledAt` | `GET /api/patients/{patientId}/appointments` | The query filters by patient and orders the result by scheduled time. The index matches the filter-first, sort-second access pattern. | Composite |

The existing primary keys already cover lookups by entity ID. The original single-column foreign-key indexes on `Medications.PatientId` and `Appointments.PatientId` were replaced by the new composite indexes because the composite indexes retain the patient-first access path while supporting the additional query operation.

## 2. Index Implementation

The indexes were configured in `CardiacPatientMonitoring.Api/Data/ApplicationDbContext.cs` inside `OnModelCreating`:

```csharp
b.Entity<Medication>()
    .HasIndex(m => new { m.PatientId, m.Name });

b.Entity<Appointment>()
    .HasIndex(a => new { a.PatientId, a.ScheduledAt });
```

Both indexes are composite. Their column order follows the actual query shape: equality filtering by `PatientId` first, then the medication search column or appointment sort column.

## 3. Migration

- Migration: `20260909172310_AddPerformanceIndexes`
- Purpose: replace the two single-column indexes with query-specific composite indexes.
- Created indexes: `IX_Medications_PatientId_Name` and `IX_Appointments_PatientId_ScheduledAt`.
- Removed indexes: `IX_Medications_PatientId` and `IX_Appointments_PatientId`.
- Migration execution: applied successfully to the configured `CardiacPatientMonitoringDb` SQL Server database.

The generated migration contains only the intended index drops and creations. No table, column, relationship, seed-data, or unrelated constraint changes were generated.

## 4. Performance Before Indexes

Not verified in the current environment.

The pre-index execution statistics were not captured before the migration was applied, and no fabricated baseline is reported. The configured EF Core development logging includes `Microsoft.EntityFrameworkCore.Database.Command` at `Information`, which can be used to capture generated SQL and timings in a future baseline run.

## 5. Performance After Indexes

Post-migration SQL samples were executed against the configured local SQL Server database with `SET STATISTICS TIME ON`:

- Medication search query: CPU `0 ms`, elapsed `1 ms`.
- Appointment patient filter and schedule ordering query: CPU `0 ms`, elapsed `0 ms`.

These are single executions against the seeded database and are not sufficient to establish a statistically meaningful improvement.

## 6. Before / After Comparison

| Query / Endpoint | Index | Before | After | Improvement |
|------------------|-------|--------|-------|-------------|
| Medication listing with patient and name search | `IX_Medications_PatientId_Name` | Not verified in the current environment. | 1 ms elapsed, 0 ms CPU | Not verified in the current environment. |
| Appointment listing with patient filter and `ScheduledAt` ordering | `IX_Appointments_PatientId_ScheduledAt` | Not verified in the current environment. | 0 ms elapsed, 0 ms CPU | Not verified in the current environment. |

## 7. Index-by-Index Analysis

### Index 1

- Columns: `Medications.PatientId`, `Medications.Name`
- Index name: `IX_Medications_PatientId_Name`
- Query: `GET /api/patients/{patientId}/medications?search=...`
- Reason: narrow the medication search to one patient before evaluating the optional name predicate.
- Before: Not verified in the current environment.
- After: 1 ms elapsed, 0 ms CPU for the equivalent SQL sample.
- Improvement: Not verified in the current environment.
- Evidence: SQL Server reported the index exists; `SET STATISTICS TIME` reported the post-migration sample timing. Because the name predicate is implemented with `Contains`, the search may still require a scan within the selected patient range.

### Index 2

- Columns: `Appointments.PatientId`, `Appointments.ScheduledAt`
- Index name: `IX_Appointments_PatientId_ScheduledAt`
- Query: `GET /api/patients/{patientId}/appointments`
- Reason: support the patient filter and the required chronological ordering.
- Before: Not verified in the current environment.
- After: 0 ms elapsed, 0 ms CPU for the equivalent SQL sample.
- Improvement: Not verified in the current environment.
- Evidence: SQL Server reported the index exists; `SET STATISTICS TIME` reported the post-migration sample timing.

## 8. Testing

- API build: passed.
- Automated tests: passed, 38 total, 0 failed, 0 skipped.
- Migration generation: passed.
- Migration list: `20260909172310_AddPerformanceIndexes` shown as pending before update.
- Migration application: passed with `dotnet ef database update`.
- Database verification: passed; SQL Server reported both expected indexes and their expected columns.
- Query execution: passed for both representative SQL queries.
- Performance measurements: post-migration samples captured; before/after improvement not verified in the current environment.

## 9. Final Status

- Indexes identified: completed, two candidates selected from actual service queries.
- Indexes added: completed, two composite indexes configured and migrated.
- Composite index added: completed; both indexes are composite.
- Migration created: completed.
- Migration applied: completed against the configured local SQL Server database.
- Performance measured: post-migration only.
- Improvements observed: not verified in the current environment.
- Remaining limitations: no pre-index baseline and only one seeded row per query, so the recorded timings are not a reliable improvement comparison.
