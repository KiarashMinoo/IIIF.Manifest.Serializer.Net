# Resource

## Contents

- [Overview](#overview)
- [Files](#files)
- [Types & Members](#types--members)
- [Diagrams](#diagrams)
- [Package Dependencies](#package-dependencies)
- [See Also](#see-also)

## Overview

This folder models the legacy W3C `cnt:ContentAsText` shape - an inline literal text value with no
dereferenceable `id`, used as the body of the legacy `EmbeddedContent` annotation wrapper (see
[`../README.md`](../README.md)).

## Files

| File | Primary type(s) | LOC (approx) | Responsibility |
| --- | --- | --- | --- |
| `EmbeddedContentResource.cs` | `EmbeddedContentResource` | 34 | The `cnt:ContentAsText` inline-text resource (`chars`, `language`). |

## Types & Members

| Type | Kind | Summary | Inherits/Implements | Key Members |
| --- | --- | --- | --- | --- |
| `EmbeddedContentResource` | class | Inline literal-text resource | `BaseResource<EmbeddedContentResource>` | `Chars`, `Language` |

### EmbeddedContentResource

- **Kind / Namespace**: `class`, `IIIF.Manifests.Serializer.Nodes.Contents.Embedded.Resource`.
- **Inherits**: `BaseResource<EmbeddedContentResource>`.
- **Key properties**: `Chars : string` (`chars`) - the literal text content. `Language : string` (`language`) - the text's language.
- **Constructors**: `EmbeddedContentResource(string chars, string language)` - constructs with an empty `id` and `type = "cnt:ContentAsText"` (a `cnt:ContentAsText` body is a literal inline value, never a dereferenceable resource, so it deliberately has no real `@id`). Note (from the source comment): this constructor previously had a bug where the type string was accidentally passed as `@id` and misspelled (`"ContextAsText"`) - both are now fixed.
- **Usage Recipe**:
  ```csharp
  var textResource = new EmbeddedContentResource("A hand-written marginal note.", "en");
  var embedded = new EmbeddedContent("https://example.org/anno/note1", textResource, canvas.Id);
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

- [`../README.md`](../README.md) - parent `Embedded` folder (the `EmbeddedContent` wrapper carrying this resource).
- [`../../README.md`](../../README.md) - grandparent `Contents` grouping folder.
- [`../../../README.md`](../../../README.md) - `Nodes` folder.
- [`../../../../README.md`](../../../../README.md) - repository/docs top-level documentation.

[↑ Back to top](#contents)
