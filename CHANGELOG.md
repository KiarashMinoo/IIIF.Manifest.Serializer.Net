﻿﻿# Changelog

All notable changes to IIIF.Manifest.Serializer.Net will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.5.0] - 2026-02-16

### Added
- **IIIF Version Attributes** - Comprehensive version tracking for all classes and properties
  - **IIIFVersionAttribute** - Base attribute for API version specification
  - **PresentationAPIAttribute** - Marks Presentation API features with version ranges
  - **ImageAPIAttribute** - Marks Image API features with version ranges
  - **AuthAPIAttribute**, **SearchAPIAttribute**, **DiscoveryAPIAttribute**, **ContentStateAPIAttribute** - For future API support
  - Applied to all resource types, properties, and value objects
  - Indicates deprecated features with replacement suggestions
  - Specifies minimum/maximum supported versions
  - Includes notes about version-specific behavior

### Changed
- All resource classes now annotated with API version information
- All properties marked with their supported API versions
- Deprecated properties (e.g., `sequences`, `images`) clearly marked with 3.0 replacements
- Value objects annotated to show cross-version compatibility

### Benefits
- **Documentation** - Clear indication of which API version supports each feature
- **Migration Planning** - Deprecated features clearly marked with replacements
- **Tooling Support** - Attributes enable IDE warnings and analysis tools
- **Future-Proofing** - Prepared for IIIF Presentation API 3.0 migration

## [1.4.0] - 2026-02-16

### Added
- **Layer** resource type for annotation layers (IIIF Presentation API 2.0)
- **AnnotationList** resource type for annotation collections (IIIF Presentation API 2.0)
- **VideoResource** for video/moving image content (dctypes:MovingImage)
- **AudioResource** for audio/sound content (dctypes:Sound)
- **Canvas duration** property for A/V content support
- **Collection paging** - first, last, next, prev, startIndex properties
- **Collection members** property for mixed content
- **Structure ViewingDirection** support
- **Structure members** property for mixed range content

### Changed
- **Structure** now implements `IViewingDirectionSupport<Structure>`
- **Collection** supports full paging navigation per IIIF 2.0 spec
- **Canvas** supports duration for time-based media

### Technical Details
- All new types follow existing patterns (JsonConverter, BaseNode/BaseResource)
- Full serialization/deserialization support
- Backward compatible with existing code

## [1.2.0] - 2026-02-16

### Added
- **IIIF Image API 3.0 Support** - Complete implementation of Image API 3.0 properties
  - **Size** class for predefined image size options
  - **sizes** property - Array of available image sizes for client optimization
  - **maxWidth** property - Maximum width server will return
  - **maxHeight** property - Maximum height server will return
  - **maxArea** property - Maximum area (width × height) constraint
  - **rights** property - Rights statement URL
  - **preferredFormats** collection - Preferred image formats (webp, jpg, etc.)
  - **extraQualities** collection - Additional quality parameters beyond compliance level
  - **extraFeatures** collection - Additional features supported by server
- **ImageApi3Tests** - 7 comprehensive tests for all Image API 3.0 properties
- **SizeJsonConverter** - JSON converter for Size objects

### Changed
- **Service** class extended with 9 new properties (all optional, backward compatible)
- **ServiceJsonConverter** enhanced to handle Image API 3.0 properties
- **ServiceJsonConverter** now handles optional @type field for broader compatibility

### Technical Details
- Backward compatible with IIIF Image API 2.x
- Forward compatible with IIIF Image API 3.0
- All new properties are optional
- Fluent API maintained for all new properties
- Full test coverage (7/7 tests passing)

## [1.1.0] - 2026-02-16

### Added
- **Collection resource type** - Full support for IIIF Collection resource for organizing manifests
  - Nested collections support
  - Manifest references
  - Total count property
  - ViewingDirection support
- **ViewingHint class implementation** - Refactored from enum to class following ViewingDirection pattern
  - Static properties for all IIIF 2.0 viewing hints (Paged, Continuous, Individuals, FacingPages, NonPaged, Top, MultiPart)
  - Proper null handling instead of Unspecified enum value
  - Consistent with other property types using ValuableItem pattern
- **Comprehensive test project** - Unit tests using xUnit and FluentAssertions
  - ManifestTests - Top-level manifest serialization/deserialization
  - CanvasTests - Canvas node with images and metadata
  - SequenceTests - Canvas ordering and navigation
  - StructureTests - Hierarchical ranges and navigation
  - CollectionTests - Collection resource with nested collections
  - ServiceTests - IIIF Image Service with tiles
  - ViewingHintTests - All viewing hint values and serialization
  - 27+ test cases with comprehensive coverage
- **Example project** - Interactive console application with 5 practical examples
  - Single Image Manifest - Minimal manifest example
  - Multi-Page Book with Deep Zoom - IIIF Image Service integration
  - Hierarchical Structure - Table of contents with ranges
  - Deserialize and Modify - Load and edit existing manifests
  - Collection with Viewing Hints - Organize multiple manifests
- **Documentation**
  - Test project README with usage instructions
  - Example project README with detailed descriptions
  - Implementation summary document
  - ViewingHint implementation details
  - API versioning guidelines

### Changed
- **ViewingHint** now uses `null` instead of `Unspecified` enum value for cleaner null-checking
- **BaseNodeJsonConverter** updated to check for null ViewingHint
- Improved consistency across property serialization patterns
- Enhanced examples in main README

### Fixed
- ViewingHint serialization now properly omits null values from JSON
- Consistent null handling across all property types

### Technical Details
- Target frameworks: .NET Standard 2.0, .NET Standard 2.1, .NET Framework 4.5.1
- Test framework: xUnit with FluentAssertions
- Example project: .NET 8.0 console application
- Full IIIF Presentation API 2.0 compliance maintained

## [1.0.0] - Initial Release

### Added
- Core IIIF Presentation API 2.0 implementation
- Manifest, Sequence, Canvas, Structure nodes
- Image, EmbeddedContent, Segment, OtherContent annotations
- Full metadata support (Label, Description, Metadata, Attribution, etc.)
- IIIF Image Service support with tiles
- ViewingDirection support
- Custom JsonConverters for all types
- TrackableObject pattern for change tracking
- Fluent API for building manifests
- Comprehensive documentation

### Supported IIIF Resources
- Manifest (top-level resource)
- Sequence (canvas ordering)
- Canvas (painting surface)
- Range/Structure (navigation)
- Image annotations
- ImageResource with IIIF Image Service
- Service (IIIF Image API descriptor)
- Tiles (deep-zoom specifications)

### Supported Properties
- Label (multi-language)
- Metadata (structured key-value pairs)
- Description (multi-language text)
- Thumbnail, Attribution, License, Logo
- ViewingDirection (left-to-right, right-to-left, etc.)
- ViewingHint (as enum)
- NavDate
- Related, Rendering, SeeAlso, Within
- StartCanvas

### Technical Features
- JSON-LD @context, @id, @type support
- Single-or-array serialization
- Multi-language strings
- Required field validation
- Immutable properties with fluent API
- Change tracking via ModifiedProperties

---

## Version History

- **1.5.0** (2026-02-16) - IIIF version attributes for all classes and properties, deprecation tracking
- **1.4.0** (2026-02-16) - Layer, AnnotationList, VideoResource, AudioResource, Canvas duration, Collection paging
- **1.3.0** (2026-02-16) - Value objects for fluent API (ImageFormat, ImageQuality, ImageFeature, Behavior, Motivation, ResourceType, Profile, Context, Rights, Language, TimeMode)
- **1.2.0** (2026-02-16) - IIIF Image API 3.0 support, Size class, enhanced Service properties
- **1.1.0** (2026-02-16) - Collection support, enhanced ViewingHint, comprehensive tests and examples
- **1.0.0** (Initial) - Core IIIF Presentation API 2.0 implementation

## Upgrade Guide

### From 1.0.0 to 1.1.0

This is a **backward-compatible** update. No breaking changes.

#### ViewingHint Changes (Optional Migration)

If you were checking for `ViewingHint.Unspecified`, update to null checks:

**Before:**
```csharp
if (manifest.ViewingHint == ViewingHint.Unspecified) {
    // No hint set
}
```

**After:**
```csharp
if (manifest.ViewingHint == null) {
    // No hint set
}
```

The API remains otherwise identical. All existing code will continue to work.

#### New Features Available

```csharp
// New: Collection support
var collection = new Collection("https://example.org/collection", new Label("My Collection"));
collection.AddManifest("https://example.org/manifest/1");

// Enhanced: ViewingHint with static properties
manifest.SetViewingHint(ViewingHint.Paged);
canvas.SetViewingHint(ViewingHint.Top);
```

## Future Roadmap

### Planned for 2.0.0
- IIIF Presentation API 3.0 support
- Breaking changes for v3 data model
- Migration utilities from 2.0 to 3.0

### Under Consideration
- IIIF Auth API support
- Content Search API support
- Change Discovery API support
- Manifest validation utilities
- Performance optimizations

## References

- [Semantic Versioning 2.0.0](https://semver.org/spec/v2.0.0.html)
- [IIIF Presentation API 2.0](https://iiif.io/api/presentation/2.0/)
- [IIIF Versioning Notes](https://iiif.io/api/annex/notes/semver/)
