# BuildHub

![C#](https://img.shields.io/badge/C%23-512BD4?logo=csharp&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-net10.0-512BD4?logo=dotnet&logoColor=white)
![Angular](https://img.shields.io/badge/Angular-21.1.1-DD0031?logo=angular&logoColor=white)
![TypeScript](https://img.shields.io/badge/TypeScript-3178C6?logo=typescript&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?logo=microsoftsqlserver&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-2496ED?logo=docker&logoColor=white)

BuildHub is a self-hosted build orchestration platform (UI + API + services) aimed at managing **build pipelines** and executing them on one or more **agents**.
At its core, BuildHub models builds as an **Execution Plan** made of ordered **Execution Steps**. Steps are generated via the **CommandBuilder** library.

> Status: early-stage foundation. Some command builders are implemented, others are stubs.

## Docs

- [Docs index](docs/README.md)
- [Architecture](docs/architecture.md)
- [Communication](docs/communication.md)
- [Planned services](docs/services.md)
- [Feature catalog (considered)](docs/feature-catalog.md)
- [Pipeline (Execution Plans & Steps)](docs/pipeline.md)
- [CommandBuilder](docs/command-builder.md)
- [DataEngine](docs/data-engine.md)
- [Observability](docs/observability.md)
- [Authentication roadmap](docs/authentication.md)
- [Development setup](docs/development.md)

## Repository layout

- [`Frontend/`](Frontend/README.md) — Angular 21.1.1 app (PrimeNG 21.0.4, NgRx 21.0.1)
- [`API/`](API/README.md) — ASP.NET Core (net10.0) API (OpenAPI + Scalar UI)
- `Common/` — shared utilities (configuration, logging via Serilog)
- [`DataEngine/`](DataEngine/README.md) — data-access library (connection pooling, query builder, transactions)
- [`CommandBuilder/`](CommandBuilder/README.md) — generates execution steps (Git, TFS, MSBuild, IncrediBuild, VisualStudio, …)
- `Databases/` — SQL projects (Core, Users, IntegrationTests)
- `Infrastructure/` — infra glue (currently minimal)
- `UnitTests/` — unit tests across modules

## Quick start (dev)

### Seq (logs)

There is a `docker-compose.yaml` with a Seq service.

1. Copy `.env.example` to `.env` and set values.
2. Start Seq:

```bash
docker compose up -d
```

Then configure your app to send logs to Seq (see [Observability](docs/observability.md)).

### API

From repo root:

```bash
dotnet build BuildHub.sln
dotnet run --project API
```

In **Development**, the API exposes:
- OpenAPI JSON via `MapOpenApi()`
- Scalar UI at `/api-docs` (configured in `API/Startup.cs`)

### Frontend

```bash
cd Frontend
npm install
npm run start
```

## Roadmap highlights

- **Auth (current):** token-based auth (simple access + refresh tokens)
- **Auth (future):** OAuth and/or separate auth service
- **Observability:** OpenTelemetry across services, an OTel Collector container, Prometheus + Grafana; considering Jaeger and Tempo for trace following


## Docs (additional)
- [Communication](./docs/communication.md)
- [Planned services](./docs/services.md)
- [Feature catalog (considered)](./docs/feature-catalog.md)
