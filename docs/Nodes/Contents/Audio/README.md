# Audio

## Contents

- [Overview](#overview)
- [Files](#files)
- [Types & Members](#types--members)
- [Diagrams](#diagrams)
- [Package Dependencies](#package-dependencies)
- [See Also](#see-also)

## Overview

This folder models the legacy (2.x) `oa:Annotation` wrapper used to paint an audio resource onto a
`Canvas` - the pre-3.0 shape that `Canvas.Audios` (in `../../README.md`) presents as a computed
view over the 3.0-native `Items`/`Annotation` storage. It pairs with [`Resource`](Resource/README.md)
for the actual `AudioResource` body. In 3.0-native code, prefer constructing an `Annotation` whose
body is an `AudioResource` directly and calling `Canvas.AddAnnotation`, rather than constructing an
`Audio` wrapper by hand.

## Files

| File | Primary type(s) | LOC (approx) | Responsibility |
| --- | --- | --- | --- |
| `Audio.cs` | `Audio` | 32 | Legacy 2.x `oa:Annotation` wrapper pairing an `AudioResource` with a target canvas (`on`). |

## Types & Members

| Type | Kind | Summary | Inherits/Implements | Key Members |
| --- | --- | --- | --- | --- |
| `Audio` | class | Legacy audio-painting annotation wrapper | `BaseContent<Audio, AudioResource>` | `Motivation`, `On` |

### Audio

- **Kind / Namespace**: `class`, `IIIF.Manifests.Serializer.Nodes.Contents.Audio`.
- **Inherits**: `BaseContent<Audio, AudioResource>` (see `Shared/Content/BaseContent.cs`, which itself derives from `BaseNode<TBaseContent>` and adds `Format`/`Height`/`Width`/`Resource`).
- **Key properties**:
  - `Motivation : string` (`motivation`) - always `"sc:painting"` (set in the constructor).
  - `On : string` (`on`) - the id of the `Canvas` (or canvas region) this audio is painted onto.
  - Inherited from `BaseContent<Audio, AudioResource>`: `Resource : AudioResource` (`resource`), plus `Format`/`Height`/`Width` from `BaseContent<Audio>`.
- **Constructors**: `[JsonConstructor] Audio(string id, AudioResource resource, string on)`.
- **Usage Recipe** (legacy-shim construction; for new 3.0-native code, build an `Annotation` with an `AudioResource` body via `Canvas.AddAnnotation` instead - see `Canvas.ToAudio`/`ToAnnotation` internal mapping in `../../README.md`):
  ```csharp
  var audioResource = new AudioResource("https://example.org/audio/track1.mp3", "audio/mp3")
      .SetDuration(180.0);
  var audio = new Audio("https://example.org/anno/audio1", audioResource, canvas.Id);
  ```

[↑ Back to top](#contents)

## Diagrams

*Not applicable - single self-contained type (composition with `AudioResource` is documented in [`Resource/README.md`](Resource/README.md)).*

[↑ Back to top](#contents)

## Package Dependencies

| Package | Version | Description | Links |
| --- | --- | --- | --- |
| Newtonsoft.Json | 13.0.4 | JSON.NET - this SDK's serialization engine (custom JsonConverters, attribute-driven read/write) | [NuGet](https://www.nuget.org/packages/Newtonsoft.Json/13.0.4) |

[↑ Back to top](#contents)

## See Also

- [`Resource/README.md`](Resource/README.md) - the `AudioResource` body type this wrapper carries.
- [`../README.md`](../README.md) - parent `Contents` grouping folder.
- [`../../README.md`](../../README.md) - grandparent `Nodes` folder (`Canvas.Audios` computed view).
- [`../../../README.md`](../../../README.md) - repository/docs top-level documentation.

[↑ Back to top](#contents)
