# Communication and protocols

> **Last updated:** 2026-02-15  
> **Navigation:** [Docs index](./README.md)

This document explains **how components talk to each other**:
- UI ↔ API
- API ↔ services
- services ↔ agents (WebSockets)
- where logs/metrics/traces flow

> This is intended to be concrete enough to guide implementation, while still allowing iteration.

## Channels at a glance

### UI ↔ API
- **HTTPS REST** for CRUD and queries (pipelines, runs, agents, users, settings).
- **WebSocket (or SignalR)** for realtime updates:
  - run/step state changes
  - live log tailing
  - agent online/offline

### API ↔ services
- Initially in-process workers are acceptable.
- If/when split:
  - internal **HTTP/gRPC** for commands (dispatch, cancel, queries)
  - optional event stream later for state changes

### Services ↔ Agents
- **WebSocket** (agent gateway) is the preferred channel for:
  - registration/hello
  - heartbeats
  - plan/command dispatch
  - status/log streaming
  - cancel/interrupt

### Telemetry
- **Logs:** Serilog → Seq
- **Metrics & traces:** OTLP → OTel Collector → Prometheus + Grafana, plus trace backend (Jaeger/Tempo)

---

## Recommended identifiers (correlation)
These IDs should exist everywhere (requests, messages, logs, telemetry):
- `runId` — the build/run identity
- `planId` / `planVersion` — what was executed
- `agentId` — which agent executed it
- `correlationId` — UI/API correlation per action (optional)
- `traceId` — distributed tracing id (W3C trace context)

**Rule of thumb:** every WS message includes `runId`, and every log line includes `runId` + `agentId` when applicable.

---

## Sequence: trigger run → dispatch → completion

```mermaid
sequenceDiagram
  participant UI as Frontend
  participant API as API
  participant SCH as Scheduler/Dispatcher
  participant GW as Agent Gateway (WS)
  participant AG as Agent

  UI->>API: POST /runs (trigger)
  API->>API: Build ExecutionPlan (CommandBuilder)
  API->>SCH: Enqueue run (Queued)
  SCH->>GW: Assign run to agent (candidate selection)
  GW->>AG: Dispatch plan/commands (WS)
  AG-->>GW: Accepted + started
  AG-->>GW: Step events + log chunks
  GW-->>API: Persist updates + push realtime
  API-->>UI: Realtime updates (WS) + query endpoints
  AG-->>GW: Run completed (success/fail/canceled)
  GW-->>API: Final state + artifacts metadata
```

---

## Sequence: log streaming with ack/backpressure (sketch)

```mermaid
sequenceDiagram
  participant AG as Agent
  participant GW as Agent Gateway
  participant API as API

  AG->>GW: LogChunk(seq=101, stream=stdout, data=...)
  GW-->>AG: Ack(lastReceived=101)
  AG->>GW: LogChunk(seq=102, ...)
  Note over AG,GW: If GW slow → AG buffers up to limit
  GW-->>AG: Backpressure(on=true, maxRate=...)
  AG-->>GW: Compaction policy (drop/merge) if limit reached
  GW->>API: Store / forward logs
```

---

## Cancel semantics
- UI calls `POST /runs/{runId}/cancel`
- Scheduler marks run as canceling/canceled
- Gateway sends `Cancel(runId, reason)` to agent
- Agent:
  - attempts graceful stop
  - then force-kills after timeout
  - reports final state

---

## Versioning strategy
- WebSocket messages include:
  - `protocolVersion`
  - `agentVersion`
  - `planVersion`
- API/gateway can reject incompatible agents with an “update required” reason.

## Related
- [Planned services](./services.md)
- [Pipeline](./pipeline.md)
- [Feature catalog](./feature-catalog.md)
