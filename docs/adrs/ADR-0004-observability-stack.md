# ADR-0004 — Observability baseline (OTel Collector + Prometheus + Grafana)

> **Status:** Accepted (baseline)  
> **Date:** 2026-02-15  
> **Navigation:** [ADRs index](./README.md) · [Docs index](../README.md)

## Context
We want more than logs, but we want incremental adoption and to avoid lock-in.
We also want a stack we can self-host.

## Decision
Adopt the baseline observability stack:
- **OpenTelemetry Collector (container)** as the ingestion/routing point (OTLP)
- **Prometheus** for metrics storage/scraping
- **Grafana** for dashboards (metrics + traces)
- Keep **Seq** for logs

## Options considered
- **OTel Collector + Prometheus + Grafana (chosen)**
  - ✅ Open standards (OTel)
  - ✅ Flexible routing/export
  - ✅ Great self-host story
  - ❌ More components to configure than “just logs”
- **Single integrated APM solution**
  - ✅ Simple setup
  - ❌ Often SaaS / lock-in; not aligned with self-host-first
- **Elastic APM**
  - ✅ Integrated platform
  - ❌ Heavier ops; not the current direction

## Decision drivers
- OpenTelemetry as the standard instrumentation path
- Modular stack: logs vs metrics vs visualization
- Ability to change trace backend without reinstrumenting

## Consequences
- API/services should export via OTLP.
- Need sampling strategy (especially for traces).
- Grafana becomes the “home” UI for telemetry.

## Follow-ups
- Add `otel-collector` container config and wiring.
- Define default sampling and core dashboards.

## References
- [Observability](../observability.md)
- [Architecture](../architecture.md)

