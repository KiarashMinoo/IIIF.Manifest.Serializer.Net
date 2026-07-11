# IIIF.Manifest.Serializer.Net

Version-aware .NET models and serializers for IIIF Presentation API resources, with support for legacy 2.x JSON and modern Presentation API 3.0 output.

The core package targets `netstandard2.1`, uses `Newtonsoft.Json`, and exposes a fluent object model for building, reading, converting, and serializing IIIF manifests, collections, canvases, annotation pages, annotations, content resources, image services, and related IIIF service payloads.

## Status

- Core package version: `3.0.0` from `Directory.Build.props`.
- Core target framework: `netstandard2.1`.
- Extension target frameworks: `netstandard2.1;net8.0`.
- Test project target framework: `net8.0`.
- Checked-in coverage summary: about 72% line coverage.
- Root README status: current project overview. Older generated docs under `docs/` may lag behind the current 3.0-first model.

## What It Supports

- IIIF Presentation API 2.0, 2.1, and 3.0 version-aware read/write paths.
- `Manifest`, `Collection`, `Canvas`, `Range`/`Structure`, `AnnotationPage`, `Annotation`, and standalone `AnnotationCollection`.
- Computed compatibility views for legacy 2.x shapes such as `sequences`, `images`, `otherContent`, `license`, `attribution`, `within`, and `description`.
- 3.0-native concepts such as `items`, `behavior`, `rights`, `requiredStatement`, `partOf`, `summary`, `provider`, `homepage`, `thumbnail`, `rendering`, `seeAlso`, `placeholderCanvas`, and `start`.
- W3C-style annotation bodies and targets, including `TextualBody`, `Choice`, multiple bodies, multiple targets, `SpecificResource`, and selectors.
- IIIF Image API service descriptors, including tiles, sizes, preferred/extra formats, extra qualities/features, and standalone `info.json` serialization.
- IIIF Auth 1.0 and Auth 2.0 service models, including Auth 2.0 probe/access/token/logout services and response payloads.
- IIIF Content Search 2.0 service and response models.
- IIIF Change Discovery 1.0 ordered collection/page and activity models.
- IIIF Content State 1.0 objects and `iiif-content` base64url encode/decode helpers.
- Extension packages for navPlace, Georeference, and Text Granularity.
- A cookbook example project with faithful C# reconstructions of 71 real IIIF Cookbook recipes.

## Standards Coverage

| Area | Current coverage |
| --- | --- |
| Presentation API | 2.0, 2.1, and 3.0 manifests and collections, plus 3.0 annotation collections. |
| W3C Annotation model | Annotation pages, annotation collections, multiple bodies, multiple targets, textual bodies, choices, specific resources, selectors, stylesheet, and `timeMode`. |
| Image API | Embedded service descriptors, tiles, sizes, dimensions, preferred/extra formats, qualities, features, protocol, and standalone `info.json`. |
| Auth API | Auth 1.0 service shape and Auth 2.0 probe/access/token/logout services plus response payloads. |
| Content Search API | Search/autocomplete services and response/result models. |
| Change Discovery API | Ordered collections, ordered collection pages, activities, actors, targets, datasets, rights, and paging. |
| Content State API | Content-state annotation objects, targets, point selectors, and `iiif-content` codec helpers. |
| Extensions | navPlace, Georeference, and Text Granularity packages. |

## Projects

| Path | Purpose |
| --- | --- |
| `src/IIIF.Manifest.Serializer.Net` | Core library and version-aware serializer. |
| `extensions/IIIF.Manifest.Serializer.Net.NavPlace` | navPlace / GeoJSON-LD extension types. |
| `extensions/IIIF.Manifest.Serializer.Net.Georeference` | Georeference annotation, target, selector, and transformation types. |
| `extensions/IIIF.Manifest.Serializer.Net.TextGranularity` | Text Granularity extension values and helpers. |
| `examples/IIIF.Manifest.Serializer.Net.Cookbook` | IIIF Cookbook recipe builders. |
| `examples/IIIF.Manifest.Serializer.Net.Examples` | Smaller demo catalog. |
| `tests/IIIF.Manifest.Serializer.Net.Tests` | xUnit coverage for serializers, compatibility views, services, extensions, and examples. |
| `docs/SDK_VERSIONING_GUIDE.md` | Detailed implementation and design history for the multi-version model. |

## Architecture

`IiifSerializer` is a partial facade. The main `IiifSerializer.cs` file keeps the public `Serialize`/`Deserialize` overloads and version dispatch, while resource-specific read/write logic lives in focused partial files:

- `IiifSerializer.Manifest.cs`
- `IiifSerializer.Collection.cs`
- `IiifSerializer.Canvas.cs`
- `IiifSerializer.Range.cs`
- `IiifSerializer.Annotation.cs`
- `IiifSerializer.AnnotationCollection.cs`
- `IiifSerializer.NodeExtras.cs`
- `IiifSerializer.Metadata.cs`
- `IiifSerializer.Provider.cs`
- `IiifSerializer.Service.cs`
- `IiifSerializer.ImageLikeResources.cs`
- `IiifSerializer.LinkResources.cs`
- `IiifSerializer.Helpers.cs`

The model uses fluent mutation methods backed by `TrackableObject<T>`. Additional/extension properties are bridged through `JsonExtensionData`, allowing extension payloads such as navPlace, Georeference, and Text Granularity data to survive JSON round trips.

## Quick Start

```csharp
using IIIF.Manifests.Serializer;
using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Nodes.Contents.Annotation;
using IIIF.Manifests.Serializer.Nodes.Contents.Image.Resource;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.Services;

var manifest = new Manifest(
    "https://example.org/iiif/book/manifest",
    new Label("Example Manifest"));

var canvas = new Canvas(
    "https://example.org/iiif/book/canvas/p1",
    new Label("Page 1"),
    height: 1800,
    width: 1200);

var image = new ImageResource(
        "https://example.org/iiif/book/page1/full/max/0/default.jpg",
        "image/jpeg")
    .SetHeight(1800)
    .SetWidth(1200);

image.AddService(new Service(
    "http://iiif.io/api/image/3/context.json",
    "https://example.org/iiif/book/page1",
    "level1"));

canvas.AddAnnotation(new Annotation(
    "https://example.org/iiif/book/annotation/p1-image",
    image,
    canvas.Id));

manifest
    .AddItem(canvas)
    .SetRights(Rights.CcBy)
    .SetRequiredStatement(new RequiredStatement(
        new Label("Attribution"),
        new Description("Provided by Example Library")));

var v3Json = IiifSerializer.Serialize(manifest);

var v2Json = IiifSerializer.Serialize(
    manifest,
    new IiifSerializerOptions(IiifPresentationVersion.V2_1));

var parsed = IiifSerializer.DeserializeManifest(v3Json);
```

`IiifSerializerOptions.Default` writes Presentation API 3.0. Use `IiifPresentationVersion.V2_0` or `IiifPresentationVersion.V2_1` when a legacy output shape is required.

## Version-Aware Model

The library stores most resources in a 3.0-shaped model and exposes legacy 2.x properties as compatibility views. For example:

- `Manifest.Items` is the preferred canvas ordering; `Manifest.Sequences` is a computed legacy view.
- `Canvas.Items` holds `AnnotationPage`/`Annotation`; `Canvas.Images`, `Audios`, and `Videos` are computed legacy views.
- `Rights`, `RequiredStatement`, `PartOf`, and `Summary` are preferred; `License`, `Attribution`, `Within`, and `Description` remain readable legacy views.
- Legacy mutation APIs are marked obsolete with compile-time errors where there is a 3.0 replacement.

This lets callers read old manifests, work with the current model, and write either 2.x or 3.0 JSON.

## Serializer Entry Points

```csharp
IiifSerializer.Serialize(manifest);
IiifSerializer.Serialize(manifest, new IiifSerializerOptions(IiifPresentationVersion.V2_1));
IiifSerializer.DeserializeManifest(json);

IiifSerializer.Serialize(collection);
IiifSerializer.DeserializeCollection(json);

IiifSerializer.Serialize(annotationCollection);
IiifSerializer.DeserializeAnnotationCollection(json);
```

Version detection is handled by `IiifPresentationVersionDetector` using context, type, and structural cues.

## Annotation and Resource Modeling

The current annotation model covers the common Presentation 3.0/W3C Annotation shapes:

- `Annotation.Body` for a single body and `Annotation.Bodies` for sibling bodies.
- `Annotation.Target` for a single target and `Annotation.Targets` for multi-target annotations.
- `AnnotationTarget` for bare URI targets, typed resource references, and SpecificResource selectors.
- `TextualBody` for inline text, `Choice` for mutually exclusive alternatives, and `SpecificResource` for selected or styled sources.
- Selectors: `FragmentSelector`, `PointSelector`, `ImageApiSelector`, and `SvgSelector`.
- `BaseResourceJsonConverter` for polymorphic body dispatch across image, audio, video, textual, embedded, choice, specific-resource, and registered extension resource types.

## Extensions

The extension projects add spec-specific types and fluent helpers:

- navPlace: `NavPlace`, `Feature`, `Geometry`, geometry collections, coordinates, and `SetNavPlace`.
- Georeference: georeferencing annotations, SVG selectors, targets, resource coordinates, and polynomial/thin-plate-spline transformations.
- Text Granularity: `page`, `block`, `paragraph`, `line`, `word`, and `glyph`.

All three extension projects target `netstandard2.1;net8.0` and reference the core package.

## Cookbook Examples

The cookbook project is organized into recipe-set classes:

- `FoundationRecipes`
- `CanvasAndStructureRecipes`
- `CollectionAndChoiceRecipes`
- `MediaVariationRecipes`
- `LinkingAndOperaRecipes`
- `DescriptivePropertiesRecipes`
- `ProviderAndTaggingRecipes`
- `AnnotationCollectionRecipes`
- `AdvancedCompositionRecipes`

`CookbookCatalog.GetAll()` returns the complete catalog as `ExampleDefinition` entries. Tests round-trip manifest and collection examples through both 2.1 and 3.0 serializer paths.

The catalog excludes cookbook folders that do not contain manifest JSON of their own. The implemented recipe sets are intended to exercise the serializer surface, not just provide snippets.

## Demo Catalog

`examples/IIIF.Manifest.Serializer.Net.Examples` contains a smaller `DemoCatalog` with examples for:

- Search and access services.
- Deep zoom image services.
- Paged books.
- Collection browsing.
- Legacy annotation list serialization.
- Map/navPlace data.

## Build and Test

The repository uses the .NET SDK configured by `global.json`:

```powershell
dotnet restore IIIF.Manifest.Serializer.Net.sln
dotnet build IIIF.Manifest.Serializer.Net.sln
dotnet test tests/IIIF.Manifest.Serializer.Net.Tests/IIIF.Manifest.Serializer.Net.Tests.csproj
```

Run the cookbook examples:

```powershell
dotnet run --project examples/IIIF.Manifest.Serializer.Net.Cookbook/IIIF.Manifest.Serializer.Net.Cookbook.csproj
```

Collect coverage:

```powershell
dotnet test tests/IIIF.Manifest.Serializer.Net.Tests/IIIF.Manifest.Serializer.Net.Tests.csproj `
  --collect:"XPlat Code Coverage" `
  --settings tests/IIIF.Manifest.Serializer.Net.Tests/coverlet.runsettings `
  --results-directory ./coverage-raw
```

The checked-in coverage summary currently reports line coverage around 72%.

## Packaging

The core library version is defined in `Directory.Build.props` and is currently `3.0.0`. The core package metadata describes the package as:

> Version-aware IIIF Presentation API manifest serializer using Newtonsoft.Json.

Extension projects have their own package IDs:

- `IIIF.Manifest.Serializer.Net.NavPlace`
- `IIIF.Manifest.Serializer.Net.Georeference`
- `IIIF.Manifest.Serializer.Net.TextGranularity`

## Documentation Notes

`docs/SDK_VERSIONING_GUIDE.md` is the authoritative design record for the multi-version reshape and standards coverage work. `docs/README.md` is an older generated documentation index and may not reflect the current 3.0-first model as accurately as this root README and the versioning guide.

When changing serializer behavior, update tests and prefer checking the real JSON shape through `IiifSerializer` rather than relying on direct `JsonConvert` output. Direct `JsonConvert` is still used for legacy compatibility surfaces and for specific standalone payloads, but the version-aware serializer is the intended public entry point for manifests, collections, and annotation collections.

## License

See [LICENSE](LICENSE).
