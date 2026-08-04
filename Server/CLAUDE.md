# Budgan — Claude reference

ASP.NET Core Web API (`net10.0`) for budget/account tracking. Layered solution, strict one-way dependency chain:

```
BudganGlobal <- BudganInfra <- BudganServices <- BudganSvr
                     ^
               BudganInfra_Test
```

- `BudganGlobal` — shared primitives only (`MResult<T>`, `BudganErrorValue`, `BudganException`). No dependencies.
- `BudganInfra` — EF Core data layer: `DataContext`, entities, migrations, repositories. Depends on `BudganGlobal`.
- `BudganServices` — use cases (application/business logic). Depends on `BudganInfra`.
- `BudganSvr` — Web API host: controllers, DTOs, `Program.cs`. Depends on `BudganServices` + `BudganInfra`.
- `BudganInfra_Test` — xUnit tests for `BudganInfra`, EF Core InMemory provider, plain `Xunit.Assert` (no Moq/FluentAssertions).

Full narrative writeup: `Doc/Architecture.md`. Read that for context/rationale; use this file for conventions and commands.

## Feature pattern (follow this for every new endpoint)

```
Controller (BudganSvr)  --API DTO-->
UseCase Factory (BudganServices)   -- builds --> UseCase --BO-->
Repository facade (BudganInfra)    -- builds --> RepoOp --DAO--> DataContext -> SQL Server
```

Three distinct model tiers per feature, never reuse one across layers:
- API DTO: `BudganSvr/Api/Controllers/<Feature>/Models/`
- BO (business object): `BudganServices/UseCases/<Feature>/`
- DAO (data object): `BudganInfra/Repositories/Models/` + `Repositories/<Feature>/`

Only `ColumnsMapping` is fully wired end-to-end today (Save/Get/GetList/Delete) — use it as the reference implementation. `Account` has only a Save repo op, no use case/controller. `UserAccount` is a stub (entity + empty repo, nothing else). Don't assume every entity has all four operations.

### Repository layer — NOT a generic `IRepository<T>`

One class per operation, not per entity:
- `IRepositoryOperation` / `IRepositoryOperationWithResultValue<T>` — base interfaces (`Succeeded`, `ExecuteAsync()`, `ResultValue`).
- `BaseRepositoryOperation` / `BaseRepositoryOperationWithResultValue<T>` (`BudganInfra/Repositories/`) — shared base, includes `ValidateCanPerformUpdate` (optimistic concurrency: compares DAO's `Timestamp` to persisted entity's `Timestamp`, throws `BudganException` on mismatch/not-found).
- Concrete op: `Repositories/<Entity>/<Operation>/<Operation><Entity>RepoOp.cs`, e.g. `Repositories/ColumnsMapping/Save/SaveColumnsMappingRepoOp.cs`.
- Per-entity facade (`AccountRepository`, `ColumnsMappingRepository`, `UserAccountRepository` — all `internal`, exposed via public `I*Repository`) just constructs the right op class. Don't add CRUD methods directly to the facade interface — add a new op class instead.

### Use-case layer

- `UseCases/<Feature>/<Operation>/` — one class per operation, CQRS-style naming, `internal`.
- `BaseUseCase` / `BaseUseCaseWithResultValue<T>` — mirrors the repo base classes.
- A single per-feature **factory** (e.g. `ColumnsMappingUseCaseFactory`) is the only thing registered in DI (`Scoped`); it takes the repository interface and `new`s up use cases on demand. Don't register individual use cases in DI — add them to the factory.

### API layer

- Controllers: `BudganSvr/Api/Controllers/<Feature>/<Feature>Controller.cs`, route `api/[controller]`.
- Response envelope: `Api/Types/ApiResult.cs` — always return `ApiSuccessResult<T>` or `ApiErrorResult<E>`, never a bare value/exception.
- No auth, no CORS configured anywhere in the solution. Don't assume `[Authorize]` does anything.

### Entities (`BudganInfra/DBContext/Tables/`)

- `BaseEntity` — `Id: Guid` (PK) + `Timestamp: DateTime` (optimistic-concurrency token, `GETUTCDATE()` default). All entities except `UserAccount` extend it.
- `ColumnsMapping` — CSV import column mapping (index + header text per field).
- `Account` — `Name`, `AccountType`, required FK `ColumnsMapping`.
- `UserAccount` — doesn't extend `BaseEntity`, no `OnModelCreating` config yet, no repo ops. Treat as unfinished scaffolding.

### Migrations

Single migration (`InitialCreate`) is **regenerated in place** while the schema is still being shaped, rather than accumulating incremental migrations — this is deliberate for now, don't "fix" it by leaving stale migrations around. Workflow when the model changes:
```bash
DOTNET_ROOT=/opt/homebrew/Cellar/dotnet/10.0.301/libexec ~/.dotnet/tools/dotnet-ef database update 0 --project BudganInfra --startup-project BudganSvr   # revert on local dev DB
DOTNET_ROOT=/opt/homebrew/Cellar/dotnet/10.0.301/libexec ~/.dotnet/tools/dotnet-ef migrations remove --project BudganInfra --startup-project BudganSvr
DOTNET_ROOT=/opt/homebrew/Cellar/dotnet/10.0.301/libexec ~/.dotnet/tools/dotnet-ef migrations add InitialCreate --project BudganInfra --startup-project BudganSvr
DOTNET_ROOT=/opt/homebrew/Cellar/dotnet/10.0.301/libexec ~/.dotnet/tools/dotnet-ef database update --project BudganInfra --startup-project BudganSvr
```
`dotnet-ef` is a global tool but `~/.dotnet/tools` isn't on PATH and its apphost can't locate the SDK without `DOTNET_ROOT` set explicitly (Homebrew dotnet install) — always pass it, or the command fails with "You must install .NET to run this application." Local dev DB: SQL Server on `localhost,1433`, db `Budgan`, `sa`/`P@ssword` (see `appsettings.Development.json`, `Database/Start-MssqlContainer.ps1`). Confirm with the user before reverting/dropping tables on a real (non-throwaway) DB.

## Plan output

When a plan is created (e.g. via plan mode), write it to an `.md` file and open it in an editor tab — do not print the plan to the console.

## Commands

```bash
dotnet build BudganInfra          # or any project name; whole-solution build works too
dotnet test BudganInfra_Test
```
Startup project for `dotnet-ef` commands is always `BudganSvr` (it holds the connection string / DI wiring via `AddInfrastructure`).

## Error handling

`BudganGlobal.MResult<T>` / `BudganErrorValue` / `BudganException` are the shared result/error vocabulary — use them instead of inventing new exception types or bare booleans when adding repo ops or use cases.

## Known gaps (don't silently "fix" — flag/ask first)

- No authentication/authorization, no CORS policy.
- No CI (`.github/workflows` doesn't exist); builds/tests/migrations are run locally.
- `Account` and `UserAccount` are partially implemented (see above) — expanding them to full CRUD is expected future work, not a bug.
