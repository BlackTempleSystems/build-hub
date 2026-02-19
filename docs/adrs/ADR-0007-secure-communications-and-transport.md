# ADR-0007: Secure communications (UI ↔ API ↔ Services) and transport hardening

- **Status:** Accepted (iterative; revisit when moving to OAuth / dedicated Auth service)
- **Date:** 2026-02-15
- **Decision owners:** Build Hub team
- **Related:** Architecture & services docs; Observability stack (OTel, Prometheus, Grafana); Token-based auth roadmap.

## Context

Build Hub is designed as:

- **Angular UI** communicating to **Build Hub API** via **HTTPS + WebSockets (WSS)**.
- **Services** (build/execution-related) will run on **different machines** and communicate using **WebSockets**, with the **services initiating the connection** to the API (and potentially a dedicated **WS Hub** in the future). fileciteturn2file3
- Current deployment approach is **Docker** (Windows now, potentially Linux later). fileciteturn2file6

Threat model / goals:

- Prevent **MITM** on any traffic that crosses a network boundary.
- Avoid exposing internal services/admin tooling to the public internet.
- Provide “enterprise-grade” workload identity and **automatic key rotation** (avoid manual certificate handling).
- Keep future **WS Hub split** easy (API and Hub can separate without breaking service connectivity). fileciteturn2file4

## Decision

### 1) Internet edge (Browser UI ↔ API)

**Use a reverse proxy / gateway as the only public entrypoint**:

- Expose **only port 443** publicly.
- Enforce **HTTPS + WSS only** (no plaintext HTTP/WS).
- Apply gateway controls: **HSTS**, **rate limiting**, and basic “WAF-ish” request filtering where practical. fileciteturn2file2

**WebSocket hardening (browser)**:

- Validate **Origin** on every WebSocket handshake (allowlist) to reduce cross-site WebSocket abuse patterns. fileciteturn2file2
- Require authentication at handshake (Bearer token) and treat WS connections as short-lived:
  - when access token expires, **close** and **reconnect** with a new token (don’t attempt “refresh within the same socket”). fileciteturn2file2

**Identity**:

- Browser → API uses standard token auth (JWT). Recommended “enterprise SPA” login pattern is **OIDC Authorization Code + PKCE**. fileciteturn2file2
- Note: **browser mTLS** is not considered reliable/practical; we rely on TLS + correct certs + HSTS at the edge.

### 2) Cross-machine backend traffic (Services ↔ WS Hub)

Treat the network as hostile (even if “same LAN”), and design for **public-network safety**:

- Prefer a **private overlay network / VPN** (WireGuard / Tailscale / site-to-site) when traffic may traverse public networks. fileciteturn2file6
- Enforce **mTLS** for service-to-hub connectivity, using **workload identity** with automatic rotation:
  - **SPIFFE/SPIRE** for identity issuance.
  - **Envoy sidecars** in front of the WS Hub and each Service; Envoy terminates/initiates **WSS+mTLS** and handles certificate rotation via **SDS** from SPIRE Agent. fileciteturn2file4

This avoids embedding certificate rotation logic into application code (“how do I refresh certs in .NET service?”), and enables **policy-based authorization** by workload identity (SPIFFE ID). fileciteturn2file4

### 3) Observability + admin tooling exposure

- **Prometheus + OTel Collector**: internal-only (no public ports). fileciteturn2file17
- **Grafana + Seq**: internal-only by default; if remote access is needed, put behind VPN or proxy auth + IP allowlist. fileciteturn2file2
- If OTLP must cross networks, secure the collector with TLS/mTLS (receiver TLS configuration) rather than leaving it open. fileciteturn2file17

### 4) Future-proofing: split WS Hub from API

Design now for a separate WS Hub by introducing stable endpoints:

- Public: `app.example.com` (UI + `/api` routes)
- Internal (VPN-only): `hub.internal` for service connections (`wss://hub.internal/...`) requiring mTLS.
- Services always connect to `hub.internal`; the implementation can move from API-integrated hub to dedicated hub later without changing clients. fileciteturn2file4

## Considered options and why they were not chosen (for now)

1) **Network isolation only (Docker networks + firewall)**  
   - Good baseline on a single host, but insufficient for cross-machine scenarios and provides no workload identity. fileciteturn2file13

2) **VPN only (no mTLS)**  
   - Helps against MITM on public links, but inside the VPN any compromised node can impersonate others without additional identity/auth.

3) **TLS everywhere with an internal CA (manual cert issuance)**  
   - Secure, but operationally heavy without automation; rotation becomes painful. fileciteturn2file18

4) **mTLS with manually managed certs**  
   - Strong identity, but still suffers from bootstrapping and rotation overhead.

5) **Service mesh (Istio/Linkerd/Consul Connect)**  
   - Great for Kubernetes; high operational overhead for “Docker on VMs” early stage. fileciteturn2file13

6) **Application-layer auth between services (OAuth2 Client Credentials / JWT / HMAC / API keys)**  
   - Viable alternative, especially if mTLS is postponed.  
   - Still needs TLS; identity can be weaker than workload-attested certificates, and secret distribution/rotation becomes the team’s responsibility. fileciteturn2file13

## Consequences

### Positive

- Strong defense against MITM across public networks: **TLS at edge + VPN + mTLS**.
- Enterprise-grade workload identity + automatic rotation via SPIRE.
- Reduced blast radius: internal ports not exposed, admin tools protected.
- Clean path to separate WS Hub from API without breaking service clients.

### Negative / costs

- Additional components and operational complexity:
  - Reverse proxy / gateway
  - VPN overlay (if needed)
  - SPIRE Server + SPIRE Agents
  - Envoy sidecars per workload
- Requires careful identity policy and registration management (SPIRE entries/selectors).

## Implementation notes (high level)

### Edge (browser traffic)

- Reverse proxy terminates TLS, redirects 80 → 443 (optional).
- Strict TLS config + HSTS + rate limits.
- Route:
  - `/` → UI
  - `/api` → API
  - Optionally protect `/grafana`, `/seq` behind auth/VPN

### Service-to-hub (cross-machine)

- Service initiates outbound connection to Hub (preferred for NAT/firewalls). fileciteturn2file3
- Use Envoy on both ends:
  - Service app speaks WS to `localhost`
  - Envoy upgrades and transports over **WSS+mTLS**
- SPIRE bootstrap:
  - Start SPIRE Server centrally (often on “hub” machine).
  - Run SPIRE Agent per machine.
  - Use join tokens / node attestation for first install.
  - Define registration entries mapping workloads → SPIFFE IDs.
  - Authorize which SPIFFE IDs may connect to the hub and/or specific hub routes.

### Observability

- Remove public `ports:` from Prometheus/Collector by default.
- Grafana/Seq access only via VPN or via reverse proxy with auth.
- OTLP receivers secured if telemetry is sent cross-network.

## Diagram (target secure communication)

```mermaid
flowchart LR
  subgraph Internet
    U[Browser / User]
  end

  subgraph PublicEdge["Public edge (only 443)"]
    RP[Reverse proxy / Gateway\nTLS termination + HSTS + rate limits]
    UI[Angular UI]
    API[Build Hub API\nREST + WS]
  end

  subgraph PrivateNetwork["Private / VPN overlay (treat as hostile)"]
    HUB["WS Hub<br/>(can be split from API)"]
    ENHUB[Envoy sidecar\nWSS + mTLS]
    SPServer[SPIRE Server]
    Obs[Seq + OTel Collector + Prometheus + Grafana\ninternal-only]
  end

  subgraph ServiceHost["Service machine(s)"]
    SVC[Service workload]
    ENSVC[Envoy sidecar\nWSS + mTLS]
    SPAgent[SPIRE Agent]
  end

  U -->|HTTPS/WSS| RP
  RP --> UI
  RP -->|/api| API

  API -.-> HUB
  HUB --> ENHUB
  ENHUB <--> |mTLS over WSS| ENSVC
  ENSVC --> SVC

  SPServer --- SPAgent
  SPAgent --> ENSVC
  SPServer --> ENHUB

  Obs --- API
  Obs --- HUB
```

## Sequence (service connects to hub with mTLS)

```mermaid
sequenceDiagram
  autonumber
  participant S as Service workload
  participant ES as Envoy (service side)
  participant SA as SPIRE Agent (service host)
  participant EH as Envoy (hub side)
  participant SS as SPIRE Server
  participant H as WS Hub

  SA->>SS: Bootstrap/join (token or attestation)
  SS-->>SA: Trust bundle + identity policy
  SA-->>ES: Issue SVID (cert) via SDS (auto-rotated)

  ES->>EH: Connect (WSS) with mTLS handshake
  EH->>SS: Validate chain / trust bundle (cached/rotated)
  EH-->>ES: mTLS established (workload identity verified)
  ES->>H: Upgrade to WS / forward frames
  H-->>S: WS session established (authenticated by identity + app auth where needed)
```

## Operational checklist (minimum)

- [ ] Only gateway ports exposed publicly; internal services have no host-published ports. fileciteturn2file8
- [ ] HTTPS/WSS only; HSTS enabled; rate limits configured. fileciteturn2file2
- [ ] Origin allowlist for browser WS.
- [ ] Services connect outbound to hub; hub protected by mTLS (SPIRE + Envoy). fileciteturn2file3
- [ ] Grafana/Seq not public; Prometheus/Collector internal-only. fileciteturn2file2
- [ ] Secrets rotated; least privilege between services.

## Open questions / follow-ups

- How/where to run VPN overlay (WireGuard vs Tailscale) when treating “public network” as hostile. fileciteturn2file6
- Define SPIFFE ID naming scheme (`spiffe://buildhub/{env}/{service}` etc.) and authorization policies.
- Decide whether the WS hub should be:
  - VPN-only internal endpoint (preferred), or
  - behind the public gateway with strict mTLS and firewall rules.
