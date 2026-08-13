# Week 4 — Authentication, Authorization & Input Validation 🔐

## Overview

Week 4 focused on securing the ASP.NET Core API by adding authentication, authorization, input validation, and API security hardening.

The week covered the complete flow from user registration and login to protected routes, role-based access control, request validation, and additional security measures.

## What I Learned

* 🔐 ASP.NET Core Identity
* 🎟️ JWT Authentication
* 🛡️ Authorization & Role-Based Access Control
* ✅ Input Validation with FluentValidation
* 🚦 Rate Limiting
* 🌐 CORS
* 🔒 HTTPS & HSTS
* 🧱 Security Headers
* 💉 SQL Injection Prevention

## Day 1 — ASP.NET Core Identity 👤

Integrated ASP.NET Core Identity with Entity Framework Core and implemented user registration.

### Key Topics

* `IdentityUser`
* `IdentityRole`
* `IdentityDbContext`
* `UserManager`
* Secure password hashing
* User registration

### Result

Built the foundation for secure user management and authentication.

---

## Day 2 — JWT Authentication 🔑

Implemented JWT-based authentication and token issuance after successful login.

### Key Topics

* JWT structure and claims
* Login endpoint
* JWT token generation
* JWT Bearer Authentication
* Token expiration
* Issuer, audience, and signing key validation

### Result

Users can authenticate and receive a signed JWT to access protected API resources.

---

## Day 3 — Authorization & RBAC 🛡️

Added authorization controls to restrict access to API resources.

### Key Topics

* `[Authorize]`
* `User` and `Admin` roles
* Role-Based Access Control
* Claims-Based Authorization
* Policy-Based Authorization
* `401 Unauthorized`
* `403 Forbidden`

### Result

Protected API operations based on authenticated users, roles, and permissions.

---

## Day 4 — FluentValidation ✅

Added structured input validation using FluentValidation.

### Key Topics

* `AbstractValidator`
* `RuleFor`
* Create request validation
* Update request validation
* Business rules
* Structured validation errors

### Result

Invalid requests are rejected before reaching the controller logic with clear validation messages.

---

## Day 5 — API Security Hardening 🔒

Focused on additional security controls used to harden production APIs.

### Key Topics

* Rate Limiting
* CORS
* HTTPS Redirection
* HSTS
* Security Headers
* SQL Injection Prevention

### Result

Reviewed and applied additional security measures to reduce common API security risks.

---

## Testing 🧪

Postman was used throughout the week to test:

* User registration
* Login
* JWT authentication
* Protected endpoints
* Role-based access
* Unauthorized and forbidden requests
* Invalid input
* Validation error responses
* API security configurations

## Technical Stack

| Category             | Technology            |
| -------------------- | --------------------- |
| Language             | C#                    |
| Framework            | ASP.NET Core          |
| Authentication       | ASP.NET Core Identity |
| Token Authentication | JWT Bearer            |
| Authorization        | Roles & Policies      |
| Validation           | FluentValidation      |
| ORM                  | Entity Framework Core |
| Database             | SQL Server            |
| API Testing          | Postman               |

## Key Takeaway

Week 4 transformed the API from a basic CRUD application into a more secure backend by implementing authentication, authorization, input validation, and security hardening techniques used in real-world ASP.NET Core applications.
