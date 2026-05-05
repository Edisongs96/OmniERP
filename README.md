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

This repository currently contains the base monorepo structure, domain model, Application DTOs, Application ports, and Application use cases for order lookup, safe order update, and order form catalog retrieval through cache-aside.

Infrastructure adapters, API endpoints, persistence, concrete cache implementation, and UI workflows will be added in later steps.

## Main Folders

- `backend/`: .NET solution, source projects, and test projects.
- `frontend/`: Angular 18+ application.
- `docs/`: Mermaid diagrams and manual API request file.

## Next Build Steps

1. Implement infrastructure adapters for persistence, repositories, cache, seed data, and catalog simulation.
2. Expose API endpoints and middleware mappings.
3. Build the Angular order edit feature.
4. Add integration tests for API behavior and infrastructure cache behavior.
