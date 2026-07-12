# MetadataProperty

## Contents

- [Overview](#overview)
- [Files](#files)
- [Types & Members](#types--members)
- [Diagrams](#diagrams)
- [Package Dependencies](#package-dependencies)
- [See Also](#see-also)

## Overview

This folder models IIIF's `metadata` property - the repeatable `{label, value}` pairs that appear
on `Manifest`, `Collection`, `Canvas`, and `Range` for arbitrary descriptive display (author, date,
physical description, etc.), distinct from the SDK's own single-purpose properties like `Label`
or `RequiredStatement`. `Metadata` is stable across Presentation API 2.0/2.1/3.0 - unlike many other
types in [`Properties`](../README.md), it has no legacy/3.0 split - and each entry's value is one or
more language-taggable [`MetadataValue`](MetadataValue/README.md) instances.

## Files

| File | Primary type(s) | LOC (approx) | Responsibility |
| --- | --- | --- | --- |
| `Metadata.cs` | `Metadata` | 78 | A single `{label, value[]}` metadata pair, with fluent `AddValue`/`ResetValue` helpers. |

## Types & Members

| Type | Kind | Summary | Inherits/Implements | Key Members |
| --- | --- | --- | --- | --- |
| `Metadata` | class | A single label/value(s) metadata pair | `TrackableObject<Metadata>` | `Label : string`, `Value : IReadOnlyCollection<MetadataValue>`, `AddValue(...)`, `ResetValue(...)` |

### Metadata

- **Kind / Namespace**: class, `IIIF.Manifests.Serializer.Properties.MetadataProperty`.
- **Inherits**: `TrackableObject<Metadata>` directly (no id/type of its own - a metadata entry is a
  plain object, not an identified resource).
- **Key properties**:
  - `Label : string` (`[JsonProperty("label")]`, required, non-nullable).
  - `Value : IReadOnlyCollection<MetadataValue.MetadataValue>` (`[JsonProperty("value")]`,
    `[JsonConverter(typeof(ObjectArrayJsonConverter))]`, defaults to `[]`) - see
    [`MetadataValue`](MetadataValue/README.md).
- **Constructors**:
  - `[JsonConstructor] private Metadata(string label)` - used by the JSON reader; leaves `Value` empty until populated.
  - `Metadata(string label, MetadataValue.MetadataValue value)` - single value.
  - `Metadata(string label, string value)` - convenience overload, wraps `value` in a new `MetadataValue`.
  - `Metadata(string label, string value, string language)` - convenience overload with a language tag.
- **Key methods**:
  - `AddValue(MetadataValue.MetadataValue value) : Metadata` / `AddValue(string value, string language) : Metadata` / `AddValue(string value) : Metadata` - appends another value to the existing collection (a metadata entry may carry multiple values, e.g. per-language variants).
  - `ResetValue(MetadataValue.MetadataValue value) : Metadata` / `ResetValue(string value, string language) : Metadata` / `ResetValue(string value) : Metadata` - replaces the entire `Value` collection with a single new value.
- **Usage Recipe**:
  ```csharp
  var author = new Metadata("Author", "Jane Doe")
      .AddValue("Jane Doe (autrice)", "fr");

  manifest.AddMetadata(author);
  ```

[↑ Back to top](#contents)

## Diagrams

*Not applicable - single self-contained type (`Metadata` composes `MetadataValue` from the child
folder, but has no sibling type of its own to diagram against).*

[↑ Back to top](#contents)

## Package Dependencies

| Package | Version | Description | Links |
| --- | --- | --- | --- |
| Newtonsoft.Json | 13.0.4 | JSON.NET - this SDK's serialization engine (custom JsonConverters, attribute-driven read/write) | [NuGet](https://www.nuget.org/packages/Newtonsoft.Json/13.0.4) |

[↑ Back to top](#contents)

## See Also

- [`docs/README.md`](../../README.md) - top-level SDK documentation.
- [`Properties`](../README.md) - parent folder.
- [`Properties/MetadataProperty/MetadataValue`](MetadataValue/README.md) - the `MetadataValue` type held by `Metadata.Value`.

[↑ Back to top](#contents)
