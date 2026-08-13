# Day 5 — API Security Hardening

## Overview

Implemented security hardening measures for the ASP.NET Core API to protect sensitive endpoints and reduce common security risks.

## What I Learned

- Configuring Rate Limiting in ASP.NET Core
- Applying stricter rate limits to sensitive endpoints
- Configuring named CORS policies
- Enabling HTTPS Redirection and HSTS
- Adding Content-Security-Policy headers
- Preventing SQL Injection with Entity Framework Core
- Understanding parameterized queries
- Testing API security configurations with Postman

## Implementation

### Rate Limiting

Configured separate rate limiting policies for general endpoints and the login endpoint.

- General endpoints: `30 requests per minute`
- Login endpoint: `5 requests per minute`
- Exceeded limits return `429 Too Many Requests`

### CORS

Implemented a named CORS policy called `AllowFrontend`.

- Allows only the specified frontend origin
- Allows required HTTP headers
- Allows required HTTP methods
- Verified allowed and disallowed origins

### Security Headers

Configured security-related middleware including:

- HTTPS Redirection
- HSTS
- Content-Security-Policy

These settings provide additional protection against common web security risks.

### SQL Injection Prevention

Reviewed the API codebase to ensure Entity Framework Core queries use parameterization by default.

Confirmed that:

- LINQ queries are parameterized automatically
- No unsafe raw SQL queries using unparameterized string interpolation are used
- `FromSqlRaw` with direct user input is avoided

## Testing

Tested the implemented security features using **Postman**, including:

- Rate limiting on the login endpoint
- General endpoint rate limiting
- CORS origin configuration
- API responses after exceeding request limits
- Review of database queries for SQL Injection risks

## Technologies

- C#
- ASP.NET Core
- ASP.NET Core Identity
- JWT Authentication
- Entity Framework Core
- FluentValidation
- SQL Server
- Postman