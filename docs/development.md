# BuildHub Development Setup

This guide focuses on:
- the project structure and local prerequisites
- direct local run commands
- the files used for Docker-based development
- the `.env` variables and what each one means
- the main Docker commands to build and run the different combinations

## Project overview

This repository contains:
- an ASP.NET Core API targeting `net10.0`
- an Angular frontend
- shared libraries such as `Common`, `DataEngine`, and `CommandBuilder`
- SQL database projects

## Prerequisites

For local development, install:
- a .NET SDK compatible with `net10.0`
- Node.js and npm
- Docker Desktop or Docker Engine if you want to use the containerized setup

## Direct local run commands

### Run the API locally
```bash
dotnet run --project API
```

In Development, the API exposes:
- Scalar API docs at `/api-docs`
- the OpenAPI endpoint through `MapOpenApi()`

### Run the frontend locally
```bash
cd Frontend
npm install
npm run start
```

### Run tests
```bash
dotnet test UnitTests
```

---


## Compose files

### `docker-compose.yml`
Main app stack:
- `sql-server`
- `db-init`
- `api`
- `frontend`

### `docker-compose.observability.yml`
Observability stack:
- `seq`
- `otel-collector`
- `prometheus`
- `grafana`

### `docker-compose.prod.yml`
Production/release override.

---

## `.env` variables

Example:

```env
BUILD_CONFIGURATION=Release
ASPNETCORE_ENVIRONMENT=Development

MSSQL_IMAGE_TAG=2025-latest
MSSQL_PORT=1433
MSSQL_SA_PASSWORD=test@mssql-2025

BUILDHUB_CORE_DB=BuildHubCore
BUILDHUB_USERS_DB=BuildHubUsers
BUILDHUB_TEST_DB=BuildHubIntegrationTests

API_PORT=8080
FRONTEND_PORT=4200

SEQ_IMAGE_TAG=latest
SEQ_PORT=5341
SEQ_PASSWORD=ChangeThisSeqAdminPassword123!

OTEL_COLLECTOR_VERSION=0.147.0
OTEL_GRPC_PORT=4317
OTEL_HTTP_PORT=4318
OTEL_METRICS_PORT=8889

PROMETHEUS_VERSION=v3.4.2
PROMETHEUS_PORT=9090

GRAFANA_VERSION=latest
GRAFANA_PORT=3000
GRAFANA_ADMIN_USER=admin
GRAFANA_ADMIN_PASSWORD=ChangeThisGrafanaPassword123!

SEQ_SERVER_URL=http://seq:80
OTEL_EXPORTER_OTLP_ENDPOINT=http://otel-collector:4317
OTEL_EXPORTER_OTLP_PROTOCOL=grpc

API_IMAGE=buildhub-api:latest
FRONTEND_IMAGE=buildhub-frontend:latest
```

### Variable descriptions

#### Build / app mode
- `BUILD_CONFIGURATION` — .NET build configuration, usually `Release` or `Debug`.
- `ASPNETCORE_ENVIRONMENT` — ASP.NET Core environment, usually `Development` or `Production`.

#### SQL Server
- `MSSQL_IMAGE_TAG` — SQL Server Docker image tag.
- `MSSQL_PORT` — host port mapped to SQL Server container port `1433`.
- `MSSQL_SA_PASSWORD` — SQL Server `sa` password.

#### Database names
- `BUILDHUB_CORE_DB` — name of the Core database.
- `BUILDHUB_USERS_DB` — name of the Users database.
- `BUILDHUB_TEST_DB` — name of the Integration Tests database.

#### API / frontend ports
- `API_PORT` — host port for the API container.
- `FRONTEND_PORT` — host port for the frontend container.

#### Seq
- `SEQ_IMAGE_TAG` — Seq Docker image tag.
- `SEQ_PORT` — host port for Seq UI.
- `SEQ_PASSWORD` — Seq admin password on first start.

#### OpenTelemetry Collector
- `OTEL_COLLECTOR_VERSION` — OTel Collector image version.
- `OTEL_GRPC_PORT` — host port for OTLP gRPC.
- `OTEL_HTTP_PORT` — host port for OTLP HTTP.
- `OTEL_METRICS_PORT` — host port for Prometheus scraping of collector metrics.

#### Prometheus
- `PROMETHEUS_VERSION` — Prometheus image version.
- `PROMETHEUS_PORT` — host port for Prometheus UI.

#### Grafana
- `GRAFANA_VERSION` — Grafana image version.
- `GRAFANA_PORT` — host port for Grafana UI.
- `GRAFANA_ADMIN_USER` — Grafana admin username.
- `GRAFANA_ADMIN_PASSWORD` — Grafana admin password.

#### App observability endpoints
- `SEQ_SERVER_URL` — Seq URL used by the API logging configuration.
- `OTEL_EXPORTER_OTLP_ENDPOINT` — OTel endpoint used by the API.
- `OTEL_EXPORTER_OTLP_PROTOCOL` — OTel protocol, usually `grpc`.

#### Production image tags
- `API_IMAGE` — prebuilt API image tag for production/release runs.
- `FRONTEND_IMAGE` — prebuilt frontend image tag for production/release runs.

---

## Main URLs

When running locally:

- API: `http://localhost:8080`
- Frontend: `http://localhost:4200`
- SQL Server: `localhost,1433`

With observability:
- Seq: `http://localhost:5341`
- Prometheus: `http://localhost:9090`
- Grafana: `http://localhost:3000`

---

## Main build commands

### Build API only
```bash
docker compose build api
```

### Build frontend only
```bash
docker compose build frontend
```

### Build DB init image only
```bash
docker compose build db-init
```

### Build app stack
```bash
docker compose --profile frontend --profile db build
```

### Build app + observability
```bash
docker compose -f docker-compose.yml -f docker-compose.observability.yml --profile frontend --profile db build
```

### Force full rebuild without cache
```bash
docker compose -f docker-compose.yml -f docker-compose.observability.yml --profile frontend --profile db build --no-cache
```

---

## Main run commands

### 1) API only
Runs:
- `sql-server`
- `api`

```bash
docker compose --profile api up --build
```

### 2) API + frontend
Runs:
- `sql-server`
- `api`
- `frontend`

```bash
docker compose --profile frontend up --build
```

### 3) DB init only
Runs:
- `sql-server`
- `db-init`

```bash
docker compose --profile db up --build db-init
```

### 4) API + DB init
Runs:
- `sql-server`
- `db-init`
- `api`

```bash
docker compose --profile api --profile db up --build
```

### 5) API + frontend + DB init
Runs:
- `sql-server`
- `db-init`
- `api`
- `frontend`

```bash
docker compose --profile frontend --profile db up --build
```

### 6) Observability only
Runs:
- `seq`
- `otel-collector`
- `prometheus`
- `grafana`

```bash
docker compose -f docker-compose.observability.yml up --build
```

### 7) API + observability
Runs:
- `sql-server`
- `api`
- `seq`
- `otel-collector`
- `prometheus`
- `grafana`

```bash
docker compose -f docker-compose.yml -f docker-compose.observability.yml --profile api up --build
```

### 8) API + frontend + observability
Runs:
- `sql-server`
- `api`
- `frontend`
- `seq`
- `otel-collector`
- `prometheus`
- `grafana`

```bash
docker compose -f docker-compose.yml -f docker-compose.observability.yml --profile frontend up --build
```

### 9) Full local stack
Runs:
- `sql-server`
- `db-init`
- `api`
- `frontend`
- `seq`
- `otel-collector`
- `prometheus`
- `grafana`

```bash
docker compose -f docker-compose.yml -f docker-compose.observability.yml --profile frontend --profile db up --build
```

---

## Same commands in detached mode

Add `-d` at the end.

Example:

```bash
docker compose -f docker-compose.yml -f docker-compose.observability.yml --profile frontend --profile db up --build -d
```

---

## Stop commands

### Stop current stack
```bash
docker compose down
```

### Stop app + observability stack
```bash
docker compose -f docker-compose.yml -f docker-compose.observability.yml down
```

### Stop and remove volumes too
```bash
docker compose down -v
```

Be careful with `-v` because it removes persisted data.

---

## Production-like run

```bash
docker compose -f docker-compose.yml -f docker-compose.prod.yml --env-file .env.prod up -d
```

With observability:

```bash
docker compose -f docker-compose.yml -f docker-compose.prod.yml -f docker-compose.observability.yml --env-file .env.prod up -d
```

---

## Recommended common commands

### Fast local app run
```bash
docker compose --profile frontend up --build
```

### Full local stack
```bash
docker compose -f docker-compose.yml -f docker-compose.observability.yml --profile frontend --profile db up --build
```

### DB update only
```bash
docker compose --profile db up --build db-init
```
