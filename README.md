# OmniERP Orders PoC

Proof of concept full stack for the **Order Editing Module** of OmniERP.

The project is intentionally scoped to two production-style concerns:

- Backend caching for rarely changing order form catalogs.
- Optimistic concurrency control to prevent silent data loss during concurrent order edits.

## Stack

- Backend: ASP.NET Core Web API, C#, Clean Architecture, xUnit.
- Frontend: Angular 18+, standalone components, Reactive Forms, Signals.
- Persistence target: EF Core InMemory or SQLite.
- Cache target: IMemoryCache behind an `ICacheProvider` port.

## Current State

This repository currently contains the base monorepo structure, domain model, Application DTOs, Application ports, Application use cases, Infrastructure adapters, and REST API endpoints for the order editing PoC.

UI workflows will be added in later steps.

## Main Folders

- `backend/`: .NET solution, source projects, and test projects.
- `frontend/`: Angular 18+ application.
- `docs/`: Mermaid diagrams and manual API request file.

## Infrastructure

- Local persistence uses EF Core InMemory for the PoC.
- Initial seed creates demo order `1001`.
- Catalog reads use a slow source simulator.
- Runtime cache uses `IMemoryCache` behind the Application `ICacheProvider` port.

## Run The API

```powershell
dotnet run --project backend/src/OmniERP.Api
```

Local URLs:

- API base URL: `http://localhost:5000`
- Swagger UI: `http://localhost:5000/swagger`

## REST Endpoints

- `GET /api/v1/health`
- `GET /api/v1/orders/{id}`
- `PUT /api/v1/orders/{id}`
- `GET /api/v1/catalogs/order-form`
- `POST /api/v1/catalogs/cache/invalidate`

## Manual Validation

- Demo order: `GET http://localhost:5000/api/v1/orders/1001`
- First catalog call: `GET http://localhost:5000/api/v1/catalogs/order-form` returns metadata source `slow-source`.
- Second catalog call returns metadata source `cache`.
- Cache reset: `POST http://localhost:5000/api/v1/catalogs/cache/invalidate`.
- Concurrency conflict: send `PUT /api/v1/orders/1001` twice with the same stale `version`; the second request returns `409 Conflict`.

Manual request samples are available in `docs/api.http`.

## Test

```powershell
dotnet test backend/OmniERP.sln
```

## Next Build Steps

1. Build the Angular order edit feature.
2. Add frontend integration against the REST API.
3. Add final README walkthrough and screenshots.
