# Day 1 — ASP.NET Core Identity & User Registration 🔐

## Overview

Day 1 focused on integrating ASP.NET Core Identity into the API and implementing secure user registration.

## What I Learned

* 🔐 ASP.NET Core Identity and its role in user management
* 🗄️ Integrating Identity with Entity Framework Core
* 👤 Using `IdentityUser` and `IdentityRole`
* 🔑 Secure password hashing with Identity
* 📝 Implementing a user registration endpoint

## Implementation

* Extended `AppDbContext` with `IdentityDbContext`
* Configured ASP.NET Core Identity in `Program.cs`
* Added Identity migrations to the database
* Implemented user registration using `UserManager`
* Tested valid and invalid registration requests using Postman

## Technologies

* C#
* ASP.NET Core
* ASP.NET Core Identity
* Entity Framework Core
* SQL Server
* Postman

## Key Takeaway

Built the authentication foundation of the API using ASP.NET Core Identity and secure password management.
