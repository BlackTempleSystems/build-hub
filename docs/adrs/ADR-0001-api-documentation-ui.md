# ADR-0001 — API documentation UI (Scalar + OpenAPI)

> **Status:** Accepted  
> **Date:** 2026-02-15  
> **Navigation:** [ADRs index](./README.md) · [Docs index](../README.md)

## Context
We want an API reference UI for developers/QA that is:
- OpenAPI-first (single source of truth)
- self-host friendly
- low-maintenance and easy to keep accurate

The project already relies on OpenAPI generation and uses Scalar as a viewer.

## Decision
Use **OpenAPI + Scalar** for API documentation, and **avoid exposing Swagger UI endpoints**.

## Options considered
- **Swagger UI (Swashbuckle)**
  - ✅ Familiar and common
  - ❌ Another UI surface to maintain/expose
  - ❌ Can accidentally keep `/swagger` around and even auto-open it in dev
- **Scalar (chosen)**
  - ✅ Clean UX, OpenAPI-first
  - ✅ Fits “self-host first”
  - ❌ Requires disciplined OpenAPI metadata (responses, schemas, auth)
- **Other OpenAPI viewers (Redoc/Stoplight/etc.)**
  - ✅ Mature alternatives
  - ❌ Not preferred; avoid adding more tools

## Decision drivers
- Reduce surface area (don’t expose extra endpoints)
- Keep docs accurate by keeping OpenAPI as the single source of truth
- Low operational overhead

## Consequences
- The quality of the docs depends on OpenAPI completeness:
  - response codes per endpoint
  - auth schemes
  - stable models and examples

## Follow-ups
- Ensure `/swagger` is removed/disabled and not auto-opened.
- Standardize response documentation (200/400/401/403/404/409/500 as applicable).

## References
- [Architecture](../architecture.md)

