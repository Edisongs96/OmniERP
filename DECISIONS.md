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

Implementation note:

- HTTP access is isolated in order and catalog API services.
- The order edit page delegates local state to a Signals store.
- The UI shows functional feedback for `409 Conflict` responses.
- The UI does not apply automatic merges when a stale version is rejected.

## 5. Cache Strategy — Problema A

### Decisión

Usar el patrón Cache-Aside a través de la abstracción `ICacheProvider`.

Adaptador PoC: `IMemoryCache`.
Adaptador de producción futura: Redis a través del mismo puerto.

### Por qué Cache-Aside y no otras estrategias

**Cache-Aside vs. Read-Through/Write-Through:**
Read-Through delega la carga al proveedor de caché, acoplando la política de caché a la infraestructura. Cache-Aside mantiene la política de caché en el caso de uso (`GetOrderCatalogsUseCase`), lo que permite cambiar el proveedor sin modificar la lógica de negocio. La capa Application controla cuándo cachear, por cuánto tiempo y con qué clave.

**Cache-Aside vs. sin caché:**
Sin caché, cada apertura del formulario dispara lecturas idénticas a la fuente lenta (~2 s). Con volumen concurrente, esto satura la red y la base de datos con datos que no cambian. Cache-Aside elimina ese costo en todas las lecturas posteriores a la primera dentro del TTL.

**Cache-Aside vs. caché en el repositorio:**
Cachear en el repositorio viola la Dependency Inversion — Infrastructure sabría cuándo cachear, una decisión que pertenece a Application. Cache-Aside en el caso de uso mantiene la responsabilidad en la capa correcta.

### Cómo se mantiene actualizado en producción

1. **TTL explícito:** `CacheDurations.OrderFormCatalogsTtl` controla la vida de la entrada. Si los catálogos cambian raramente, un TTL de 24 horas con invalidación explícita es suficiente.
2. **Invalidación activa:** `POST /api/v1/catalogs/cache/invalidate` permite que el proceso administrativo invalide la caché al modificar catálogos sin esperar al TTL.
3. **Actualización automática:** al expirar el TTL, el siguiente request recarga desde la fuente y repuebla la caché.

### Cómo escalaría en producción real

Con múltiples instancias del backend (horizontal scaling), `IMemoryCache` es local a cada nodo — cada instancia tiene su propia caché, potencialmente desincronizada. La solución ya está contemplada: registrar `RedisCache` (que implemente `ICacheProvider`) en lugar de `MemoryCacheProvider`. Redis centraliza la caché para todos los nodos. La capa Application no cambia en absoluto: ningún caso de uso ni ningún test de Application necesita modificarse.

## 6. Concurrency Strategy — Problema B

### Decisión

Usar concurrencia optimista con campo `version` entero en la entidad `Order`.

Comportamiento:

- Versión coincide: el update se aplica y el campo `version` se incrementa en 1.
- Versión obsoleta: el update se rechaza con HTTP `409 Conflict` y código `ORDER_CONCURRENCY_CONFLICT`.

Implementación: la comparación y mutación de versión viven en `Order.Update(...)` en el Domain. `UpdateOrderUseCase` orquesta la carga y el guardado. Ante conflicto, devuelve `UpdateOrderResult.ConflictResult` sin persistir nada.

### Por qué optimista y NO pesimista (locks de base de datos)

La concurrencia pesimista bloquea el registro con un lock exclusivo mientras el agente lo edita. En un sistema web con múltiples agentes concurrentes esto genera:

- Colas de espera: si un agente abre el pedido y lo deja sin guardar, el lock bloquea a todos los demás indefinidamente hasta que expire el timeout.
- Riesgo de deadlocks con tablas relacionadas.
- Experiencia de usuario degradada: el sistema aparece lento o bloqueado sin causa aparente.

La concurrencia optimista no bloquea nada. El conflicto es la excepción — en la mayoría de los casos los agentes trabajan en pedidos distintos y nunca hay colisión. Pagar el costo de locking para el caso excepcional no es justificable.

### Por qué optimista y NO "last write wins" (sin protección)

Last-write-wins es exactamente el problema que se reportó en producción: el Agente 2 sobrescribe silenciosamente los cambios del Agente 1 sin ningún aviso. En un pedido con dirección de entrega crítica, eso significa un paquete enviado al destino incorrecto. No hay mecanismo de detección ni notificación. No es una opción aceptable.

### Por qué optimista y NO Event Sourcing

Event Sourcing resuelve la concurrencia almacenando todos los eventos de cambio en lugar del estado actual, permitiendo reconstruir el historial completo. Es la solución más poderosa pero también la más compleja:

- Requiere una arquitectura completamente diferente (event store, proyecciones, handlers).
- Incrementa la complejidad operacional significativamente.
- No es proporcional al alcance de esta PoC, que demuestra un problema de concurrencia puntual.

La concurrencia optimista con versión resuelve el problema con complejidad mínima y es defendible en producción para este caso de uso.

### Limitaciones del enfoque (honestidad técnica)

- **Conflictos de campos independientes:** si el Agente 1 cambia la dirección y el Agente 2 cambia solo el comentario, el sistema reporta conflicto aunque semánticamente no hay colisión. El usuario debe recargar y re-aplicar su cambio. Esta fricción es intencional — es preferible a permitir un merge automático sin supervisión.
- **No hay historial de versiones:** el sistema solo sabe la versión actual. Para auditoría completa se necesitaría un audit log separado.
- **EF Core InMemory:** en la PoC el check de versión ocurre en la capa Domain antes de persistir, no como `WHERE version=N` en SQL. Con una base de datos real, EF Core puede configurarse con `IsRowVersion()` o `IsConcurrencyToken()` para que el check ocurra a nivel de SQL, añadiendo protección adicional contra race conditions extremos.

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

## UX del conflicto de concurrencia

### Decisión adoptada

La interfaz de usuario no realiza merge automático de los cambios en conflicto.

### Justificación

- El backend es la autoridad de integridad del pedido.
- Un merge automático en el frontend podría introducir datos incoherentes
  sin supervisión humana.
- En un entorno empresarial crítico, la decisión sobre qué dato prevalece
  debe tomarse por un usuario, no por el sistema.

### Comportamiento implementado

- Al recibir error 409 ORDER_CONCURRENCY_CONFLICT, el frontend muestra el
  ConflictDialogComponent con la comparación campo a campo.
- Los cambios intentados se conservan en `lastAttemptedChanges` del store.
- El usuario puede elegir: recargar el pedido actual o copiar su comentario
  intentado para aplicarlo manualmente después de recargar.
- El formulario no se limpia automáticamente.

### Trade-offs aceptados

- El flujo requiere una acción manual del usuario, lo que puede percibirse
  como fricción. Sin embargo, esta fricción es intencional para evitar
  pérdida silenciosa de datos.
- En un sistema de mayor escala, podría evaluarse un merge asistido con
  revisión explícita de cada campo en conflicto.
