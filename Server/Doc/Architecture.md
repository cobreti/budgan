# Budgan Architecture

## Overview

Budgan is an ASP.NET Core Web API (target framework `net10.0`) for personal finance/budget tracking (accounts, account balances, and CSV column-mapping configuration for statement imports). The solution is split into four class libraries plus a web host, following a layered architecture with a strict, one-directional dependency chain:

```
BudganGlobal  <--  BudganInfra  <--  BudganServices  <--  BudganSvr
                        ^
                        |
                  BudganInfra_Test
```

| Project | Type | Purpose |
|---|---|---|
| `BudganGlobal` | class library | Cross-cutting primitives shared by every other layer: result/error types. No dependencies of its own. |
| `BudganInfra` | class library | Data access layer: EF Core `DataContext`, entity classes, migrations, and a repository layer. Depends on `BudganGlobal`. |
| `BudganServices` | class library | Application/use-case layer: one class per business operation. Depends on `BudganInfra` (and transitively `BudganGlobal`). |
| `BudganSvr` | ASP.NET Core Web API (`Sdk="Microsoft.NET.Sdk.Web"`) | HTTP host: controllers, DTOs, app bootstrap. Depends on `BudganServices` and `BudganInfra`. |
| `BudganInfra_Test` | xUnit test project | Unit tests for `BudganInfra` repository operations, using EF Core's InMemory provider. |

## Request flow

Each feature follows the same four-tier flow, with a distinct model type translated at every boundary:

```
HTTP request
   -> Controller (BudganSvr)          — API DTO
   -> UseCase Factory (BudganServices) — builds the use case
   -> UseCase (BudganServices)         — BO (business object)
   -> Repository Operation (BudganInfra) — DAO (data object) -> EF Core -> SQL Server
```

Example (the only fully-wired feature today, `ColumnsMapping`):

1. `ColumnsMappingController` (`BudganSvr/Api/Controllers/ColumnsMapping/ColumnsMappingController.cs`) receives the HTTP request and an API-layer DTO (e.g. `AddOrUpdateColumnsMapping`).
2. It resolves `IColumnsMappingUseCaseFactory` (`BudganServices`) and asks it for the relevant use case (e.g. `AddOrUpdateColumnsMappingUseCase`).
3. The use case maps the API DTO to a business object (`BOAddOrUpdateColumnsMapping`), then to a DAO (`DaoSaveColumnsMapping`), and calls `IColumnsMappingRepository`.
4. The repository (`BudganInfra/Repositories/ColumnsMapping`) returns a per-operation object (e.g. `SaveColumnsMappingRepoOp`) whose `ExecuteAsync()` performs an EF Core add-or-update against `DataContext`.
5. Results flow back up as `Succeeded` / `ResultValue` / error, and the controller wraps the outcome in `ApiSuccessResult<T>` / `ApiErrorResult<E>` (`BudganSvr/Api/Types/ApiResult.cs`).

This is a **command/operation pattern**, not a generic `IRepository<T>`: there is no single repository interface with CRUD methods — each operation (Save, Get, GetList, Delete, ...) is its own class implementing `IRepositoryOperation` / `IRepositoryOperationWithResultValue<T>`, built by a thin per-entity repository facade (e.g. `AccountRepository`, `ColumnsMappingRepository`).

## Layer details

### BudganGlobal — shared primitives

- `MResult<T>` (`MResult.cs`) — a `Success`/`Failure` result struct carrying either a `Value` or a `BudganErrorValue`.
- `Errors/BudganErrorValue.cs` and `Errors/Exceptions/` — the shared error model and `BudganException` used to signal domain failures (e.g. not-found, concurrency conflicts) up through the layers.

### BudganInfra — data access

**Entities** (`DBContext/Tables/`), all but `UserAccount` extend `BaseEntity` (`Id: Guid` PK + `Timestamp: DateTime`, used as an optimistic-concurrency token):

- `ColumnsMapping` — describes how CSV/import columns map to fields (index + header text for card number, date, amount, description).
- `Account` — a budget account (`Name`, `AccountType`, required FK to `ColumnsMapping`).
- `UserAccount` — a lightweight standalone entity (own `Id`/`Key`, no `Timestamp`), not yet configured in `OnModelCreating` and not yet wired into any use case (stub repository only).

**`DataContext`** (`DBContext/DataContext.cs`) exposes `DbSet`s for all three entities. `OnModelCreating` configures `Timestamp` to default to `GETUTCDATE()` for `ColumnsMapping` and `Account`.

**Repositories** (`Repositories/`) — per-entity facades (`AccountRepository`, `ColumnsMappingRepository`, `UserAccountRepository`, each `internal` behind a public `I*Repository` interface) that construct per-operation classes:
- `BaseRepositoryOperation` / `BaseRepositoryOperationWithResultValue<T>` — shared base classes providing `Succeeded`, error state, and `ValidateCanPerformUpdate` (compares the incoming DAO's `Timestamp` against the persisted entity's `Timestamp` to detect concurrent edits, throwing `BudganException` on mismatch or not-found).
- Concrete operations live under `Repositories/<Entity>/<Operation>/`, e.g. `Repositories/ColumnsMapping/Save/SaveColumnsMappingRepoOp.cs`. `ColumnsMapping` has full CRUD (Save/Get/GetList/Delete); `Account` currently only has Save; `UserAccount` has no operations yet.
- DAO models live in `Repositories/Models` (`DaoBaseUpdateModel` with `Id`/`Timestamp`) and per-feature Dao classes (`DaoSaveAccount`, `DaoSaveColumnsMapping`, etc.).

**`ServiceCollectionExtensions.AddInfrastructure(services, configuration)`** registers `DataContext` via `AddDbContext` + `UseSqlServer(configuration.GetConnectionString("DefaultConnection"))` (with `MigrationsAssembly` pinned to `BudganInfra`), and registers `IUserAccountRepository`, `IColumnsMappingRepository`, `IAccountRepository` as `Scoped`.

**Migrations** — a single migration, `InitialCreate` (`BudganInfra/Migrations/`), creating all three tables. It is regenerated in place (rather than accumulating incremental migrations) while the schema is still being actively shaped.

### BudganServices — use cases

Structure: `UseCases/<DomainConcept>/<Operation>/`, one class per business operation (e.g. `UseCases/ColumnsMapping/{AddOrUpdate,Delete,Get,GetList}`), following a CQRS-ish command/query split rather than one "service" class per entity.

- `BaseUseCase` / `BaseUseCaseWithResultValue<T>` — shared `Succeeded`/`ResultValue` state, mirroring the repository layer's base classes.
- A per-feature **factory** (e.g. `ColumnsMappingUseCaseFactory`, implementing `IColumnsMappingUseCaseFactory`) is the only thing registered in DI; it takes the relevant repository interface as a constructor dependency and manually `new`s up individual (`internal`) use case instances on demand. Use cases themselves are not individually DI-registered.
- `ServiceCollectionExtensions.AddBudganServices(services)` registers `IColumnsMappingUseCaseFactory -> ColumnsMappingUseCaseFactory` as `Scoped`.

### BudganSvr — API host

- **Bootstrap** (`Program.cs`): builds configuration from `appsettings.json` -> `appsettings.{Environment}.json` -> environment variables; registers `AddControllers`, `AddOpenApi`, `AddEndpointsApiExplorer`, `AddSwaggerGen`, then the custom `AddInfrastructure` and `AddBudganServices` extensions. In `Development` only, it calls `app.Services.ApplyMigrations()` (auto-applies EF migrations) and serves Swagger/OpenAPI UI — production is expected to use migration bundles instead. Pipeline: `UseHttpsRedirection` -> `UseStaticFiles` -> `MapControllers`.
- Supports a `WAIT_FOR_DEBUGGER` env-var gate that busy-waits for a debugger to attach before continuing startup (used for containerized remote debugging).
- **Controllers** (`Api/Controllers/<Feature>/`): only `ColumnsMappingController` exists today, at route `api/ColumnsMapping`, exposing `POST AddOrUpdate`, `GET List`, `GET {id}`, `DELETE {id}`. Per-feature DTOs live alongside it in a `Models/` subfolder (e.g. `AddOrUpdateColumnsMapping`, `GetColumnsMapping`, `ListColumnsMapping`).
- **API envelopes** (`Api/Types/ApiResult.cs`): `ApiResult` (base, `Succeeded` flag), `ApiSuccessResult<T>` (adds `SuccessValue`), `ApiErrorResult<E>` (adds `ErrorValue`) — the uniform response shape every controller action returns.
- **No authentication/authorization is configured** — no `[Authorize]` attributes, no `AddAuthentication`/JWT/cookie setup anywhere in the solution. No CORS policy is configured either. Both are notable gaps if this API is ever exposed beyond local/trusted use.

### BudganInfra_Test — data-layer tests

xUnit project targeting `BudganInfra`, using `Microsoft.EntityFrameworkCore.InMemory` (real EF Core behavior against an in-memory provider, not mocks) and plain `Xunit.Assert` (no Moq/FluentAssertions). Currently covers only `SaveColumnsMappingRepoOp` (add, update, optimistic-concurrency timestamp checks, not-found, invalid id). `Account` and `UserAccount` repository operations have no test coverage yet.

## Local development & deployment

- **Docker**: `BudganSvr/Dockerfile` is a multi-stage build (`base` -> `build` -> `debug` -> `publish` -> `final`), with a `debug` stage that installs `vsdbg` for remote debugging and a `base` stage with `procps` for Rider's debugger attach.
- **`compose.yaml`** (repo root) defines: `budgansvr` (debug profile, builds the `debug` target, sets `WAIT_FOR_DEBUGGER=true`, mounts source + a Rider debugger volume), `budgansvr-release` (release profile, `final` target, `ASPNETCORE_ENVIRONMENT=Production`), and a `shell` helper service. Both API variants expose port 5222.
- **`Database/Start-MssqlContainer.ps1`** — helper script to spin up a local SQL Server container for `DefaultConnection` (`localhost,1433` in `appsettings.Development.json`).
- **No CI/CD** — there is no `.github/workflows` directory; builds, tests, and migrations are run locally/manually today.

## Known gaps / in-progress areas

- `UserAccount` is defined as an entity and has a repository stub, but has no EF configuration, no repository operations, no use cases, and no controller — it is scaffolding, not a working feature yet.
- `Account` has a Save repository operation only (no Get/List/Delete) and no use-case or controller layer yet.
- No authentication, authorization, or CORS configuration.
- No automated CI pipeline; migrations are currently regenerated in place as a single `InitialCreate` migration rather than accumulated incrementally, which is reasonable pre-release but will need to switch to incremental migrations once any environment beyond local dev holds real data.
