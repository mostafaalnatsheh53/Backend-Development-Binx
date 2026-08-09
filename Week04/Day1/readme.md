# Day 1 — ASP.NET Core Identity & User Registration

## Overview

In this day, I learned how to use ASP.NET Core Identity to manage users and implement user registration in an ASP.NET Core Web API.

## Topics Covered

- ASP.NET Core Identity
- Identity with Entity Framework Core
- `IdentityDbContext`
- `IdentityUser` and `IdentityRole`
- `UserManager`
- User Registration
- Password Hashing
- PBKDF2 and Salt

## Implementation

### Identity Setup

Updated `AppDbContext` to inherit from `IdentityDbContext` and added Identity services in `Program.cs`.

### User Registration

Implemented:

```http
POST /api/Auth/register