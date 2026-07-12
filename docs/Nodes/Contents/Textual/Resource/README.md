# Resource

## Contents

- [Overview](#overview)
- [Files](#files)
- [Types & Members](#types--members)
- [Diagrams](#diagrams)
- [Package Dependencies](#package-dependencies)
- [See Also](#see-also)

## Overview

This folder models the W3C Web Annotation Model's "TextualBody" - an inline string value used as an
`Annotation`'s body for commenting/tagging/transcribing motivations (cookbook recipes
0019/0021/0103/0135/0258/0261/0269/0306/0326/0346/0464/0489/0561, among others). Unlike other
`IBaseResource` implementers, it is never dereferenceable and so has no `id` at all. It is the
3.0-native counterpart to the legacy `cnt:ContentAsText` shape modeled by
`EmbeddedContentResource` (`../../Embedded/Resource`).

## Files

| File | Primary type(s) | LOC (approx) | Responsibility |
| --- | --- | --- | --- |
| `TextualBody.cs` | `TextualBody` | 73 | The `TextualBody` inline-string annotation body (`value`, `format`, `language`). |

## Types & Members

| Type | Kind | Summary | Inherits/Implements | Key Members |
| --- | --- | --- | --- | --- |
| `TextualBody` | class | Inline text annotation body | `TrackableObject<TextualBody>`, `IBaseResource` | `Type`, `Value`, `Format`, `Language`, `SetFormat`, `SetLanguage` |

### TextualBody

- **Kind / Namespace**: `class`, `IIIF.Manifests.Serializer.Nodes.Contents.Textual.Resource`.
- **Inherits/Implements**: `TrackableObject<TextualBody>` directly (not `BaseResource<T>` - it has no `id`), `IBaseResource` (explicit `ResourceType? IBaseResource.Type => new(Type)`).
- **Attributes**: `[PresentationAPI("3.0")]`.
- **Key properties**:
  - `Type : string` (`type`) - defaults to `"TextualBody"`.
  - `Value : string` (`value`) - the literal text.
  - `Format : string?` (`format`) - e.g. `"text/plain"`.
  - `Language : string?` (`language`).
- **Key methods**: `SetFormat(string)`, `SetLanguage(string)` - fluent.
- **Constructors**: `[JsonConstructor] TextualBody(string value)`.
- **Usage Recipe** (cookbook recipe 0021-tagging, a simple comment annotation):
  ```csharp
  var comment = new TextualBody("This is a marginal note about the illustration.")
      .SetFormat("text/plain")
      .SetLanguage("en");
  var annotation = new Annotation("https://example.org/anno/comment1", comment, canvas.Id)
      .SetMotivation("commenting");
  canvas.AddAnnotation(annotation);
  ```

[↑ Back to top](#contents)

## Diagrams

*Not applicable - single self-contained type.*

## Package Dependencies

| Package | Version | Description | Links |
| --- | --- | --- | --- |
| Newtonsoft.Json | 13.0.4 | JSON.NET - this SDK's serialization engine (custom JsonConverters, attribute-driven read/write) | [NuGet](https://www.nuget.org/packages/Newtonsoft.Json/13.0.4) |

[↑ Back to top](#contents)

## See Also

- [`../README.md`](../README.md) - parent `Textual` grouping folder.
- [`../../README.md`](../../README.md) - grandparent `Contents` grouping folder.
- [`../../../README.md`](../../../README.md) - `Nodes` folder.
- [`../../Embedded/Resource/README.md`](../../Embedded/Resource/README.md) - `EmbeddedContentResource`, the legacy equivalent this type replaces.
- [`../../../../README.md`](../../../../README.md) - repository/docs top-level documentation.

[↑ Back to top](#contents)
