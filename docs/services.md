# Planned services

> **Last updated:** 2026-02-15  
> **Navigation:** [Docs index](./README.md)

This document captures the **service decomposition** we are aiming for.  
In early stages, some of these responsibilities can live **inside the API** (modularized), and later be split into separate services.

## Why split into services
- Isolate long-running work (dispatching, reconciliation, log ingestion) from the request/response API.
- Avoid “everything is in one process” bottlenecks.
- Make scaling and failure isolation possible.

## Proposed services (target end-state)

### 1) Scheduler / Dispatcher service
**Responsibility**
- Owns run queueing and agent selection (capability matching, priorities, concurrency limits).
- Moves runs through: `Queued → Dispatched → Running → Completed/Failed/Canceled`.
- Applies retry policies (infra-only retries) and backoff rules.
- “Preflight” check: refuse a run if no eligible agents exist.

**Interfaces**
- Reads/writes run/agent state in DB.
- Talks to **Agent Gateway** to assign work.
- Emits events/telemetry for dashboards.

**Start simple**
- A background worker hosted in the API process is OK for v1.

---

### 2) Agent Gateway (WebSocket hub)
**Responsibility**
- Maintains **agent connections** (hello/auth, heartbeats, reconnect/resume).
- Delivers work to agents (plan/commands) and receives:
  - step state events
  - run completion events
  - log chunks / structured markers
- Applies backpressure/ack rules to protect API/services from log storms.

**Interfaces**
- WebSocket endpoint for agents.
- Writes connection + heartbeat state to DB/Cache.
- Forwards updates to API/UI realtime stream.

---

### 3) Execution Orchestrator (optional separate)
**Responsibility**
- If your model becomes a DAG, parallel steps, approvals, etc., this service owns:
  - step dependency resolution
  - parallel execution coordination
  - “resume from failed step”
  - deterministic reruns

**Start simple**
- For sequential steps, the agent can execute locally and report state; orchestration can stay minimal.

---

### 4) Artifact service
**Responsibility**
- Accepts artifact uploads (files, reports) and exposes authenticated downloads.
- Applies retention and pruning policies.
- Supports “release artifact” promotion (later).

**Interfaces**
- API endpoints for upload/download metadata.
- Storage backend: filesystem / S3-compatible / other (**TBD**).

---

### 5) Notification / Webhook service
**Responsibility**
- Outgoing webhooks for run completed/failed.
- Later: email, Slack/Teams, issue tracker integration.
- Retry + dead-letter patterns for delivery failures.

---

### 6) Secrets / Config service (or Infisical integration boundary)
**Responsibility**
- Central place to fetch secrets “just in time” for a run/step.
- Enforces scoping (org/project/plan/run).
- Ensures secrets are **never** written to logs and can be rotated/revoked.

---

### 7) Reconciler / Maintenance jobs
**Responsibility**
- Detect stuck runs.
- Resolve “orphan runs” (agent disconnect mid-run).
- Quarantine agents after repeated infra failures.
- Clean retention: logs/artifacts/workspaces.

## Communication patterns (internal)
- **Today (simple):** direct method calls / shared DB + WebSocket hub
- **Soon:** internal HTTP/gRPC between API and services
- **Later (if needed):** message bus (NATS/RabbitMQ/Redis streams) for events and decoupling

## Related
- [Communication](./communication.md)
- [Architecture](./architecture.md)
- [Feature catalog](./feature-catalog.md)
