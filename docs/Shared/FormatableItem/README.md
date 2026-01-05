# FormatableItem

## Contents
- [Overview](#overview)
- [Files](#files)
- [Types & Members](#types--members)
- [Examples](#examples)
- [See Also](#see-also)

## Overview

FormatableItem extends BaseItem with a format field for MIME type specifications. Used for simple link types (Rendering, SeeAlso, Within) that have @id/@type but need format metadata. FormatableItemJsonConverter inherits BaseItemJsonConverter with DisableTypeChecking=true to make @type optional for link resources using Newtonsoft.Json.

## Files

| File | Primary type(s) | LOC (approx) | Responsibility |
|------|-----------------|--------------|----------------|
| [FormatableItem.cs](../../../src/IIIF.Manifest.Serializer.Net/Shared/FormatableItem/FormatableItem.cs) | `FormatableItem<T>` | 24 | Adds format field to BaseItem |
| [FormatableItemJsonConverter.cs](../../../src/IIIF.Manifest.Serializer.Net/Shared/FormatableItem/FormatableItemJsonConverter.cs) | `FormatableItemJsonConverter<T>` | 32 | Converts FormatableItem with optional @type (DisableTypeChecking=true) |

[↑ Back to top](#contents)

## Types & Members

| Type | Kind | Summary | Inherits/Implements | Key Members |
|------|------|---------|---------------------|-------------|
| `FormatableItem<T>` | Abstract class | BaseItem with format field | `BaseItem<T>` | `Format`, `SetFormat` |
| `FormatableItemJsonConverter<T>` | Abstract class | Converts FormatableItem; disables @type checking | `BaseItemJsonConverter<T>` | DisableTypeChecking=true |

[↑ Back to top](#contents)

## Examples

```csharp
// Rendering inherits FormatableItem
var rendering = new Rendering("https://example.org/book.pdf", "PDF Version")
    .SetFormat("application/pdf");

// SeeAlso inherits FormatableItem
var seeAlso = new SeeAlso("https://example.org/metadata.xml")
    .SetFormat("application/xml");
```

[↑ Back to top](#contents)

## See Also

- [../BaseItem/README.md](../BaseItem/README.md) – Parent BaseItem class
- [../../Properties/Rendering/README.md](../../Properties/Rendering/README.md) – Rendering uses FormatableItem
- [../README.md](../README.md) – Parent Shared folder

[↑ Back to top](#contents)
