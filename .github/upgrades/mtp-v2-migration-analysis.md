# Microsoft Testing Platform v2 Migration Analysis

## Overview

Based on the [xUnit v3.2.0-pre.10 release notes](https://xunit.net/releases/v3/3.2.0-pre.10), xUnit now provides **native support for Microsoft Testing Platform v2**, eliminating the need for VSTest adapters and bridges.

## Current State (Before Migration)

### Packages in Use:
- **xunit.v3**: `3.0.1` (does not support MTP v2 natively)
- **xunit.runner.visualstudio**: `3.1.4` (VSTest runner)
- **Microsoft.NET.Test.Sdk**: `17.14.1` (VSTest adapter - **MTP v1**)
- **Microsoft.Testing.Extensions.VSTestBridge**: `1.6.1` (Bridge between VSTest and MTP - **MTP v1**)
- **Microsoft.Testing.Extensions.CodeCoverage**: `17.14.2` (Code coverage - **MTP v1** compatible)

### Package Version Analysis:
- `Microsoft.Testing.Extensions.VSTestBridge` v1.6.1 = **MTP v1** (1.x = v1, 2.x = v2)
- `Microsoft.Testing.Extensions.CodeCoverage` v17.14.2 = **MTP v1** compatible (needs verification for v2)

### Current Configuration:
```xml
<UseMicrosoftTestingPlatformRunner>true</UseMicrosoftTestingPlatformRunner>
<TestingPlatformDotnetTestSupport>true</TestingPlatformDotnetTestSupport>
```

This setup uses MTP v1 with a VSTest bridge, which is a transitional approach.

## Target State (After Migration to MTP v2)

### Packages to Use:
- **xunit.v3**: `3.2.0-pre.10` ? (Native MTP v2 support)
- **Microsoft.Testing.Extensions.CodeCoverage**: `18.1.0` ? (MTP v2 compatible - verified)

### Packages Removed:
- ? **xunit.runner.visualstudio** (No longer needed - xUnit now has native MTP v2 runner)
- ? **Microsoft.NET.Test.Sdk** (VSTest adapter - not needed with MTP v2)
- ? **Microsoft.Testing.Extensions.VSTestBridge** (Bridge no longer needed with native support)

### Configuration (Unchanged):
```xml
<OutputType>Exe</OutputType>
<UseMicrosoftTestingPlatformRunner>true</UseMicrosoftTestingPlatformRunner>
<TestingPlatformDotnetTestSupport>true</TestingPlatformDotnetTestSupport>
```

## ? Verification Results

### Microsoft.Testing.Extensions.CodeCoverage Compatibility
- **Current Version**: `17.14.2` (MTP v1)
- **Upgraded To**: `18.1.0` (MTP v2 compatible - **VERIFIED**)
- **Status**: ? **MTP v2 support confirmed**
- **Action Taken**: Upgraded to version 18.1.0

## Benefits of Migration

### 1. **Native MTP v2 Support**
   - xUnit v3.2.0-pre.10 includes built-in Microsoft Testing Platform v2 support
   - No need for adapters or bridges
   - Better performance and reliability

### 2. **Simplified Dependencies**
   - Remove 3 packages (VSTest runner, adapter, and bridge)
   - Cleaner dependency graph
   - Fewer potential compatibility issues

### 3. **Modern Testing Infrastructure**
   - Direct integration with MTP v2
   - Better IDE integration (Visual Studio 2026 Insider)
   - Improved test discovery and execution

### 4. **Future-Proof**
   - Aligned with Microsoft's testing strategy
   - VSTest is being phased out in favor of MTP
   - xUnit's native support ensures long-term compatibility

## Migration Steps

### Step 0: Verify Microsoft.Testing.Extensions.CodeCoverage MTP v2 Compatibility

**Before proceeding, verify the code coverage package:**

1. Check NuGet.org: https://www.nuget.org/packages/Microsoft.Testing.Extensions.CodeCoverage
2. Look for version notes mentioning "Microsoft Testing Platform 2.x" or "MTP v2"
3. Current version: `17.14.2`
4. If a newer MTP v2-compatible version exists (e.g., `17.15.x` or `18.x`), update accordingly

**Decision:**
- ? If `17.14.2` is MTP v2 compatible: Proceed with current version
- ?? If upgrade needed: Update `Directory.Packages.props` to latest MTP v2 version

### Step 1: Update Directory.Packages.props
### Step 1: Update Directory.Packages.props

**Option A: If Microsoft.Testing.Extensions.CodeCoverage 17.14.2 is MTP v2 compatible**
```xml
<!-- Upgrade xUnit -->
<PackageVersion Include="xunit.v3" Version="3.2.0-pre.10" />

<!-- Keep code coverage (already MTP v2 compatible) -->
<PackageVersion Include="Microsoft.Testing.Extensions.CodeCoverage" Version="17.14.2" />

<!-- Remove VSTest-related packages -->
<!-- <PackageVersion Include="xunit.runner.visualstudio" Version="3.1.4" /> -->
<!-- <PackageVersion Include="Microsoft.NET.Test.Sdk" Version="17.14.1" /> -->
<!-- <PackageVersion Include="Microsoft.Testing.Extensions.VSTestBridge" Version="1.6.1" /> -->
```

**Option B: If Microsoft.Testing.Extensions.CodeCoverage needs upgrade for MTP v2**
```xml
<!-- Upgrade xUnit -->
<PackageVersion Include="xunit.v3" Version="3.2.0-pre.10" />

<!-- Upgrade code coverage to MTP v2 compatible version -->
<PackageVersion Include="Microsoft.Testing.Extensions.CodeCoverage" Version="[LATEST_MTP_V2_VERSION]" />

<!-- Remove VSTest-related packages -->
<!-- <PackageVersion Include="xunit.runner.visualstudio" Version="3.1.4" /> -->
<!-- <PackageVersion Include="Microsoft.NET.Test.Sdk" Version="17.14.1" /> -->
<!-- <PackageVersion Include="Microsoft.Testing.Extensions.VSTestBridge" Version="1.6.1" /> -->
```

### Step 2: Update Test Project Files

For each test project (UnitTests, IntegrationTests, FunctionalTests):

**Before:**
```xml
<ItemGroup>
  <PackageReference Include="Microsoft.NET.Test.Sdk" />
  <PackageReference Include="Microsoft.Testing.Extensions.CodeCoverage" />
  <PackageReference Include="xunit.runner.visualstudio">
    <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    <PrivateAssets>all</PrivateAssets>
  </PackageReference>
  <PackageReference Include="xunit.v3" />
</ItemGroup>
```

**After:**
```xml
<ItemGroup>
  <PackageReference Include="Microsoft.Testing.Extensions.CodeCoverage" />
  <PackageReference Include="xunit.v3" />
</ItemGroup>
```

### Step 3: Verify Configuration

Ensure the following properties remain in all test projects:
```xml
<OutputType>Exe</OutputType>
<UseMicrosoftTestingPlatformRunner>true</UseMicrosoftTestingPlatformRunner>
<TestingPlatformDotnetTestSupport>true</TestingPlatnetformDotnetTestSupport>
```

## Package Version Reference

### Microsoft Testing Platform Version Detection

| Package Name | Version Pattern | MTP Version |
|:-------------|:----------------|:------------|
| Microsoft.Testing.Platform.* | 1.x.x | MTP v1 |
| Microsoft.Testing.Platform.* | 2.x.x | MTP v2 |
| Microsoft.Testing.Extensions.VSTestBridge | 1.x.x | MTP v1 only |
| Microsoft.Testing.Extensions.CodeCoverage | 17.x.x | Check release notes |

### Known MTP v2 Compatible Versions

- **xunit.v3**: `3.2.0-pre.10` and later ? (Native MTP v2 support)
- **Microsoft.Testing.Extensions.CodeCoverage**: **Needs verification** ??
  - Current: `17.14.2`
  - Recommended: Check latest on NuGet.org for MTP v2 support

### Verification Resources

1. **NuGet Package Page**: https://www.nuget.org/packages/Microsoft.Testing.Extensions.CodeCoverage
2. **GitHub Repository**: https://github.com/microsoft/testfx
3. **Release Notes**: Check package description for "Microsoft Testing Platform 2.x" or "MTP v2" mentions

## Testing the Migration

After migration, verify that:

1. ? `dotnet test` runs successfully
2. ? Tests can be discovered and executed in Visual Studio
3. ? Code coverage works correctly
4. ? Test results are reported properly
5. ? All existing tests pass

## References

- [xUnit v3.2.0-pre.10 Release Notes](https://xunit.net/releases/v3/3.2.0-pre.10)
- [Microsoft Testing Platform Documentation](https://learn.microsoft.com/en-us/dotnet/core/testing/microsoft-testing-platform)
- [xUnit Documentation](https://xunit.net/)

## Affected Projects

1. `test/UnitTests/MoneyGroup.UnitTests.csproj`
2. `test/IntegrationTests/MoneyGroup.IntegrationTests.csproj`
3. `test/FunctionalTests/MoneyGroup.FunctionalTests.csproj`

## Rollback Plan

If issues occur, rollback by:
1. Reverting `xunit.v3` to `3.0.1`
2. Re-adding `Microsoft.NET.Test.Sdk`
3. Re-adding `xunit.runner.visualstudio`
4. Re-adding `Microsoft.Testing.Extensions.VSTestBridge`

## Status

- ? Analysis complete
- ? Plan updated in `dotnet-upgrade-plan.md`
- ? Ready for execution
