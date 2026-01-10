# MoneyGroup

[![codecov](https://codecov.io/gh/vancodocton/MoneyGroup/graph/badge.svg?token=BZ79TWTVDQ)](https://codecov.io/gh/vancodocton/MoneyGroup)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=vancodocton_MoneyGroup&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=vancodocton_MoneyGroup)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=vancodocton_MoneyGroup&metric=coverage)](https://sonarcloud.io/summary/new_code?id=vancodocton_MoneyGroup)

A .NET 10 backend API demonstrating **Clean Architecture** with .NET Aspire orchestration.

## Quick Start

```bash
git clone https://github.com/vancodocton/MoneyGroup.git
cd MoneyGroup
dotnet restore --locked-mode
dotnet run --project src/AppHost/MoneyGroup.AppHost.csproj
```

The Aspire Dashboard opens at `http://localhost:15888` with all services.

## Project Structure

```
src/
├── Core/                      # Domain entities, services, interfaces
├── Infrastructure/            # EF Core repositories, data access
├── Infrastructure.SqlServer/  # SQL Server migrations & seeds
├── Infrastructure.PostgreSql/ # PostgreSQL migrations & seeds
├── WebApi/                    # Minimal API endpoints
├── AppHost/                   # .NET Aspire orchestration
└── ServiceDefaults/           # Shared Aspire configuration
test/
├── UnitTests/                 # Service & validator tests
├── IntegrationTests/          # Database tests
└── FunctionalTests/           # API endpoint tests
```

## Documentation

| Document | Description |
|----------|-------------|
| [CONTRIBUTING.md](CONTRIBUTING.md) | Development setup, architecture, testing, commands |
| [.github/copilot-instructions.md](.github/copilot-instructions.md) | AI assistant patterns and conventions |
| [AGENTS.md](AGENTS.md) | AI assistant patterns and conventions |

## License

See [LICENSE.txt](LICENSE.txt)
