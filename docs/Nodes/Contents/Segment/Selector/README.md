# Selector

## Contents

- [Overview](#overview)
- [Files](#files)
- [Types & Members](#types--members)
- [Diagrams](#diagrams)
- [Package Dependencies](#package-dependencies)
- [See Also](#see-also)

## Overview

This folder models the legacy (2.x) region selector attached to a `Segment` (see
[`../README.md`](../README.md)) - a bare `[left, top, width, height]` pixel-region array. This is
the pre-3.0 shape; the 3.0-native equivalent is the spec-general `ISelector` family
(`FragmentSelector`/`PointSelector`/`ImageApiSelector`/`SvgSelector` in `Shared/Selectors/`) used by
`AnnotationTarget`/`SpecificResource`/`ContentStateTarget` elsewhere in this SDK.

## Files

| File | Primary type(s) | LOC (approx) | Responsibility |
| --- | --- | --- | --- |
| `Selector.cs` | `Selector` | 28 | Legacy region selector: a bare `[left, top, width, height]` int array under `region`. |

## Types & Members

| Type | Kind | Summary | Inherits/Implements | Key Members |
| --- | --- | --- | --- | --- |
| `Selector` | class (primary constructor) | Legacy pixel-region selector | `BaseItem<Selector>` | `Region`, `SetRegion` (two overloads) |

### Selector

- **Kind / Namespace**: `class`, `IIIF.Manifests.Serializer.Nodes.Contents.Segment.Selector`. Declared with a primary constructor:
  ```csharp
  public class Selector(string id, string type) : BaseItem<Selector>(id, type)
  ```
- **Inherits**: `BaseItem<Selector>` - carries its own `@id`/`@type` (e.g. `"oa:FragmentSelector"`).
- **Key properties**: `Region : IReadOnlyCollection<int>` (`region`) - `[left, top, width, height]`.
- **Key methods**:
  - `SetRegion(IReadOnlyCollection<int> region)` - fluent, sets the raw array.
  - `SetRegion(int left, int top, int width, int height)` - convenience overload building the 4-element array.
- **Usage Recipe**:
  ```csharp
  var selector = new Selector("https://example.org/selector/1", "oa:FragmentSelector")
      .SetRegion(left: 0, top: 0, width: 500, height: 500);
  var segment = new Segment("https://example.org/anno/segment1", segmentResource, canvas.Id)
      .SetSelector(selector);
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

- [`../README.md`](../README.md) - parent `Segment` folder (the `Segment` wrapper carrying this selector).
- [`../Resource/README.md`](../Resource/README.md) - sibling `Resource` folder used alongside this selector.
- [`../../README.md`](../../README.md) - grandparent `Contents` grouping folder.
- [`../../../README.md`](../../../README.md) - `Nodes` folder (the 3.0-native `ISelector` family lives in `Shared/Selectors/`, out of this folder's scope).
- [`../../../../README.md`](../../../../README.md) - repository/docs top-level documentation.

[↑ Back to top](#contents)
