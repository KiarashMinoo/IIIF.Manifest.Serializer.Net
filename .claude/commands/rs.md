---
description: "Alias for /restructure — restructure a module (or the whole solution) into the CLAUDE.md-defined layout, update CI/CD + Dockerfile, and commit"
argument-hint: <module-name-or-"all">
---

Target: $ARGUMENTS

If `$ARGUMENTS` isn't a real module under `Modules/` and isn't `all`, ask which module(s) before doing anything.

1. **Branch**: `git status` — stash unrelated changes (`git stash push -u`) or ask, never discard silently. `git fetch origin`, checkout + pull the default branch, then create `refactor/<scope>-module-structure` (`<scope>` = kebab-case module name, or `solution` for `all`).
2. **Restructure** each target module's core project (`Modules/<Module>/MinooTrading.<Module>/`) per this repo's `CLAUDE.md` ("Standard module internal folder structure" + "Database-backed modules"):
   - `Domain/` ← one subfolder per aggregate, `Domain/<AggregateName>/`: the aggregate root, its strongly-typed ID (`<AggregateName>Id`), its value objects, the DTOs that represent *that aggregate's own shape*, and its repository interface (`I<AggregateName>Repository`). Enums/exceptions/state-machine logic shared across aggregates stay at `Domain/` root.
   - `Application/` ← use-case/orchestration services + command objects (`Application/Services/` or `Application/Features/<Feature>/`).
   - `Infrastructure/` ← DbContext, `IEntityTypeConfiguration`s, repository *implementations* (the interface moved to `Domain/<AggregateName>/`), external adapters.
   - `Abstractions/` ← cross-module contracts, **only** if the module has no dedicated `.Abstractions` project.
   - Root `Models/`/`ViewModels/`: leave HTTP wire-contract DTOs here (`Create*Request`, paged-list responses, etc.) that don't map 1:1 onto one aggregate. Move any DTO that *does* represent a single aggregate's shape into that aggregate's `Domain/<AggregateName>/` folder instead.
   - Leave `Controllers/`, `Views/`, `ViewComponents/`, `Helpers/`, `assets/`, `Module.cs`, `Permissions.cs`, `<Module>RouteConvention.cs`, `<Module>ModuleOptions.cs`, `MinooTrading<Module>Extensions.cs` at the project root, untouched.
   - Use `git mv` (preserve history). Update each moved file's namespace to match its new path 1:1, and fix every `using` referencing it — including per-provider projects (`MinooTrading.<Module>.{MySQL,PostgreSQL,SQLite,SQLServer,Mongo}`) and other consuming modules.
   - While touching any file (moved or fixed-up), also apply CLAUDE.md's "Coding Conventions": replace string literals that duplicate an existing identifier with `nameof(...)`, pull repeated/meaningful literals into named constants, and convert direct `ILogger` calls (`LogInformation`/`LogWarning`/`LogError`/etc.) in that file to `[LoggerMessage]` source-generated partials. Don't leave these as debt just because the file was already open for the move.
   - `dotnet build` after each module — never leave the tree broken.
3. **CI/CD + Dockerfile**: internal moves don't change project/`.csproj` paths, but check `.github/workflows/*.yml` and `Dockerfile` for any path hardcoded past the project root (not just working-directory/project-path entries), and `Modules/Directory.Build.props`'s embedded-resource globs (`assets\dist\**\*` etc., unaffected since `assets/` doesn't move). Fix what's stale; state explicitly if nothing needed changing.
4. **Commit** one final commit after reviewing `git status`/`git diff --stat`:
   ```
   git commit -m "$(cat <<'EOF'
   Restructure <scope> to Domain/Application/Infrastructure/Abstractions layout

   - <module>: moved <what> into Domain/, Application/, Infrastructure/(...)
   - Updated namespaces and cross-project `using` statements
   - Updated <workflow/Dockerfile files>, if any
   - Verified `dotnet build` passes

   Co-Authored-By: Claude Sonnet 5 <noreply@anthropic.com>
   EOF
   )"
   ```
   Don't push or open a PR unless asked separately.