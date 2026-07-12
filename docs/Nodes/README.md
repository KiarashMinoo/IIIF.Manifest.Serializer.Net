# Nodes

## Contents

- [Overview](#overview)
- [Files](#files)
- [Types & Members](#types--members)
- [Diagrams](#diagrams)
- [Package Dependencies](#package-dependencies)
- [See Also](#see-also)

## Overview

`Nodes` holds the top-level and structural IIIF Presentation API resources: `Manifest` (a single
object), `Collection` (a list of Manifests/Collections), `Canvas` (a painting surface), `Structure`
(a Range/table-of-contents entry), and the minimal `CanvasReference`/`RangeReference` stubs used to
point at them by id. It also holds three legacy-only 2.x types (`Sequence`, `Layer`,
`AnnotationList`) that have no IIIF 3.0 equivalent and exist purely as read/write shims for
round-tripping old documents. Every type here is 3.0-native internally - `Items` is the real
backing storage for child content, while 2.x-only shapes (`Sequences`, `Canvas.Images`,
`Structure.Canvases`/`Ranges`, `Collection.Manifests`/`Collections`/`Members`) are computed,
read-only views derived from `Items`. The concrete annotation body/resource types these resources
paint onto a `Canvas` live one level down, in [`Contents`](Contents/README.md).

## Files

| File | Primary type(s) | LOC (approx) | Responsibility |
| --- | --- | --- | --- |
| `AnnotationList.cs` | `AnnotationList` | 64 | 2.x-only ordered list of annotations on a canvas; superseded by `AnnotationPage`. |
| `Canvas.cs` | `Canvas` | 230 | The painting surface resource; primary storage is `Items` (`AnnotationPage`/`Annotation`), with `Images`/`Audios`/`Videos`/`OtherContents` as computed legacy views. |
| `CanvasReference.cs` | `CanvasReference` | 17 | Minimal `{id,type:"Canvas"}` stub used inside a `Structure`'s `Items` when only the id is known. |
| `Collection.cs` | `Collection` | 297 | Top-level resource grouping `Manifest`/`Collection` references via `Items`; legacy `Manifests`/`Collections`/`Members`/paging are computed/read-only views. |
| `Layer.cs` | `Layer` | 46 | 2.x-only ordered list of `AnnotationList`s; superseded by `AnnotationCollection`. |
| `Manifest.cs` | `Manifest` | 285 | The top-level resource for a single object; `Items` holds `Canvas`, with `Sequences`/`Structures`/`Start`/top-level `Services` as its other major surfaces. |
| `RangeReference.cs` | `RangeReference` | 18 | Minimal `{id,type:"Range"}` stub used inside a `Structure`'s `Items` when only the id is known. |
| `Sequence.cs` | `Sequence` | 80 | 2.x-only ordered list of `Canvas`es; superseded by `Manifest.Items`. Kept as a legacy shim only, no interaction with 3.0-native storage beyond what `Manifest.Sequences` synthesizes. |
| `Structure.cs` | `Structure` | 240 | The Range resource (structural navigation / table of contents); `Items` holds nested `Structure`/`CanvasReference`/`RangeReference`, with `Canvases`/`Ranges`/`Members` as computed legacy views. |

## Types & Members

| Type | Kind | Summary | Inherits/Implements | Key Members |
| --- | --- | --- | --- | --- |
| `AnnotationList` | class | 2.x annotation list resource | `BaseNode<AnnotationList>` | `Resources`, `WithinLayer`, `AddResource`, `RemoveResource`, `SetWithinLayer` |
| `Canvas` | class | Painting surface | `BaseNode<Canvas>`, `IDimensionSupport<Canvas>` | `Height`, `Width`, `Duration`, `Images`/`Audios`/`Videos` (views), `Annotations`, `OtherContents` (view), `PlaceholderCanvas`, `AddAnnotation`, `AddAnnotationPageReference` |
| `CanvasReference` | class | Bare canvas-id stub | `BaseItem<CanvasReference>` | ctor `(string id)` |
| `Collection` | class | Manifest/Collection grouping resource | `BaseNode<Collection>`, `IViewingDirectionSupport<Collection>` | `ViewingDirection`, `Collections`/`Manifests`/`Members` (views), `Total`/`First`/`Last`/`Next`/`Prev`/`StartIndex`, `AddManifestReference` |
| `Layer` | class | 2.x annotation-list grouping resource | `BaseNode<Layer>` | `OtherContent`, `AddOtherContent`, `RemoveOtherContent` |
| `Manifest` | class | Top-level single-object resource | `BaseNode<Manifest>`, `IViewingDirectionSupport<Manifest>` | `Services`, `NavDate`, `ViewingDirection`, `Sequences` (view), `AdditionalSequences`, `Structures`, `Start`, `PlaceholderCanvas`, `AddTopLevelService`, `AddStructure` |
| `RangeReference` | class | Bare range-id stub | `BaseItem<RangeReference>` | ctor `(string id)` |
| `Sequence` | class | 2.x canvas-ordering resource | `BaseNode<Sequence>`, `IViewingDirectionSupport<Sequence>` | `Canvases`, `StartCanvas`, `ViewingDirection`, `AddCanvas`, `SetStartCanvas` |
| `Structure` | class | Range / table-of-contents entry | `BaseNode<Structure>`, `IViewingDirectionSupport<Structure>` | `Canvases`/`Ranges`/`Members` (views), `StartCanvas`, `ViewingDirection`, `AddCanvasReference`, `AddRangeReference` |

### AnnotationList

- **Kind / Namespace**: `class`, `IIIF.Manifests.Serializer.Nodes`.
- **Inherits**: `BaseNode<AnnotationList>` (via primary ctor `AnnotationList(string id)`).
- **Attributes**: `[PresentationAPI("2.0","2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "AnnotationPage")]` - the whole type is 2.x-only, no 3.0-native reshape (see `SDK_VERSIONING_GUIDE.md` §4/§6).
- **Key properties**:
  - `Resources : IReadOnlyCollection<IBaseResource>` (`resources`) - the annotations in this list.
  - `WithinLayer : string?` (`within`) - id of the parent `Layer`.
- **Key methods**: `AddResource<TResource>(TResource)`, `RemoveResource<TResource>(TResource)`, `SetWithinLayer(string)` - all fluent.
- **Constructors**: `AnnotationList(string id)`; `[JsonConstructor] AnnotationList(string id, Label label)`.
- **Usage Recipe** (legacy-shim construction, not the 3.0-preferred path - use `AnnotationPage` for new code):
  ```csharp
  var list = new AnnotationList("https://example.org/list/1", new Label("Page 1 annotations"))
      .SetWithinLayer("https://example.org/layer/1")
      .AddResource(new ImageResource("https://example.org/image.jpg", "image/jpeg"));
  ```

### Canvas

- **Kind / Namespace**: `class`, `IIIF.Manifests.Serializer.Nodes`.
- **Inherits/Implements**: `BaseNode<Canvas>`, `IDimensionSupport<Canvas>`.
- **Attributes**: `[PresentationAPI("2.0", Notes = "Core resource. In 3.0, images/otherContent replaced by items (AnnotationPage) / annotations.")]`.
- **Key properties**:
  - `Height`, `Width : int?` (`height`/`width`) - always present per spec.
  - `Duration : double?` (`duration`) - A/V support, added in 2.1, also valid in 3.0.
  - `Images : IReadOnlyCollection<Image>` (`images`) - **legacy view**, `[PresentationAPI(IsDeprecated = true, ReplacedBy = "items")]`. Computed by filtering `Items`' `AnnotationPage.Items` for `Annotation`s whose body is `ImageResource`. Superseded by `AddAnnotation`/`Items`.
  - `Audios`/`Videos : IReadOnlyCollection<Audio>/<Video>` - same computed-view pattern for audio/video bodies (not tagged `[PresentationAPI(IsDeprecated)]` themselves, but exist for the same 2.x-shim reason).
  - `Annotations : IReadOnlyCollection<AnnotationPage>` (`annotations`) - 3.0-native replacement for `otherContent`.
  - `OtherContents : IReadOnlyCollection<OtherContent>` (`otherContent`) - **legacy view** over `Annotations`, `[PresentationAPI(IsDeprecated = true, ReplacedBy = "annotations")]`.
  - `PlaceholderCanvas : Canvas?` (`placeholderCanvas`) - 3.0-only; a canvas shown before this one's content is ready (cookbook recipe 0013-placeholderCanvas).
- **Key methods**:
  - `AddAnnotation(Annotation)` - **the 3.0-preferred way** to paint content onto a Canvas; superseded predecessor of `AddImage`/`AddAudio`/`AddVideo`.
  - `AddAnnotationPageReference(AnnotationPage)` - 3.0 replacement for `AddOtherContent`.
  - `SetDuration(double)`, `SetPlaceholderCanvas(Canvas)`.
  - `[Obsolete(error: true)]` **AddImage**, **AddAudio**, **AddVideo**, **AddOtherContent** - all superseded by `AddAnnotation`/`AddAnnotationPageReference`.
- **Constructors**: `Canvas(string id, Label label, int height, int width)`.
- **Usage Recipe**:
  ```csharp
  var canvas = new Canvas("https://example.org/canvas/p1", new Label("Folio 1r"), height: 2000, width: 1500);
  var image = new ImageResource("https://example.org/full/full/0/default.jpg", "image/jpeg")
      .SetHeight(2000).SetWidth(1500);
  canvas.AddAnnotation(new Annotation("https://example.org/anno/p1", image, canvas.Id));
  ```

### CanvasReference

- **Kind / Namespace**: `class`, `IIIF.Manifests.Serializer.Nodes`.
- **Inherits**: `BaseItem<CanvasReference>`.
- **Key members**: `[JsonConstructor] CanvasReference(string id)` - sets `Type = "Canvas"`. No other members; a pure id+type stub.
- **Usage Recipe**:
  ```csharp
  structure.AddCanvasReference("https://example.org/canvas/p1"); // constructs a CanvasReference internally
  ```

### Collection

- **Kind / Namespace**: `class`, `IIIF.Manifests.Serializer.Nodes`.
- **Inherits/Implements**: `BaseNode<Collection>`, `IViewingDirectionSupport<Collection>`.
- **Attributes**: `[PresentationAPI("2.0", ...)]`; `[System.Text.Json.Serialization.JsonConverter(typeof(SystemTextJson.CollectionSystemTextJsonConverter))]` - bridges plain `System.Text.Json.JsonSerializer.Serialize/Deserialize` calls to this SDK's Newtonsoft-based logic.
- **Key properties**:
  - `ViewingDirection : ViewingDirection?` (`viewingDirection`).
  - `Collections : IReadOnlyCollection<Collection>` (`collections`) - **legacy view** over `Items.OfType<Collection>()`, `[PresentationAPI(IsDeprecated = true, ReplacedBy = "items")]`.
  - `Manifests : IReadOnlyCollection<string>` (`manifests`) - **legacy view**; each id backs a minimal internal `Manifest` stub since 2.x only ever carried the bare id.
  - `Members : IReadOnlyCollection<object>` (`members`) - **legacy view**, the closest 2.x analogue to `items` (mixed `Manifest`/`Collection`).
  - `Total`, `First`, `Last`, `Next`, `Prev`, `StartIndex` - 2.x-only paging properties, **not** tagged obsolete (no 3.0 replacement exists; paging was removed from the spec, so these are legacy-read-only, excluded from 3.0 output).
- **Key methods**:
  - `AddManifestReference(string manifestId)` - **non-obsolete** 3.0-preferred replacement for `AddManifest`, constructs a minimal internal `Manifest` stub.
  - `SetViewingDirection`, `SetTotal`, `SetFirst`, `SetLast`, `SetNext`, `SetPrev`, `SetStartIndex`.
  - `[Obsolete(error: true)]` **AddCollection**/**RemoveCollection**/**AddManifest**/**RemoveManifest**/**AddMember**/**RemoveMember** - all superseded by `AddItem`/`AddManifestReference`.
- **Constructors**: `[JsonConstructor] internal Collection(string id)`; `Collection(string id, Label label)`; `Collection(string id, IReadOnlyCollection<Label> labels)`.
- **Usage Recipe**:
  ```csharp
  var collection = new Collection("https://example.org/collection/top", new Label("Top-level Collection"))
      .SetViewingDirection(ViewingDirection.Ltr)
      .AddManifestReference("https://example.org/manifest/1");
  ```

### Layer

- **Kind / Namespace**: `class`, `IIIF.Manifests.Serializer.Nodes`.
- **Inherits**: `BaseNode<Layer>` (primary ctor `Layer(string id)`).
- **Attributes**: `[PresentationAPI("2.0","2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "AnnotationCollection")]` - whole type is legacy-only, no 3.0 reshape.
- **Key properties**: `OtherContent : IReadOnlyCollection<string>` (`otherContent`) - ids of member `AnnotationList`s.
- **Key methods**: `AddOtherContent(string)`, `RemoveOtherContent(string)`.
- **Constructors**: `Layer(string id)`; `Layer(string id, Label label)`.
- **Usage Recipe**:
  ```csharp
  var layer = new Layer("https://example.org/layer/1", new Label("Transcription layer"))
      .AddOtherContent("https://example.org/list/1");
  ```

### Manifest

- **Kind / Namespace**: `class`, `IIIF.Manifests.Serializer.Nodes`.
- **Inherits/Implements**: `BaseNode<Manifest>`, `IViewingDirectionSupport<Manifest>`.
- **Attributes**: `[PresentationAPI("2.0", ...)]`; `[System.Text.Json.Serialization.JsonConverter(typeof(SystemTextJson.ManifestSystemTextJsonConverter))]`.
- **Key properties**:
  - `Services : IReadOnlyCollection<IBaseService>` (`services`, `[JsonConverter(typeof(ObjectArrayJsonConverter))]`) - 3.0-only top-level centralized service array; distinct from the inherited inline `Service`. Read-support only for centralized documents; this SDK always writes inline (see `SDK_VERSIONING_GUIDE.md` §5).
  - `NavDate : DateTime?` (`navDate`).
  - `ViewingDirection : ViewingDirection?` (`viewingDirection`).
  - `Sequences : IReadOnlyCollection<Sequence>` (`sequences`) - **legacy view**, `[PresentationAPI(IsDeprecated = true, ReplacedBy = "items")]`; synthesizes a single `Sequence` from `Items.OfType<Canvas>()` + `ViewingDirection`/`Start`.
  - `AdditionalSequences : IReadOnlyCollection<Sequence>` (`[JsonIgnore]`) - legacy-only overflow for a multi-sequence 2.x document; no 3.0 equivalent, preserved verbatim rather than dropped.
  - `Structures : IReadOnlyCollection<Structure>` (`structures`) - unchanged property name across 2.x/3.0 per the confirmed spec mapping.
  - `Start : AnnotationTarget?` (`start`) - 3.0-only; reuses `AnnotationTarget` since "start" allows the same shapes as an Annotation target.
  - `PlaceholderCanvas : Canvas?` (`placeholderCanvas`) - 3.0-only.
- **Key methods**:
  - `SetSequenceId(string)` - legacy-JSON-shaping affordance only, **not** obsolete (no deprecated concept of its own).
  - `AddTopLevelService<TService>(TService)`/`RemoveTopLevelService<TService>(TService)` - distinct from inherited `AddService`/`RemoveService` (inline vs. centralized).
  - `AddStructure(Structure)`/`RemoveStructure(Structure)`.
  - `[Obsolete(error: true)]` **AddSequence**/**RemoveSequence** - superseded by `Items`/`AddItem`.
- **Constructors**: `[JsonConstructor] internal Manifest(string id)`; `Manifest(string id, IReadOnlyCollection<Label> labels)`; `Manifest(string id, Label label)`.
- **Usage Recipe**:
  ```csharp
  var manifest = new Manifest("https://example.org/manifest.json", new Label("16th Century Manuscript"))
      .SetViewingDirection(ViewingDirection.Ltr);
  manifest.AddItem(canvas); // canvas : Canvas
  string json = IiifSerializer.Serialize(manifest); // defaults to 3.0
  ```

### RangeReference

- **Kind / Namespace**: `class`, `IIIF.Manifests.Serializer.Nodes`.
- **Inherits**: `BaseItem<RangeReference>`.
- **Key members**: `[JsonConstructor] RangeReference(string id)` - sets `Type = "Range"`.
- **Usage Recipe**:
  ```csharp
  structure.AddRangeReference("https://example.org/range/2"); // constructs a RangeReference internally
  ```

### Sequence

- **Kind / Namespace**: `class`, `IIIF.Manifests.Serializer.Nodes`.
- **Inherits/Implements**: `BaseNode<Sequence>`, `IViewingDirectionSupport<Sequence>`.
- **Attributes**: `[PresentationAPI("2.0","2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", ReplacedBy = "Manifest.items")]` - whole type is legacy-only; kept purely as a read/write shim, no interaction with 3.0-native storage beyond what `Manifest.Sequences` constructs on the fly.
- **Key properties**:
  - `Canvases : IReadOnlyCollection<Canvas>` (`canvases`), `[PresentationAPI(IsDeprecated = true, ReplacedBy = "items")]`.
  - `StartCanvas : StartCanvas?` (`startCanvas`), `[PresentationAPI(IsDeprecated = true, ReplacedBy = "start")]`.
  - `ViewingDirection : ViewingDirection?` (`viewingDirection`).
- **Key methods**: `AddCanvas(Canvas)`, `RemoveCanvas(Canvas)`, `SetStartCanvas(StartCanvas)`, `SetViewingDirection`.
- **Constructors**: `[JsonConstructor] Sequence()` (defaults id to `string.Empty`); `Sequence(string id)`.
- **Usage Recipe** (legacy-shim construction only - for new code, add canvases directly to `Manifest.Items`):
  ```csharp
  var sequence = new Sequence("https://example.org/sequence/normal")
      .AddCanvas(canvas)
      .SetViewingDirection(ViewingDirection.Ltr);
  ```

### Structure

- **Kind / Namespace**: `class`, `IIIF.Manifests.Serializer.Nodes`.
- **Inherits/Implements**: `BaseNode<Structure>`, `IViewingDirectionSupport<Structure>`.
- **Attributes**: `[PresentationAPI("2.0", Notes = "Called 'structures' in 2.x, 'Range' in 3.0. Canvases/ranges arrays deprecated in 3.0.")]`; `[method: JsonConstructor]` on the primary ctor.
- **Key properties**:
  - `Canvases : IReadOnlyCollection<string>` (`canvases`) - **legacy view** over `Items.OfType<CanvasReference>()`, `[PresentationAPI(IsDeprecated = true, ReplacedBy = "items")]`.
  - `Ranges : IReadOnlyCollection<string>` (`ranges`) - **legacy view**; includes both bare `RangeReference` entries and the id of any nested `Structure`.
  - `Members : IReadOnlyCollection<object>` (`members`) - **legacy view** directly over `Items`.
  - `StartCanvas : string?` (`startCanvas`), `ViewingDirection : ViewingDirection?` (`viewingDirection`).
- **Key methods**:
  - `AddCanvasReference(string)`/`RemoveCanvasReference(string)` - 3.0-preferred replacement for `AddCanvas(string)`.
  - `AddRangeReference(string)`/`RemoveRangeReference(string)` - 3.0-preferred replacement for `AddRange(string)`; to embed a full nested `Structure` instead, use inherited `AddItem`.
  - `SetStartCanvas(string)`, `SetViewingDirection`.
  - `[Obsolete(error: true)]` **AddCanvas(string)**/**RemoveCanvas(string)**/**AddRange(string)**/**RemoveRange(string)**/**AddMember**/**RemoveMember**.
- **Constructors**: `Structure(string id)`; `Structure(string id, Label label)`.
- **Usage Recipe**:
  ```csharp
  var range = new Structure("https://example.org/range/1", new Label("Chapter 1"))
      .AddCanvasReference("https://example.org/canvas/p1")
      .AddCanvasReference("https://example.org/canvas/p2");
  manifest.AddStructure(range);
  ```

[↑ Back to top](#contents)

## Diagrams

```mermaid
classDiagram
    class TrackableObject~T~
    class BaseItem~T~
    class BaseNode~T~
    class Manifest
    class Collection
    class Canvas
    class Structure
    class Sequence
    class Layer
    class AnnotationList
    class CanvasReference
    class RangeReference

    TrackableObject~T~ <|-- BaseItem~T~
    BaseItem~T~ <|-- BaseNode~T~
    BaseNode~T~ <|-- Manifest
    BaseNode~T~ <|-- Collection
    BaseNode~T~ <|-- Canvas
    BaseNode~T~ <|-- Structure
    BaseNode~T~ <|-- Sequence
    BaseNode~T~ <|-- Layer
    BaseNode~T~ <|-- AnnotationList
    BaseItem~T~ <|-- CanvasReference
    BaseItem~T~ <|-- RangeReference

    Manifest --> Canvas : Items
    Manifest --> Structure : Structures
    Manifest ..> Sequence : Sequences (computed view)
    Collection --> Manifest : Items (stub)
    Collection --> Collection : Items (nested)
    Structure --> CanvasReference : Items
    Structure --> RangeReference : Items
    Structure --> Structure : Items (nested)
    Canvas ..> "Contents.Annotation.AnnotationPage" : Items
    Sequence --> Canvas : Canvases (legacy)
    Layer --> AnnotationList : OtherContent (id refs)
```

Every resource ultimately derives from `TrackableObject<T>` through `BaseItem<T>`/`BaseNode<T>`
(see [`../Shared/README.md`](../README.md) for those base types). `Manifest`/`Collection`/
`Structure` hold their children via the shared `Items` storage; the dotted edges mark computed
legacy views rather than real composition.

[↑ Back to top](#contents)

## Package Dependencies

| Package | Version | Description | Links |
| --- | --- | --- | --- |
| Newtonsoft.Json | 13.0.4 | JSON.NET - this SDK's serialization engine (custom JsonConverters, attribute-driven read/write) | [NuGet](https://www.nuget.org/packages/Newtonsoft.Json/13.0.4) |

[↑ Back to top](#contents)

## See Also

- [`Contents/README.md`](Contents/README.md) - the concrete annotation body/resource types these Nodes paint or reference.
- [`../README.md`](../README.md) - repository/docs top-level documentation.
- [`../SDK_VERSIONING_GUIDE.md`](../SDK_VERSIONING_GUIDE.md) - the authoritative reference for the 2.x↔3.0 reshape (`Sequence`'s fate in §6 Milestone 2, `Structure`/`Collection`/`Canvas` reshapes in §6 Milestones 1/3/4).

[↑ Back to top](#contents)
