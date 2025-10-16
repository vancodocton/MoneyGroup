# .NET 10.0 Upgrade Plan

## Execution Steps

Execute steps below sequentially one by one in the order they are listed.

1. Validate that an .NET 10.0 SDK required for this upgrade is installed on the machine and if not, help to get it installed.
2. Ensure that the SDK version specified in global.json files is compatible with the .NET 10.0 upgrade.
3. Upgrade src/Core/MoneyGroup.Core.csproj
4. Upgrade src/Infrastructure/MoneyGroup.Infrastructure.csproj
5. Upgrade src/Infrastructure.SqlServer/MoneyGroup.Infrastructure.SqlServer.csproj
6. Upgrade src/Infrastructure.PostgreSql/MoneyGroup.Infrastructure.PostgreSql.csproj
7. Upgrade src/WebApi/MoneyGroup.WebApi.csproj
8. Upgrade test/UnitTests/MoneyGroup.UnitTests.csproj
9. Upgrade test/IntegrationTests/MoneyGroup.IntegrationTests.csproj
10. Upgrade test/FunctionalTests/MoneyGroup.FunctionalTests.csproj
11. Upgrade src/Postgres.Migrator/MoneyGroup.Postgres.Migrator.csproj
12. Update Docker build assets to .NET 10.0 base images (Dockerfiles and docker-compose.yml)
13. Migrate all test projects to Microsoft Testing Platform and remove VSTest usage

## Settings

This section contains settings and data used by execution steps.

### Excluded projects

Table below contains projects that do belong to the dependency graph for selected projects and should not be included in the upgrade.

| Project name                                   | Description                 |
|:-----------------------------------------------|:---------------------------:|
| docker-compose.dcproj                          | Explicitly excluded         |

### Aggregate NuGet packages modifications across all projects

NuGet packages used across all selected projects or their dependencies that need version update in projects that reference them.

| Package Name                                                | Current Version | New Version                   | Description                                    |
|:------------------------------------------------------------|:---------------:|:-----------------------------:|:-----------------------------------------------|
| Microsoft.AspNetCore.Authentication.JwtBearer               | 9.0.9           | 10.0.0-rc.2.25502.107         | Recommended for .NET 10.0                      |
| Microsoft.AspNetCore.Mvc.Testing                            | 9.0.9           | 10.0.0-rc.2.25502.107         | Recommended for .NET 10.0                      |
| Microsoft.AspNetCore.OpenApi                               | 9.0.9           | 10.0.0-rc.2.25502.107         | Recommended for .NET 10.0                      |
| Microsoft.EntityFrameworkCore                               | 9.0.9           | 10.0.0-rc.2.25502.107         | Recommended for .NET 10.0                      |
| Microsoft.EntityFrameworkCore.Design                        | 9.0.9           | 10.0.0-rc.2.25502.107         | Recommended for .NET 10.0                      |
| Microsoft.EntityFrameworkCore.Relational                    | 9.0.9           | 10.0.0-rc.2.25502.107         | Recommended for .NET 10.0                      |
| Microsoft.EntityFrameworkCore.Sqlite                        | 9.0.9           | 10.0.0-rc.2.25502.107         | Recommended for .NET 10.0                      |
| Microsoft.EntityFrameworkCore.SqlServer                     | 9.0.9           | 10.0.0-rc.2.25502.107         | Recommended for .NET 10.0                      |
| Microsoft.Extensions.ApiDescription.Server                  | 9.0.9           | 10.0.0-rc.2.25502.107         | Recommended for .NET 10.0                      |
| Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore | 9.0.9      | 10.0.0-rc.2.25502.107         | Recommended for .NET 10.0                      |
| Microsoft.Extensions.Hosting                                | 9.0.9           | 10.0.0-rc.2.25502.107         | Recommended for .NET 10.0                      |
| Microsoft.Extensions.Logging.Abstractions                   | 9.0.9           | 10.0.0-rc.2.25502.107         | Recommended for .NET 10.0                      |
| Microsoft.VisualStudio.Azure.Containers.Tools.Targets       | 1.22.1          | 1.23.0                        | Recommended for .NET 10.0 (VS 2026 Insider)    |
| Npgsql.EntityFrameworkCore.PostgreSQL                       | 9.0.4           | 10.0.0-rc.1                   | Recommended for .NET 10.0                      |

### Microsoft Testing Platform migration

Migrate test projects to Microsoft Testing Platform and remove VSTest.

- Enable Microsoft Testing Platform in all test projects.
- Remove any VSTest-only tooling, CLI usage, and configuration (vstest.console, VSTest-specific RunSettings, legacy logger/collectors).
- Ensure `dotnet test` runs use the Microsoft Testing Platform by default.

| Test project path                                   | Required changes summary                                                                 |
|:----------------------------------------------------|:------------------------------------------------------------------------------------------|
| test/UnitTests/MoneyGroup.UnitTests.csproj          | Enable Microsoft Testing Platform; remove VSTest-only config/tooling                      |
| test/IntegrationTests/MoneyGroup.IntegrationTests.csproj | Enable Microsoft Testing Platform; remove VSTest-only config/tooling                 |
| test/FunctionalTests/MoneyGroup.FunctionalTests.csproj | Enable Microsoft Testing Platform; remove VSTest-only config/tooling                  |

### Docker build assets modifications

Dockerfiles and compose definitions should be updated to use .NET 10.0 images and target framework.

| Asset path                                        | Change                                                                                 | Description                                   |
|:--------------------------------------------------|:---------------------------------------------------------------------------------------|:----------------------------------------------|
| src/WebApi/Dockerfile                             | Replace base images: sdk 9.0 -> 10.0; aspnet 9.0 -> 10.0                              | Align build and runtime images with .NET 10.0 |
| src/WebApi/Dockerfile                             | Update publish command to target framework net10.0                                    | Ensure app builds for .NET 10.0               |
| src/Postgres.Migrator/Dockerfile                  | Replace base images: sdk 9.0 -> 10.0; runtime/aspnet 9.0 -> 10.0 (whichever is used)  | Align build and runtime images with .NET 10.0 |
| src/Postgres.Migrator/Dockerfile                  | Update publish command to target framework net10.0                                    | Ensure app builds for .NET 10.0               |
| docker-compose.yml                                 | If explicit .NET image tags are referenced, update 9.0 tags to 10.0                   | Keep compose environment consistent           |

### Project upgrade details
This section contains details about each project upgrade and modifications that need to be done in the project.

#### src/Core/MoneyGroup.Core.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`

NuGet packages changes:
  - Microsoft.Extensions.Logging.Abstractions should be updated from `9.0.9` to `10.0.0-rc.2.25502.107` (recommended for .NET 10.0)

#### src/Infrastructure/MoneyGroup.Infrastructure.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`

NuGet packages changes:
  - Microsoft.EntityFrameworkCore should be updated from `9.0.9` to `10.0.0-rc.2.25502.107` (recommended for .NET 10.0)
  - Microsoft.EntityFrameworkCore.Relational should be updated from `9.0.9` to `10.0.0-rc.2.25502.107` (recommended for .NET 10.0)

#### src/Infrastructure.SqlServer/MoneyGroup.Infrastructure.SqlServer.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`

NuGet packages changes:
  - Microsoft.EntityFrameworkCore.Design should be updated from `9.0.9` to `10.0.0-rc.2.25502.107` (recommended for .NET 10.0)
  - Npgsql.EntityFrameworkCore.PostgreSQL should be updated from `9.0.4` to `10.0.0-rc.1` (recommended for .NET 10.0)
  - Microsoft.EntityFrameworkCore.SqlServer should be updated from `9.0.9` to `10.0.0-rc.2.25502.107` (recommended for .NET 10.0)

#### src/Infrastructure.PostgreSql/MoneyGroup.Infrastructure.PostgreSql.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`

NuGet packages changes:
  - Microsoft.EntityFrameworkCore.Design should be updated from `9.0.9` to `10.0.0-rc.2.25502.107` (recommended for .NET 10.0)

#### src/WebApi/MoneyGroup.WebApi.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`

NuGet packages changes:
  - Microsoft.AspNetCore.Authentication.JwtBearer should be updated from `9.0.9` to `10.0.0-rc.2.25502.107` (recommended for .NET 10.0)
  - Microsoft.AspNetCore.OpenApi should be updated from `9.0.9` to `10.0.0-rc.2.25502.107` (recommended for .NET 10.0)
  - Microsoft.EntityFrameWorkCore.Design should be updated from `9.0.9` to `10.0.0-rc.2.25502.107` (recommended for .NET 10.0)
  - Microsoft.Extensions.ApiDescription.Server should be updated from `9.0.9` to `10.0.0-rc.2.25502.107` (recommended for .NET 10.0)
  - Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore should be updated from `9.0.9` to `10.0.0-rc.2.25502.107` (recommended for .NET 10.0)
  - Microsoft.VisualStudio.Azure.Containers.Tools.Targets should be updated from `1.22.1` to `1.23.0` (recommended for .NET 10.0 by VS 2026 Insider)

#### test/UnitTests/MoneyGroup.UnitTests.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`

#### test/IntegrationTests/MoneyGroup.IntegrationTests.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`

NuGet packages changes:
  - Microsoft.EntityFrameworkCore.Sqlite should be updated from `9.0.9` to `10.0.0-rc.2.25502.107` (recommended for .NET 10.0)
  - Microsoft.Extensions.Hosting should be updated from `9.0.9` to `10.0.0-rc.2.25502.107` (recommended for .NET 10.0)

#### test/FunctionalTests/MoneyGroup.FunctionalTests.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`

NuGet packages changes:
  - Microsoft.AspNetCore.Mvc.Testing should be updated from `9.0.9` to `10.0.0-rc.2.25502.107` (recommended for .NET 10.0)

#### src/Postgres.Migrator/MoneyGroup.Postgres.Migrator.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`

NuGet packages changes:
  - Microsoft.Extensions.Hosting should be updated from `9.0.9` to `10.0.0-rc.2.25502.107` (recommended for .NET 10.0)
  - Microsoft.VisualStudio.Azure.Containers.Tools.Targets should be updated from `1.22.1` to `1.23.0` (recommended for .NET 10.0 by VS 2026 Insider)
