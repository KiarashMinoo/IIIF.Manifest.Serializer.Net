# Resources

## Contents
- [Overview](#overview)
- [Files](#files)
- [Types & Members](#types--members)
- [Examples](#examples)
- [See Also](#see-also)

## Overview

The Resources folder defines BaseResource, the foundation for all IIIF Presentation API 2.0 content resources (ImageResource, EmbeddedContentResource, SegmentResource). BaseResource extends FormatableItem to provide @id/@type/format fields for media resources. BaseResourceJsonConverter enforces resource structure and validates format presence using Newtonsoft.Json.

## Files

| File | Primary type(s) | LOC (approx) | Responsibility |
|------|-----------------|--------------|----------------|
| [BaseResource.cs](../../../../src/IIIF.Manifest.Serializer.Net/Shared/Content/Resources/BaseResource.cs) | `BaseResource<T>`, `BaseResource` | 28 | Resource base with format support |
| [BaseResourceJsonConverter.cs](../../../../src/IIIF.Manifest.Serializer.Net/Shared/Content/Resources/BaseResourceJsonConverter.cs) | `BaseResourceJsonConverter<T>`, `BaseResourceJsonConverter` | 40 | Converts resources with format validation |

[↑ Back to top](#contents)

## Types & Members

| Type | Kind | Summary | Inherits/Implements | Key Members |
|------|------|---------|---------------------|-------------|
| `BaseResource<T>` | Abstract class | Media resource with format | `FormatableItem<T>` | `Format`, `SetFormat` |
| `BaseResource` | Class | Non-generic variant | `BaseResource<BaseResource>` | Inherited members |
| `BaseResourceJsonConverter<T>` | Abstract class | Converts resources | `FormatableItemJsonConverter<T>` | Format parsing |

[↑ Back to top](#contents)

## Examples

```csharp
// ImageResource inherits BaseResource
var resource = new ImageResource("https://example.org/image.jpg", "image/jpeg")
    .SetHeight(1000)
    .SetWidth(800);
```

[↑ Back to top](#contents)

## See Also

- [../../../Nodes/Content/Image/Resource/README.md](../../../Nodes/Content/Image/Resource/README.md) – ImageResource implementation
- [../../FormatableItem/README.md](../../FormatableItem/README.md) – Parent FormatableItem
- [../README.md](../README.md) – Parent Content folder

[↑ Back to top](#contents)
