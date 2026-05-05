# DECISIONS.md - OmniERP Orders PoC

## 1. Context

OmniERP Orders PoC is focused only on the order editing module. The solution must demonstrate how to reduce repeated catalog reads and how to protect order data during concurrent edits.

## 2. Architectural Baseline

Decision: use Clean Architecture in the backend.

Reason:

- Keep domain rules independent from frameworks.
- Keep application use cases independent from persistence and cache details.
- Allow infrastructure adapters such as EF Core, IMemoryCache, and future Redis to change without rewriting use cases.

## 3. Backend Project Roles

- `OmniERP.Domain`: domain entities, value objects, and domain exceptions.
- `OmniERP.Application`: use cases, DTOs, commands, queries, and ports.
- `OmniERP.Infrastructure`: persistence, repositories, cache adapters, seed data, and simulators.
- `OmniERP.Api`: HTTP controllers, middleware, dependency injection, and host configuration.

## 4. Frontend Baseline

Decision: use Angular 18+ with standalone components and feature-based structure under `features/orders`.

Reason:

- Keep the order editing feature isolated.
- Use Reactive Forms for edit workflows.
- Use Signals for local loading, saving, error, and conflict state.

## 5. Cache Strategy

Decision: use cache-aside through an `ICacheProvider` abstraction.

PoC adapter: `IMemoryCache`.

Future production option: Redis adapter through the same application port.

## 6. Concurrency Strategy

Decision: use optimistic concurrency with an order version value.

Expected behavior:

- Matching version: update succeeds and increments version.
- Stale version: update is rejected with HTTP 409 Conflict.

## 7. Open Decisions

- EF Core InMemory vs SQLite for local persistence.
- Exact API response contract for concurrency conflicts.
- Cache TTL value for order form catalogs.
- Frontend conflict resolution UX details.
