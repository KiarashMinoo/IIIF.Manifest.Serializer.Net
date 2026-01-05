# IIIF.Manifest.Serializer.Net

A comprehensive .NET library for serializing IIIF Presentation API 2.0 manifests using Newtonsoft.Json.

## Overview

The IIIF (International Image Interoperability Framework) standard defines two APIs which allow access to digital images and metadata. IIIF defines a protocol to allow interoperability between digital repositories. The National Library of Wales (NLW) has implemented both the Presentation API (metadata) and Image API (digital images) that have allowed us to collaborate in a number of projects that would have otherwise been more difficult or not possible without this public API.

**Key Benefits:**
- International Image Interoperability Framework
- Exposure and access to digital images and metadata
- Interoperability between digital repositories
- Plug-and-play software
- Promotes reusability

## Documentation

Complete production-quality documentation is available under [`/docs`](docs/README.md). The catalog below links to key areas and subfolders.

### Core Areas

- **[Helpers](docs/Helpers/README.md)** `Types:4` `Files:4` `Diagrams:✓` – Utility methods for collections, JSON parsing, date handling, manifest metadata
- **[Nodes](docs/Nodes/README.md)** `Types:4` `Files:0` `Diagrams:✓` – IIIF domain model (Manifest, Sequence, Canvas, Structure, annotations)
  - [Canvas](docs/Nodes/Canvas/README.md) `Types:2` `Files:2` `Diagrams:✓`
  - [Manifest](docs/Nodes/Manifest/README.md) `Types:2` `Files:2` `Diagrams:✓`
  - [Sequence](docs/Nodes/Sequence/README.md) `Types:2` `Files:2` `Diagrams:✓`
  - [Structure](docs/Nodes/Structure/README.md) `Types:2` `Files:2` `Diagrams:✓`
  - [Content](docs/Nodes/Content/README.md) `Types:8` `Files:0` `Diagrams:✓` – Image, Embedded, Segment, OtherContent
- **[Properties](docs/Properties/README.md)** `Types:9` `Files:9` `Diagrams:✓` – IIIF metadata (Label, Description, Metadata, Service, Tile, etc.)
  - [Description](docs/Properties/Description/README.md) `Types:2` `Files:2` `Diagrams:✓`
  - [Metadata](docs/Properties/Metadata/README.md) `Types:2` `Files:2` `Diagrams:✓`
  - [Service](docs/Properties/Service/README.md) `Types:2` `Files:2` `Diagrams:✓`
  - [Tile](docs/Properties/Tile/README.md) `Types:2` `Files:2` `Diagrams:✓`
- **[Shared](docs/Shared/README.md)** `Types:10` `Files:1` `Diagrams:✓` – Foundation types (TrackableObject, BaseItem, BaseNode, BaseContent hierarchies)
  - [Trackable](docs/Shared/Trackable/README.md) `Types:3` `Files:3` `Diagrams:✓`
  - [BaseItem](docs/Shared/BaseItem/README.md) `Types:2` `Files:2` `Diagrams:✓`
  - [BaseNode](docs/Shared/BaseNode/README.md) `Types:2` `Files:2` `Diagrams:✓`
  - [Content](docs/Shared/Content/README.md) `Types:4` `Files:2` `Diagrams:✓`

**Last generated:** January 2026

## Getting Started

### Installation

```powershell
dotnet add package IIIF.Manifest.Serializer.Net
```

### Quick Example

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

## Building

```powershell
# Restore dependencies
dotnet restore

# Build library
dotnet build -c Release

# Run sample/benchmark
dotnet run --project src/IIIF.Manifest.Serializer.Net.Test
```

## Target Frameworks

- **netstandard2.0** – Broad compatibility across modern .NET
- **net451** – Legacy .NET Framework support

## Key Features

1. **IIIF Presentation API 2.0 Compliance** – Full spec implementation with validation
2. **Immutable Properties** – All mutations through fluent SetPropertyValue pattern
3. **Change Tracking** – ModifiedProperties dictionary for dirty field detection
4. **Single-or-Array Serialization** – Converters emit arrays only when count > 1
5. **Multi-Language Support** – Description, MetadataValue with @value/@language
6. **IIIF Image Service** – Deep-zoom via Service/Tile with scaleFactors
7. **Flexible Annotations** – Image painting, embedded text, segment regions
8. **Hierarchical Navigation** – Structure/Range with canvas/range collections

## Architecture

- **TrackableObject Pattern**: All domain types track property changes via reflection-based SetPropertyValue
- **Custom JsonConverters**: Enforce IIIF validation (@context/@id/@type requirements)
- **Fluent API**: Chainable Set*/Add*/Remove* methods for immutable updates
- **Type Hierarchy**: TrackableObject → BaseItem → BaseNode → Manifest/Canvas/etc.

See [architecture documentation](docs/README.md#architecture-overview) for details.

## License

Apache-2.0

## Contributing

When extending the library:

1. **Inherit from appropriate base** – TrackableObject → BaseItem → BaseNode → domain type
2. **Use SetPropertyValue** – All mutations through helper to track ModifiedProperties
3. **Create custom JsonConverter** – Validate required tokens, parse/write fields
4. **Follow fluent pattern** – Return `this` from all setters for chaining
5. **Update documentation** – Maintain README consistency across all folders

See the [documentation standards](docs/README.md#documentation-standards) for more details