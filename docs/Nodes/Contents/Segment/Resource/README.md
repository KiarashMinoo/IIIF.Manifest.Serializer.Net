# Resource

## Contents

- [Overview](#overview)
- [Files](#files)
- [Types & Members](#types--members)
- [Diagrams](#diagrams)
- [Package Dependencies](#package-dependencies)
- [See Also](#see-also)

## Overview

This folder models the resource half of the legacy `Segment` annotation wrapper (see
[`../README.md`](../README.md)) - a resource whose type is supplied by the caller (unlike
`ImageResource`/`AudioResource`/`VideoResource`, which hardcode their `ResourceType`) and which
wraps a `Full` sibling resource representing the un-cropped/un-trimmed original.

## Files

| File | Primary type(s) | LOC (approx) | Responsibility |
| --- | --- | --- | --- |
| `SegmentResource.cs` | `SegmentResource` | 23 | A typed resource wrapping a `Full` (uncropped) sibling resource. |

## Types & Members

| Type | Kind | Summary | Inherits/Implements | Key Members |
| --- | --- | --- | --- | --- |
| `SegmentResource` | class (primary constructor) | Segment body resource | `BaseResource<SegmentResource>` | `Full`, `SetFull` |

### SegmentResource

- **Kind / Namespace**: `class`, `IIIF.Manifests.Serializer.Nodes.Contents.Segment.Resource`. Declared with a primary constructor:
  ```csharp
  public class SegmentResource(string id, ResourceType type) : BaseResource<SegmentResource>(id, type)
  ```
- **Inherits**: `BaseResource<SegmentResource>`.
- **Key properties**: `Full : BaseResource?` (`full`) - the un-cropped/un-trimmed original resource this segment is a part of.
- **Key methods**: `SetFull(BaseResource)` - fluent.
- **Constructors**: `SegmentResource(string id, ResourceType type)` - unlike the sibling `ImageResource`/`AudioResource`/`VideoResource` types, `type` is caller-supplied rather than hardcoded, since a segment can wrap any resource kind.
- **Usage Recipe**:
  ```csharp
  var segmentResource = new SegmentResource("https://example.org/segment/1", ResourceType.Image)
      .SetFull(new ImageResource("https://example.org/full/full/0/default.jpg", "image/jpeg"));
  var segment = new Segment("https://example.org/anno/segment1", segmentResource, canvas.Id);
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

- [`../README.md`](../README.md) - parent `Segment` folder (the `Segment` wrapper carrying this resource).
- [`../Selector/README.md`](../Selector/README.md) - sibling `Selector` folder used alongside this resource.
- [`../../README.md`](../../README.md) - grandparent `Contents` grouping folder.
- [`../../../README.md`](../../../README.md) - `Nodes` folder.
- [`../../../../README.md`](../../../../README.md) - repository/docs top-level documentation.

[↑ Back to top](#contents)
