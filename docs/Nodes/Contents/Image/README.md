# Image

## Contents

- [Overview](#overview)
- [Files](#files)
- [Types & Members](#types--members)
- [Diagrams](#diagrams)
- [Package Dependencies](#package-dependencies)
- [See Also](#see-also)

## Overview

This folder models the legacy (2.x) `oa:Annotation` wrapper used to paint an image resource onto a
`Canvas` - the pre-3.0 shape that `Canvas.Images` (in `../../README.md`) presents as a computed view
over the 3.0-native `Items`/`Annotation` storage. It pairs with [`Resource`](Resource/README.md) for
the actual `ImageResource` body, typically alongside a IIIF Image API `Service` for
dynamic image delivery. In 3.0-native code, prefer constructing an `Annotation` whose body is an
`ImageResource` directly and calling `Canvas.AddAnnotation`, rather than constructing an `Image`
wrapper by hand.

## Files

| File | Primary type(s) | LOC (approx) | Responsibility |
| --- | --- | --- | --- |
| `Image.cs` | `Image` | 31 | Legacy 2.x `oa:Annotation` wrapper pairing an `ImageResource` with a target canvas (`on`). |

## Types & Members

| Type | Kind | Summary | Inherits/Implements | Key Members |
| --- | --- | --- | --- | --- |
| `Image` | class | Legacy image-painting annotation wrapper | `BaseContent<Image, ImageResource>` | `Motivation`, `On` |

### Image

- **Kind / Namespace**: `class`, `IIIF.Manifests.Serializer.Nodes.Contents.Image`.
- **Inherits**: `BaseContent<Image, ImageResource>`.
- **Key properties**:
  - `Motivation : string` (`motivation`) - always `"sc:painting"` (set in the constructor).
  - `On : string` (`on`) - the id of the `Canvas` (or region) this image is painted onto.
  - Inherited: `Resource : ImageResource` (`resource`), plus `Format`/`Height`/`Width` from `BaseContent<Image>`.
- **Constructors**: `Image(string id, ImageResource resource, string on)`.
- **Usage Recipe** (legacy-shim construction; for new 3.0-native code build an `Annotation` with an `ImageResource` body via `Canvas.AddAnnotation` instead - the internal `Canvas.ToImage`/`ToAnnotation` helpers show the exact mapping):
  ```csharp
  var imageResource = new ImageResource("https://example.org/full/full/0/default.jpg", ImageFormat.Jpeg)
      .SetHeight(2000).SetWidth(1500)
      .AddService(new Service("http://iiif.io/api/image/3/context.json", "https://example.org/iiif/manuscript", "level2"));
  var image = new Image("https://example.org/anno/p1", imageResource, canvas.Id);
  ```

[↑ Back to top](#contents)

## Diagrams

*Not applicable - single self-contained type (composition with `ImageResource` is documented in [`Resource/README.md`](Resource/README.md)).*

## Package Dependencies

| Package | Version | Description | Links |
| --- | --- | --- | --- |
| Newtonsoft.Json | 13.0.4 | JSON.NET - this SDK's serialization engine (custom JsonConverters, attribute-driven read/write) | [NuGet](https://www.nuget.org/packages/Newtonsoft.Json/13.0.4) |

[↑ Back to top](#contents)

## See Also

- [`Resource/README.md`](Resource/README.md) - the `ImageResource` body type this wrapper carries.
- [`../README.md`](../README.md) - parent `Contents` grouping folder.
- [`../../README.md`](../../README.md) - grandparent `Nodes` folder (`Canvas.Images` computed view).
- [`../../../README.md`](../../../README.md) - repository/docs top-level documentation.

[↑ Back to top](#contents)
