# ImageResource

## Contents
- [Overview](#overview)
- [Files](#files)
- [Types & Members](#types--members)
- [Examples](#examples)
- [See Also](#see-also)

## Overview

ImageResource represents visual content with @id URI, format (MIME type), dimensions (height/width), and optional IIIF Image API service for deep-zoom. Type is "dctypes:Image". Implements IDimenssionSupport for dimension handling using Newtonsoft.Json.

## Files

| File | Primary type(s) | LOC (approx) | Responsibility |
|------|-----------------|--------------|----------------|
| [ImageResource.cs](../../../../../src/IIIF.Manifest.Serializer.Net/Nodes/Content/Image/Resource/ImageResource.cs) | `ImageResource` | 24 | Image resource with dimensions and service |
| [ImageResourceJsonConverter.cs](../../../../../src/IIIF.Manifest.Serializer.Net/Nodes/Content/Image/Resource/ImageResourceJsonConverter.cs) | `ImageResourceJsonConverter` | 45 | Parses dimensions, service |

[↑ Back to top](#contents)

## Types & Members

| Type | Kind | Summary | Inherits/Implements | Key Members |
|------|------|---------|---------------------|-------------|
| `ImageResource` | Class | Image with format, dimensions, service | `BaseResource<ImageResource>`, `IDimenssionSupport` | `Height`, `Width`, `Format`, `Service`, `SetHeight`, `SetWidth` |

[↑ Back to top](#contents)

## Examples

```csharp
var resource = new ImageResource("https://example.org/image.jpg", "image/jpeg")
    .SetHeight(2000)
    .SetWidth(1600);

// Add IIIF Image Service for deep-zoom
var service = new Service(
    "http://iiif.io/api/image/2/context.json",
    "https://example.org/iiif/image",
    "http://iiif.io/api/image/2/level1.json"
).AddTile(new Tile(512, new[] { 1, 2, 4, 8 }));

resource.SetService(service);
```

[↑ Back to top](#contents)

## See Also

- [../README.md](../README.md) – Parent Image folder
- [../../../../Properties/Service/README.md](../../../../Properties/Service/README.md) – Service details

[↑ Back to top](#contents)
