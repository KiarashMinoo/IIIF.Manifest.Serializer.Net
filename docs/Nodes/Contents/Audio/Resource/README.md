# Resource

## Contents

- [Overview](#overview)
- [Files](#files)
- [Types & Members](#types--members)
- [Diagrams](#diagrams)
- [Package Dependencies](#package-dependencies)
- [See Also](#see-also)

## Overview

This folder models the IIIF Audio content resource used both as the legacy `Audio` wrapper's body
(see [`../README.md`](../README.md)) and as an `Annotation`'s body in 3.0-native code (`sound`
motivation content, e.g. cookbook recipe 0434-choice-av). It carries the A/V-specific `duration`
property alongside the format/label/service surface shared by every resource type.

## Files

| File | Primary type(s) | LOC (approx) | Responsibility |
| --- | --- | --- | --- |
| `AudioResource.cs` | `AudioResource` | 31 | The `sound`-type IIIF content resource; adds `duration` to the shared resource surface. |

## Types & Members

| Type | Kind | Summary | Inherits/Implements | Key Members |
| --- | --- | --- | --- | --- |
| `AudioResource` | class | Audio content resource | `BaseResource<AudioResource>` | `Duration`, `SetDuration` |

### AudioResource

- **Kind / Namespace**: `class`, `IIIF.Manifests.Serializer.Nodes.Contents.Audio.Resource`.
- **Inherits**: `BaseResource<AudioResource>` (`FormattableItem<T>` → `BaseItem<T>`; inherits `Format`, `Label`, `Service`, `Id`/`Type`/`Context`).
- **Key properties**: `Duration : double?` (`duration`) - length in seconds.
- **Key methods**: `SetDuration(double)` - fluent.
- **Constructors**: `AudioResource(string id, string format)` - sets `Type` to `ResourceType.Sound` and calls `SetFormat(format)`.
- **Usage Recipe**:
  ```csharp
  var audioResource = new AudioResource("https://example.org/audio/track1.mp3", "audio/mp3")
      .SetDuration(180.0)
      .SetLabel(new Label("English narration"));
  var annotation = new Annotation("https://example.org/anno/audio1", audioResource, canvas.Id)
      .SetMotivation("painting");
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

- [`../README.md`](../README.md) - parent `Audio` folder (the legacy `Audio` wrapper carrying this resource).
- [`../../README.md`](../../README.md) - grandparent `Contents` grouping folder.
- [`../../../README.md`](../../../README.md) - `Nodes` folder (`Canvas.AddAnnotation`).
- [`../../../../README.md`](../../../../README.md) - repository/docs top-level documentation.

[↑ Back to top](#contents)
