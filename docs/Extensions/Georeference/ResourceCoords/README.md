# ResourceCoords

## Contents

- [Overview](#overview)
- [Files](#files)
- [Types & Members](#types--members)
- [Diagrams](#diagrams)
- [Package Dependencies](#package-dependencies)
- [See Also](#see-also)

## Overview

This is a subfolder of the `IIIF.Manifest.Serializer.Net.Georeference` NuGet package (same assembly
as [../README.md](../README.md), not a separate package). It models the Georeference extension's
`resourceCoords` property: per the spec, `resourceCoords` pins a navPlace `Feature` to specific
pixel/frame coordinates on the resource being georeferenced (as opposed to the Feature's `geometry`,
which describes the corresponding real-world/map location). The one type here,
`ResourceCoordExtensions`, is a fluent extension-method class attaching `resourceCoords` to a
`FeatureProperties` instance (from the navPlace package) via the core SDK's additional-properties
mechanism — no core SDK or navPlace package changes are required.

[↑ Back to top](#contents)

## Files

| File | Primary type(s) | LOC (approx) | Responsibility |
| --- | --- | --- | --- |
| `ResourceCoordExtensions.cs` | `ResourceCoordExtensions` | 31 | Fluent `SetResourceCoords`/`ResourceCoords` extension members attaching pixel-space coordinates to a navPlace `FeatureProperties`. |

[↑ Back to top](#contents)

## Types & Members

| Type | Kind | Summary | Inherits/Implements | Key Members |
| --- | --- | --- | --- | --- |
| `ResourceCoordExtensions` | static class | Fluent attach-point for `resourceCoords` on `FeatureProperties` | *(none)* | `SetResourceCoords(IReadOnlyCollection<double>)`, `ResourceCoords` (get) |

### ResourceCoordExtensions

- **Kind / Namespace**: static class, `IIIF.Manifests.Serializer.Extensions.ResourceCoords`
- **Constants**: `ResourceCoordsJName = "resourceCoords"`.
- **Key members**: an `extension(FeatureProperties featureProperties)` block (C# extension-member syntax) exposing:
  - `SetResourceCoords(IReadOnlyCollection<double> resourceCoords) : FeatureProperties` — calls `featureProperties.SetAdditionalProperty(ResourceCoordsJName, resourceCoords)`.
  - `ResourceCoords : IReadOnlyCollection<double>?` (get) — calls `featureProperties.GetAdditionalProperty<FeatureProperties, double[]>(ResourceCoordsJName)`; carries `[GeoreferenceExtension("3.0")]`.
- **Usage Recipe**:
  ```csharp
  using IIIF.Manifests.Serializer.Extensions;
  using IIIF.Manifests.Serializer.Extensions.ResourceCoords;

  // A ground control point: its map location (geometry) plus its pixel location on the source image.
  var gcpFeature = new Feature("https://example.org/iiif/georef/1/gcp1")
      .SetGeometry(new Geometry(GeometryType.Point).AddCoordinate(new CoordinateItem(-73.968285, 40.785091)))
      .SetProperties(new FeatureProperties().SetResourceCoords([120.5, 340.25]));

  IReadOnlyCollection<double>? pixelCoords = gcpFeature.Properties?.ResourceCoords;
  ```

[↑ Back to top](#contents)

## Diagrams

*Not applicable — single self-contained type.*

[↑ Back to top](#contents)

## Package Dependencies

| Package | Version | Description | Links |
| --- | --- | --- | --- |
| Newtonsoft.Json | 13.0.4 | JSON.NET — the core SDK's serialization engine, also used here | [NuGet](https://www.nuget.org/packages/Newtonsoft.Json/13.0.4) |
| IIIF.Manifest.Serializer.Net | (ProjectReference) | Core SDK — supplies `IAdditionalPropertiesSupport<T>`'s `SetAdditionalProperty`/`GetAdditionalProperty` mechanism and the `[GeoreferenceExtension]` attribute | [../../../README.md](../../../README.md) |
| IIIF.Manifest.Serializer.Net.NavPlace | (ProjectReference, via the parent Georeference project) | Supplies `FeatureProperties`, the type this folder's extension methods attach to | [../../NavPlace/README.md](../../NavPlace/README.md) |

[↑ Back to top](#contents)

## See Also

- [../README.md](../README.md) — the parent Georeference package (this folder is part of the same assembly)
- [../Transformations/README.md](../Transformations/README.md) — sibling subfolder: the transformation algorithms used alongside `resourceCoords`
- [../../README.md](../../README.md) — Extensions index (all 3 extension packages)
- [../../../README.md](../../../README.md) — docs root / core SDK overview
- [../../../SDK_VERSIONING_GUIDE.md](../../../SDK_VERSIONING_GUIDE.md) — see [Milestone 17: model Georeference Annotation wrapper](../../../SDK_VERSIONING_GUIDE.md#milestone-17-done--model-georeference-annotation-wrapper)

[↑ Back to top](#contents)
