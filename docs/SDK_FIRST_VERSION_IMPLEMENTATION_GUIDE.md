# SDK First Version Implementation Guide

This guide defines the target architecture and staged implementation plan for the first stable SDK version of `IIIF.Manifest.Serializer.Net`.

The project goal is a C#/.NET SDK that can parse legacy IIIF documents, expose a modern object model for application code, and serialize back to any supported IIIF version selected by the caller. The latest stable Presentation API model is the primary write model. Legacy properties must remain readable for compatibility but must not be the preferred mutation surface.

## 1. Scope

### Primary SDK responsibilities

- Read IIIF documents from legacy and current formats.
- Detect the document family and version from `@context`, `type`/`@type`, and structural cues.
- Normalize parsed documents into one canonical in-memory model.
- Serialize the canonical model into a caller-selected output version.
- Preserve extension and unknown properties where possible.
- Keep obsolete legacy members read-only or computed from modern state.
- Provide cookbook/demo examples as executable examples and regression tests.
- Provide high code coverage for version detection, conversion, serialization, extension handling, and examples.

### Version targets

| Area | Read support | Write support | Notes |
| --- | --- | --- | --- |
| Metadata API 1.0 | Compatibility importer | Optional legacy writer only if needed | Replaced by Presentation API; should not be a preferred authoring model. |
| Presentation API 2.0 | Yes | Yes | Legacy output shape. |
| Presentation API 2.1 / 2.1.1 | Yes | Yes | Main legacy output shape. |
| Presentation API 3.0 | Yes | Yes; default | Current stable authoring/output model. |
| Presentation API 4.0 RC | Research/compatibility tracking | No default writer yet | Treat as future-readiness until stable. |
| Image API 1.x/2.x/3.0 service descriptors | Yes | Yes where represented | Used by Presentation resources and standalone `info.json`. |
| Auth 1.0 / Authorization Flow 2.0 | Yes | Yes where represented | Service and response payload models. |
| Content Search 1.0 / 2.0 | Yes | Yes where represented | Search/autocomplete services and responses. |
| Change Discovery 1.0 | Yes | Yes where represented | Ordered collections/pages and activity payloads. |
| Content State 1.0 | Yes | Yes | Include `iiif-content` encode/decode helpers. |
| Approved Presentation extensions | Yes | Yes | navPlace, Text Granularity, Georeference. |

## 2. Current implementation snapshot

The current repository already has a good foundation:

- Core package and three extension packages.
- Version-aware serializer facade.
- Compatibility views for several Presentation 2.x properties.
- System.Text.Json bridge for key root resources.
- Cookbook and demo example projects.
- CI, SAST, patch-management, NuGet publishing workflows.

Known cleanup items discovered during the initial scan:

- Align license metadata and the repository `LICENSE` file.
- Ensure all test projects, including architecture tests, run in CI and publish workflows.
- Align workflow SDK versions with `global.json`.
- Make README version/framework statements consistent with project files.
- Decide whether extension type registration should be explicit rather than relying only on static constructors.

## 3. Canonical model principle

The SDK should use a latest-stable canonical model internally. For now, that means Presentation API 3.0 for Presentation resources.

Rules:

1. Public authoring APIs should prefer modern names and shapes.
2. Legacy properties should be computed compatibility views.
3. Legacy mutation APIs should be obsolete and compile-time blocked when a modern replacement exists.
4. Unknown fields and extension fields must round-trip through extension-data storage.
5. Parsing must be tolerant; writing must be deliberate and version-specific.
6. Validation must be separate from parsing so legacy or partially invalid documents can still be loaded for migration.

## 4. Version strategy

### Detection

Create or extend version detection around these signals:

- JSON-LD context URI.
- `id`/`type` versus `@id`/`@type`.
- Presentation 2.x structures: `sequences`, `canvases`, `images`, `otherContent`, `structures`.
- Presentation 3.0 structures: `items`, `annotations`, `partOf`, `requiredStatement`, `rights`, `provider`.
- Metadata API 1.0 structures: manifest/sequence/canvas/layer concepts and old context.
- Service context/profile combinations for Image/Auth/Search.

### Options

Use one options object for all serialization paths:

```csharp
public sealed record IiifSerializerOptions(
    IiifPresentationVersion Version = IiifPresentationVersion.V3_0,
    IiifLegacyMode LegacyMode = IiifLegacyMode.ReadOnlyCompatibility,
    IiifUnknownPropertyHandling UnknownPropertyHandling = IiifUnknownPropertyHandling.Preserve,
    bool Indented = true);
```

Potential future options:

- `ValidateBeforeWrite`
- `StrictVersionCompliance`
- `EmitContext`
- `IncludeEmptyCollections`
- `PreserveInputOrdering`

## 5. Legacy read-only property model

Recommended pattern:

```csharp
[PresentationAPI("2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "requiredStatement")]
[JsonProperty("attribution")]
public IReadOnlyCollection<Attribution> Attribution =>
    RequiredStatement?.Value.Select(x => new Attribution(x.Value)).ToList() ?? [];

[PresentationAPI("3.0")]
[JsonIgnore]
public RequiredStatement? RequiredStatement { get; private set; }

public T SetRequiredStatement(RequiredStatement requiredStatement)
{
    RequiredStatement = requiredStatement;
    return (T)this;
}
```

Read-only legacy members should be hydrated by legacy readers and then projected into modern storage. They should not be the primary storage unless there is no modern equivalent.

## 6. Converter/importer/exporter architecture

Recommended long-term shape:

```text
Json Document
   ↓
IiifVersionDetector
   ↓
Version-specific reader/importer
   ↓
Canonical model
   ↓
Version-specific writer/exporter
   ↓
Json Document
```

Suggested interfaces:

```csharp
public interface IIiifReader<out T>
{
    T Read(JObject document, IiifReadContext context);
}

public interface IIiifWriter<in T>
{
    JObject Write(T resource, IiifWriteContext context);
}

public interface IIiifVersionConverter<T>
{
    T Normalize(T resource, IiifPresentationVersion sourceVersion);
}
```

This keeps the public facade simple while making version behavior testable in isolation.

## 7. API and extension coverage matrix

Implement and test the following coverage areas:

| Area | Required SDK surface |
| --- | --- |
| Presentation | Collection, Manifest, Canvas, Range, AnnotationPage, Annotation, AnnotationCollection, Content resources, Choice, SpecificResource, selectors. |
| Descriptive properties | label, metadata, summary/description, requiredStatement/attribution, rights/license, provider, thumbnail, homepage/related, seeAlso, rendering, services. |
| Technical properties | id/@id, type/@type, format, height, width, duration, behavior/viewingHint, viewingDirection, timeMode. |
| Structural properties | items, structures, annotations, partOf/within, start, placeholderCanvas, accompanyingCanvas. |
| Image API | service descriptors, profile, tiles, sizes, preferred/extra formats, qualities, features, standalone info.json. |
| Auth | Auth 1.0 service shape, Authorization Flow 2.0 probe/access/token/logout services and response payloads. |
| Search | Search/autocomplete services and Content Search 1.0/2.0 responses. |
| Change Discovery | OrderedCollection, OrderedCollectionPage, Activity, actor, target, dataset, rights, paging. |
| Content State | Content-state annotation model and base64url `iiif-content` codec. |
| Extensions | navPlace, Text Granularity, Georeference. |

## 8. Cookbook and demos plan

The cookbook and demos should be treated as executable conformance examples.

Implementation structure:

```text
examples/
  IIIF.Manifest.Serializer.Net.Cookbook/
    Recipes/
      Recipe0001_SimplestManifestImage.cs
      Recipe0002_SimplestManifestAudio.cs
      ...
  IIIF.Manifest.Serializer.Net.Demos/
    WellcomeCollectionDemo.cs
    StanfordNewspaperDemo.cs
    BiblissimaReunificationDemo.cs
```

Testing structure:

```text
tests/
  IIIF.Manifest.Serializer.Net.Tests/
    CookbookRoundTripTests.cs
    CookbookJsonShapeTests.cs
    DemoCatalogTests.cs
    LegacyImportTests.cs
    VersionedWriteTests.cs
```

Each recipe should have at least:

- Builder test: recipe can construct a valid object.
- V3 JSON-shape test.
- Legacy V2.1 write test when the pattern has a legacy equivalent.
- Round-trip test.
- Unknown/extension property preservation test where relevant.

## 9. Test and coverage strategy

Target maximum practical coverage, not artificial line coverage.

Required test categories:

- Version detection tests.
- Reader tests for real legacy samples.
- Writer tests for each selected output version.
- Compatibility-property tests.
- Obsolete mutation API tests.
- JSON shape tests using real expected fragments.
- Round-trip tests for all examples.
- Extension round-trip tests.
- Error and invalid-input tests.
- System.Text.Json bridge tests.
- Architecture tests for layering and dependency direction.

CI should run:

```bash
dotnet restore IIIF.Manifest.Serializer.Net.slnx
dotnet build IIIF.Manifest.Serializer.Net.slnx --configuration Release --no-restore -p:GeneratePackageOnBuild=false
dotnet test IIIF.Manifest.Serializer.Net.slnx --configuration Release --no-build
dotnet test tests/IIIF.Manifest.Serializer.Net.Tests/IIIF.Manifest.Serializer.Net.Tests.csproj --configuration Debug --collect:"XPlat Code Coverage" --settings tests/IIIF.Manifest.Serializer.Net.Tests/coverlet.runsettings
```

Coverage should include core and extension packages, not demo/test harness assemblies.

## 10. Implementation phases

### Phase 0 — Governance and safety

- Fix license mismatch.
- Fix README version/framework inconsistencies.
- Ensure all tests run in CI.
- Align workflow SDK versions with `global.json`.
- Add this implementation guide.

### Phase 1 — Version detector and options hardening

- Expand `IiifPresentationVersion` with explicit legacy values if missing.
- Add Metadata API 1.0 detection.
- Add tests for contexts and structural cues.
- Add serializer options tests.

### Phase 2 — Legacy import normalization

- Add Metadata API 1.0 importer if needed.
- Harden Presentation 2.0/2.1 importers.
- Ensure all legacy data lands in modern canonical storage.
- Make obsolete properties read-only/computed.

### Phase 3 — Versioned writers

- Verify 2.0, 2.1, and 3.0 writers against spec/cookbook fixtures.
- Ensure unsupported modern fields are downgraded deterministically or omitted with documented behavior.
- Add writer behavior tests for every compatibility property.

### Phase 4 — Services and auxiliary APIs

- Complete Image API descriptors and standalone `info.json` behavior.
- Complete Auth 1.0/Auth 2.0 models.
- Complete Search 1.0/Search 2.0 models.
- Complete Change Discovery and Content State coverage.

### Phase 5 — Extensions

- Confirm navPlace, Text Granularity, and Georeference against current extension specs.
- Replace brittle static registration with explicit registration or module initialization if required.
- Add extension round-trip tests.

### Phase 6 — Cookbook and demos

- Inventory every cookbook recipe.
- Mark each as implemented, not applicable, pending, or blocked by missing model support.
- Implement missing recipes as executable examples.
- Add demos for official demo patterns where direct manifest examples are available.

### Phase 7 — Validation and release hardening

- Add optional validation layer.
- Add package README validation.
- Add NuGet packaging checks.
- Add public API compatibility checks.
- Publish prerelease package and verify consumption from a clean sample app.

## 11. Definition of done for first stable SDK

- Parses Presentation 2.0/2.1/3.0 and selected Metadata API 1.0 legacy samples.
- Writes Presentation 2.0/2.1/3.0 from the canonical model.
- Uses Presentation 3.0 as the default writer.
- Obsolete properties are read-only/computed where modern replacements exist.
- Unknown and extension properties round-trip.
- All cookbook examples that map to supported APIs are implemented or explicitly inventoried with reasons.
- CI runs all test projects.
- Coverage report is generated from core and extension packages.
- README, docs, package metadata, and license metadata are consistent.
