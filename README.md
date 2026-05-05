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

This repository currently contains the base monorepo structure only. Business logic, domain model, API endpoints, persistence, cache implementation, and UI workflows will be added in later steps.

## Main Folders

- `backend/`: .NET solution, source projects, and test projects.
- `frontend/`: Angular 18+ application.
- `docs/`: Mermaid diagrams and manual API request file.

## Next Build Steps

1. Define backend application ports and DTOs for Orders and Catalogs.
2. Add domain model and optimistic concurrency rules.
3. Implement infrastructure adapters for persistence and cache.
4. Expose API endpoints.
5. Build the Angular order edit feature.
6. Add tests for cache behavior, concurrency conflicts, and API mappings.
