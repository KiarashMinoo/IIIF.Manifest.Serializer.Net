# IIIF.Manifest.Serializer.Net Documentation

Complete production-quality documentation for the IIIF Presentation API 2.0 manifest serialization library.

## Documentation Structure

All 27 README files have been created following the comprehensive template with:
- ✅ Anchor-linked table of contents
- ✅ 2-5 sentence overview with IIIF context
- ✅ Files table (file | types | LOC | responsibility)
- ✅ Types & Members table
- ✅ Per-type details
- ✅ Mermaid diagrams (class hierarchies, sequence flows)
- ✅ Realistic IIIF examples with JSON output
- ✅ Cross-linked See Also sections

## Documentation Index

### Core Folders

1. **[Helpers](Helpers/README.md)** – Utility methods for collections, JSON parsing, date handling, manifest metadata
2. **[Shared](Shared/README.md)** – Foundation types: TrackableObject, BaseItem, BaseNode, BaseContent hierarchies
3. **[Nodes](Nodes/README.md)** – IIIF domain model: Manifest, Sequence, Canvas, Structure, annotations
4. **[Properties](Properties/README.md)** – IIIF metadata properties: Label, Description, Metadata, Service, etc.

### Shared Foundation (8 READMEs)

- **[Shared/Trackable](Shared/Trackable/README.md)** – TrackableObject pattern with ModifiedProperties tracking
- **[Shared/BaseItem](Shared/BaseItem/README.md)** – @context/@id/@type/service foundation
- **[Shared/BaseNode](Shared/BaseNode/README.md)** – 15+ metadata fields (label, description, thumbnail, etc.)
- **[Shared/Content](Shared/Content/README.md)** – BaseContent hierarchy for annotations
- **[Shared/Content/Resources](Shared/Content/Resources/README.md)** – BaseResource foundation for content resources
- **[Shared/Exceptions](Shared/Exceptions/README.md)** – Custom JSON validation exceptions
- **[Shared/FormatableItem](Shared/FormatableItem/README.md)** – Items with format field
- **[Shared/ValuableItem](Shared/ValuableItem/README.md)** – Simple string property wrappers

### Nodes Domain Model (6 READMEs)

- **[Nodes/Manifest](Nodes/Manifest/README.md)** – Top-level IIIF resource with sequences, structures
- **[Nodes/Sequence](Nodes/Sequence/README.md)** – Ordered canvas collection with viewing direction
- **[Nodes/Canvas](Nodes/Canvas/README.md)** – Painting surface with dimensions, images
- **[Nodes/Structure](Nodes/Structure/README.md)** – Hierarchical navigation (ranges)
- **[Nodes/Content](Nodes/Content/README.md)** – Annotation types overview

### Content Annotations (8 READMEs)

- **[Nodes/Content/Image](Nodes/Content/Image/README.md)** – Painting annotations linking ImageResource
- **[Nodes/Content/Image/Resource](Nodes/Content/Image/Resource/README.md)** – Image resources with IIIF Image Service
- **[Nodes/Content/Embedded](Nodes/Content/Embedded/README.md)** – Embedded text annotations
- **[Nodes/Content/Embedded/Resource](Nodes/Content/Embedded/Resource/README.md)** – Text resources with chars/language
- **[Nodes/Content/Segment](Nodes/Content/Segment/README.md)** – Region annotations with selectors
- **[Nodes/Content/Segment/Resource](Nodes/Content/Segment/Resource/README.md)** – Segment resources
- **[Nodes/Content/Segment/Selector](Nodes/Content/Segment/Selector/README.md)** – xywh region selectors
- **[Nodes/Content/OtherContent](Nodes/Content/OtherContent/README.md)** – External annotation list links

### Properties Metadata (9 READMEs)

- **[Properties/Description](Properties/Description/README.md)** – Multi-language descriptions
- **[Properties/Metadata](Properties/Metadata/README.md)** – Label-value metadata pairs
- **[Properties/Metadata/MetadataValue](Properties/Metadata/MetadataValue/README.md)** – Individual metadata values
- **[Properties/Rendering](Properties/Rendering/README.md)** – Alternative format links (PDF, EPUB)
- **[Properties/Service](Properties/Service/README.md)** – IIIF Image API service descriptors
- **[Properties/Tile](Properties/Tile/README.md)** – Deep-zoom tile specifications
- **[Properties/Within](Properties/Within/README.md)** – Parent collection links
- **[Properties/Interfaces](Properties/Interfaces/README.md)** – IDimenssionSupport, IViewingDirectionSupport

## Architecture Overview

### TrackableObject Pattern

All domain types inherit from `TrackableObject<T>` which provides:
- **SetPropertyValue**: Reflection-based immutable property mutation
- **ModifiedProperties**: Dictionary tracking changed fields
- **INotifyPropertyChanged/INotifyPropertyChanging**: Change notification events

```csharp
manifest.AddLabel(new Label("Title"))
        .SetThumbnail(thumbnail)
        .AddSequence(sequence); // Fluent chaining
```

### JsonConverter Hierarchy

Custom converters enforce IIIF validation:
- **TrackableObjectJsonConverter** → clears ModifiedProperties, forces indented formatting
- **BaseItemJsonConverter** → validates @context/@id/@type required fields
- **BaseNodeJsonConverter** → parses 15+ metadata fields
- Domain converters → enforce resource/on/dimension requirements

### IIIF Domain Model

```
Manifest (top-level)
  └─ Sequence (ordered views)
       └─ Canvas (painting surface with dimensions)
            ├─ Image (painting annotation)
            │    └─ ImageResource (with Service for deep-zoom)
            ├─ EmbeddedContent (text annotations)
            ├─ Segment (region annotations with Selector)
            └─ OtherContent (external annotation lists)
```

## Key Features

1. **IIIF Presentation API 2.0 Compliance**: Full spec implementation with validation
2. **Immutable Properties**: All mutations through fluent SetPropertyValue pattern
3. **Change Tracking**: ModifiedProperties dictionary for dirty field detection
4. **Single-or-Array Serialization**: Converters emit arrays only when count > 1
5. **Multi-Language Support**: Description, MetadataValue with @value/@language
6. **IIIF Image Service**: Deep-zoom via Service/Tile with scaleFactors
7. **Flexible Annotations**: Image painting, embedded text, segment regions
8. **Hierarchical Navigation**: Structure/Range with canvas/range collections

## Quick Start Example

```csharp
using IIIF.Manifest.Serializer.Net;

// Create manifest
var manifest = new Manifest(
    "https://example.org/manifest",
    new Label("16th Century Manuscript")
).SetViewingDirection(ViewingDirection.RightToLeft);

// Add sequence with canvas
var sequence = new Sequence("https://example.org/seq1");
var canvas = new Canvas(
    "https://example.org/canvas/1",
    new Label("Folio 1r"),
    2000,  // height
    1500   // width
);

// Add image with IIIF Image Service
var imageResource = new ImageResource(
    "https://example.org/image.jpg",
    "image/jpeg"
).SetHeight(2000).SetWidth(1500);

var service = new Service(
    "http://iiif.io/api/image/2/context.json",
    "https://example.org/iiif/img1",
    "http://iiif.io/api/image/2/level1.json"
).AddTile(new Tile(512, new[] { 1, 2, 4, 8 }));

imageResource.SetService(service);

var image = new Image(
    "https://example.org/anno/1",
    imageResource,
    canvas.Id
);

canvas.AddImage(image);
sequence.AddCanvas(canvas);
manifest.AddSequence(sequence);

// Serialize to JSON
string json = JsonConvert.SerializeObject(manifest);
```

## Documentation Standards

Each README includes:

1. **Anchor TOC** – Quick navigation to all sections
2. **Overview** – 2-5 sentences explaining purpose and IIIF context
3. **Files Table** – File paths, primary types, LOC, responsibilities
4. **Types & Members Table** – Type, kind, summary, inheritance, key members
5. **Per-Type Details** – Focused explanations of each type's role
6. **Mermaid Diagrams** – Class hierarchies and sequence flows
7. **Examples** – Realistic C# code with JSON output
8. **See Also** – Cross-links to parent/sibling/related documentation

## Target Frameworks

- **netstandard2.0** – Broad compatibility
- **net451** – Legacy .NET Framework support

## Dependencies

- **Newtonsoft.Json** – JSON serialization with custom converters

## Build & Test

```powershell
# Build library
dotnet build IIIF.Manifest.Serializer.Net.sln

# Run sample/benchmark
dotnet run --project src/IIIF.Manifest.Serializer.Net.Test
```

## Contributing

When extending the library:

1. **Inherit from appropriate base** – TrackableObject → BaseItem → BaseNode → domain type
2. **Use SetPropertyValue** – All mutations through helper to track ModifiedProperties
3. **Create custom JsonConverter** – Validate required tokens, parse/write fields
4. **Follow fluent pattern** – Return `this` from all setters for chaining
5. **Update documentation** – Maintain README consistency across all folders

## License

See [LICENSE](../LICENSE) file in repository root.

---

**Documentation Generated**: Complete production-quality documentation set for IIIF.Manifest.Serializer.Net  
**Total README Files**: 27  
**Coverage**: All folders with comprehensive sections, diagrams, examples, cross-links
