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
| Npgsql.EntityFrameworkCore.PostgreSQL                |             | 10.0.0-rc.1               |           |
| Microsoft.AspNetCore.Mvc.Testing                     |             | 10.0.0-rc.2.25502.107     |           |

## All commits

| Commit ID | Description |
|:----------|:------------|
| be6bef9e  | Added UseMicrosoftTestingPlatformRunner and TestingPlatformDotnetTestSupport to Directory.Build.props; removed incorrect MicrosoftTestingPlatform property. |

## Next steps

- Run tests across all test projects using `dotnet test` (Microsoft.Testing.Platform enabled)
- Verify Docker images build and run with .NET 10 base images
- Monitor for any preview-to-RTM package updates once .NET 10 GA is available

---
Tokens/cost summary not available in this environment.
