# EmbeddedContentResource

## Contents
- [Overview](#overview)
- [Files](#files)
- [Types & Members](#types--members)
- [Examples](#examples)
- [See Also](#see-also)

## Overview

EmbeddedContentResource holds embedded text with chars (content string) and language (language code). Type is "cnt:ContentAsText". Used for transcriptions, captions embedded directly in manifest using Newtonsoft.Json.

## Files

| File | Primary type(s) | LOC (approx) | Responsibility |
|------|-----------------|--------------|----------------|
| [EmbeddedContentResource.cs](../../../../../src/IIIF.Manifest.Serializer.Net/Nodes/Content/Embedded/Resource/EmbeddedContentResource.cs) | `EmbeddedContentResource` | 22 | Text resource with chars and language |
| [EmbeddedContentResourceJsonConverter.cs](../../../../../src/IIIF.Manifest.Serializer.Net/Nodes/Content/Embedded/Resource/EmbeddedContentResourceJsonConverter.cs) | `EmbeddedContentResourceJsonConverter` | 42 | Parses chars/language fields |

[↑ Back to top](#contents)

## Types & Members

| Type | Kind | Summary | Inherits/Implements | Key Members |
|------|------|---------|---------------------|-------------|
| `EmbeddedContentResource` | Class | Text with chars and language | `BaseResource<EmbeddedContentResource>` | `Chars`, `Language` |

[↑ Back to top](#contents)

## Examples

```csharp
var textResource = new EmbeddedContentResource(
    "This is the transcribed text from the manuscript.",
    "en"
);
```

[↑ Back to top](#contents)

## See Also

- [../README.md](../README.md) – Parent Embedded folder

[↑ Back to top](#contents)
