# Feature catalog (considered)

> **Last updated:** 2026-02-15  
> **Navigation:** [Docs index](./README.md)

This file is the long-term **feature backlog** for Build Hub, expanded with:
- **Benefits** (why it’s worth doing)
- **How to add** (high-level implementation approach)
- **Where it lives** (UI/API/services/agents/DB)

It keeps the original categories and bullet lists from your notes (deduplicated).

## Table of contents
- [Product and domain model](#product-and-domain-model)
- [Build plans and pipelines](#build-plans-and-pipelines)
- [Build execution on agents](#build-execution-on-agents)
- [Source control and triggers](#source-control-and-triggers)
- [Scheduling, queues, and dispatch](#scheduling-queues-and-dispatch)
- [WebSocket protocol and messaging](#websocket-protocol-and-messaging)
- [Logs and log experience](#logs-and-log-experience)
- [Artifacts, tests, and reports](#artifacts-tests-and-reports)
- [Agents, fleets, and machine telemetry](#agents-fleets-and-machine-telemetry)
- [Security and compliance](#security-and-compliance)
- [Reliability and correctness](#reliability-and-correctness)
- [Observability and monitoring](#observability-and-monitoring)
- [Admin and operator tooling](#admin-and-operator-tooling)
- [UI and product polish](#ui-and-product-polish)
- [Integrations and public API](#integrations-and-public-api)
- [Data management and compliance](#data-management-and-compliance)
- [Developer experience and testing](#developer-experience-and-testing)
- [Analytics and insights](#analytics-and-insights)

---

## How to use this catalog
- Add a label per item: **MVP / Soon / Later**
- Track dependencies (e.g., log viewer depends on log chunk protocol)
- When promoting an item:
  1) update the data model
  2) design the API contract
  3) implement service/agent changes
  4) update UI
  5) add observability + tests

---


## Product and domain model
### Entities you’ll likely need
**Why:** Defines the core entities and boundaries so the rest of the system can scale (projects, plans, runs, agents, artifacts).

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- Organization / Workspace
- Project
- Repository (URL, provider type, credentials reference)
- Build Plan / Pipeline (definition, steps, requirements)
- Build Run (an execution instance)
- Step Run (per-step state/duration/output)
- Agent (machine identity)
- Agent Pool / Queue
- Artifacts (files + metadata)
- Secrets (scoped)
- Credentials (repo tokens, feed creds)
- Audit Events
- Notifications (subscriptions, delivery status)
**Benefits:**
- Stable foundation for everything else (queueing, audits, retention).
- Reproducible runs and reliable history.
- Easier to evolve features without data rewrites.

**How to add (high level):**
- Define entities and boundaries first (Org/Project/Repo/Plan/Run/Agent/Artifact/Secret/Audit).
- Add DB migrations and seed data for dev.
- Expose CRUD endpoints with paging/filtering; add audit events for changes.
- Keep run-time snapshots immutable (plan used, source ref used).

**Where it lives:** DB · API · UI


## Build plans and pipelines
### Plan/pipeline capabilities
**Why:** Defines how build logic is authored, versioned, and executed consistently across agents and over time.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- Steps: restore/build/test/publish/script
- Conditionals: run step only if branch/tag matches
- Variables: plan variables + runtime variables
- Matrix builds (optional): run on multiple SDK versions
- Templates: reusable step templates
- Parameterized runs: inputs when triggering
- Environments: dev/stage/prod variants
- Plan versioning + migration strategy
**Benefits:**
- Rerun the same build deterministically.
- Safer changes (plan versioning prevents breaking old runs).
- Reusable templates reduce duplication.

**How to add (high level):**
- Choose a plan definition format (UI builder + JSON/YAML under the hood).
- Version plans immutably; store a plan snapshot on every run.
- Add server-side validation + 'dry run' compilation to ExecutionPlan.
- Keep step types extensible (plugin identifiers + typed metadata).

**Where it lives:** UI · API · Scheduler · Agent

### Even if you’re early
**Why:** data loss is the fastest way to kill momentum.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- DB migrations strategy
- backup/restore test
**Benefits:**
- Rerun the same build deterministically.
- Safer changes (plan versioning prevents breaking old runs).
- Reusable templates reduce duplication.

**How to add (high level):**
- Choose a plan definition format (UI builder + JSON/YAML under the hood).
- Version plans immutably; store a plan snapshot on every run.
- Add server-side validation + 'dry run' compilation to ExecutionPlan.
- Keep step types extensible (plugin identifiers + typed metadata).

**Where it lives:** UI · API · Scheduler · Agent


## Build execution on agents
### Running builds safely and reliably
**Why:** Makes builds reproducible, controllable, and safe to run on remote machines (timeouts, cancellation, isolation, and resource protection).

**Goal:** Deterministic builds with strong control (cancel/timeout), isolation, and fast troubleshooting.

**Includes:**

- Workspace creation per run
**Benefits:**
- Reliable builds and fewer stuck processes.
- Safer host machines (cleanup, limits, isolation).
- Faster troubleshooting with structured step events.

**How to add (high level):**
- Implement workspace lifecycle per run (create → execute → cleanup).
- Implement process management: stdout/stderr capture, exit codes, kill tree on cancel.
- Apply policies: step timeout, run timeout, 'no output for N minutes'.
- Add tool detection + preflight validation before starting.

**Where it lives:** Agent · Agent Gateway · API

### Workspace cleanup policies
**Why:** Makes builds reproducible, controllable, and safe to run on remote machines (timeouts, cancellation, isolation, and resource protection).

**Goal:** Deterministic builds with strong control (cancel/timeout), isolation, and fast troubleshooting.

**Includes:**

- always clean
- keep on failure
- keep last N
**Benefits:**
- Reliable builds and fewer stuck processes.
- Safer host machines (cleanup, limits, isolation).
- Faster troubleshooting with structured step events.

**How to add (high level):**
- Implement workspace lifecycle per run (create → execute → cleanup).
- Implement process management: stdout/stderr capture, exit codes, kill tree on cancel.
- Apply policies: step timeout, run timeout, 'no output for N minutes'.
- Add tool detection + preflight validation before starting.

**Where it lives:** Agent · Agent Gateway · API

### Process management
**Why:** Makes builds reproducible, controllable, and safe to run on remote machines (timeouts, cancellation, isolation, and resource protection).

**Goal:** Deterministic builds with strong control (cancel/timeout), isolation, and fast troubleshooting.

**Includes:**

- capture stdout/stderr separately
- exit code
- kill tree on cancel
**Benefits:**
- Reliable builds and fewer stuck processes.
- Safer host machines (cleanup, limits, isolation).
- Faster troubleshooting with structured step events.

**How to add (high level):**
- Implement workspace lifecycle per run (create → execute → cleanup).
- Implement process management: stdout/stderr capture, exit codes, kill tree on cancel.
- Apply policies: step timeout, run timeout, 'no output for N minutes'.
- Add tool detection + preflight validation before starting.

**Where it lives:** Agent · Agent Gateway · API

### Timeouts
**Why:** Makes builds reproducible, controllable, and safe to run on remote machines (timeouts, cancellation, isolation, and resource protection).

**Goal:** Deterministic builds with strong control (cancel/timeout), isolation, and fast troubleshooting.

**Includes:**

- per step timeout
- whole run timeout
- “no output for N minutes”
**Benefits:**
- Reliable builds and fewer stuck processes.
- Safer host machines (cleanup, limits, isolation).
- Faster troubleshooting with structured step events.

**How to add (high level):**
- Implement workspace lifecycle per run (create → execute → cleanup).
- Implement process management: stdout/stderr capture, exit codes, kill tree on cancel.
- Apply policies: step timeout, run timeout, 'no output for N minutes'.
- Add tool detection + preflight validation before starting.

**Where it lives:** Agent · Agent Gateway · API

### Resource limits
**Why:** Makes builds reproducible, controllable, and safe to run on remote machines (timeouts, cancellation, isolation, and resource protection).

**Goal:** Deterministic builds with strong control (cancel/timeout), isolation, and fast troubleshooting.

**Includes:**

- CPU/memory caps (if possible)
- max disk usage
**Benefits:**
- Reliable builds and fewer stuck processes.
- Safer host machines (cleanup, limits, isolation).
- Faster troubleshooting with structured step events.

**How to add (high level):**
- Implement workspace lifecycle per run (create → execute → cleanup).
- Implement process management: stdout/stderr capture, exit codes, kill tree on cancel.
- Apply policies: step timeout, run timeout, 'no output for N minutes'.
- Add tool detection + preflight validation before starting.

**Where it lives:** Agent · Agent Gateway · API

### Tool detection
**Why:** Makes builds reproducible, controllable, and safe to run on remote machines (timeouts, cancellation, isolation, and resource protection).

**Goal:** Deterministic builds with strong control (cancel/timeout), isolation, and fast troubleshooting.

**Includes:**

- detect dotnet/msbuild versions
- validate required tools before starting
**Benefits:**
- Reliable builds and fewer stuck processes.
- Safer host machines (cleanup, limits, isolation).
- Faster troubleshooting with structured step events.

**How to add (high level):**
- Implement workspace lifecycle per run (create → execute → cleanup).
- Implement process management: stdout/stderr capture, exit codes, kill tree on cancel.
- Apply policies: step timeout, run timeout, 'no output for N minutes'.
- Add tool detection + preflight validation before starting.

**Where it lives:** Agent · Agent Gateway · API

### Environment injection
**Why:** Makes builds reproducible, controllable, and safe to run on remote machines (timeouts, cancellation, isolation, and resource protection).

**Goal:** Deterministic builds with strong control (cancel/timeout), isolation, and fast troubleshooting.

**Includes:**

- env vars
- working directory
- PATH modifications for toolchains
- “Dry run” validation mode (plan sanity check without executing)
**Benefits:**
- Reliable builds and fewer stuck processes.
- Safer host machines (cleanup, limits, isolation).
- Faster troubleshooting with structured step events.

**How to add (high level):**
- Implement workspace lifecycle per run (create → execute → cleanup).
- Implement process management: stdout/stderr capture, exit codes, kill tree on cancel.
- Apply policies: step timeout, run timeout, 'no output for N minutes'.
- Add tool detection + preflight validation before starting.

**Where it lives:** Agent · Agent Gateway · API

### Sandboxing/isolation
**Why:** Makes builds reproducible, controllable, and safe to run on remote machines (timeouts, cancellation, isolation, and resource protection).

**Goal:** Deterministic builds with strong control (cancel/timeout), isolation, and fast troubleshooting.

**Includes:**

- run under low-privilege user
- containerized steps (Docker)
- VM isolation (heavy)
**Benefits:**
- Reliable builds and fewer stuck processes.
- Safer host machines (cleanup, limits, isolation).
- Faster troubleshooting with structured step events.

**How to add (high level):**
- Implement workspace lifecycle per run (create → execute → cleanup).
- Implement process management: stdout/stderr capture, exit codes, kill tree on cancel.
- Apply policies: step timeout, run timeout, 'no output for N minutes'.
- Add tool detection + preflight validation before starting.

**Where it lives:** Agent · Agent Gateway · API

### Caching
**Why:** Makes builds reproducible, controllable, and safe to run on remote machines (timeouts, cancellation, isolation, and resource protection).

**Goal:** Deterministic builds with strong control (cancel/timeout), isolation, and fast troubleshooting.

**Includes:**

- NuGet cache persistence
- repo clone cache
- incremental build caches
**Benefits:**
- Reliable builds and fewer stuck processes.
- Safer host machines (cleanup, limits, isolation).
- Faster troubleshooting with structured step events.

**How to add (high level):**
- Implement workspace lifecycle per run (create → execute → cleanup).
- Implement process management: stdout/stderr capture, exit codes, kill tree on cancel.
- Apply policies: step timeout, run timeout, 'no output for N minutes'.
- Add tool detection + preflight validation before starting.

**Where it lives:** Agent · Agent Gateway · API

### Parallelism
**Why:** Makes builds reproducible, controllable, and safe to run on remote machines (timeouts, cancellation, isolation, and resource protection).

**Goal:** Deterministic builds with strong control (cancel/timeout), isolation, and fast troubleshooting.

**Includes:**

- parallel steps when safe
- parallel runs per agent (configurable)
**Benefits:**
- Reliable builds and fewer stuck processes.
- Safer host machines (cleanup, limits, isolation).
- Faster troubleshooting with structured step events.

**How to add (high level):**
- Implement workspace lifecycle per run (create → execute → cleanup).
- Implement process management: stdout/stderr capture, exit codes, kill tree on cancel.
- Apply policies: step timeout, run timeout, 'no output for N minutes'.
- Add tool detection + preflight validation before starting.

**Where it lives:** Agent · Agent Gateway · API

### Build trace capture
**Why:** Makes builds reproducible, controllable, and safe to run on remote machines (timeouts, cancellation, isolation, and resource protection).

**Goal:** Deterministic builds with strong control (cancel/timeout), isolation, and fast troubleshooting.

**Includes:**

- msbuild binary logs (.binlog)
- dotnet diagnostic logs
**Benefits:**
- Reliable builds and fewer stuck processes.
- Safer host machines (cleanup, limits, isolation).
- Faster troubleshooting with structured step events.

**How to add (high level):**
- Implement workspace lifecycle per run (create → execute → cleanup).
- Implement process management: stdout/stderr capture, exit codes, kill tree on cancel.
- Apply policies: step timeout, run timeout, 'no output for N minutes'.
- Add tool detection + preflight validation before starting.

**Where it lives:** Agent · Agent Gateway · API


## Source control and triggers
### Minimal but necessary
**Why:** Improves completeness and maintainability.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- Store: repo URL, default branch, commit SHA, branch, PR number (optional)
**Benefits:**
- Reproducible checkouts and reruns.
- Supports larger repos and common Git patterns.
- Enables future automation via webhooks.

**How to add (high level):**
- Store repo URL/default branch + commit SHA on each run.
- Implement checkout policies (shallow depth, submodules, LFS) as step metadata.
- Use secrets references for PAT/SSH keys (never embed raw secrets in plans).
- Later: add webhook triggers and PR status updates.

**Where it lives:** API · Scheduler · Agent

### Checkout strategies
**Why:** Improves completeness and maintainability.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- shallow clone depth
- submodules
- LFS
**Benefits:**
- Reproducible checkouts and reruns.
- Supports larger repos and common Git patterns.
- Enables future automation via webhooks.

**How to add (high level):**
- Store repo URL/default branch + commit SHA on each run.
- Implement checkout policies (shallow depth, submodules, LFS) as step metadata.
- Use secrets references for PAT/SSH keys (never embed raw secrets in plans).
- Later: add webhook triggers and PR status updates.

**Where it lives:** API · Scheduler · Agent

### Credentials
**Why:** Improves completeness and maintainability.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- PAT / SSH key references (stored as secrets)
**Benefits:**
- Reproducible checkouts and reruns.
- Supports larger repos and common Git patterns.
- Enables future automation via webhooks.

**How to add (high level):**
- Store repo URL/default branch + commit SHA on each run.
- Implement checkout policies (shallow depth, submodules, LFS) as step metadata.
- Use secrets references for PAT/SSH keys (never embed raw secrets in plans).
- Later: add webhook triggers and PR status updates.

**Where it lives:** API · Scheduler · Agent

### Commit metadata display
**Why:** Improves completeness and maintainability.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- author, message, timestamp
**Benefits:**
- Reproducible checkouts and reruns.
- Supports larger repos and common Git patterns.
- Enables future automation via webhooks.

**How to add (high level):**
- Store repo URL/default branch + commit SHA on each run.
- Implement checkout policies (shallow depth, submodules, LFS) as step metadata.
- Use secrets references for PAT/SSH keys (never embed raw secrets in plans).
- Later: add webhook triggers and PR status updates.

**Where it lives:** API · Scheduler · Agent

### Nice-to-have
**Why:** Improves completeness and maintainability.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- PR status checks (pending/success/fail)
- Build “required” checks gating merges
- Build on tag/release creation
- Auto-cancel previous runs on same branch when new commit arrives
**Benefits:**
- Reproducible checkouts and reruns.
- Supports larger repos and common Git patterns.
- Enables future automation via webhooks.

**How to add (high level):**
- Store repo URL/default branch + commit SHA on each run.
- Implement checkout policies (shallow depth, submodules, LFS) as step metadata.
- Use secrets references for PAT/SSH keys (never embed raw secrets in plans).
- Later: add webhook triggers and PR status updates.

**Where it lives:** API · Scheduler · Agent

### General
**Why:** reproducibility + rerun is impossible without this.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- Store “source reference” on runs: repo URL, branch, commit SHA
- Agent checkout strategy: shallow clone, fetch depth, submodules, LFS if needed
**Benefits:**
- Reproducible checkouts and reruns.
- Supports larger repos and common Git patterns.
- Enables future automation via webhooks.

**How to add (high level):**
- Store repo URL/default branch + commit SHA on each run.
- Implement checkout policies (shallow depth, submodules, LFS) as step metadata.
- Use secrets references for PAT/SSH keys (never embed raw secrets in plans).
- Later: add webhook triggers and PR status updates.

**Where it lives:** API · Scheduler · Agent


## Scheduling, queues, and dispatch
### Agent selection logic
**Why:** Improves throughput and fairness by matching builds to the right agents, managing queues, and handling overload safely.

**Goal:** Predictable build starts, minimal mismatches, and stable throughput under load.

**Includes:**

- Capability matching (OS, SDK, tool versions, tags)
- Pool/queue routing (Windows pool, Linux pool, etc.)
**Benefits:**
- Higher throughput and predictable start times.
- Fewer failures due to wrong agent/tooling.
- Better operational control under load.

**How to add (high level):**
- Agent advertises capabilities; scheduler matches requirements.
- Implement queues with priorities and concurrency limits.
- Add retry policies (infra-only) + backoff.
- Add drain/quarantine modes and preflight checks.

**Where it lives:** Scheduler · DB · Agent Gateway · API

### Concurrency
**Why:** Improves throughput and fairness by matching builds to the right agents, managing queues, and handling overload safely.

**Goal:** Predictable build starts, minimal mismatches, and stable throughput under load.

**Includes:**

- per agent limit
- per project limit
- global limit
**Benefits:**
- Higher throughput and predictable start times.
- Fewer failures due to wrong agent/tooling.
- Better operational control under load.

**How to add (high level):**
- Agent advertises capabilities; scheduler matches requirements.
- Implement queues with priorities and concurrency limits.
- Add retry policies (infra-only) + backoff.
- Add drain/quarantine modes and preflight checks.

**Where it lives:** Scheduler · DB · Agent Gateway · API

### Priorities
**Why:** Improves throughput and fairness by matching builds to the right agents, managing queues, and handling overload safely.

**Goal:** Predictable build starts, minimal mismatches, and stable throughput under load.

**Includes:**

- hotfix > normal
- per project weighting
**Benefits:**
- Higher throughput and predictable start times.
- Fewer failures due to wrong agent/tooling.
- Better operational control under load.

**How to add (high level):**
- Agent advertises capabilities; scheduler matches requirements.
- Implement queues with priorities and concurrency limits.
- Add retry policies (infra-only) + backoff.
- Add drain/quarantine modes and preflight checks.

**Where it lives:** Scheduler · DB · Agent Gateway · API

### Affinity
**Why:** Improves throughput and fairness by matching builds to the right agents, managing queues, and handling overload safely.

**Goal:** Predictable build starts, minimal mismatches, and stable throughput under load.

**Includes:**

- prefer same agent for same repo (cache)
**Benefits:**
- Higher throughput and predictable start times.
- Fewer failures due to wrong agent/tooling.
- Better operational control under load.

**How to add (high level):**
- Agent advertises capabilities; scheduler matches requirements.
- Implement queues with priorities and concurrency limits.
- Add retry policies (infra-only) + backoff.
- Add drain/quarantine modes and preflight checks.

**Where it lives:** Scheduler · DB · Agent Gateway · API

### Anti-affinity
**Why:** Improves throughput and fairness by matching builds to the right agents, managing queues, and handling overload safely.

**Goal:** Predictable build starts, minimal mismatches, and stable throughput under load.

**Includes:**

- don’t run two heavy builds on same machine
**Benefits:**
- Higher throughput and predictable start times.
- Fewer failures due to wrong agent/tooling.
- Better operational control under load.

**How to add (high level):**
- Agent advertises capabilities; scheduler matches requirements.
- Implement queues with priorities and concurrency limits.
- Add retry policies (infra-only) + backoff.
- Add drain/quarantine modes and preflight checks.

**Where it lives:** Scheduler · DB · Agent Gateway · API

### Preflight check
**Why:** Improves throughput and fairness by matching builds to the right agents, managing queues, and handling overload safely.

**Goal:** Predictable build starts, minimal mismatches, and stable throughput under load.

**Includes:**

- refuse run if no eligible agents
**Benefits:**
- Higher throughput and predictable start times.
- Fewer failures due to wrong agent/tooling.
- Better operational control under load.

**How to add (high level):**
- Agent advertises capabilities; scheduler matches requirements.
- Implement queues with priorities and concurrency limits.
- Add retry policies (infra-only) + backoff.
- Add drain/quarantine modes and preflight checks.

**Where it lives:** Scheduler · DB · Agent Gateway · API

### Queue management
**Why:** Improves throughput and fairness by matching builds to the right agents, managing queues, and handling overload safely.

**Goal:** Predictable build starts, minimal mismatches, and stable throughput under load.

**Includes:**

- Global queue vs per-pool queue
- ETA estimate (“3 mins wait”)
- Fairness (avoid starvation)
- Manual reorder (admin)
- Scheduled runs (cron-like)
- “Quiet hours” / maintenance windows
**Benefits:**
- Higher throughput and predictable start times.
- Fewer failures due to wrong agent/tooling.
- Better operational control under load.

**How to add (high level):**
- Agent advertises capabilities; scheduler matches requirements.
- Implement queues with priorities and concurrency limits.
- Add retry policies (infra-only) + backoff.
- Add drain/quarantine modes and preflight checks.

**Where it lives:** Scheduler · DB · Agent Gateway · API

### Automatic retry policies
**Why:** Improves throughput and fairness by matching builds to the right agents, managing queues, and handling overload safely.

**Goal:** Predictable build starts, minimal mismatches, and stable throughput under load.

**Includes:**

- retry only on infrastructure errors
- backoff + max attempts
**Benefits:**
- Higher throughput and predictable start times.
- Fewer failures due to wrong agent/tooling.
- Better operational control under load.

**How to add (high level):**
- Agent advertises capabilities; scheduler matches requirements.
- Implement queues with priorities and concurrency limits.
- Add retry policies (infra-only) + backoff.
- Add drain/quarantine modes and preflight checks.

**Where it lives:** Scheduler · DB · Agent Gateway · API

### Quarantine bad agents
**Why:** Improves throughput and fairness by matching builds to the right agents, managing queues, and handling overload safely.

**Goal:** Predictable build starts, minimal mismatches, and stable throughput under load.

**Includes:**

- N failures in a row → remove from rotation
**Benefits:**
- Higher throughput and predictable start times.
- Fewer failures due to wrong agent/tooling.
- Better operational control under load.

**How to add (high level):**
- Agent advertises capabilities; scheduler matches requirements.
- Implement queues with priorities and concurrency limits.
- Add retry policies (infra-only) + backoff.
- Add drain/quarantine modes and preflight checks.

**Where it lives:** Scheduler · DB · Agent Gateway · API

### Drain mode
**Why:** Improves throughput and fairness by matching builds to the right agents, managing queues, and handling overload safely.

**Goal:** Predictable build starts, minimal mismatches, and stable throughput under load.

**Includes:**

- stop accepting new runs, finish current
**Benefits:**
- Higher throughput and predictable start times.
- Fewer failures due to wrong agent/tooling.
- Better operational control under load.

**How to add (high level):**
- Agent advertises capabilities; scheduler matches requirements.
- Implement queues with priorities and concurrency limits.
- Add retry policies (infra-only) + backoff.
- Add drain/quarantine modes and preflight checks.

**Where it lives:** Scheduler · DB · Agent Gateway · API

### Agents should advertise capabilities
**Why:** Improves throughput and fairness by matching builds to the right agents, managing queues, and handling overload safely.

**Goal:** Predictable build starts, minimal mismatches, and stable throughput under load.

**Includes:**

- installed SDKs (.NET versions), msbuild version
- OS, CPU/RAM, disk free
- tags: windows, linux, vs2022, net8, gpu, etc.
**Benefits:**
- Higher throughput and predictable start times.
- Fewer failures due to wrong agent/tooling.
- Better operational control under load.

**How to add (high level):**
- Agent advertises capabilities; scheduler matches requirements.
- Implement queues with priorities and concurrency limits.
- Add retry policies (infra-only) + backoff.
- Add drain/quarantine modes and preflight checks.

**Where it lives:** Scheduler · DB · Agent Gateway · API

### Scheduler picks an agent based on
**Why:** otherwise plans fail randomly on “wrong machine”.

**Goal:** Predictable build starts, minimal mismatches, and stable throughput under load.

**Includes:**

- required capabilities
- load/concurrency limits
- queue priority
**Benefits:**
- Higher throughput and predictable start times.
- Fewer failures due to wrong agent/tooling.
- Better operational control under load.

**How to add (high level):**
- Agent advertises capabilities; scheduler matches requirements.
- Implement queues with priorities and concurrency limits.
- Add retry policies (infra-only) + backoff.
- Add drain/quarantine modes and preflight checks.

**Where it lives:** Scheduler · DB · Agent Gateway · API

### Smart scheduling
**Why:** Improves throughput and fairness by matching builds to the right agents, managing queues, and handling overload safely.

**Goal:** Predictable build starts, minimal mismatches, and stable throughput under load.

**Includes:**

- Warm agent pools: keep N idle agents ready for fast start
- Affinity rules: prefer same agent for same repo (cache benefit)
- Auto-quarantine agents: if agent fails N runs in a row → stop assigning, require manual “unquarantine”
- Drain mode: take agent out of rotation but let current builds finish
**Benefits:**
- Higher throughput and predictable start times.
- Fewer failures due to wrong agent/tooling.
- Better operational control under load.

**How to add (high level):**
- Agent advertises capabilities; scheduler matches requirements.
- Implement queues with priorities and concurrency limits.
- Add retry policies (infra-only) + backoff.
- Add drain/quarantine modes and preflight checks.

**Where it lives:** Scheduler · DB · Agent Gateway · API

### Capacity planning
**Why:** Improves throughput and fairness by matching builds to the right agents, managing queues, and handling overload safely.

**Goal:** Predictable build starts, minimal mismatches, and stable throughput under load.

**Includes:**

- Estimated wait time per queue
- Per-project quotas (avoid one project hogging all agents)
- Burst mode (if you later add auto-scaling)
**Benefits:**
- Higher throughput and predictable start times.
- Fewer failures due to wrong agent/tooling.
- Better operational control under load.

**How to add (high level):**
- Agent advertises capabilities; scheduler matches requirements.
- Implement queues with priorities and concurrency limits.
- Add retry policies (infra-only) + backoff.
- Add drain/quarantine modes and preflight checks.

**Where it lives:** Scheduler · DB · Agent Gateway · API


## WebSocket protocol and messaging
### Agent “hello” message
**Why:** Ensures reliable, secure, and resumable communication between the API and agents (especially under disconnects, retries, and high log volume).

**Goal:** A versioned protocol with ack/resume semantics, clear state transitions, and safe handling of reconnects.

**Includes:**

- agentId
- agentVersion
- supported plan versions
- capabilities
**Benefits:**
- Survives disconnects and restarts cleanly.
- Protects system from log storms.
- Enables realtime UI experiences.

**How to add (high level):**
- Define versioned message schema (protocolVersion) and compatibility rules.
- Add hello/auth + heartbeat + reconnect/resume.
- For logs: sequence numbers + ack + backpressure.
- Contract-test messages (schema validation) to prevent silent breakage.

**Where it lives:** Agent Gateway · Agent · UI (optional realtime)

### Auth
**Why:** Ensures reliable, secure, and resumable communication between the API and agents (especially under disconnects, retries, and high log volume).

**Goal:** A versioned protocol with ack/resume semantics, clear state transitions, and safe handling of reconnects.

**Includes:**

- service token (bearer)
- rotation/revoke
- optional mTLS later
**Benefits:**
- Survives disconnects and restarts cleanly.
- Protects system from log storms.
- Enables realtime UI experiences.

**How to add (high level):**
- Define versioned message schema (protocolVersion) and compatibility rules.
- Add hello/auth + heartbeat + reconnect/resume.
- For logs: sequence numbers + ack + backpressure.
- Contract-test messages (schema validation) to prevent silent breakage.

**Where it lives:** Agent Gateway · Agent · UI (optional realtime)

### Heartbeats
**Why:** Ensures reliable, secure, and resumable communication between the API and agents (especially under disconnects, retries, and high log volume).

**Goal:** A versioned protocol with ack/resume semantics, clear state transitions, and safe handling of reconnects.

**Includes:**

- ping/pong
- last-seen timestamp
**Benefits:**
- Survives disconnects and restarts cleanly.
- Protects system from log storms.
- Enables realtime UI experiences.

**How to add (high level):**
- Define versioned message schema (protocolVersion) and compatibility rules.
- Add hello/auth + heartbeat + reconnect/resume.
- For logs: sequence numbers + ack + backpressure.
- Contract-test messages (schema validation) to prevent silent breakage.

**Where it lives:** Agent Gateway · Agent · UI (optional realtime)

### Dispatch build plan message includes
**Why:** Ensures reliable, secure, and resumable communication between the API and agents (especially under disconnects, retries, and high log volume).

**Goal:** A versioned protocol with ack/resume semantics, clear state transitions, and safe handling of reconnects.

**Includes:**

- runId
- plan definition snapshot
- required secrets references (not raw)
**Benefits:**
- Survives disconnects and restarts cleanly.
- Protects system from log storms.
- Enables realtime UI experiences.

**How to add (high level):**
- Define versioned message schema (protocolVersion) and compatibility rules.
- Add hello/auth + heartbeat + reconnect/resume.
- For logs: sequence numbers + ack + backpressure.
- Contract-test messages (schema validation) to prevent silent breakage.

**Where it lives:** Agent Gateway · Agent · UI (optional realtime)

### Agent responds
**Why:** Ensures reliable, secure, and resumable communication between the API and agents (especially under disconnects, retries, and high log volume).

**Goal:** A versioned protocol with ack/resume semantics, clear state transitions, and safe handling of reconnects.

**Includes:**

- accepted/rejected (reason)
- started (timestamp)
**Benefits:**
- Survives disconnects and restarts cleanly.
- Protects system from log storms.
- Enables realtime UI experiences.

**How to add (high level):**
- Define versioned message schema (protocolVersion) and compatibility rules.
- Add hello/auth + heartbeat + reconnect/resume.
- For logs: sequence numbers + ack + backpressure.
- Contract-test messages (schema validation) to prevent silent breakage.

**Where it lives:** Agent Gateway · Agent · UI (optional realtime)

### Log chunks
**Why:** Ensures reliable, secure, and resumable communication between the API and agents (especially under disconnects, retries, and high log volume).

**Goal:** A versioned protocol with ack/resume semantics, clear state transitions, and safe handling of reconnects.

**Includes:**

- sequence number
- stream (stdout/stderr)
- chunk payload (optionally compressed)
**Benefits:**
- Survives disconnects and restarts cleanly.
- Protects system from log storms.
- Enables realtime UI experiences.

**How to add (high level):**
- Define versioned message schema (protocolVersion) and compatibility rules.
- Add hello/auth + heartbeat + reconnect/resume.
- For logs: sequence numbers + ack + backpressure.
- Contract-test messages (schema validation) to prevent silent breakage.

**Where it lives:** Agent Gateway · Agent · UI (optional realtime)

### Backpressure
**Why:** Ensures reliable, secure, and resumable communication between the API and agents (especially under disconnects, retries, and high log volume).

**Goal:** A versioned protocol with ack/resume semantics, clear state transitions, and safe handling of reconnects.

**Includes:**

- API acks last received sequence
- agent buffers up to limit; then drop/compact rules
**Benefits:**
- Survives disconnects and restarts cleanly.
- Protects system from log storms.
- Enables realtime UI experiences.

**How to add (high level):**
- Define versioned message schema (protocolVersion) and compatibility rules.
- Add hello/auth + heartbeat + reconnect/resume.
- For logs: sequence numbers + ack + backpressure.
- Contract-test messages (schema validation) to prevent silent breakage.

**Where it lives:** Agent Gateway · Agent · UI (optional realtime)

### Log limits
**Why:** Ensures reliable, secure, and resumable communication between the API and agents (especially under disconnects, retries, and high log volume).

**Goal:** A versioned protocol with ack/resume semantics, clear state transitions, and safe handling of reconnects.

**Includes:**

- max log size per run
- truncation policy (first X + last Y)
**Benefits:**
- Survives disconnects and restarts cleanly.
- Protects system from log storms.
- Enables realtime UI experiences.

**How to add (high level):**
- Define versioned message schema (protocolVersion) and compatibility rules.
- Add hello/auth + heartbeat + reconnect/resume.
- For logs: sequence numbers + ack + backpressure.
- Contract-test messages (schema validation) to prevent silent breakage.

**Where it lives:** Agent Gateway · Agent · UI (optional realtime)

### Resume
**Why:** Ensures reliable, secure, and resumable communication between the API and agents (especially under disconnects, retries, and high log volume).

**Goal:** A versioned protocol with ack/resume semantics, clear state transitions, and safe handling of reconnects.

**Includes:**

- on reconnect, agent says “last sent seq”
- API says “last received seq” → resend missing
**Benefits:**
- Survives disconnects and restarts cleanly.
- Protects system from log storms.
- Enables realtime UI experiences.

**How to add (high level):**
- Define versioned message schema (protocolVersion) and compatibility rules.
- Add hello/auth + heartbeat + reconnect/resume.
- For logs: sequence numbers + ack + backpressure.
- Contract-test messages (schema validation) to prevent silent breakage.

**Where it lives:** Agent Gateway · Agent · UI (optional realtime)

### Step status events
**Why:** Ensures reliable, secure, and resumable communication between the API and agents (especially under disconnects, retries, and high log volume).

**Goal:** A versioned protocol with ack/resume semantics, clear state transitions, and safe handling of reconnects.

**Includes:**

- stepId, state, timestamps, duration
**Benefits:**
- Survives disconnects and restarts cleanly.
- Protects system from log storms.
- Enables realtime UI experiences.

**How to add (high level):**
- Define versioned message schema (protocolVersion) and compatibility rules.
- Add hello/auth + heartbeat + reconnect/resume.
- For logs: sequence numbers + ack + backpressure.
- Contract-test messages (schema validation) to prevent silent breakage.

**Where it lives:** Agent Gateway · Agent · UI (optional realtime)

### Run completed event
**Why:** Ensures reliable, secure, and resumable communication between the API and agents (especially under disconnects, retries, and high log volume).

**Goal:** A versioned protocol with ack/resume semantics, clear state transitions, and safe handling of reconnects.

**Includes:**

- success/fail/canceled
- exit codes
- failure category + message
**Benefits:**
- Survives disconnects and restarts cleanly.
- Protects system from log storms.
- Enables realtime UI experiences.

**How to add (high level):**
- Define versioned message schema (protocolVersion) and compatibility rules.
- Add hello/auth + heartbeat + reconnect/resume.
- For logs: sequence numbers + ack + backpressure.
- Contract-test messages (schema validation) to prevent silent breakage.

**Where it lives:** Agent Gateway · Agent · UI (optional realtime)

### Cancel command
**Why:** Ensures reliable, secure, and resumable communication between the API and agents (especially under disconnects, retries, and high log volume).

**Goal:** A versioned protocol with ack/resume semantics, clear state transitions, and safe handling of reconnects.

**Includes:**

- runId, reason
- agent ack cancel received
**Benefits:**
- Survives disconnects and restarts cleanly.
- Protects system from log storms.
- Enables realtime UI experiences.

**How to add (high level):**
- Define versioned message schema (protocolVersion) and compatibility rules.
- Add hello/auth + heartbeat + reconnect/resume.
- For logs: sequence numbers + ack + backpressure.
- Contract-test messages (schema validation) to prevent silent breakage.

**Where it lives:** Agent Gateway · Agent · UI (optional realtime)

### Reconciliation on restart/disconnect
**Why:** Ensures reliable, secure, and resumable communication between the API and agents (especially under disconnects, retries, and high log volume).

**Goal:** A versioned protocol with ack/resume semantics, clear state transitions, and safe handling of reconnects.

**Includes:**

- API restart: agents re-register and report active runs
- Agent restart mid-run: API marks run as lost unless agent resumes
- “Orphan runs” cleanup job
**Benefits:**
- Survives disconnects and restarts cleanly.
- Protects system from log storms.
- Enables realtime UI experiences.

**How to add (high level):**
- Define versioned message schema (protocolVersion) and compatibility rules.
- Add hello/auth + heartbeat + reconnect/resume.
- For logs: sequence numbers + ack + backpressure.
- Contract-test messages (schema validation) to prevent silent breakage.

**Where it lives:** Agent Gateway · Agent · UI (optional realtime)

### Schema evolution
**Why:** Ensures reliable, secure, and resumable communication between the API and agents (especially under disconnects, retries, and high log volume).

**Goal:** A versioned protocol with ack/resume semantics, clear state transitions, and safe handling of reconnects.

**Includes:**

- Message version field
**Benefits:**
- Survives disconnects and restarts cleanly.
- Protects system from log storms.
- Enables realtime UI experiences.

**How to add (high level):**
- Define versioned message schema (protocolVersion) and compatibility rules.
- Add hello/auth + heartbeat + reconnect/resume.
- For logs: sequence numbers + ack + backpressure.
- Contract-test messages (schema validation) to prevent silent breakage.

**Where it lives:** Agent Gateway · Agent · UI (optional realtime)

### Compatibility matrix
**Why:** Ensures reliable, secure, and resumable communication between the API and agents (especially under disconnects, retries, and high log volume).

**Goal:** A versioned protocol with ack/resume semantics, clear state transitions, and safe handling of reconnects.

**Includes:**

- API supports agentVersion range
- agent supports planVersion range
- Deprecation strategy
**Benefits:**
- Survives disconnects and restarts cleanly.
- Protects system from log storms.
- Enables realtime UI experiences.

**How to add (high level):**
- Define versioned message schema (protocolVersion) and compatibility rules.
- Add hello/auth + heartbeat + reconnect/resume.
- For logs: sequence numbers + ack + backpressure.
- Contract-test messages (schema validation) to prevent silent breakage.

**Where it lives:** Agent Gateway · Agent · UI (optional realtime)

### Not just “chunk logs”, but enforce
**Why:** log storms can crash your API or agent.

**Goal:** A versioned protocol with ack/resume semantics, clear state transitions, and safe handling of reconnects.

**Includes:**

- max buffered log bytes per run
- drop/compact strategy under pressure
- max status update rate (e.g., CPU every 5s, not 20x/sec)
**Benefits:**
- Survives disconnects and restarts cleanly.
- Protects system from log storms.
- Enables realtime UI experiences.

**How to add (high level):**
- Define versioned message schema (protocolVersion) and compatibility rules.
- Add hello/auth + heartbeat + reconnect/resume.
- For logs: sequence numbers + ack + backpressure.
- Contract-test messages (schema validation) to prevent silent breakage.

**Where it lives:** Agent Gateway · Agent · UI (optional realtime)


## Logs and log experience
### Stream logs to
**Why:** Turns raw build output into a usable debugging experience while keeping storage costs and noise under control.

**Goal:** Fast live streaming + searchable retained logs without overwhelming API/agents.

**Includes:**

- DB (not ideal for huge logs) OR
- file/object storage (recommended later)
**Benefits:**
- Fast debugging (core product UX).
- Lower support time and faster triage.
- Controlled storage growth.

**How to add (high level):**
- Stream logs as chunks (stdout/stderr) with timestamps and stepId.
- Store logs efficiently (Seq now; object/file storage later).
- Build UI viewer: search, jump to error, download, filter per step.
- Add retention and truncation policies to control storage.

**Where it lives:** Agent · Gateway · Storage · UI

### Index key fields
**Why:** Turns raw build output into a usable debugging experience while keeping storage costs and noise under control.

**Goal:** Fast live streaming + searchable retained logs without overwhelming API/agents.

**Includes:**

- runId, stepId, timestamp, severity type
**Benefits:**
- Fast debugging (core product UX).
- Lower support time and faster triage.
- Controlled storage growth.

**How to add (high level):**
- Stream logs as chunks (stdout/stderr) with timestamps and stepId.
- Store logs efficiently (Seq now; object/file storage later).
- Build UI viewer: search, jump to error, download, filter per step.
- Add retention and truncation policies to control storage.

**Where it lives:** Agent · Gateway · Storage · UI

### Retention policy
**Why:** Turns raw build output into a usable debugging experience while keeping storage costs and noise under control.

**Goal:** Fast live streaming + searchable retained logs without overwhelming API/agents.

**Includes:**

- by age
- by tags (“keep releases longer”)
**Benefits:**
- Fast debugging (core product UX).
- Lower support time and faster triage.
- Controlled storage growth.

**How to add (high level):**
- Stream logs as chunks (stdout/stderr) with timestamps and stepId.
- Store logs efficiently (Seq now; object/file storage later).
- Build UI viewer: search, jump to error, download, filter per step.
- Add retention and truncation policies to control storage.

**Where it lives:** Agent · Gateway · Storage · UI

### UI log viewer must-haves
**Why:** Turns raw build output into a usable debugging experience while keeping storage costs and noise under control.

**Goal:** Fast live streaming + searchable retained logs without overwhelming API/agents.

**Includes:**

- Live stream
- Pause/resume auto-scroll
- Search in logs
- Highlight errors/warnings
- Jump to first error
- Download logs
- Collapse repeated lines
- Filter by step, severity, stdout/stderr
**Benefits:**
- Fast debugging (core product UX).
- Lower support time and faster triage.
- Controlled storage growth.

**How to add (high level):**
- Stream logs as chunks (stdout/stderr) with timestamps and stepId.
- Store logs efficiently (Seq now; object/file storage later).
- Build UI viewer: search, jump to error, download, filter per step.
- Add retention and truncation policies to control storage.

**Where it lives:** Agent · Gateway · Storage · UI

### Nice-to-have log intelligence
**Why:** Turns raw build output into a usable debugging experience while keeping storage costs and noise under control.

**Goal:** Fast live streaming + searchable retained logs without overwhelming API/agents.

**Includes:**

- Parse MSBuild errors/warnings into structured items
- Detect flaky patterns
- “Similar failures” grouping
- Log bookmarks / annotations
**Benefits:**
- Fast debugging (core product UX).
- Lower support time and faster triage.
- Controlled storage growth.

**How to add (high level):**
- Stream logs as chunks (stdout/stderr) with timestamps and stepId.
- Store logs efficiently (Seq now; object/file storage later).
- Build UI viewer: search, jump to error, download, filter per step.
- Add retention and truncation policies to control storage.

**Where it lives:** Agent · Gateway · Storage · UI


## Artifacts, tests, and reports
### Artifacts
**Why:** Preserves build outputs and test evidence so runs are actionable, auditable, and easy to consume.

**Goal:** Every run produces retrievable outputs and structured test/quality evidence.

**Includes:**

- Upload artifacts (zip, binaries, packages)
- Artifact list per run (name, size, checksum)
- Download links with authorization
- Retention + pruning
- Artifact promotion (e.g., mark as “release artifact”)
**Benefits:**
- Runs become actionable (download outputs, test evidence).
- Supports quality gates and trends later.
- Makes builds auditable and shareable.

**How to add (high level):**
- Upload artifacts with metadata (name, size, checksum, content-type).
- Parse TRX/JUnit and store summaries; keep raw reports as artifacts.
- Add retention policies and 'release' promotion (later).
- Provide secure downloads (authorized, expiring links later).

**Where it lives:** Agent · Artifact service · UI · DB

### Parse and display
**Why:** Preserves build outputs and test evidence so runs are actionable, auditable, and easy to consume.

**Goal:** Every run produces retrievable outputs and structured test/quality evidence.

**Includes:**

- TRX / JUnit
- coverage summary
- Flaky test tracking
- Trend charts (pass rate, duration)
- Store raw report files as artifacts
**Benefits:**
- Runs become actionable (download outputs, test evidence).
- Supports quality gates and trends later.
- Makes builds auditable and shareable.

**How to add (high level):**
- Upload artifacts with metadata (name, size, checksum, content-type).
- Parse TRX/JUnit and store summaries; keep raw reports as artifacts.
- Add retention policies and 'release' promotion (later).
- Provide secure downloads (authorized, expiring links later).

**Where it lives:** Agent · Artifact service · UI · DB

### Build metadata
**Why:** Preserves build outputs and test evidence so runs are actionable, auditable, and easy to consume.

**Goal:** Every run produces retrievable outputs and structured test/quality evidence.

**Includes:**

- build output summary
- commit used
- dependency versions (optional)
**Benefits:**
- Runs become actionable (download outputs, test evidence).
- Supports quality gates and trends later.
- Makes builds auditable and shareable.

**How to add (high level):**
- Upload artifacts with metadata (name, size, checksum, content-type).
- Parse TRX/JUnit and store summaries; keep raw reports as artifacts.
- Add retention policies and 'release' promotion (later).
- Provide secure downloads (authorized, expiring links later).

**Where it lives:** Agent · Artifact service · UI · DB


## Agents, fleets, and machine telemetry
### Agent health data
**Why:** Keeps agents healthy and schedulable by tracking machine state, capabilities, and fleet-level operations.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- CPU, RAM, disk free
- network connectivity
- process count (optional)
- queue/run count
- toolchain list (dotnet sdks installed)
- last update time
**Benefits:**
- Prevents silent agent decay and flaky infrastructure.
- Better scheduling decisions and stability.
- Faster operations and recovery.

**How to add (high level):**
- Heartbeat includes CPU/RAM/disk/toolchains + current workload.
- Scheduler respects health signals (disk low → stop scheduling).
- Add drain/quarantine/unquarantine workflows.
- Expose an Agent page and diagnostics bundle.

**Where it lives:** Agent · Gateway · Scheduler · UI

### Reliability actions
**Why:** Keeps agents healthy and schedulable by tracking machine state, capabilities, and fleet-level operations.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- “agent out of disk” → stop scheduling
- “agent too hot” (CPU high) → reduce concurrency
- auto-restart agent service (local watchdog)
- “self-test” button (agent verifies toolchain + write permission)
**Benefits:**
- Prevents silent agent decay and flaky infrastructure.
- Better scheduling decisions and stability.
- Faster operations and recovery.

**How to add (high level):**
- Heartbeat includes CPU/RAM/disk/toolchains + current workload.
- Scheduler respects health signals (disk low → stop scheduling).
- Add drain/quarantine/unquarantine workflows.
- Expose an Agent page and diagnostics bundle.

**Where it lives:** Agent · Gateway · Scheduler · UI

### Even if you start small
**Why:** scaling from 2 agents to 20 is painful without pools.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- pools/queues (Windows pool, Linux pool)
- tags + routing rules
**Benefits:**
- Prevents silent agent decay and flaky infrastructure.
- Better scheduling decisions and stability.
- Faster operations and recovery.

**How to add (high level):**
- Heartbeat includes CPU/RAM/disk/toolchains + current workload.
- Scheduler respects health signals (disk low → stop scheduling).
- Add drain/quarantine/unquarantine workflows.
- Expose an Agent page and diagnostics bundle.

**Where it lives:** Agent · Gateway · Scheduler · UI


## Security and compliance
### Users
**Why:** Protects your build infrastructure (which is effectively remote code execution) and prevents secret leakage, abuse, and unauthorized actions.

**Goal:** Least-privilege access, safe secret delivery/redaction, auditable actions, and hardened endpoints.

**Includes:**

- roles (admin/member/viewer)
- project-level permissions
**Benefits:**
- Build agents are RCE nodes—security must be intentional.
- Reduces risk of credential leaks and abuse.
- Enables auditing and governance.

**How to add (high level):**
- RBAC: admin/member/viewer; project-level permissions.
- Secrets: scoped, encrypted at rest, JIT delivery, redaction.
- Agent identity: service tokens (rotate/revoke), limit pools/projects.
- Hardening: rate limits, CORS/CSP, audit events.

**Where it lives:** API · Services · DB · UI · Agent

### Agents
**Why:** Protects your build infrastructure (which is effectively remote code execution) and prevents secret leakage, abuse, and unauthorized actions.

**Goal:** Least-privilege access, safe secret delivery/redaction, auditable actions, and hardened endpoints.

**Includes:**

- separate credential type from users
- scoping: agent token allowed pools/projects only
- API keys for integrations (webhooks, triggers)
**Benefits:**
- Build agents are RCE nodes—security must be intentional.
- Reduces risk of credential leaks and abuse.
- Enables auditing and governance.

**How to add (high level):**
- RBAC: admin/member/viewer; project-level permissions.
- Secrets: scoped, encrypted at rest, JIT delivery, redaction.
- Agent identity: service tokens (rotate/revoke), limit pools/projects.
- Hardening: rate limits, CORS/CSP, audit events.

**Where it lives:** API · Services · DB · UI · Agent

### Secret scopes
**Why:** Protects your build infrastructure (which is effectively remote code execution) and prevents secret leakage, abuse, and unauthorized actions.

**Goal:** Least-privilege access, safe secret delivery/redaction, auditable actions, and hardened endpoints.

**Includes:**

- org/project/plan/run
**Benefits:**
- Build agents are RCE nodes—security must be intentional.
- Reduces risk of credential leaks and abuse.
- Enables auditing and governance.

**How to add (high level):**
- RBAC: admin/member/viewer; project-level permissions.
- Secrets: scoped, encrypted at rest, JIT delivery, redaction.
- Agent identity: service tokens (rotate/revoke), limit pools/projects.
- Hardening: rate limits, CORS/CSP, audit events.

**Where it lives:** API · Services · DB · UI · Agent

### Delivery rules
**Why:** Protects your build infrastructure (which is effectively remote code execution) and prevents secret leakage, abuse, and unauthorized actions.

**Goal:** Least-privilege access, safe secret delivery/redaction, auditable actions, and hardened endpoints.

**Includes:**

- fetch just-in-time
- never store raw secrets in logs
**Benefits:**
- Build agents are RCE nodes—security must be intentional.
- Reduces risk of credential leaks and abuse.
- Enables auditing and governance.

**How to add (high level):**
- RBAC: admin/member/viewer; project-level permissions.
- Secrets: scoped, encrypted at rest, JIT delivery, redaction.
- Agent identity: service tokens (rotate/revoke), limit pools/projects.
- Hardening: rate limits, CORS/CSP, audit events.

**Where it lives:** API · Services · DB · UI · Agent

### Redaction engine
**Why:** Protects your build infrastructure (which is effectively remote code execution) and prevents secret leakage, abuse, and unauthorized actions.

**Goal:** Least-privilege access, safe secret delivery/redaction, auditable actions, and hardened endpoints.

**Includes:**

- exact match redaction
- regex redaction
**Benefits:**
- Build agents are RCE nodes—security must be intentional.
- Reduces risk of credential leaks and abuse.
- Enables auditing and governance.

**How to add (high level):**
- RBAC: admin/member/viewer; project-level permissions.
- Secrets: scoped, encrypted at rest, JIT delivery, redaction.
- Agent identity: service tokens (rotate/revoke), limit pools/projects.
- Hardening: rate limits, CORS/CSP, audit events.

**Where it lives:** API · Services · DB · UI · Agent

### Secret rotation workflows
**Why:** Protects your build infrastructure (which is effectively remote code execution) and prevents secret leakage, abuse, and unauthorized actions.

**Goal:** Least-privilege access, safe secret delivery/redaction, auditable actions, and hardened endpoints.

**Includes:**

- rotate without downtime
- Audit secret access events
**Benefits:**
- Build agents are RCE nodes—security must be intentional.
- Reduces risk of credential leaks and abuse.
- Enables auditing and governance.

**How to add (high level):**
- RBAC: admin/member/viewer; project-level permissions.
- Secrets: scoped, encrypted at rest, JIT delivery, redaction.
- Agent identity: service tokens (rotate/revoke), limit pools/projects.
- Hardening: rate limits, CORS/CSP, audit events.

**Where it lives:** API · Services · DB · UI · Agent

### Restrict custom scripts
**Why:** Protects your build infrastructure (which is effectively remote code execution) and prevents secret leakage, abuse, and unauthorized actions.

**Goal:** Least-privilege access, safe secret delivery/redaction, auditable actions, and hardened endpoints.

**Includes:**

- allowlist commands
- sandbox user
- Validate plan signatures (optional)
**Benefits:**
- Build agents are RCE nodes—security must be intentional.
- Reduces risk of credential leaks and abuse.
- Enables auditing and governance.

**How to add (high level):**
- RBAC: admin/member/viewer; project-level permissions.
- Secrets: scoped, encrypted at rest, JIT delivery, redaction.
- Agent identity: service tokens (rotate/revoke), limit pools/projects.
- Hardening: rate limits, CORS/CSP, audit events.

**Where it lives:** API · Services · DB · UI · Agent

### Isolate builds
**Why:** Protects your build infrastructure (which is effectively remote code execution) and prevents secret leakage, abuse, and unauthorized actions.

**Goal:** Least-privilege access, safe secret delivery/redaction, auditable actions, and hardened endpoints.

**Includes:**

- dedicated low-privilege OS user
- separate workspaces with ACLs
**Benefits:**
- Build agents are RCE nodes—security must be intentional.
- Reduces risk of credential leaks and abuse.
- Enables auditing and governance.

**How to add (high level):**
- RBAC: admin/member/viewer; project-level permissions.
- Secrets: scoped, encrypted at rest, JIT delivery, redaction.
- Agent identity: service tokens (rotate/revoke), limit pools/projects.
- Hardening: rate limits, CORS/CSP, audit events.

**Where it lives:** API · Services · DB · UI · Agent

### Network policy per run (optional)
**Why:** Protects your build infrastructure (which is effectively remote code execution) and prevents secret leakage, abuse, and unauthorized actions.

**Goal:** Least-privilege access, safe secret delivery/redaction, auditable actions, and hardened endpoints.

**Includes:**

- block internet for certain jobs
**Benefits:**
- Build agents are RCE nodes—security must be intentional.
- Reduces risk of credential leaks and abuse.
- Enables auditing and governance.

**How to add (high level):**
- RBAC: admin/member/viewer; project-level permissions.
- Secrets: scoped, encrypted at rest, JIT delivery, redaction.
- Agent identity: service tokens (rotate/revoke), limit pools/projects.
- Hardening: rate limits, CORS/CSP, audit events.

**Where it lives:** API · Services · DB · UI · Agent

### Hardening basics
**Why:** Protects your build infrastructure (which is effectively remote code execution) and prevents secret leakage, abuse, and unauthorized actions.

**Goal:** Least-privilege access, safe secret delivery/redaction, auditable actions, and hardened endpoints.

**Includes:**

- Rate limiting (login + triggers + WS connect)
- WAF/proxy headers handling
- CORS locked down
- CSP in Angular
- PII handling rules
**Benefits:**
- Build agents are RCE nodes—security must be intentional.
- Reduces risk of credential leaks and abuse.
- Enables auditing and governance.

**How to add (high level):**
- RBAC: admin/member/viewer; project-level permissions.
- Secrets: scoped, encrypted at rest, JIT delivery, redaction.
- Agent identity: service tokens (rotate/revoke), limit pools/projects.
- Hardening: rate limits, CORS/CSP, audit events.

**Where it lives:** API · Services · DB · UI · Agent

### General
**Why:** agents are effectively remote code execution nodes.

**Goal:** Least-privilege access, safe secret delivery/redaction, auditable actions, and hardened endpoints.

**Includes:**

- Agents authenticate with service tokens (not user tokens)
- Token rotation/revoke
- Restrict which projects/queues an agent can accept
- Never send secrets in plain logs
**Benefits:**
- Build agents are RCE nodes—security must be intentional.
- Reduces risk of credential leaks and abuse.
- Enables auditing and governance.

**How to add (high level):**
- RBAC: admin/member/viewer; project-level permissions.
- Secrets: scoped, encrypted at rest, JIT delivery, redaction.
- Agent identity: service tokens (rotate/revoke), limit pools/projects.
- Hardening: rate limits, CORS/CSP, audit events.

**Where it lives:** API · Services · DB · UI · Agent

### Decide early
**Why:** you’ll eventually run untrusted-ish scripts.

**Goal:** Least-privilege access, safe secret delivery/redaction, auditable actions, and hardened endpoints.

**Includes:**

- are builds run as the agent user, a dedicated service user, or isolated per run?
- do you allow custom scripts? if yes, what restrictions?
- Even a minimal choice like “dedicated low-privilege user + workspace ACLs” helps.
**Benefits:**
- Build agents are RCE nodes—security must be intentional.
- Reduces risk of credential leaks and abuse.
- Enables auditing and governance.

**How to add (high level):**
- RBAC: admin/member/viewer; project-level permissions.
- Secrets: scoped, encrypted at rest, JIT delivery, redaction.
- Agent identity: service tokens (rotate/revoke), limit pools/projects.
- Hardening: rate limits, CORS/CSP, audit events.

**Where it lives:** API · Services · DB · UI · Agent

### Supply chain hygiene
**Why:** Protects your build infrastructure (which is effectively remote code execution) and prevents secret leakage, abuse, and unauthorized actions.

**Goal:** Least-privilege access, safe secret delivery/redaction, auditable actions, and hardened endpoints.

**Includes:**

- Signed agent binaries + verification
- Allowlist tools (only approved executables)
- Network policy per run (block internet for certain jobs)
**Benefits:**
- Build agents are RCE nodes—security must be intentional.
- Reduces risk of credential leaks and abuse.
- Enables auditing and governance.

**How to add (high level):**
- RBAC: admin/member/viewer; project-level permissions.
- Secrets: scoped, encrypted at rest, JIT delivery, redaction.
- Agent identity: service tokens (rotate/revoke), limit pools/projects.
- Hardening: rate limits, CORS/CSP, audit events.

**Where it lives:** API · Services · DB · UI · Agent

### Secret handling upgrades
**Why:** Protects your build infrastructure (which is effectively remote code execution) and prevents secret leakage, abuse, and unauthorized actions.

**Goal:** Least-privilege access, safe secret delivery/redaction, auditable actions, and hardened endpoints.

**Includes:**

- Secret scopes (org/project/plan)
- Secret rotation reminders
- Redaction rules (regex + known tokens)
**Benefits:**
- Build agents are RCE nodes—security must be intentional.
- Reduces risk of credential leaks and abuse.
- Enables auditing and governance.

**How to add (high level):**
- RBAC: admin/member/viewer; project-level permissions.
- Secrets: scoped, encrypted at rest, JIT delivery, redaction.
- Agent identity: service tokens (rotate/revoke), limit pools/projects.
- Hardening: rate limits, CORS/CSP, audit events.

**Where it lives:** API · Services · DB · UI · Agent

### Immutable audit for
**Why:** Protects your build infrastructure (which is effectively remote code execution) and prevents secret leakage, abuse, and unauthorized actions.

**Goal:** Least-privilege access, safe secret delivery/redaction, auditable actions, and hardened endpoints.

**Includes:**

- plan edits
- secret access
**Benefits:**
- Build agents are RCE nodes—security must be intentional.
- Reduces risk of credential leaks and abuse.
- Enables auditing and governance.

**How to add (high level):**
- RBAC: admin/member/viewer; project-level permissions.
- Secrets: scoped, encrypted at rest, JIT delivery, redaction.
- Agent identity: service tokens (rotate/revoke), limit pools/projects.
- Hardening: rate limits, CORS/CSP, audit events.

**Where it lives:** API · Services · DB · UI · Agent

### immutable audit events for
**Why:** Protects your build infrastructure (which is effectively remote code execution) and prevents secret leakage, abuse, and unauthorized actions.

**Goal:** Least-privilege access, safe secret delivery/redaction, auditable actions, and hardened endpoints.

**Includes:**

- plan changes
- run triggers/cancels
- secrets access
- agent token changes
**Benefits:**
- Build agents are RCE nodes—security must be intentional.
- Reduces risk of credential leaks and abuse.
- Enables auditing and governance.

**How to add (high level):**
- RBAC: admin/member/viewer; project-level permissions.
- Secrets: scoped, encrypted at rest, JIT delivery, redaction.
- Agent identity: service tokens (rotate/revoke), limit pools/projects.
- Hardening: rate limits, CORS/CSP, audit events.

**Where it lives:** API · Services · DB · UI · Agent


## Reliability and correctness
### Idempotency & dedup
**Why:** Improves completeness and maintainability.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- Idempotency key when triggering runs
- Deduplicate “duplicate dispatch” after reconnect
**Benefits:**
- No 'running forever' jobs.
- Recoverable restarts and disconnects.
- Predictable behavior under failures.

**How to add (high level):**
- Single source of truth for run/step state machine.
- Reconciler detects stuck/lost runs and resolves them.
- Idempotency keys for triggers and dispatch.
- Define retry rules (infra-only by default).

**Where it lives:** Scheduler · Gateway · DB · Agent

### Strict state transitions
**Why:** Improves completeness and maintainability.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- Queued → Dispatched → Running → Completed
- Lost → Failed (after timeout) etc.
**Benefits:**
- No 'running forever' jobs.
- Recoverable restarts and disconnects.
- Predictable behavior under failures.

**How to add (high level):**
- Single source of truth for run/step state machine.
- Reconciler detects stuck/lost runs and resolves them.
- Idempotency keys for triggers and dispatch.
- Define retry rules (infra-only by default).

**Where it lives:** Scheduler · Gateway · DB · Agent

### Run reconciliation jobs
**Why:** Improves completeness and maintainability.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- detect stuck runs
- mark timed-out runs
- mark lost agent runs
**Benefits:**
- No 'running forever' jobs.
- Recoverable restarts and disconnects.
- Predictable behavior under failures.

**How to add (high level):**
- Single source of truth for run/step state machine.
- Reconciler detects stuck/lost runs and resolves them.
- Idempotency keys for triggers and dispatch.
- Define retry rules (infra-only by default).

**Where it lives:** Scheduler · Gateway · DB · Agent

### Retry policy correctness
**Why:** Improves completeness and maintainability.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- retry only on infra reasons
- do not retry on compilation/test failure (unless configured)
**Benefits:**
- No 'running forever' jobs.
- Recoverable restarts and disconnects.
- Predictable behavior under failures.

**How to add (high level):**
- Single source of truth for run/step state machine.
- Reconciler detects stuck/lost runs and resolves them.
- Idempotency keys for triggers and dispatch.
- Define retry rules (infra-only by default).

**Where it lives:** Scheduler · Gateway · DB · Agent

### Backups & disaster recovery
**Why:** Improves completeness and maintainability.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- DB backups
- artifact storage backups
- restore procedure tested
**Benefits:**
- No 'running forever' jobs.
- Recoverable restarts and disconnects.
- Predictable behavior under failures.

**How to add (high level):**
- Single source of truth for run/step state machine.
- Reconciler detects stuck/lost runs and resolves them.
- Idempotency keys for triggers and dispatch.
- Define retry rules (infra-only by default).

**Where it lives:** Scheduler · Gateway · DB · Agent

### General
**Why:** build agents are remote code execution nodes; you need a clear trust boundary. prevents accidental abuse and keeps scheduling fair. WS protocols break silently unless tested. build agents slowly die from disk bloat and leftover state. you’ll break agents otherwise when you evolve the plan schema. logs are the biggest data stream and the easiest to overload your system. prevents “any agent can run anything” / “any user can run prod plan”. raw logs are painful; summaries help. helps investigate without turning your system into an RCE nightmare. this is a core operator workflow in build systems. builds aren’t useful without outputs. your UI will be judged on this. required once builds hang. WS connections drop. This is your #1 reality check. prevents accidental or malicious overload. makes the app usable even before it’s “feature-complete”.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- Each agent has a unique identity (agentId) + long-lived credential
- Prefer mTLS (client cert) or signed registration flow (bootstrap token → rotate to real token)
- Ability to revoke an agent instantly
- per project max concurrent builds
- per agent max runtime per day (optional)
- contract tests for WS messages (schema validation)
- replay tests (feed recorded WS traffic to API in CI)
- Per-run workspace cleanup policy (always clean / keep last N / keep on failure for debug)
- Disk pressure protection: if disk < X%, agent stops accepting builds
- Temp + cache directories separated
- Agent reports agentVersion + supported planVersion
- API can refuse incompatible agents or send “update required”
- Rolling upgrade strategy
- Chunking with sequence numbers
- Ack/backpressure (don’t let logs OOM the agent/API)
- Compression (optional)
- Log size limits + truncation rules (“kept first X + last Y”)
- Project isolation: an agent token can be limited to specific projects/queues
- Build plan authorization: who can trigger what
- Parse/store test results (TRX/JUnit), code coverage summary
- Optional: attach “diagnostic bundle” on failure (env info, tool versions)
- “Keep workspace on failure”
- “Upload workspace snapshot/artifacts”
- Remote shell is risky; better to provide controlled diagnostics
- Retry same plan
- Retry from failed step (if steps are deterministic)
- Retry on another agent (“run elsewhere”)
- “Re-run with same commit + same env”
- Upload/download artifacts (zip output, test results, nuget packages)
- Artifact retention policy
- Link artifacts to build run
- Search within logs
- Highlight error patterns
- “Jump to first error”
- Download full log
- Cancel build button (UI → API → agent)
- Agent tries graceful stop first, then hard kill after timeout
- Mark run as Canceled with reason
- heartbeat every N seconds
- if lost: mark agent Offline
- if a build is running: mark run as “Lost/Unknown” then “Failed due to agent disconnect” after timeout
- optional: allow resume if agent reconnects quickly
- login endpoint rate limit
- WS connection limits per user/IP
- first-run wizard / “create first build”
- clear empty states + sample data
**Benefits:**
- No 'running forever' jobs.
- Recoverable restarts and disconnects.
- Predictable behavior under failures.

**How to add (high level):**
- Single source of truth for run/step state machine.
- Reconciler detects stuck/lost runs and resolves them.
- Idempotency keys for triggers and dispatch.
- Define retry rules (infra-only by default).

**Where it lives:** Scheduler · Gateway · DB · Agent

### Store per run
**Why:** debugging and compliance—without this you can’t reproduce issues.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- toolchain versions (dotnet/msbuild), OS, agent version
- repo/commit + checkout method
- env vars (non-secret) + list of injected secret keys (names only)
**Benefits:**
- No 'running forever' jobs.
- Recoverable restarts and disconnects.
- Predictable behavior under failures.

**How to add (high level):**
- Single source of truth for run/step state machine.
- Reconciler detects stuck/lost runs and resolves them.
- Idempotency keys for triggers and dispatch.
- Define retry rules (infra-only by default).

**Where it lives:** Scheduler · Gateway · DB · Agent

### Define what happens if
**Why:** Improves completeness and maintainability.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- API restarts mid-run
- agent restarts mid-run
- WS reconnects and says “I was running runId=123”
**Benefits:**
- No 'running forever' jobs.
- Recoverable restarts and disconnects.
- Predictable behavior under failures.

**How to add (high level):**
- Single source of truth for run/step state machine.
- Reconciler detects stuck/lost runs and resolves them.
- Idempotency keys for triggers and dispatch.
- Define retry rules (infra-only by default).

**Where it lives:** Scheduler · Gateway · DB · Agent

### You need a reconciliation protocol
**Why:** otherwise you’ll get “stuck in Running forever”.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- API asks for last known run states
- agent reports “running/finished + exit code + last log sequence”
**Benefits:**
- No 'running forever' jobs.
- Recoverable restarts and disconnects.
- Predictable behavior under failures.

**How to add (high level):**
- Single source of truth for run/step state machine.
- Reconciler detects stuck/lost runs and resolves them.
- Idempotency keys for triggers and dispatch.
- Define retry rules (infra-only by default).

**Where it lives:** Scheduler · Gateway · DB · Agent

### UI showing
**Why:** Improves completeness and maintainability.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- which agents have which SDKs/tools
- which build plans require what
- mismatch detection before scheduling (“no agent matches plan requirements”)
**Benefits:**
- No 'running forever' jobs.
- Recoverable restarts and disconnects.
- Predictable behavior under failures.

**How to add (high level):**
- Single source of truth for run/step state machine.
- Reconciler detects stuck/lost runs and resolves them.
- Idempotency keys for triggers and dispatch.
- Define retry rules (infra-only by default).

**Where it lives:** Scheduler · Gateway · DB · Agent

### Build plans as a sequence of “steps
**Why:** Improves completeness and maintainability.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- DotNetRestore, MSBuild, Test, Publish, Script
**Benefits:**
- No 'running forever' jobs.
- Recoverable restarts and disconnects.
- Predictable behavior under failures.

**How to add (high level):**
- Single source of truth for run/step state machine.
- Reconciler detects stuck/lost runs and resolves them.
- Idempotency keys for triggers and dispatch.
- Define retry rules (infra-only by default).

**Where it lives:** Scheduler · Gateway · DB · Agent

### Later you can add
**Why:** Improves completeness and maintainability.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- DockerBuild, NpmBuild, TerraformPlan, etc.
**Benefits:**
- No 'running forever' jobs.
- Recoverable restarts and disconnects.
- Predictable behavior under failures.

**How to add (high level):**
- Single source of truth for run/step state machine.
- Reconciler detects stuck/lost runs and resolves them.
- Idempotency keys for triggers and dispatch.
- Define retry rules (infra-only by default).

**Where it lives:** Scheduler · Gateway · DB · Agent

### Instead of one giant log stream, also store
**Why:** makes troubleshooting 10x faster.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- step started/finished timestamps
- duration
- step status (Running/Success/Fail/Skipped)
**Benefits:**
- No 'running forever' jobs.
- Recoverable restarts and disconnects.
- Predictable behavior under failures.

**How to add (high level):**
- Single source of truth for run/step state machine.
- Reconciler detects stuck/lost runs and resolves them.
- Idempotency keys for triggers and dispatch.
- Define retry rules (infra-only by default).

**Where it lives:** Scheduler · Gateway · DB · Agent

### Build plans often need
**Why:** Improves completeness and maintainability.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- nuget feeds creds, signing keys, env vars
**Benefits:**
- No 'running forever' jobs.
- Recoverable restarts and disconnects.
- Predictable behavior under failures.

**How to add (high level):**
- Single source of truth for run/step state machine.
- Reconciler detects stuck/lost runs and resolves them.
- Idempotency keys for triggers and dispatch.
- Define retry rules (infra-only by default).

**Where it lives:** Scheduler · Gateway · DB · Agent

### Do
**Why:** otherwise you’ll leak credentials into logs/UI/Seq.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- secrets stored server-side encrypted
- agent receives secrets just-in-time
- redact secrets from logs
**Benefits:**
- No 'running forever' jobs.
- Recoverable restarts and disconnects.
- Predictable behavior under failures.

**How to add (high level):**
- Single source of truth for run/step state machine.
- Reconciler detects stuck/lost runs and resolves them.
- Idempotency keys for triggers and dispatch.
- Define retry rules (infra-only by default).

**Where it lives:** Scheduler · Gateway · DB · Agent

### Even if you only have “admin” vs “user” now, add a simple model early
**Why:** later you’ll want org/team separation and it’s painful to retrofit.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- roles (Admin, Member, Viewer)
- permissions per domain (Builds, Agents, Jobs, Settings)
**Benefits:**
- No 'running forever' jobs.
- Recoverable restarts and disconnects.
- Predictable behavior under failures.

**How to add (high level):**
- Single source of truth for run/step state machine.
- Reconciler detects stuck/lost runs and resolves them.
- Idempotency keys for triggers and dispatch.
- Define retry rules (infra-only by default).

**Where it lives:** Scheduler · Gateway · DB · Agent

### A lightweight event log like
**Why:** Improves completeness and maintainability.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- “Build created”, “Agent started”, “Job failed”, “Settings changed”
**Benefits:**
- No 'running forever' jobs.
- Recoverable restarts and disconnects.
- Predictable behavior under failures.

**How to add (high level):**
- Single source of truth for run/step state machine.
- Reconciler detects stuck/lost runs and resolves them.
- Idempotency keys for triggers and dispatch.
- Define retry rules (infra-only by default).

**Where it lives:** Scheduler · Gateway · DB · Agent

### Shown as
**Why:** helps support/debugging and makes the product feel “real”.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- UI activity feed + stored server-side
**Benefits:**
- No 'running forever' jobs.
- Recoverable restarts and disconnects.
- Predictable behavior under failures.

**How to add (high level):**
- Single source of truth for run/step state machine.
- Reconciler detects stuck/lost runs and resolves them.
- Idempotency keys for triggers and dispatch.
- Define retry rules (infra-only by default).

**Where it lives:** Scheduler · Gateway · DB · Agent

### If those WS services are doing work, you’ll want
**Why:** otherwise you’ll get “ghost jobs” and unclear failures.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- Job states (Queued, Running, Succeeded, Failed, Canceled, Retrying)
- Retries/backoff rules
- Idempotency (avoid double-processing)
- Dead-letter / failure reason
**Benefits:**
- No 'running forever' jobs.
- Recoverable restarts and disconnects.
- Predictable behavior under failures.

**How to add (high level):**
- Single source of truth for run/step state machine.
- Reconciler detects stuck/lost runs and resolves them.
- Idempotency keys for triggers and dispatch.
- Define retry rules (infra-only by default).

**Where it lives:** Scheduler · Gateway · DB · Agent

### Since you have WS
**Why:** WS edge cases will eat time if not designed.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- reconnect + backoff
- heartbeat/ping
- versioned message schema
- auth for WS (token refresh handling)
- “at-least-once vs exactly-once” message guarantees (pick one)
**Benefits:**
- No 'running forever' jobs.
- Recoverable restarts and disconnects.
- Predictable behavior under failures.

**How to add (high level):**
- Single source of truth for run/step state machine.
- Reconciler detects stuck/lost runs and resolves them.
- Idempotency keys for triggers and dispatch.
- Define retry rules (infra-only by default).

**Where it lives:** Scheduler · Gateway · DB · Agent

### Even if minimal
**Why:** these are the first things you’ll need when a token leaks or someone forgets a password.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- password hashing, lockout after repeated fails
- password reset flow (email or admin reset)
- refresh token rotation + revoke-all-sessions
**Benefits:**
- No 'running forever' jobs.
- Recoverable restarts and disconnects.
- Predictable behavior under failures.

**How to add (high level):**
- Single source of truth for run/step state machine.
- Reconciler detects stuck/lost runs and resolves them.
- Idempotency keys for triggers and dispatch.
- Define retry rules (infra-only by default).

**Where it lives:** Scheduler · Gateway · DB · Agent

### If your services run builds / agents
**Why:** otherwise you’ll end up storing secrets in plain config/logs.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- where secrets live (API keys, tokens)
- encryption at rest (even if simple)
**Benefits:**
- No 'running forever' jobs.
- Recoverable restarts and disconnects.
- Predictable behavior under failures.

**How to add (high level):**
- Single source of truth for run/step state machine.
- Reconciler detects stuck/lost runs and resolves them.
- Idempotency keys for triggers and dispatch.
- Define retry rules (infra-only by default).

**Where it lives:** Scheduler · Gateway · DB · Agent

### You already asked about this — make sure
**Why:** self-hosting + multiple services needs fast diagnostics.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- each WS service has health + “connected to broker/DB” readiness
- API has liveness/readiness
**Benefits:**
- No 'running forever' jobs.
- Recoverable restarts and disconnects.
- Predictable behavior under failures.

**How to add (high level):**
- Single source of truth for run/step state machine.
- Reconciler detects stuck/lost runs and resolves them.
- Idempotency keys for triggers and dispatch.
- Define retry rules (infra-only by default).

**Where it lives:** Scheduler · Gateway · DB · Agent

### Even minimal
**Why:** background work + WS services implies async outcomes.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- in-app notifications for job completion/failure
- optional email later
**Benefits:**
- No 'running forever' jobs.
- Recoverable restarts and disconnects.
- Predictable behavior under failures.

**How to add (high level):**
- Single source of truth for run/step state machine.
- Reconciler detects stuck/lost runs and resolves them.
- Idempotency keys for triggers and dispatch.
- Define retry rules (infra-only by default).

**Where it lives:** Scheduler · Gateway · DB · Agent


## Observability and monitoring
### structured logs
**Why:** Lets you detect issues early and debug faster by correlating logs, metrics, traces, and UI errors.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- runId, agentId, projectId, planId
- correlationId/traceId
- event types (RunStarted, AgentConnected, PlanDispatched, StepFailed)
- log level policy
- redaction before logging
**Benefits:**
- Faster debugging across services.
- Early detection of regressions and outages.
- Capacity planning and operational confidence.

**How to add (high level):**
- Standardize correlation fields (runId, agentId, traceId) everywhere.
- OTel instrumentation + Collector routing.
- Prometheus metrics for queues/WS/build durations.
- Grafana dashboards + alerts; trace backend (Jaeger/Tempo) later.

**Where it lives:** All components

### API
**Why:** Lets you detect issues early and debug faster by correlating logs, metrics, traces, and UI errors.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- request rate, duration histograms, 4xx/5xx rate
**Benefits:**
- Faster debugging across services.
- Early detection of regressions and outages.
- Capacity planning and operational confidence.

**How to add (high level):**
- Standardize correlation fields (runId, agentId, traceId) everywhere.
- OTel instrumentation + Collector routing.
- Prometheus metrics for queues/WS/build durations.
- Grafana dashboards + alerts; trace backend (Jaeger/Tempo) later.

**Where it lives:** All components

### Scheduler
**Why:** Lets you detect issues early and debug faster by correlating logs, metrics, traces, and UI errors.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- queue length, dispatch latency
**Benefits:**
- Faster debugging across services.
- Early detection of regressions and outages.
- Capacity planning and operational confidence.

**How to add (high level):**
- Standardize correlation fields (runId, agentId, traceId) everywhere.
- OTel instrumentation + Collector routing.
- Prometheus metrics for queues/WS/build durations.
- Grafana dashboards + alerts; trace backend (Jaeger/Tempo) later.

**Where it lives:** All components

### Agents
**Why:** Lets you detect issues early and debug faster by correlating logs, metrics, traces, and UI errors.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- online count, capacity, disk low count
**Benefits:**
- Faster debugging across services.
- Early detection of regressions and outages.
- Capacity planning and operational confidence.

**How to add (high level):**
- Standardize correlation fields (runId, agentId, traceId) everywhere.
- OTel instrumentation + Collector routing.
- Prometheus metrics for queues/WS/build durations.
- Grafana dashboards + alerts; trace backend (Jaeger/Tempo) later.

**Where it lives:** All components

### Builds
**Why:** Lets you detect issues early and debug faster by correlating logs, metrics, traces, and UI errors.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- duration histograms by plan/project
- success/failure rates by reason
- retry count
**Benefits:**
- Faster debugging across services.
- Early detection of regressions and outages.
- Capacity planning and operational confidence.

**How to add (high level):**
- Standardize correlation fields (runId, agentId, traceId) everywhere.
- OTel instrumentation + Collector routing.
- Prometheus metrics for queues/WS/build durations.
- Grafana dashboards + alerts; trace backend (Jaeger/Tempo) later.

**Where it lives:** All components

### WS
**Why:** Lets you detect issues early and debug faster by correlating logs, metrics, traces, and UI errors.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- connection count, reconnect rate, message rate
**Benefits:**
- Faster debugging across services.
- Early detection of regressions and outages.
- Capacity planning and operational confidence.

**How to add (high level):**
- Standardize correlation fields (runId, agentId, traceId) everywhere.
- OTel instrumentation + Collector routing.
- Prometheus metrics for queues/WS/build durations.
- Grafana dashboards + alerts; trace backend (Jaeger/Tempo) later.

**Where it lives:** All components

### Alerts
**Why:** Lets you detect issues early and debug faster by correlating logs, metrics, traces, and UI errors.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- queue growing continuously
- agents offline spike
- run stuck in running
- log ingestion lag
**Benefits:**
- Faster debugging across services.
- Early detection of regressions and outages.
- Capacity planning and operational confidence.

**How to add (high level):**
- Standardize correlation fields (runId, agentId, traceId) everywhere.
- OTel instrumentation + Collector routing.
- Prometheus metrics for queues/WS/build durations.
- Grafana dashboards + alerts; trace backend (Jaeger/Tempo) later.

**Where it lives:** All components

### Traces across
**Why:** Lets you detect issues early and debug faster by correlating logs, metrics, traces, and UI errors.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- API trigger → dispatch → agent ack → log ingestion → completion
- Propagate trace context via WS messages
- Sampling strategy (don’t trace everything at 100%)
**Benefits:**
- Faster debugging across services.
- Early detection of regressions and outages.
- Capacity planning and operational confidence.

**How to add (high level):**
- Standardize correlation fields (runId, agentId, traceId) everywhere.
- OTel instrumentation + Collector routing.
- Prometheus metrics for queues/WS/build durations.
- Grafana dashboards + alerts; trace backend (Jaeger/Tempo) later.

**Where it lives:** All components

### Frontend errors (Sentry)
**Why:** Lets you detect issues early and debug faster by correlating logs, metrics, traces, and UI errors.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- Angular runtime errors
- HTTP error breadcrumbs
- correlate with correlationId/runId
**Benefits:**
- Faster debugging across services.
- Early detection of regressions and outages.
- Capacity planning and operational confidence.

**How to add (high level):**
- Standardize correlation fields (runId, agentId, traceId) everywhere.
- OTel instrumentation + Collector routing.
- Prometheus metrics for queues/WS/build durations.
- Grafana dashboards + alerts; trace backend (Jaeger/Tempo) later.

**Where it lives:** All components

### If you do Seq + Prometheus + Sentry
**Why:** otherwise you’ll be blind when real usage starts.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- correlationId everywhere (API ↔ services ↔ UI)
- structured logs (no blobs)
- metrics for WS (connections, message rate, backlog)
- Sentry for Angular errors
**Benefits:**
- Faster debugging across services.
- Early detection of regressions and outages.
- Capacity planning and operational confidence.

**How to add (high level):**
- Standardize correlation fields (runId, agentId, traceId) everywhere.
- OTel instrumentation + Collector routing.
- Prometheus metrics for queues/WS/build durations.
- Grafana dashboards + alerts; trace backend (Jaeger/Tempo) later.

**Where it lives:** All components


## Admin and operator tooling
### Operators will need
**Why:** reality is messy; you need admin levers.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- mark run as failed/canceled manually
- force-disconnect agent / force-drain
**Benefits:**
- Operators can recover quickly from real-world failures.
- Less time spent firefighting.
- Better supportability for self-host customers.

**How to add (high level):**
- Admin actions: cancel, retry, reassign, drain, quarantine.
- Diagnostics: copy run summary, download logs, agent report.
- Maintenance mode and safe levers (no DB surgery).

**Where it lives:** UI · API · Scheduler · Gateway

### One click
**Why:** Gives operators safe levers to recover from real-world failures without manual DB edits or server access.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- agent diagnostic report (disk, tools, versions)
- last N logs
- last crash reason
- WS connection history
**Benefits:**
- Operators can recover quickly from real-world failures.
- Less time spent firefighting.
- Better supportability for self-host customers.

**How to add (high level):**
- Admin actions: cancel, retry, reassign, drain, quarantine.
- Diagnostics: copy run summary, download logs, agent report.
- Maintenance mode and safe levers (no DB surgery).

**Where it lives:** UI · API · Scheduler · Gateway

### Run replay
**Why:** Gives operators safe levers to recover from real-world failures without manual DB edits or server access.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- Re-run using exact same plan + commit + env snapshot (as close as possible)
**Benefits:**
- Operators can recover quickly from real-world failures.
- Less time spent firefighting.
- Better supportability for self-host customers.

**How to add (high level):**
- Admin actions: cancel, retry, reassign, drain, quarantine.
- Diagnostics: copy run summary, download logs, agent report.
- Maintenance mode and safe levers (no DB surgery).

**Where it lives:** UI · API · Scheduler · Gateway

### Admin controls
**Why:** Gives operators safe levers to recover from real-world failures without manual DB edits or server access.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- Force cancel run
- Reassign run to another agent
- Quarantine/unquarantine agent
- Drain agent
- Reset agent token
- Recompute agent capabilities (refresh)
- Manage pools/queues and concurrency rules
- Maintenance mode
**Benefits:**
- Operators can recover quickly from real-world failures.
- Less time spent firefighting.
- Better supportability for self-host customers.

**How to add (high level):**
- Admin actions: cancel, retry, reassign, drain, quarantine.
- Diagnostics: copy run summary, download logs, agent report.
- Maintenance mode and safe levers (no DB surgery).

**Where it lives:** UI · API · Scheduler · Gateway

### Diagnostics tools
**Why:** Gives operators safe levers to recover from real-world failures without manual DB edits or server access.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- “Download diagnostics bundle” (agent/system info)
- View WS connection history
- View last N agent errors
- Run replay details
**Benefits:**
- Operators can recover quickly from real-world failures.
- Less time spent firefighting.
- Better supportability for self-host customers.

**How to add (high level):**
- Admin actions: cancel, retry, reassign, drain, quarantine.
- Diagnostics: copy run summary, download logs, agent report.
- Maintenance mode and safe levers (no DB surgery).

**Where it lives:** UI · API · Scheduler · Gateway

### Support features
**Why:** Gives operators safe levers to recover from real-world failures without manual DB edits or server access.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- “copy run summary” button
- shareable links
- export runs to CSV/JSON
**Benefits:**
- Operators can recover quickly from real-world failures.
- Less time spent firefighting.
- Better supportability for self-host customers.

**How to add (high level):**
- Admin actions: cancel, retry, reassign, drain, quarantine.
- Diagnostics: copy run summary, download logs, agent report.
- Maintenance mode and safe levers (no DB surgery).

**Where it lives:** UI · API · Scheduler · Gateway


## UI and product polish
### Build UX
**Why:** Makes the system pleasant to use day-to-day: faster navigation, clearer runs, better logs, and fewer support questions.

**Goal:** Operators can find builds, understand failures, and take action in seconds.

**Includes:**

- Step grouping + collapsible sections (restore/build/test/publish)
- Log annotations: agent can emit structured markers (“warning”, “error”, “step-start”) so UI can jump around
- Build summary extraction: show “X warnings, Y errors, failing project name” (parsed from MSBuild output)
- Failure assistant: map common errors to hints (“SDK not installed”, “NuGet auth failed”)
**Benefits:**
- Makes the product feel 'real' and productive.
- Reduces time-to-triage and friction.
- Improves adoption and retention.

**How to add (high level):**
- Run timeline + step grouping + log viewer enhancements.
- Search/saved filters + bulk actions.
- Onboarding wizard (create first plan, install agent).
- Compare runs (durations/env/log around failure).

**Where it lives:** UI · API

### Compare two runs
**Why:** Makes the system pleasant to use day-to-day: faster navigation, clearer runs, better logs, and fewer support questions.

**Goal:** Operators can find builds, understand failures, and take action in seconds.

**Includes:**

- duration diff per step
- log diff around failure
- environment/tool version diff
- Regression detection: “build time increased 35% vs baseline”
**Benefits:**
- Makes the product feel 'real' and productive.
- Reduces time-to-triage and friction.
- Improves adoption and retention.

**How to add (high level):**
- Run timeline + step grouping + log viewer enhancements.
- Search/saved filters + bulk actions.
- Onboarding wizard (create first plan, install agent).
- Compare runs (durations/env/log around failure).

**Where it lives:** UI · API

### Policy checks: fail build if
**Why:** Makes the system pleasant to use day-to-day: faster navigation, clearer runs, better logs, and fewer support questions.

**Goal:** Operators can find builds, understand failures, and take action in seconds.

**Includes:**

- tests fail
- coverage < threshold
- warnings > threshold
- forbidden dependencies detected
- Manual approval step (optional): “approve before publish”
**Benefits:**
- Makes the product feel 'real' and productive.
- Reduces time-to-triage and friction.
- Improves adoption and retention.

**How to add (high level):**
- Run timeline + step grouping + log viewer enhancements.
- Search/saved filters + bulk actions.
- Onboarding wizard (create first plan, install agent).
- Compare runs (durations/env/log around failure).

**Where it lives:** UI · API

### Notifications & collaboration
**Why:** Makes the system pleasant to use day-to-day: faster navigation, clearer runs, better logs, and fewer support questions.

**Goal:** Operators can find builds, understand failures, and take action in seconds.

**Includes:**

- Watch builds / subscribe to project
- Mentions in run comments
- Digest notifications: daily “failed builds” report
**Benefits:**
- Makes the product feel 'real' and productive.
- Reduces time-to-triage and friction.
- Improves adoption and retention.

**How to add (high level):**
- Run timeline + step grouping + log viewer enhancements.
- Search/saved filters + bulk actions.
- Onboarding wizard (create first plan, install agent).
- Compare runs (durations/env/log around failure).

**Where it lives:** UI · API

### Navigation and productivity
**Why:** Makes the system pleasant to use day-to-day: faster navigation, clearer runs, better logs, and fewer support questions.

**Goal:** Operators can find builds, understand failures, and take action in seconds.

**Includes:**

- Global search (Ctrl+K)
- Saved filters/views
- Bulk actions (cancel/retry)
- Tags and labels
- Favorites / pinned projects
- Keyboard shortcuts
**Benefits:**
- Makes the product feel 'real' and productive.
- Reduces time-to-triage and friction.
- Improves adoption and retention.

**How to add (high level):**
- Run timeline + step grouping + log viewer enhancements.
- Search/saved filters + bulk actions.
- Onboarding wizard (create first plan, install agent).
- Compare runs (durations/env/log around failure).

**Where it lives:** UI · API

### Run UX
**Why:** Makes the system pleasant to use day-to-day: faster navigation, clearer runs, better logs, and fewer support questions.

**Goal:** Operators can find builds, understand failures, and take action in seconds.

**Includes:**

- Timeline view
- Step-by-step statuses
- Log viewer enhancements (above)
- Build summary card (warnings/errors/tests/artifacts)
- Compare runs
**Benefits:**
- Makes the product feel 'real' and productive.
- Reduces time-to-triage and friction.
- Improves adoption and retention.

**How to add (high level):**
- Run timeline + step grouping + log viewer enhancements.
- Search/saved filters + bulk actions.
- Onboarding wizard (create first plan, install agent).
- Compare runs (durations/env/log around failure).

**Where it lives:** UI · API

### Notifications
**Why:** Makes the system pleasant to use day-to-day: faster navigation, clearer runs, better logs, and fewer support questions.

**Goal:** Operators can find builds, understand failures, and take action in seconds.

**Includes:**

- in-app notifications
- email (later)
- webhooks / Slack (later)
- subscribe/unsubscribe per project/plan
- in-app + optional email/webhook later
- “Job failed/succeeded”
- Webhooks: when job completes/fails, send to another system (super useful).
- Export: CSV/JSON export of jobs/builds, or “download run summary”.
**Benefits:**
- Makes the product feel 'real' and productive.
- Reduces time-to-triage and friction.
- Improves adoption and retention.

**How to add (high level):**
- Run timeline + step grouping + log viewer enhancements.
- Search/saved filters + bulk actions.
- Onboarding wizard (create first plan, install agent).
- Compare runs (durations/env/log around failure).

**Where it lives:** UI · API

### Onboarding
**Why:** Makes the system pleasant to use day-to-day: faster navigation, clearer runs, better logs, and fewer support questions.

**Goal:** Operators can find builds, understand failures, and take action in seconds.

**Includes:**

- “create first plan” wizard
- sample plan templates
- agent install guide in UI
- “no agents online” helper
**Benefits:**
- Makes the product feel 'real' and productive.
- Reduces time-to-triage and friction.
- Improves adoption and retention.

**How to add (high level):**
- Run timeline + step grouping + log viewer enhancements.
- Search/saved filters + bulk actions.
- Onboarding wizard (create first plan, install agent).
- Compare runs (durations/env/log around failure).

**Where it lives:** UI · API

### Formalize
**Why:** prevents random plan failures.

**Goal:** Operators can find builds, understand failures, and take action in seconds.

**Includes:**

- capabilities (installed tools, OS, tags)
- requirements in plan (needs: windows + vs2022 + net8)
- mismatch errors that are clear
**Benefits:**
- Makes the product feel 'real' and productive.
- Reduces time-to-triage and friction.
- Improves adoption and retention.

**How to add (high level):**
- Run timeline + step grouping + log viewer enhancements.
- Search/saved filters + bulk actions.
- Onboarding wizard (create first plan, install agent).
- Compare runs (durations/env/log around failure).

**Where it lives:** UI · API

### General
**Why:** hanging builds are common. prevents “agent updated and broke old plans” and makes reruns reproducible. prevents one noisy project from starving others. huge speedups and cost savings.

**Goal:** Operators can find builds, understand failures, and take action in seconds.

**Includes:**

- Timeout per step + whole run timeout
- Heartbeat from build process itself (not only WS)
- Detect “no log output for N minutes” → warn/kill
- Version every build plan schema (planVersion)
- Validate plan before sending to agent
- Keep a stored copy of the exact plan used for a run
- per-agent max parallel builds
- global queue
- priorities (hotfix > normal)
- fair scheduling by project/team
- NuGet cache persistence per agent
- repo clone cache (mirror)
- incremental builds where possible
**Benefits:**
- Makes the product feel 'real' and productive.
- Reduces time-to-triage and friction.
- Improves adoption and retention.

**How to add (high level):**
- Run timeline + step grouping + log viewer enhancements.
- Search/saved filters + bulk actions.
- Onboarding wizard (create first plan, install agent).
- Compare runs (durations/env/log around failure).

**Where it lives:** UI · API

### Nice-to-have UI features specifically
**Why:** Makes the system pleasant to use day-to-day: faster navigation, clearer runs, better logs, and fewer support questions.

**Goal:** Operators can find builds, understand failures, and take action in seconds.

**Includes:**

- Agent page: live CPU/RAM/disk, current job, last heartbeat, installed tooling list
- Run summary: status, duration, agent used, commit, triggered by, artifacts, test summary
- Compare runs: diff durations/steps between two builds
- Pinned “known issues”: show common failure types and fixes
**Benefits:**
- Makes the product feel 'real' and productive.
- Reduces time-to-triage and friction.
- Improves adoption and retention.

**How to add (high level):**
- Run timeline + step grouping + log viewer enhancements.
- Search/saved filters + bulk actions.
- Onboarding wizard (create first plan, install agent).
- Compare runs (durations/env/log around failure).

**Where it lives:** UI · API

### If I had to flag the ones that aren’t optional for long
**Why:** Makes the system pleasant to use day-to-day: faster navigation, clearer runs, better logs, and fewer support questions.

**Goal:** Operators can find builds, understand failures, and take action in seconds.

**Includes:**

- Agent capability matching + scheduling
- Heartbeat/offline detection + run state reconciliation
- Cancellation
- Secrets redaction + safe secret delivery
**Benefits:**
- Makes the product feel 'real' and productive.
- Reduces time-to-triage and friction.
- Improves adoption and retention.

**How to add (high level):**
- Run timeline + step grouping + log viewer enhancements.
- Search/saved filters + bulk actions.
- Onboarding wizard (create first plan, install agent).
- Compare runs (durations/env/log around failure).

**Where it lives:** UI · API

### For this build-agent world, add
**Why:** Makes the system pleasant to use day-to-day: faster navigation, clearer runs, better logs, and fewer support questions.

**Goal:** Operators can find builds, understand failures, and take action in seconds.

**Includes:**

- RunId and CorrelationId on every WS message + log line
**Benefits:**
- Makes the product feel 'real' and productive.
- Reduces time-to-triage and friction.
- Improves adoption and retention.

**How to add (high level):**
- Run timeline + step grouping + log viewer enhancements.
- Search/saved filters + bulk actions.
- Onboarding wizard (create first plan, install agent).
- Compare runs (durations/env/log around failure).

**Where it lives:** UI · API

### Prometheus metrics
**Why:** Makes the system pleasant to use day-to-day: faster navigation, clearer runs, better logs, and fewer support questions.

**Goal:** Operators can find builds, understand failures, and take action in seconds.

**Includes:**

- queue length, build duration histograms, agent online count, failure rate by reason
**Benefits:**
- Makes the product feel 'real' and productive.
- Reduces time-to-triage and friction.
- Improves adoption and retention.

**How to add (high level):**
- Run timeline + step grouping + log viewer enhancements.
- Search/saved filters + bulk actions.
- Onboarding wizard (create first plan, install agent).
- Compare runs (durations/env/log around failure).

**Where it lives:** UI · API

### Seq structured logs
**Why:** Makes the system pleasant to use day-to-day: faster navigation, clearer runs, better logs, and fewer support questions.

**Goal:** Operators can find builds, understand failures, and take action in seconds.

**Includes:**

- run started/finished, agent connected/disconnected, plan dispatched/ack, step failed
**Benefits:**
- Makes the product feel 'real' and productive.
- Reduces time-to-triage and friction.
- Improves adoption and retention.

**How to add (high level):**
- Run timeline + step grouping + log viewer enhancements.
- Search/saved filters + bulk actions.
- Onboarding wizard (create first plan, install agent).
- Compare runs (durations/env/log around failure).

**Where it lives:** UI · API

### Sentry
**Why:** Makes the system pleasant to use day-to-day: faster navigation, clearer runs, better logs, and fewer support questions.

**Goal:** Operators can find builds, understand failures, and take action in seconds.

**Includes:**

- Angular UI errors only (and maybe API exceptions)
**Benefits:**
- Makes the product feel 'real' and productive.
- Reduces time-to-triage and friction.
- Improves adoption and retention.

**How to add (high level):**
- Run timeline + step grouping + log viewer enhancements.
- Search/saved filters + bulk actions.
- Onboarding wizard (create first plan, install agent).
- Compare runs (durations/env/log around failure).

**Where it lives:** UI · API

### UX / Quality-of-life
**Why:** Makes the system pleasant to use day-to-day: faster navigation, clearer runs, better logs, and fewer support questions.

**Goal:** Operators can find builds, understand failures, and take action in seconds.

**Includes:**

- Global command/search bar (Ctrl+K): jump to builds/jobs/agents, quick actions.
- Saved filters / views: “My failed jobs”, “Last 24h”, “Hotfix runs”.
- Bulk actions: retry/cancel/archive multiple jobs.
- Undo for destructive actions (or “soft delete”).
- Keyboard shortcuts + “copy id / copy link” buttons everywhere.
- Deep links to exact job/run/log line and shareable URLs.
**Benefits:**
- Makes the product feel 'real' and productive.
- Reduces time-to-triage and friction.
- Improves adoption and retention.

**How to add (high level):**
- Run timeline + step grouping + log viewer enhancements.
- Search/saved filters + bulk actions.
- Onboarding wizard (create first plan, install agent).
- Compare runs (durations/env/log around failure).

**Where it lives:** UI · API

### Realtime & observability in the UI
**Why:** Makes the system pleasant to use day-to-day: faster navigation, clearer runs, better logs, and fewer support questions.

**Goal:** Operators can find builds, understand failures, and take action in seconds.

**Includes:**

- Live job timeline: steps update in real time (WS), with a clear state machine.
**Benefits:**
- Makes the product feel 'real' and productive.
- Reduces time-to-triage and friction.
- Improves adoption and retention.

**How to add (high level):**
- Run timeline + step grouping + log viewer enhancements.
- Search/saved filters + bulk actions.
- Onboarding wizard (create first plan, install agent).
- Compare runs (durations/env/log around failure).

**Where it lives:** UI · API

### Log streaming with
**Why:** Makes the system pleasant to use day-to-day: faster navigation, clearer runs, better logs, and fewer support questions.

**Goal:** Operators can find builds, understand failures, and take action in seconds.

**Includes:**

- pause/resume
- auto-scroll toggle
**Benefits:**
- Makes the product feel 'real' and productive.
- Reduces time-to-triage and friction.
- Improves adoption and retention.

**How to add (high level):**
- Run timeline + step grouping + log viewer enhancements.
- Search/saved filters + bulk actions.
- Onboarding wizard (create first plan, install agent).
- Compare runs (durations/env/log around failure).

**Where it lives:** UI · API

### User-friendly error pages
**Why:** Makes the system pleasant to use day-to-day: faster navigation, clearer runs, better logs, and fewer support questions.

**Goal:** Operators can find builds, understand failures, and take action in seconds.

**Includes:**

- “Something went wrong” with correlationId
- “Reconnect” button if WS drops
- Status page inside the app: show API/service connectivity + last heartbeat.
**Benefits:**
- Makes the product feel 'real' and productive.
- Reduces time-to-triage and friction.
- Improves adoption and retention.

**How to add (high level):**
- Run timeline + step grouping + log viewer enhancements.
- Search/saved filters + bulk actions.
- Onboarding wizard (create first plan, install agent).
- Compare runs (durations/env/log around failure).

**Where it lives:** UI · API

### Reliability / “operator” features
**Why:** Makes the system pleasant to use day-to-day: faster navigation, clearer runs, better logs, and fewer support questions.

**Goal:** Operators can find builds, understand failures, and take action in seconds.

**Includes:**

- Retry with options: retry from failed step, retry with same inputs, retry with new params.
- Throttling / concurrency limits per service or per project.
- Maintenance mode: temporarily block new jobs while keeping UI accessible.
- Circuit breaker visibility: show when a service is degraded / reconnecting.
**Benefits:**
- Makes the product feel 'real' and productive.
- Reduces time-to-triage and friction.
- Improves adoption and retention.

**How to add (high level):**
- Run timeline + step grouping + log viewer enhancements.
- Search/saved filters + bulk actions.
- Onboarding wizard (create first plan, install agent).
- Compare runs (durations/env/log around failure).

**Where it lives:** UI · API

### Security / admin
**Why:** Makes the system pleasant to use day-to-day: faster navigation, clearer runs, better logs, and fewer support questions.

**Goal:** Operators can find builds, understand failures, and take action in seconds.

**Includes:**

- API keys/service tokens for services (separate from user auth).
- Session management: “log out other sessions”, “active sessions list”.
- Basic admin panel: manage users, roles, service health, feature flags.
**Benefits:**
- Makes the product feel 'real' and productive.
- Reduces time-to-triage and friction.
- Improves adoption and retention.

**How to add (high level):**
- Run timeline + step grouping + log viewer enhancements.
- Search/saved filters + bulk actions.
- Onboarding wizard (create first plan, install agent).
- Compare runs (durations/env/log around failure).

**Where it lives:** UI · API

### Developer experience
**Why:** Makes the system pleasant to use day-to-day: faster navigation, clearer runs, better logs, and fewer support questions.

**Goal:** Operators can find builds, understand failures, and take action in seconds.

**Includes:**

- Sandbox/demo mode: sample data so the UI is never empty.
- Feature flags: hide unfinished features without branching hell.
- Changelog / release notes in-app (simple markdown).
**Benefits:**
- Makes the product feel 'real' and productive.
- Reduces time-to-triage and friction.
- Improves adoption and retention.

**How to add (high level):**
- Run timeline + step grouping + log viewer enhancements.
- Search/saved filters + bulk actions.
- Onboarding wizard (create first plan, install agent).
- Compare runs (durations/env/log around failure).

**Where it lives:** UI · API

### Product-level features (make it usable as a real app)
**Why:** Makes the system pleasant to use day-to-day: faster navigation, clearer runs, better logs, and fewer support questions.

**Goal:** Operators can find builds, understand failures, and take action in seconds.

**Includes:**

- Organizations / Projects / Workspaces: even if you start single-tenant, design entities so you can later group data (Org → Project → Builds/Jobs).
- Search + filtering + sorting across core entities (builds, jobs, agents, logs). This becomes mandatory fast.
- Tags / labels for builds/jobs (“release”, “hotfix”, “customer-x”).
- Templates / presets (job templates, build configs) to reduce repeated setup.
- Versioning of configurations (so you can rollback and compare changes).
**Benefits:**
- Makes the product feel 'real' and productive.
- Reduces time-to-triage and friction.
- Improves adoption and retention.

**How to add (high level):**
- Run timeline + step grouping + log viewer enhancements.
- Search/saved filters + bulk actions.
- Onboarding wizard (create first plan, install agent).
- Compare runs (durations/env/log around failure).

**Where it lives:** UI · API

### Collaboration / governance
**Why:** Makes the system pleasant to use day-to-day: faster navigation, clearer runs, better logs, and fewer support questions.

**Goal:** Operators can find builds, understand failures, and take action in seconds.

**Includes:**

- Invites + team management (invite users by email, roles).
- Approvals (optional): e.g., approving a deploy/run to production.
- Comments / mentions on jobs/builds (minimal collaboration layer).
- Audit trail (who changed what, when) — you already touched this, but it’s big.
**Benefits:**
- Makes the product feel 'real' and productive.
- Reduces time-to-triage and friction.
- Improves adoption and retention.

**How to add (high level):**
- Run timeline + step grouping + log viewer enhancements.
- Search/saved filters + bulk actions.
- Onboarding wizard (create first plan, install agent).
- Compare runs (durations/env/log around failure).

**Where it lives:** UI · API

### Reliability features for multi-service/WS systems
**Why:** Makes the system pleasant to use day-to-day: faster navigation, clearer runs, better logs, and fewer support questions.

**Goal:** Operators can find builds, understand failures, and take action in seconds.

**Includes:**

- Message contract versioning (v1/v2) + backward compatibility strategy.
**Benefits:**
- Makes the product feel 'real' and productive.
- Reduces time-to-triage and friction.
- Improves adoption and retention.

**How to add (high level):**
- Run timeline + step grouping + log viewer enhancements.
- Search/saved filters + bulk actions.
- Onboarding wizard (create first plan, install agent).
- Compare runs (durations/env/log around failure).

**Where it lives:** UI · API

### Guaranteed delivery strategy
**Why:** Makes the system pleasant to use day-to-day: faster navigation, clearer runs, better logs, and fewer support questions.

**Goal:** Operators can find builds, understand failures, and take action in seconds.

**Includes:**

- at-least-once with idempotency keys (common)
- or exactly-once (hard, usually not worth it early)
- Backpressure handling (what happens if consumers are slow?).
- Dead-letter queue concept (even if it’s just a table) for failed messages.
- Graceful shutdown and in-flight work draining for services.
- Service discovery / registration (API knows what services are alive/capable).
**Benefits:**
- Makes the product feel 'real' and productive.
- Reduces time-to-triage and friction.
- Improves adoption and retention.

**How to add (high level):**
- Run timeline + step grouping + log viewer enhancements.
- Search/saved filters + bulk actions.
- Onboarding wizard (create first plan, install agent).
- Compare runs (durations/env/log around failure).

**Where it lives:** UI · API

### Security hardening (cheap to add early)
**Why:** Makes the system pleasant to use day-to-day: faster navigation, clearer runs, better logs, and fewer support questions.

**Goal:** Operators can find builds, understand failures, and take action in seconds.

**Includes:**

- CORS policy locked down (prod).
- CSRF strategy (less critical if you’re pure bearer tokens, but be intentional).
- Content Security Policy (CSP) for Angular (prevents whole classes of attacks).
- Secrets redaction in logs + UI (don’t accidentally display tokens).
- PII classification (“never log these fields”) baked into logging helpers.
**Benefits:**
- Makes the product feel 'real' and productive.
- Reduces time-to-triage and friction.
- Improves adoption and retention.

**How to add (high level):**
- Run timeline + step grouping + log viewer enhancements.
- Search/saved filters + bulk actions.
- Onboarding wizard (create first plan, install agent).
- Compare runs (durations/env/log around failure).

**Where it lives:** UI · API

### Developer experience / maintainability
**Why:** Makes the system pleasant to use day-to-day: faster navigation, clearer runs, better logs, and fewer support questions.

**Goal:** Operators can find builds, understand failures, and take action in seconds.

**Includes:**

- API versioning strategy (even simple /v1).
- Error contract standard (ProblemDetails everywhere, consistent error codes).
- Feature flags (even a simple DB/config flag system).
- Background jobs scheduling (if any timed jobs exist).
- DB migrations + seed and “dev environment bootstrap”.
**Benefits:**
- Makes the product feel 'real' and productive.
- Reduces time-to-triage and friction.
- Improves adoption and retention.

**How to add (high level):**
- Run timeline + step grouping + log viewer enhancements.
- Search/saved filters + bulk actions.
- Onboarding wizard (create first plan, install agent).
- Compare runs (durations/env/log around failure).

**Where it lives:** UI · API

### Release/version stamp everywhere
**Why:** Makes the system pleasant to use day-to-day: faster navigation, clearer runs, better logs, and fewer support questions.

**Goal:** Operators can find builds, understand failures, and take action in seconds.

**Includes:**

- UI shows app version
- API exposes version endpoint
- logs include version
- Sentry tags include release
**Benefits:**
- Makes the product feel 'real' and productive.
- Reduces time-to-triage and friction.
- Improves adoption and retention.

**How to add (high level):**
- Run timeline + step grouping + log viewer enhancements.
- Search/saved filters + bulk actions.
- Onboarding wizard (create first plan, install agent).
- Compare runs (durations/env/log around failure).

**Where it lives:** UI · API

### Correlation everywhere
**Why:** Makes the system pleasant to use day-to-day: faster navigation, clearer runs, better logs, and fewer support questions.

**Goal:** Operators can find builds, understand failures, and take action in seconds.

**Includes:**

- correlationId + traceId propagation through WS messages
- Rate-limited error reporting from UI (don’t spam Sentry/Seq during outages).
**Benefits:**
- Makes the product feel 'real' and productive.
- Reduces time-to-triage and friction.
- Improves adoption and retention.

**How to add (high level):**
- Run timeline + step grouping + log viewer enhancements.
- Search/saved filters + bulk actions.
- Onboarding wizard (create first plan, install agent).
- Compare runs (durations/env/log around failure).

**Where it lives:** UI · API

### Ops / self-hosting essentials
**Why:** Makes the system pleasant to use day-to-day: faster navigation, clearer runs, better logs, and fewer support questions.

**Goal:** Operators can find builds, understand failures, and take action in seconds.

**Includes:**

- Docker compose “one command up” for UI+API+services+Seq+Prom/Grafana (even if you later move to k8s).
- Config layering (dev/stage/prod) + secrets injection.
- Backup/restore runbook (even basic).
- Resource limits (services don’t eat the host).
**Benefits:**
- Makes the product feel 'real' and productive.
- Reduces time-to-triage and friction.
- Improves adoption and retention.

**How to add (high level):**
- Run timeline + step grouping + log viewer enhancements.
- Search/saved filters + bulk actions.
- Onboarding wizard (create first plan, install agent).
- Compare runs (durations/env/log around failure).

**Where it lives:** UI · API


## Integrations and public API
### Webhooks
**Why:** Connects Build Hub to the rest of your ecosystem (Git providers, feeds, notifications) and enables automation.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- push triggers
- PR triggers
**Benefits:**
- Automates workflows and reduces manual steps.
- Connects Build Hub to existing ecosystems.
- Enables 'build as a service' usage.

**How to add (high level):**
- Outgoing webhooks (signed) with retries.
- API keys/service tokens for incoming triggers.
- Git provider integrations (PR status checks) later.
- Integrate package feeds/registries if needed.

**Where it lives:** API · Notification service · UI

### Webhooks + API integrations
**Why:** Connects Build Hub to the rest of your ecosystem (Git providers, feeds, notifications) and enables automation.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- Outgoing webhooks with signature
- Incoming “trigger build” API tokens
- Git providers later (GitHub/GitLab/Bitbucket) for PR status checks
**Benefits:**
- Automates workflows and reduces manual steps.
- Connects Build Hub to existing ecosystems.
- Enables 'build as a service' usage.

**How to add (high level):**
- Outgoing webhooks (signed) with retries.
- API keys/service tokens for incoming triggers.
- Git provider integrations (PR status checks) later.
- Integrate package feeds/registries if needed.

**Where it lives:** API · Notification service · UI

### Integrations you’ll likely want
**Why:** Connects Build Hub to the rest of your ecosystem (Git providers, feeds, notifications) and enables automation.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- GitHub/GitLab/Bitbucket webhooks
- NuGet feed integration (private feeds)
- Slack/Teams notifications
- Webhooks for run complete/fail
- Issue tracker integration (create ticket on failure)
- Container registry integration (if you build containers later)
**Benefits:**
- Automates workflows and reduces manual steps.
- Connects Build Hub to existing ecosystems.
- Enables 'build as a service' usage.

**How to add (high level):**
- Outgoing webhooks (signed) with retries.
- API keys/service tokens for incoming triggers.
- Git provider integrations (PR status checks) later.
- Integrate package feeds/registries if needed.

**Where it lives:** API · Notification service · UI

### Public REST endpoints
**Why:** Connects Build Hub to the rest of your ecosystem (Git providers, feeds, notifications) and enables automation.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- trigger run
- list runs
- fetch artifacts
- manage plans
- Webhooks with signing secret
- Rate limits & API keys
**Benefits:**
- Automates workflows and reduces manual steps.
- Connects Build Hub to existing ecosystems.
- Enables 'build as a service' usage.

**How to add (high level):**
- Outgoing webhooks (signed) with retries.
- API keys/service tokens for incoming triggers.
- Git provider integrations (PR status checks) later.
- Integrate package feeds/registries if needed.

**Where it lives:** API · Notification service · UI

### General
**Why:** builds are async; notifications make it usable.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- Webhook on build complete/fail
- Slack/Teams later
**Benefits:**
- Automates workflows and reduces manual steps.
- Connects Build Hub to existing ecosystems.
- Enables 'build as a service' usage.

**How to add (high level):**
- Outgoing webhooks (signed) with retries.
- API keys/service tokens for incoming triggers.
- Git provider integrations (PR status checks) later.
- Integrate package feeds/registries if needed.

**Where it lives:** API · Notification service · UI


## Data management and compliance
### Keep logs/artifacts
**Why:** Controls storage growth and supports privacy/compliance needs like retention, export, and deletion.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- 7/30/90 days
- longer for “release” tagged builds
- Automatic pruning jobs
**Benefits:**
- Controls storage growth.
- Supports compliance needs in self-host environments.
- Predictable cost and maintenance.

**How to add (high level):**
- Retention policies for logs/artifacts per project.
- Pruning jobs and restore/runbooks.
- Export/delete project data later if needed.

**Where it lives:** DB · Storage · Maintenance jobs

### Retention and privacy
**Why:** Controls storage growth and supports privacy/compliance needs like retention, export, and deletion.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- log retention settings per project
- artifact retention settings
- delete project data
- export data (for compliance)
**Benefits:**
- Controls storage growth.
- Supports compliance needs in self-host environments.
- Predictable cost and maintenance.

**How to add (high level):**
- Retention policies for logs/artifacts per project.
- Pruning jobs and restore/runbooks.
- Export/delete project data later if needed.

**Where it lives:** DB · Storage · Maintenance jobs


## Developer experience and testing
### docker-compose “one command up” for
**Why:** Reduces regressions and speeds up development via consistent environments, simulators, and contract tests.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- API + DB + Seq + Prom + Grafana + (Tempo/Jaeger)
- seed data
- fake agent simulator for testing WS protocol
**Benefits:**
- Fewer regressions and faster onboarding.
- Confidence when evolving protocol/schema.
- Catches WS edge cases early.

**How to add (high level):**
- docker-compose: API + DB + Seq + Prom + Grafana (+ trace backend).
- WS contract tests and recorded traffic replay tests.
- Scheduler tests (capability matching) and load tests (log streaming).

**Where it lives:** Repo tooling · CI · API/Agent tests

### Testing strategy
**Why:** Reduces regressions and speeds up development via consistent environments, simulators, and contract tests.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- WS contract tests (schema validation)
- recorded traffic replay tests
- scheduler tests (capability matching)
- load tests for logs streaming
- chaos tests (drop WS, restart API mid-run)
**Benefits:**
- Fewer regressions and faster onboarding.
- Confidence when evolving protocol/schema.
- Catches WS edge cases early.

**How to add (high level):**
- docker-compose: API + DB + Seq + Prom + Grafana (+ trace backend).
- WS contract tests and recorded traffic replay tests.
- Scheduler tests (capability matching) and load tests (log streaming).

**Where it lives:** Repo tooling · CI · API/Agent tests

### Versioning strategy
**Why:** Reduces regressions and speeds up development via consistent environments, simulators, and contract tests.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- API versioning (/v1)
- plan schema versioning
- agent version compatibility rules
**Benefits:**
- Fewer regressions and faster onboarding.
- Confidence when evolving protocol/schema.
- Catches WS edge cases early.

**How to add (high level):**
- docker-compose: API + DB + Seq + Prom + Grafana (+ trace backend).
- WS contract tests and recorded traffic replay tests.
- Scheduler tests (capability matching) and load tests (log streaming).

**Where it lives:** Repo tooling · CI · API/Agent tests


## Analytics and insights
### Analytics
**Why:** Helps you improve build reliability and speed by turning run history into trends and actionable insights.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- Build success rate per repo
- Mean time to fix (MTTF) per project
- Flakiest tests ranking
- Slowest steps ranking
**Benefits:**
- Turns history into actionable improvements.
- Finds bottlenecks and flaky tests.
- Helps planning and prioritization.

**How to add (high level):**
- Standardize failure reasons and store them on runs/steps.
- Compute aggregates (success rate, slowest steps, flakiest tests).
- Expose trends in UI and/or Grafana.

**Where it lives:** DB · API · UI · Grafana

### Standardize failure reasons
**Why:** lets you build dashboards and auto-suggest fixes.

**Goal:** Make the feature reliable, safe, and easy to operate.

**Includes:**

- CheckoutFailed, ToolMissing, CompilationError, TestsFailed, Timeout, AgentDisconnected, OutOfDisk, Canceled, etc.
**Benefits:**
- Turns history into actionable improvements.
- Finds bottlenecks and flaky tests.
- Helps planning and prioritization.

**How to add (high level):**
- Standardize failure reasons and store them on runs/steps.
- Compute aggregates (success rate, slowest steps, flakiest tests).
- Expose trends in UI and/or Grafana.

**Where it lives:** DB · API · UI · Grafana


