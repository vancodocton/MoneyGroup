# Unit-Test Foundation Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Move the 28 misplaced unit tests out of `FunctionalTests`, standardise the test idiom on NSubstitute + Given_When_Then + hand-written builders, and close the unit-testable coverage gaps.

**Architecture:** One `test/UnitTests` project referencing `MoneyGroup.WebApi` (which transitively supplies `Core` and `Infrastructure`), with folders mirroring `src/`. Three phases: relocate without changing behaviour, then convert the idiom, then add new tests. Each task ends green.

**Tech Stack:** .NET 10, xUnit v3 (`xunit.v3.mtp-v2` 3.2.2) on Microsoft.Testing.Platform, NSubstitute 6.2.0, FluentValidation 12.1.1, Ardalis.Specification 9.3.1, Riok.Mapperly 4.3.1.

**Spec:** `docs/superpowers/specs/2026-08-15-unit-test-foundation-design.md`

## Global Constraints

- **`TreatWarningsAsErrors=true`** (`Directory.Build.props`). Never add `#pragma warning disable` — fix the cause.
- **Central package management.** Every version goes in `Directory.Packages.props`; `<PackageReference>` in a csproj carries **no** `Version` attribute.
- **Lock files.** `RestorePackagesWithLockFile=true`. Any package change requires the affected `packages.lock.json` to be regenerated and committed in the *same* commit, or CI fails under `RestoreLockedMode`.
- **Never `git add -A` or `git add .`** Stage explicit paths. `src/WebApi/MoneyGroup.WebApi.json` is a generated OpenAPI document marked `text: auto`, and it re-churns line endings on every Windows build. `git diff --numstat` on it is empty when the change is line-endings only — discard it with `git checkout -- src/WebApi/MoneyGroup.WebApi.json`.
- **Assertions stay on xUnit `Assert`.** Shouldly is explicitly deferred (spec decision D6). Do not introduce it.
- **`testconfig.json` must not be modified.** The coverage-config correction is deliberately deferred; the low coverage percentage is expected. See the spec's *Deferred* section.
- **Test naming is `Given_When_Then`.** Class-name suffix is `Tests` (plural).
- **Cancellation tokens in tests use `TestContext.Current.CancellationToken`** (xUnit v3), never `default` or `CancellationToken.None`, except where asserting a specific token value.
- Run all commands from the worktree root: `D:\source\vancodocton\MoneyGroup\.claude\worktrees\test-unit-foundation`.

## Baseline

`main` @ `fc33b0a` — **63 tests, 0 failures, 2 skipped.** UnitTests 8, IntegrationTests 6, FunctionalTests 48, AppHost.Tests 1.

Full-suite runs need a seeded database first (the functional suite corrupts its own seed; fixed in a later slice):

```bash
docker compose up -d --wait mssql
docker exec -w /mssql-server-setup-scripts.d moneygroup-mssql-1 bash ./reset.sh
```

The fast loop used by most tasks needs no database:

```bash
dotnet test test/UnitTests/MoneyGroup.UnitTests.csproj
```

---

# Phase 1 — Relocation

Pure motion. **Total test count must remain exactly 63 through the entire phase.**

---

### Task 1: Give UnitTests access to WebApi

**Files:**
- Modify: `test/UnitTests/MoneyGroup.UnitTests.csproj`
- Modify: `test/UnitTests/packages.lock.json` (regenerated)

**Interfaces:**
- Consumes: nothing
- Produces: `MoneyGroup.UnitTests` can reference types from `MoneyGroup.WebApi`, `MoneyGroup.Core` and `MoneyGroup.Infrastructure`.

- [ ] **Step 1: Record the baseline test count**

```bash
dotnet test --solution MoneyGroup.slnx
```

Expected: `total: 63`, `failed: 0`, `skipped: 2`. If the functional tests fail, run the seed reset from the Baseline section and re-run.

- [ ] **Step 2: Add the project reference**

In `test/UnitTests/MoneyGroup.UnitTests.csproj`, replace the existing `ProjectReference` item group:

```xml
  <ItemGroup>
    <ProjectReference Include="..\..\src\Core\MoneyGroup.Core.csproj" />
  </ItemGroup>
```

with:

```xml
  <ItemGroup>
    <ProjectReference Include="..\..\src\WebApi\MoneyGroup.WebApi.csproj" />
  </ItemGroup>
```

`WebApi` already references `Core` and `Infrastructure`, so one reference covers all three.

- [ ] **Step 3: Regenerate the lock file**

```bash
dotnet restore test/UnitTests/MoneyGroup.UnitTests.csproj --force-evaluate
```

Expected: `test/UnitTests/packages.lock.json` is rewritten with the new transitive graph.

- [ ] **Step 4: Verify nothing broke**

```bash
dotnet test test/UnitTests/MoneyGroup.UnitTests.csproj
```

Expected: PASS, `total: 8`.

- [ ] **Step 5: Commit**

```bash
git add test/UnitTests/MoneyGroup.UnitTests.csproj test/UnitTests/packages.lock.json
git commit -m "test: reference WebApi from UnitTests

Prepares UnitTests to receive the validator and authorization tests
currently misplaced in FunctionalTests. WebApi transitively supplies
Core and Infrastructure."
```

---

### Task 2: Relocate OrderServiceTest

**Files:**
- Delete: `test/UnitTests/Services/OrderServiceTest.cs`
- Create: `test/UnitTests/Core/Services/OrderServiceTests.cs`

**Interfaces:**
- Consumes: Task 1's project reference
- Produces: class `MoneyGroup.UnitTests.Core.Services.OrderServiceTests` — 8 tests, still Moq-based.

- [ ] **Step 1: Move the file**

```bash
git mv test/UnitTests/Services/OrderServiceTest.cs test/UnitTests/Core/Services/OrderServiceTests.cs
```

- [ ] **Step 2: Update the namespace, class name and field type**

In `test/UnitTests/Core/Services/OrderServiceTests.cs`:

Change the namespace line from `namespace MoneyGroup.UnitTests.Services;` to:

```csharp
namespace MoneyGroup.UnitTests.Core.Services;
```

Change the class declaration and remove the pragma. Replace:

```csharp
public class OrderServiceTest
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
#pragma warning disable CA1859 // Use concrete types when possible for improved performance
    private readonly IOrderService _orderService;
#pragma warning restore CA1859 // Use concrete types when possible for improved performance

    public OrderServiceTest()
```

with:

```csharp
[Trait("Category", "Unit")]
public class OrderServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly OrderService _orderService;

    public OrderServiceTests()
```

Typing the field as the concrete `OrderService` is what retires `CA1859`; the pragma is no longer needed and must not be reintroduced.

- [ ] **Step 3: Run tests**

```bash
dotnet test test/UnitTests/MoneyGroup.UnitTests.csproj
```

Expected: PASS, `total: 8`.

- [ ] **Step 4: Commit**

```bash
git add test/UnitTests/Core/Services/OrderServiceTests.cs
git commit -m "test: relocate OrderServiceTest to Core/Services

Mirrors src/ layout. Retires the CA1859 pragma by typing the field as
the concrete OrderService."
```

---

### Task 3: Relocate the validator tests

**Files:**
- Delete: `test/FunctionalTests/Validators/OrderDtoValidatorTest.cs`
- Delete: `test/FunctionalTests/Validators/OrderPaginatedRequestValidatorTests.cs`
- Delete: `test/FunctionalTests/Validators/ParticipantDtoValidatorTest.cs`
- Create: `test/UnitTests/WebApi/Validators/OrderDtoValidatorTests.cs`
- Create: `test/UnitTests/WebApi/Validators/OrderPaginatedRequestValidatorTests.cs`
- Create: `test/UnitTests/WebApi/Validators/ParticipantDtoValidatorTests.cs`

**Interfaces:**
- Consumes: Task 1's project reference
- Produces: 23 tests in namespace `MoneyGroup.UnitTests.WebApi.Validators`.

- [ ] **Step 1: Move the three files**

```bash
git mv test/FunctionalTests/Validators/OrderDtoValidatorTest.cs test/UnitTests/WebApi/Validators/OrderDtoValidatorTests.cs
git mv test/FunctionalTests/Validators/OrderPaginatedRequestValidatorTests.cs test/UnitTests/WebApi/Validators/OrderPaginatedRequestValidatorTests.cs
git mv test/FunctionalTests/Validators/ParticipantDtoValidatorTest.cs test/UnitTests/WebApi/Validators/ParticipantDtoValidatorTests.cs
```

- [ ] **Step 2: Fix OrderDtoValidatorTests — namespace, name, and delete the pointless fixture**

In `test/UnitTests/WebApi/Validators/OrderDtoValidatorTests.cs`, replace everything from `namespace` down to the end of the constructor:

```csharp
namespace MoneyGroup.FunctionalTests.Validators;

public class OrderDtoValidatorTestFixture
{
    public OrderDtoValidator Validator { get; }

    public OrderDtoValidatorTestFixture()
    {
        Validator = new OrderDtoValidator(new ParticipantDtoValidator());
    }
}

public class OrderDtoValidatorTest
    : IClassFixture<OrderDtoValidatorTestFixture>
{
    private readonly OrderDtoValidator _validator;

    public OrderDtoValidatorTest(OrderDtoValidatorTestFixture fixture)
    {
        _validator = fixture.Validator;
    }
```

with:

```csharp
namespace MoneyGroup.UnitTests.WebApi.Validators;

[Trait("Category", "Unit")]
public class OrderDtoValidatorTests
{
    private readonly OrderDtoValidator _validator = new(new ParticipantDtoValidator());
```

The fixture existed only to cache a stateless, allocation-free validator.

- [ ] **Step 3: Fix ParticipantDtoValidatorTests — stop using a production type as a fixture**

In `test/UnitTests/WebApi/Validators/ParticipantDtoValidatorTests.cs`, replace:

```csharp
namespace MoneyGroup.FunctionalTests.Validators;

public class ParticipantDtoValidatorTest
    : IClassFixture<ParticipantDtoValidator>
{
    private readonly ParticipantDtoValidator _validator;

    public ParticipantDtoValidatorTest(ParticipantDtoValidator validator)
    {
        _validator = validator;
    }
```

with:

```csharp
namespace MoneyGroup.UnitTests.WebApi.Validators;

[Trait("Category", "Unit")]
public class ParticipantDtoValidatorTests
{
    private readonly ParticipantDtoValidator _validator = new();
```

- [ ] **Step 4: Fix OrderPaginatedRequestValidatorTests — namespace and class name only**

In `test/UnitTests/WebApi/Validators/OrderPaginatedRequestValidatorTests.cs`, change:

```csharp
namespace MoneyGroup.FunctionalTests.Validators;

public class OrderPaginatedRequestValidatorTests
{
```

to:

```csharp
namespace MoneyGroup.UnitTests.WebApi.Validators;

[Trait("Category", "Unit")]
public class OrderPaginatedRequestValidatorTests
{
```

- [ ] **Step 5: Run both projects**

```bash
dotnet test test/UnitTests/MoneyGroup.UnitTests.csproj
```

Expected: PASS, `total: 31` (8 + 23).

```bash
dotnet test test/FunctionalTests/MoneyGroup.FunctionalTests.csproj
```

Expected: PASS, `total: 25` (48 − 23), `skipped: 2`. If `DeleteOrder_ValidId_ReturnsNoContent` fails, run the seed reset — that failure is the known, deferred seed-corruption issue, not a regression from this task.

- [ ] **Step 6: Commit**

```bash
git add test/UnitTests/WebApi/Validators test/FunctionalTests/Validators
git commit -m "test: relocate validator tests to UnitTests

These 23 tests never start a host or touch a database. Also removes
OrderDtoValidatorTestFixture (cached a stateless object) and stops
ParticipantDtoValidatorTests using a production type as an
IClassFixture."
```

---

### Task 4: Relocate the authorization handler test

**Files:**
- Delete: `test/FunctionalTests/Authorizations/DenyUnauthorizedUserHandlerTest.cs`
- Create: `test/UnitTests/WebApi/Authorizations/DenyUnauthorizedUserHandlerTests.cs`

**Interfaces:**
- Consumes: Task 1's project reference
- Produces: 5 tests in namespace `MoneyGroup.UnitTests.WebApi.Authorizations`. Still Moq-based; converted in Task 8.

- [ ] **Step 1: Move the file**

```bash
git mv test/FunctionalTests/Authorizations/DenyUnauthorizedUserHandlerTest.cs test/UnitTests/WebApi/Authorizations/DenyUnauthorizedUserHandlerTests.cs
```

- [ ] **Step 2: Update namespace and class name**

Change:

```csharp
namespace MoneyGroup.FunctionalTests.Authorizations;

public class DenyUnauthorizedUserHandlerTest
{
```

to:

```csharp
namespace MoneyGroup.UnitTests.WebApi.Authorizations;

[Trait("Category", "Unit")]
public class DenyUnauthorizedUserHandlerTests
{
```

Also rename the constructor from `DenyUnauthorizedUserHandlerTest()` to `DenyUnauthorizedUserHandlerTests()`.

- [ ] **Step 3: Build and fix the JWT claim-names namespace if needed**

```bash
dotnet build test/UnitTests/MoneyGroup.UnitTests.csproj
```

The file currently imports `System.IdentityModel.Tokens.Jwt`. If the build fails with `CS0246` on `JwtRegisteredClaimNames`, replace that using directive with the one production code uses:

```csharp
using Microsoft.IdentityModel.JsonWebTokens;
```

Both types define `EmailVerified` as the string `"email_verified"`, so no test behaviour changes. `DenyUnauthorizedUserHandler` itself uses the `Microsoft.IdentityModel.JsonWebTokens` variant, so aligning is correct regardless.

- [ ] **Step 4: Run both projects**

```bash
dotnet test test/UnitTests/MoneyGroup.UnitTests.csproj
```

Expected: PASS, `total: 36` (31 + 5).

```bash
dotnet test test/FunctionalTests/MoneyGroup.FunctionalTests.csproj
```

Expected: PASS, `total: 20`, `skipped: 2`.

- [ ] **Step 5: Commit**

```bash
git add test/UnitTests/WebApi/Authorizations test/FunctionalTests/Authorizations
git commit -m "test: relocate DenyUnauthorizedUserHandler test to UnitTests

Uses Moq and NullLoggerFactory only; no host, no HTTP, no database.
FunctionalTests is now 20 genuinely-functional tests."
```

---

### Task 5: Drop Moq from FunctionalTests and confirm parity

**Files:**
- Modify: `test/FunctionalTests/MoneyGroup.FunctionalTests.csproj`
- Modify: `test/FunctionalTests/packages.lock.json` (regenerated)

**Interfaces:**
- Consumes: Tasks 3 and 4 having emptied the mocking usage out of `FunctionalTests`
- Produces: `FunctionalTests` with no mocking dependency.

- [ ] **Step 1: Confirm nothing in FunctionalTests still uses Moq**

```bash
grep -rn "Moq" test/FunctionalTests --include=*.cs
```

Expected: no output.

- [ ] **Step 2: Remove the package reference**

In `test/FunctionalTests/MoneyGroup.FunctionalTests.csproj`, delete this line from the `PackageReference` item group:

```xml
    <PackageReference Include="Moq" />
```

- [ ] **Step 3: Regenerate the lock file**

```bash
dotnet restore test/FunctionalTests/MoneyGroup.FunctionalTests.csproj --force-evaluate
```

- [ ] **Step 4: Verify full-suite parity**

```bash
docker exec -w /mssql-server-setup-scripts.d moneygroup-mssql-1 bash ./reset.sh
dotnet test --solution MoneyGroup.slnx
```

Expected: `total: 63`, `failed: 0`, `skipped: 2` — **identical to the Phase 1 baseline.** Any other number means a test was lost in a move; do not proceed until it is 63.

- [ ] **Step 5: Commit**

```bash
git add test/FunctionalTests/MoneyGroup.FunctionalTests.csproj test/FunctionalTests/packages.lock.json
git commit -m "test: drop Moq from FunctionalTests

Nothing there mocks any more. Phase 1 complete: 63 tests before and
after, now distributed 36 unit / 6 integration / 20 functional / 1 smoke."
```

---

# Phase 2 — Lock the idiom

Still 63 tests throughout.

---

### Task 6: Add NSubstitute

**Files:**
- Modify: `Directory.Packages.props`
- Modify: `test/UnitTests/MoneyGroup.UnitTests.csproj`
- Modify: `test/UnitTests/packages.lock.json` (regenerated)

**Interfaces:**
- Produces: `NSubstitute` available in `MoneyGroup.UnitTests`, alongside Moq during the transition.

- [ ] **Step 1: Pin the version centrally**

In `Directory.Packages.props`, add this line to the `ItemGroup`, keeping alphabetical order (immediately after the `Moq` line):

```xml
    <PackageVersion Include="NSubstitute" Version="6.2.0" />
```

- [ ] **Step 2: Reference it from UnitTests**

In `test/UnitTests/MoneyGroup.UnitTests.csproj`, add to the `PackageReference` item group (note: no `Version` attribute — central package management supplies it):

```xml
    <PackageReference Include="NSubstitute" />
```

- [ ] **Step 3: Regenerate the lock file**

```bash
dotnet restore test/UnitTests/MoneyGroup.UnitTests.csproj --force-evaluate
```

- [ ] **Step 4: Verify the build still passes**

```bash
dotnet test test/UnitTests/MoneyGroup.UnitTests.csproj
```

Expected: PASS, `total: 36`.

- [ ] **Step 5: Commit**

```bash
git add Directory.Packages.props test/UnitTests/MoneyGroup.UnitTests.csproj test/UnitTests/packages.lock.json
git commit -m "test: add NSubstitute 6.2.0 to UnitTests

Coexists with Moq for the duration of the conversion."
```

---

### Task 7: Convert OrderServiceTests to NSubstitute

**Files:**
- Modify: `test/UnitTests/Core/Services/OrderServiceTests.cs`

**Interfaces:**
- Consumes: NSubstitute from Task 6
- Produces: `OrderServiceTests` with no Moq usage.

> **Critical:** Moq's `Verify` throws when the call did not happen. NSubstitute's equivalent is `Received()`, and **a forgotten `Received()` assertion passes silently** — the test still goes green while verifying nothing. Every `Verify` below must become an explicit `Received`/`DidNotReceive`. Calls returning `Task` must be awaited: `await sub.Received(1).Method(...)`.

- [ ] **Step 1: Replace the using directive and the fields**

Replace `using Moq;` with:

```csharp
using NSubstitute;
```

Replace the field declarations and constructor body:

```csharp
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly OrderService _orderService;

    public OrderServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _orderService = new OrderService(_orderRepositoryMock.Object, _userRepositoryMock.Object);
    }
```

with:

```csharp
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IOrderRepository _orderRepository = Substitute.For<IOrderRepository>();
    private readonly OrderService _orderService;

    public OrderServiceTests()
    {
        _orderService = new OrderService(_orderRepository, _userRepository);
    }
```

- [ ] **Step 2: Convert `GetOrderByIdAsync_ValidId_ShouldReturnOrder`**

Replace the body with:

```csharp
        // Arrange
        var id = 1;
        var orderDto = new OrderDetailedDto { Id = id };
        var cancellationToken = TestContext.Current.CancellationToken;

        _orderRepository
            .FirstOrDefaultAsync<OrderDetailedDto>(Arg.Any<EntityByIdSpec<Order>>(), cancellationToken)
            .Returns(orderDto);

        // Act
        var result = await _orderService.GetOrderByIdAsync(id, cancellationToken);

        // Assert
        await _orderRepository.Received(1)
            .FirstOrDefaultAsync<OrderDetailedDto>(Arg.Any<EntityByIdSpec<Order>>(), cancellationToken);
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
```

- [ ] **Step 3: Convert `GetOrderByIdAsync_InvalidId_ShouldReturnNull`**

```csharp
        // Arrange
        var invalidId = -1;
        var cancellationToken = TestContext.Current.CancellationToken;

        _orderRepository
            .FirstOrDefaultAsync<OrderDetailedDto>(Arg.Any<EntityByIdSpec<Order>>(), cancellationToken)
            .Returns((OrderDetailedDto?)null);

        // Act
        var result = await _orderService.GetOrderByIdAsync(invalidId, cancellationToken);

        // Assert
        await _orderRepository.Received(1)
            .FirstOrDefaultAsync<OrderDetailedDto>(Arg.Any<EntityByIdSpec<Order>>(), cancellationToken);
        Assert.Null(result);
```

- [ ] **Step 4: Convert `CreateOrderAsync_ValidDto_ShouldAddOrder`**

```csharp
        // Arrange
        var newOrderId = 1;
        var model = new OrderDto
        {
            BuyerId = 1,
            Participants =
            [
                new() { ParticipantId = 2 },
                new() { ParticipantId = 3 },
            ],
        };

        _userRepository.AnyAsync(Arg.Any<EntityByIdSpec<User>>(), Arg.Any<CancellationToken>())
            .Returns(true);
        _userRepository.CountAsync(Arg.Any<EntityByIdsSpec<User>>(), Arg.Any<CancellationToken>())
            .Returns(2);
        _orderRepository.AddAsync(Arg.Any<OrderDto>(), Arg.Any<CancellationToken>())
            .Returns(_ =>
            {
                model.Id = newOrderId;
                return model;
            });

        // Act
        await _orderService.CreateOrderAsync(model, TestContext.Current.CancellationToken);

        // Assert
        await _userRepository.Received(1).AnyAsync(Arg.Any<EntityByIdSpec<User>>(), Arg.Any<CancellationToken>());
        await _userRepository.Received(1).CountAsync(Arg.Any<EntityByIdsSpec<User>>(), Arg.Any<CancellationToken>());
        await _orderRepository.Received(1).AddAsync(Arg.Any<OrderDto>(), Arg.Any<CancellationToken>());
        Assert.Equal(newOrderId, model.Id);
```

- [ ] **Step 5: Convert `CreateOrderAsync_NoParticipants_ShouldAddOrder`**

```csharp
        // Arrange
        var newOrderId = 1;
        var model = new OrderDto
        {
            BuyerId = 1,
            Participants = [],
        };

        _userRepository.AnyAsync(Arg.Any<EntityByIdSpec<User>>(), Arg.Any<CancellationToken>())
            .Returns(true);
        _orderRepository.AddAsync(Arg.Any<OrderDto>(), Arg.Any<CancellationToken>())
            .Returns(_ =>
            {
                model.Id = newOrderId;
                return model;
            });

        // Act
        await _orderService.CreateOrderAsync(model, TestContext.Current.CancellationToken);

        // Assert
        await _userRepository.Received(1).AnyAsync(Arg.Any<EntityByIdSpec<User>>(), Arg.Any<CancellationToken>());
        await _userRepository.DidNotReceive().CountAsync(Arg.Any<EntityByIdsSpec<User>>(), Arg.Any<CancellationToken>());
        await _orderRepository.Received(1).AddAsync(Arg.Any<OrderDto>(), Arg.Any<CancellationToken>());
        Assert.Equal(newOrderId, model.Id);
```

- [ ] **Step 6: Convert `CreateOrderAsync_InvalidBuyer_ShouldThrowInvalidOperationException`**

```csharp
        // Arrange
        var model = new OrderDto
        {
            BuyerId = -1,
            Participants =
            [
                new() { ParticipantId = 2 },
                new() { ParticipantId = 3 },
            ],
        };

        _userRepository.AnyAsync(Arg.Any<EntityByIdSpec<User>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        var ex = await Assert.ThrowsAsync<BuyerNotFoundException>(
            () => _orderService.CreateOrderAsync(model, TestContext.Current.CancellationToken));

        // Assert
        await _userRepository.Received(1).AnyAsync(Arg.Any<EntityByIdSpec<User>>(), Arg.Any<CancellationToken>());
        await _orderRepository.DidNotReceive().AddAsync(Arg.Any<OrderDto>(), Arg.Any<CancellationToken>());
        Assert.Equal("Buyer not found", ex.Message);
```

- [ ] **Step 7: Convert `CreateOrderAsync_InvalidParticipants_ShouldThrowInvalidOperationException`**

```csharp
        // Arrange
        var model = new OrderDto
        {
            BuyerId = 1,
            Participants =
            [
                new() { ParticipantId = 2 },
                new() { ParticipantId = -1 },
            ],
        };

        _userRepository.AnyAsync(Arg.Any<EntityByIdSpec<User>>(), Arg.Any<CancellationToken>())
            .Returns(true);
        _userRepository.CountAsync(Arg.Any<EntityByIdsSpec<User>>(), Arg.Any<CancellationToken>())
            .Returns(1); // only 1 of the 2 requested participants exists

        // Act
        var ex = await Assert.ThrowsAsync<ParticipantNotFoundException>(
            () => _orderService.CreateOrderAsync(model, TestContext.Current.CancellationToken));

        // Assert
        await _userRepository.Received(1).AnyAsync(Arg.Any<EntityByIdSpec<User>>(), Arg.Any<CancellationToken>());
        await _userRepository.Received(1).CountAsync(Arg.Any<EntityByIdsSpec<User>>(), Arg.Any<CancellationToken>());
        await _orderRepository.DidNotReceive().AddAsync(Arg.Any<OrderDto>(), Arg.Any<CancellationToken>());
        Assert.Equal("Participant not found", ex.Message);
```

- [ ] **Step 8: Convert `RemoveOrderAsync_OrderExists_ShouldRemoveOrder`**

NSubstitute returns a completed `Task` by default, so the `RemoveAsync` setup line disappears entirely.

```csharp
        // Arrange
        var orderId = 1;
        var order = new Order { Id = orderId };

        _orderRepository.FirstOrDefaultAsync(Arg.Any<EntityByIdSpec<Order>>(), Arg.Any<CancellationToken>())
            .Returns(order);

        // Act
        var result = await _orderService.RemoveOrderAsync(orderId, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result);
        await _orderRepository.Received(1).FirstOrDefaultAsync(Arg.Any<EntityByIdSpec<Order>>(), Arg.Any<CancellationToken>());
        await _orderRepository.Received(1).RemoveAsync(order, Arg.Any<CancellationToken>());
```

- [ ] **Step 9: Convert `RemoveOrderAsync_OrderNotFound_ShouldThrowInvalidOperationException`**

```csharp
        // Arrange
        var orderId = 1;

        _orderRepository.FirstOrDefaultAsync(Arg.Any<EntityByIdSpec<Order>>(), Arg.Any<CancellationToken>())
            .Returns((Order?)null);

        // Act
        var result = await _orderService.RemoveOrderAsync(orderId, TestContext.Current.CancellationToken);

        // Assert
        Assert.False(result);
        await _orderRepository.Received(1).FirstOrDefaultAsync(Arg.Any<EntityByIdSpec<Order>>(), Arg.Any<CancellationToken>());
        await _orderRepository.DidNotReceive().RemoveAsync(Arg.Any<Order>(), Arg.Any<CancellationToken>());
```

- [ ] **Step 10: Confirm no Moq remains in this file**

```bash
grep -n "Moq\|Mock<\|\.Object\|It\.IsAny\|Times\." test/UnitTests/Core/Services/OrderServiceTests.cs
```

Expected: no output.

- [ ] **Step 11: Run tests**

```bash
dotnet test test/UnitTests/MoneyGroup.UnitTests.csproj
```

Expected: PASS, `total: 36`.

- [ ] **Step 12: Commit**

```bash
git add test/UnitTests/Core/Services/OrderServiceTests.cs
git commit -m "test: convert OrderServiceTests to NSubstitute

Every Moq Verify becomes an explicit Received/DidNotReceive. Also
replaces 'default' cancellation tokens with
TestContext.Current.CancellationToken."
```

---

### Task 8: Convert the authorization handler test and retire Moq

**Files:**
- Modify: `test/UnitTests/WebApi/Authorizations/DenyUnauthorizedUserHandlerTests.cs`
- Modify: `test/UnitTests/MoneyGroup.UnitTests.csproj`
- Modify: `test/UnitTests/packages.lock.json` (regenerated)

**Interfaces:**
- Consumes: NSubstitute from Task 6
- Produces: no Moq anywhere in the solution. A real `ClaimsPrincipal` replaces `Mock<ClaimsPrincipal>`.

> The current test mocks `ClaimsPrincipal.FindFirst` while production calls the `FindFirstValue` **extension method**, which internally delegates to `FindFirst`. That couples the test to a BCL implementation detail. A real `ClaimsPrincipal` removes the coupling and is shorter.

- [ ] **Step 1: Replace the whole file**

Write `test/UnitTests/WebApi/Authorizations/DenyUnauthorizedUserHandlerTests.cs`:

```csharp
using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.IdentityModel.JsonWebTokens;

using MoneyGroup.Core.Abstractions;
using MoneyGroup.Core.Models.Users;
using MoneyGroup.WebApi.Authorizations;

using NSubstitute;

namespace MoneyGroup.UnitTests.WebApi.Authorizations;

[Trait("Category", "Unit")]
public class DenyUnauthorizedUserHandlerTests
{
    private const string Email = "user@domain.com";

    private readonly IUserService _userService = Substitute.For<IUserService>();
    private readonly DenyUnauthorizedUserHandler _handler;

    public DenyUnauthorizedUserHandlerTests()
    {
        _handler = new DenyUnauthorizedUserHandler(
            NullLoggerFactory.Instance.CreateLogger<DenyUnauthorizedUserHandler>(),
            _userService);
    }

    private static AuthorizationHandlerContext ContextFor(ClaimsPrincipal user) =>
        new([new DenyUnauthorizedUserRequirement()], user, resource: null);

    private static ClaimsPrincipal Anonymous() => new(new ClaimsIdentity());

    private static ClaimsPrincipal Authenticated(params Claim[] claims) =>
        new(new ClaimsIdentity(claims, authenticationType: "TestAuth"));

    [Fact]
    public async Task GivenUser_WhenNotAuthenticated_ThenDoesNotSucceed()
    {
        // Arrange
        var context = ContextFor(Anonymous());

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task GivenUser_WhenEmailClaimMissing_ThenDoesNotSucceed()
    {
        // Arrange
        var context = ContextFor(Authenticated());

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task GivenUser_WhenEmailVerifiedClaimMissing_ThenDoesNotSucceed()
    {
        // Arrange
        var context = ContextFor(Authenticated(new Claim(ClaimTypes.Email, Email)));

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task GivenVerifiedEmail_WhenUserNotFound_ThenDoesNotSucceed()
    {
        // Arrange
        var context = ContextFor(Authenticated(
            new Claim(ClaimTypes.Email, Email),
            new Claim(JwtRegisteredClaimNames.EmailVerified, "true")));

        _userService.GetUserByEmailAsync(Email, Arg.Any<CancellationToken>())
            .Returns((UserDto?)null);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
        await _userService.Received(1).GetUserByEmailAsync(Email, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GivenVerifiedEmail_WhenUserFound_ThenSucceeds()
    {
        // Arrange
        var context = ContextFor(Authenticated(
            new Claim(ClaimTypes.Email, Email),
            new Claim(JwtRegisteredClaimNames.EmailVerified, "true")));

        _userService.GetUserByEmailAsync(Email, Arg.Any<CancellationToken>())
            .Returns(new UserDto { Id = 1, Name = "User", Email = Email });

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.True(context.HasSucceeded);
        await _userService.Received(1).GetUserByEmailAsync(Email, Arg.Any<CancellationToken>());
    }
}
```

This file also completes the Given_When_Then rename for these five tests, so Task 9 does not touch it.

- [ ] **Step 2: Remove the Moq package reference**

In `test/UnitTests/MoneyGroup.UnitTests.csproj`, delete:

```xml
    <PackageReference Include="Moq" />
```

- [ ] **Step 3: Regenerate the lock file**

```bash
dotnet restore test/UnitTests/MoneyGroup.UnitTests.csproj --force-evaluate
```

- [ ] **Step 4: Confirm Moq is gone from the whole solution**

```bash
grep -rn "Moq" test --include=*.cs --include=*.csproj
```

Expected: no output.

- [ ] **Step 5: Run tests**

```bash
dotnet test test/UnitTests/MoneyGroup.UnitTests.csproj
```

Expected: PASS, `total: 36`.

- [ ] **Step 6: Commit**

```bash
git add test/UnitTests/WebApi/Authorizations/DenyUnauthorizedUserHandlerTests.cs test/UnitTests/MoneyGroup.UnitTests.csproj test/UnitTests/packages.lock.json
git commit -m "test: convert authz handler test to NSubstitute, drop Moq

Replaces Mock<ClaimsPrincipal> with a real ClaimsPrincipal, removing
the coupling to FindFirstValue's internal use of FindFirst. Moq is now
absent from the solution."
```

---

### Task 9: Rename remaining tests to Given_When_Then

**Files:**
- Modify: `test/UnitTests/Core/Services/OrderServiceTests.cs`
- Modify: `test/UnitTests/WebApi/Validators/OrderDtoValidatorTests.cs`
- Modify: `test/UnitTests/WebApi/Validators/OrderPaginatedRequestValidatorTests.cs`
- Modify: `test/UnitTests/WebApi/Validators/ParticipantDtoValidatorTests.cs`

**Interfaces:**
- Produces: every test method in `UnitTests` named `Given…_When…_Then…`.

`OrderDtoValidatorTests` already uses this convention; only its `GivenOrderDto_WhenValid_ThenReturnNoError` style needs the `Then` verb normalised. `DenyUnauthorizedUserHandlerTests` was renamed in Task 8.

- [ ] **Step 1: Rename the OrderServiceTests methods**

| From | To |
|---|---|
| `GetOrderByIdAsync_ValidId_ShouldReturnOrder` | `GivenOrderId_WhenOrderExists_ThenReturnsOrder` |
| `GetOrderByIdAsync_InvalidId_ShouldReturnNull` | `GivenOrderId_WhenOrderMissing_ThenReturnsNull` |
| `CreateOrderAsync_ValidDto_ShouldAddOrder` | `GivenOrder_WhenBuyerAndParticipantsExist_ThenAddsOrder` |
| `CreateOrderAsync_NoParticipants_ShouldAddOrder` | `GivenOrder_WhenNoParticipants_ThenAddsOrderWithoutCountingParticipants` |
| `CreateOrderAsync_InvalidBuyer_ShouldThrowInvalidOperationException` | `GivenOrder_WhenBuyerMissing_ThenThrowsBuyerNotFound` |
| `CreateOrderAsync_InvalidParticipants_ShouldThrowInvalidOperationException` | `GivenOrder_WhenAnyParticipantMissing_ThenThrowsParticipantNotFound` |
| `RemoveOrderAsync_OrderExists_ShouldRemoveOrder` | `GivenOrderId_WhenOrderExists_ThenRemovesOrderAndReturnsTrue` |
| `RemoveOrderAsync_OrderNotFound_ShouldThrowInvalidOperationException` | `GivenOrderId_WhenOrderMissing_ThenReturnsFalse` |

The last two old names were actively wrong — neither throws.

- [ ] **Step 2: Rename the OrderPaginatedRequestValidatorTests methods**

| From | To |
|---|---|
| `Should_Have_Error_When_BuyerId_Is_Less_Than_Or_Equal_To_Zero` | `GivenRequest_WhenBuyerIdNotPositive_ThenHasErrorForBuyerId` |
| `Should_Have_Error_When_ParticipantId_Is_Less_Than_Or_Equal_To_Zero` | `GivenRequest_WhenParticipantIdNotPositive_ThenHasErrorForParticipantId` |
| `Should_Have_Error_When_TotalMax_Is_Less_Than_Or_Equal_To_Zero` | `GivenRequest_WhenTotalMaxNotPositive_ThenHasErrorForTotalMax` |
| `Should_Have_Error_When_TotalMin_Is_Less_Than_Or_Equal_To_Zero` | `GivenRequest_WhenTotalMinNotPositive_ThenHasErrorForTotalMin` |
| `Should_Have_Error_When_TotalMin_Is_Greater_Than_TotalMax` | `GivenRequest_WhenTotalMinExceedsTotalMax_ThenHasErrorForTotalMin` |
| `Should_Have_Error_When_Page_Is_Less_Than_One` | `GivenRequest_WhenPageBelowOne_ThenHasErrorForPage` |
| `Should_Have_Error_When_Size_Is_Less_Than_One` | `GivenRequest_WhenSizeBelowOne_ThenHasErrorForSize` |
| `Should_Not_Have_Error_When_Valid_Model` | `GivenRequest_WhenValid_ThenHasNoErrors` |

- [ ] **Step 3: Rename the ParticipantDtoValidatorTests method**

| From | To |
|---|---|
| `GivenOrderDto_WhenParticipantIdZero_ThenReturnError` | `GivenParticipant_WhenParticipantIdZero_ThenHasErrorForParticipantId` |

- [ ] **Step 4: Normalise the OrderDtoValidatorTests `Then` verbs**

| From | To |
|---|---|
| `GivenOrderDto_WhenValid_ThenReturnNoError` | `GivenOrderDto_WhenValid_ThenHasNoErrors` |
| `GivenOrderDto_WhenTitleEmpty_ThenReturnError` | `GivenOrderDto_WhenTitleEmpty_ThenHasErrorForTitle` |
| `GivenOrderDto_WhenDescriptionNull_ThenReturnNoError` | `GivenOrderDto_WhenDescriptionNull_ThenHasNoErrorForDescription` |
| `GivenOrderDto_WhenBuyerIdZero_ThenReturnError` | `GivenOrderDto_WhenBuyerIdZero_ThenHasErrorForBuyerId` |
| `GivenOrderDto_WhenTotalNegative_ThenReturnError` | `GivenOrderDto_WhenTotalNegative_ThenHasErrorForTotal` |
| `GivenOrderDto_WhenParticipantsNull_ThenReturnError` | `GivenOrderDto_WhenParticipantsNull_ThenHasErrorForParticipants` |
| `GivenOrderDto_WhenParticipantsEmpty_ThenReturnError` | `GivenOrderDto_WhenParticipantsEmpty_ThenHasErrorForParticipants` |
| `GivenOrderDto_WhenParticipantsContainsNull_ThenReturnError` | `GivenOrderDto_WhenParticipantsContainsNull_ThenHasErrorForParticipants` |
| `GivenOrderDto_WhenParticipantsDuplicate_ThenReturnError` | `GivenOrderDto_WhenParticipantsDuplicate_ThenHasDuplicatedParticipantError` |

- [ ] **Step 5: Verify no old-style names survive**

```bash
grep -rn "public.*Task\|public.*void" test/UnitTests --include=*Tests.cs | grep -v "Given" | grep -v "private" | grep -v "static"
```

Expected: no output (helper and factory methods are `private static` and therefore filtered out).

- [ ] **Step 6: Run tests**

```bash
dotnet test test/UnitTests/MoneyGroup.UnitTests.csproj
```

Expected: PASS, `total: 36`.

- [ ] **Step 7: Commit**

```bash
git add test/UnitTests
git commit -m "test: standardise on Given_When_Then naming

Also corrects two names that contradicted their assertions:
RemoveOrderAsync_OrderNotFound_ShouldThrowInvalidOperationException
asserted Assert.False and never threw."
```

---

### Task 10: Introduce test data builders

**Files:**
- Create: `test/UnitTests/Builders/OrderDtoBuilder.cs`
- Create: `test/UnitTests/Builders/UserDtoBuilder.cs`
- Modify: `test/UnitTests/WebApi/Validators/OrderDtoValidatorTests.cs`
- Modify: `test/UnitTests/Core/Services/OrderServiceTests.cs`

**Interfaces:**
- Produces:
  - `MoneyGroup.UnitTests.Builders.OrderDtoBuilder` — `static OrderDtoBuilder Valid()`, `WithTitle(string?)`, `WithDescription(string?)`, `WithTotal(decimal)`, `WithBuyer(int)`, `WithParticipants(params int[])`, `WithNoParticipants()`, `WithNullParticipants()`, `OrderDto Build()`
  - `MoneyGroup.UnitTests.Builders.UserDtoBuilder` — `static UserDtoBuilder Valid()`, `WithId(int)`, `WithName(string)`, `WithEmail(string?)`, `UserDto Build()`

- [ ] **Step 1: Write OrderDtoBuilder**

Create `test/UnitTests/Builders/OrderDtoBuilder.cs`:

```csharp
using MoneyGroup.Core.Models.Orders;

namespace MoneyGroup.UnitTests.Builders;

/// <summary>
/// Builds <see cref="OrderDto"/> instances for tests. <see cref="Valid"/> returns a
/// dto that passes every OrderDtoValidator rule, so each test only states the one
/// field it cares about.
/// </summary>
public sealed class OrderDtoBuilder
{
    private int _id;
    private string _title = "Title";
    private string? _description = "Description";
    private decimal _total;
    private int _buyerId = 1;
    private IEnumerable<ParticipantDto> _participants = [new ParticipantDto { ParticipantId = 1 }];

    public static OrderDtoBuilder Valid() => new();

    public OrderDtoBuilder WithId(int id)
    {
        _id = id;
        return this;
    }

    public OrderDtoBuilder WithTitle(string? title)
    {
        _title = title!;
        return this;
    }

    public OrderDtoBuilder WithDescription(string? description)
    {
        _description = description;
        return this;
    }

    public OrderDtoBuilder WithTotal(decimal total)
    {
        _total = total;
        return this;
    }

    public OrderDtoBuilder WithBuyer(int buyerId)
    {
        _buyerId = buyerId;
        return this;
    }

    public OrderDtoBuilder WithParticipants(params int[] participantIds)
    {
        _participants = participantIds.Select(id => new ParticipantDto { ParticipantId = id }).ToList();
        return this;
    }

    public OrderDtoBuilder WithParticipants(IEnumerable<ParticipantDto> participants)
    {
        _participants = participants;
        return this;
    }

    public OrderDtoBuilder WithNoParticipants()
    {
        _participants = [];
        return this;
    }

    public OrderDtoBuilder WithNullParticipants()
    {
        _participants = null!;
        return this;
    }

    public OrderDto Build() => new()
    {
        Id = _id,
        Title = _title,
        Description = _description,
        Total = _total,
        BuyerId = _buyerId,
        Participants = _participants,
    };
}
```

- [ ] **Step 2: Write UserDtoBuilder**

Create `test/UnitTests/Builders/UserDtoBuilder.cs`:

```csharp
using MoneyGroup.Core.Models.Users;

namespace MoneyGroup.UnitTests.Builders;

/// <summary>
/// Builds <see cref="UserDto"/> instances for tests.
/// </summary>
public sealed class UserDtoBuilder
{
    private int _id = 1;
    private string _name = "User";
    private string? _email = "user@domain.com";

    public static UserDtoBuilder Valid() => new();

    public UserDtoBuilder WithId(int id)
    {
        _id = id;
        return this;
    }

    public UserDtoBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public UserDtoBuilder WithEmail(string? email)
    {
        _email = email;
        return this;
    }

    public UserDto Build() => new()
    {
        Id = _id,
        Name = _name,
        Email = _email,
    };
}
```

- [ ] **Step 3: Build to confirm the builders compile**

```bash
dotnet build test/UnitTests/MoneyGroup.UnitTests.csproj
```

Expected: build succeeds with no warnings (warnings are errors).

- [ ] **Step 4: Adopt OrderDtoBuilder in OrderDtoValidatorTests**

Add `using MoneyGroup.UnitTests.Builders;` and replace each inline `new OrderDto() { … }` arrangement. For example, `GivenOrderDto_WhenValid_ThenHasNoErrors` becomes:

```csharp
        // Arrange
        var order = OrderDtoBuilder.Valid().Build();
```

and `GivenOrderDto_WhenParticipantsDuplicate_ThenHasDuplicatedParticipantError` becomes:

```csharp
        // Arrange
        var order = OrderDtoBuilder.Valid().WithParticipants(1, 1).Build();
```

The remaining seven follow the same shape: `WithTitle("   ")`, `WithDescription(null)`, `WithBuyer(0)`, `WithTotal(-1)`, `WithNullParticipants()`, `WithNoParticipants()`, and for the contains-null case `WithParticipants([null!, null!])`.

> Note the behavioural difference this exposes: the old tests constructed an `OrderDto` with *only* the field under test set, so unrelated rules also fired. Using `Valid()` means only the rule under test fires. `ShouldHaveValidationErrorFor` still passes, and the tests become genuinely single-purpose.

- [ ] **Step 5: Adopt OrderDtoBuilder in OrderServiceTests**

Replace the four inline `new OrderDto { … }` arrangements, e.g. in `GivenOrder_WhenBuyerAndParticipantsExist_ThenAddsOrder`:

```csharp
        var model = OrderDtoBuilder.Valid().WithBuyer(1).WithParticipants(2, 3).Build();
```

and in `GivenOrder_WhenNoParticipants_ThenAddsOrderWithoutCountingParticipants`:

```csharp
        var model = OrderDtoBuilder.Valid().WithBuyer(1).WithNoParticipants().Build();
```

- [ ] **Step 6: Run tests**

```bash
dotnet test test/UnitTests/MoneyGroup.UnitTests.csproj
```

Expected: PASS, `total: 36`.

- [ ] **Step 7: Verify full-suite parity — Phase 2 gate**

```bash
docker exec -w /mssql-server-setup-scripts.d moneygroup-mssql-1 bash ./reset.sh
dotnet test --solution MoneyGroup.slnx
```

Expected: `total: 63`, `failed: 0`, `skipped: 2`.

- [ ] **Step 8: Commit**

```bash
git add test/UnitTests/Builders test/UnitTests/Core test/UnitTests/WebApi
git commit -m "test: introduce OrderDtoBuilder and UserDtoBuilder

Collapses the duplicated inline DTO literals. Valid() returns an object
that passes every validator rule, so each test states only the field it
is about. Phase 2 complete: still 63 tests, no Moq, one naming
convention."
```

---

# Phase 3 — Close the unit-testable gaps

Test count grows from 63 to roughly 103.

---

### Task 11: Test UserService

**Files:**
- Create: `test/UnitTests/Core/Services/UserServiceTests.cs`

**Interfaces:**
- Consumes: `UserDtoBuilder` from Task 10
- Produces: 6 tests covering all three `UserService` methods.

- [ ] **Step 1: Write the failing tests**

Create `test/UnitTests/Core/Services/UserServiceTests.cs`:

```csharp
using MoneyGroup.Core.Abstractions;
using MoneyGroup.Core.Entities;
using MoneyGroup.Core.Models.Paginations;
using MoneyGroup.Core.Models.Users;
using MoneyGroup.Core.Services;
using MoneyGroup.Core.Specifications;
using MoneyGroup.UnitTests.Builders;

using NSubstitute;

namespace MoneyGroup.UnitTests.Core.Services;

[Trait("Category", "Unit")]
public class UserServiceTests
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userService = new UserService(_userRepository);
    }

    [Fact]
    public async Task GivenUserId_WhenUserExists_ThenReturnsUser()
    {
        // Arrange
        var user = UserDtoBuilder.Valid().WithId(7).Build();
        var cancellationToken = TestContext.Current.CancellationToken;

        _userRepository.FirstOrDefaultAsync<UserDto>(Arg.Any<EntityByIdSpec<User>>(), cancellationToken)
            .Returns(user);

        // Act
        var result = await _userService.GetUserByIdAsync(7, cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(7, result.Id);
        await _userRepository.Received(1)
            .FirstOrDefaultAsync<UserDto>(Arg.Any<EntityByIdSpec<User>>(), cancellationToken);
    }

    [Fact]
    public async Task GivenUserId_WhenUserMissing_ThenReturnsNull()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;

        _userRepository.FirstOrDefaultAsync<UserDto>(Arg.Any<EntityByIdSpec<User>>(), cancellationToken)
            .Returns((UserDto?)null);

        // Act
        var result = await _userService.GetUserByIdAsync(int.MaxValue, cancellationToken);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GivenEmail_WhenUserExists_ThenReturnsUser()
    {
        // Arrange
        var user = UserDtoBuilder.Valid().WithEmail("known@domain.com").Build();
        var cancellationToken = TestContext.Current.CancellationToken;

        _userRepository.FirstOrDefaultAsync<UserDto>(Arg.Any<UserByEmailSpec>(), cancellationToken)
            .Returns(user);

        // Act
        var result = await _userService.GetUserByEmailAsync("known@domain.com", cancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("known@domain.com", result.Email);
        await _userRepository.Received(1)
            .FirstOrDefaultAsync<UserDto>(Arg.Any<UserByEmailSpec>(), cancellationToken);
    }

    [Fact]
    public async Task GivenEmail_WhenUserMissing_ThenReturnsNull()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;

        _userRepository.FirstOrDefaultAsync<UserDto>(Arg.Any<UserByEmailSpec>(), cancellationToken)
            .Returns((UserDto?)null);

        // Act
        var result = await _userService.GetUserByEmailAsync("absent@domain.com", cancellationToken);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GivenPagingOptions_WhenUsersExist_ThenReturnsPaginatedUsers()
    {
        // Arrange
        var options = new UserPaginatedOptions(page: 1, size: 10);
        var expected = new PaginatedModel<UserDto>
        {
            Page = 1,
            Count = 1,
            Total = 1,
            Items = [UserDtoBuilder.Valid().Build()],
        };
        var cancellationToken = TestContext.Current.CancellationToken;

        _userRepository.GetByPageAsync<UserDto>(Arg.Any<UserPaginatedSpec>(), cancellationToken)
            .Returns(expected);

        // Act
        var result = await _userService.GetUsersByPageAsync(options, cancellationToken);

        // Assert
        Assert.Same(expected, result);
        await _userRepository.Received(1)
            .GetByPageAsync<UserDto>(Arg.Any<UserPaginatedSpec>(), cancellationToken);
    }

    [Fact]
    public async Task GivenPagingOptions_WhenKeywordSupplied_ThenPassesSpecificationBuiltFromOptions()
    {
        // Arrange
        var options = new UserPaginatedOptions(page: 2, size: 5) { Keyword = "ruong" };
        var cancellationToken = TestContext.Current.CancellationToken;

        _userRepository.GetByPageAsync<UserDto>(Arg.Any<UserPaginatedSpec>(), cancellationToken)
            .Returns(new PaginatedModel<UserDto> { Page = 2, Count = 0, Total = 0, Items = [] });

        // Act
        await _userService.GetUsersByPageAsync(options, cancellationToken);

        // Assert
        await _userRepository.Received(1).GetByPageAsync<UserDto>(
            Arg.Is<UserPaginatedSpec>(spec =>
                spec.PaginatedOptions.Page == 2 && spec.PaginatedOptions.Size == 5),
            cancellationToken);
    }
}
```

- [ ] **Step 2: Run to verify they pass**

```bash
dotnet test test/UnitTests/MoneyGroup.UnitTests.csproj
```

Expected: PASS, `total: 42`.

These tests describe existing behaviour, so they pass immediately — that is expected for characterisation tests of untested code. If any fails, the failure is a real defect: investigate before continuing.

- [ ] **Step 3: Commit**

```bash
git add test/UnitTests/Core/Services/UserServiceTests.cs
git commit -m "test: add UserService unit tests

Covers all three methods, including GetUserByEmailAsync which was
previously unreachable in tests because functional tests bypass
authorization."
```

---

### Task 12: Test the specifications

**Files:**
- Create: `test/UnitTests/Core/Specifications/SpecificationTests.cs`

**Interfaces:**
- Consumes: nothing new
- Produces: 14 tests. Uses `Specification<T>.Evaluate(IEnumerable<T>)` and `IsSatisfiedBy(T)` — both verified present on Ardalis.Specification 9.3.1.

- [ ] **Step 1: Write the tests**

Create `test/UnitTests/Core/Specifications/SpecificationTests.cs`:

```csharp
using MoneyGroup.Core.Entities;
using MoneyGroup.Core.Models.Orders;
using MoneyGroup.Core.Models.Paginations;
using MoneyGroup.Core.Models.Users;
using MoneyGroup.Core.Specifications;

namespace MoneyGroup.UnitTests.Core.Specifications;

[Trait("Category", "Unit")]
public class SpecificationTests
{
    private static Order Order(int id, int buyerId = 1, decimal total = 100, params int[] participantIds) => new()
    {
        Id = id,
        Title = $"Order {id}",
        BuyerId = buyerId,
        Total = total,
        Participants = [.. participantIds.Select(p => new OrderParticipant { ParticipantId = p, OrderId = id })],
    };

    [Fact]
    public void GivenEntityByIdSpec_WhenIdMatches_ThenIsSatisfied()
    {
        var spec = new EntityByIdSpec<Order>(5);

        Assert.True(spec.IsSatisfiedBy(Order(5)));
        Assert.False(spec.IsSatisfiedBy(Order(6)));
    }

    [Fact]
    public void GivenEntityByIdsSpec_WhenIdInSet_ThenIsSatisfied()
    {
        var spec = new EntityByIdsSpec<User>([1, 3]);

        Assert.True(spec.IsSatisfiedBy(new User { Id = 1, Name = "A" }));
        Assert.True(spec.IsSatisfiedBy(new User { Id = 3, Name = "C" }));
        Assert.False(spec.IsSatisfiedBy(new User { Id = 2, Name = "B" }));
    }

    [Fact]
    public void GivenEntityByIdsSpec_WhenIdsEmpty_ThenNothingIsSatisfied()
    {
        var spec = new EntityByIdsSpec<User>([]);

        Assert.False(spec.IsSatisfiedBy(new User { Id = 1, Name = "A" }));
    }

    [Fact]
    public void GivenUserByEmailSpec_WhenEmailMatches_ThenIsSatisfied()
    {
        var spec = new UserByEmailSpec("a@b.com");

        Assert.True(spec.IsSatisfiedBy(new User { Id = 1, Name = "A", Email = "a@b.com" }));
        Assert.False(spec.IsSatisfiedBy(new User { Id = 2, Name = "B", Email = "other@b.com" }));
        Assert.False(spec.IsSatisfiedBy(new User { Id = 3, Name = "C", Email = null }));
    }

    [Fact]
    public void GivenBasePaginatedSpec_WhenFirstPage_ThenSkipsNoneAndTakesPageSize()
    {
        var spec = new BasePaginatedSpecification<User>(new PaginatedOptions(page: 1, size: 3));

        Assert.Equal(0, spec.Skip);
        Assert.Equal(3, spec.Take);
    }

    [Fact]
    public void GivenBasePaginatedSpec_WhenThirdPage_ThenSkipsTwoPages()
    {
        var spec = new BasePaginatedSpecification<User>(new PaginatedOptions(page: 3, size: 10));

        Assert.Equal(20, spec.Skip);
        Assert.Equal(10, spec.Take);
    }

    [Fact]
    public void GivenBasePaginatedSpec_WhenEvaluated_ThenReturnsRequestedWindow()
    {
        var users = Enumerable.Range(1, 10)
            .Select(i => new User { Id = i, Name = $"U{i}" })
            .ToList();
        var spec = new BasePaginatedSpecification<User>(new PaginatedOptions(page: 2, size: 3));

        var result = spec.Evaluate(users).ToList();

        Assert.Equal(3, result.Count);
        Assert.Equal([4, 5, 6], result.Select(u => u.Id));
    }

    [Fact]
    public void GivenUserPaginatedSpec_WhenKeywordSupplied_ThenFiltersByNameContains()
    {
        var users = new List<User>
        {
            new() { Id = 1, Name = "Truong" },
            new() { Id = 2, Name = "Duc" },
            new() { Id = 3, Name = "Manh" },
        };
        var spec = new UserPaginatedSpec(new UserPaginatedOptions(1, 10) { Keyword = "ruo" });

        var result = spec.Evaluate(users).ToList();

        Assert.Equal([1], result.Select(u => u.Id));
    }

    [Fact]
    public void GivenUserPaginatedSpec_WhenKeywordBlank_ThenReturnsEveryone()
    {
        var users = new List<User>
        {
            new() { Id = 1, Name = "Truong" },
            new() { Id = 2, Name = "Duc" },
        };
        var spec = new UserPaginatedSpec(new UserPaginatedOptions(1, 10) { Keyword = "   " });

        var result = spec.Evaluate(users).ToList();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void GivenOrderPaginatedSpec_WhenNoFilters_ThenOrdersByIdDescending()
    {
        var orders = new List<Order> { Order(1), Order(3), Order(2) };
        var spec = new OrderPaginatedSpec(new OrderPaginatedOptions(null, null, null, null, 1, 10));

        var result = spec.Evaluate(orders).ToList();

        Assert.Equal([3, 2, 1], result.Select(o => o.Id));
    }

    [Fact]
    public void GivenOrderPaginatedSpec_WhenBuyerIdSupplied_ThenFiltersByBuyer()
    {
        var orders = new List<Order> { Order(1, buyerId: 1), Order(2, buyerId: 2) };
        var spec = new OrderPaginatedSpec(new OrderPaginatedOptions(2, null, null, null, 1, 10));

        var result = spec.Evaluate(orders).ToList();

        Assert.Equal([2], result.Select(o => o.Id));
    }

    [Fact]
    public void GivenOrderPaginatedSpec_WhenParticipantIdSupplied_ThenFiltersByParticipant()
    {
        var orders = new List<Order>
        {
            Order(1, participantIds: [1, 2]),
            Order(2, participantIds: [3]),
        };
        var spec = new OrderPaginatedSpec(new OrderPaginatedOptions(null, 3, null, null, 1, 10));

        var result = spec.Evaluate(orders).ToList();

        Assert.Equal([2], result.Select(o => o.Id));
    }

    [Fact]
    public void GivenOrderPaginatedSpec_WhenTotalBoundsSupplied_ThenFiltersInclusively()
    {
        var orders = new List<Order>
        {
            Order(1, total: 50),
            Order(2, total: 100),
            Order(3, total: 150),
        };
        var spec = new OrderPaginatedSpec(new OrderPaginatedOptions(null, null, 150m, 100m, 1, 10));

        var result = spec.Evaluate(orders).ToList();

        Assert.Equal([3, 2], result.Select(o => o.Id));
    }

    [Fact]
    public void GivenOrderPaginatedSpec_WhenAllFiltersCombined_ThenAppliesEveryFilter()
    {
        var orders = new List<Order>
        {
            Order(1, buyerId: 1, total: 100, participantIds: [1]),
            Order(2, buyerId: 1, total: 500, participantIds: [1]),
            Order(3, buyerId: 2, total: 100, participantIds: [1]),
            Order(4, buyerId: 1, total: 100, participantIds: [9]),
        };
        var spec = new OrderPaginatedSpec(new OrderPaginatedOptions(1, 1, 200m, 50m, 1, 10));

        var result = spec.Evaluate(orders).ToList();

        Assert.Equal([1], result.Select(o => o.Id));
    }
}
```

- [ ] **Step 2: Run the tests**

```bash
dotnet test test/UnitTests/MoneyGroup.UnitTests.csproj
```

Expected: PASS, `total: 56`.

`Evaluate(IEnumerable<T>)` applies `Skip`/`Take`: `Ardalis.Specification.PaginationEvaluator` implements `IInMemoryEvaluator`, verified by reflection against the 9.3.1 assembly. `WhereEvaluator` and `OrderEvaluator` do too, which is what makes the filter and ordering assertions valid in-memory.

- [ ] **Step 3: Commit**

```bash
git add test/UnitTests/Core/Specifications/SpecificationTests.cs
git commit -m "test: add specification unit tests

Covers all six specifications, including OrderPaginatedSpec's filter
combinations and ordering, which were previously exercised only
indirectly through repository integration tests."
```

---

### Task 13: Test the Mapperly mapper

**Files:**
- Create: `test/UnitTests/Infrastructure/Mapperly/MapperTests.cs`

**Interfaces:**
- Consumes: nothing new
- Produces: 6 tests over `MoneyGroup.Infrastructure.Mapperly.Mapper`.

> These contribute no coverage — Mapperly's generated code carries `[GeneratedCode]`. They are still the highest-value tests in this phase: a generated mapper silently dropping a property is exactly the failure a functional test's `Assert.NotNull` cannot catch.

- [ ] **Step 1: Write the tests**

Create `test/UnitTests/Infrastructure/Mapperly/MapperTests.cs`:

```csharp
using MoneyGroup.Core.Entities;
using MoneyGroup.Core.Models.Orders;
using MoneyGroup.Core.Models.Users;
using MoneyGroup.Infrastructure.Mapperly;
using MoneyGroup.UnitTests.Builders;

namespace MoneyGroup.UnitTests.Infrastructure.Mapperly;

[Trait("Category", "Unit")]
public class MapperTests
{
    private readonly Mapper _mapper = new();

    [Fact]
    public void GivenParticipantDto_WhenMapped_ThenCopiesParticipantId()
    {
        // Arrange
        var dto = new ParticipantDto { ParticipantId = 42 };

        // Act
        var entity = _mapper.Map(dto);

        // Assert
        Assert.Equal(42, entity.ParticipantId);
    }

    [Fact]
    public void GivenOrderDto_WhenMapped_ThenCopiesEveryScalarField()
    {
        // Arrange
        var dto = OrderDtoBuilder.Valid()
            .WithId(7)
            .WithTitle("Dinner")
            .WithDescription("Team dinner")
            .WithTotal(1234.56m)
            .WithBuyer(3)
            .WithParticipants(1, 2)
            .Build();

        // Act
        var entity = _mapper.Map(dto);

        // Assert
        Assert.Equal(7, entity.Id);
        Assert.Equal("Dinner", entity.Title);
        Assert.Equal("Team dinner", entity.Description);
        Assert.Equal(1234.56m, entity.Total);
        Assert.Equal(3, entity.BuyerId);
        Assert.Equal([1, 2], entity.Participants.Select(p => p.ParticipantId));
    }

    [Fact]
    public void GivenOrderDto_WhenMapped_ThenLeavesBuyerNavigationUnset()
    {
        // Arrange
        var dto = OrderDtoBuilder.Valid().WithBuyer(3).Build();

        // Act
        var entity = _mapper.Map(dto);

        // Assert
        Assert.Null(entity.Buyer);
    }

    [Fact]
    public void GivenOrderParticipant_WhenMapped_ThenFlattensParticipantName()
    {
        // Arrange
        var entity = new OrderParticipant
        {
            ParticipantId = 5,
            Participant = new User { Id = 5, Name = "Manh" },
        };

        // Act
        var dto = _mapper.Map(entity);

        // Assert
        Assert.Equal(5, dto.ParticipantId);
        Assert.Equal("Manh", dto.ParticipantName);
    }

    [Fact]
    public void GivenOrder_WhenMapped_ThenFlattensBuyerName()
    {
        // Arrange
        var entity = new Order
        {
            Id = 1,
            Title = "Order 1",
            Description = "desc",
            Total = 10_000m,
            BuyerId = 1,
            Buyer = new User { Id = 1, Name = "Truong" },
            Participants =
            [
                new() { ParticipantId = 2, Participant = new User { Id = 2, Name = "Duc" } },
            ],
        };

        // Act
        var dto = _mapper.Map(entity);

        // Assert
        Assert.Equal(1, dto.Id);
        Assert.Equal("Order 1", dto.Title);
        Assert.Equal(10_000m, dto.Total);
        Assert.Equal("Truong", dto.BuyerName);
        var participant = Assert.Single(dto.Participants);
        Assert.Equal(2, participant.ParticipantId);
        Assert.Equal("Duc", participant.ParticipantName);
    }

    [Fact]
    public void GivenUserQueryable_WhenProjected_ThenMapsIdNameAndEmail()
    {
        // Arrange
        var users = new List<User>
        {
            new() { Id = 1, Name = "Truong", Email = "t@d.com" },
            new() { Id = 2, Name = "Duc", Email = null },
        }.AsQueryable();

        // Act
        var projected = _mapper.Project(users).ToList();

        // Assert
        Assert.Equal(2, projected.Count);
        Assert.Equal([1, 2], projected.Select(u => u.Id));
        Assert.Equal(["Truong", "Duc"], projected.Select(u => u.Name));
        Assert.Equal("t@d.com", projected[0].Email);
        Assert.Null(projected[1].Email);
    }
}
```

- [ ] **Step 2: Run the tests**

```bash
dotnet test test/UnitTests/MoneyGroup.UnitTests.csproj
```

Expected: PASS, `total: 62`.

`GivenOrderDto_WhenMapped_ThenLeavesBuyerNavigationUnset` asserts `Assert.Null(entity.Buyer)` even though `Order.Buyer` is declared `= null!`. The `[MapperIgnoreTarget(nameof(Order.Buyer))]` attribute means Mapperly never assigns it, so it stays null at runtime. If this fails, Mapperly's ignore is not doing what the attribute claims — a real finding worth reporting.

- [ ] **Step 3: Commit**

```bash
git add test/UnitTests/Infrastructure/Mapperly/MapperTests.cs
git commit -m "test: add Mapperly mapper unit tests

Pins the entity/DTO boundary, including the BuyerName and
ParticipantName flattening and the ignored Buyer navigation. Adds no
coverage (generated code is attributed [GeneratedCode]) but catches
silent property drops that functional assertions cannot."
```

---

### Task 14: Test BusinessValidationExceptionHandler

**Files:**
- Create: `test/UnitTests/WebApi/Middlewares/BusinessValidationExceptionHandlerTests.cs`

**Interfaces:**
- Produces: 4 tests over `BusinessValidationExceptionHandler.TryHandleAsync`.

- [ ] **Step 1: Write the tests**

Create `test/UnitTests/WebApi/Middlewares/BusinessValidationExceptionHandlerTests.cs`:

```csharp
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using MoneyGroup.Core.Exceptions;
using MoneyGroup.WebApi.Middlewares;

using NSubstitute;

namespace MoneyGroup.UnitTests.WebApi.Middlewares;

[Trait("Category", "Unit")]
public class BusinessValidationExceptionHandlerTests
{
    private readonly IProblemDetailsService _problemDetailsService = Substitute.For<IProblemDetailsService>();
    private readonly BusinessValidationExceptionHandler _handler;

    public BusinessValidationExceptionHandlerTests()
    {
        _handler = new BusinessValidationExceptionHandler(_problemDetailsService);
    }

    [Fact]
    public async Task GivenUnrelatedException_WhenHandled_ThenReturnsFalseAndWritesNothing()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();

        // Act
        var handled = await _handler.TryHandleAsync(
            httpContext, new InvalidOperationException("boom"), TestContext.Current.CancellationToken);

        // Assert
        Assert.False(handled);
        await _problemDetailsService.DidNotReceive().WriteAsync(Arg.Any<ProblemDetailsContext>());
    }

    [Fact]
    public async Task GivenBuyerNotFound_WhenHandled_ThenReturnsTrue()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();

        // Act
        var handled = await _handler.TryHandleAsync(
            httpContext, new BuyerNotFoundException(), TestContext.Current.CancellationToken);

        // Assert
        Assert.True(handled);
    }

    [Fact]
    public async Task GivenBuyerNotFound_WhenHandled_ThenSetsBadRequestStatus()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();

        // Act
        await _handler.TryHandleAsync(
            httpContext, new BuyerNotFoundException(), TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(StatusCodes.Status400BadRequest, httpContext.Response.StatusCode);
    }

    [Fact]
    public async Task GivenParticipantNotFound_WhenHandled_ThenWritesProblemDetailsCarryingTheMessage()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        var exception = new ParticipantNotFoundException();

        // Act
        await _handler.TryHandleAsync(httpContext, exception, TestContext.Current.CancellationToken);

        // Assert
        await _problemDetailsService.Received(1).WriteAsync(
            Arg.Is<ProblemDetailsContext>(ctx =>
                ctx.HttpContext == httpContext
                && ctx.Exception == exception
                && ctx.ProblemDetails.Detail == "Participant not found"));
    }
}
```

- [ ] **Step 2: Run the tests**

```bash
dotnet test test/UnitTests/MoneyGroup.UnitTests.csproj
```

Expected: PASS, `total: 66`.

- [ ] **Step 3: Commit**

```bash
git add test/UnitTests/WebApi/Middlewares/BusinessValidationExceptionHandlerTests.cs
git commit -m "test: add BusinessValidationExceptionHandler unit tests

Covers the non-business-exception passthrough, the 400 status, and that
the exception message reaches ProblemDetails.Detail."
```

---

### Task 15: Test OrderService.GetOrdersByPageAsync

**Files:**
- Modify: `test/UnitTests/Core/Services/OrderServiceTests.cs`

**Interfaces:**
- Produces: 2 additional tests on the existing class.

- [ ] **Step 1: Add the tests**

Append these two methods to `OrderServiceTests`, and add `using MoneyGroup.Core.Models.Paginations;` to the file's usings:

```csharp
    [Fact]
    public async Task GivenPagingOptions_WhenOrdersExist_ThenReturnsPaginatedOrders()
    {
        // Arrange
        var options = new OrderPaginatedOptions(null, null, null, null, 1, 10);
        var expected = new PaginatedModel<OrderDetailedDto>
        {
            Page = 1,
            Count = 1,
            Total = 1,
            Items = [new OrderDetailedDto { Id = 1, Title = "Order 1" }],
        };

        _orderRepository.GetByPageAsync<OrderDetailedDto>(Arg.Any<OrderPaginatedSpec>(), Arg.Any<CancellationToken>())
            .Returns(expected);

        // Act
        var result = await _orderService.GetOrdersByPageAsync(options);

        // Assert
        Assert.Same(expected, result);
    }

    [Fact]
    public async Task GivenPagingOptions_WhenCalled_ThenPassesSpecificationBuiltFromOptions()
    {
        // Arrange
        var options = new OrderPaginatedOptions(buyerId: 4, null, null, null, page: 3, size: 7);

        _orderRepository.GetByPageAsync<OrderDetailedDto>(Arg.Any<OrderPaginatedSpec>(), Arg.Any<CancellationToken>())
            .Returns(new PaginatedModel<OrderDetailedDto> { Page = 3, Count = 0, Total = 0, Items = [] });

        // Act
        await _orderService.GetOrdersByPageAsync(options);

        // Assert
        await _orderRepository.Received(1).GetByPageAsync<OrderDetailedDto>(
            Arg.Is<OrderPaginatedSpec>(spec => spec.Skip == 14 && spec.Take == 7),
            Arg.Any<CancellationToken>());
    }
```

`Skip == 14` is `(3 − 1) × 7`, from `BasePaginatedSpecification`.

- [ ] **Step 2: Run the tests**

```bash
dotnet test test/UnitTests/MoneyGroup.UnitTests.csproj
```

Expected: PASS, `total: 68`.

- [ ] **Step 3: Commit**

```bash
git add test/UnitTests/Core/Services/OrderServiceTests.cs
git commit -m "test: cover OrderService.GetOrdersByPageAsync

The only OrderService method with no unit test. Asserts the paging
specification is built from the supplied options."
```

---

### Task 16: Make endpoint handlers testable

**Files:**
- Modify: `src/WebApi/Endpoints/OrderEndpoints.cs`
- Modify: `src/WebApi/Endpoints/UserEndpoints.cs`
- Modify: `src/WebApi/MoneyGroup.WebApi.csproj`

**Interfaces:**
- Produces: all 7 handlers become `internal static`, callable from `MoneyGroup.UnitTests`:
  - `OrderEndpoints.GetOrdersAsync(OrderPaginatedRequest, IOrderService)` → `Task<Results<Ok<PaginatedModel<OrderDetailedDto>>, ValidationProblem>>`
  - `OrderEndpoints.GetOrderByIdAsync(int, IOrderService)` → `Task<Results<Ok<OrderDetailedDto>, NotFound>>`
  - `OrderEndpoints.CreateOrderAsync(OrderDto, IOrderService)` → `Task<Results<CreatedAtRoute<OrderDto>, ValidationProblem>>`
  - `OrderEndpoints.DeleteOrderAsync(int, IOrderService, CancellationToken)` → `Task<Results<NoContent, NotFound>>`
  - `UserEndpoints.GetUsersAsync(UserPaginatedRequest, IUserService)` → `Task<Results<Ok<PaginatedModel<UserDto>>, ValidationProblem>>`
  - `UserEndpoints.GetUserByIdAsync(int, IUserService, CancellationToken)` → `Task<Results<Ok<UserDto>, NotFound>>`
  - `UserEndpoints.GetExecutingUser(HttpContext)` → `Ok<UserDto>`

**This is the only production-code change in the entire plan.**

- [ ] **Step 1: Normalise OrderEndpoints handler accessibility**

In `src/WebApi/Endpoints/OrderEndpoints.cs`, change all four handler declarations to `internal static`:

```csharp
    internal static async Task<Results<Ok<PaginatedModel<OrderDetailedDto>>, ValidationProblem>> GetOrdersAsync([AsParameters] OrderPaginatedRequest request, [FromServices] IOrderService orderService)
```

```csharp
    internal static async Task<Results<CreatedAtRoute<OrderDto>, ValidationProblem>> CreateOrderAsync(OrderDto input, IOrderService orderService)
```

```csharp
    internal static async Task<Results<NoContent, NotFound>> DeleteOrderAsync(int id, IOrderService orderService, CancellationToken cancellationToken)
```

```csharp
    internal static async Task<Results<Ok<OrderDetailedDto>, NotFound>> GetOrderByIdAsync(int id, IOrderService orderService)
```

The last one was `public static` — the inconsistency this step removes.

- [ ] **Step 2: Normalise UserEndpoints handler accessibility**

In `src/WebApi/Endpoints/UserEndpoints.cs`:

```csharp
    internal static async Task<Results<Ok<PaginatedModel<UserDto>>, ValidationProblem>> GetUsersAsync([AsParameters] UserPaginatedRequest request, [FromServices] IUserService userService)
```

```csharp
    internal static async Task<Results<Ok<UserDto>, NotFound>> GetUserByIdAsync(int id, [FromServices] IUserService userService, CancellationToken cancellationToken)
```

```csharp
    internal static Ok<UserDto> GetExecutingUser(HttpContext httpContext)
```

- [ ] **Step 3: Grant UnitTests access to internals**

In `src/WebApi/MoneyGroup.WebApi.csproj`, add to the item group that already contains the `InternalsVisibleTo` entry:

```xml
    <InternalsVisibleTo Include="MoneyGroup.UnitTests" />
```

so it reads:

```xml
    <InternalsVisibleTo Include="MoneyGroup.FunctionalTests" />
    <InternalsVisibleTo Include="MoneyGroup.UnitTests" />
```

- [ ] **Step 4: Verify the application still builds and routes**

```bash
dotnet build MoneyGroup.slnx
```

Expected: build succeeds. Minimal-API endpoint registration uses method-group references inside the same class, so accessibility changes do not affect routing.

- [ ] **Step 5: Verify the full suite still passes**

```bash
docker exec -w /mssql-server-setup-scripts.d moneygroup-mssql-1 bash ./reset.sh
dotnet test --solution MoneyGroup.slnx
```

Expected: `failed: 0`. Functional endpoint tests confirm routing is unaffected.

- [ ] **Step 6: Discard the regenerated OpenAPI document if it appears**

```bash
git status --short src/WebApi/MoneyGroup.WebApi.json
```

If listed, confirm it is line-endings only and discard it:

```bash
git diff --numstat src/WebApi/MoneyGroup.WebApi.json
git checkout -- src/WebApi/MoneyGroup.WebApi.json
```

Empty `--numstat` output means no content changed.

- [ ] **Step 7: Commit**

```bash
git add src/WebApi/Endpoints/OrderEndpoints.cs src/WebApi/Endpoints/UserEndpoints.cs src/WebApi/MoneyGroup.WebApi.csproj
git commit -m "refactor(api): make endpoint handlers internal for unit testing

Normalises all seven handlers to 'internal static', removing the
existing inconsistency where GetOrderByIdAsync and GetUserByIdAsync were
public while their siblings were private. Extends the InternalsVisibleTo
pattern already used for FunctionalTests and IntegrationTests."
```

---

### Task 17: Test the endpoint handlers

**Files:**
- Create: `test/UnitTests/WebApi/Endpoints/OrderEndpointsTests.cs`
- Create: `test/UnitTests/WebApi/Endpoints/UserEndpointsTests.cs`

**Interfaces:**
- Consumes: the `internal static` handlers from Task 16, `OrderDtoBuilder` and `UserDtoBuilder` from Task 10
- Produces: 11 tests.

- [ ] **Step 1: Write OrderEndpointsTests**

Create `test/UnitTests/WebApi/Endpoints/OrderEndpointsTests.cs`:

```csharp
using Microsoft.AspNetCore.Http.HttpResults;

using MoneyGroup.Core.Abstractions;
using MoneyGroup.Core.Models.Orders;
using MoneyGroup.Core.Models.Paginations;
using MoneyGroup.UnitTests.Builders;
using MoneyGroup.WebApi.Endpoints;

using NSubstitute;

namespace MoneyGroup.UnitTests.WebApi.Endpoints;

[Trait("Category", "Unit")]
public class OrderEndpointsTests
{
    private readonly IOrderService _orderService = Substitute.For<IOrderService>();

    [Fact]
    public async Task GivenOrderId_WhenOrderExists_ThenReturnsOk()
    {
        // Arrange
        var order = new OrderDetailedDto { Id = 1, Title = "Order 1" };
        _orderService.GetOrderByIdAsync(1, Arg.Any<CancellationToken>()).Returns(order);

        // Act
        var result = await OrderEndpoints.GetOrderByIdAsync(1, _orderService);

        // Assert
        var ok = Assert.IsType<Ok<OrderDetailedDto>>(result.Result);
        Assert.Same(order, ok.Value);
    }

    [Fact]
    public async Task GivenOrderId_WhenOrderMissing_ThenReturnsNotFound()
    {
        // Arrange
        _orderService.GetOrderByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns((OrderDetailedDto?)null);

        // Act
        var result = await OrderEndpoints.GetOrderByIdAsync(int.MaxValue, _orderService);

        // Assert
        Assert.IsType<NotFound>(result.Result);
    }

    [Fact]
    public async Task GivenOrderId_WhenOrderRemoved_ThenReturnsNoContent()
    {
        // Arrange
        _orderService.RemoveOrderAsync(1, Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var result = await OrderEndpoints.DeleteOrderAsync(1, _orderService, TestContext.Current.CancellationToken);

        // Assert
        Assert.IsType<NoContent>(result.Result);
    }

    [Fact]
    public async Task GivenOrderId_WhenNothingRemoved_ThenReturnsNotFound()
    {
        // Arrange
        _orderService.RemoveOrderAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(false);

        // Act
        var result = await OrderEndpoints.DeleteOrderAsync(
            int.MaxValue, _orderService, TestContext.Current.CancellationToken);

        // Assert
        Assert.IsType<NotFound>(result.Result);
    }

    [Fact]
    public async Task GivenOrder_WhenCreated_ThenReturnsCreatedAtRouteWithNewId()
    {
        // Arrange
        var input = OrderDtoBuilder.Valid().WithBuyer(1).WithParticipants(1, 2).Build();
        _orderService
            .When(s => s.CreateOrderAsync(input, Arg.Any<CancellationToken>()))
            .Do(_ => input.Id = 99);

        // Act
        var result = await OrderEndpoints.CreateOrderAsync(input, _orderService);

        // Assert
        var created = Assert.IsType<CreatedAtRoute<OrderDto>>(result.Result);
        Assert.Same(input, created.Value);
        Assert.Equal("GetOrderById", created.RouteName);
        Assert.Equal(99, created.RouteValues["id"]);
    }

    [Fact]
    public async Task GivenPagingRequest_WhenCalled_ThenReturnsOkWithServiceResult()
    {
        // Arrange
        var request = new OrderPaginatedRequest(null, null, null, null, 1, 10);
        var expected = new PaginatedModel<OrderDetailedDto>
        {
            Page = 1,
            Count = 0,
            Total = 0,
            Items = [],
        };
        _orderService.GetOrdersByPageAsync(request).Returns(expected);

        // Act
        var result = await OrderEndpoints.GetOrdersAsync(request, _orderService);

        // Assert
        var ok = Assert.IsType<Ok<PaginatedModel<OrderDetailedDto>>>(result.Result);
        Assert.Same(expected, ok.Value);
    }
}
```

- [ ] **Step 2: Write UserEndpointsTests**

Create `test/UnitTests/WebApi/Endpoints/UserEndpointsTests.cs`:

```csharp
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

using MoneyGroup.Core.Abstractions;
using MoneyGroup.Core.Models.Paginations;
using MoneyGroup.Core.Models.Users;
using MoneyGroup.UnitTests.Builders;
using MoneyGroup.WebApi.Endpoints;
using MoneyGroup.WebApi.Features;

using NSubstitute;

namespace MoneyGroup.UnitTests.WebApi.Endpoints;

[Trait("Category", "Unit")]
public class UserEndpointsTests
{
    private readonly IUserService _userService = Substitute.For<IUserService>();

    [Fact]
    public async Task GivenUserId_WhenUserExists_ThenReturnsOk()
    {
        // Arrange
        var user = UserDtoBuilder.Valid().WithId(1).Build();
        _userService.GetUserByIdAsync(1, Arg.Any<CancellationToken>()).Returns(user);

        // Act
        var result = await UserEndpoints.GetUserByIdAsync(
            1, _userService, TestContext.Current.CancellationToken);

        // Assert
        var ok = Assert.IsType<Ok<UserDto>>(result.Result);
        Assert.Same(user, ok.Value);
    }

    [Fact]
    public async Task GivenUserId_WhenUserMissing_ThenReturnsNotFound()
    {
        // Arrange
        _userService.GetUserByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns((UserDto?)null);

        // Act
        var result = await UserEndpoints.GetUserByIdAsync(
            int.MaxValue, _userService, TestContext.Current.CancellationToken);

        // Assert
        Assert.IsType<NotFound>(result.Result);
    }

    [Fact]
    public async Task GivenPagingRequest_WhenCalled_ThenReturnsOkWithServiceResult()
    {
        // Arrange
        var request = new UserPaginatedRequest(keyword: null, page: 1, size: 10);
        var expected = new PaginatedModel<UserDto> { Page = 1, Count = 0, Total = 0, Items = [] };
        _userService.GetUsersByPageAsync(request, Arg.Any<CancellationToken>()).Returns(expected);

        // Act
        var result = await UserEndpoints.GetUsersAsync(request, _userService);

        // Assert
        var ok = Assert.IsType<Ok<PaginatedModel<UserDto>>>(result.Result);
        Assert.Same(expected, ok.Value);
    }

    [Fact]
    public void GivenHttpContext_WhenCurrentUserFeaturePresent_ThenReturnsThatUser()
    {
        // Arrange
        var user = UserDtoBuilder.Valid().WithId(5).Build();
        var httpContext = new DefaultHttpContext();
        httpContext.Features.Set<ICurrentUserFeature>(new CurrentUserFeature { User = user });

        // Act
        var result = UserEndpoints.GetExecutingUser(httpContext);

        // Assert
        Assert.Same(user, result.Value);
    }

    [Fact]
    public void GivenHttpContext_WhenCurrentUserFeatureMissing_ThenThrowsArgumentNull()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => UserEndpoints.GetExecutingUser(httpContext));
    }
}
```

- [ ] **Step 3: Run the tests**

```bash
dotnet test test/UnitTests/MoneyGroup.UnitTests.csproj
```

Expected: PASS, `total: 79`.

- [ ] **Step 4: Final full-suite verification — Phase 3 gate**

```bash
docker exec -w /mssql-server-setup-scripts.d moneygroup-mssql-1 bash ./reset.sh
dotnet test --solution MoneyGroup.slnx
```

Expected: `total: 106` (79 unit + 6 integration + 20 functional + 1 smoke), `failed: 0`, `skipped: 2`.

- [ ] **Step 5: Verify formatting and coverage**

```bash
dotnet format MoneyGroup.slnx --verify-no-changes -v diag --severity info
```

Expected: no changes required.

```bash
dotnet test --solution MoneyGroup.slnx --coverage --config-file $PWD/testconfig.json
dotnet coverage merge ./test/*/bin/Debug/net10.0/TestResults/*.xml --output coverage.xml --output-format xml
```

Expected: roughly 68.4% overall. **This looks low because the denominator still counts generated code — that is expected and correct. Do not modify `testconfig.json`.**

- [ ] **Step 6: Commit**

```bash
git add test/UnitTests/WebApi/Endpoints
git commit -m "test: add endpoint handler unit tests

Handlers return typed Results<,>, so Ok/NotFound branches assert
directly with no host and no database. Covers GetExecutingUser, whose
CurrentUserFeature path had no test because the functional suite's two
/my tests are skipped."
```

- [ ] **Step 7: Clean up the coverage artifact**

```bash
git status --short
```

If `coverage.xml` is untracked, delete it — it is a build artifact:

```bash
rm -f coverage.xml
```

Confirm the tree is clean before finishing.

---

## Completion criteria

- [ ] `dotnet test --solution MoneyGroup.slnx` reports **106 total, 0 failed, 2 skipped** (after a seed reset)
- [ ] `dotnet format MoneyGroup.slnx --verify-no-changes` passes
- [ ] `grep -rn "Moq" test --include=*.cs --include=*.csproj` returns nothing
- [ ] Every test method in `test/UnitTests` is named `Given…_When…_Then…`
- [ ] `testconfig.json` is unmodified
- [ ] No `#pragma warning disable` was added
- [ ] Working tree is clean; `src/WebApi/MoneyGroup.WebApi.json` is unmodified

## Known-open on completion

The functional suite still corrupts its own seed data — `DeleteOrder_ValidId_ReturnsNoContent` deletes Order 3 and `CreateOrder_ValidDto_ReturnsCreatedOrder` leaks rows, so a second consecutive local run fails. This is deliberate: Slice 2 (Testcontainers + Respawn) fixes the root cause. Run the seed reset before any full-suite run.
