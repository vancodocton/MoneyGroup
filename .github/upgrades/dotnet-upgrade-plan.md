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
13. Configure .NET SDK 10 to use Microsoft.Testing.Platform for dotnet test command

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

### .NET SDK 10 Testing Platform Configuration

Starting with .NET SDK 10, the `dotnet test` command natively supports Microsoft.Testing.Platform when test projects are configured as executable test projects.

**Current State:**
- ? All test projects already use Microsoft Testing Platform with xUnit v3.2.0-pre.10
- ? Test projects have `<OutputType>Exe</OutputType>` for executable test projects
- ? Test projects have `<UseMicrosoftTestingPlatformRunner>true</UseMicrosoftTestingPlatformRunner>`
- ? Test projects have `<TestingPlatformDotnetTestSupport>true</TestingPlatformDotnetTestSupport>`
- ? xUnit v3.2.0-pre.10 (includes native Microsoft.Testing.Platform support)
- ? Microsoft.Testing.Extensions.CodeCoverage 18.1.0 (compatible)
- ? No VSTest packages present (Microsoft.NET.Test.Sdk, xunit.runner.visualstudio already removed)

**Understanding .NET SDK 10 + Microsoft.Testing.Platform Integration:**

According to [official .NET testing documentation](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-with-dotnet-test), when using Microsoft.Testing.Platform with .NET SDK 10:

1. **Executable test projects** - Test projects must be executable (`<OutputType>Exe</OutputType>`)
2. **Native integration** - `dotnet test` automatically detects and runs executable test projects
3. **No VSTest required** - Microsoft.Testing.Platform is a modern alternative to VSTest
4. **Framework support** - Works with MSTest, xUnit v3, NUnit (with appropriate adapters)

**Required Configuration for .NET SDK 10:**

**1. Create/Update `Directory.Build.props` in solution root (Global Configuration):**

Configure Microsoft.Testing.Platform settings globally for all test projects:

```xml
<Project>
  <PropertyGroup>
    <!-- Enable Microsoft.Testing.Platform runner globally for all test projects -->
    <UseMicrosoftTestingPlatformRunner>true</UseMicrosoftTestingPlatformRunner>
    
    <!-- Enable dotnet test integration with Microsoft.Testing.Platform -->
    <TestingPlatformDotnetTestSupport>true</TestingPlatformDotnetTestSupport>
  </PropertyGroup>
</Project>
```

**What These Global Properties Enable:**

- `<UseMicrosoftTestingPlatformRunner>true</UseMicrosoftTestingPlatformRunner>` - Enables Microsoft.Testing.Platform runner for all test projects
- `<TestingPlatformDotnetTestSupport>true</TestingPlatformDotnetTestSupport>` - Enables `dotnet test` integration with Microsoft.Testing.Platform

**Note:** `EnableMSTestRunner` property is NOT needed when using xUnit. This property only applies to MSTest projects.

**2. Simplify Test Project Files:**

With global configuration in `Directory.Build.props`, test projects only need:

```xml
<PropertyGroup>
  <TargetFramework>net10.0</TargetFramework>
  <OutputType>Exe</OutputType>
  <!-- All Microsoft.Testing.Platform properties inherited from Directory.Build.props -->
</PropertyGroup>
```

**3. No Changes Needed to global.json:**

The current `global.json` is correct:
```json
{
  "sdk": {
    "version": "10.0.100-rc.2.25502.107",
    "rollForward": "latestMajor"
  }
}
```

**Note:** Do NOT add `msbuild-sdks` sections - they are not needed for .NET SDK 10 with Microsoft.Testing.Platform.

**4. How dotnet test Works with Microsoft.Testing.Platform:**

When you run `dotnet test`:
1. SDK detects test projects with `<OutputType>Exe</OutputType>`
2. Global properties from `Directory.Build.props` apply to all test projects
3. `TestingPlatformDotnetTestSupport=true` enables Microsoft.Testing.Platform mode
4. xUnit v3.2.0-pre.10's native Microsoft.Testing.Platform integration handles test execution
5. Test results are displayed in the console using Microsoft.Testing.Platform output format
6. Exit code indicates success/failure

**Summary of Required Changes:**

| Configuration File                              | Required changes summary                                                                 |
|:------------------------------------------------|:------------------------------------------------------------------------------------------|
| Directory.Build.props (solution root)           | **CREATE/UPDATE** - Add global Microsoft.Testing.Platform configuration properties:<br>- `<UseMicrosoftTestingPlatformRunner>true</UseMicrosoftTestingPlatformRunner>`<br>- `<TestingPlatformDotnetTestSupport>true</TestingPlatformDotnetTestSupport>` |
| Test project files (3 projects)                 | **OPTIONAL CLEANUP** - Can remove `<UseMicrosoftTestingPlatformRunner>` and `<TestingPlatformDotnetTestSupport>` properties since they will be inherited from Directory.Build.props. Keep `<OutputType>Exe</OutputType>`. |
| global.json                                     | **NO CHANGES NEEDED** - Current configuration is correct |

**Benefits of Global Configuration:**

- ? **Centralized Management** - All test settings in one place
- ? **Consistency** - All test projects automatically get the same configuration
- ? **Cleaner Project Files** - Test projects only need `<OutputType>Exe</OutputType>`
- ? **Easier Maintenance** - Change once in Directory.Build.props instead of 3+ test projects
- ? **Future-Proof** - New test projects automatically inherit the correct settings
- ? **No MSTest Conflicts** - xUnit doesn't require `EnableMSTestRunner` property

**What You Should NOT Do:**

- ? Do NOT remove `<OutputType>Exe</OutputType>` from test projects - it's still required
- ? Do NOT add `msbuild-sdks` to global.json - not needed for SDK 10
- ? Do NOT add `<EnableMSTestRunner>false</EnableMSTestRunner>` - not needed for xUnit projects
- ? Do NOT use `<EnableMicrosoftTestingPlatform>` - use `<UseMicrosoftTestingPlatformRunner>` instead

**References:**
- [Unit testing with dotnet test](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-with-dotnet-test)
- [Microsoft.Testing.Platform Overview](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-platform-intro)
- [xUnit v3.2.0-pre.10 Release Notes](https://xunit.net/releases/v3/3.2.0-pre.10)

#### test/UnitTests/MoneyGroup.UnitTests.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`
  - **OPTIONAL CLEANUP** - Can remove `<UseMicrosoftTestingPlatformRunner>` and `<TestingPlatformDotnetTestSupport>` since they will be inherited from Directory.Build.props
  - Keep `<OutputType>Exe</OutputType>` (required)

NuGet packages changes:
  - All packages already at correct versions (no changes needed)

#### test/IntegrationTests/MoneyGroup.IntegrationTests.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`
  - **OPTIONAL CLEANUP** - Can remove `<UseMicrosoftTestingPlatformRunner>` and `<TestingPlatformDotnetTestSupport>` since they will be inherited from Directory.Build.props
  - Keep `<OutputType>Exe</OutputType>` (required)

NuGet packages changes:
  - Microsoft.EntityFrameworkCore.Sqlite should be updated from `9.0.9` to `10.0.0-rc.2.25502.107` (recommended for .NET 10.0)
  - Microsoft.Extensions.Hosting should be updated from `9.0.9` to `10.0.0-rc.2.25502.107` (recommended for .NET 10.0)

#### test/FunctionalTests/MoneyGroup.FunctionalTests.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`
  - **OPTIONAL CLEANUP** - Can remove `<UseMicrosoftTestingPlatformRunner>` and `<TestingPlatformDotnetTestSupport>` since they will be inherited from Directory.Build.props
  - Keep `<OutputType>Exe</OutputType>` (required)

NuGet packages changes:
  - Microsoft.AspNetCore.Mvc.Testing should be updated from `9.0.9` to `10.0.0-rc.2.25502.107` (recommended for .NET 10.0)
