---
mode: agent
description: "Always scan and generate tests (no inputs). Reuse or create IIIF.Manifest.Serializer.Net.UnitTests and IIIF.Manifest.Serializer.Net.ArchTests, ensure they are in the .sln under a 'Tests' solution folder. Generate xUnit + NSubstitute unit tests mirroring source, and architecture tests with NetArchTest. Non-interactive and idempotent. Bitbucket/GitHub-neutral."
---

# Repo Test Generator (no inputs; always scan & update)

Act autonomously and **do not ask for confirmations**. Use safe defaults. Make idempotent changes.  
**Never modify production code** — only create/update files inside the test projects.

The repo may be hosted on GitHub or Bitbucket; this has **no impact** on behavior except for occasional CI-related paths (`.github`, `.bitbucket`) that must be ignored while scanning for source projects.

---

## 0) Global behavior (no arguments)

- Always perform a **full scan** of the repository.
- **Reuse** existing test projects named (case-insensitive):
  - `IIIF.Manifest.Serializer.Net.UnitTests`
  - `IIIF.Manifest.Serializer.Net.ArchTests`
- If missing, **create** them under:
  - `Tests/UnitTests/IIIF.Manifest.Serializer.Net.UnitTests.csproj`
  - `Tests/ArchTests/IIIF.Manifest.Serializer.Net.ArchTests.csproj`
- **Always** make sure both projects are inside a **solution (`.sln`)** under a solution folder named **`Tests`**.  
  - If no `.sln` exists, create one at the repo root (name it after the repo folder).

---

## 1) Discovery (filesystem, deep, no depth limit)

Walk the entire repo except these paths/segments (case-insensitive):

- `Tests/**` (the tests you manage)
- `.git`
- `.github` (but keep `.github/prompts`)
- `.bitbucket`
- `.vs`, `.idea`
- `bin`, `obj`, `packages`, `artifacts`, typical caches
- `node_modules`
- any path whose **segment** matches (case-insensitive):  
  `test`, `tests`, `.tests`, `*unit*test*`, `*integration*test*`

From the remaining paths:

- Identify **testable source projects**: `*.csproj` not under excluded paths and not themselves test projects.
- For each directory with C# code, enumerate **public concrete types** (classes/records/structs) that contain logic.

### Path mapping from code → tests

When creating test file paths, **drop common root segments** if they appear at the start of a path (case-insensitive):

- `src`, `source`, `app`, `apps`, `packages`, `projects`, `modules`

Examples:

- `src/Infrastructure/Pipelines/ChannelService.cs`
  → `Infrastructure/Pipelines/ChannelServiceTests.cs` in the UnitTests project.

- `app/Application/Channel/Direct.cs`
  → `Application/Channel/DirectTests.cs` in the UnitTests project.

Keep directories to mirror the logical structure; only remove the noisy top-level segments.

---

## 2) Test projects (reuse or create)

### 2.1 Reuse if found

- Search for csproj whose filename or `<AssemblyName>` equals **IIIF.Manifest.Serializer.Net.UnitTests** and **IIIF.Manifest.Serializer.Net.ArchTests** (case-insensitive).
- Record absolute paths; **keep original names/locations**.
- When reusing, do **not** downgrade existing package versions (only add missing ones or upgrade when clearly safe).

### 2.2 Create if missing

#### Unit tests project (IIIF.Manifest.Serializer.Net.UnitTests)

If no suitable unit test project exists, create `Tests/UnitTests/IIIF.Manifest.Serializer.Net.UnitTests.csproj` with baseline:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <IsPackable>false</IsPackable>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.11.1" />
    <PackageReference Include="xunit" Version="2.9.2" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.8.2">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="NSubstitute" Version="5.1.0" />
    <PackageReference Include="coverlet.collector" Version="6.0.2">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
  </ItemGroup>
</Project>
```

Add project references from `IIIF.Manifest.Serializer.Net.UnitTests` to all discovered source projects that you will generate tests for.

#### Architecture tests project (IIIF.Manifest.Serializer.Net.ArchTests)

If no suitable architecture test project exists, create `Tests/ArchTests/IIIF.Manifest.Serializer.Net.ArchTests.csproj` with baseline:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <IsPackable>false</IsPackable>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.11.1" />
    <PackageReference Include="xunit" Version="2.9.2" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.8.2">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="NetArchTest.Rules" Version="1.3.2" />
  </ItemGroup>
</Project>
```

Add project references from `IIIF.Manifest.Serializer.Net.ArchTests` to all relevant source projects (typically the main application/infrastructure assemblies).

---

## 3) Solution management

- Locate an existing `.sln` file at the repo root, or create one named after the repo folder.
- Ensure both test projects are:

  - Included in the solution.
  - Placed under a solution folder named `Tests`.

- Do not remove any existing projects or solution folders.

---

## 4) Unit test generation rules (xUnit + NSubstitute)

For each discovered **public concrete type** in source projects:

1. Compute its logical test path using the mapping rules.
2. Use the **same namespace root** but under a `Tests`-appropriate root, e.g., `MyCompany.MyProduct.Tests` or `MyCompany.MyProduct.UnitTests` (inherit from existing patterns if present).
3. For each type:

   - Create or update a `*Tests` class:
     - `FooService` → `FooServiceTests`.
   - Ensure the test class is `public` and marked with `[Collection]` only if necessary (reuse existing patterns).
   - Generate tests for:
     - Public methods with branches/conditions.
     - Public async methods (using `async Task` tests).
     - Constructor behaviors (guard clauses, dependency handling).

4. Use **NSubstitute** for dependencies:

   - Inject mocks for constructor parameters that are interfaces or abstract types.
   - Use `Substitute.For<T>()` to create mocks.
   - Show minimal but meaningful verification of interactions.

5. Test style:

   - xUnit `[Fact]` for single-scenario tests.
   - `[Theory]` + `[InlineData(...)]` for basic parameterized cases (when appropriate).
   - Aim for **compilable but minimal** tests that can be refined later.
   - Avoid over-mocking; keep tests focused on one behavior per method.

6. Idempotency:

   - When regenerating tests, **preserve** any hand-written tests if possible:
     - Only update or add regions clearly marked as generated.
     - Or follow a pattern like `// <auto-generated> DO NOT EDIT` for generated files, and do not touch purely manual tests.

---

## 5) Architecture tests (NetArchTest)

In `IIIF.Manifest.Serializer.Net.ArchTests`:

- Generate tests that enforce architecture rules, such as:

  - Namespaces / layers (e.g., Application should not depend on Infrastructure directly).
  - Types naming conventions (`*Controller`, `*Service`, `*Repository`, etc.).
  - No forbidden references (e.g., UI frameworks inside domain layer).

- Use `NetArchTest.Rules` to express rules like:

  ```csharp
  [Fact]
  public void Domain_Should_Not_Depend_On_Infrastructure()
  {
      var result = Types
          .InAssembly(typeof(MyDomainRoot).Assembly)
          .Should()
          .NotHaveDependencyOn("MyCompany.MyProduct.Infrastructure")
          .GetResult();

      result.IsSuccessful.Should().BeTrue();
  }
  ```

- Adjust namespaces/assemblies according to the actual repo structure.

---

## 6) CI / Bitbucket / GitHub neutrality

- Ignore `.github`, `.bitbucket`, and CI-specific folders when scanning for source projects, except when they contain prompt/config files unrelated to tests (`.github/prompts` should still be left untouched).
- Do not add or modify CI configuration files (Bitbucket Pipelines, GitHub Actions) unless explicitly asked in a future prompt.

---

## 7) Final report

After making all changes, provide a short textual summary (for the user) describing:

- Which test projects were reused or created.
- How many new/updated unit test files were produced.
- How many architecture test files were produced or updated.
- Any notable gaps (e.g., projects skipped because they had no public concrete types).

Make sure the behavior is deterministic and idempotent across runs with the same repository contents.
