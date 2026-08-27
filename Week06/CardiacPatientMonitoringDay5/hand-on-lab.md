# Day 5 — Sprint 1 Close-Out Hands-On Lab

## Objective

Close Sprint 1 with an evidence-based record of the Cardiac Patient Monitoring API: catalog browsing, order creation, stock rules, money calculations, database history, review state, and follow-up work. Evidence is deliberately classified as **Actually verified**, **Found in the repository**, or **Not verified**.

## API Verification

### Catalog Browsing

- **Found in the repository:** `GET /api/patients` is authorized and accepts `page`, `pageSize`, `search`, `gender`, `bornBefore`, and `sort`. The implementation supports `nameAsc`, `nameDesc`, `oldest`, and `newest` sort modes.
- **Actually verified:** Not applicable. The attempted automated HTTP catalog verification did not complete.
- **Not verified:** The two `PatientEndpointIntegrationTests` failed before assertions because exception middleware logging tried to write to the Windows Event Log and access was denied. Postman was not executed, and no saved Postman result was found.

### Order Creation

- **Actually verified:** `dotnet test` ran the three `OrderServiceTests` successfully. The available-stock case creates an order, returns a line total and order total of `251.00`, and reduces stock from 5 to 3.
- **Found in the repository:** Authorized `POST /api/orders` calls `OrderService.CreateAsync`. The request accepts a customer ID and product/quantity items only; prices and totals are obtained/calculated server-side.
- **Not verified:** No order HTTP integration test, Postman order request, or live API request was found or executed. The full endpoint contract is not verified by the service tests alone.

### Stock Availability and Rejection

- **Actually verified:** The insufficient-stock service test passed. It confirms an exception is raised and neither an order nor either product's stock changes. The duplicate-product test also passed, confirming quantities are combined before stock is checked.
- **Found in the repository:** The service validates every requested product before decrementing stock and uses a relational transaction when the database provider supports one.
- **Not verified:** Rollback behavior has not been run against SQL Server; the passing tests use EF Core's in-memory provider, which does not support relational transactions.

### Order Totals and Line Totals

- **Actually verified:** The available-stock service test asserts a unit price of `125.50`, a line total of `251.00`, the order total of `251.00`, and the persisted line total.
- **Found in the repository:** `OrderItem.LineTotal`, `Order.OrderTotal`, and `Product.UnitPrice` are configured as `decimal(18,2)`; the API response includes unit price, line total, and order total.
- **Not verified:** Totals have not been verified through an HTTP response or a live SQL Server database.

### Commands and Actual Results

| Command | Actual result |
| --- | --- |
| `dotnet build .\CardiacPatientMonitoring.slnx --nologo` | Succeeded with 0 errors and 4 `NU1900` warnings because NuGet vulnerability data could not be reached. |
| `dotnet test .\CardiacPatientMonitoring.Tests\CardiacPatientMonitoring.Tests.csproj --nologo --filter "FullyQualifiedName~OrderServiceTests|FullyQualifiedName~PatientEndpointIntegrationTests"` | 3 passed, 2 failed. The failures were both catalog integration tests blocked by Windows Event Log access. |
| `dotnet test .\CardiacPatientMonitoring.Tests\CardiacPatientMonitoring.Tests.csproj --nologo` | 32 passed, 2 failed, 34 total. The same two catalog integration tests failed for the Event Log permission error. |

Postman could not be executed in this close-out. The included collection contains authentication, vital-sign, medication, and appointment requests, but no catalog or order request; it is **found in the repository**, not evidence of an executed API verification.

## Acceptance-Criteria Review

No Sprint 1 backlog or acceptance-criteria artifact was found in the project. Consequently, no task can be formally marked accepted from repository evidence alone. The available implementation-level review is:

| Candidate Sprint 1 outcome | Evidence classification | Close-out result |
| --- | --- | --- |
| Browse the patient catalog | Found in the repository; automated HTTP verification blocked | Implemented, but not verified end-to-end |
| Create an order when stock is available | Actually verified by service test | Complete at service level; HTTP verification pending |
| Reject an order when stock is insufficient | Actually verified by service test | Complete at service level; HTTP verification pending |
| Calculate and persist monetary totals | Actually verified by service test; mappings found | Complete at service level; HTTP/SQL verification pending |
| Make order persistence transactional | Found in the repository | Implemented, but not verified against a relational database |

## Sprint 2 Backlog

1. **Fix catalog integration-test logging.** In the test environment, remove or replace the Windows Event Log logger that causes `ExceptionHandlingMiddleware` to fail while logging. Rerun the two patient endpoint tests and record their real HTTP results.
2. **Add order endpoint integration tests.** Cover an authenticated successful `POST /api/orders` and insufficient-stock rejection. Assert status codes, response totals, persisted order items, and stock invariants.
3. **Extend and run the Postman collection.** Add authenticated patient catalog and order requests, including an insufficient-stock case. Execute against a configured environment and store only actual results.
4. **Verify SQL Server behavior.** Apply the four migrations to the configured SQL Server database and run successful and rejected order scenarios to validate the relational transaction/rollback path.
5. **Record formal acceptance criteria.** Create a Sprint 2 checklist with owner, scenario, expected result, verification method, and acceptance status.

## Unresolved Code-Review Feedback

- **Found in the repository:** No mentor review request, review comments, or feedback record was found.
- **Actually verified:** There are no recorded unresolved mentor comments to carry forward.
- **Not verified:** External review systems and pull-request discussions were not available from this project, so absence outside the repository cannot be claimed.

No mentor-feedback item is added to the Sprint 2 backlog because no actual feedback was found. The testability and verification gaps above are separate actionable backlog items.

## Sprint Retrospective

### What Went Well

- The Sprint 1 code establishes a clear patient, product, order, and order-item model with migrations and an ERD asset.
- Core stock and monetary behavior has focused automated test coverage: all three order-service tests passed.
- Order totals are derived from stored product prices, not client input, and stock is validated before persistence.

### What Didn't Go Well

- The full test suite is not green: 2 of 34 tests are blocked by Windows Event Log permissions.
- The catalog endpoint and order controller lack successful end-to-end verification evidence.
- The Postman collection has no catalog/order scenarios, and no formal acceptance-criteria or mentor-review artifact was found.

### One Concrete Sprint 2 Action

Before adding Sprint 2 features, configure the test host to avoid Windows Event Log writes and make the catalog integration tests pass; then use the same test-host approach for order endpoint tests.

## ERD

The supplied ERD is available at [ERD/ERD.png](ERD/ERD.png).

```text
Patient 1 ──< VitalSign
Patient 1 ──< Medication
Patient 1 ──< Appointment
Patient 1 ──< Order 1 ──< OrderItem >── 1 Product
CareCategory (standalone lookup entity)
```

The relationship summary is **found in the repository** in `ApplicationDbContext`; visual accuracy of the supplied ERD against the current model was not independently verified.

## Migration History

| Migration | Purpose | Evidence status |
| --- | --- | --- |
| `20260814130519_InitialCreate` | Creates Identity, patients, vital signs, medications, and appointments; seeds initial clinical data. | Found in the repository |
| `20260824170100_Day2EntitySetup` | Adds care categories and changes appointment deletion to restrict. | Found in the repository |
| `20260826185619_AddOrdersAndProducts` | Adds products, orders, order items, their foreign keys, indexes, and seeded product. | Found in the repository |
| `20260826210000_AddOrderMonetaryFields` | Adds `UnitPrice`, `LineTotal`, and `OrderTotal` with `decimal(18,2)` precision; prices the seed product. | Found in the repository |

Migration application was **not verified** against a live SQL Server database.

## Pull Request

- **Found in the repository:** No pull-request link, number, or review record was found.
- **Actually verified:** No pull request was verified.
- **Not verified:** Remote hosting and external pull-request state were not inspected; no link is provided.

## Final Sprint 1 Status

Sprint 1 is **implemented and partially verified**. The core order service rules are actually verified by passing tests, while catalog HTTP behavior, order HTTP behavior, SQL Server migration/transaction behavior, formal acceptance sign-off, and pull-request/review evidence remain unverified. The concrete implementation and verification gaps are captured in the Sprint 2 backlog above.
