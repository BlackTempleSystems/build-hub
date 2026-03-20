# ADR-0005 — SQL Server in Docker for development

> **Status:** Accepted  
> **Date:** 2026-02-15  
> **Navigation:** [ADRs index](./README.md) · [Docs index](../README.md)

## Context
We need a reproducible local dev environment with consistent SQL Server versions and minimal onboarding friction.

## Decision
Use **SQL Server in Docker** for local development (and possibly integration tests).

## Options considered
- **Local SQL Server install**
  - ✅ Works once installed
  - ❌ More machine-to-machine variance
  - ❌ Harder onboarding and version parity
- **SQL Server in Docker (chosen)**
  - ✅ Reproducible, easy onboarding
  - ✅ Works well with compose-based local environments
  - ❌ Requires correct volume setup to persist data

## Decision drivers
- Reproducibility
- Onboarding speed
- Version consistency

## Consequences
- Use Docker volumes so data persists across container restarts.
- Document connection strings and dev credentials pattern.

## Follow-ups
- Add/maintain compose setup and reset scripts.

## References
- [Development](../development.md)

