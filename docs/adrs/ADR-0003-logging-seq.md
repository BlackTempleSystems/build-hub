# ADR-0003 — Centralized structured logging with Seq

> **Status:** Accepted  
> **Date:** 2026-02-15  
> **Navigation:** [ADRs index](./README.md) · [Docs index](../README.md)

## Context
Early development needs fast feedback loops. With multiple services (and potential agents), we need searchable logs with correlation across components.

## Decision
Use **Seq** as the centralized structured log store for the API and services (and optionally agents).

## Options considered
- **Seq (chosen)**
  - ✅ Easy to self-host
  - ✅ Great structured log search and filtering
  - ✅ Good fit for small team speed
- **Elastic/ELK**
  - ✅ Powerful and flexible
  - ❌ Heavier operational overhead early
- **Cloud logging**
  - ✅ Managed
  - ❌ Not aligned with self-host-first and cost constraints

## Decision drivers
- Self-host friendly
- Minimal setup and maintenance
- Strong structured logging support

## Consequences
- Standardize log fields (recommended):
  - `traceId`, `requestId`, `userId` (when available), `service`, `env`, `version`
- Define retention policy later.

## Follow-ups
- Ensure correlation IDs are generated and propagated everywhere.
- Decide if agent logs also go to Seq.

## References
- [Observability](../observability.md)

