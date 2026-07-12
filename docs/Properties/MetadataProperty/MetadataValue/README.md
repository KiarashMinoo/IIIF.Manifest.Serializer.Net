# MetadataValue

## Contents

- [Overview](#overview)
- [Files](#files)
- [Types & Members](#types--members)
- [Diagrams](#diagrams)
- [Package Dependencies](#package-dependencies)
- [See Also](#see-also)

## Overview

This folder holds the single leaf type used by [`Metadata.Value`](../README.md) - one
language-taggable `{"@value", "@language"}` entry within a `metadata` pair. It mirrors
`Properties.Description`'s `@value`/`@language` shape closely (both wrap `ValuableItemJsonConverter<T>`
and override `Value` to add the `[JsonProperty("@value")]` annotation), but lives in its own nested
namespace/folder because it is specific to the `metadata` property rather than being a
general-purpose descriptive string.

## Files

| File | Primary type(s) | LOC (approx) | Responsibility |
| --- | --- | --- | --- |
| `MetadataValue.cs` | `MetadataValue` | 26 | A single language-taggable metadata value (`@value`/`@language`). |

## Types & Members

| Type | Kind | Summary | Inherits/Implements | Key Members |
| --- | --- | --- | --- | --- |
| `MetadataValue` | class | Language-taggable metadata value | `ValuableItem<MetadataValue>` | `Value : string` (`@value`), `Language : string?` (`@language`), `SetValue(string)` |

### MetadataValue

- **Kind / Namespace**: class, `IIIF.Manifests.Serializer.Properties.MetadataProperty.MetadataValue`.
  `[JsonConverter(typeof(ValuableItemJsonConverter<MetadataValue>))]`.
- **Inherits**: `ValuableItem<MetadataValue>`.
- **Key properties**:
  - `Value : string` (`[JsonProperty("@value")]`, overrides `ValuableItem<T>.Value` to attach the JSON property name).
  - `Language : string?` (`[JsonProperty("@language")]`).
- **Constructors**: `MetadataValue(string value)`; `MetadataValue(string value, string language)`.
- **Key methods**: `SetValue(string value) : MetadataValue` (via `SetElementValue(a => a.Value, value)`).
- **Usage Recipe**:
  ```csharp
  using IIIF.Manifests.Serializer.Properties.MetadataProperty;
  using IIIF.Manifests.Serializer.Properties.MetadataProperty.MetadataValue;

  var metadata = new Metadata("Date", new MetadataValue("1523"))
      .AddValue(new MetadataValue("XVIe siècle", "fr"));
  ```

[↑ Back to top](#contents)

## Diagrams

*Not applicable - single self-contained type.*

[↑ Back to top](#contents)

## Package Dependencies

| Package | Version | Description | Links |
| --- | --- | --- | --- |
| Newtonsoft.Json | 13.0.4 | JSON.NET - this SDK's serialization engine (custom JsonConverters, attribute-driven read/write) | [NuGet](https://www.nuget.org/packages/Newtonsoft.Json/13.0.4) |

[↑ Back to top](#contents)

## See Also

- [`docs/README.md`](../../../README.md) - top-level SDK documentation.
- [`Properties/MetadataProperty`](../README.md) - parent folder; `Metadata.Value` holds a collection of this type.
- [`Properties`](../../README.md) - the wider descriptive-properties folder.

[↑ Back to top](#contents)
