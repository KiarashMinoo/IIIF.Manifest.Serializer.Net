# IIIF SDK Versioning Guide

This guide defines the first SDK-oriented implementation plan for the library.
The goal is to let users work with current IIIF Presentation API features while
still reading legacy manifests.

## Package Versioning

The SDK/package version is the source of truth in `Directory.Build.props`.
SDK feature work should advance the minor component only, for example `1.5.0` to `1.6.0`.
Patch changes should be reserved for bug fixes that do not change SDK behavior, and major changes should be reserved for breaking public API changes.
## Goals

- Read IIIF Presentation API 2.0, 2.1, and 3.0 manifests.
- Prefer the latest supported feature model in public APIs.
- Save manifests as the IIIF Presentation API version requested by the user.
- Preserve unknown and extension properties where practical.
- Keep obsolete IIIF properties readable for legacy documents.
- Prevent obsolete IIIF properties from being the preferred write path.

## Supported Versions

| Version | Read | Write | Notes |
| --- | --- | --- | --- |
| Presentation API 2.0 | Yes | Yes | Legacy Shared Canvas model. |
| Presentation API 2.1 | Yes | Yes | Default legacy write target. |
| Presentation API 3.0 | Yes | Yes | Preferred write target. |
| Presentation API 1.0 | Best effort | No | Legacy read may be added after v2/v3 are stable. |

## Public SDK Shape

The primary entry point should be version-aware:

```csharp
var manifest = IiifSerializer.Deserialize(json);

var v2Json = IiifSerializer.Serialize(
    manifest,
    new IiifSerializerOptions(IiifPresentationVersion.V2_1));

var v3Json = IiifSerializer.Serialize(
    manifest,
    new IiifSerializerOptions(IiifPresentationVersion.V3_0));
```

Version detection should inspect, in order:

- `@context`
- `id` / `@id`
- `type` / `@type`
- `items`
- `sequences`

## Model Strategy

The public model should move toward latest IIIF concepts:

- `id` and `type`
- `items`
- `behavior`
- `summary`
- `rights`
- `requiredStatement`
- Web Annotation `body` and `target`

Legacy members remain readable:

- `@id`
- `@type`
- `sequences`
- `canvases`
- `images`
- `resource`
- `on`
- `viewingHint`
- `attribution`
- `license`

Obsolete members should not be the normal write API. They may remain as read-only
properties populated by deserialization and conversion.

## Serialization Strategy

Attributes such as `PresentationAPIAttribute` describe compatibility, but they
are not enough for serialization because IIIF 2.x and 3.0 have different JSON
structures.

The SDK needs explicit version mappers:

- v2 writer: `Manifest -> sequences -> canvases -> images -> resource/on`
- v3 writer: `Manifest -> items(Canvas) -> items(AnnotationPage) -> items(Annotation) -> body/target`

The mapper must also translate common renamed properties:

| IIIF 2.x | IIIF 3.0 |
| --- | --- |
| `@id` | `id` |
| `@type` | `type` |
| `viewingHint` | `behavior` |
| `license` | `rights` |
| `attribution` | `requiredStatement` |
| `sequences[0].canvases` | `items` |
| `canvas.images` | `canvas.items[0].items` |
| `annotation.resource` | `annotation.body` |
| `annotation.on` | `annotation.target` |

## Testing Requirements

Each implementation step must include tests. Coverage should include:

- Version detection from contexts and structural fallbacks.
- Default serializer options.
- v2 serialization keeps legacy keys.
- v3 serialization emits latest keys.
- v3 serialization does not emit v2-only keys.
- Legacy v2 JSON can be read.
- Simple v3 JSON can be read or normalized.
- Obsolete properties are readable but not part of the preferred write API.
- Invalid or empty JSON fails predictably.

## Milestones

1. Add version enum, serializer options, and version detection.
2. Add `IiifSerializer` as the public SDK entry point.
3. Add v2 output path using the existing serializer.
4. Add v3 output path for simple manifests and image annotations.
5. Add v3 read normalization for simple manifests.
6. Make obsolete write APIs internal or remove them from the preferred public API.
7. Expand conversion coverage for audio, video, structures/ranges, services, and extensions.

