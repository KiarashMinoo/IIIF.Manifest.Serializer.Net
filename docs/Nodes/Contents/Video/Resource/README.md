# Resource

## Contents

- [Overview](#overview)
- [Files](#files)
- [Types & Members](#types--members)
- [Diagrams](#diagrams)
- [Package Dependencies](#package-dependencies)
- [See Also](#see-also)

## Overview

This folder models the IIIF Video content resource used both as the legacy `Video` wrapper's body
(see [`../README.md`](../README.md)) and as an `Annotation`'s body in 3.0-native code. It carries
`Height`/`Width` (via `IDimensionSupport<VideoResource>`) and `Duration` alongside the
format/label/service surface shared by every resource type.

## Files

| File | Primary type(s) | LOC (approx) | Responsibility |
| --- | --- | --- | --- |
| `VideoResource.cs` | `VideoResource` | 59 | The `video`-type IIIF content resource; adds `height`/`width`/`duration` to the shared resource surface. |

## Types & Members

| Type | Kind | Summary | Inherits/Implements | Key Members |
| --- | --- | --- | --- | --- |
| `VideoResource` | class | Video content resource | `BaseResource<VideoResource>`, `IDimensionSupport<VideoResource>` | `Height`, `Width`, `Duration`, `SetHeight`, `SetWidth`, `SetDuration` |

### VideoResource

- **Kind / Namespace**: `class`, `IIIF.Manifests.Serializer.Nodes.Contents.Video.Resource`.
- **Inherits/Implements**: `BaseResource<VideoResource>`, `IDimensionSupport<VideoResource>`.
- **Key properties**: `Height : int?` (`height`), `Width : int?` (`width`), `Duration : double?` (`duration`) - length in seconds.
- **Key methods**: `SetHeight(int)`, `SetWidth(int)`, `SetDuration(double)` - all fluent.
- **Constructors**: `VideoResource(string id, string format)` - sets `Type = ResourceType.Video` and calls `SetFormat(format)`.
- **Usage Recipe** (cookbook-style A/V painting):
  ```csharp
  var videoResource = new VideoResource("https://example.org/video/reel1.mp4", "video/mp4")
      .SetHeight(1080).SetWidth(1920).SetDuration(3600.0);
  var annotation = new Annotation("https://example.org/anno/video1", videoResource, canvas.Id);
  canvas.AddAnnotation(annotation);
  canvas.SetDuration(3600.0);
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

- [`../README.md`](../README.md) - parent `Video` folder (the legacy `Video` wrapper carrying this resource).
- [`../../README.md`](../../README.md) - grandparent `Contents` grouping folder.
- [`../../../README.md`](../../../README.md) - `Nodes` folder (`Canvas.AddAnnotation`, `Canvas.SetDuration`).
- [`../../../../README.md`](../../../../README.md) - repository/docs top-level documentation.

[↑ Back to top](#contents)
