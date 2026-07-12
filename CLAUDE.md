# CLAUDE.md

Project instructions for Claude Code working in this repository.

## What this is

A .NET library (`IIIF.Manifest.Serializer.Net`) for reading and writing **IIIF Presentation API**
resources in C# - `Manifest`, `Collection`, `Canvas`, `Range`, `Annotation`/`AnnotationPage`,
`AnnotationCollection`, and Content State 1.0 documents - plus the Auth, Search, Discovery, and
Image API service descriptors, and the navPlace/Georeference/Text Granularity extensions.

## The mandate (why this SDK is shaped the way it is)

This was originally a **Presentation API 2.0-only** library. The project's mandate, carried out
across several sessions and documented in full in [`docs/SDK_VERSIONING_GUIDE.md`](docs/SDK_VERSIONING_GUIDE.md),
was to reshape it into a **version-aware** SDK:

1. **Read every version** - Presentation API 2.0, 2.1, and 3.0 - without data loss, including
   genuinely legacy documents already in the wild.
2. **Write whichever version the caller asks for** (`IiifSerializer.Serialize(x, options)`),
   defaulting to 3.0. The public, preferred model surface is 3.0-shaped; legacy 2.x is a supported
   output format, not the primary way to construct new documents.
3. **Legacy properties become computed, read-only views.** The internal storage is 3.0-native
   (`Items`, `Behavior`, `Rights`, `RequiredStatement`, `Summary`, `PartOf`, ...). Old 2.x-only
   concepts (`Sequences`, `Canvas.Images`, `ViewingHint`, `License`, `Attribution`, `Within`, ...) are
   *getters only* - synthesized on demand from the same 3.0-native storage, never tagged
   `[Obsolete]` (reading old-shaped data must never produce compiler noise).
4. **Legacy mutators are read-only from now on.** Every setter/adder tied to a legacy shape
   (`AddSequence`, `AddImage`, `SetLicense`, `AddAttribution`, ...) is
   `[Obsolete("...", error: true)]` - a compile-time wall, not a runtime one. Code that already
   targets the old API keeps compiling as-is (Obsolete doesn't affect already-built binaries); new
   source code must go through the 3.0-native fluent API (`AddItem`, `AddAnnotation`, `SetRights`,
   `SetRequiredStatement`, ...) or explicitly suppress the warning-as-error. **This is the end
   state the user asked for: users work with the latest features; every obsolete item is
   read-only.**
5. **Cross-check against the wider IIIF ecosystem, not just the Presentation API spec itself.**
   [`github.com/IIIF/awesome-iiif`](https://github.com/IIIF/awesome-iiif)'s Standards section (Auth,
   Content Search, Change Discovery, Content State, Image API, plus the navPlace/Georeference/Text
   Granularity extensions) and [`github.com/IIIF`](https://github.com/IIIF) itself (the org's other
   repos - spec sources, cookbook, technical notes) are the reference for what a
   manifest/service-descriptor library like this one should model, beyond the Presentation API core.
   This was done once as a deep pass (see §10 onward in the versioning guide) and should be
   revisited periodically for anything new upstream, not treated as a one-time checklist.
6. **Ship runnable examples.** Every real recipe in
   [`iiif.io/api/cookbook`](https://iiif.io/api/cookbook/) (`github.com/IIIF/cookbook-recipes`) is
   implemented as a faithful C# builder in `examples/IIIF.Manifest.Serializer.Net.Cookbook`; a
   smaller hand-picked set lives in `examples/IIIF.Manifest.Serializer.Net.Examples`.
   [`iiif.io/demos/`](https://iiif.io/demos/) was checked and confirmed to be an external-links
   showcase page with no manifest content of its own - not a source for examples.
7. **Tests land with every change**, not in a separate pass at the end - round-trip (legacy in →
   3.0-native model → legacy views reflect the same data; 3.0 in → re-serialize as 3.0 →
   structurally equivalent; legacy in → 3.0 out; 3.0-constructed → legacy out), plus targeted
   gap-fill passes against the coverage report. Maximize coverage, but never at the cost of writing
   low-value tests for the sake of a number - see "Testing discipline" below.

## Current state (high-level - see the versioning guide for the authoritative, dated history)

- Presentation API 2.0, 2.1, and 3.0 - full read/write, version auto-detected on read.
- Auth 1.0 and 2.0 (4 real service types + response payloads), Content Search 2.0 (descriptors +
  response shapes), Change Discovery 1.0 (paging + activities), Content State 1.0 (deep-link object
  + base64url codec), Image API service descriptor (2.x/3.0 toggle, tiles, extraFormats, info.json).
- Extensions: navPlace, Georeference, Text Granularity - each an independently-versioned NuGet
  package.
- `System.Text.Json` interop: `Manifest`/`Collection`/`AnnotationCollection`/`ContentState` each
  carry a bridging converter, so using System.Text.Json directly (or ASP.NET Core's default
  (de)serialization) produces the same correct output as `IiifSerializer` - see
  [`docs/README.md`](docs/README.md#newtonsoftjson-and-systemtextjson-interop).
- Cookbook: 71 real recipes (78 catalog entries), built around a Strategy + Registry design
  (`IRecipeSet` implementations aggregated by `CookbookCatalog`).
- ~393 tests (xUnit + AwesomeAssertions - **not** FluentAssertions, see "Licensing" below), plus a
  dedicated `ArchTests` project (NetArchTest.Rules) enforcing namespace layering. ~82% line
  coverage.
- Full generated API reference under `docs/` mirroring the source tree (see
  [`docs/README.md`](docs/README.md#docs-catalog) for the catalog).

Don't re-derive any of this from first principles - read
[`docs/SDK_VERSIONING_GUIDE.md`](docs/SDK_VERSIONING_GUIDE.md) first. It's a dated, append-only
milestone log: each round/milestone explains a real decision and the reasoning behind it. Add new
rounds to the end; don't rewrite history that's already there.

## Where to look

| Need | Look here |
| --- | --- |
| Project overview, quick start, install | [`docs/README.md`](docs/README.md) |
| Architecture rationale, 2.x↔3.0 mapping table, full milestone history | [`docs/SDK_VERSIONING_GUIDE.md`](docs/SDK_VERSIONING_GUIDE.md) |
| Per-folder API reference (types, members, diagrams) | `docs/<mirrored-source-path>/README.md` - index at [`docs/README.md#docs-catalog`](docs/README.md#docs-catalog) |
| Cookbook recipes | `examples/IIIF.Manifest.Serializer.Net.Cookbook` |
| Hand-picked feature demos | `examples/IIIF.Manifest.Serializer.Net.Examples` |
| Unit tests | `tests/IIIF.Manifest.Serializer.Net.Tests` |
| Architecture/layering tests | `tests/IIIF.Manifest.Serializer.Net.ArchTests` |
| Coverage report | `coverage-report/` (regenerate per "Build & test" below) |

## Conventions to follow

- **Base class chain**: everything derives from `TrackableObject<T>` (change-tracked, fluent
  setters via `GetElementValue`/`SetElementValue`), through `BaseItem<T>` (`@id`/`@type`/embedded
  `service`) and `BaseNode<T>` (label/summary/metadata/thumbnail/rendering/homepage/seeAlso/rights/
  requiredStatement/partOf/behavior/provider - shared by every top-level resource).
- **New 3.0-native property**: back any legacy 2.x equivalent with a *computed, read-only* view -
  never the other way around. Legacy getters stay untagged; legacy setters get
  `[Obsolete(error: true)]`.
- **`[JsonIgnore]` new 3.0-only storage properties** that shouldn't leak into legacy 2.x JSON via
  plain `JsonConvert` reflection (this bit the project twice already - see the versioning guide's
  "Known follow-up" notes).
- **`IiifSerializer`** is a thin Facade - it holds only the public `Serialize`/`Deserialize*`
  overloads and version dispatch. The hand-rolled 3.0 read/write logic lives in sibling
  `IiifSerializer.<ResourceType>.cs` partial-class files, one per resource-type responsibility. Add
  new resource-type read/write logic in its own partial file rather than growing one already-large
  file.
- **Licensing discipline for dependencies**: MIT/BSD/Apache-2.0 only, no paid tier, no
  telemetry/sponsorware. Concretely: use **AwesomeAssertions**, not FluentAssertions ≥ 8.0.0
  (relicensed to a paid commercial model); never introduce Moq (2023 SponsorLink telemetry
  incident) - NSubstitute if a mocking library is ever genuinely needed (it currently isn't - this
  is a data/serialization library with nothing to substitute).
- **Testing discipline**: xUnit + AwesomeAssertions, no AAA-comment boilerplate, descriptive test
  names (`<Member>_<Scenario>_<Expected>`). Land tests with the change that needs them, not in a
  separate pass. When asked to improve coverage, gap-fill against the actual coverage report
  (`dotnet test ... --collect:"XPlat Code Coverage"` + `reportgenerator`) rather than mechanically
  generating a test file per type regardless of existing coverage - many types here are trivial
  value wrappers where a full round-trip test elsewhere already exercises them.
- **Architecture tests** (`tests/IIIF.Manifest.Serializer.Net.ArchTests`) encode the real,
  verified namespace layering (`Shared`/`Properties` must not depend on `Nodes`, with two
  explicitly-documented exceptions; `Helpers` types must be static). Check it before introducing a
  new cross-namespace dependency, and update it (with a documented reason) if a new dependency is
  genuinely intentional rather than accidental.
- **Never modify production code during a test-generation-only pass** - if gap-fill testing
  surfaces a real bug (this has happened - see `PolynomialTransformationTests`/
  `DimensionAndViewingDirectionSupportHelperTests` for two known, currently-dead ones), document it
  in the test and flag it; don't silently fix it unless the user asks you to.
- **Don't wipe `/docs`** without explicit confirmation - `docs/README.md` and
  `docs/SDK_VERSIONING_GUIDE.md` are hand-authored, not regenerated from source.

## Build & test

```powershell
dotnet build IIIF.Manifest.Serializer.Net.slnx
dotnet test tests/IIIF.Manifest.Serializer.Net.Tests/IIIF.Manifest.Serializer.Net.Tests.csproj
dotnet test tests/IIIF.Manifest.Serializer.Net.ArchTests/IIIF.Manifest.Serializer.Net.ArchTests.csproj
```

Coverage:

```powershell
dotnet test tests/IIIF.Manifest.Serializer.Net.Tests/IIIF.Manifest.Serializer.Net.Tests.csproj `
  --collect:"XPlat Code Coverage" `
  --settings tests/IIIF.Manifest.Serializer.Net.Tests/coverlet.runsettings `
  --results-directory ./coverage-raw
reportgenerator -reports:"coverage-raw/**/coverage.cobertura.xml" -targetdir:coverage-report -reporttypes:"TextSummary;Html;MarkdownSummaryGithub"
```

## Known stale file

`.github/copilot-instructions.md` still describes the *original* Presentation-2.0-only shape of
this library (per-type subfolders like `Nodes/Manifest/`, `Sequence`-centric model, "doesn't
implement Auth/Search/Discovery/Content State") - all superseded by the work this file describes.
Treat it as historical, not current guidance, until it's rewritten.
