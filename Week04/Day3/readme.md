# Day 1 — Authentication & Authorization with ASP.NET Core Identity

## Overview

This day focused on implementing authentication and authorization in an ASP.NET Core Web API using ASP.NET Core Identity, JWT authentication, roles, claims, and policy-based authorization.

The implementation was tested using Postman to verify protected routes, role-based access, policy-based access, and JWT token handling.

---

## Learning Objectives

By the end of this day, the following concepts were implemented:

- ASP.NET Core Identity
- User registration and login
- JWT authentication
- JWT claims
- Role-based authorization
- Claims-based authorization
- Policy-based authorization
- Protecting API endpoints with `[Authorize]`
- Restricting endpoints to specific roles
- Testing `401 Unauthorized` and `403 Forbidden`
- Managing roles using `UserManager` and `RoleManager`
- Reusing JWT tokens in Postman

---

# 1. ASP.NET Core Identity

ASP.NET Core Identity was used to manage application users and roles.

Identity was connected to Entity Framework Core using the application database context.

```csharp
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();