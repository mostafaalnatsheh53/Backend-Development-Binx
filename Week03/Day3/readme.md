# Day 3 - Entity Framework Core Setup & Code-First Migrations

## Overview

This project demonstrates how to integrate Entity Framework Core with SQL Server using the Code-First approach. It includes creating entity models, configuring the DbContext, setting up the database connection, generating migrations, and applying them to create the database schema.

---

## Learning Objectives

- Install and configure Entity Framework Core with SQL Server.
- Define entity classes and configure relationships.
- Create and configure a DbContext.
- Use Code-First Migrations.
- Configure SQL Server connection strings.
- Apply migrations to create the database.

---

## Technologies Used

- .NET
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server Express
- EF Core Migrations

---

## Project Structure

```
Day3
│
├── Data
│   └── AppDbContext.cs
│
├── Models
│   ├── Customer.cs
│   ├── Product.cs
│   ├── Order.cs
│   └── OrderItem.cs
│
├── Migrations
│
├── Program.cs
└── appsettings.json
```

---

## Database Entities

- Customer
- Product
- Order
- OrderItem

Relationships:

- Customer → Orders (One-to-Many)
- Order → OrderItems (One-to-Many)
- Product → OrderItems (One-to-Many)

---

## EF Core Setup

Installed the required packages:

```bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
```

---

## Connection String

Configured SQL Server Express in `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=Day3Db;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

---

## Register DbContext

Registered the DbContext in `Program.cs`:

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"));
});
```

---

## Create Migration

```bash
dotnet ef migrations add InitialCreate
```

---

## Apply Migration

```bash
dotnet ef database update
```

---

## What I Learned

- How Entity Framework Core works.
- How to create entity models.
- How to configure DbContext.
- How to connect an ASP.NET Core application to SQL Server.
- How Code-First Migrations work.
- How to create and update a database using EF Core.
- The importance of connection strings and application configuration.

---

## Outcome

Successfully configured Entity Framework Core with SQL Server, created the database schema using Code-First Migrations, and prepared the project for implementing CRUD operations in future lessons.