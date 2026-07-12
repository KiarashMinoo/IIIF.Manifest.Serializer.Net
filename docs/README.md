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
tests/IIIF.Manifest.Serializer.Net.Tests        xUnit + FluentAssertions test suite
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

## Extension packages

Each extension is a standalone NuGet package that adds 3.0-only, spec-defined data to any
`BaseNode` via the SDK's additional-properties mechanism - no core SDK changes needed to consume
them:

- **NavPlace** - GeoJSON `navPlace` geolocation (`Point`/`LineString`/`Polygon`/`GeometryCollection`).
- **Georeference** - the Georeference extension's map-registration Annotation wrapper.
- **TextGranularity** - the `page`/`block`/`paragraph`/`line`/`word`/`glyph` transcription-granularity vocabulary.

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
dotnet build IIIF.Manifest.Serializer.Net.sln
dotnet test tests/IIIF.Manifest.Serializer.Net.Tests/IIIF.Manifest.Serializer.Net.Tests.csproj
```

336 tests (xUnit + FluentAssertions), including per-feature round-trip tests, an automatic
per-catalog-entry round-trip test for every Cookbook recipe, and the System.Text.Json interop
suite. Coverage is collected via `coverlet.collector` in CI; see `coverage-report/` for the latest
generated report (no hard coverage gate is enforced yet - report visibility comes first).

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
   dotnet build IIIF.Manifest.Serializer.Net.sln
   dotnet test tests/IIIF.Manifest.Serializer.Net.Tests/IIIF.Manifest.Serializer.Net.Tests.csproj
   ```

## License

See [LICENSE](../LICENSE) in the repository root.
