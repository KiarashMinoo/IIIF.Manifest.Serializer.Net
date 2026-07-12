# IIIF.Manifest.Serializer.Net

A .NET (netstandard2.1) library for reading and writing **IIIF Presentation API** manifests -
`Manifest`, `Collection`, `Canvas`, `Range`, `Annotation`/`AnnotationPage`, `AnnotationCollection`,
and Content State 1.0 documents - across **Presentation API 2.0, 2.1, and 3.0**, plus the Auth,
Search, Discovery, and Image API service descriptors and the navPlace/Georeference/Text
Granularity extensions.

## Contents

- [Why this library](#why-this-library)
- [Install](#install)
- [Quick start](#quick-start)
- [Multi-version serialization](#multi-version-serialization)
- [Newtonsoft.Json and System.Text.Json interop](#newtonsoftjson-and-systemtextjson-interop)
- [Project layout](#project-layout)
- [The object model](#the-object-model)
- [`IiifSerializer` architecture](#iifserializer-architecture)
- [Services](#services)
- [Extension packages](#extension-packages)
- [Examples and Cookbook](#examples-and-cookbook)
- [Testing](#testing)
- [Documentation index](#documentation-index)
- [Contributing](#contributing)
- [Release hygiene](#release-hygiene)
- [License](#license)

## Why this library

Real-world IIIF content is a mix of vintage 2.0/2.1 manifests and modern 3.0 documents, often both
in the same collection. This SDK's model is **3.0-native internally** - `Items`, `Behavior`,
`Rights`, `RequiredStatement`, `Summary`, `PartOf`, etc. are the real backing storage - while every
legacy 2.x-only concept (`Sequences`, `Canvas.Images`, `ViewingHint`, `License`, `Attribution`,
`Within`, ...) is a **computed, read-only view** derived from that same storage. That means:

- Parsing a 2.0/2.1 document and re-serializing it as 3.0 (or vice versa) just works, with no
  manual mapping code required.
- Reading old-shaped data never breaks: legacy getters (`manifest.Sequences`, `canvas.Images`, ...)
  keep working after parsing any version.
- Writing new documents against the legacy shape is intentionally a compile error: legacy
  mutators (`AddSequence`, `AddImage`, `SetLicense`, ...) are `[Obsolete(error: true)]`, steering
  new code onto the 3.0-native API (`AddItem`, `AddAnnotation`, `SetRights`, ...) without deleting
  the types existing consumers may still reference by name.

See [`SDK_VERSIONING_GUIDE.md`](SDK_VERSIONING_GUIDE.md) for the full architecture rationale, the
confirmed 2.x↔3.0 property mapping table, and the session-by-session history of how the reshape was
implemented and verified.

## Install

```powershell
dotnet add package IIIF.Manifest.Serializer.Net
```

Optional extension packages, each versioned and shipped independently:

```powershell
dotnet add package IIIF.Manifest.Serializer.Net.NavPlace
dotnet add package IIIF.Manifest.Serializer.Net.Georeference
dotnet add package IIIF.Manifest.Serializer.Net.TextGranularity
```

## Quick start

```csharp
using IIIF.Manifests.Serializer;
using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Nodes.Contents.Annotation;
using IIIF.Manifests.Serializer.Nodes.Contents.Image.Resource;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.Services;

var manifest = new Manifest("https://example.org/iiif/manuscript/manifest.json", new Label("16th Century Manuscript"))
    .SetViewingDirection(ViewingDirection.Ltr);

var canvas = new Canvas("https://example.org/iiif/manuscript/canvas/p1", new Label("Folio 1r"), height: 2000, width: 1500);

var image = new ImageResource("https://example.org/iiif/manuscript/full/full/0/default.jpg", "image/jpeg")
    .SetHeight(2000).SetWidth(1500)
    .AddService(new Service("http://iiif.io/api/image/3/context.json", "https://example.org/iiif/manuscript", "level2"));

canvas.AddAnnotation(new Annotation("https://example.org/iiif/manuscript/annotation/p1", image, canvas.Id));
manifest.AddItem(canvas);

// Defaults to Presentation API 3.0.
string json = IiifSerializer.Serialize(manifest);

// Round-trip: version is auto-detected from the JSON shape.
Manifest parsed = IiifSerializer.DeserializeManifest(json);
```

`Collection` and the standalone `AnnotationCollection` document type have the same
`Serialize`/`Deserialize*` shape (`IiifSerializer.Serialize(collection)`,
`IiifSerializer.DeserializeCollection(json)`, etc.).

## Multi-version serialization

```csharp
using IIIF.Manifests.Serializer;

// Write the same 3.0-native model as legacy 2.1 JSON instead.
string legacyJson = IiifSerializer.Serialize(manifest, new IiifSerializerOptions(IiifPresentationVersion.V2_1));

// Reading is always version-agnostic - the SDK detects 2.0/2.1/3.0 from the document's own shape.
Manifest fromLegacy = IiifSerializer.DeserializeManifest(legacyJson);

// Legacy views over the same 3.0-native storage still work after parsing either version:
var firstSequenceCanvases = fromLegacy.Sequences.Single().Canvases; // computed, read-only
```

`IiifPresentationVersionDetector` also recognizes (but does not import/export) two further
`IiifPresentationVersion` values, for classification rather than authoring: `Metadata_1_0` (the
"Shared Canvas" predecessor to Presentation 2.0, `@context`:
`http://www.shared-canvas.org/ns/context.json`) and `V4_0_Rc` (Presentation 4.0, still a draft as
of this writing, `@context`: `http://iiif.io/api/presentation/4/context.json`). Requesting either
as a **write** target throws `NotSupportedException` rather than silently producing output - see
[`SDK_VERSIONING_GUIDE.md`, Round 6](SDK_VERSIONING_GUIDE.md#round-6-harden-version-detection---metadata-api-10-and-presentation-40-rc-tracking)
for the full research and rationale.

### Downgrade limitations (writing 3.0-native data as legacy 2.x)

Writing a 3.0-native model as `V2_0`/`V2_1` is **deterministic**, but not always lossless - some
3.0-only concepts have no 2.x equivalent at all. This SDK's rule (per
[`SDK_VERSIONING_GUIDE.md`, Round 10](SDK_VERSIONING_GUIDE.md#round-10-versioned-writer-audit-issue-7)):
convert to the legacy shape when a safe equivalent exists, otherwise omit the property from legacy
output rather than guessing or throwing.

| 3.0-native property | 2.x write behavior |
| --- | --- |
| `summary`, `requiredStatement`, `rights`, `partOf`, `homepage` | Always converted to `description`/`attribution`/`license`/`within`/`related` - these are computed legacy views over the same storage, so nothing is lost. |
| `behavior` | Converted to `viewingHint` only for the values valid in both (`paged`, `continuous`, `individuals`, `facing-pages`, `non-paged`, `multi-part`). Behavior-only values (`unordered`, `sequence`, `thumbnail-nav`, `no-nav`, `auto-advance`, `no-auto-advance`, `repeat`, `no-repeat`, `together`, `hidden`) are omitted - `viewingHint` is left unset rather than picking an arbitrary/incorrect value. |
| `items` beyond the first `Manifest` sequence | A `Manifest` with more than one legacy `sequence` on read keeps the extra sequences on `AdditionalSequences` precisely so they round-trip back out on 2.x write - see [Round 7](SDK_VERSIONING_GUIDE.md#round-7-audit-legacy-import-normalization-sdk-phase-2). A **3.0-constructed** `Manifest` only ever has one canvas ordering (`Items`), so it always writes as a single `sequence`. |
| `placeholderCanvas`, `accompanyingCanvas`, `start`, top-level `services` | 3.0-only, no 2.x equivalent shape at all - omitted entirely from `V2_0`/`V2_1` output. |
| `AnnotationCollection` | 3.0-only resource type - `Serialize(annotationCollection, ...)` throws `NotSupportedException` for any non-`V3_0` target rather than attempting a legacy shape that doesn't exist. |

## Newtonsoft.Json and System.Text.Json interop

This SDK is built on **Newtonsoft.Json** (custom `[JsonConverter]`s, `IiifSerializer`'s hand-rolled
3.0 writer/reader), so calling `IiifSerializer.Serialize`/`Deserialize*` - or, for the 2.x shape,
plain `JsonConvert.SerializeObject(manifest)` - always works correctly with no extra setup.

`Manifest`, `Collection`, `AnnotationCollection`, and `ContentState` also each carry a
`System.Text.Json` bridging converter, so a developer who serializes/deserializes with
**System.Text.Json** instead - directly, or implicitly via ASP.NET Core's default request/response
(de)serialization - gets the same correct result with no extra configuration:

```csharp
using System.Text.Json;

// Defaults to IIIF Presentation API 3.0, same as IiifSerializer.Serialize(manifest).
string json = JsonSerializer.Serialize(manifest);

// Version is auto-detected from the JSON shape, same as IiifSerializer.DeserializeManifest.
Manifest parsed = JsonSerializer.Deserialize<Manifest>(json)!;

// Also works directly as an ASP.NET Core action result - no [JsonConverter] registration needed:
// return Ok(manifest);
```

Each bridge converter (`src/IIIF.Manifest.Serializer.Net/SystemTextJson/`) delegates to this SDK's
existing Newtonsoft-based logic rather than reimplementing it, so there is exactly one source of
truth for how each document type reads and writes IIIF JSON regardless of which library a
consumer's application happens to use elsewhere. Nested types (`Canvas`, `Service`, selectors, ...)
have no System.Text.Json converter of their own - they're only ever reached inside one of the 4
top-level documents' own JSON tree, which the bridge builds via the existing Newtonsoft logic.

## Project layout

```
src/IIIF.Manifest.Serializer.Net/     Core SDK (netstandard2.1)
extensions/                           Independently-versioned extension packages
  IIIF.Manifest.Serializer.Net.NavPlace          navPlace geolocation extension
  IIIF.Manifest.Serializer.Net.Georeference      Georeference (image-to-map) extension
  IIIF.Manifest.Serializer.Net.TextGranularity   Text Granularity extension
examples/
  IIIF.Manifest.Serializer.Net.Examples          Small hand-picked feature demos
  IIIF.Manifest.Serializer.Net.Cookbook          Every github.com/IIIF/cookbook-recipes recipe, runnable
tests/
  IIIF.Manifest.Serializer.Net.Tests             xUnit + AwesomeAssertions test suite
  IIIF.Manifest.Serializer.Net.ArchTests         NetArchTest.Rules namespace-layering checks
docs/                                            This documentation set
```

## The object model

Everything derives from `TrackableObject<T>` (change-tracked, fluent-setter base), through
`BaseItem<T>` (`@id`/`@type`/embedded `service`) and `BaseNode<T>` (the descriptive properties
shared by every top-level resource: label, summary, metadata, thumbnail, rendering, homepage,
seeAlso, rights, requiredStatement, partOf, behavior, provider).

| Type | Namespace | Role |
| --- | --- | --- |
| `Manifest` | `Nodes` | Top-level resource for a single object; `Items` holds `Canvas`. |
| `Collection` | `Nodes` | Top-level resource grouping `Manifest`/`Collection` references via `Items`. |
| `Canvas` | `Nodes` | A painting surface; `Items` holds `AnnotationPage`/`Annotation`. |
| `Structure` (Range) | `Nodes` | Hierarchical navigation (tables of contents, chapters). |
| `Annotation` / `AnnotationPage` | `Nodes.Contents.Annotation` | W3C Web Annotation body/target model used for painting, commenting, tagging, and supplementing. |
| `AnnotationCollection` | `Nodes.Contents.Annotation` | Standalone paged annotation list document (`total`/`first`/`last`). |
| `ContentState` | `Nodes.Contents.ContentState` | IIIF Content State 1.0 deep-link document, plus base64url codec. |
| `Choice` | `Nodes.Contents.Choice` | Mutually-exclusive body alternatives (language/format/quality variants). |
| Resource bodies | `Nodes.Contents.{Image,Audio,Video,Textual,Embedded,OtherContent,Segment}` | The concrete annotation body/resource types. |
| Selectors | `Shared.Selectors` | `FragmentSelector`, `PointSelector`, `ImageApiSelector`, `SvgSelector`. |

Legacy-only types with no 3.0 replacement (`Sequence`, `Layer`, `AnnotationList`) are kept as
read/write shims for round-tripping old documents - they are not part of the recommended 3.0
authoring API.

## `IiifSerializer` architecture

`IiifSerializer` (`src/IIIF.Manifest.Serializer.Net/IiifSerializer.cs`) is a thin **Facade**: it
holds only the public `Serialize`/`Deserialize*` overloads and the 2.x-vs-3.0 version dispatch.
2.0/2.1 read/write is plain `JsonConvert` against the model's own `[JsonProperty]`/`[JsonConverter]`
attributes (the 3.0-native storage makes this direct). The hand-rolled 3.0 read/write logic lives
in sibling `partial class` files split by resource-type responsibility, so no single file holds the
whole serializer:

| File | Responsibility |
| --- | --- |
| `IiifSerializer.Manifest.cs` | `Manifest` read/write |
| `IiifSerializer.Collection.cs` | `Collection` read/write |
| `IiifSerializer.AnnotationCollection.cs` | `AnnotationCollection` read/write |
| `IiifSerializer.Canvas.cs` | `Canvas` read/write |
| `IiifSerializer.Range.cs` | `Structure`/Range read/write |
| `IiifSerializer.Annotation.cs` | `AnnotationPage`/`Annotation` and the polymorphic body dispatch (Image/Audio/Video/Textual/Choice/SpecificResource/extension types) |
| `IiifSerializer.NodeExtras.cs` | Properties shared by every `BaseNode` (rights, requiredStatement, partOf, summary, metadata, thumbnail, rendering, homepage, seeAlso, behavior) |
| `IiifSerializer.Metadata.cs` / `.ImageLikeResources.cs` / `.LinkResources.cs` / `.Provider.cs` / `.Service.cs` | The individual descriptive-resource read/write helpers used by `NodeExtras` |
| `IiifSerializer.Helpers.cs` | Shared language-map/label/description parsing |

`IiifPresentationVersionDetector` inspects a JSON document's own shape (`@context`/`items` vs.
`sequences`, etc.) to pick the right reader automatically.

## Services

`Properties/Services/` models the embedded/top-level service descriptors: the Image API service
(`Service`, with `AsImageService2()`/`AsImageService3()` toggles), Auth 1.0 (`AuthService1`), the
four Auth 2.0 service types plus their response payloads (`Properties/Services/Auth2/`), Search 2.0
descriptors and response shapes (`Properties/Services/Search/`), Change Discovery 1.0
(`DiscoveryService`, `Properties/Services/Discovery/`), and Content State 1.0
(`ContentStateService`). `ServiceJsonConverter` dispatches polymorphically on read by `@type`/`type`
and, where that's ambiguous (Auth 1.0 vs. 2.0), by `@context`.

### Auxiliary API coverage table

Per-family status for the non-core Presentation API surface (Image, Auth, Search, Discovery,
Content State) - what's modeled, whether both standalone and embedded forms work, and any
explicitly out-of-scope areas. See
[`SDK_VERSIONING_GUIDE.md`, Round 11](SDK_VERSIONING_GUIDE.md#round-11-auxiliary-api-surface-audit-issue-8)
for the full audit this table summarizes.

| Family | Descriptor(s) | Standalone payload | Embedded on a resource | Notes / out-of-scope |
| --- | --- | --- | --- | --- |
| **Image API** | `Service` (`profile`, `protocol`, `width`/`height`, `tiles`, `sizes`, `preferredFormats`, `extraFormats`, `extraQualities`, `extraFeatures`, 3.0-only `maxWidth`/`maxHeight`/`maxArea`/`rights`) | `ToInfoJson()`/`FromInfoJson()` read+write a standalone `info.json` (unprefixed `id`/`type`) | Yes - `service` on any content resource (`ImageResource`, etc.) and on Manifest/Canvas | A Presentation-3-shaped document embedding an `ImageService2` (not `ImageService3`) is a documented, spec-allowed mixed-version scenario - see `ServiceRoundTripTests.ImageService_Should_RoundTripWhenEmbeddedOnAnImageResource_ThroughIiifSerializer`. |
| **Auth** | Auth 1.0 `AuthService1`; Auth 2.0 `AuthProbeService2`/`AuthAccessService2`/`AuthAccessTokenService2`/`AuthLogoutService2` plus response payloads (`AuthProbeResult2`, `AuthAccessToken2`, `AuthAccessTokenError2`) | Response payloads only (Auth has no standalone "info.json"-equivalent document per spec) | Yes - embedded on Manifest; the full probe→access→token/logout chain round-trips as nested inline services | Auth 1.0 and 2.0 are mutually exclusive per document (a resource picks one flow), both supported for read/write. |
| **Content Search** | `SearchService`/`AutoCompleteService` descriptors; `SearchResponse` (hit-highlighting `AnnotationPage` with paging), `TermPageResponse` (autocomplete) | Response payloads are standalone documents fetched from the service's `id` URL - not embedded | Yes - `SearchService` (with a nested `AutoCompleteService`) embeds on Manifest/Collection | **Search 1.0 legacy response compatibility is explicitly out of scope**: it predates and is structurally superseded by Search 2.0, and this SDK's `Profile` property already accepts a Search-1.0-shaped profile URI as a plain string extension point without needing a dedicated legacy response type. |
| **Change Discovery** | `DiscoveryService` (`OrderedCollection`), `DiscoveryCollectionPage` (`OrderedCollectionPage`), `Activity`/`ActivityObject`/`DiscoveryAgent`/`DiscoveryDataset` | Yes - a `DiscoveryCollectionPage`/`DiscoveryService` document is fetched standalone from its `id` | Yes - `DiscoveryService` embeds on Manifest/Collection as a `service` entry | `Activity.Type` is a plain string, not a closed enum - every spec activity type (`Create`/`Update`/`Delete`/`Move`/`Add`/`Remove`) round-trips identically, not just the ones with dedicated fixtures. |
| **Content State** | `ContentState` (annotation-shaped deep link), `ContentStateTarget` (whole-resource or Media-Fragment-suffixed region target), `ContentStatePointSelector` | `ContentStateCodec.Encode`/`Decode` - the `iiif-content` base64url string is the standalone form by definition | `ContentStateService` embeds on Manifest/Collection to advertise where content-state links are accepted | **No region/`FragmentSelector` object exists by design**, not omission - Content State 1.0 has no selector form for `xywh` region targeting; it's expressed as a `#xywh=...` Media Fragment suffix directly on the target `id` string (confirmed against spec; see `SDK_VERSIONING_GUIDE.md` Milestone 21). |

## Extension packages

Each extension is a standalone NuGet package that adds 3.0-only, spec-defined data to any
`BaseNode`/`Annotation` via the SDK's additional-properties mechanism - consuming one needs no core
SDK source changes, though reading/writing through `IiifSerializer`'s hand-rolled V3 path
specifically does rely on a small, targeted preservation step in core (see
[SDK_VERSIONING_GUIDE.md, Round 12](SDK_VERSIONING_GUIDE.md#round-12-extension-package-hardening-issue-9)):

- **NavPlace** - GeoJSON `navPlace` geolocation (`Point`/`LineString`/`Polygon`/`GeometryCollection`),
  attached to Manifest/Collection/Canvas/Range.
- **Georeference** - the Georeference extension's map-registration Annotation wrapper, plus
  polynomial/thin-plate-spline transformations and resource-coordinate pixel targets.
- **TextGranularity** - the `page`/`block`/`paragraph`/`line`/`word`/`glyph` transcription-granularity
  vocabulary, attached to an Annotation.

All three explicitly expose an idempotent `Register()` (`NavPlaceExtensions.Register()`,
`GeoreferenceExtensions.Register()`, `TextGranularityExtensions.Register()`) for callers who want a
deliberate bootstrap step; a static-constructor fallback (`Feature`'s, for the Annotation-body-dispatch
case) means most usage works without ever calling it explicitly.

**Versioning note**: each extension's `.csproj` references core via `ProjectReference`, which
`dotnet pack` converts into a `PackageReference` pinned to the exact core version built at pack
time (e.g. Georeference `1.0.1` pinned to core `3.0.1`, not a floating range) - bump an extension's
own package version alongside any core release it's packed against.

## Examples and Cookbook

- **`examples/IIIF.Manifest.Serializer.Net.Examples`** - a small hand-picked set of feature demos
  (`DemoCatalog.GetAll()`). Run with `dotnet run --project examples/IIIF.Manifest.Serializer.Net.Examples`.
- **`examples/IIIF.Manifest.Serializer.Net.Cookbook`** - a faithful C# reconstruction of every real
  recipe in [`github.com/IIIF/cookbook-recipes`](https://github.com/IIIF/cookbook-recipes) (71
  recipes, 78 catalog entries counting recipes with more than one manifest/document). Run with
  `dotnet run --project examples/IIIF.Manifest.Serializer.Net.Cookbook`.

  The Cookbook catalog is built around **Strategy + Registry**: `IRecipeSet` is the Strategy
  interface (`GetRecipes(): IEnumerable<ExampleDefinition>`), implemented by nine thematic
  recipe-set classes (`FoundationRecipes`, `CanvasAndStructureRecipes`,
  `CollectionAndChoiceRecipes`, `MediaVariationRecipes`, `LinkingAndOperaRecipes`,
  `DescriptivePropertiesRecipes`, `ProviderAndTaggingRecipes`, `AnnotationCollectionRecipes`,
  `AdvancedCompositionRecipes`), each covering one contiguous slice of recipe numbers. Shared
  construction helpers (`Id`, `NewManifest`, `NewCanvas`, `PaintingImage`, ...) live in
  `RecipeBuilders.cs`. `CookbookCatalog` itself is a thin Registry that aggregates the nine
  strategies:

  ```csharp
  public static class CookbookCatalog
  {
      private static readonly IReadOnlyList<IRecipeSet> RecipeSets =
      [
          new FoundationRecipes(), new CanvasAndStructureRecipes(), /* ... */
      ];

      public static IReadOnlyList<ExampleDefinition> GetAll() =>
          RecipeSets.SelectMany(set => set.GetRecipes()).ToList();
  }
  ```

  Every catalog entry is round-tripped through `IiifSerializer` by the test suite, so the Cookbook
  doubles as a fidelity check against real IIIF documents, not just internal consistency.

## Testing

```powershell
dotnet build IIIF.Manifest.Serializer.Net.slnx
dotnet test tests/IIIF.Manifest.Serializer.Net.Tests/IIIF.Manifest.Serializer.Net.Tests.csproj
dotnet test tests/IIIF.Manifest.Serializer.Net.ArchTests/IIIF.Manifest.Serializer.Net.ArchTests.csproj
```

393 unit tests (xUnit + AwesomeAssertions), including per-feature round-trip tests, an automatic
per-catalog-entry round-trip test for every Cookbook recipe, and the System.Text.Json interop
suite - plus 8 architecture tests (NetArchTest.Rules) enforcing namespace layering and naming
conventions. Coverage is collected via `coverlet.collector` in CI; see `coverage-report/` for the
latest generated report (no hard coverage gate is enforced yet - report visibility comes first).

## Documentation index

- [`SDK_VERSIONING_GUIDE.md`](SDK_VERSIONING_GUIDE.md) - the authoritative architecture reference:
  the 2.x↔3.0 property mapping table, the Obsolete-tagging convention, and the full milestone
  history of how the multi-version reshape, the extended standards coverage (Auth/Search/Discovery/
  Content State/Image API), the Cookbook catalog, and the structural refactor described above were
  each implemented and verified.
- Every folder under `src/IIIF.Manifest.Serializer.Net/` and `extensions/*` has its own generated
  API reference under `docs/` (types, members, attributes, Mermaid diagrams, package dependencies),
  mirroring the source tree 1:1 (`src/IIIF.Manifest.Serializer.Net/Nodes/Contents/Annotation` →
  [`docs/Nodes/Contents/Annotation`](Nodes/Contents/Annotation/README.md), etc.). These are
  regenerated from source and doc-comments, not hand-maintained - see the **Docs Catalog** below for
  the full index, or start from [`docs/Nodes/README.md`](Nodes/README.md),
  [`docs/Properties/README.md`](Properties/README.md), [`docs/Shared/README.md`](Shared/README.md),
  [`docs/Attributes/README.md`](Attributes/README.md), [`docs/Helpers/README.md`](Helpers/README.md),
  [`docs/SystemTextJson/README.md`](SystemTextJson/README.md), and
  [`docs/Extensions/README.md`](Extensions/README.md).

### Docs Catalog

Two-level index of every generated area (badges: `Types` = public/internal types documented,
`Files` = source files documented, `Diagrams` = whether a `## Diagrams` section has at least one
Mermaid block). Sorted alphabetically; children shown are each area's direct subfolders only -
follow a child's own link for its further nesting (e.g. `Nodes/Contents` has 10 more content-type
subfolders, `Properties/Services` has 4).

- [Attributes](Attributes/README.md) `Types:10` `Files:10` `Diagrams:✓`
- [Extensions](Extensions/README.md) `Types:22` `Files:23` `Diagrams:✓`
  - [NavPlace](Extensions/NavPlace/README.md) `Types:9` `Files:10` `Diagrams:✓`
  - [Georeference](Extensions/Georeference/README.md) `Types:11` `Files:11` `Diagrams:✓`
  - [TextGranularity](Extensions/TextGranularity/README.md) `Types:2` `Files:2` `Diagrams:✓`
- [Helpers](Helpers/README.md) `Types:6` `Files:6` `Diagrams:✓`
- [Nodes](Nodes/README.md) `Types:34` `Files:34` `Diagrams:✓`
  - [Contents](Nodes/Contents/README.md) `Types:0` `Files:0` `Diagrams:✓` *(pure grouping folder - 10 further content-type subfolders)*
- [Properties](Properties/README.md) `Types:65` `Files:65` `Diagrams:✓`
  - [Interfaces](Properties/Interfaces/README.md) `Types:4` `Files:4` `Diagrams:✓`
  - [MetadataProperty](Properties/MetadataProperty/README.md) `Types:1` `Files:1` `Diagrams:✗` *(single self-contained type)*
  - [Services](Properties/Services/README.md) `Types:8` `Files:8` `Diagrams:✓` *(3 further subfolders: Auth2, Discovery, Search)*
- [Shared](Shared/README.md) `Types:39` `Files:36` `Diagrams:✓`
  - [Content](Shared/Content/README.md) `Types:2` `Files:1` `Diagrams:✓`
  - [Exceptions](Shared/Exceptions/README.md) `Types:3` `Files:3` `Diagrams:✓`
  - [Selectors](Shared/Selectors/README.md) `Types:6` `Files:6` `Diagrams:✓`
  - [Service](Shared/Service/README.md) `Types:1` `Files:1` `Diagrams:✗` *(single self-contained type)*
  - [Trackable](Shared/Trackable/README.md) `Types:9` `Files:10` `Diagrams:✓`
  - [ValuableItem](Shared/ValuableItem/README.md) `Types:2` `Files:2` `Diagrams:✓`
- [SystemTextJson](SystemTextJson/README.md) `Types:4` `Files:4` `Diagrams:✓`

**Totals**: ~180 types and ~178 source files documented across 44 folders (plus the `Extensions`
navigational index), spanning the core SDK (`src/IIIF.Manifest.Serializer.Net/`) and all 3
extension packages.

**Last generated**: 2026-07-12.

### Coverage Audit

Every folder in scope (`src/IIIF.Manifest.Serializer.Net/**` and `extensions/*/**`, excluding
`bin`/`obj`) has a generated README with all required sections (Contents, Overview, Files,
Types & Members, Diagrams, Package Dependencies, See Also): ✅ all 44 folders, plus the
`Extensions` navigational index. No folder needed a retry pass or was left with placeholder
content. `examples/*` and `tests/*` were intentionally excluded from this run (examples are
demo/consumer code, not documented library API surface; tests are excluded per policy). The
top-level `docs/README.md` (this file) and `docs/SDK_VERSIONING_GUIDE.md` were preserved
unchanged by the regeneration, per an explicit decision to keep this hand-authored content rather
than have it overwritten by source-derived generation.

## Contributing

1. Read [`SDK_VERSIONING_GUIDE.md`](SDK_VERSIONING_GUIDE.md) before touching version-aware
   serialization code - it documents decisions (Obsolete scope, multi-sequence handling, services
   write strategy) that aren't obvious from the code alone.
2. New 3.0-native properties: inherit the `TrackableObject`/`BaseItem`/`BaseNode` fluent
   `SetX`/`AddX` pattern; back legacy 2.x concepts with computed, read-only views, never the other
   way around.
3. Land tests with every change - round-trip (legacy in → 3.0 out, and back), not just "does it
   parse."
4. Run the full build and test suite before opening a PR:
   ```powershell
   dotnet build IIIF.Manifest.Serializer.Net.slnx
   dotnet test tests/IIIF.Manifest.Serializer.Net.Tests/IIIF.Manifest.Serializer.Net.Tests.csproj
   dotnet test tests/IIIF.Manifest.Serializer.Net.ArchTests/IIIF.Manifest.Serializer.Net.ArchTests.csproj
   ```

## Release hygiene

- **Versioning**: the core package version lives in the root [`Directory.Build.props`](../Directory.Build.props)
  (`<Version>`); each extension's version is set independently in
  [`extensions/Directory.Build.props`](../extensions/Directory.Build.props). Bump the relevant file,
  not individual `.csproj` files - `Directory.Packages.props` under `src/`, `extensions/`,
  `examples/`, and `tests/` centralizes third-party package *versions*, not this project's own.
- **License**: the repository ships under **MIT** ([`LICENSE`](../LICENSE)). `PackageLicenseExpression`
  (root `Directory.Build.props`) and the README license badge must always agree with the `LICENSE`
  file - if one ever changes, update all three in the same commit.
- **CI** (`.github/workflows/ci.yml`): every push/PR restores, builds in Release, runs both test
  projects (`Tests` and `ArchTests`) in Release, then re-runs `Tests` in Debug with coverage
  collection (Release strips PDBs, so coverage needs a Debug build to instrument against) and
  publishes the coverage summary to the job summary plus an artifact.
- **SAST** (`.github/workflows/sast.yml`): CodeQL (`security-extended`+`security-and-quality`) runs
  on push/PR/weekly schedule. By default this is **informational only** - `fail-on-findings` is
  `false` unless explicitly set `true` on a manual `workflow_dispatch` run. A separate
  `pr-security-gate` job always hard-fails a PR if it has any *open* **High/Critical** code-scanning
  alert, regardless of the `fail-on-findings` setting - so PRs are never blocked by low/medium
  findings, only ones that actually matter. This is the explicit, decided policy; don't change the
  High/Critical threshold without updating this note.
- **Patch management** (`.github/workflows/patch-management.yml`): an OWASP Dependency-Check run,
  triggered weekly and whenever `Directory.Build.props`/`Directory.Packages.props`/`*.csproj`/
  `*.slnx`/`global.json` change. Fails the build on packages with a known CVE at CVSS ≥ 7 by default
  (`fail-on-vulnerable: true`); does **not** fail merely for being outdated
  (`fail-on-outdated: false`) - outdated-but-not-vulnerable dependencies are tracked via an
  auto-opened issue instead of blocking anything.
- **Publishing** (`.github/workflows/publish-nuget.yml`): triggers on `v*.*.*` tags, or manually via
  `workflow_dispatch` for a `prerelease` channel with an optional version suffix. Builds, runs both
  test projects, packs all 4 packages (core + 3 extensions), and pushes every `.nupkg` to nuget.org
  with `--skip-duplicate`. All workflows pin the .NET SDK via `global-json-file: global.json` -
  never hardcode a different SDK version in a workflow.
- **Package smoke test** - run this locally before tagging a release to catch packaging problems
  CI's `--no-build` steps wouldn't surface:
  ```powershell
  dotnet restore IIIF.Manifest.Serializer.Net.slnx
  dotnet build IIIF.Manifest.Serializer.Net.slnx --configuration Release --no-restore -p:GeneratePackageOnBuild=false
  dotnet test IIIF.Manifest.Serializer.Net.slnx --configuration Release --no-build
  dotnet pack src/IIIF.Manifest.Serializer.Net/IIIF.Manifest.Serializer.Net.csproj --configuration Release --no-build
  dotnet pack extensions/IIIF.Manifest.Serializer.Net.NavPlace/IIIF.Manifest.Serializer.Net.NavPlace.csproj --configuration Release --no-build
  dotnet pack extensions/IIIF.Manifest.Serializer.Net.Georeference/IIIF.Manifest.Serializer.Net.Georeference.csproj --configuration Release --no-build
  dotnet pack extensions/IIIF.Manifest.Serializer.Net.TextGranularity/IIIF.Manifest.Serializer.Net.TextGranularity.csproj --configuration Release --no-build
  ```
  Confirm every `.nupkg` contains its `README.md` and a correct `<license>`/`<projectUrl>`/
  `<repository>` before pushing a tag.

## License

See [LICENSE](../LICENSE) in the repository root.
