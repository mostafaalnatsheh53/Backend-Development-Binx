# Day - Add Indexes & Profile Performance

## Overview

This day applies database indexes to the cardiac patient monitoring API using the real EF Core query patterns. Indexes can reduce the rows SQL Server must inspect for filtered queries and can support requested ordering.

## Learning Objectives

- Database indexes.
- EF Core Fluent API.
- Composite indexes.
- EF Core migrations.
- Query performance.
- Execution plans.
- Performance measurement.

## Schema & Query Analysis

The project was inspected from `ApplicationDbContext`, the entity models, migrations, services, controllers, logging settings, and tests. The relevant query patterns are:

- Medication listings filter by `PatientId` and optionally search `Name`.
- Appointment listings filter by `PatientId`, optionally filter by `Status`, and sort by `ScheduledAt`.
- Vital-sign listings filter by `PatientId`; the existing foreign-key index remains appropriate for that query.
- Entity ID lookups use primary keys, so additional ID indexes were not added.

Existing foreign-key indexes on the selected child tables were not duplicated. The migration replaces the two single-column indexes with composite indexes that preserve the leading patient lookup and support the additional operation.

## Indexes Added

| Entity | Column(s) | Type | Query/Endpoint | Purpose |
|--------|-----------|------|----------------|---------|
| Medication | `PatientId`, `Name` | Composite | `GET /api/patients/{patientId}/medications?search=...` | Patient-scoped medication search |
| Appointment | `PatientId`, `ScheduledAt` | Composite | `GET /api/patients/{patientId}/appointments` | Patient filter plus chronological ordering |

## EF Core Fluent API

The indexes are configured in `CardiacPatientMonitoring.Api/Data/ApplicationDbContext.cs` within `OnModelCreating`:

```csharp
b.Entity<Medication>()
    .HasIndex(m => new { m.PatientId, m.Name });

b.Entity<Appointment>()
    .HasIndex(a => new { a.PatientId, a.ScheduledAt });
```

The most selective/common equality key is first in each composite index. The migration names are `IX_Medications_PatientId_Name` and `IX_Appointments_PatientId_ScheduledAt`.

## Migration

Migration `20260909172310_AddPerformanceIndexes` drops the old `IX_Medications_PatientId` and `IX_Appointments_PatientId` indexes and creates the two composite indexes. The migration was applied successfully to the configured `CardiacPatientMonitoringDb` SQL Server database.

## Performance Profiling

Development configuration logs EF Core database commands at `Information` level. In addition, representative SQL queries were executed directly against SQL Server with `SET STATISTICS TIME ON` after the migration. Both expected indexes were verified through `sys.indexes` and `sys.index_columns`.

Test conditions were the configured local SQL Server database with the existing seeded data. The database contains only one seeded row for each selected query, so these timings are smoke measurements rather than a scalable benchmark.

## Before vs After

| Query / Endpoint | Before | After | Improvement |
|------------------|--------|-------|-------------|
| Medication listing with patient and name search | Not verified in the current environment. | 1 ms elapsed, 0 ms CPU | Not verified in the current environment. |
| Appointment listing with patient filter and schedule ordering | Not verified in the current environment. | 0 ms elapsed, 0 ms CPU | Not verified in the current environment. |

No improvement claim is made because a pre-index baseline was not captured.

## Index-by-Index Results

`IX_Medications_PatientId_Name` exists on `Medications(PatientId, Name)` and was exercised by the equivalent patient/name query. The post-migration sample took 1 ms elapsed and 0 ms CPU. The before value and measurable improvement are not verified in the current environment.

`IX_Appointments_PatientId_ScheduledAt` exists on `Appointments(PatientId, ScheduledAt)` and was exercised by the equivalent patient/order query. The post-migration sample took 0 ms elapsed and 0 ms CPU. The before value and measurable improvement are not verified in the current environment.

## Testing & Verification

- API build: passed.
- Automated tests: passed, 38/38, with 0 failures and 0 skipped tests.
- Migration: generated and listed successfully.
- Migration application: completed successfully with `dotnet ef database update`.
- Database verification: both expected indexes and column orders were returned by SQL Server metadata queries.
- Query execution: both representative queries returned the seeded records.
- Performance: post-migration SQL Server timings were captured; before/after comparison is not verified in the current environment.

## Files Changed

- `CardiacPatientMonitoring.Api/Data/ApplicationDbContext.cs`: added the two composite Fluent API indexes.
- `CardiacPatientMonitoring.Api/Migrations/20260909172310_AddPerformanceIndexes.cs`: adds and removes the intended indexes.
- `CardiacPatientMonitoring.Api/Migrations/20260909172310_AddPerformanceIndexes.Designer.cs`: generated migration model.
- `CardiacPatientMonitoring.Api/Migrations/ApplicationDbContextModelSnapshot.cs`: updated EF Core model snapshot.
- `hand-on-lab.md`: completed the detailed lab report.
- `README.md`: documents the whole day and its verification status.

## What I Learned

Indexes should follow real filtering, joining, and sorting patterns rather than being added arbitrarily. Composite index column order matters: equality/filter keys normally lead, followed by the secondary search or sort key. EF Core Fluent API expresses the model intent, while migrations safely apply the schema change. Query logging, SQL Server execution statistics, and execution plans provide evidence; a single tiny seeded dataset is not enough to claim a performance improvement.

## Final Status

| Area | Status |
|------|--------|
| Index Candidates | Two candidates identified from actual service queries |
| Indexes Added | Two composite indexes added |
| Composite Index | Completed; both added indexes are composite |
| EF Core Migration | Created: `20260909172310_AddPerformanceIndexes` |
| Migration Applied | Completed against configured local SQL Server |
| Performance Profiling | Post-migration profiling completed |
| Before/After Measurement | Not verified in the current environment |
| Testing | Build passed; 38 tests passed |
| Documentation | Completed |
