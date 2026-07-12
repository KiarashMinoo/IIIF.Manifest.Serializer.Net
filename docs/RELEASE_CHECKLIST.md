# Release Checklist

Run through this before tagging a release / triggering `.github/workflows/publish-nuget.yml`'s
`publish` job (issue #13, "SDK Phase 7"). Most items are already automated by that workflow's
`pack` job - this checklist exists so a human (or a future CI gate) can confirm the same things
deliberately, and so the checks are documented in one place rather than only implicit in the
workflow YAML.

## Tests and quality gates

- [ ] `dotnet test tests/IIIF.Manifest.Serializer.Net.Tests/IIIF.Manifest.Serializer.Net.Tests.csproj --configuration Release` passes (all unit tests, including the full Cookbook/Demo catalog round-trip regression suite).
- [ ] `dotnet test tests/IIIF.Manifest.Serializer.Net.ArchTests/IIIF.Manifest.Serializer.Net.ArchTests.csproj --configuration Release` passes (namespace layering/naming conventions).
- [ ] Coverage report regenerated and reviewed for any new, uncovered public surface (`reportgenerator` per `docs/README.md`'s "Testing" section) - no hard gate is enforced yet, but check for regressions.
- [ ] `dotnet build IIIF.Manifest.Serializer.Net.slnx --configuration Release` produces 0 warnings, 0 errors.

## Packaging

- [ ] `Directory.Build.props`'s `<Version>` (core) and `extensions/Directory.Build.props`'s `<Version>` (all 3 extensions share one version number, independent of core) both reflect the intended release version.
- [ ] Package metadata verified: `Authors`/`Company`/`Product`/license expression/repository URL in `Directory.Build.props` are current and match this repository (see "License metadata" below).
- [ ] `./scripts/smoke-test-packages.ps1` passes locally - packs all 4 packages, installs them into a throwaway clean console app via `--source` (never a `ProjectReference`), and confirms a Manifest with a navPlace extension actually builds/serializes/round-trips through the *published artifact*, not just the in-repo project. This also runs automatically in `publish-nuget.yml`'s `pack` job, before packages are uploaded/published - a failure there blocks the `publish` job entirely.
- [ ] README install example (`docs/README.md`'s "Install" section) matches the actual package IDs and current version.

## License

- [ ] License metadata (`Directory.Build.props`/`extensions/Directory.Build.props`'s `PackageLicenseExpression`) matches the repository's actual `LICENSE` file (MIT, per `CLAUDE.md`'s licensing-discipline convention).
- [ ] No new dependency was introduced with an incompatible license, a paid tier, or telemetry/sponsorware since the last release - re-check anything added to `Directory.Packages.props` in `src/`/`extensions/`/`tests/` against `CLAUDE.md`'s licensing-discipline rule (MIT/BSD/Apache-2.0 only; no FluentAssertions ≥ 8.0.0; no Moq).

## Documentation

- [ ] `docs/SDK_VERSIONING_GUIDE.md`'s `## Status` line and round history are up to date with whatever round(s) this release includes.
- [ ] `docs/COOKBOOK_COVERAGE.md`/`docs/DEMO_COVERAGE.md` re-verified against upstream if either the Cookbook recipe set or the official demos page changed since the last check (see each doc's own "How this was verified"/"was researched" section for the re-check method).
- [ ] `docs/IIIF_UPSTREAM_COVERAGE_MATRIX.md` re-verified if a meaningful amount of time has passed since its last research pass (it records its own retrieval date - check whether upstream standards, `awesome-iiif`, or the validators have moved since then).
- [ ] Generated per-folder API reference docs under `docs/` (mirroring the source tree) are regenerated if any public API changed - see `docs/README.md`'s "Documentation index"/"Docs Catalog" for the full index and how these are produced.

## Public API compatibility

This SDK does not currently run an automated public-API-diff tool (e.g.
`Microsoft.CodeAnalysis.PublicApiAnalyzers`, `Microsoft.DotNet.ApiCompat`) - adopting one is a
larger, separate change (it requires committing an initial `PublicAPI.Shipped.txt`/
`PublicAPI.Unshipped.txt` baseline covering the SDK's entire existing public surface across 4
packages, which is a substantial one-time cost of its own). Until that's adopted:

- [ ] Manually confirm no public type/member was removed or had its signature changed in a
  binary-incompatible way since the last release, **unless** it was already `[Obsolete(error:
  true)]` in the previous release (per `CLAUDE.md`'s "Legacy mutators" convention - removing an
  already-error-obsoleted member across a version bump is the one sanctioned breaking-change path).
- [ ] If a `major`/breaking version bump is genuinely intended, document the breaking changes in
  the release notes explicitly - this SDK has no changelog file yet; consider adding one alongside
  the first release that needs to communicate a breaking change.
- [ ] **Future consideration**: revisit adopting `Microsoft.CodeAnalysis.PublicApiAnalyzers` (MIT
  licensed, satisfies the licensing-discipline rule) once the API surface is considered stable
  enough to baseline - not blocking for this release.

## Optional: official IIIF validator checks

Per `docs/IIIF_UPSTREAM_COVERAGE_MATRIX.md` §3's research: the **Presentation API validator**
(`IIIF/presentation-validator`) can validate local files/directories fully offline via its
`validate-dir` CLI command or official GitHub Action, with no live server dependency - a real,
concrete option for this checklist. The **Image API validator** cannot be used the same way (it
requires a live running Image API server responding to real tile/region/size requests, not a
static `info.json` file), so it's explicitly deferred, not silently skipped.

- [ ] **Deferred, not yet wired into CI**: run the Cookbook catalog's serialized output (both V3
  and V2.1, for every recipe) through the Presentation API validator's `validate-dir` before a
  release, to catch spec-shape regressions the SDK's own tests might miss. This is a genuine,
  actionable follow-up - not implemented in this pass because issue #13 (which this checklist
  comes from) explicitly keeps "enabling validator CI directly" out of scope, deferring it to a
  dedicated future change. If/when implemented: write each recipe's output JSON to a temp
  directory, install/run `iiif-validator validate-dir` (or the `IIIF/presentation-validator@main`
  GitHub Action) against it, fail the release workflow on any validator-reported error.
- [ ] Image API validator: **not planned** for this SDK's own CI without a new mock Image-API-server
  test harness (a distinct future feature, per the coverage matrix's finding) - re-evaluate only if
  that harness is ever built for other reasons.

## Post-tag

- [ ] Tag pushed (`v*.*.*`) triggers `publish-nuget.yml`'s `pack` → `publish` pipeline automatically; confirm the Actions run succeeds end-to-end, including the smoke-test step, before considering the release complete.
- [ ] Packages appear on nuget.org with the expected version(s) shortly after the `publish` job completes.
