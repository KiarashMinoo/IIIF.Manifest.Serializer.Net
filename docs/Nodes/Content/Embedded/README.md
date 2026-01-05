# Embedded

## Contents
- [Overview](#overview)
- [Files](#files)
- [Types & Members](#types--members)
- [Examples](#examples)
- [See Also](#see-also)

## Overview

EmbeddedContent annotations embed textual content (EmbeddedContentResource with chars and language) directly in the manifest. Used for transcriptions, captions. Type is "oa:Annotation" using Newtonsoft.Json.

## Files

| File | Primary type(s) | LOC (approx) | Responsibility |
|------|-----------------|--------------|----------------|
| [EmbeddedContent.cs](../../../../src/IIIF.Manifest.Serializer.Net/Nodes/Content/Embedded/EmbeddedContent.cs) | `EmbeddedContent` | 18 | Text annotation with EmbeddedContentResource |
| [EmbeddedContentJsonConverter.cs](../../../../src/IIIF.Manifest.Serializer.Net/Nodes/Content/Embedded/EmbeddedContentJsonConverter.cs) | `EmbeddedContentJsonConverter` | 50 | Parses chars/language |

[↑ Back to top](#contents)

## Types & Members

| Type | Kind | Summary | Inherits/Implements | Key Members |
|------|------|---------|---------------------|-------------|
| `EmbeddedContent` | Class | Text annotation | `BaseContent<EmbeddedContent, EmbeddedContentResource>` | `On`, `Resource` |

[↑ Back to top](#contents)

## Examples

```csharp
var textResource = new EmbeddedContentResource("Transcribed text here", "en");
var embedded = new EmbeddedContent("https://example.org/anno/text1", textResource, canvas.Id);
canvas.AddOtherContent(embedded);
```

[↑ Back to top](#contents)

## See Also

- [./Resource/README.md](./Resource/README.md) – EmbeddedContentResource details
- [../README.md](../README.md) – Parent Content folder

[↑ Back to top](#contents)
