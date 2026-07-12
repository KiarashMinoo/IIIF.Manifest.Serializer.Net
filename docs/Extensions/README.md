# Extensions

3 independently-versioned, independently-published NuGet packages that each add **3.0-only,
spec-defined data** to any `BaseNode<T>` (the core SDK's shared descriptive-property base) via the
core SDK's "additional properties" / extension-data mechanism. None of these packages need to
modify the core SDK to work, and each ships as its own assembly with its own version number.

| Package | NuGet ID | Models | Docs |
| --- | --- | --- | --- |
| navPlace | `IIIF.Manifest.Serializer.Net.NavPlace` | GeoJSON-shaped geolocation (`Point`/`LineString`/`Polygon`/`GeometryCollection`) attachable to `Collection`/`Manifest`/`Range`/`Canvas` | [./NavPlace/README.md](./NavPlace/README.md) |
| Georeference | `IIIF.Manifest.Serializer.Net.Georeference` | The Georeference extension's map-registration Annotation (`motivation: "georeferencing"`), pixel-space `resourceCoords`, and the two allowed transformation algorithms | [./Georeference/README.md](./Georeference/README.md) |
| Text Granularity | `IIIF.Manifest.Serializer.Net.TextGranularity` | The `page`/`block`/`paragraph`/`line`/`word`/`glyph` transcription-granularity vocabulary attachable to Annotations | [./TextGranularity/README.md](./TextGranularity/README.md) |

All three share the C# root namespace `IIIF.Manifests.Serializer.Extensions` (with `.ResourceCoords`
and `.Transformations` sub-namespaces inside the Georeference package) - the namespace does **not**
distinguish which physical package/assembly a type ships in. Each package's own README states this
explicitly and names the assembly in its Overview.

Georeference depends on navPlace (`ProjectReference` to
`IIIF.Manifest.Serializer.Net.NavPlace.csproj`) because its Annotation body reuses navPlace's
`NavPlace`/`Feature` FeatureCollection shape directly. navPlace and Text Granularity have no
dependency on each other or on Georeference. All three depend on the core
`IIIF.Manifest.Serializer.Net` SDK project and on `Newtonsoft.Json`.

See [`../README.md`](../README.md) for the core SDK overview and
[`../SDK_VERSIONING_GUIDE.md`](../SDK_VERSIONING_GUIDE.md) for the milestone history behind these
three extensions (Milestones 15-17 in particular).
