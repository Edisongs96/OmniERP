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

This repository currently contains the base monorepo structure, domain model, Application DTOs, Application ports, Application use cases, and Infrastructure adapters for local persistence, caching, catalog simulation, and seed data.

API endpoints and UI workflows will be added in later steps.

## Main Folders

- `backend/`: .NET solution, source projects, and test projects.
- `frontend/`: Angular 18+ application.
- `docs/`: Mermaid diagrams and manual API request file.

## Infrastructure

- Local persistence uses EF Core InMemory for the PoC.
- Initial seed creates demo order `1001`.
- Catalog reads use a slow source simulator.
- Runtime cache uses `IMemoryCache` behind the Application `ICacheProvider` port.

## Next Build Steps

1. Expose API endpoints and middleware mappings.
2. Build the Angular order edit feature.
3. Add integration tests for API behavior.
