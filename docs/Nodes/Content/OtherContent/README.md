# OtherContent

## Contents
- [Overview](#overview)
- [Files](#files)
- [Types & Members](#types--members)
- [Examples](#examples)
- [See Also](#see-also)

## Overview

OtherContent links to external annotation lists (sc:AnnotationList). Used for transcriptions, commentary stored externally. Inherits BaseContent without resource field using Newtonsoft.Json.

## Files

| File | Primary type(s) | LOC (approx) | Responsibility |
|------|-----------------|--------------|----------------|
| [OtherContent.cs](../../../../src/IIIF.Manifest.Serializer.Net/Nodes/Content/OtherContent/OtherContent.cs) | `OtherContent` | 12 | Link to annotation list |
| [OtherContentJsonConverter.cs](../../../../src/IIIF.Manifest.Serializer.Net/Nodes/Content/OtherContent/OtherContentJsonConverter.cs) | `OtherContentJsonConverter` | 35 | Simple converter |

[↑ Back to top](#contents)

## Types & Members

| Type | Kind | Summary | Inherits/Implements | Key Members |
|------|------|---------|---------------------|-------------|
| `OtherContent` | Class | Link to external annotations | `BaseContent<OtherContent>` | (only id/type from BaseContent) |

[↑ Back to top](#contents)

## Examples

```csharp
var otherContent = new OtherContent("https://example.org/annotations/list1");
canvas.AddOtherContent(otherContent);
```

[↑ Back to top](#contents)

## See Also

- [../README.md](../README.md) – Parent Content folder

[↑ Back to top](#contents)
