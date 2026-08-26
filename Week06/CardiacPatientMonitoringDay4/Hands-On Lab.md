# Hands-On Lab: Order Creation

## Implementation status

1. **Order creation with stock validation — complete.** `OrdersController.Create` accepts `CreateOrderRequestDto` and delegates to `OrderService.CreateAsync`. The service loads the requested products, combines duplicate product quantities, verifies product existence and stock before it changes any data, and rejects insufficient stock with a client error.
2. **Line and order totals — complete.** `OrderService` uses `Product.UnitPrice` from the database to calculate each `OrderItem.LineTotal` and then `Order.OrderTotal`. The request DTO does not accept client prices or totals.
3. **Single database transaction — complete.** For relational EF Core providers, `OrderService.CreateAsync` uses `BeginTransactionAsync`; order creation, order-item creation, and stock decrements are saved together, committed on success, and rolled back on failure. The in-memory test provider has no transaction support, so its focused tests validate the business logic without opening a database transaction.
4. **Pull request — not performed.** The current branch is `main`; no branch was pushed and no pull request was opened.
5. **Mentor review — not performed.** No mentor review request or feedback was found in the local repository.

## Verification

- `dotnet build .\CardiacPatientMonitoring.slnx --nologo` - passed with 0 warnings and 0 errors.
- `dotnet test .\CardiacPatientMonitoring.Tests\CardiacPatientMonitoring.Tests.csproj --nologo --filter FullyQualifiedName~OrderServiceTests` - passed: 3/3 order tests.
- The full `dotnet test` run completed with 32 passing and 2 failing existing patient integration tests. Both failures are caused by denied Windows Event Log write access while exception middleware logs, not by order creation.
