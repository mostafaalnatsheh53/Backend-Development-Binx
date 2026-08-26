# Cardiac Patient Monitoring - Day 4

## Day 4 overview

Day 4 extends the Cardiac Patient Monitoring API beyond simple CRUD by introducing order creation with stock-aware business rules. The project uses ASP.NET Core, EF Core, SQL Server, ASP.NET Identity, JWT authentication, and xUnit tests.

## Learning objectives

- Keep HTTP handling in controllers and business rules in services.
- Validate stock before persisting an order.
- Calculate money values on the server from trusted database data.
- Use an EF Core transaction for related database changes.
- Verify business rules with focused automated tests.

## Existing API structure

- `Models/Entities.cs` contains `Patient`, `Product`, `Order`, and `OrderItem` entities.
- `DTOs/Dtos.cs` contains `CreateOrderRequestDto`, `CreateOrderItemDto`, and order response DTOs.
- `Controllers/Controllers.cs` exposes the authorized `POST /api/orders` endpoint through `OrdersController`.
- `Services/Services.cs` contains `IOrderService` and `OrderService`.
- `Data/ApplicationDbContext.cs` configures entity relationships, decimal precision, and product seed data.

## Business logic beyond simple CRUD

An order cannot be created by simply saving the request body. `OrderService.CreateAsync` applies business rules before saving data:

1. Confirms that the customer exists.
2. Combines quantities when the same product appears more than once in a request.
3. Loads all requested products from the database.
4. Confirms every product exists and has sufficient stock.
5. Decrements stock, creates the order items, and saves the order only after validation succeeds.

## Order creation flow

Send an authenticated request to `POST /api/orders`:

```json
{
  "customerId": 1,
  "items": [
    { "productId": 1, "quantity": 2 }
  ]
}
```

The endpoint accepts `CreateOrderRequestDto` and delegates all business rules to `OrderService`. It returns `201 Created` with an `OrderResponseDto` when successful. Invalid requests and insufficient stock are handled through the application's Problem Details middleware.

## Stock validation

`Product.StockQuantity` is checked for every requested product before stock is decremented or an order is added. If any product is unknown or lacks stock, the service throws a client-error exception and no order is persisted. This prevents partial orders when one item cannot be fulfilled.

## Line total and order total calculation

The client supplies only product IDs and quantities. The service reads `Product.UnitPrice` from the database and calculates:

```text
LineTotal = UnitPrice x Quantity
OrderTotal = Sum(LineTotal)
```

`OrderItem.LineTotal` and `Order.OrderTotal` use `decimal(18,2)` database precision. Client-provided prices and totals are not trusted.

## Database transactions

For relational providers, `OrderService.CreateAsync` starts an EF Core transaction with `BeginTransactionAsync`. The transaction includes order creation, order-item creation, and stock decrement. It commits only after `SaveChangesAsync` succeeds and rolls back if an exception occurs.

The test project uses EF Core's in-memory provider, which does not support relational transactions; transaction behavior is therefore applied in the SQL Server runtime while the unit tests verify the order business rules.

## Database migration

`20260826210000_AddOrderMonetaryFields` adds the `UnitPrice`, `LineTotal`, and `OrderTotal` monetary columns to the existing product and order tables. The existing product seed is assigned a unit price of `125.00`.

## Pull request preparation

No branch was pushed and no pull request was opened as part of this work. Before opening one, create a feature branch, review `git diff`, run the verification commands below, and use a focused commit such as:

```text
Implement transactional order creation with stock validation
```

## Mentor code review

No mentor review request or feedback is recorded in this local project. The implementation is ready for mentor review after the branch is pushed and a pull request is opened.

## Hands-on lab

The concise implementation record and requirement status are in [Hands-On Lab.md](Hands-On%20Lab.md).

## Verification and testing

```powershell
dotnet build .\CardiacPatientMonitoring.slnx --nologo
dotnet test .\CardiacPatientMonitoring.Tests\CardiacPatientMonitoring.Tests.csproj --nologo --filter FullyQualifiedName~OrderServiceTests
```

- Build: passed with 0 warnings and 0 errors.
- Focused order tests: passed, 3 of 3.
- Full test run: 32 passed and 2 existing patient integration tests failed because Windows Event Log write access was denied while middleware logged an exception. This is unrelated to order creation.
