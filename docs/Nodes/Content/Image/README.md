# Image

## Contents
- [Overview](#overview)
- [Files](#files)
- [Types & Members](#types--members)
- [Examples](#examples)
- [See Also](#see-also)

## Overview

Image annotations link visual content (ImageResource) to canvas regions. Primary painting annotations with motivation="sc:painting". ImageJsonConverter enforces required resource and "on" fields using Newtonsoft.Json.

## Files

| File | Primary type(s) | LOC (approx) | Responsibility |
|------|-----------------|--------------|----------------|
| [Image.cs](../../../../src/IIIF.Manifest.Serializer.Net/Nodes/Content/Image/Image.cs) | `Image` | 20 | Painting annotation linking ImageResource to canvas |
| [ImageJsonConverter.cs](../../../../src/IIIF.Manifest.Serializer.Net/Nodes/Content/Image/ImageJsonConverter.cs) | `ImageJsonConverter` | 60 | Validates resource/on required |

[↑ Back to top](#contents)

## Types & Members

| Type | Kind | Summary | Inherits/Implements | Key Members |
|------|------|---------|---------------------|-------------|
| `Image` | Class | Painting annotation with motivation="sc:painting" | `BaseContent<Image, ImageResource>` | `Motivation`, `On`, `Resource` |

[↑ Back to top](#contents)

## Examples

```csharp
var resource = new ImageResource("https://example.org/image.jpg", "image/jpeg")
    .SetHeight(1000).SetWidth(800);
var image = new Image("https://example.org/anno/1", resource, canvas.Id);
canvas.AddImage(image);
```

[↑ Back to top](#contents)

## See Also

- [./Resource/README.md](./Resource/README.md) – ImageResource details
- [../README.md](../README.md) – Parent Content folder

[↑ Back to top](#contents)
