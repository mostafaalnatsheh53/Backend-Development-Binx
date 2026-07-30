# Day 5 – Middleware Pipeline & Dependency Injection

## Overview

This project demonstrates the core concepts of the ASP.NET Core request pipeline, custom middleware, middleware ordering, dependency injection (DI), service lifetimes, and constructor injection.

---

## Topics Covered

### 1. Middleware Pipeline
- Understanding the ASP.NET Core request pipeline.
- How every HTTP request passes through middleware components.
- Creating custom middleware.
- Executing code before and after the next middleware.

### 2. Built-in Middleware
- UseHttpsRedirection()
- MapControllers()
- OpenAPI integration

### 3. Middleware Ordering
- Learned that middleware executes in the order it is registered.
- Tested incorrect middleware placement.
- Observed how incorrect ordering affects request execution.
- Restored the correct pipeline order.

### 4. Dependency Injection (DI)
- Registered services using the built-in DI container.
- Used Scoped lifetime for service registration.
- Learned the difference between:
  - AddTransient
  - AddScoped
  - AddSingleton

### 5. Constructor Injection
- Injected services into controllers using constructor injection.
- Used interfaces instead of concrete implementations.
- Improved loose coupling and maintainability.

---

## Hands-On Lab

### Task 1
Created a custom middleware that logs:
- HTTP Method
- Request Path

### Task 2
Placed the middleware in an incorrect position to observe the pipeline behavior, then corrected the order.

### Task 3
Created:
- IOrderService
- OrderService

Registered the service using:

```csharp
builder.Services.AddScoped<IOrderService, OrderService>();
```

### Task 4
Injected `IOrderService` into the controller using constructor injection and consumed it inside an API endpoint.

### Task 5
Prepared the Week 2 summary in Notion including:
- Generics
- LINQ
- Async/Await
- ASP.NET Core API
- Middleware
- Dependency Injection
- GitHub repository

---

## Project Structure

```
Day5
│
├── Controllers
│   └── HandsOnLabController.cs
│
├── Services
│   ├── IOrderService.cs
│   └── OrderService.cs
│
├── Program.cs
└── README.md
```

---

## What I Learned

- How the ASP.NET Core middleware pipeline processes requests.
- Why middleware order is critical.
- How Dependency Injection works internally.
- The differences between Transient, Scoped, and Singleton lifetimes.
- How Constructor Injection improves code flexibility.
- Why interfaces are preferred over concrete classes.

---

## Technologies Used

- C#
- .NET SDK
- ASP.NET Core
- OpenAPI
- Postman
- Visual Studio Code
- Git & GitHub
- Notion