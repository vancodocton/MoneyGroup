# .NET 10 Upgrade Report

## Project target framework modifications

| Project name                                            | Old Target Framework | New Target Framework | Commits |
|:--------------------------------------------------------|:--------------------:|:--------------------:|:--------|
| src/Core/MoneyGroup.Core.csproj                         |   net9.0             | net10.0              |         |
| src/Infrastructure/MoneyGroup.Infrastructure.csproj     |   net9.0             | net10.0              |         |
| src/Infrastructure.SqlServer/MoneyGroup.Infrastructure.SqlServer.csproj |   net9.0 | net10.0 |         |
| src/Infrastructure.PostgreSql/MoneyGroup.Infrastructure.PostgreSql.csproj |   net9.0 | net10.0 |         |
| src/WebApi/MoneyGroup.WebApi.csproj                     |   net9.0             | net10.0              |         |
| test/UnitTests/MoneyGroup.UnitTests.csproj              |   net9.0             | net10.0              |         |
| test/IntegrationTests/MoneyGroup.IntegrationTests.csproj|   net9.0             | net10.0              |         |
| test/FunctionalTests/MoneyGroup.FunctionalTests.csproj  |   net9.0             | net10.0              |         |
| src/Postgres.Migrator/MoneyGroup.Postgres.Migrator.csproj | net9.0            | net10.0              |         |

## NuGet Packages

| Package Name                                         | Old Version | New Version               | Commit Id |
|:-----------------------------------------------------|:-----------:|:-------------------------:|:----------|
| Microsoft.AspNetCore.Authentication.JwtBearer        |             | 10.0.0-rc.2.25502.107     |           |
| Microsoft.AspNetCore.OpenApi                         |             | 10.0.0-rc.2.25502.107     |           |
| Microsoft.EntityFrameworkCore                        |             | 10.0.0-rc.2.25502.107     |           |
| Microsoft.EntityFrameworkCore.Design                 |             | 10.0.0-rc.2.25502.107     |           |
| Microsoft.EntityFrameworkCore.Relational             |             | 10.0.0-rc.2.25502.107     |           |
| Microsoft.EntityFrameworkCore.Sqlite                 |             | 10.0.0-rc.2.25502.107     |           |
| Microsoft.EntityFrameworkCore.SqlServer              |             | 10.0.0-rc.2.25502.107     |           |
| Microsoft.Extensions.ApiDescription.Server           |             | 10.0.0-rc.2.25502.107     |           |
| Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore | | 10.0.0-rc.2.25502.107 |           |
| Microsoft.Extensions.Hosting                         |             | 10.0.0-rc.2.25502.107     |           |
| Microsoft.Extensions.Logging.Abstractions            |             | 10.0.0-rc.2.25502.107     |           |
| Microsoft.VisualStudio.Azure.Containers.Tools.Targets|             | 1.23.0                    |           |
| Npgsql.EntityFrameworkCore.PostgreSQL                | 9.0.4       | 10.0.0-rc.2               | ee22382   |
| Microsoft.AspNetCore.Mvc.Testing                     |             | 10.0.0-rc.2.25502.107     |           |

## .NET Tools

| Tool Name            | Old Version | New Version               | Commit Id |
|:---------------------|:-----------:|:-------------------------:|:----------|
| dotnet-ef            | 9.0.10      | 10.0.0-rc.2.25502.107     | 3e36aff   |

## All commits

| Commit ID | Description |
|:----------|:------------|
| 3e36aff   | Upgrade dotnet-ef tool to 10.0.0-rc.2.25502.107 for .NET 10.0 |
| ee22382   | Update Npgsql.EntityFrameworkCore.PostgreSQL to version 10.0.0-rc.2 |
| be6bef9e  | Added UseMicrosoftTestingPlatformRunner and TestingPlatformDotnetTestSupport to Directory.Build.props; removed incorrect MicrosoftTestingPlatform property. |

## Project feature upgrades

### .config/dotnet-tools.json

Successfully upgraded .NET tools to versions compatible with .NET 10.0:

- **dotnet-ef**: Updated from `9.0.10` to `10.0.0-rc.2.25502.107`
  - Entity Framework Core command-line tools now match the .NET 10.0 EF Core packages
  - Verified with `dotnet ef --version` command
- **dotnet-sonarscanner**: Remains at `10.1.2` (compatible with .NET 10.0)
- **dotnet-coverage**: Remains at `17.14.2` (compatible with .NET 10.0)
- **Tool restoration**: Successfully ran `dotnet tool restore` to apply changes

### src/Infrastructure.PostgreSql/MoneyGroup.Infrastructure.PostgreSql.csproj

Successfully upgraded to .NET 10.0 with the following changes:

- **Target Framework**: Updated from `net9.0` to `net10.0`
- **Npgsql.EntityFrameworkCore.PostgreSQL**: Updated to version `10.0.0-rc.2` (corrected from initially planned 10.0.0-rc.1)
- **Entity Framework Core packages**: Updated to `10.0.0-rc.2.25502.107`
- **Build validation**: Passed with no errors
- **Package management**: Using centralized package version management via Directory.Packages.props

All packages are compatible with .NET 10.0 and the project builds successfully.

## Next steps

- Run tests across all test projects using `dotnet test` (Microsoft.Testing.Platform enabled)
- Verify Docker images build and run with .NET 10 base images
- Monitor for any preview-to-RTM package updates once .NET 10 GA is available

---
Tokens/cost summary not available in this environment.
