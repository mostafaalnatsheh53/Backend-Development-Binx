# Day 2 — JWT Authentication & Token Issuance 🔑

## Overview

Day 2 focused on implementing JWT-based authentication and issuing access tokens after a successful login.

## What I Learned

* 🔑 JWT structure and claims
* 👤 User authentication with ASP.NET Core Identity
* 🎟️ Creating and signing JWT tokens
* 🛡️ Configuring JWT Bearer Authentication
* ⏱️ Token expiration and lifetime validation

## Implementation

* Implemented a login endpoint using `SignInManager`
* Verified user credentials securely
* Created JWT claims for authenticated users
* Generated signed JWT access tokens
* Configured JWT Bearer authentication in `Program.cs`
* Configured issuer, audience, signing key, and token lifetime
* Tested authentication using Postman

## Technologies

* C#
* ASP.NET Core
* ASP.NET Core Identity
* JWT Bearer Authentication
* Entity Framework Core
* Postman

## Key Takeaway

Implemented a complete JWT authentication flow that allows authenticated users to securely access protected API resources.
