# IIIF API Implementation Summary

## Overview

This document summarizes the IIIF API implementation status for the IIIF.Manifest.Serializer.Net library based on analysis of the IIIF specifications.

## Supported IIIF APIs

### IIIF Presentation API 2.0/2.1 ✅ Full Support

| Resource Type | Status | File |
|--------------|--------|------|
| **Manifest** | ✅ Complete | `Nodes/Manifest/Manifest.cs` |
| **Collection** | ✅ Complete | `Nodes/Collection/Collection.cs` |
| **Sequence** | ✅ Complete | `Nodes/Sequence/Sequence.cs` |
| **Canvas** | ✅ Complete | `Nodes/Canvas/Canvas.cs` |
| **Range/Structure** | ✅ Complete | `Nodes/Structure/Structure.cs` |
| **Layer** | ✅ Complete | `Nodes/Layer/Layer.cs` |
| **AnnotationList** | ✅ Complete | `Nodes/AnnotationList/AnnotationList.cs` |
| **Image** | ✅ Complete | `Nodes/Content/Image/Image.cs` |
| **ImageResource** | ✅ Complete | `Nodes/Content/Image/Resource/ImageResource.cs` |
| **VideoResource** | ✅ Complete | `Nodes/Content/Video/Resource/VideoResource.cs` |
| **AudioResource** | ✅ Complete | `Nodes/Content/Audio/Resource/AudioResource.cs` |
| **OtherContent** | ✅ Complete | `Nodes/Content/OtherContent/OtherContent.cs` |
| **EmbeddedContent** | ✅ Complete | `Nodes/Content/Embedded/EmbeddedContent.cs` |
| **Segment** | ✅ Complete | `Nodes/Content/Segment/Segment.cs` |

### IIIF Image API 2.x/3.0 ✅ Service Support

| Feature | Status | Notes |
|---------|--------|-------|
| **Service** | ✅ Complete | Full info.json descriptor |
| **Tiles** | ✅ Complete | Tile specifications |
| **Sizes** | ✅ Complete | Available size options |
| **Profile** | ✅ Complete | Compliance levels |
| **maxWidth/maxHeight/maxArea** | ✅ Complete | Dimension constraints |
| **preferredFormats** | ✅ Complete | Format preferences |
| **extraQualities** | ✅ Complete | Quality options |
| **extraFeatures** | ✅ Complete | Feature flags |
| **rights** | ✅ Complete | Rights statement |

## Properties Support

### Descriptive Properties

| Property | Status | Notes |
|----------|--------|-------|
| `label` | ✅ Complete | Multi-language support |
| `description` | ✅ Complete | Multi-language support |
| `metadata` | ✅ Complete | Key-value pairs |
| `attribution` | ✅ Complete | Credit information |
| `license` | ✅ Complete | License URI |
| `logo` | ✅ Complete | Logo image |
| `thumbnail` | ✅ Complete | Thumbnail image |

### Technical Properties

| Property | Status | Notes |
|----------|--------|-------|
| `@context` | ✅ Complete | JSON-LD context |
| `@id` | ✅ Complete | Resource identifier |
| `@type` | ✅ Complete | Resource type |
| `format` | ✅ Complete | MIME type |
| `height` | ✅ Complete | Pixel height |
| `width` | ✅ Complete | Pixel width |
| `duration` | ✅ Complete | A/V duration |
| `viewingDirection` | ✅ Complete | View direction |
| `viewingHint` | ✅ Complete | Viewing hints |
| `navDate` | ✅ Complete | Navigation date |

### Linking Properties

| Property | Status | Notes |
|----------|--------|-------|
| `related` | ✅ Complete | Related resources |
| `rendering` | ✅ Complete | Alternative formats |
| `seeAlso` | ✅ Complete | External links |
| `within` | ✅ Complete | Parent reference |
| `service` | ✅ Complete | Service links |
| `startCanvas` | ✅ Complete | Start canvas ref |

### Paging Properties (Collections)

| Property | Status | Notes |
|----------|--------|-------|
| `first` | ✅ Complete | First page |
| `last` | ✅ Complete | Last page |
| `next` | ✅ Complete | Next page |
| `prev` | ✅ Complete | Previous page |
| `total` | ✅ Complete | Total count |
| `startIndex` | ✅ Complete | Start index |

## Value Objects

All value objects follow the `ValuableItem<T>` pattern for fluent API:

| Value Object | Static Properties | Custom Values |
|--------------|-------------------|---------------|
| `ViewingDirection` | Ltr, Rtl, Ttb, Btt | ✅ Supported |
| `ViewingHint` | Paged, Continuous, etc. | ✅ Supported |
| `ImageFormat` | Jpg, Png, Webp, etc. | ✅ Supported |
| `ImageQuality` | Default, Color, Gray, Bitonal | ✅ Supported |
| `ImageFeature` | RegionByPx, SizeByW, etc. | ✅ Supported |
| `Behavior` | Paged, AutoAdvance, etc. | ✅ Supported |
| `Motivation` | Painting, Commenting, etc. | ✅ Supported |
| `ResourceType` | Manifest, Canvas, etc. | ✅ Supported |
| `Profile` | Level0, Level1, Level2 | ✅ Supported |
| `Context` | Presentation2, Image3, etc. | ✅ Supported |
| `Rights` | CcBy, Cc0, etc. | ✅ Supported |
| `Language` | English, French, etc. | ✅ Supported |
| `TimeMode` | Trim, Scale, Loop | ✅ Supported |

## IIIF Extensions Support

### Approved Extensions ✅ Implemented

| Extension | Version | Status | Description |
|-----------|---------|--------|-------------|
| **Text Granularity** | 1.0 | ✅ Complete | OCR text segmentation levels |
| **navPlace** | 1.0 | ✅ Complete | Geographic location support |
| **Georeference** | 1.0 | ✅ Complete | Map georeferencing with GCPs |

### Text Granularity Extension

Provides text granularity levels for high-resolution text content:

| Granularity | Description | Use Case |
|-------------|-------------|----------|
| `Character` | Individual character level | OCR with character positioning |
| `Word` | Word-level segmentation | Word-based text search |
| `Line` | Line-level segmentation | Line-based text display |
| `Block` | Block-level segmentation | Paragraph/region text |
| `Page` | Page-level segmentation | Full page text |

### navPlace Extension

GeoJSON-LD geographic location support for IIIF resources:

| Feature | Description | Implementation |
|---------|-------------|----------------|
| **Point Geometry** | WGS84 coordinates | `Point` class with longitude/latitude |
| **Feature Collection** | GeoJSON FeatureCollection | `FeatureCollection` with features array |
| **Feature Properties** | Optional metadata | `FeatureProperties` with label support |
| **Resource Support** | Available on all resources | `BaseNode.NavPlace` property |

### Georeference Extension

Map georeferencing using Ground Control Points:

| Feature | Description | Implementation |
|---------|-------------|----------------|
| **Ground Control Points** | Image-to-world mapping | `GroundControlPoint` with image/world coordinates |
| **Transformations** | Coordinate transformations | `Transformation` with algorithm parameters |
| **CRS Support** | Coordinate reference systems | `Georeference.Crs` property |
| **Resource Support** | Available on all resources | `BaseNode.Georeference` property |

## Project Structure

```
src/IIIF.Manifest.Serializer.Net/
├── Attributes/              # IIIF extension attributes
│   ├── IIIFVersionAttribute.cs
│   ├── TextGranularityExtensionAttribute.cs
│   ├── NavPlaceExtensionAttribute.cs
│   └── GeoreferenceExtensionAttribute.cs
├── Nodes/
│   ├── AnnotationList/      # AnnotationList resource
│   ├── Canvas/              # Canvas resource
│   ├── Collection/          # Collection resource
│   ├── Content/
│   │   ├── Audio/           # Audio content
│   │   ├── Embedded/        # Embedded content
│   │   ├── Image/           # Image content
│   │   ├── OtherContent/    # Other content
│   │   ├── Segment/         # Segment content
│   │   └── Video/           # Video content
│   ├── Layer/               # Layer resource
│   ├── Manifest/            # Manifest resource
│   ├── Sequence/            # Sequence resource
│   └── Structure/           # Range/Structure resource
├── Properties/
│   ├── Behavior.cs          # Behavior value object
│   ├── Context.cs           # Context value object
│   ├── ImageFeature.cs      # ImageFeature value object
│   ├── ImageFormat.cs       # ImageFormat value object
│   ├── ImageQuality.cs      # ImageQuality value object
│   ├── Language.cs          # Language value object
│   ├── Motivation.cs        # Motivation value object
│   ├── Profile.cs           # Profile value object
│   ├── ResourceType.cs      # ResourceType value object
│   ├── Rights.cs            # Rights value object
│   ├── TimeMode.cs          # TimeMode value object
│   ├── ViewingDirection.cs  # ViewingDirection value object
│   ├── ViewingHint.cs       # ViewingHint value object
│   ├── TextGranularity.cs   # Text Granularity extension
│   ├── NavPlace.cs          # navPlace extension
│   ├── Georeference.cs      # Georeference extension
│   ├── Description/         # Description property
│   ├── Metadata/            # Metadata property
│   ├── Rendering/           # Rendering property
│   ├── Service/             # IIIF Image Service
│   ├── Size/                # Size property
│   ├── Tile/                # Tile property
│   └── Within/              # Within property
├── Shared/
│   ├── BaseItem/            # Base item with @id, @type
│   ├── BaseNode/            # Base node with metadata
│   ├── Content/             # Content base classes
│   ├── Exceptions/          # Custom exceptions
│   ├── FormatableItem/      # Format support
│   ├── Trackable/           # Change tracking
│   └── ValuableItem/        # Value object base
└── Helpers/
    ├── CollectionHelper.cs  # Collection utilities
    ├── DatetimeHelper.cs    # Date parsing
    ├── JsonHelper.cs        # JSON utilities
    └── ManifestHelper.cs    # Manifest utilities
```

## Version History

| Version | Features |
|---------|----------|
| **1.4.0** | Layer, AnnotationList, VideoResource, AudioResource, Canvas duration, Collection paging |
| **1.3.0** | Value objects (ImageFormat, ImageQuality, ImageFeature, Behavior, Motivation, etc.) |
| **1.2.0** | IIIF Image API 3.0 support (sizes, maxWidth, preferredFormats, etc.) |
| **1.1.0** | Collection, ViewingHint refactoring, comprehensive tests |
| **1.0.0** | Core IIIF Presentation API 2.0 implementation |

## Usage Examples

### Creating a Manifest

```csharp
var manifest = new Manifest("https://example.org/manifest", new Label("My Manifest"))
    .SetViewingDirection(ViewingDirection.Ltr)
    .SetViewingHint(ViewingHint.Paged);

var canvas = new Canvas("https://example.org/canvas/1", new Label("Page 1"), 1000, 800)
    .SetDuration(120.5); // For A/V content

var sequence = new Sequence("https://example.org/sequence")
    .AddCanvas(canvas);

manifest.AddSequence(sequence);
```

### Creating a Collection with Paging

```csharp
var collection = new Collection("https://example.org/collection", new Label("My Collection"))
    .SetTotal(1000)
    .SetFirst("https://example.org/collection?page=1")
    .SetLast("https://example.org/collection?page=100")
    .SetNext("https://example.org/collection?page=2")
    .SetStartIndex(0);
```

### Creating Video Content

```csharp
var videoResource = new VideoResource("https://example.org/video.mp4", "video/mp4")
    .SetWidth(1920)
    .SetHeight(1080)
    .SetDuration(3600.0);
```

### Creating an Image Service

```csharp
var service = new Service(Context.Image3.Value, "https://example.org/iiif/image", Profile.Level2.Value)
    .SetHeight(4000)
    .SetWidth(3000)
    .SetMaxWidth(2000)
    .AddPreferredFormat(ImageFormat.Webp)
    .AddExtraFeature(ImageFeature.RotationArbitrary)
    .SetRights(Rights.CcBy);
```

## Compliance

This library targets **IIIF Presentation API 2.0/2.1** with support for:
- All required and recommended properties
- All resource types
- Full JSON-LD serialization
- Round-trip serialization/deserialization
- **IIIF Approved Extensions:**
  - Text Granularity Extension 1.0
  - navPlace Extension 1.0
  - Georeference Extension 1.0

## Future Work

For IIIF Presentation API 3.0 support, a major version (2.0.0) is planned with:
- New data model (`items` instead of `sequences/canvases`)
- `AnnotationPage` and `Annotation` resources
- Updated property names and structures
- Migration utilities

## References

- [IIIF Presentation API 2.0](https://iiif.io/api/presentation/2.0/)
- [IIIF Presentation API 2.1](https://iiif.io/api/presentation/2.1/)
- [IIIF Image API 2.1](https://iiif.io/api/image/2.1/)
- [IIIF Image API 3.0](https://iiif.io/api/image/3.0/)
- [IIIF API Index](https://iiif.io/api/index.html)

