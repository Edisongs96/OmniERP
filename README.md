# OmniERP Orders PoC

> Prueba de Concepto Full Stack para el Módulo de Edición de Pedidos de OmniERP.
> Stack: ASP.NET Core 8 · Angular 18+ · Clean Architecture · Docker

---

## Tabla de contenidos

1. [Contexto del problema](#1-contexto-del-problema)
2. [Solución técnica](#2-solución-técnica)
3. [Stack y arquitectura](#3-stack-y-arquitectura)
4. [Ejecución rápida con Docker](#4-ejecución-rápida-con-docker)
5. [Ejecución local sin Docker](#5-ejecución-local-sin-docker)
6. [Endpoints disponibles](#6-endpoints-disponibles)
7. [Validación manual — Problema A (caché)](#7-validación-manual--problema-a-caché)
8. [Validación manual — Problema B (concurrencia)](#8-validación-manual--problema-b-concurrencia)
9. [Pruebas automatizadas](#9-pruebas-automatizadas)
10. [Estructura del repositorio](#10-estructura-del-repositorio)
11. [Decisiones técnicas](#11-decisiones-técnicas)

---

## 1. Contexto del problema

### Problema A — Cuello de botella en catálogos

El formulario de edición de pedidos necesita cargar catálogos de valores de referencia: estados de pedido y métodos de envío. Estos valores cambian raramente, pero cada apertura del formulario dispara una lectura a la base de datos que tarda aproximadamente 2 segundos.

Con múltiples agentes trabajando simultáneamente, este patrón satura la red y la base de datos con lecturas idénticas y repetidas para datos que prácticamente no cambian.

**Síntoma observable:** la primera carga del formulario tarda ~2 s. Las siguientes deberían ser instantáneas pero no lo son si no hay caché.

### Problema B — Pérdida silenciosa de datos por concurrencia

Dos agentes abren el mismo pedido al mismo tiempo. Ambos ven la versión 1. El Agente 1 guarda un cambio en la dirección de entrega y el sistema incrementa la versión a 2. El Agente 2, sin recargar, guarda un cambio en el comentario interno con la versión 1 obsoleta.

Sin protección de concurrencia, el sistema aplica el cambio del Agente 2, sobrescribiendo silenciosamente la dirección del Agente 1. El dato queda incorrecto sin que ningún agente lo sepa.

**Síntoma observable:** un agente puede perder cambios guardados sin ningún aviso de error.

---

## 2. Solución técnica

### Solución A — Caché backend con IMemoryCache

El caso de uso `GetOrderFormCatalogsUseCase` implementa el patrón cache-aside a través del puerto `ICacheProvider`:

- **Primera petición:** consulta la fuente lenta (simulación de ~2 s), almacena el resultado en `IMemoryCache` con TTL de 60 minutos.
- **Peticiones siguientes:** devuelve el resultado en caché en menos de 50 ms.
- **Metadata:** cada respuesta incluye `source` (`cache` o `slow-source`), `durationMs` y `cacheKey` para que el frontend pueda mostrar el estado al agente.
- **Evolución futura:** el `ICacheProvider` permite cambiar a Redis sin tocar la capa de Application.

### Solución B — Concurrencia optimista con campo `version`

Cada pedido tiene un campo `version` entero que se incrementa con cada actualización exitosa:

- El frontend envía la versión que tiene en pantalla junto con cada `PUT /orders/{id}`.
- El backend compara la versión enviada con la almacenada. Si coinciden, aplica el cambio e incrementa la versión. Si no coinciden, devuelve `409 Conflict` con el código `ORDER_CONCURRENCY_CONFLICT` y un snapshot del pedido actual.
- El frontend muestra el `ConflictDialogComponent` con una tabla comparativa campo a campo. El agente puede elegir recargar el pedido actual o copiar su comentario intentado para aplicarlo manualmente. No hay merge automático.

---

## 3. Stack y arquitectura

**Backend**
- ASP.NET Core 8 Web API
- Clean Architecture (Api / Application / Domain / Infrastructure)
- EF Core InMemory (PoC) — reemplazable por SQLite o SQL Server
- IMemoryCache con abstracción `ICacheProvider` — reemplazable por Redis
- xUnit

**Frontend**
- Angular 18+ con standalone components
- Angular Signals para estado local de la feature
- Reactive Forms con validación
- Arquitectura por features bajo `features/orders`

**Infraestructura**
- Docker + docker-compose
- nginx sirviendo el build de producción de Angular
- Proxy `/api/` en nginx para comunicación interna entre contenedores

Los diagramas de arquitectura, flujo de concurrencia y estrategia de caché están disponibles en la carpeta `docs/`.

---

## 4. Ejecución rápida con Docker

### Prerequisitos

- Docker Desktop instalado y corriendo
- Puertos 5000 y 4200 disponibles

### Comandos

```bash
# Clonar el repositorio
git clone <URL_DEL_REPOSITORIO>
cd omnierp-orders-poc

# Construir y levantar todos los servicios
docker-compose up --build

# En segundo plano
docker-compose up --build -d
```

El frontend espera a que el backend supere el health check antes de iniciar.

### URLs

| Servicio | URL |
|----------|-----|
| Frontend | http://localhost:4200 |
| Pedido de prueba | http://localhost:4200/orders/1001 |
| Backend API | http://localhost:5000 |
| Swagger UI | http://localhost:5000/swagger |
| Health check | http://localhost:5000/api/v1/health |

### Detener servicios

```bash
docker-compose down
```

---

## 5. Ejecución local sin Docker

### Prerequisitos

- .NET 8 SDK
- Node.js 18+ y npm
- Angular CLI 18+ (`npm install -g @angular/cli`)

### Backend

```bash
cd backend
dotnet restore OmniERP.sln
dotnet run --project src/OmniERP.Api
```

La API queda disponible en `http://localhost:5000`. Swagger en `http://localhost:5000/swagger`.

### Frontend

```bash
cd frontend
npm install
npm start
```

La aplicación queda disponible en `http://localhost:4200`.

---

## 6. Endpoints disponibles

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/v1/health` | Health check |
| GET | `/api/v1/orders/{id}` | Obtener pedido por ID |
| PUT | `/api/v1/orders/{id}` | Actualizar pedido con control de concurrencia |
| GET | `/api/v1/catalogs/order-form` | Catálogos con metadata de caché |
| POST | `/api/v1/catalogs/cache/invalidate` | Invalidar caché de catálogos |

Ver ejemplos completos con cuerpos de petición en `docs/api.http`.

---

## 7. Validación manual — Problema A (caché)

1. Abre `http://localhost:4200/orders/1001`.
2. Observa el badge de catálogos: **"Catálogos desde fuente lenta simulada"** — primera carga ~2 s.
3. Recarga la página (F5).
4. Observa el badge: **"Catálogos desde caché ✓"** — carga en menos de 50 ms.
5. Para resetear la caché y repetir la demostración:

```bash
curl -X POST http://localhost:5000/api/v1/catalogs/cache/invalidate
```

---

## 8. Validación manual — Problema B (concurrencia)

1. Abre `http://localhost:4200/orders/1001` en **dos pestañas** del navegador.
2. **Pestaña A**: modifica la dirección de entrega → Guardar.
3. **Pestaña B**: (sin recargar) modifica el comentario interno → Guardar.
4. **Resultado esperado en Pestaña B**: aparece el **ConflictDialog** con:
   - Título "⚠️ Conflicto de actualización detectado"
   - Tabla comparativa campo a campo con la fila "Dirección de entrega" marcada como **Diferente**
   - Tres opciones: Recargar pedido actual / Copiar comentario intentado / Cerrar
5. Verifica en Swagger (`GET /api/v1/orders/1001`) que la dirección de la Pestaña A **no fue sobrescrita**.

---

## 9. Pruebas automatizadas

### Backend

```bash
cd backend
dotnet test OmniERP.sln
```

Resultado esperado: **28 pruebas — 28 passed**

| Proyecto | Pruebas |
|----------|---------|
| OmniERP.Application.Tests | 14 |
| OmniERP.Infrastructure.Tests | 7 |
| OmniERP.Api.Tests | 7 |

### Frontend

```bash
cd frontend
npm test -- --watch=false
```

Resultado esperado: **16 pruebas — 16 SUCCESS**

---

## 10. Estructura del repositorio

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
|   +-- Dockerfile
|
+-- frontend/
|   +-- src/
|   |   +-- app/
|   |   |   +-- features/
|   |   |   |   +-- orders/
|   |   |   |       +-- pages/
|   |   |   |       +-- components/
|   |   |   |       +-- services/
|   |   |   |       +-- models/
|   |   |   |       +-- state/
|   |   |   |       +-- testing/
|   |   |   +-- app.routes.ts
|   |   +-- environments/
|   |   +-- styles.scss
|   |
|   +-- angular.json
|   +-- package.json
|   +-- Dockerfile
|   +-- nginx.conf
|
+-- docs/
|   +-- architecture.mmd
|   +-- concurrency-flow.mmd
|   +-- cache-strategy.mmd
|   +-- api.http
|
+-- DECISIONS.md
+-- README.md
+-- AGENTS.md
+-- docker-compose.yml
+-- .gitignore
```

---

## 11. Decisiones técnicas

Ver [DECISIONS.md](./DECISIONS.md) para el análisis completo de:

- **Problema A:** Estrategia de caché con `ICacheProvider` y `IMemoryCache`
- **Problema B:** Control de concurrencia optimista con campo `version`
- **UX del conflicto:** Por qué no hay merge automático en el frontend
- **Arquitectura:** Clean Architecture, separación de capas, PoC vs. producción

---

## Diagramas

Los diagramas Mermaid están disponibles en `docs/`:

| Archivo | Descripción |
|---------|-------------|
| `architecture.mmd` | Arquitectura general — dependencias entre capas (frontend, API, Application, Domain, Infrastructure) |
| `concurrency-flow.mmd` | Flujo secuencial del conflicto de concurrencia entre dos agentes |
| `cache-strategy.mmd` | Estrategia cache-aside: miss, hit, invalidación |
