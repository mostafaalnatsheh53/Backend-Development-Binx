# Day 4 — Input Validation with FluentValidation

## Overview

Implemented input validation for the ASP.NET Core API using **FluentValidation** to ensure incoming requests follow defined validation and business rules.

## What I Learned

- DataAnnotations vs FluentValidation
- Creating validators with `AbstractValidator`
- Using `RuleFor` to define validation rules
- Integrating FluentValidation into the ASP.NET Core pipeline
- Returning structured `400 Bad Request` responses
- Validating Create and Update requests
- Testing validation rules with Postman

## Implementation

### CreateOrderRequest

Implemented validation rules for:

- `CustomerId` must be greater than `0`
- `Total` must be greater than `0`
- `OrderDate` cannot be in the future

### UpdateOrderRequest

Implemented validation for:

- `CustomerId` must be greater than `0`

## API Validation

FluentValidation was registered in the ASP.NET Core pipeline so invalid requests are automatically rejected before reaching the controller logic.

Validation errors are returned using a structured response containing:

- HTTP Status Code `400`
- Field name
- Specific validation error message
- Trace ID

## Testing

Tested the validation rules using **Postman** by sending invalid requests and verifying that each rule returns the expected error message.

## Technologies

- C#
- ASP.NET Core
- FluentValidation
- Entity Framework Core
- SQL Server
- Postman