# .NET 10.0 Upgrade Plan

## Execution Steps

Execute steps below sequentially one by one in the order they are listed.

1. Validate that an .NET 10.0 SDK required for this upgrade is installed on the machine and if not, help to get it installed.
2. Ensure that the SDK version specified in global.json files is compatible with the .NET 10.0 upgrade.
2a. Update global.json to use the latest .NET 10.0 SDK version
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
| xunit.v3                                                    | 3.0.1           | 3.2.0-pre.10                  | Native Microsoft Testing Platform v2 support   |

### Microsoft Testing Platform v2 migration

Migrate test projects to Microsoft Testing Platform v2 with native xUnit v3 support.

- Upgrade xUnit v3 to version 3.2.0-pre.10 which includes native MTP v2 support (.NET SDK 10 compatible)
- **Configure .NET SDK 10 support** per xUnit v3.2.0-pre.10 requirements:
  - Set `<EnableMSTestRunner>false</EnableMSTestRunner>` to disable MSTest compatibility layer
  - Ensure `<UseMicrosoftTestingPlatformRunner>true</UseMicrosoftTestingPlatformRunner>` for MTP v2
  - Ensure `<TestingPlatformDotnetTestSupport>true</TestingPlatformDotnetTestSupport>` for CLI support
  - Keep `<OutputType>Exe</OutputType>` for standalone test executable
- **Verify** `Microsoft.Testing.Extensions.CodeCoverage` MTP v2 compatibility:
  - Upgraded to version: `18.1.0` (MTP v2 compatible - verified)
- Remove `Microsoft.NET.Test.Sdk` (VSTest adapter, not needed with MTP v2)
- Remove `Microsoft.Testing.Extensions.VSTestBridge` (not needed with native MTP v2 support)
- Remove `xunit.runner.visualstudio` (VSTest runner, replaced by native MTP v2)

**Important .NET SDK 10 Configuration:**
According to xUnit v3.2.0-pre.10 release notes, when using .NET SDK 10 or later, you must set `<EnableMSTestRunner>false</EnableMSTestRunner>` in your test projects to prevent conflicts with the MSTest runner that is enabled by default in .NET SDK 10.

| Test project path                                   | Required changes summary                                                                 |
|:----------------------------------------------------|:------------------------------------------------------------------------------------------|
| test/UnitTests/MoneyGroup.UnitTests.csproj          | Upgrade xUnit; remove VSTest packages; add EnableMSTestRunner=false for SDK 10           |
| test/IntegrationTests/MoneyGroup.IntegrationTests.csproj | Upgrade xUnit; remove VSTest packages; add EnableMSTestRunner=false for SDK 10     |
| test/FunctionalTests/MoneyGroup.FunctionalTests.csproj | Upgrade xUnit; remove VSTest packages; add EnableMSTestRunner=false for SDK 10       |

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
  - Add `<EnableMSTestRunner>false</EnableMSTestRunner>` (required for .NET SDK 10 with xUnit)
  - Ensure `<UseMicrosoftTestingPlatformRunner>true</UseMicrosoftTestingPlatformRunner>` is set
  - Ensure `<TestingPlatformDotnetTestSupport>true</TestingPlatformDotnetTestSupport>` is set

NuGet packages changes:
  - xunit.v3 should be updated from `3.0.1` to `3.2.0-pre.10` (native MTP v2 support for .NET SDK 10)
  - Microsoft.Testing.Extensions.CodeCoverage should be updated from `17.14.2` to `18.1.0` (MTP v2 compatible)
  - Remove Microsoft.NET.Test.Sdk (VSTest adapter not needed)
  - Remove xunit.runner.visualstudio (VSTest runner replaced by native MTP v2)

#### test/IntegrationTests/MoneyGroup.IntegrationTests.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`
  - Add `<EnableMSTestRunner>false</EnableMSTestRunner>` (required for .NET SDK 10 with xUnit)
  - Ensure `<UseMicrosoftTestingPlatformRunner>true</UseMicrosoftTestingPlatformRunner>` is set
  - Ensure `<TestingPlatformDotnetTestSupport>true</TestingPlatformDotnetTestSupport>` is set

NuGet packages changes:
  - Microsoft.EntityFrameworkCore.Sqlite should be updated from `9.0.9` to `10.0.0-rc.2.25502.107` (recommended for .NET 10.0)
  - Microsoft.Extensions.Hosting should be updated from `9.0.9` to `10.0.0-rc.2.25502.107` (recommended for .NET 10.0)
  - xunit.v3 should be updated from `3.0.1` to `3.2.0-pre.10` (native MTP v2 support for .NET SDK 10)
  - Microsoft.Testing.Extensions.CodeCoverage should be updated from `17.14.2` to `18.1.0` (MTP v2 compatible)
  - Remove Microsoft.NET.Test.Sdk (VSTest adapter not needed)
  - Remove xunit.runner.visualstudio (VSTest runner replaced by native MTP v2)

#### test/FunctionalTests/MoneyGroup.FunctionalTests.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`
  - Add `<EnableMSTestRunner>false</EnableMSTestRunner>` (required for .NET SDK 10 with xUnit)
  - Ensure `<UseMicrosoftTestingPlatformRunner>true</UseMicrosoftTestingPlatformRunner>` is set
  - Ensure `<TestingPlatformDotnetTestSupport>true</TestingPlatformDotnetTestSupport>` is set

NuGet packages changes:
  - Microsoft.AspNetCore.Mvc.Testing should be updated from `9.0.9` to `10.0.0-rc.2.25502.107` (recommended for .NET 10.0)
  - xunit.v3 should be updated from `3.0.1` to `3.2.0-pre.10` (native MTP v2 support for .NET SDK 10)
  - Microsoft.Testing.Extensions.CodeCoverage should be updated from `17.14.2` to `18.1.0` (MTP v2 compatible)
  - Remove Microsoft.NET.Test.Sdk (VSTest adapter not needed)
  - Remove xunit.runner.visualstudio (VSTest runner replaced by native MTP v2)

#### src/Postgres.Migrator/MoneyGroup.Postgres.Migrator.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`

NuGet packages changes:
  - Microsoft.Extensions.Hosting should be updated from `9.0.9` to `10.0.0-rc.2.25502.107` (recommended for .NET 10.0)
  - Microsoft.VisualStudio.Azure.Containers.Tools.Targets should be updated from `1.22.1` to `1.23.0` (recommended for .NET 10.0 by VS 2026 Insider)
### .NET SDK Configuration

Update solution to use the latest .NET 10.0 SDK for consistent build environment.

| Configuration File | Current SDK Version | Target SDK Version | Change Required |
|:-------------------|:-------------------:|:------------------:|:----------------|
| global.json        | 9.0.305             | 10.0.100-rc.2.25502.107 | Update to latest .NET 10 SDK |

**SDK Version Strategy:**
- Use `rollForward: "latestMajor"` to allow automatic rollforward to newer SDK versions
- Updated to .NET 10.0.100-rc.2.25502.107 (latest available)
- SDK version aligns with .NET 10.0 runtime and NuGet packages

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
| xunit.v3                                                    | 3.0.1           | 3.2.0-pre.10                  | Native Microsoft Testing Platform v2 support   |

### Microsoft Testing Platform v2 migration

Migrate test projects to Microsoft Testing Platform v2 with native xUnit v3 support.

- Upgrade xUnit v3 to version 3.2.0-pre.10 which includes native MTP v2 support
- **Verify** `Microsoft.Testing.Extensions.CodeCoverage` MTP v2 compatibility:
  - Current version: `17.14.2` (needs MTP v2 verification)
  - If not MTP v2 compatible, upgrade to latest MTP v2-compatible version
  - Check NuGet.org release notes for MTP v2 support confirmation
- Remove `Microsoft.NET.Test.Sdk` (VSTest adapter, not needed with MTP v2)
- Remove `Microsoft.Testing.Extensions.VSTestBridge` (not needed with native MTP v2 support)
- Remove `xunit.runner.visualstudio` (VSTest runner, replaced by native MTP v2)
- Ensure `UseMicrosoftTestingPlatformRunner=true` and `TestingPlatformDotnetTestSupport=true` are set

**Important:** Before applying changes, verify `Microsoft.Testing.Extensions.CodeCoverage` v17.14.2 is MTP v2 compatible on NuGet.org. If not, identify and upgrade to the latest MTP v2-compatible version.

| Test project path                                   | Required changes summary                                                                 |
|:----------------------------------------------------|:------------------------------------------------------------------------------------------|
| test/UnitTests/MoneyGroup.UnitTests.csproj          | Upgrade xUnit to 3.2.0-pre.10; remove VSTest packages; use native MTP v2                  |
| test/IntegrationTests/MoneyGroup.IntegrationTests.csproj | Upgrade xUnit to 3.2.0-pre.10; remove VSTest packages; use native MTP v2            |
| test/FunctionalTests/MoneyGroup.FunctionalTests.csproj | Upgrade xUnit to 3.2.0-pre.10; remove VSTest packages; use native MTP v2              |

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
