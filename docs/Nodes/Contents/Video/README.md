# Video

## Contents

- [Overview](#overview)
- [Files](#files)
- [Types & Members](#types--members)
- [Diagrams](#diagrams)
- [Package Dependencies](#package-dependencies)
- [See Also](#see-also)

## Overview

This folder models the legacy (2.x) `oa:Annotation` wrapper used to paint a video resource onto a
`Canvas` - the pre-3.0 shape that `Canvas.Videos` (in `../../README.md`) presents as a computed view
over the 3.0-native `Items`/`Annotation` storage. It pairs with [`Resource`](Resource/README.md) for
the actual `VideoResource` body. In 3.0-native code, prefer constructing an `Annotation` whose body
is a `VideoResource` directly and calling `Canvas.AddAnnotation`, rather than constructing a `Video`
wrapper by hand.

## Files

| File | Primary type(s) | LOC (approx) | Responsibility |
| --- | --- | --- | --- |
| `Video.cs` | `Video` | 31 | Legacy 2.x `oa:Annotation` wrapper pairing a `VideoResource` with a target canvas (`on`). |

## Types & Members

| Type | Kind | Summary | Inherits/Implements | Key Members |
| --- | --- | --- | --- | --- |
| `Video` | class | Legacy video-painting annotation wrapper | `BaseContent<Video, VideoResource>` | `Motivation`, `On` |

### Video

- **Kind / Namespace**: `class`, `IIIF.Manifests.Serializer.Nodes.Contents.Video`.
- **Inherits**: `BaseContent<Video, VideoResource>`.
- **Key properties**:
  - `Motivation : string` (`motivation`) - defaults to `"sc:painting"` (also explicitly re-set to the same value in the constructor).
  - `On : string` (`on`) - the id of the `Canvas` (or region) this video is painted onto.
  - Inherited: `Resource : VideoResource` (`resource`), plus `Format`/`Height`/`Width` from `BaseContent<Video>`.
- **Constructors**: `Video(string id, VideoResource resource, string on)`.
- **Usage Recipe** (legacy-shim construction; for new 3.0-native code build an `Annotation` with a `VideoResource` body via `Canvas.AddAnnotation` instead - see `Canvas.ToVideo`/`ToAnnotation` internal mapping in `../../README.md`):
  ```csharp
  var videoResource = new VideoResource("https://example.org/video/reel1.mp4", "video/mp4")
      .SetHeight(1080).SetWidth(1920).SetDuration(3600.0);
  var video = new Video("https://example.org/anno/video1", videoResource, canvas.Id);
  ```

[↑ Back to top](#contents)

## Diagrams

*Not applicable - single self-contained type (composition with `VideoResource` is documented in [`Resource/README.md`](Resource/README.md)).*

## Package Dependencies

| Package | Version | Description | Links |
| --- | --- | --- | --- |
| Newtonsoft.Json | 13.0.4 | JSON.NET - this SDK's serialization engine (custom JsonConverters, attribute-driven read/write) | [NuGet](https://www.nuget.org/packages/Newtonsoft.Json/13.0.4) |

[↑ Back to top](#contents)

## See Also

- [`Resource/README.md`](Resource/README.md) - the `VideoResource` body type this wrapper carries.
- [`../README.md`](../README.md) - parent `Contents` grouping folder.
- [`../../README.md`](../../README.md) - grandparent `Nodes` folder (`Canvas.Videos` computed view).
- [`../../../README.md`](../../../README.md) - repository/docs top-level documentation.

[↑ Back to top](#contents)
