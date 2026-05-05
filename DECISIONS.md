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

Application note:

- `UpdateOrderUseCase` orchestrates repository loading and saving.
- The version comparison and mutation rule live in `Order.Update(...)` in the Domain layer.
- On conflict, Application returns an `UpdateOrderResult` with `ORDER_CONCURRENCY_CONFLICT` data and does not save.

## 7. Application Cache-Aside Use Case

Decision: `GetOrderCatalogsUseCase` implements cache-aside through `ICacheProvider`.

Expected behavior:

- Read `CacheKeys.OrderFormCatalogs` first.
- On cache hit, return cached catalogs with metadata source `cache`.
- On cache miss, load catalogs through `ICatalogRepository`, store them with a 24 hour TTL, and return metadata source `slow-source`.
- Application does not know `IMemoryCache`, Redis, EF Core, or infrastructure details.

## 8. Open Decisions

- Exact API response contract for concurrency conflicts.
- Whether the 24 hour catalog cache TTL remains fixed or becomes runtime configuration.
- Frontend conflict resolution UX details.

## 9. Infrastructure Adapters

Decision: use EF Core InMemory as the local persistence adapter for the PoC.

Reason:

- Keeps local execution lightweight.
- Allows repository behavior and seed data to be tested without external services.
- Can be replaced by SQLite or a production database without changing Application use cases.

Decision: implement `ICacheProvider` with `IMemoryCache` for the PoC.

Reason:

- Demonstrates backend cache behavior with minimal operational overhead.
- Keeps Redis as a future adapter behind the same Application port.

Decision: catalog repositories delegate to `SlowCatalogSource`.

Reason:

- Keeps catalog latency simulation in Infrastructure.
- Keeps cache policy in `GetOrderCatalogsUseCase`, not in repositories.

## 10. REST API Layer

Decision: keep API controllers thin.

Reason:

- Controllers validate HTTP input shape and delegate to Application use cases.
- Application results are mapped to HTTP responses in the API layer.
- Domain entities are not exposed directly as HTTP contracts.

Decision: represent expected optimistic concurrency failures as HTTP `409 Conflict`.

Reason:

- A stale order version is a business conflict, not an unexpected server failure.
- `UpdateOrderUseCase` returns conflict data and the API maps it to `409 Conflict`.
- Unexpected failures are handled by a small global exception middleware returning a clean `500` response.
