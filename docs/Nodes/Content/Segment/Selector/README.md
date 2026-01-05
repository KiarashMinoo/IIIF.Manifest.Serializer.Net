# Selector

## Contents
- [Overview](#overview)
- [Files](#files)
- [Types & Members](#types--members)
- [Examples](#examples)
- [See Also](#see-also)

## Overview

Selector defines spatial or temporal regions for segment annotations. Supports region (xywh rectangle: x, y, width, height). Type varies (oa:FragmentSelector, etc.) using Newtonsoft.Json.

## Files

| File | Primary type(s) | LOC (approx) | Responsibility |
|------|-----------------|--------------|----------------|
| [Selector.cs](../../../../../src/IIIF.Manifest.Serializer.Net/Nodes/Content/Segment/Selector/Selector.cs) | `Selector` | 22 | Region selector with xywh support |
| [SelectorJsonConverter.cs](../../../../../src/IIIF.Manifest.Serializer.Net/Nodes/Content/Segment/Selector/SelectorJsonConverter.cs) | `SelectorJsonConverter` | 38 | Parses region array |

[↑ Back to top](#contents)

## Types & Members

| Type | Kind | Summary | Inherits/Implements | Key Members |
|------|------|---------|---------------------|-------------|
| `Selector` | Class | Region selector | `BaseItem<Selector>` | `Region`, `SetRegion(list)`, `SetRegion(x,y,w,h)` |

[↑ Back to top](#contents)

## Examples

```csharp
var selector = new Selector("...", "oa:FragmentSelector")
    .SetRegion(100, 100, 200, 200); // x=100, y=100, width=200, height=200
```

[↑ Back to top](#contents)

## See Also

- [../README.md](../README.md) – Parent Segment folder

[↑ Back to top](#contents)
