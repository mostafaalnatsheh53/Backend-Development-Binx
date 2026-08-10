# Day 2 — JWT Authentication & Token Issuance

## Overview

This day focused on implementing authentication using ASP.NET Core Identity and JSON Web Tokens (JWT).

The API was extended with user registration, login, JWT token issuance, Bearer authentication, claims, and token expiration.

---

## Learning Objectives

- Understand JWT structure and claims.
- Implement user registration and login using ASP.NET Core Identity.
- Issue a signed JWT after successful authentication.
- Configure JWT Bearer authentication.
- Validate JWT issuer, audience, signing key, and expiration.
- Understand short-lived access tokens and refresh tokens.

---

## Topics Covered

### 1. JWT Structure and Claims

A JWT consists of three parts:

- Header
- Payload
- Signature

The payload contains claims such as the user's ID and email.

The JWT is signed, not encrypted, so sensitive information should not be stored in its claims.

---

### 2. User Registration

Implemented a registration endpoint using ASP.NET Core Identity.

**Endpoint:**

```http
POST /api/Auth/register