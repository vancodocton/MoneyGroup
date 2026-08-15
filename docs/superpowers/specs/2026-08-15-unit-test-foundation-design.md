# Unit-Test Foundation — Design

**Date:** 2026-08-15
**Status:** Approved
**Slice:** 1 of 4 (see *Deferred work* for the remainder)
**Baseline:** `main` @ `fc33b0a` — 63 tests, 0 failures, 2 skipped

---

## Problem

The test suite's shape does not match what it tests.

Of 48 tests in `MoneyGroup.FunctionalTests`, **28 never start a host, never issue an
HTTP request and never touch a database.** They construct a plain object and call a
method — they are unit tests living in the functional project. The consequence is not
per-test runtime; it is that the project references `Microsoft.AspNetCore.Mvc.Testing`
and all of `WebApi`, so there is no way to run "just the fast tests", and a reader
cannot tell which level of the pyramid a file belongs to.

Separately, the reported coverage figure is misleading. 82% of the lines counted are
generated code — OpenAPI XML-comment transformers, the JSON source-generator context,
the minimal-API route builder, Mapperly. Hand-written code is already ~89% covered, but
the number reported to Codecov and SonarCloud is 68%.

Full findings and evidence: see the test-suite audit
(`https://claude.ai/code/artifact/50a27bf9-5108-44e3-abfa-628327d805e1`).

## Goals

1. Put every test in a project whose name describes what it actually does.
2. Establish one idiom — mocking library, naming convention, data construction — before
   the bulk of the suite gets written.
3. Close the gaps that are unit-testable.

## Non-goals

- **Moving the coverage number materially.** Phase 3 adds ~15 covered lines, which against
  the current denominator is 68.0% → ~68.4%. This is expected, not a shortfall. The
  *reported* figure is distorted by generated code, but correcting that is deferred — see
  *Coverage reality* below.
- **Changing coverage configuration.** `testconfig.json` is not touched in this slice.
- Database isolation, authorization testing, CI restructuring, Postgres support. All
  deferred — see *Deferred work*.
- Fixing the functional suite's seed-data corruption. Deferred to Slice 2, whose
  Testcontainers + Respawn work fixes the root cause; a stopgap here would be throwaway.

## Constraints

- **.NET 10** (`global.json` pins SDK `10.0.302`, `rollForward: latestMinor`).
- **xUnit v3** via `xunit.v3.mtp-v2` 3.2.2 on Microsoft.Testing.Platform. Not changing.
- **Central package management** — all versions go in `Directory.Packages.props`.
- **`TreatWarningsAsErrors=true`** (`Directory.Build.props`). New code must be
  warning-clean; do not add `#pragma warning disable`.
- **Lock files** — `RestorePackagesWithLockFile=true`. Any new package requires the
  affected `packages.lock.json` to be regenerated and committed, or CI fails under
  `RestoreLockedMode`.
- The functional suite requires a seeded SQL Server and remains locally fragile for the
  whole of this slice. `src/Infrastructure.SqlServer/Docker/scripts/reset.sh` (run inside
  the container) restores the seed.

---

## Coverage reality

> **Analysis only — no configuration change ships in this slice.** The exclusion described
> here is deferred: it rebases a metric the team is managed on, and should not land until
> whoever owns that metric understands it. Recorded here so the analysis is not lost and
> the change can be picked up deliberately later.

Measured on `main` @ `fc33b0a`, full suite, after a seed reset.

| Configuration | Coverage | Denominator |
|---|---:|---:|
| Current `testconfig.json` | 68.0% | 3716 lines |
| Excluding `[GeneratedCode]` + `[ExcludeFromCodeCoverage]` | **89.0%** | 662 lines |
| Also excluding `[CompilerGenerated]` | 93.7% | 333 lines |

The third row is **rejected**: `CompilerGeneratedAttribute` covers async state machines,
and every service and handler method in this codebase is async. Excluding it produces a
flattering number by hiding real logic. Verified — under that config `Core` drops from
128 tracked lines to 94, losing the `OrderService` async bodies.

Where the 73 genuinely-uncovered lines live under the accepted configuration:

| Lines | Location | Slice |
|---:|---|---|
| 18 | `DenyUnauthorizedUserHandler.HandleRequirementAsync` — `CurrentUserFeature` branch | 3 (auth) |
| ~24 | `EfRepository` `AddAsync` / `UpdateAsync` / `FirstOrDefaultAsync` / `AnyAsync` | 2 (repo integration) |
| 5 | `UserEndpoints` `/my` | 3 (auth) |
| ~10 | `UserByEmailSpec`, `UserPaginatedSpec`, `UserService.GetUserByEmailAsync` | **1 (this slice)** |
| ~16 | `Program` else-branch, `ApplicationDbContext`, framework metadata | not worth chasing |

**Correction to the original audit:** `UserService` is *not* uncovered — the functional
tests exercise it indirectly. Only `GetUserByEmailAsync` (3 lines) is uncovered, and only
because authorization is bypassed. It has no *direct unit test*, which remains a valid
design complaint, but it is not a coverage hole. The audit's claim that closing these gaps
"is where coverage actually moves" was wrong.

---

## Decisions

| # | Decision | Outcome | Rationale |
|---|---|---|---|
| D1 | Unit-test topology | One `test/UnitTests` project referencing `WebApi` | `WebApi` transitively covers `Core` and `Infrastructure`. Three layer-split projects is over-structure for ~1400 lines of source. |
| D2 | How the 28 tests move | Move as-is in Phase 1; rename in Phase 2 | Keeps the "moved" diff separate from the "changed" diff. Test count parity is the check. |
| D5 | Mocking library | NSubstitute | No `Setup`/`.Object` ceremony; readability is the stated goal. Also lets `Mock<ClaimsPrincipal>` be replaced by a real principal. |
| D6 | Assertion library | **Deferred** — stay on xUnit `Assert` | The `ShouldBe` argument order reverses relative to `Assert.Equal`, making silent wrong-direction assertions the main hazard in this slice. Revisit as its own cycle. |
| D7 | Test framework | xUnit v3, unchanged | Already current. Not the problem. |
| D8 | Test data | Hand-written builders | Explicit and reproducible; AutoFixture's randomness complicates failure reproduction for four DTOs. |
| D10 | Endpoint handlers | `internal` + `InternalsVisibleTo` | Handlers return typed `Results<,>`, directly assertable. Extends a pattern already present twice in the repo. |
| — | Naming convention | `Given_When_Then` | Chosen for readability on the validator-heavy tests Phase 3 adds. Costs ~36 renames in this slice. |
| — | Coverage config | **Deferred** — `testconfig.json` untouched | Rebases a managed metric; should not ship until its owner understands it. Analysis retained under *Coverage reality*. |
| — | Sonar coverage path | **Not touched** | `sonar.cs.vscoveragexml.reportsPaths` is absent from `dotnet.yml`, but SonarCloud is reported as configured correctly. Revisit only if Sonar shows a problem. |

---

## Architecture

### Target project shape

```
test/
  UnitTests/          pure, in-memory, no host, no DB.        36 → ~76 tests
    Builders/         hand-written test-data builders
    Core/             services, specifications
    Infrastructure/   Mapperly mapper
    WebApi/           validators, authorization, endpoints, middleware
  IntegrationTests/   real EF Core + SQL Server.                       6 tests
  FunctionalTests/    HTTP through WebApplicationFactory.      48 → 20 tests
  AppHost.Tests/      Aspire smoke.                                    1 test
```

Folders inside `UnitTests` mirror `src/` so the test for a type is findable from its
path. `UnitTests` gains one project reference:

```xml
<ProjectReference Include="..\..\src\WebApi\MoneyGroup.WebApi.csproj" />
```

`FunctionalTests` loses its `Moq` package reference (nothing there mocks after the move).

**Accepted trade-off:** nothing structurally prevents a host-based test being added to
`UnitTests` once it references `WebApi`. Mitigated by folder convention and review, not
by the build. The alternative — three layer-split test projects — was rejected as
over-structure at this codebase size.

### Phase 1 — relocation

| From | To |
|---|---|
| `FunctionalTests/Validators/OrderDtoValidatorTest.cs` | `UnitTests/WebApi/Validators/OrderDtoValidatorTests.cs` |
| `FunctionalTests/Validators/OrderPaginatedRequestValidatorTests.cs` | `UnitTests/WebApi/Validators/OrderPaginatedRequestValidatorTests.cs` |
| `FunctionalTests/Validators/ParticipantDtoValidatorTest.cs` | `UnitTests/WebApi/Validators/ParticipantDtoValidatorTests.cs` |
| `FunctionalTests/Authorizations/DenyUnauthorizedUserHandlerTest.cs` | `UnitTests/WebApi/Authorizations/DenyUnauthorizedUserHandlerTests.cs` |
| `UnitTests/Services/OrderServiceTest.cs` | `UnitTests/Core/Services/OrderServiceTests.cs` |

Namespaces follow the folders. Class-name suffix standardizes on `Tests`.
`[Trait("Category", "Unit")]` is added at class level — the project split is the real
fast-run mechanism, but traits give Slice 4's CI filtering something to select on.

Two fixture cleanups land here because they are part of moving the files, not separate
behaviour changes:

- `OrderDtoValidatorTestFixture` (an `IClassFixture` wrapping a stateless validator) is
  deleted; the validator becomes a plain field.
- `ParticipantDtoValidatorTest : IClassFixture<ParticipantDtoValidator>` — which uses a
  **production type** as an xUnit fixture — becomes a plain field.

**Invariant: total test count must be 63 before and after.** No assertion or logic edits.

### Phase 2 — lock the idiom

Three mechanical passes, each its own commit, each ending green.

1. **Moq → NSubstitute** across the ~36 unit tests. `Mock<ClaimsPrincipal>` is replaced
   by a real `ClaimsPrincipal` built from `ClaimsPrincipalBuilder`, removing the current
   coupling to `FindFirstValue`'s internal call to `FindFirst`.
2. **`Given_When_Then` renames** across the same tests.
3. **Builders** introduced under `UnitTests/Builders/`, and the duplicated inline literals
   collapsed onto them.

Also retires `#pragma warning disable CA1859` in `OrderServiceTests` by typing the field
as `OrderService` rather than `IOrderService`.

Builder shape — fluent, immutable-in, explicit defaults, no randomness:

```csharp
var dto = OrderDtoBuilder.Valid()
    .WithBuyer(1)
    .WithParticipants(1, 2)
    .Build();
```

Builders live in `UnitTests` only for this slice. If Slice 2 needs them for integration
tests, promoting them to a shared project is that slice's decision.

### Phase 3 — close the unit-testable gaps

| Target | New tests | Coverage effect |
|---|---:|---|
| `UserService` — 3 methods | ~6 | +3 lines |
| 6 specifications | ~12 | +7 lines |
| Mapperly `Mapper` | ~6 | 0 — generated, now excluded |
| `BusinessValidationExceptionHandler` | ~4 | 0 — already covered |
| `OrderService.GetOrdersByPageAsync` | ~2 | 0 |
| 7 endpoint handlers | ~10 | +5 lines |

The Mapperly and exception-handler tests contribute no coverage but are the highest-value
tests in the phase: a source-generated mapper silently dropping a property is exactly the
failure that `Assert.NotNull` in a functional test cannot catch.

**Production change — the only one in this slice.** All 7 minimal-API handlers in
`OrderEndpoints` and `UserEndpoints` normalize to `internal static`, resolving an existing
inconsistency (`GetOrderByIdAsync` and `GetUserByIdAsync` are already `public static`
while their siblings are `private static`). Plus one line in `MoneyGroup.WebApi.csproj`:

```xml
<InternalsVisibleTo Include="MoneyGroup.UnitTests" />
```

This extends a pattern already used twice — `WebApi → FunctionalTests` and
`Infrastructure → IntegrationTests`.

---

## Verification

Every phase ends with a green run; no phase is complete on a red suite.

| Check | Command |
|---|---|
| Fast loop | `dotnet test test/UnitTests/MoneyGroup.UnitTests.csproj` |
| Full suite | seed reset, then `dotnet test --solution MoneyGroup.slnx` |
| Coverage delta | `dotnet test --solution MoneyGroup.slnx --coverage --config-file $PWD/testconfig.json` then `dotnet coverage merge` |
| Format gate | `dotnet format MoneyGroup.slnx --verify-no-changes -v diag --severity info` |

Seed reset, required before any full-suite run until Slice 2:

```bash
docker exec -w /mssql-server-setup-scripts.d moneygroup-mssql-1 bash ./reset.sh
```

Phase-specific gates:

- **Phase 1** — test count is exactly 63 before and after; `FunctionalTests` reports 20.
- **Phase 2** — count stays 63; no `Moq` reference remains in any test project.
- **Phase 3** — count reaches ~103; coverage ~68.4% under the unchanged `testconfig.json`
  (2526+15 covered of 3716); `dotnet format` clean.

The Phase 3 coverage gate looks unimpressive because the denominator still includes
generated code. Against hand-written code only it is ~91%. Do not "fix" this by editing
`testconfig.json` — that change is deliberately deferred.

## Risks

| Risk | Mitigation |
|---|---|
| A moved test silently disappears (namespace or file collision) | Assert exact test-count parity at Phase 1; 63 before, 63 after |
| NSubstitute rewrite changes assertion semantics — `Received()` is not `Verify()` and a missing `Received` assertion passes silently | Convert one file per commit with a green run between; every converted `Verify` must become an explicit `Received` |
| `UnitTests` referencing `WebApi` invites host-based tests | Folder convention + review. Accepted trade-off of the one-project decision |
| Given_When_Then churn produces a large, noisy diff | Renames are their own commit, separate from behaviour changes |
| New package versions break the CI lock-file gate | Regenerate and commit `packages.lock.json` in the same commit that adds NSubstitute |
| `src/WebApi/MoneyGroup.WebApi.json` (generated OpenAPI doc, `text: auto`) re-churns line endings on every Windows build and gets swept into a commit | Never `git add -A`; stage explicit paths. `git diff --numstat` on it is empty when the change is line-endings only |

## Deferred work

| Slice | Content | Why deferred |
|---|---|---|
| 2 | Testcontainers.MsSql + Respawn; provider-parameterised DbContext factory; `EfRepository` integration tests (~24 uncovered lines) | Needs its own design; fixes the seed corruption at the root |
| 3 | `TestAuthHandler`; un-skip the two `/my` tests; `CurrentUserFeature` (~23 uncovered lines) | Largest single coverage block, but needs Slice 2's fixtures |
| 4 | CI fast/slow job split; Aspire nightly; `src/AppHost` coverage exclusion shipped in the same commit | Depends on Phase 1's traits |
| later | Postgres as a second provider | Wanted, explicitly not now. Slice 2 builds the seam so this is additive |
| later | Shouldly adoption | D6 deferred; converts ~76 tests if taken up after this slice |
| later | Coverage-config correction | Deferred pending owner understanding. Self-contained, ~15 min, no test changes — see below |

### Deferred: coverage-config correction

Recorded so it can be picked up deliberately. Adds two attribute exclusions to
`testconfig.json`; takes the reported figure from 68.0% to 89.0% by removing generated
code from the denominator.

```json
"CodeCoverage": {
    "Attributes": {
        "Exclude": [
            "^System\\.CodeDom\\.Compiler\\.GeneratedCodeAttribute$",
            "^System\\.Diagnostics\\.CodeAnalysis\\.ExcludeFromCodeCoverageAttribute$"
        ]
    },
    "Sources": { "Exclude": [ ".*\\\\Migrations\\\\.*.cs" ] }
}
```

Notes for whoever takes it:

- **No workflow change needed.** `.github/workflows/dotnet.yml` already runs
  `dotnet test --coverage --config-file $pwd/testconfig.json`, so reports are filtered at
  generation; `dotnet coverage merge` preserves it and `codecov-action` consumes the
  merged `coverage.xml`.
- **Do not add `CompilerGeneratedAttribute`.** It yields a flattering 93.7% by excluding
  async state machines — i.e. the body of every service and handler method here.
- 68% → 89% is a *rise*, so it passes Codecov's default "must not decrease" status, but it
  permanently rebases history. Ship alone, with a commit message explaining the
  discontinuity.
- Unrelated but adjacent: `dotnet.yml` passes no
  `/d:sonar.cs.vscoveragexml.reportsPaths`. SonarCloud is reported as configured
  correctly, so this is only worth revisiting if Sonar shows a coverage problem.

**Known-open while this slice runs:** the functional suite corrupts its own seed data
(deletes Order 3, leaks `New order` rows) and fails on a second consecutive local run.
CI is unaffected — its container is recreated per run and self-seeds. Slice 2 fixes it.
