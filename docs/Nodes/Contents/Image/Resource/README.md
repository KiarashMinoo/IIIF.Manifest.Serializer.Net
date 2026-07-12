# Resource

## Contents

- [Overview](#overview)
- [Files](#files)
- [Types & Members](#types--members)
- [Diagrams](#diagrams)
- [Package Dependencies](#package-dependencies)
- [See Also](#see-also)

## Overview

This folder models the IIIF Image content resource used both as the legacy `Image` wrapper's body
(see [`../README.md`](../README.md)) and as an `Annotation`'s body in 3.0-native code. It carries
`Height`/`Width` (via `IDimensionSupport<ImageResource>`) alongside the format/label/service surface
shared by every resource type - typically paired with an embedded IIIF Image API `Service` for
dynamic image delivery (level0/1/2 profiles).

## Files

| File | Primary type(s) | LOC (approx) | Responsibility |
| --- | --- | --- | --- |
| `ImageResource.cs` | `ImageResource` | 46 | The `image`-type IIIF content resource; adds `height`/`width` to the shared resource surface. |

## Types & Members

| Type | Kind | Summary | Inherits/Implements | Key Members |
| --- | --- | --- | --- | --- |
| `ImageResource` | class | Image content resource | `BaseResource<ImageResource>`, `IDimensionSupport<ImageResource>` | `Height`, `Width`, `SetHeight`, `SetWidth` |

### ImageResource

- **Kind / Namespace**: `class`, `IIIF.Manifests.Serializer.Nodes.Contents.Image.Resource`.
- **Inherits/Implements**: `BaseResource<ImageResource>`, `IDimensionSupport<ImageResource>`.
- **Key properties**: `Height : int?` (`height`), `Width : int?` (`width`).
- **Key methods**: `SetHeight(int)`, `SetWidth(int)` - fluent.
- **Constructors**: `[JsonConstructor] private ImageResource(string id, string format)` - sets `Type = ResourceType.Image` and calls `SetFormat(format)`; public convenience `ImageResource(string id, ImageFormat format)` delegates to it via `format.Value`.
- **Usage Recipe**:
  ```csharp
  var imageResource = new ImageResource("https://example.org/full/full/0/default.jpg", ImageFormat.Jpeg)
      .SetHeight(2000).SetWidth(1500)
      .AddService(new Service("http://iiif.io/api/image/3/context.json", "https://example.org/iiif/manuscript", "level2"));
  var annotation = new Annotation("https://example.org/anno/p1", imageResource, canvas.Id);
  canvas.AddAnnotation(annotation);
  ```

[↑ Back to top](#contents)

## Diagrams

*Not applicable - single self-contained type.*

## Package Dependencies

| Package | Version | Description | Links |
| --- | --- | --- | --- |
| Newtonsoft.Json | 13.0.4 | JSON.NET - this SDK's serialization engine (custom JsonConverters, attribute-driven read/write) | [NuGet](https://www.nuget.org/packages/Newtonsoft.Json/13.0.4) |

[↑ Back to top](#contents)

## See Also

- [`../README.md`](../README.md) - parent `Image` folder (the legacy `Image` wrapper carrying this resource).
- [`../../README.md`](../../README.md) - grandparent `Contents` grouping folder.
- [`../../../README.md`](../../../README.md) - `Nodes` folder (`Canvas.AddAnnotation`).
- [`../../../../README.md`](../../../../README.md) - repository/docs top-level documentation.

[↑ Back to top](#contents)
