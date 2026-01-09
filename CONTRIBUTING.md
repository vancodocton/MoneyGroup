# Contributing to MoneyGroup

## Technology Stack

| Category | Technology | Version |
|----------|------------|---------|
| Runtime | .NET | 10.0.101 |
| API | ASP.NET Core Minimal APIs | 10.0 |
| ORM | Entity Framework Core | 10.0.1 |
| Validation | FluentValidation | 12.0.0 |
| Mapping | Riok.Mapperly | 4.2.1 |
| Queries | Ardalis.Specification | 9.3.1 |
| Orchestration | .NET Aspire | 13.1.0 |
| Testing | xUnit v3 (MTP-v2) | 3.2.1 |

---

## Architecture

```
WebApi (Presentation) → Core (Business) → Infrastructure (Data)
```

**Core** has no dependencies on Infrastructure or WebApi.

### Layers

| Layer | Location | Contents |
|-------|----------|----------|
| **Core** | `src/Core/` | Entities, Services, Interfaces, Specifications, Validators, DTOs |
| **Infrastructure** | `src/Infrastructure/` | `EfRepository<T>`, `ApplicationDbContext` |
| **Infrastructure.SqlServer** | `src/Infrastructure.SqlServer/` | SQL Server migrations, seed scripts |
| **WebApi** | `src/WebApi/` | Endpoints, Middleware, Authorization |

### Domain Model

```
User (Id, Name, Email?)
  │
  ├─1:N─► Order (Id, Title, Description?, Total, BuyerId)
  │         │
  └─N:M────►OrderParticipant (OrderId, ParticipantId)
```

**Delete Behavior:**
- Delete Order → Cascades to OrderParticipants
- Delete User → Restricted if referenced anywhere

---

## Development Setup

### Prerequisites

- .NET SDK 10.0.101+
- Docker Desktop
- Visual Studio 2022 17.13+ or VS Code with C# Dev Kit

### Run with Aspire (Recommended)

```bash
dotnet restore --locked-mode
dotnet run --project src/AppHost/MoneyGroup.AppHost.csproj
```

### Run WebApi Only

```bash
dotnet run --project src/WebApi
# Swagger: https://localhost:7xxx/swagger
```

---

## Commands

### Build & Run

```bash
dotnet build                                    # Build solution
dotnet watch --project src/WebApi               # Watch mode
dotnet format                                   # Format code
```

### Database Migrations

```bash
# Add migration (SQL Server)
cd src/Infrastructure.SqlServer
.\migrations-add.ps1 -Name "MigrationName"

# Apply migration
dotnet ef database update --project src/Infrastructure.SqlServer --startup-project src/WebApi

# Generate SQL script
dotnet ef migrations script --project src/Infrastructure.SqlServer --startup-project src/WebApi -o migration.sql
```

### Testing

```bash
dotnet test                                           # All tests
dotnet test test/UnitTests                            # Unit tests only
dotnet test --filter "FullyQualifiedName~OrderService"  # Filter by name
dotnet test --collect:"Code Coverage"                 # With coverage
```

### Docker

```bash
docker-compose up -d                # Start services
docker-compose logs -f mssql        # View logs
docker-compose down -v              # Stop and remove volumes
```

### Authentication (JWT)

```bash
dotnet user-jwts create -o token --scheme Bearer --claim email_verified=true --claim email=test@example.com
dotnet user-jwts list               # List tokens
dotnet user-jwts clear --force      # Clear all tokens
```

---

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/Order` | List orders (paginated) |
| GET | `/api/Order/{id}` | Get order by ID |
| POST | `/api/Order` | Create order |
| DELETE | `/api/Order/{id}` | Delete order |
| GET | `/api/User` | List users (paginated) |
| GET | `/api/User/{id}` | Get user by ID |
| POST | `/api/User` | Create user |
| DELETE | `/api/User/{id}` | Delete user |
| GET | `/health` | Health check |
| GET | `/alive` | Liveness probe |

All endpoints require JWT Bearer authentication.

---

## Testing Strategy

| Test Type | Project | Focus | Dependencies |
|-----------|---------|-------|--------------|
| Unit | `test/UnitTests` | Services, Validators | Mocked |
| Integration | `test/IntegrationTests` | Repositories, Queries | Real DB (Testcontainers) |
| Functional | `test/FunctionalTests` | API Endpoints, Auth | WebApplicationFactory |

---

## Branch & Commit Conventions

### Branches

- `main` - Production-ready
- `feature/*` - New features
- `bugfix/*` - Bug fixes
- `chore/*` - Maintenance

### Commits

```
feat: Add payment endpoint
fix: Resolve order calculation bug
chore: Update dependencies
docs: Improve API documentation
test: Add order service tests
```

---

## Troubleshooting

### Docker Issues

```bash
docker-compose logs mssql                           # Check logs
docker inspect moneygroup-mssql --format='{{.State.Health.Status}}'
docker-compose down -v && docker-compose up -d      # Reset
```

### Migration Errors

```bash
dotnet ef database update --verbose                 # Verbose output
dotnet ef database drop --force                     # Drop database (dev only!)
```

### Authentication Issues

```bash
dotnet user-jwts clear --force
dotnet user-jwts create -o token --scheme Bearer --claim email_verified=true
```

---

## Resources

- [.NET Aspire](https://learn.microsoft.com/dotnet/aspire/)
- [Minimal APIs](https://learn.microsoft.com/aspnet/core/fundamentals/minimal-apis)
- [EF Core](https://learn.microsoft.com/ef/core/)
- [FluentValidation](https://docs.fluentvalidation.net/)
- [Ardalis.Specification](https://github.com/ardalis/Specification)
