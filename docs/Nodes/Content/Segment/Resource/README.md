# SegmentResource

## Contents
- [Overview](#overview)
- [Files](#files)
- [Types & Members](#types--members)
- [Examples](#examples)
- [See Also](#see-also)

## Overview

SegmentResource holds segment content with optional "full" reference to complete resource. Used with Selector to target canvas regions using Newtonsoft.Json.

## Files

| File | Primary type(s) | LOC (approx) | Responsibility |
|------|-----------------|--------------|----------------|
| [SegmentResource.cs](../../../../../src/IIIF.Manifest.Serializer.Net/Nodes/Content/Segment/Resource/SegmentResource.cs) | `SegmentResource` | 20 | Segment resource with full reference |
| [SegmentResourceJsonConverter.cs](../../../../../src/IIIF.Manifest.Serializer.Net/Nodes/Content/Segment/Resource/SegmentResourceJsonConverter.cs) | `SegmentResourceJsonConverter` | 40 | Parses full field |

[↑ Back to top](#contents)

## Types & Members

| Type | Kind | Summary | Inherits/Implements | Key Members |
|------|------|---------|---------------------|-------------|
| `SegmentResource` | Class | Resource with optional full reference | `BaseResource<SegmentResource>` | `Full`, `SetFull` |

[↑ Back to top](#contents)

## Examples

```csharp
var resource = new SegmentResource("https://example.org/detail.jpg", "dctypes:Image")
    .SetFormat("image/jpeg")
    .SetFull(new BaseResource("https://example.org/full.jpg"));
```

[↑ Back to top](#contents)

## See Also

- [../README.md](../README.md) – Parent Segment folder

[↑ Back to top](#contents)
