# ADR-0002 — Authentication v1 (JWT access tokens + refresh tokens)

> **Status:** Accepted (v1), with roadmap  
> **Date:** 2026-02-15  
> **Navigation:** [ADRs index](./README.md) · [Docs index](../README.md)

## Context
We need authentication early, but the product is **self-hosted-first** and the backend uses a **custom DB communication library**.
We want something minimal that ships quickly, without committing to a full identity platform before core workflows stabilize.

## Decision
Implement a **token-based auth v1**:
- login with username/email + password
- API issues **JWT access token** + **refresh token**
- refresh tokens are stored **in-memory for now** (migrate to DB later)

## Options considered
- **ASP.NET Core Identity**
  - ✅ Mature and feature-rich
  - ❌ Opinionated storage model + additional setup
  - ❌ Potential friction with the custom DB layer
- **OpenIddict / “full” OIDC server inside the app**
  - ✅ Standards-based and future-proof
  - ❌ More moving parts than needed for v1
- **Separate auth service / gateway**
  - ✅ Clear separation; can evolve independently
  - ❌ Additional service + ops complexity early
- **Simple JWT + refresh token (chosen)**
  - ✅ Minimal dependencies and fast to deliver
  - ✅ Fits self-host-first
  - ❌ Requires careful rotation/invalidation design

## Decision drivers
- Minimal dependencies for v1
- Compatibility with custom DB layer
- Preserve a clean upgrade path later

## Consequences
We must define and implement:
- access/refresh lifetimes
- refresh rotation strategy (and what “rotation” means for your system)
- invalidation rules (logout, password change, compromise)
- storage migration plan (in-memory → DB)

## Roadmap
Future evaluation:
- **OAuth2/OIDC**
- and/or **separate auth service** (gateway/IdP)

## Follow-ups
- Document lifetimes + rotation rules.
- Move refresh tokens to DB when ready.
- Ensure OpenAPI reflects auth schemes.

## References
- [Architecture](../architecture.md)
- [Authentication](../authentication.md) (if present)

