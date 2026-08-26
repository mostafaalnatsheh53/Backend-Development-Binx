# Hands-On Lab: Implement Order Creation and Open a Pull Request

## Objective

Implement an order-creation endpoint that checks product stock before creating an order. The API must reject an order when the requested quantity is greater than the available stock, and it must never create a partial order in that case.

By the end of this lab, you should have:

- Order and order-item models.
- A request and response DTO for order creation.
- A `POST /api/orders` endpoint.
- Stock validation before persistence.
- Line totals and an order total calculated during creation.
- Tests for successful and rejected orders.
- A focused Git branch and pull request.

## Suggested API Contract

### Endpoint

```http
POST /api/orders
Authorization: Bearer <token>
Content-Type: application/json
```

### Request body

```json
{
  "customerId": 1,
  "items": [
    {
      "productId": 10,
      "quantity": 2,
      "unitPrice": 125.00,
      "lineTotal": 250.00
    }
  ],
  "orderTotal": 250.00
}
```

### Successful response

Return `201 Created` with the created order and its generated identifier.

```json
{
  "id": 25,
  "customerId": 1,
  "status": "Created",
  "items": [
    {
      "productId": 10,
      "quantity": 2
    }
  ]
}
```

### Insufficient-stock response

Return `400 Bad Request` using the application's existing ProblemDetails middleware. The response should explain that there is insufficient stock and identify the affected product.

The database must remain unchanged when any item fails the stock check.

## 2. Calculate Line Totals and the Order Total

Add `UnitPrice` to `Product`, `LineTotal` to `OrderItem`, and `OrderTotal` to `Order`.
Use `decimal` values with two database decimal places for monetary amounts.

During order creation, after all stock checks pass:

1. Calculate each line total as `UnitPrice * Quantity`.
2. Calculate the order total as the sum of all line totals.
3. Store both the line totals and the order total with the order.
4. Return the calculated amounts in the response.

The total must be calculated on the server from the product price. Do not accept a client-provided total as authoritative. The stock validation must still happen before any order or stock changes are persisted.

## Implementation Tasks

### 3. Create the domain models

Add the entities required by the existing EF Core data layer:

- `Product`: `Id`, `Name`, `UnitPrice`, and `StockQuantity`.
- `Order`: `Id`, `CustomerId`, `Status`, `CreatedAt`, `OrderTotal`, and a collection of items.
- `OrderItem`: `Id`, `OrderId`, `ProductId`, `Quantity`, and `LineTotal`.

Use suitable required fields and configure relationships in `ApplicationDbContext`.

### 4. Create DTOs

Add:

- `CreateOrderRequestDto` containing `CustomerId` and a required list of items.
- `CreateOrderItemDto` containing `ProductId` and a positive `Quantity`.
- Response DTOs for the created order and its items.

Use validation attributes where they provide useful request validation. Reject empty item lists and non-positive quantities.

### 5. Implement the order service

Create an order service following the existing service pattern.

The create operation should:

1. Validate the request.
2. Load all requested products in one database query.
3. Confirm that every product exists.
4. Confirm that each requested quantity is available.
5. Decrease stock for every requested product.
6. Calculate and assign each line total and the order total.
7. Create the order and order items.
8. Save the complete operation atomically.
9. Return the created order.

Do not save the order before all stock checks pass. Consider a transaction or a concurrency-safe update if the application can receive simultaneous orders.

### 6. Add the controller endpoint

Add an authorized `OrdersController` with:

```csharp
[HttpPost]
public async Task<ActionResult<OrderResponseDto>> Create(
    CreateOrderRequestDto request)
```

Use `CreatedAtAction` or another appropriate `201 Created` response. Keep business rules in the service rather than in the controller.

### 7. Register dependencies and database changes

Register the service in `Program.cs`, create an EF Core migration, and update the database using the repository's normal migration workflow.

Add seed data or test setup that provides at least one product with known stock, for example:

- Product 10: stock quantity `5`.
- Product 10: unit price `125.00`.

## Tests

Add focused tests covering:

1. A valid order returns `201 Created` and decreases stock.
2. A valid order returns the expected line total and order total.
3. An order requesting more than available stock returns `400 Bad Request`.
4. An insufficient-stock request does not create an order.
5. A request containing multiple items is rejected without changing any product stock when one item is unavailable.
6. A request for an unknown product returns the expected client error.
7. Invalid quantities are rejected by validation.

Run the tests from the Day4 directory:

```powershell
dotnet test .\CardiacPatientMonitoring.Tests\CardiacPatientMonitoring.Tests.csproj --nologo
```

Build the solution:

```powershell
dotnet build .\CardiacPatientMonitoring.slnx --nologo
```

## Manual Verification

1. Start the API in Development mode.
2. Authenticate and copy the JWT token.
3. Create an order whose quantity is within stock.
4. Confirm the response is `201 Created`.
5. Submit an order whose quantity exceeds stock.
6. Confirm the response is `400 Bad Request` with `application/problem+json`.
7. Verify that no order was created and that stock was not reduced by the rejected request.

The endpoint can be tested through Swagger or the Day4 Postman collection.

## Pull Request Workflow

Create a feature branch before making changes:

```powershell
git switch -c feat/order-creation-stock-validation
```

Review the changes and run the tests:

```powershell
git diff
dotnet test .\CardiacPatientMonitoring.Tests\CardiacPatientMonitoring.Tests.csproj --nologo
dotnet build .\CardiacPatientMonitoring.slnx --nologo
```

Commit the implementation with a focused message:

```powershell
git add .
git commit -m "Implement order creation with stock validation"
```

Push the branch:

```powershell
git push -u origin feat/order-creation-stock-validation
```

Open a pull request with:

- A short summary of the endpoint and stock rule.
- The test commands and their results.
- Example success and insufficient-stock requests.
- Any assumptions about concurrency, transactions, or product data.

## Definition of Done

- The endpoint is protected by the existing authentication setup.
- Valid orders are persisted and stock is decreased correctly.
- Insufficient stock returns a client error and makes no database changes.
- All new behavior is covered by automated tests.
- The solution builds successfully.
- The branch is pushed and a pull request is opened for review.
