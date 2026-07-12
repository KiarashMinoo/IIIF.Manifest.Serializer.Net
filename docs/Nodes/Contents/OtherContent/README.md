# OtherContent

## Contents

- [Overview](#overview)
- [Files](#files)
- [Types & Members](#types--members)
- [Diagrams](#diagrams)
- [Package Dependencies](#package-dependencies)
- [See Also](#see-also)

## Overview

This folder models the legacy (2.x) `Canvas.otherContent` reference - a bare `sc:AnnotationList`
pointer (id only) attached to a Canvas for supplementary annotations (e.g. a transcription layer)
separate from the Canvas's own painting content. `Canvas.OtherContents` (in `../../README.md`)
presents this as a computed, read-only legacy view over the 3.0-native `Canvas.Annotations`
(`AnnotationPage` references); the 3.0-preferred replacement is `Canvas.AddAnnotationPageReference`.

## Files

| File | Primary type(s) | LOC (approx) | Responsibility |
| --- | --- | --- | --- |
| `OtherContent.cs` | `OtherContent` | 5 | Bare `sc:AnnotationList` reference id, legacy `Canvas.otherContent` entry. |

## Types & Members

| Type | Kind | Summary | Inherits/Implements | Key Members |
| --- | --- | --- | --- | --- |
| `OtherContent` | class (primary constructor) | Legacy AnnotationList reference | `BaseContent<OtherContent>` | ctor `(string id)` |

### OtherContent

- **Kind / Namespace**: `class`, `IIIF.Manifests.Serializer.Nodes.Contents.OtherContent`. Declared as a
  one-line primary-constructor class:
  ```csharp
  public class OtherContent(string id) : BaseContent<OtherContent>(id, "sc:AnnotationList");
  ```
- **Inherits**: `BaseContent<OtherContent>` (single-type-parameter overload - no `Resource`; this is a
  bare reference, not a resource-carrying wrapper). Inherits `Format`/`Height`/`Width` from
  `BaseContent<TBaseContent>`, though none are meaningful for a bare reference in practice.
- **Constructors**: `OtherContent(string id)` - sets `Type = "sc:AnnotationList"`.
- **Usage Recipe** (legacy-shim construction; for new 3.0-native code use `Canvas.AddAnnotationPageReference(AnnotationPage)` instead):
  ```csharp
  var otherContentRef = new OtherContent("https://example.org/list/1");
  // canvas.AddOtherContent(otherContentRef) is [Obsolete(error: true)] for new code -
  // prefer: canvas.AddAnnotationPageReference(new AnnotationPage("https://example.org/list/1"));
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

- [`../README.md`](../README.md) - parent `Contents` grouping folder.
- [`../../README.md`](../../README.md) - `Nodes` folder (`Canvas.OtherContents` computed view, `Canvas.AddAnnotationPageReference` replacement).
- [`../Annotation/README.md`](../Annotation/README.md) - `AnnotationPage`, the 3.0-native replacement concept.
- [`../../../README.md`](../../../README.md) - repository/docs top-level documentation.

[↑ Back to top](#contents)
