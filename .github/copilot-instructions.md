# GitHub Copilot Instructions for MoneyGroup

## Project Context

MoneyGroup is a .NET 10 Clean Architecture backend API with:
- **Core**: Entities, Services, Interfaces, DTOs, Specifications, Validators
- **Infrastructure**: `EfRepository<T>`, `ApplicationDbContext`, Mapperly mappers
- **WebApi**: Minimal API Endpoints, Middleware, Authorization

**Key Rule**: Core NEVER depends on Infrastructure or WebApi.

---

## File Locations

| Component | Location |
|-----------|----------|
| Entity | `src/Core/Entities/{Name}.cs` |
| DTO | `src/Core/Models/{Plural}/{Name}Dto.cs` |
| Service Interface | `src/Core/Abstractions/I{Name}Service.cs` |
| Service Implementation | `src/Core/Services/{Name}Service.cs` |
| Repository Interface | `src/Core/Abstractions/I{Name}Repository.cs` |
| Repository Implementation | `src/Infrastructure/Data/{Name}Repository.cs` |
| Specification | `src/Core/Specifications/{Name}Spec.cs` |
| Validator | `src/Core/Validators/{Name}Validator.cs` |
| Endpoint | `src/WebApi/Endpoints/{Name}Endpoints.cs` |
| Migration (SQL Server) | `src/Infrastructure.SqlServer/Data/Migrations/` |
| Seed Scripts | `src/Infrastructure.SqlServer/Docker/scripts/` |
| Unit Test | `test/UnitTests/{Layer}/{Class}Tests.cs` |
| Integration Test | `test/IntegrationTests/{Category}/{Class}Tests.cs` |
| Functional Test | `test/FunctionalTests/{Category}/{Class}Tests.cs` |

---

## Code Templates

### Entity

```csharp
namespace MoneyGroup.Core.Entities;

public class Payment : EntityBase
{
    public string Description { get; set; } = null!;
    public decimal Amount { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
}
```

### DTO

```csharp
namespace MoneyGroup.Core.Models.Payments;

public class PaymentDto
{
    public int Id { get; set; }
    public string Description { get; set; } = null!;
    public decimal Amount { get; set; }
    public int OrderId { get; set; }
}

public class PaymentDetailedDto
{
    public int Id { get; set; }
    public string Description { get; set; } = null!;
    public decimal Amount { get; set; }
    public int OrderId { get; set; }
    public OrderDto Order { get; set; } = null!;
}
```

### Repository Interface

```csharp
namespace MoneyGroup.Core.Abstractions;

public interface IPaymentRepository : IRepository<Payment>
{
    Task<PaymentDto> AddAsync(PaymentDto dto, CancellationToken cancellationToken = default);
}
```

### Repository Implementation

```csharp
namespace MoneyGroup.Infrastructure.Data;

public sealed class PaymentRepository : EfRepository<Payment>, IPaymentRepository
{
    public PaymentRepository(ApplicationDbContext context, IMapper mapper)
        : base(context, mapper) { }

    public async Task<PaymentDto> AddAsync(PaymentDto dto, CancellationToken cancellationToken = default)
    {
        var entity = _mapper.Map<Payment>(dto);

        await _dbSet.AddAsync(entity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        dto.Id = entity.Id;
        return dto;
    }
}
```

### Service Interface

```csharp
namespace MoneyGroup.Core.Abstractions;

public interface IPaymentService
{
    Task<PaymentDetailedDto?> GetPaymentByIdAsync(int id, CancellationToken ct = default);
    Task CreatePaymentAsync(PaymentDto model, CancellationToken ct = default);
    ValueTask<bool> RemovePaymentAsync(int id, CancellationToken ct = default);
    Task<PaginatedModel<PaymentDetailedDto>> GetPaymentsByPageAsync(PaymentPaginatedOptions options);
}
```

### Service Implementation

```csharp
namespace MoneyGroup.Core.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IOrderRepository _orderRepository;

    public PaymentService(
        IPaymentRepository paymentRepository,
        IOrderRepository orderRepository)
    {
        _paymentRepository = paymentRepository;
        _orderRepository = orderRepository;
    }

    public async Task<PaymentDetailedDto?> GetPaymentByIdAsync(int id, CancellationToken ct = default)
    {
        return await _paymentRepository.FirstOrDefaultAsync<PaymentDetailedDto>(
            new EntityByIdSpec<Payment>(id), ct);
    }

    public async Task CreatePaymentAsync(PaymentDto model, CancellationToken ct = default)
    {
        if (!await _orderRepository.AnyAsync(new EntityByIdSpec<Order>(model.OrderId), ct))
        {
            throw new OrderNotFoundException();
        }
        await _paymentRepository.AddAsync(model, ct);
    }

    public async ValueTask<bool> RemovePaymentAsync(int id, CancellationToken ct = default)
    {
        var entity = await _paymentRepository.FirstOrDefaultAsync(
            new EntityByIdSpec<Payment>(id), ct);
        if (entity == null) return false;
        await _paymentRepository.RemoveAsync(entity, ct);
        return true;
    }

    public Task<PaginatedModel<PaymentDetailedDto>> GetPaymentsByPageAsync(PaymentPaginatedOptions options)
    {
        return _paymentRepository.GetByPageAsync<PaymentDetailedDto>(
            new PaymentPaginatedSpec(options));
    }
}
```

### Endpoint

```csharp
namespace MoneyGroup.WebApi.Endpoints;

public static class PaymentEndpoints
{
    public static void MapPaymentEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Payment")
            .AddFluentValidationAutoValidation()
            .RequireAuthorization()
            .WithTags("Payment");

        group.MapGet("/", GetPaymentsAsync).WithName("GetPayments");
        group.MapGet("/{id:int}", GetPaymentByIdAsync).WithName("GetPaymentById");
        group.MapPost("/", CreatePaymentAsync).WithName("CreatePayment");
        group.MapDelete("/{id:int}", DeletePaymentAsync).WithName("DeletePayment");
    }

    private static async Task<Results<Ok<PaginatedModel<PaymentDetailedDto>>, ValidationProblem>> GetPaymentsAsync(
        [AsParameters] PaymentPaginatedRequest request,
        [FromServices] IPaymentService service)
    {
        var result = await service.GetPaymentsByPageAsync(request);
        return TypedResults.Ok(result);
    }

    private static async Task<Results<Ok<PaymentDetailedDto>, NotFound>> GetPaymentByIdAsync(
        int id, IPaymentService service)
    {
        var result = await service.GetPaymentByIdAsync(id);
        return result == null ? TypedResults.NotFound() : TypedResults.Ok(result);
    }

    private static async Task<Results<CreatedAtRoute<PaymentDto>, ValidationProblem>> CreatePaymentAsync(
        PaymentDto input, IPaymentService service)
    {
        await service.CreatePaymentAsync(input);
        return TypedResults.CreatedAtRoute(input, "GetPaymentById", new { id = input.Id });
    }

    private static async Task<Results<NoContent, NotFound>> DeletePaymentAsync(
        int id, IPaymentService service, CancellationToken ct)
    {
        var result = await service.RemovePaymentAsync(id, ct);
        return !result ? TypedResults.NotFound() : TypedResults.NoContent();
    }
}
```

### Specification

```csharp
namespace MoneyGroup.Core.Specifications;

public class PaymentPaginatedSpec : Specification<Payment>, IPaginatedSpecification<Payment>
{
    public IPaginatedOptions PaginatedOptions { get; }

    public PaymentPaginatedSpec(PaymentPaginatedOptions options)
    {
        PaginatedOptions = options;
        Query.OrderBy(p => p.Id)
             .Skip(options.Skip)
             .Take(options.Take);

        if (options.OrderId.HasValue)
            Query.Where(p => p.OrderId == options.OrderId.Value);
    }
}
```

### Validator

```csharp
namespace MoneyGroup.Core.Validators;

public class PaymentDtoValidator : AbstractValidator<PaymentDto>
{
    public PaymentDtoValidator()
    {
        RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.OrderId).GreaterThan(0);
    }
}
```

### Unit Test

```csharp
namespace MoneyGroup.UnitTests.Services;

public class PaymentServiceTests
{
    private readonly Mock<IPaymentRepository> _paymentRepo = new();
    private readonly Mock<IOrderRepository> _orderRepo = new();

    [Fact]
    public async Task CreatePaymentAsync_OrderNotFound_ThrowsException()
    {
        // Arrange
        _orderRepo.Setup(r => r.AnyAsync(It.IsAny<ISpecification<Order>>(), default))
            .ReturnsAsync(false);
        var service = new PaymentService(_paymentRepo.Object, _orderRepo.Object);

        // Act & Assert
        await Assert.ThrowsAsync<OrderNotFoundException>(
            () => service.CreatePaymentAsync(new PaymentDto { OrderId = 999 }));
    }

    [Fact]
    public async Task CreatePaymentAsync_ValidPayment_CallsRepository()
    {
        // Arrange
        _orderRepo.Setup(r => r.AnyAsync(It.IsAny<ISpecification<Order>>(), default))
            .ReturnsAsync(true);
        var service = new PaymentService(_paymentRepo.Object, _orderRepo.Object);
        var dto = new PaymentDto { Description = "Test", Amount = 100, OrderId = 1 };

        // Act
        await service.CreatePaymentAsync(dto);

        // Assert
        _paymentRepo.Verify(r => r.AddAsync(dto, default), Times.Once);
    }
}
```

---

## Workflows

### Add New Entity

1. Create Entity in `src/Core/Entities/`
2. Add `DbSet<T>` to `ApplicationDbContext`
3. Configure relationships in `OnModelCreating()`
4. Create DTOs in `src/Core/Models/{Plural}/`
5. Create Repository Interface in `src/Core/Abstractions/`
6. Implement Repository in `src/Infrastructure/Data/`
7. Create Service Interface in `src/Core/Abstractions/`
8. Implement Service in `src/Core/Services/`
9. Create Validator in `src/Core/Validators/`
10. Create Endpoints in `src/WebApi/Endpoints/`
11. Register in `Program.cs`:
    ```csharp
    builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
    builder.Services.AddScoped<IPaymentService, PaymentService>();
    app.MapPaymentEndpoints();
    ```
12. Create Migration:
    ```bash
    cd src/Infrastructure.SqlServer
    .\migrations-add.ps1 -Name "AddPayment"
    ```
13. Register DTOs in `AppJsonSerializerContext.cs`:
    ```csharp
    [JsonSerializable(typeof(PaymentDto))]
    [JsonSerializable(typeof(PaymentDetailedDto))]
    [JsonSerializable(typeof(PaginatedModel<PaymentDetailedDto>))]
    ```
14. Write tests (Unit, Integration, Functional)

### Add New Endpoint

1. Add method to Service Interface
2. Implement in Service
3. Create DTO if needed
4. Add Validator if needed
5. Add endpoint handler to `{Entity}Endpoints.cs`
6. Write functional test

### Fix Bug

1. Reproduce and understand the issue
2. Locate problematic code
3. Implement minimal fix following existing patterns
4. Add regression test
5. Run `dotnet test` to verify

---

## Conventions

### Naming

- Entities: `Order`, `User`, `Payment`
- DTOs: `OrderDto`, `OrderDetailedDto`
- Interfaces: `IOrderService`, `IOrderRepository`
- Services: `OrderService`
- Repositories: `OrderRepository` (extends `EfRepository<T>`)
- Specifications: `OrderPaginatedSpec`, `EntityByIdSpec<T>`
- Validators: `OrderDtoValidator`
- Endpoints: `OrderEndpoints`

### Async Methods

Always suffix with `Async`:
```csharp
Task<T?> GetByIdAsync(int id, CancellationToken ct = default);
Task CreateAsync(T entity, CancellationToken ct = default);
ValueTask<bool> RemoveAsync(int id, CancellationToken ct = default);
```

### Return Types (Endpoints)

Use `Results<TSuccess, TError>`:
```csharp
Results<Ok<T>, NotFound>              // GET by ID
Results<Ok<PaginatedModel<T>>, ValidationProblem>  // GET list
Results<CreatedAtRoute<T>, ValidationProblem>      // POST
Results<NoContent, NotFound>          // DELETE
```

### Error Handling

- 400: Validation failed (FluentValidation) or business rule violation
- 401: No/invalid JWT token
- 403: Insufficient permissions
- 404: Entity not found
- 500: Unexpected server error

---

## Quick Commands

```bash
dotnet build                                    # Build
dotnet test                                     # Test
dotnet run --project src/AppHost                # Run with Aspire
dotnet run --project src/WebApi                 # Run API only
cd src/Infrastructure.SqlServer && .\migrations-add.ps1 -Name "Name"  # Add migration
dotnet user-jwts create -o token --scheme Bearer --claim email_verified=true  # Create JWT
```
