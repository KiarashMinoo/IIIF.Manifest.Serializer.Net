# Segment

## Contents
- [Overview](#overview)
- [Files](#files)
- [Types & Members](#types--members)
- [Examples](#examples)
- [See Also](#see-also)

## Overview

Segment annotations target specific canvas regions using Selector (xywh rectangles, temporal segments). SegmentResource with optional "full" reference to complete resource. Motivation="sc:painting" using Newtonsoft.Json.

## Files

| File | Primary type(s) | LOC (approx) | Responsibility |
|------|-----------------|--------------|----------------|
| [Segment.cs](../../../../src/IIIF.Manifest.Serializer.Net/Nodes/Content/Segment/Segment.cs) | `Segment` | 26 | Region annotation with selector |
| [SegmentJsonConverter.cs](../../../../src/IIIF.Manifest.Serializer.Net/Nodes/Content/Segment/SegmentJsonConverter.cs) | `SegmentJsonConverter` | 65 | Parses selector, resource |

[↑ Back to top](#contents)

## Types & Members

| Type | Kind | Summary | Inherits/Implements | Key Members |
|------|------|---------|---------------------|-------------|
| `Segment` | Class | Region annotation with selector | `BaseContent<Segment, SegmentResource>` | `Motivation`, `On`, `Selector`, `Resource`, `SetSelector` |

[↑ Back to top](#contents)

## Examples

```csharp
var resource = new SegmentResource("https://example.org/detail.jpg", "dctypes:Image");
var selector = new Selector("...", "oa:FragmentSelector").SetRegion(100, 100, 200, 200);
var segment = new Segment("...", resource, canvas.Id).SetSelector(selector);
```

[↑ Back to top](#contents)

## See Also

- [./Resource/README.md](./Resource/README.md) – SegmentResource details
- [./Selector/README.md](./Selector/README.md) – Selector details
- [../README.md](../README.md) – Parent Content folder

[↑ Back to top](#contents)
