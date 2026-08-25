# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

.NET 10 Clean Architecture backend API (orders split among users) orchestrated by .NET Aspire, plus a Vite/React client app. Solution file is `MoneyGroup.slnx` (XML solution format — not `.sln`).

## Commands

Build/test/format commands must target the solution explicitly; several tools do not auto-discover `.slnx`.

```bash
dotnet restore MoneyGroup.slnx
dotnet build MoneyGroup.slnx
dotnet format MoneyGroup.slnx --verify-no-changes -v diag --severity info   # exactly what CI enforces
dotnet tool restore                                                          # dotnet-ef, dotnet-sonarscanner, dotnet-coverage
```

### Running

```bash
aspire run                                          # preferred; AppHost orchestrates SQL Server + WebApi + ClientApp
dotnet run --project src/AppHost                    # equivalent without the Aspire CLI
dotnet run --project src/WebApi                     # API alone; needs a SQL Server reachable at SqlServerConnection
```

### Testing

Tests run on **Microsoft.Testing.Platform** (MTP), not VSTest — `global.json` sets `test.runner`, and `Directory.Build.props` sets `UseMicrosoftTestingPlatformRunner`. This changes the CLI syntax: use `--solution`, and MTP filter flags rather than `--filter`.

```bash
dotnet test --solution MoneyGroup.slnx                              # all tests
dotnet test --project test/UnitTests                               # one project
dotnet test --solution MoneyGroup.slnx --coverage --config-file $pwd/testconfig.json --report-xunit
```

**Integration and functional tests need a live SQL Server first:**

```bash
docker compose up --wait mssql      # exposes localhost:1433, sa/password123!
```

`test/IntegrationTests/appsettings.json` hardcodes `localhost,1433`. Note this is a *different* instance than the one Aspire's AppHost starts (that one uses port **1435** with `ContainerLifetime.Persistent`), so `aspire run` does not satisfy the integration tests.

Functional tests authenticate with a **real Google-issued** ID token. `WebApiFactory` exchanges `Test:Google:{ClientId,ClientSecret,RefreshToken}` (user secrets locally, repo secrets in CI) for an id_token at host startup. Without those three values the client is left unauthenticated and every endpoint test fails with 401 — this is the usual reason a fresh clone shows red functional tests.

### Migrations

Each provider project owns its own migrations and holds the `MigrationsAssembly`. Run the helper from inside the provider directory:

```bash
cd src/Infrastructure.SqlServer && ./migrations-add.ps1 -Name "AddPayment"
cd src/Infrastructure.PostgreSql && ./migrations-add.ps1 -Name "AddPayment"
```

The script wraps `dotnet ef migrations add --configuration Release --no-build -p .`, so **build first** — `--no-build` means a stale binary silently produces a stale migration. A schema change to `ApplicationDbContext` requires adding the migration in *both* provider projects.

### JWT for local API calls

```bash
dotnet user-jwts create -o token --scheme Bearer --claim email_verified=true --claim email=test@example.com
```

## Architecture

```
WebApi (endpoints, validators, auth) → Core (entities, services, specs) ← Infrastructure (EF, Mapperly)
                                                                            ↑
                                            Infrastructure.SqlServer / Infrastructure.PostgreSql (migrations only)
```

**Core never references Infrastructure or WebApi.** Core defines `IRepository<T>`, `IMapper`, and the service interfaces; Infrastructure implements them.

### Request flow

Endpoint (`Results<TSuccess, TError>` union) → `I{Name}Service` → `I{Name}Repository` → `EfRepository<T>` → Ardalis specification applied to `DbSet<T>` → Mapperly `ProjectTo<TResult>` into a DTO. Projection happens in the database — repositories return DTOs, not entities, for read paths.

### Pieces that are easy to miss

- **`ApplicationDbContext` lives in `src/Infrastructure`**, not in the provider projects. The provider projects contribute only the `UseSqlServer`/`UseNpgsql` registration extension and the migrations folder.
- **`src/Postgres.Migrator` is misnamed** — it references `Infrastructure.SqlServer` and calls `AddApplicationDbContextSqlServer`, so it migrates SQL Server. It's a worker container that runs `Database.MigrateAsync()` on startup.
- **Validators live in `src/WebApi/Validators/`, not in Core.** They are registered by hand as singletons in `Program.cs` — adding a validator file is not enough, it must be added to the `#region Validators` block. Validation is wired via `SharpGrip.FluentValidation.AutoValidation` (`.AddFluentValidationAutoValidation()` on the endpoint group).
- **`AppJsonSerializerContext`** (`src/Core/Models/`) is a source-generated `JsonSerializerContext` inserted at the front of the resolver chain. Every new DTO and every `PaginatedModel<T>` closed generic used on the wire needs a `[JsonSerializable(typeof(...))]` attribute, or serialization falls back/fails at runtime.
- **Mapperly `Mapper`** (`src/Infrastructure/Mapperly/Mapper.cs`) is a `[Mapper]` partial class. Add `public partial X Map(Y)` for object mapping and `public partial IQueryable<X> Project(IQueryable<Y>)` for the queryable projection used by `ProjectTo<T>`; the source generator fills in the body. A missing `Project` overload surfaces as a runtime failure in `EfRepository`, not a compile error.
- **Business-rule violations throw** `BussinessValidationException` subclasses (note the spelling) from Core services; `BusinessValidationExceptionHandler` maps them to 400.
- **Delete behavior:** deleting an `Order` cascades to `OrderParticipant`; deleting a `User` is `Restrict`ed wherever referenced (as buyer or participant).
- **`src/ClientApp`** is a Vite/React 19 app wired in via `builder.AddViteApp(...)`. It has its own `npm run dev|build|lint`, and its `.esproj` is in the solution but is excluded from the CI backend path filter.

### Adding an entity — the full checklist

Entity in `Core/Entities/` → `DbSet<T>` + `OnModelCreating` config in `ApplicationDbContext` → DTOs in `Core/Models/{Plural}/` → `I{Name}Repository` in `Core/Abstractions/` → implementation in `Infrastructure/Data/` → `I{Name}Service` + `Services/` implementation → specifications in `Core/Specifications/` → validator in `WebApi/Validators/` → endpoints in `WebApi/Endpoints/` → register repository/service/validator and call `Map{Name}Endpoints()` in `Program.cs` → `[JsonSerializable]` entries in `AppJsonSerializerContext` → migration in **both** provider projects → tests.

## Build constraints that bite

- **`TreatWarningsAsErrors=true`** solution-wide (only `NU1902`/`NU1903` are exempt). Unused usings and analyzer suggestions break the build.
- **Central Package Management** — all versions live in `Directory.Packages.props`. `PackageReference` in a csproj must carry no `Version` attribute.
- **Lock files are on** (`RestorePackagesWithLockFile=true`), and CI restores with `RestoreLockedMode=true`. Any dependency change requires committing the regenerated `packages.lock.json` for every affected project, or CI restore fails. `src/AppHost` opts out (`RestorePackagesWithLockFile=false`) and has no lock file.
- **`dotnet_separate_import_directive_groups = true`** with `dotnet_sort_system_directives_first` — using directives are grouped by root namespace with a blank line between groups. `dotnet format` is enforced at `--severity info`, so this is a build gate, not a preference.
- Line endings are CRLF for `.cs` (LF for `.sh`) per `.editorconfig`.

## Conventions

- File-scoped namespaces; usings outside the namespace.
- Async methods suffixed `Async`, `CancellationToken cancellationToken = default` as the last parameter.
- Endpoints are `static` classes exposing `Map{Name}Endpoints(this IEndpointRouteBuilder)`, with private static handlers returning `Results<...>`:
  `Results<Ok<T>, NotFound>` (GET by id), `Results<Ok<PaginatedModel<T>>, ValidationProblem>` (list), `Results<CreatedAtRoute<T>, ValidationProblem>` (POST), `Results<NoContent, NotFound>` (DELETE).
- Specifications: `EntityByIdSpec<T>`, `EntityByIdsSpec<T>`, `{Name}PaginatedSpec` deriving from `BasePaginatedSpecification<T>` (which applies `Skip((Page - 1) * Size).Take(Size)` — pages are 1-based).
- Unit tests use Moq against Core interfaces; integration tests hit real SQL Server; functional tests use `WebApiFactory : WebApplicationFactory<Program>`; `test/AppHost.Tests` boots the whole Aspire app via `DistributedApplicationFactory`.
- Commits follow `feat:` / `fix:` / `chore:` / `docs:` / `test:`; branches `feature/*`, `bugfix/*`, `chore/*` off `main`.

## Aspire notes

`AGENTS.md` holds the Aspire agent guidance. Key points: the app model is in `src/AppHost/AppHost.cs` (the repo does not use the `apphost.cs` single-file layout referenced generically in that doc); changes there require restarting the app. Use the Aspire MCP tools (list resources, console/structured logs, traces) to diagnose before changing code, and `list integrations` + `get integration docs` before adding a resource — match the integration version to the `Aspire.AppHost.Sdk` version (currently 13.4.6). The Aspire **workload is obsolete** — never install it.

`.github/copilot-instructions.md` has been deleted from the working tree; its useful content is folded into this file.
