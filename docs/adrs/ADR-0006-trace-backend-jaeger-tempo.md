# ADR-0006 — Trace backend choice (Jaeger vs Grafana Tempo)

> **Status:** Proposed  
> **Date:** 2026-02-15  
> **Navigation:** [ADRs index](./README.md) · [Docs index](../README.md)

## Context
With OTel Collector + Grafana, we need a trace storage/query backend that is:
- self-host friendly
- operationally simple for a small team
- integrates well with Grafana
- fits expected scale

## Decision
Not finalized yet: evaluate **Jaeger** and/or **Grafana Tempo** as the trace backend.

## Options considered
- **Jaeger**
  - ✅ Mature and widely used
  - ✅ Strong UI and ecosystem
  - ❌ Operational choices depend on selected storage backend
- **Grafana Tempo**
  - ✅ Designed to pair with Grafana
  - ✅ Scales well; cost-effective patterns
  - ❌ Different query model/tradeoffs vs Jaeger
- **Run both temporarily**
  - ✅ Compare with real traces
  - ❌ Duplicate effort/cost

## Decision drivers
- Operational simplicity
- Good Grafana integration
- Sustainable storage cost

## Consequences
- OTel Collector exporter config depends on this choice.
- Sampling rules directly impact storage needs.

## Follow-ups
- Run a short POC with real traces from API/services.
- Pick one, update this ADR to **Accepted** and document the final reasons.

## References
- [Observability](../observability.md)

