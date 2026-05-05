# AGENTS.md - OmniERP Orders PoC

## 1. Project Mission

This repository hosts **OmniERP Orders PoC**, a professional full stack proof of concept for the **Order Editing Module** of OmniERP.

The project must stay focused on order editing. Do not expand the scope into a complete ERP.

## 2. Business Problems To Prove

### Problem A - Frequent-read Catalog Bottleneck

The order edit form needs catalogs such as:

- Order statuses.
- Shipping methods.

These values change rarely, but repeated reads can saturate the network and database.

The backend must simulate a slow catalog source that takes approximately 2 seconds on the first request. Subsequent requests for the same catalog data must be served from backend cache in less than 50 ms.

### Problem B - Silent Data Loss From Concurrent Editing

Two agents can open the same order at the same time.

The system must prevent stale saves from silently overwriting newer changes. If a user submits an outdated order version, the backend must detect the collision and the frontend must show clear conflict feedback.

## 3. Required Stack

### Backend

- ASP.NET Core Web API.
- C#.
- Clean Architecture.
- EF Core InMemory or SQLite for local persistence.
- IMemoryCache for the PoC cache implementation.
- `ICacheProvider` abstraction so Redis can be added later without changing application use cases.
- xUnit for backend tests.

### Frontend

- Angular 18 or higher.
- TypeScript.
- Standalone components.
- Reactive Forms.
- Angular Signals for local feature state.
- HttpClient services.
- Explicit loading, saving, error, and conflict states.

## 4. Required Repository Structure

Respect this structure as the project is built:

```text
omnierp-orders-poc/
|
+-- backend/
|   +-- src/
|   |   +-- OmniERP.Api/
|   |   |   +-- Controllers/
|   |   |   +-- Middlewares/
|   |   |   +-- Extensions/
|   |   |   +-- Program.cs
|   |   |   +-- appsettings.json
|   |   |
|   |   +-- OmniERP.Application/
|   |   |   +-- Orders/
|   |   |   |   +-- Commands/
|   |   |   |   +-- Queries/
|   |   |   |   +-- Dtos/
|   |   |   |   +-- UseCases/
|   |   |   +-- Catalogs/
|   |   |   |   +-- Dtos/
|   |   |   |   +-- UseCases/
|   |   |   +-- Common/
|   |   |   +-- Ports/
|   |   |
|   |   +-- OmniERP.Domain/
|   |   |   +-- Orders/
|   |   |   |   +-- Order.cs
|   |   |   |   +-- OrderItem.cs
|   |   |   |   +-- OrderConflictException.cs
|   |   |   +-- Catalogs/
|   |   |   +-- Common/
|   |   |
|   |   +-- OmniERP.Infrastructure/
|   |       +-- Persistence/
|   |       +-- Repositories/
|   |       +-- Cache/
|   |       +-- Seed/
|   |       +-- Simulators/
|   |
|   +-- tests/
|   |   +-- OmniERP.Application.Tests/
|   |   +-- OmniERP.Infrastructure.Tests/
|   |   +-- OmniERP.Api.Tests/
|   |
|   +-- OmniERP.sln
|
+-- frontend/
|   +-- src/
|   |   +-- app/
|   |   |   +-- core/
|   |   |   |   +-- api/
|   |   |   |   +-- interceptors/
|   |   |   |   +-- models/
|   |   |   +-- features/
|   |   |   |   +-- orders/
|   |   |   |       +-- pages/
|   |   |   |       +-- components/
|   |   |   |       +-- services/
|   |   |   |       +-- models/
|   |   |   |       +-- state/
|   |   |   +-- shared/
|   |   |   |   +-- components/
|   |   |   |   +-- utils/
|   |   |   +-- app.routes.ts
|   |   +-- environments/
|   |
|   +-- angular.json
|   +-- package.json
|
+-- docs/
|   +-- architecture.mmd
|   +-- concurrency-flow.mmd
|   +-- cache-strategy.mmd
|   +-- api.http
|
+-- DECISIONS.md
+-- README.md
+-- docker-compose.yml
+-- .gitignore
```

## 5. Architectural Boundaries

- Domain must not depend on Application, Infrastructure, API, Angular, EF Core, or cache libraries.
- Application owns use cases, DTOs, ports, and orchestration.
- Infrastructure implements ports for persistence, cache, seed data, and external-source simulation.
- API exposes HTTP endpoints and maps exceptions to appropriate responses.
- Frontend owns UI state, form validation, user feedback, and HTTP service integration.

## 6. Decisions That Require Confirmation

Ask before changing any of these decisions:

- Replacing Clean Architecture with another backend style.
- Replacing ASP.NET Core Web API or Angular.
- Choosing persistence other than EF Core InMemory or SQLite.
- Removing the `ICacheProvider` abstraction.
- Replacing optimistic concurrency with pessimistic locking.
- Expanding scope beyond the order editing module.
- Adding authentication, authorization, payments, inventory, invoices, or full ERP workflows.

## 7. Expected Technical Proof Points

- First catalog request demonstrates slow-source behavior around 2 seconds.
- Repeated catalog request demonstrates cache behavior under 50 ms.
- Order update succeeds when submitted version matches the stored version.
- Order update returns HTTP 409 when submitted version is stale.
- UI presents clear conflict feedback and does not silently overwrite current order data.
- Tests cover cache behavior, concurrency conflict behavior, and API response mapping.

## 8. Documentation Expectations

- `README.md` must explain local execution, endpoints, demo flow, and test execution.
- `DECISIONS.md` must record relevant architecture decisions and tradeoffs.
- `docs/architecture.mmd` must show the Clean Architecture dependency direction.
- `docs/cache-strategy.mmd` must show cache miss and cache hit behavior.
- `docs/concurrency-flow.mmd` must show stale update detection.
- `docs/api.http` must provide manual HTTP requests for validating the PoC.

## 9. Working Rules

- Keep changes small, defensible, and aligned with the mandatory structure.
- Prefer simple, explicit code over broad abstractions.
- Do not implement business logic while preparing documentation-only scaffolding.
- Do not generate backend or frontend projects until the construction plan is confirmed.
- Keep all work traceable to the two required production problems.
