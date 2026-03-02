# Development setup

This repo contains:
- ASP.NET Core API (net10.0)
- Angular frontend (Angular 21.1.1)
- Shared libs (Common, DataEngine, CommandBuilder)
- SQL projects

## Prerequisites

- .NET SDK compatible with `net10.0`
- Node.js + npm
- Docker (optional, for Seq)

## Run API

```bash
dotnet run --project API
```

In Development:
- Scalar API docs: `/api-docs`
- OpenAPI endpoint: exposed via `MapOpenApi()`

## Run Frontend

```bash
cd Frontend
npm install
npm run start
```

## Run tests

```bash
dotnet test UnitTests
```

## Local logging (Seq)

1. Copy `.env.example` → `.env`
2. Start Seq:

```bash
docker compose up -d
```

Then verify `SeqServerUrl` points to your Seq instance.
