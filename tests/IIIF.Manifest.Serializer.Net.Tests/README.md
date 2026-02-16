# IIIF.Manifest.Serializer.Net - Tests

This project contains comprehensive unit tests for the IIIF Manifest Serializer library using xUnit and FluentAssertions.

## Running Tests

From the repository root:

```bash
dotnet test
```

Or specifically for this test project:

```bash
dotnet test tests/IIIF.Manifest.Serializer.Net.Tests/IIIF.Manifest.Serializer.Net.Tests.csproj
```

## Test Structure

### Nodes Tests

- **ManifestTests.cs** - Tests for top-level Manifest serialization/deserialization
  - Single canvas manifests
  - Metadata handling
  - ViewingDirection support
  - NavDate parsing
  - Multiple labels

- **CanvasTests.cs** - Tests for Canvas node
  - Required properties (id, label, dimensions)
  - Image content
  - Multiple images (recto/verso)
  - Thumbnail support

- **SequenceTests.cs** - Tests for Sequence node
  - Canvas ordering
  - StartCanvas references
  - ViewingDirection overrides

- **StructureTests.cs** - Tests for Structure/Range navigation
  - Canvas references
  - Nested ranges
  - StartCanvas in structures
  - Mixed canvases and ranges

- **CollectionTests.cs** - Tests for Collection resource
  - Manifest references
  - Nested collections
  - Total count
  - ViewingDirection

### Properties Tests

- **ServiceTests.cs** - Tests for IIIF Image Service
  - Basic properties (id, profile)
  - Dimensions
  - Tile specifications
  - Service attachment to ImageResource

- **ViewingHintTests.cs** - Tests for ViewingHint enum
  - All hint values (paged, continuous, individuals, facing-pages, non-paged, top, multi-part)
  - Serialization format
  - Deserialization from JSON
  - Round-trip conversion

## Test Coverage

The tests cover:

- ✅ Serialization to JSON
- ✅ Deserialization from JSON
- ✅ Round-trip (serialize → deserialize → compare)
- ✅ Required field validation
- ✅ Optional field handling
- ✅ Enum value conversion
- ✅ Collection properties
- ✅ Fluent API chaining
- ✅ IIIF Presentation API 2.0 compliance

## Technologies

- **xUnit** - Testing framework
- **FluentAssertions** - Fluent assertion library for readable tests
- **Newtonsoft.Json** - JSON serialization (same as main library)

## Adding New Tests

When adding new features to the library:

1. Create a new test file in the appropriate folder (Nodes/ or Properties/)
2. Follow the naming convention: `{ClassName}Tests.cs`
3. Use FluentAssertions for readable assertions
4. Test both serialization and deserialization
5. Include edge cases and error conditions

