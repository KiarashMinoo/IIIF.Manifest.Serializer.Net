# Embedded

## Contents

- [Overview](#overview)
- [Files](#files)
- [Types & Members](#types--members)
- [Diagrams](#diagrams)
- [Package Dependencies](#package-dependencies)
- [See Also](#see-also)

## Overview

This folder models the legacy (2.x) `oa:Annotation` wrapper for the W3C `cnt:ContentAsText` shape -
an inline, literal text value (never dereferenceable, so it has no id) painted onto a Canvas, e.g.
for transcription or commenting motivations predating the 3.0-native `TextualBody`. It pairs with
[`Resource`](Resource/README.md) for the actual `EmbeddedContentResource` body. Distinct from
`Nodes.Contents.Textual.Resource.TextualBody`, which is the 3.0-native equivalent concept.

## Files

| File | Primary type(s) | LOC (approx) | Responsibility |
| --- | --- | --- | --- |
| `EmbeddedContent.cs` | `EmbeddedContent` | 22 | Legacy 2.x `oa:Annotation` wrapper pairing an `EmbeddedContentResource` with a target (`on`). |

## Types & Members

| Type | Kind | Summary | Inherits/Implements | Key Members |
| --- | --- | --- | --- | --- |
| `EmbeddedContent` | class | Legacy inline-text-painting annotation wrapper | `BaseContent<EmbeddedContent, EmbeddedContentResource>` | `On` |

### EmbeddedContent

- **Kind / Namespace**: `class`, `IIIF.Manifests.Serializer.Nodes.Contents.Embedded`.
- **Inherits**: `BaseContent<EmbeddedContent, EmbeddedContentResource>`.
- **Key properties**: `On : string` (`on`) - the id of the Canvas (or region) this content is painted onto. Also inherits `Resource : EmbeddedContentResource` (`resource`) plus `Format`/`Height`/`Width` from `BaseContent<EmbeddedContent>`.
- **Constructors**: `EmbeddedContent(string id, EmbeddedContentResource resource, string on)`.
- **Usage Recipe** (legacy-shim construction; for new 3.0-native code use `TextualBody` as an `Annotation` body instead - see `Nodes.Contents.Textual.Resource`):
  ```csharp
  var textResource = new EmbeddedContentResource("A hand-written marginal note.", "en");
  var embedded = new EmbeddedContent("https://example.org/anno/note1", textResource, canvas.Id);
  ```

[↑ Back to top](#contents)

## Diagrams

*Not applicable - single self-contained type (composition with `EmbeddedContentResource` is documented in [`Resource/README.md`](Resource/README.md)).*

## Package Dependencies

| Package | Version | Description | Links |
| --- | --- | --- | --- |
| Newtonsoft.Json | 13.0.4 | JSON.NET - this SDK's serialization engine (custom JsonConverters, attribute-driven read/write) | [NuGet](https://www.nuget.org/packages/Newtonsoft.Json/13.0.4) |

[↑ Back to top](#contents)

## See Also

- [`Resource/README.md`](Resource/README.md) - the `EmbeddedContentResource` body type this wrapper carries.
- [`../README.md`](../README.md) - parent `Contents` grouping folder.
- [`../../README.md`](../../README.md) - grandparent `Nodes` folder.
- [`../Textual/README.md`](../Textual/README.md) - `TextualBody`, the 3.0-native equivalent concept.
- [`../../../README.md`](../../../README.md) - repository/docs top-level documentation.

[↑ Back to top](#contents)
