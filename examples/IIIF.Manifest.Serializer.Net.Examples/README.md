# IIIF.Manifest.Serializer.Net - Examples

This project contains practical examples demonstrating how to use the IIIF Manifest Serializer library to create IIIF Presentation API 2.0 compliant manifests.

## Running Examples

From the repository root:

```bash
dotnet run --project examples/IIIF.Manifest.Serializer.Net.Examples
```

Run a specific example:

```bash
dotnet run --project examples/IIIF.Manifest.Serializer.Net.Examples -- single
dotnet run --project examples/IIIF.Manifest.Serializer.Net.Examples -- book
dotnet run --project examples/IIIF.Manifest.Serializer.Net.Examples -- structure
dotnet run --project examples/IIIF.Manifest.Serializer.Net.Examples -- deserialize
dotnet run --project examples/IIIF.Manifest.Serializer.Net.Examples -- collection
dotnet run --project examples/IIIF.Manifest.Serializer.Net.Examples -- all
```

## Available Examples

### 1. Single Image Manifest (`SingleImageExample.cs`)

Creates a minimal manifest with one canvas and one image annotation.

**Demonstrates:**
- Basic manifest structure
- Canvas with dimensions
- Image resource with painting annotation
- Metadata (title, author, date)
- Attribution and description
- Thumbnails

**Use case:** Simple single-page documents, individual images

### 2. Multi-Page Book with Deep Zoom (`BookManifestExample.cs`)

Creates a book manifest with multiple pages and IIIF Image Service for zooming.

**Demonstrates:**
- Multiple canvases in a sequence
- IIIF Image Service integration
- Tile specifications for deep zoom
- Right-to-left viewing direction
- License and thumbnail metadata
- Page thumbnails for navigation

**Use case:** Books, manuscripts, multi-page documents with zoom capability

### 3. Manifest with Hierarchical Structure (`StructuredManifestExample.cs`)

Creates a poetry anthology with table of contents using Structure/Range.

**Demonstrates:**
- Hierarchical navigation structures
- Multiple ranges (parts, poems)
- Nested structures
- StartCanvas for each section
- Canvas references in ranges
- Range-to-range relationships

**Use case:** Books with chapters, multi-section documents, anthologies

### 4. Deserialize and Modify (`DeserializeExample.cs`)

Shows how to load, modify, and re-serialize existing IIIF manifests.

**Demonstrates:**
- Deserializing JSON to Manifest objects
- Modifying existing manifests
- Adding metadata to loaded manifests
- Round-trip conversion
- Working with existing IIIF resources

**Use case:** Editing existing manifests, migration, enrichment

### 5. Collection with Viewing Hints (`CollectionExample.cs`)

Creates a collection hierarchy with multiple manifests and viewing hints.

**Demonstrates:**
- Collection resource type
- Nested collections
- Manifest references
- ViewingHint enum (paged, continuous, individuals, multi-part, top, facing-pages)
- Collection metadata
- Total count for paging
- Embedded manifest example

**Use case:** Digital libraries, institutional repositories, curated collections

## Example Output

Each example outputs formatted JSON that can be:

- Saved to a `.json` file
- Loaded in IIIF viewers (Mirador, Universal Viewer, etc.)
- Validated against IIIF Presentation API 2.0
- Served from a web server for testing

## Key Concepts Covered

### IIIF Resource Hierarchy
```
Collection (top-level organization)
  └─ Manifest (single object)
       └─ Sequence (viewing order)
            └─ Canvas (painting surface)
                 └─ Image/Content (annotations)
                      └─ ImageResource (with optional Service)
```

### Viewing Hints

- **paged** - Each canvas is a separate page (books)
- **continuous** - Scroll continuously (scrolls)
- **individuals** - Separate unrelated items (collections)
- **facing-pages** - Show two canvases side-by-side
- **non-paged** - Canvas not displayed by default
- **top** - First canvas shown alone (cover)
- **multi-part** - Split across multiple resources

### Viewing Direction

- **left-to-right** - Western books (default)
- **right-to-left** - Arabic/Hebrew manuscripts
- **top-to-bottom** - Scrolls
- **bottom-to-top** - Rare usage

### IIIF Image Service

Enables deep zoom and derivatives:
- **Profile** - Capability level (level0, level1, level2)
- **Tiles** - Tile sizes and scale factors
- **Dimensions** - Image width/height
- **Context** - IIIF Image API version

## Extending Examples

To create your own example:

1. Create a new file in `Examples/` folder
2. Create a static class with a `Run()` method
3. Add your example logic
4. Update `Program.cs` to include it in the menu
5. Document what it demonstrates

Example template:

```csharp
public static class MyExample
{
    public static void Run()
    {
        Console.WriteLine("=== My Example ===\n");
        
        // Create your manifest
        var manifest = new Manifest("https://example.org/manifest", new Label("Title"));
        
        // ... build your structure ...
        
        var json = JsonConvert.SerializeObject(manifest, Formatting.Indented);
        Console.WriteLine(json);
        
        Console.WriteLine("\n=== End of Example ===\n");
    }
}
```

## Resources

- [IIIF Presentation API 2.0 Specification](https://iiif.io/api/presentation/2.0/)
- [IIIF Image API](https://iiif.io/api/image/)
- [IIIF Cookbook](https://iiif.io/api/cookbook/)
- [IIIF Awesome List](https://github.com/IIIF/awesome-iiif)

