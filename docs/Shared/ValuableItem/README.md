# ValuableItem

## Contents
- [Overview](#overview)
- [Files](#files)
- [Types & Members](#types--members)
- [Examples](#examples)
- [See Also](#see-also)

## Overview

ValuableItem provides a simple value-wrapper TrackableObject for string properties that serialize as plain strings without @id/@type. Used for Label, Attribution, License, Logo, Thumbnail, Related, StartCanvas—types that need change tracking but minimal structure. ValuableItemJsonConverter writes Value as plain string using Newtonsoft.Json.

## Files

| File | Primary type(s) | LOC (approx) | Responsibility |
|------|-----------------|--------------|----------------|
| [ValuableItem.cs](../../../src/IIIF.Manifest.Serializer.Net/Shared/ValuableItem/ValuableItem.cs) | `ValuableItem<T>` | 16 | Simple value-wrapper with string Value property |
| [ValuableItemJsonConverter.cs](../../../src/IIIF.Manifest.Serializer.Net/Shared/ValuableItem/ValuableItemJsonConverter.cs) | `ValuableItemJsonConverter<T>` | 28 | Serializes Value as plain string |

[↑ Back to top](#contents)

## Types & Members

| Type | Kind | Summary | Inherits/Implements | Key Members |
|------|------|---------|---------------------|-------------|
| `ValuableItem<T>` | Class | Simple value-wrapper with immutable Value | `TrackableObject<T>` | `Value` (string) |
| `ValuableItemJsonConverter<T>` | Class | Writes Value as plain string, reads string into Value | `TrackableObjectJsonConverter<T>` | ReadJson, WriteJson |

[↑ Back to top](#contents)

## Examples

```csharp
// Label inherits ValuableItem<Label>
var label = new Label("My Book");
// Serializes as: "label": "My Book"

// Attribution inherits ValuableItem<Attribution>
var attribution = new Attribution("© 2024 Archive");
// Serializes as: "attribution": "© 2024 Archive"
```

[↑ Back to top](#contents)

## See Also

- [../Trackable/README.md](../Trackable/README.md) – Parent TrackableObject
- [../../Properties/README.md](../../Properties/README.md) – Property types using ValuableItem
- [../README.md](../README.md) – Parent Shared folder

[↑ Back to top](#contents)
