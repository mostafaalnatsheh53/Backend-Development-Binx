# Day 5 — API Security Hardening 🛡️

## Overview

Day 5 focused on strengthening the API with additional security controls and reviewing common API security risks.

## What I Learned

* 🛡️ Rate Limiting
* 🌐 CORS configuration
* 🔒 HTTPS and HSTS
* 🧱 Security Headers
* 💉 SQL Injection Prevention

## Implementation

* Configured rate limiting to control excessive requests
* Configured a named CORS policy for trusted origins
* Enabled HTTPS redirection
* Configured HSTS for secure communication
* Reviewed security headers and their role in API protection
* Reviewed Entity Framework Core parameterized queries and SQL injection prevention

## Security Review

The API was reviewed to ensure that:

* Sensitive endpoints are protected against excessive requests
* Only trusted origins can access the API from browsers
* HTTP traffic is redirected to HTTPS
* Secure headers are configured
* User input is not directly concatenated into SQL queries

## Technologies

* C#
* ASP.NET Core
* Entity Framework Core
* CORS
* Rate Limiting
* HTTPS / HSTS

## Key Takeaway

Hardened the API with additional security controls and reviewed common vulnerabilities that can affect production backend applications.
