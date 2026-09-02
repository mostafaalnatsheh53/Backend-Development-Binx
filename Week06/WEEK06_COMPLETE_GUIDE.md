# Week 06 Complete Guide

## Week 06 Overview

This guide is an evidence-based explanation of the five saved Week 06 project snapshots. The primary evidence is the code, migrations, model snapshots, tests, and configuration under `Week06/CardiacPatientMonitoringDay1` through `CardiacPatientMonitoringDay5`. READMEs and labs are treated as instructions or reported verification, not proof of a live SQL Server or Postman run.

The work is cumulative. Each `CardiacPatientMonitoringDayN` directory is a complete snapshot, rather than a single project edited in place. The final snapshot contains an ASP.NET Core API using EF Core, SQL Server configuration, ASP.NET Core Identity, JWT authentication, protected patient/vital-sign/medication/appointment endpoints, a patient catalog, and an order workflow.

Evidence labels used here:

- **Implemented — Found in repository:** source, migration, test, configuration, or documentation asset is present.
- **Actually verified:** the repository's Day 5 close-out records a command and result.
- **Planned/Required — No implementation evidence found:** the instructions ask for something but the saved project does not prove it happened.

Important scope note: `Product`, `Order`, and `OrderItem` are technically implemented on Day 4, even though they are not natural clinical-monitoring entities. They must not be confused with Day 1's future clinical baseline, whose `Clinician`, devices, alerts, care plans, and audit features remain planned.

## Day 1 — Sprint 1 Planning and Database Design

### Purpose

Day 1 established a shared database design before adding new features. Its purpose was to inspect the existing Cardiac Patient Monitoring API model, distinguish it from a more professional future design, apply normalization, and produce an ERD/DBML representation that accurately reflects the current database.

### Learning Objectives

- Define a measurable sprint goal and Definition of Done: make the intended database outcome testable rather than a vague "work on the database" task.
- Turn scope into small backlog items: separate inspection, schema documentation, normalization, ERD work, and risk review.
- Read EF Core evidence: entity classes, `DbContext`, Identity model, migration, generated designer, model snapshot, and connection configuration together determine the actual schema.
- Apply 1NF/2NF/3NF: store individual facts in columns, keep dependent rows in related tables, and avoid putting clinician details or multiple clinical readings in one field.
- Model the schema visually: DBML/ERD should include keys, types, relationships, indexes, nullability, and delete behavior.

### Required Work

The README and lab require Sprint 1 planning, a current-schema inventory, a normalized future baseline, DBML for dbdiagram.io, migration/snapshot comparison, and documentation of risks such as free-text clinician names, Identity roles, InMemory testing, and UTC handling.

### Actual Work

**Implemented — Found in repository:** `CardiacPatientMonitoringDay1/README.md` and `Hands-On Lab.md` contain the sprint goal, backlog table, future baseline, normalization discussion, DBML, relationship summary, and risks. `ERD/ERD.png` and `CardiacPatientMonitoring_Report.pdf` are present.

The Day 1 application already contains four domain tables: `Patients`, `VitalSigns`, `Medications`, and `Appointments`; Identity creates its normal `AspNet*` tables; EF creates `__EFMigrationsHistory`. `InitialCreate` is present at `CardiacPatientMonitoring.Api/Migrations/20260814130519_InitialCreate.cs`, together with its designer and model snapshot.

The lab's board labels every item **To Do**, even while the surrounding README says the work is completed. That is a documentation inconsistency: it is evidence of planning content, not an accurately updated task board.

### Technical Explanation

EF Core turns C# entity classes and fluent configuration into a relational model. A migration is the incremental SQL-like recipe to move a database to that model; the model snapshot lets EF compare the current model when generating the next migration. `IdentityDbContext<ApplicationUser>` adds credential, claims, token, login, role, and user-role tables without custom domain entities for them.

The actual Day 1 model uses `DateOnly` for patient and medication dates, `DateTime` for measurements and appointments, and SQL Server decimal precision for vital measurements. A required `PatientId` foreign key represents one patient with many child rows. Cascade delete means deleting a patient removes its dependent vital signs, medications, and—on Day 1—appointments.

### Project-Specific Explanation

The application records a patient's demographics, repeated vital-sign readings, prescriptions, and appointments. It does **not** link `Patient` to an Identity user, so an authenticated account is not yet an ownership or access-control model for clinical records. `Appointments.ClinicianName` is only text, so the system cannot enforce that a clinician exists or associate an appointment with a clinician account.

### Before vs After

Before Day 1, the saved evidence does not contain an earlier Week 06 snapshot, so the prior state cannot be reconstructed. After Day 1, the already-existing implementation is documented as the current schema, while a substantially broader normalized clinical platform is explicitly marked future scope. No Day 1 migration was created beyond the pre-existing `InitialCreate` migration.

### Important Files

- `CardiacPatientMonitoringDay1/Hands-On Lab.md` — the main design artifact: backlog, future baseline, DBML, actual schema, and risks.
- `CardiacPatientMonitoringDay1/CardiacPatientMonitoring.Api/Models/Entities.cs` — application entities and navigation properties.
- `.../Data/ApplicationDbContext.cs` — SQL types/precision, relationships, and seed data.
- `.../Migrations/20260814130519_InitialCreate.cs` — database-level evidence for the initial tables, keys, and Identity schema.
- `.../Migrations/ApplicationDbContextModelSnapshot.cs` — EF's recorded model state.

### Database Impact

No new Day 1 schema change is evidenced. The documented initial schema has integer identity primary keys on domain tables; `VitalSigns.PatientId`, `Medications.PatientId`, and `Appointments.PatientId` reference `Patients.Id`. It has indexes on those foreign keys, cascade deletion for all three in the initial migration, patient-name length limits, vital decimal precision, medication-name limit, and appointment-status length limit. Identity uses string (`nvarchar(450)`) keys and includes its own unique normalized username/role indexes.

### API Impact

No Day 1 API addition is identified from the Day 1-to-Day 2 source comparison. The existing API is protected by JWT authentication except for register/login, and already has patient, vital-sign, medication, and appointment CRUD controllers.

### Testing

The test project exists. The Day 1 documentation correctly identifies a limitation: tests use EF Core InMemory, so they do not prove SQL Server foreign-key enforcement, migration application, decimal behavior, indexes, cascade behavior, or relational transactions.

### Hands-On Lab

The lab asks for full schema planning and gives a large future normalized design. Its DBML section deliberately documents the then-current schema, not the future tables. **Implemented — Found in repository:** the lab, ERD asset, and initial migration exist. **Planned/Required — No implementation evidence found:** future `Clinician`, `PatientClinicianAssignment`, devices/readings, conditions/allergies, risk assessments, alerting, notifications, care plans, notes, and audit log code or migrations.

### Definition of Done

**Partially complete / documentation complete.** The documentation, DBML, current schema, risk list, and ERD asset exist. The board itself was not moved from To Do, and no evidence proves its proposed database GUI verification or a live SQL Server schema comparison. The guide therefore treats it as completed design documentation, not fully externally verified implementation.

### Issues and Decisions

- Free-text `ClinicianName` is not normalized into a clinician relationship.
- `AddIdentityCore<ApplicationUser>()` creates the Identity model but role-management services are not registered with `AddRoles`.
- `datetime2` does not contain timezone offset; source uses UTC seed values and should remain consistent.
- No domain-specific uniqueness constraints exist.
- Future baseline tables are useful design direction but are not implementation claims.

### What I Should Understand

You should be able to explain the difference between an EF entity, an EF migration, and a model snapshot; why a foreign key plus navigation property expresses a one-to-many relationship; why delete behavior is a data-retention decision; and why the future ERD is not the current database.

### Mentor Explanation

"I began Sprint 1 by auditing the real EF Core model and migration rather than designing from assumptions. I documented the current patient, vital-sign, medication, appointment, and Identity schema in DBML, checked normalization, and recorded gaps such as the free-text clinician field and the lack of a patient-user link."

## Day 2 — Full EF Core Model, Relationships, Seed Data, and Migration

### Purpose

Day 2 turned the reviewed design into a more explicit EF Core model. The key practical outcome was a reference/lookup table and a deliberate change to appointment deletion so scheduled clinical history cannot be accidentally removed with a patient.

### Learning Objectives

- Model tables as C# classes and add navigation properties so EF and application code can traverse relationships.
- Use Fluent API instead of relying only on conventions, especially for delete rules and field limits.
- Seed stable reference data with `HasData`, allowing migrations to insert known lookup values.
- Generate and review a migration before applying it, ensuring the emitted database change reflects the intended model.

### Required Work

The Day 2 lab asks for entities for the ERD, at least two explicit relationships, a seeded reference table, a reviewed migration, and schema verification in a database GUI/client.

### Actual Work

**Implemented — Found in repository:** Day 2 changes `Entities.cs`, `ApplicationDbContext.cs`, the model snapshot, and adds migration `20260824170100_Day2EntitySetup.cs` plus its designer. It introduces `CareCategory` (`Id`, required `Name`, optional `Description`), `DbSet<CareCategory>`, a maximum length of 80 for its name, and three seed rows: Low Risk, Moderate Risk, and High Risk.

The migration drops and recreates `FK_Appointments_Patients_PatientId` with `ReferentialAction.Restrict`, creates `CareCategories`, and inserts the seed records. Vital signs and medications retain cascade delete. The prior app/controller/service/test source has no Day 2-to-Day 3 changes apart from later catalog work; no new Day 2 HTTP endpoint was added for care categories.

The README says all core tables were implemented, but they were already present in the Day 1 snapshot. The actual incremental Day 2 work is the care-category addition and appointment delete-rule change—not initial creation of every domain entity.

### Technical Explanation

`HasData` is migration-managed seed data: its key values are fixed so EF can emit inserts/updates in migrations. `DeleteBehavior.Restrict` makes the database reject deletion of a patient that still has appointments. This differs from cascade deletion, where dependent rows disappear automatically. The choice preserves scheduled history but requires application code to resolve/cancel/delete appointments first.

### Project-Specific Explanation

`CareCategory` could support patient risk/care classification, but the final model has no foreign key from `Patient` (or another entity) to `CareCategory` and no controller/service for it. Therefore it is currently a standalone seeded lookup, not an active patient classification feature.

### Before vs After

Before Day 2: appointments were configured to cascade from patients in `InitialCreate`; there was no `CareCategories` table.

What changed: a standalone reference table was seeded and appointment deletion became restricted.

After Day 2: deleting a patient with an appointment is blocked at the relational database level; the model contains three predefined risk labels, but no API uses them.

### Important Files

- `CardiacPatientMonitoringDay2/CardiacPatientMonitoring.Api/Models/Entities.cs` — adds `CareCategory`.
- `.../Data/ApplicationDbContext.cs` — configures its length/seed data and changes appointment delete behavior.
- `.../Migrations/20260824170100_Day2EntitySetup.cs` — authoritative incremental database recipe.
- `CardiacPatientMonitoringDay2/Hands-On Lab.md` — official task/checklist and example EF commands.

### Database Impact

Creates `CareCategories` with identity `Id`, required `nvarchar(80)` `Name`, nullable `nvarchar(max)` `Description`, and three seed records. It changes the `Appointments.PatientId -> Patients.Id` foreign key from cascade to restrict. **Planned/Required — No implementation evidence found:** actual application of this migration to a live SQL Server database or GUI-based schema inspection.

### API Impact

No `CareCategoriesController`, care-category DTO, or care-category endpoint is present. Existing API routes remain unchanged for this incremental day.

### Testing

No Day 2-specific test-file addition is visible in the snapshot comparison. Existing unit/integration tests do not prove the SQL Server restrict foreign key or seed data behavior because the test provider is InMemory.

### Hands-On Lab

The lab is largely satisfied in source/migrations: entities, fluent relationships, seed data, and migration exist. **Planned/Required — No implementation evidence found:** a recorded `dotnet ef database update` output or SQL Server/GUI verification. The README reports a successful build, but this guide does not treat that prose alone as reproducible current verification.

### Definition of Done

**Partially complete.** Required model/migration elements are present; live-database application and verification are not evidenced. The "every table in the Day 1 ERD" wording is also imperfect because Day 1's future-baseline tables were never implemented and were explicitly future scope.

### Issues and Decisions

- `CareCategory` is unreferenced, so it adds schema without current business behavior.
- Restrict protects appointment history but can cause patient deletion to return an error unless related appointments are handled.
- The migration's date prefix is repository evidence; it is not proof that the migration ran against SQL Server.

### What I Should Understand

Explain why navigation properties do not by themselves decide delete behavior, how Fluent API does, why reference data needs stable keys, and why reviewing a migration catches unintended schema changes.

### Mentor Explanation

"I converted the reviewed model into explicit EF Core configuration. I added and seeded a care-category lookup and changed appointment deletion from cascade to restrict so a patient cannot be deleted while appointments still preserve scheduling history. I reviewed the resulting migration; a live SQL Server run is not recorded."

## Day 3 — Paginated Patient Catalog Endpoint

### Purpose

Day 3 makes the primary patient resource usable as a client-facing catalog rather than returning an unbounded list. It addresses scalability, predictable responses, input validation, DTO projection, and protected access.

### Learning Objectives

- Define request/query and response DTOs so external API shape is deliberate rather than an EF entity leak.
- Compose an `IQueryable` with optional filters, sorting, count, pagination, and database-side projection.
- Use deterministic ordering (`Id` tie-breaker) so page boundaries do not move unpredictably when values tie.
- Test controller delegation and authenticated HTTP access.

### Required Work

The lab requires an authenticated `GET /api/patients` supporting pagination, at least two filters, multiple sort modes, a paged DTO response, Postman scenarios, and automated verification.

### Actual Work

**Implemented — Found in repository:** `DTOs/Dtos.cs` adds `PatientCatalogQuery` and generic `PagedResponseDto<T>`. `IPatientService.GetAllAsync` changes from `GetAllAsync(string? search)` returning `IEnumerable<PatientResponseDto>` to `GetAllAsync(PatientCatalogQuery)` returning a paged response. `PatientService` now validates page values, filters by name search, exact gender, and `BornBefore`, sorts by `nameAsc`, `nameDesc`, `oldest`, or `newest`, counts before paging, and projects straight to `PatientResponseDto`.

`PatientsController.GetAll` now accepts `[FromQuery] PatientCatalogQuery`, advertises 200/400 response types, and remains protected by class-level `[Authorize]`. `PatientsControllerTests.cs` changes to test forwarding the query and response. `PatientEndpointIntegrationTests.cs` changes to read a paged response and verify an authenticated request sees seeded Alex Taylor; it also checks that an anonymous request receives 401.

No migration, entity, DbContext, or configuration changes appear from Day 2 to Day 3.

### Technical Explanation

`AsNoTracking()` avoids change tracking for read-only query results. The service retains an `IQueryable` until `CountAsync`/`ToListAsync`, allowing EF Core to translate filtering, ordering, skip/take, and projection into the database query. The `totalCount` is calculated before `Skip`/`Take`; `totalPages` is the ceiling of count divided by page size. Invalid page values and unsupported sort strings throw `ArgumentException`, which the exception middleware maps to HTTP 400.

### Project-Specific Explanation

The catalog returns only `Id`, name, date of birth, gender, and phone number—fields in `PatientResponseDto`—not navigation collections or EF entities. It is appropriate for browsing registered patients, but it does not implement clinician-scoped access: any authenticated JWT bearer can access it because authorization has no roles, claims policy, or patient-user ownership check.

### Before vs After

Before Day 3: `GET /api/patients` accepted only optional name search and returned an unpaged enumerable.

What changed: structured query parameters, filtering, four sorts, page limits (1–100), count metadata, DTO projection, and corresponding tests.

After Day 3: clients can call `GET /api/patients?page=1&pageSize=10&search=...&gender=...&bornBefore=...&sort=nameAsc` and receive `items`, `page`, `pageSize`, `totalCount`, and `totalPages`.

### Important Files

- `CardiacPatientMonitoringDay3/CardiacPatientMonitoring.Api/DTOs/Dtos.cs` — catalog query and generic paged-response contracts.
- `.../Services/Services.cs` — query composition, validation, ordering, counting, and projection.
- `.../Controllers/Controllers.cs` — endpoint model binding and documented result types.
- `.../Tests/PatientsControllerTests.cs` — controller-query forwarding test.
- `.../Tests/PatientEndpointIntegrationTests.cs` — authenticated/unauthenticated HTTP cases using `CustomWebApplicationFactory`.
- `CardiacPatientMonitoringDay3/Hands-On Lab.md` — requested scenarios and expected response structure.

### Database Impact

No schema migration or table change was created. The endpoint reads the existing `Patients` table. It uses query-time filtering/sorting; no indexes were added specifically for catalog search or filters.

### API Impact

**Real endpoint:** protected `GET /api/patients`.

Query values are `page` (default 1), `pageSize` (default 10; 1–100), `search`, `gender`, `bornBefore`, and `sort` (`nameAsc`, `nameDesc`, `oldest`, `newest`). Success returns 200 with a paged DTO. Invalid paging/sort travels through middleware as 400. Missing/invalid JWT is 401.

### Testing

The controller test proves the controller passes the catalog object to the service. The integration tests are designed to prove 200 for a JWT request and 401 with no JWT. Day 5 records that both integration tests later fail before assertions because middleware logging attempts a Windows Event Log write that is denied. Thus the test code exists, but successful end-to-end verification is **not** established.

### Hands-On Lab

The source completes the listed endpoint, filters, sort modes, page DTO, projection, and documented Postman combinations. The lab marks automated tests passing, but that conflicts with the later Day 5 recorded outcome (2 integration failures). **Planned/Required — No implementation evidence found:** executed Postman results for the catalog.

### Definition of Done

**Partially complete.** The endpoint and tests are implemented. End-to-end test execution is blocked by the logging permission error reported on Day 5; no Postman execution record is saved. The lab's checked "automated tests passing" item is not supported by the close-out evidence.

### Issues and Decisions

- Search uses `Contains` on first/last name; matching behavior and efficiency depend on provider/collation, and no dedicated search index exists.
- Filtered catalog access is authenticated but not role- or ownership-based.
- Invalid sort values deliberately return a generic 400 problem response rather than a detailed supported-values payload.
- HTTP catalog tests expose a Windows Event Log permission problem in the test host.

### What I Should Understand

Be able to explain why list endpoints need pagination, why count precedes paging, why stable sort matters, how model binding fills a query DTO, and why DTO projection helps prevent overexposure of entity data.

### Mentor Explanation

"I upgraded patient listing into an authenticated paginated catalog. The service applies search, gender and birth-date filters, four deterministic sort options, then counts and projects only patient response fields. I added controller and HTTP test coverage, though the HTTP tests need a logging-environment fix before they can be treated as passing verification."

## Day 4 — Transactional Order Creation and Stock Validation

### Purpose

Day 4 introduces business logic beyond CRUD: creating an order must validate availability, calculate money on the server, change stock consistently, and avoid partial persistence when a step fails.

### Learning Objectives

- Model a parent/line-item workflow using `Order` and `OrderItem`.
- Validate all inputs and stock before changing inventory.
- Treat stored product price as the authority; never trust totals supplied by a client.
- Use a transaction with a relational database so order, lines, and stock changes commit or roll back together.
- Add focused service tests for successful, rejected, and duplicate-item scenarios.

### Required Work

The Day 4 README/lab call for order creation with stock validation, server-calculated line/order totals, one transaction, a migration, focused verification, plus pull-request and mentor-review preparation.

### Actual Work

**Implemented — Found in repository:** Day 4 adds `Product`, `Order`, and `OrderItem` to `Models/Entities.cs`; `Products`, `Orders`, and `OrderItems` DbSets/configuration; request/response DTOs; `IOrderService`/`OrderService`; and protected `POST /api/orders`. `Program.cs` registers `IOrderService` as scoped.

Migration `20260826185619_AddOrdersAndProducts` creates the three tables, relationships, indexes, and a seed product (`Remote ECG Monitor`, stock 5). Migration `20260826210000_AddOrderMonetaryFields` adds `Product.UnitPrice`, `OrderItem.LineTotal`, and `Order.OrderTotal`, all `decimal(18,2)`, and updates the seed product to price 125.00.

`OrderService.CreateAsync` checks that the request has items, verifies the customer/patient, combines duplicate product lines by product ID, verifies each product exists and has stock, decrements stock only after all checks, calculates line totals from database `UnitPrice`, creates the order, saves, and returns server-calculated totals. It begins an EF transaction only when `db.Database.IsRelational()` is true, commits after save, and rolls back on exception.

`OrderServiceTests.cs` is added with three InMemory tests: stock available, insufficient stock leaves stock/orders unchanged, and duplicate product lines are summed before validation.

### Technical Explanation

`Order` is the parent; `OrderItem` is the child line item. `Order.CustomerId -> Patients.Id` has restrict delete, order-to-items cascades, and product-to-order-items restricts. This prevents deleting a customer with orders and prevents deleting a product referenced by an order item; deleting an order removes its lines.

Server-calculated totals are a security/integrity design: the input accepts product ID and quantity, not price or total. The service uses `Product.UnitPrice * Quantity`, stores the resulting `LineTotal`, and sums it into `OrderTotal`. `decimal(18,2)` is appropriate for currency-like values and avoids binary floating-point rounding issues.

The prevalidation pass means an insufficient second product does not leave the first product decremented in the tracked context. A relational transaction protects against failure after changes begin. EF InMemory does not implement relational transactions, so the code intentionally bypasses `BeginTransactionAsync` under that provider.

### Project-Specific Explanation

The new flow is labeled orders/products but uses patient IDs as customers and seeds a `Remote ECG Monitor`, making it plausibly a supply/device-order workflow inside the monitoring project. It is not yet integrated with the Day 1 future `MonitoringDevice` design: an order does not assign a monitor to a patient device record, and product stock does not represent device registration.

### Before vs After

Before Day 4: there was no product, order, order-item model, stock field, order endpoint, monetary fields, or order service.

What changed: three entities/tables, two migrations, a product seed, one protected POST endpoint, server-owned totals, stock logic, relational transaction path, and three focused tests.

After Day 4: a caller can create an order for an existing patient using product/quantity items. A successful call responds 201 with calculated totals and decrements stock; unavailable stock causes a client error through exception middleware.

### Important Files

- `CardiacPatientMonitoringDay4/CardiacPatientMonitoring.Api/Models/Entities.cs` — `Product`, `Order`, `OrderItem` properties/navigation collections.
- `.../Data/ApplicationDbContext.cs` — precision, FK/delete rules, DbSets, seed product.
- `.../Migrations/20260826185619_AddOrdersAndProducts.cs` — tables/FKs/indexes/initial product seed.
- `.../Migrations/20260826210000_AddOrderMonetaryFields.cs` — money columns and 125.00 seed price.
- `.../DTOs/Dtos.cs` — create request excludes client prices/totals; response includes server results.
- `.../Services/Services.cs` — aggregation, validation, stock mutation, calculations, and transaction.
- `.../Controllers/Controllers.cs` and `Program.cs` — `POST /api/orders` and service registration.
- `.../Tests/OrderServiceTests.cs` — focused business-rule coverage.

### Database Impact

Tables added: `Products` (`Id`, `Name`, `StockQuantity`, later `UnitPrice`), `Orders` (`Id`, `CustomerId`, `Status`, `CreatedAt`, later `OrderTotal`), and `OrderItems` (`Id`, `OrderId`, `ProductId`, `Quantity`, later `LineTotal`). Foreign-key indexes exist on `Orders.CustomerId`, `OrderItems.OrderId`, and `OrderItems.ProductId`. The database seed is one Remote ECG Monitor with stock 5 and final price 125.00.

**Planned/Required — No implementation evidence found:** applying either migration to SQL Server, live database schema inspection, or a live transactional rollback test.

### API Impact

**Real endpoint:** authenticated `POST /api/orders`.

Request body contains `customerId` and a non-empty `items` list with positive `productId` and `quantity` values. It has no client unit price, line total, or order total. On success, the controller returns 201 and an `OrderResponseDto`; documented 400/404 cases result from `ArgumentException`/`NotFoundException` middleware mapping. There is no GET orders endpoint or order endpoint integration test.

### Testing

**Actually verified (as recorded in Day 5):** focused `OrderServiceTests` passed 3/3. They prove, under EF InMemory, a valid order has a 125.50 unit price and 251.00 totals in the test setup, stock goes 5 to 3, insufficient stock persists no order and does not alter either product, and duplicate quantities are aggregated before stock validation.

They do **not** prove the SQL Server transaction path, real database constraints/migrations, authentication, controller model binding, HTTP status/body, or concurrency behavior. There is no `OrdersController` unit test or HTTP integration test.

### Hands-On Lab

The lab's first three implementation requirements are present and its reported build/focused test outcomes are corroborated by Day 5 close-out documentation. **Planned/Required — No implementation evidence found:** branch push, pull request, mentor review, order HTTP/Postman execution, and relational-provider transaction verification. The README explicitly says no PR or mentor review was performed.

### Definition of Done

**Partially complete.** Service-level business logic, migrations, and focused tests are present, but the broader delivery/review and endpoint/database verification portions are absent. It should not be called fully accepted until an authenticated HTTP scenario and SQL Server transaction/migration behavior are verified.

### Issues and Decisions

- The order workflow uses `Patient` as `Customer`; terminology is inconsistent with the clinical domain.
- The transaction does not solve concurrent stock races by itself; no row-version/concurrency token or atomic conditional stock update exists.
- Product price is read at order creation but not copied as a `UnitPrice` field on `OrderItem`; the response uses the current loaded product price while persistence stores only `LineTotal` and quantity. This preserves total but not an explicit historical per-unit snapshot if product price later changes.
- InMemory tests deliberately skip relational transactions and do not validate SQL Server behavior.
- No formal PR or mentor-review artifact is present.

### What I Should Understand

Explain parent/child order modeling, why a request must not control prices, why every stock check happens before decrement, why decimal precision matters for money, and what a transaction guarantees versus what an InMemory unit test proves.

### Mentor Explanation

"I added a transactional order workflow for patient customers and monitor products. The API accepts only product IDs and quantities, checks and aggregates stock first, calculates totals from the database price, then creates the order and reduces stock together. Three service tests verify the core success and rejection rules; HTTP and SQL Server transaction verification are still follow-up work."

## Day 5 — Sprint 1 Close-Out

### Purpose

Day 5 is an honest evidence-based close-out rather than a feature day. It separates behavior actually tested from code merely found in the repository and converts incomplete verification into a Sprint 2 backlog.

### Learning Objectives

- Use reproducible evidence, not documentation assertions, for completion claims.
- Compare implementation to acceptance outcomes without inventing external proof.
- Record verification gaps and convert them into concrete next-sprint actions.
- Conduct a retrospective distinguishing implemented work, verified work, and unverified work.

### Required Work

The close-out lab requests catalog/order verification, stock and totals review, migration/ERD status, acceptance review, backlog, retrospective, and any outstanding review feedback.

### Actual Work

The Day 4-to-Day 5 non-build source comparison shows **only `README.md` changed**. Day 5 adds/updates close-out documentation (`README.md` and lowercase `hand-on-lab.md`); it does not add a new entity, DTO, controller, service, migration, configuration change, or test.

**Actually verified, as reported in the Day 5 documents:** build succeeded with zero errors and four `NU1900` warnings because NuGet vulnerability data could not be reached. A full test run had 34 tests: 32 passed, 2 failed. The three focused order service tests passed. The two failed patient catalog integration tests were blocked before assertions when exception middleware's logger tried to write to Windows Event Log and access was denied.

### Technical Explanation

Close-out correctly distinguishes test layers. A passing service test establishes business logic in its test environment; it is not proof of an HTTP contract or a relational SQL transaction. An integration test using `WebApplicationFactory` invokes the application pipeline, so a logger/provider failure can prevent endpoint assertions even when endpoint code itself is correct. `NU1900` is package vulnerability-data retrieval trouble, not a compiler error.

### Project-Specific Explanation

The final project has a catalog endpoint and order service but lacks successful end-to-end evidence for either catalog HTTP behavior or order HTTP behavior. Its included Postman collection contains authentication, vital sign, medication, and appointment requests; the Day 5 documents report no catalog/order requests and no saved execution output. Therefore presence of a collection does not prove manual API verification.

### Before vs After

Before Day 5: the code had Day 4 functionality and documentation claimed/testing context was dispersed across days.

What changed: close-out documents classified the evidence, noted the failing test environment, and created an actionable Sprint 2 backlog.

After Day 5: implementation state is unchanged, but verification status is clearer: implemented and partially verified, not fully accepted.

### Important Files

- `CardiacPatientMonitoringDay5/README.md` — high-level close-out, results, retrospective, and Sprint 2 backlog.
- `CardiacPatientMonitoringDay5/hand-on-lab.md` — detailed evidence classification and command outcomes.
- `.../Tests/CustomWebApplicationFactory.cs` — sets `Testing`, selects InMemory through `Program.cs`, and calls `EnsureCreated`; relevant to the test-host behavior.
- `.../Middleware/ExceptionHandlingMiddleware.cs` — logs unexpected exceptions; the documented Windows Event Log permission failure occurs while handling test errors.

### Database Impact

No Day 5 database change or migration exists. The close-out lists the four migrations: `InitialCreate`, `Day2EntitySetup`, `AddOrdersAndProducts`, and `AddOrderMonetaryFields`. **Planned/Required — No implementation evidence found:** live SQL Server application of all four migrations.

### API Impact

No endpoint changed on Day 5. The close-out accurately records repository-found protected `GET /api/patients` and `POST /api/orders`, but no actual Postman/live-API results are stored for them.

### Testing

The project tests service, controller/middleware, and catalog HTTP behaviors. Day 5 reports 32/34 passed, with two catalog integration failures caused by Event Log permission rather than order business logic. This is a serious verification blocker: the catalog HTTP behavior cannot be described as currently passing. The recommended remedy is to configure test logging so it does not write to Windows Event Log, then rerun the two catalog tests and add authenticated order endpoint tests.

### Hands-On Lab

The close-out lab is complete as a documentation artifact and candidly flags unverified work. It identifies no PR, no mentor review/feedback record, no formal Sprint 1 acceptance-criteria artifact, no order HTTP test, no Postman execution, and no SQL Server rollback/migration verification.

### Definition of Done

**Partially complete.** The final documents explicitly say Sprint 1 is implemented and partially verified, not fully accepted. That is the most defensible status: core order service behavior is verified, while catalog HTTP, order HTTP, relational transaction/migration verification, formal acceptance sign-off, and review evidence remain open.

### Issues and Decisions

- Test host logging needs to be independent of Windows Event Log permissions.
- There are no successful catalog integration-test results after the failure.
- There is no order endpoint integration coverage.
- The Postman collection lacks catalog/order scenarios and no executions are stored.
- No formal Sprint 1 acceptance checklist, PR, or mentor-feedback record was found in the repository.

### What I Should Understand

You should distinguish implementation from verification, unit/service tests from integration tests, and an endpoint present in source from an endpoint proven in a real HTTP run. A useful close-out records unknowns as work, rather than claiming them complete.

### Mentor Explanation

"I closed the sprint by checking evidence rather than treating code presence as proof. The order service tests passed, but the full suite has two catalog integration tests blocked by Windows Event Log permissions. I documented the gap and queued fixes for test-host logging, order HTTP coverage, Postman execution, SQL Server transaction verification, and formal acceptance criteria."

## Week 06 — Complete Story

```text
Day 1: inspect/document current EF Core schema and future clinical baseline
  ↓
Day 2: add CareCategory seed data and restrict appointment deletion
  ↓
Day 3: turn patient listing into an authenticated, paginated/filterable catalog
  ↓
Day 4: add products/orders and transactional stock/total business rules
  ↓
Day 5: close out with evidence classification and a verification backlog
```

The week moves from database design to an incremental EF migration, then a read/query API feature, then a write/business-transaction feature. The close-out shows that code grew faster than external verification: the final snapshot contains meaningful implementation, but database and HTTP proof are incomplete.

## Project Evolution

### Before Week 06

No earlier Week 06 snapshot is supplied, so the precise pre-week state cannot be verified. The earliest Day 1 snapshot already has the InitialCreate model: Identity, patients, vital signs, medications, appointments, authentication/JWT configuration, CRUD-style controllers/services, middleware, seed clinical data, a Postman collection, and tests.

### After Week 06

| Area | Implemented current state | Future / unverified state |
| --- | --- | --- |
| Database / EF Core | Four migrations; Identity; patient, vital-sign, medication, appointment, care-category, product, order, order-item tables; explicit FKs and seeds | SQL Server migration application and relational transaction rollback not verified |
| Identity / security | `ApplicationUser`, Identity stores, JWT register/login, class-level `[Authorize]` on application resources | Role services/policies, patient-user ownership, clinician authorization not implemented |
| API | Protected patient catalog; CRUD-style patient/vital/medication/appointment endpoints; `POST /api/orders` | Order GET/update endpoints, clinician/device/alert APIs; successful manual catalog/order calls not evidenced |
| Testing | Service, controller/middleware, and catalog integration test source; focused order tests reported 3/3 pass | Full suite green; passing catalog HTTP execution; order endpoint integration; relational DB tests |
| Documentation | Day READMEs/labs, ERD PNGs, PDFs, Postman collection, this guide | Formal Sprint 1 acceptance checklist, PR/review record, recorded Postman run |

## Skills Developed During Week 06

### Backend Development

- Layering controllers, DTOs, and scoped services so HTTP concerns, business rules, and persistence are separated.
- Writing validation and domain checks: dates, vital-sign consistency, appointment/medication checks, and order stock checks.
- Returning appropriate 201/204/400/401/404/500 outcomes through controllers and exception middleware.

### Database / EF Core

- Reading EF Core entities, fluent configuration, migrations, designers, and snapshots as a single schema source of truth.
- Modeling one-to-many relationships and intentional cascade/restrict deletion.
- Creating incremental migrations and `HasData` seeds.
- Configuring decimal precision and distinguishing relational-provider behavior from InMemory behavior.

### Authentication & Security

- Configuring Identity stores, JWT issuance, JWT bearer validation, and `[Authorize]`.
- Avoiding client-supplied prices/totals; mapping unexpected exceptions to a generic problem response without exposing secrets.

### API Development

- Building a paginated, filterable, sortable query endpoint with stable ordering and a generic envelope.
- Designing an order-create contract that accepts only the inputs clients should control.

### Testing

- Unit-testing services with EF InMemory and mocking controller/service boundaries.
- Creating `WebApplicationFactory` HTTP tests and recognising their environmental limitations.
- Separating passing focused tests from unverified or failing end-to-end scenarios.

### Software Engineering / Agile

- Creating a sprint goal, task breakdown, definition of done, risk log, and evidence-based retrospective.
- Converting verification gaps into prioritized follow-up work instead of silently closing them.

### Documentation

- Producing DBML/ERD-oriented schema documentation.
- Clearly separating current implementation, future design, documented claims, and actually verified outcomes.

## Final Week 06 Summary

Week 06 documents and extends a Cardiac Patient Monitoring API from its existing EF Core clinical records model to a richer snapshot with a seeded care-category lookup, a secure paginated patient catalog, and a transactional order/stock workflow. The strongest verified outcome is the order service's core business logic (3 focused tests passed). The most important remaining work is not another feature: make integration-test logging portable, verify catalog and order HTTP paths, run migrations and rollback scenarios against SQL Server, and establish formal acceptance/review evidence.
