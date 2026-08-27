# Day 5 — Sprint 1 Close-Out

## Overview

This close-out records the Sprint 1 state of the Cardiac Patient Monitoring API using only evidence available in this project and tests run on 2026-08-27. It does not treat repository code or unexecuted Postman requests as a successful manual API run.

## Learning Objectives

- Verify implemented behavior with reproducible evidence.
- Compare implementation evidence with the available acceptance criteria.
- Carry unverified or incomplete work into an actionable Sprint 2 backlog.
- Record an honest retrospective and Sprint 1 status.

## Hands-On Lab

The detailed close-out record is in [hand-on-lab.md](hand-on-lab.md). It includes evidence classifications, backlog context, migration history, and the ERD.

## API Verification

`dotnet build .\CardiacPatientMonitoring.slnx --nologo` succeeded with 0 errors. It emitted 4 `NU1900` warnings because NuGet vulnerability data could not be reached. `dotnet test .\CardiacPatientMonitoring.Tests\CardiacPatientMonitoring.Tests.csproj --nologo` ran 34 tests: 32 passed and 2 failed. Both failures are the patient catalog integration tests, blocked while the exception middleware attempts to write to the Windows Event Log without permission.

### Catalog Browsing

- **Found in the repository:** Authorized `GET /api/patients` supports pagination, search, gender and birth-date filters, and four sort modes.
- **Not verified:** The two automated HTTP catalog tests could not complete because of the Event Log permission failure. No Postman execution result was supplied.

### Order Creation

- **Actually verified:** `OrderServiceTests` passed 3/3. They verify available-stock order creation, calculated line/order totals, stock decrement, insufficient-stock rejection without persisted order or stock changes, and duplicate-product quantity aggregation.
- **Found in the repository:** Authorized `POST /api/orders` delegates to the service; the service uses a relational EF Core transaction when supported.
- **Not verified:** No HTTP integration test, Postman order request, or live SQL Server transaction run was found or executed.

## Acceptance Criteria Review

No formal Sprint 1 backlog or acceptance-criteria document was found in this project. The following is therefore an implementation-evidence review, not formal acceptance sign-off:

| Area | Evidence | Status |
| --- | --- | --- |
| Patient catalog browsing | Endpoint and query handling found; HTTP tests blocked | Implemented; endpoint verification incomplete |
| Order creation with available stock | Passing service test | Verified at service level |
| Insufficient-stock rejection with no partial changes | Passing service test | Verified at service level |
| Server-calculated line and order totals | Passing service test; decimal mappings found | Verified at service level |
| Relational transaction behavior | Transaction code found | Not verified against a relational provider |

## Sprint 2 Backlog

1. **Restore catalog integration-test execution.** Configure the test host/logger so exception logging does not attempt a Windows Event Log write without permission, then rerun both `PatientEndpointIntegrationTests` and record the HTTP results.
2. **Add order endpoint integration coverage.** Test authenticated `POST /api/orders` for success and insufficient stock; assert 201/4xx behavior, totals, line totals, and unchanged stock/order state on rejection.
3. **Complete manual API collection coverage.** Add catalog and order requests to `CardiacPatientMonitoring.postman_collection.json`, execute them against a configured API/database, and retain only the actual results.
4. **Verify relational transaction and migrations.** Run the order workflow against the configured SQL Server database after applying migrations; confirm rollback behavior for a failing order scenario.
5. **Create a formal Sprint 2 acceptance-criteria checklist.** The project contains no Sprint 1 acceptance-criteria artifact, so future close-out cannot be formally signed off from the repository alone.

## Sprint 1 Retrospective

### What Went Well

- The domain model, migrations, and order service cover the primary order workflow.
- Focused service tests passed and confirm core stock and monetary calculations.
- The API keeps pricing and total calculation on the server.

### What Didn't Go Well

- Full automated verification is not green: 2 of 34 tests fail because the test environment cannot write to Windows Event Log.
- The Postman collection has no catalog or order requests, and no execution results are available.
- A formal Sprint 1 acceptance-criteria/backlog artifact and mentor feedback record were not found.

### Sprint 2 Action

Make the test host independent of Windows Event Log and add passing HTTP integration tests for catalog and order behavior before treating an endpoint as verified.

## Sprint 1 Summary

### ERD

The supplied [ERD image](ERD/ERD.png) is present. The implemented relationships are: `Patient` has vital signs, medications, and appointments; `Order.CustomerId` references `Patient`; `Order` has order items; and each order item references a product.

### Migration History

| Migration | Repository evidence |
| --- | --- |
| `20260814130519_InitialCreate` | Creates Identity, patients, vital signs, medications, and appointments with seed data. |
| `20260824170100_Day2EntitySetup` | Adds care categories and changes appointment-to-patient deletion behavior to restrict. |
| `20260826185619_AddOrdersAndProducts` | Adds products, orders, order items, their relationships, and a seeded product. |
| `20260826210000_AddOrderMonetaryFields` | Adds `UnitPrice`, `LineTotal`, and `OrderTotal` as `decimal(18,2)` and sets the seeded product price. |

Migration application to a live SQL Server database was **not verified** in this close-out.

### Pull Request

No pull-request link or pull-request record is available in this project. A pull request is therefore **not verified** and no link is fabricated.

## Final Status

Sprint 1 implementation is present and core order rules are verified at the service level. Sprint close-out is **partially verified, not fully accepted**: catalog HTTP verification, order HTTP/manual verification, relational transaction/migration verification, and a formal acceptance-criteria record remain in the Sprint 2 backlog.
