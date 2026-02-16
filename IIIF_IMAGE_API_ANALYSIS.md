# IIIF Image API 3.0 Analysis & Implementation

## Overview

Based on https://iiif.io/api/image/3.0/

The IIIF Image API specifies a web service that returns an image in response to a standard HTTP(S) request. The URI can specify the region, size, rotation, quality characteristics and format of the requested image.

## Current Implementation Status

### ✅ Already Implemented (IIIF Image API 2.x compatible)

**Service Class** (`Properties/Service/Service.cs`):
- ✅ `@context` - API context URL
- ✅ `@id` / `id` - Base URI for the image
- ✅ `@type` / `type` - Resource type
- ✅ `profile` - Compliance level
- ✅ `width` - Full image width
- ✅ `height` - Full image height
- ✅ `tiles` - Array of tile size specifications

**Tile Class** (`Properties/Tile/Tile.cs`):
- ✅ `width` - Tile width in pixels
- ✅ `scaleFactors` - Array of scale factors

### ❌ Missing Properties for IIIF Image API 3.0

According to the IIIF Image API 3.0 specification, the following properties are defined in the `info.json` response but are missing from our Service implementation:

#### Image Information Properties

1. **sizes** - Array of available image sizes
   ```json
   "sizes": [
     { "width": 150, "height": 100 },
     { "width": 600, "height": 400 }
   ]
   ```

2. **maxWidth** - Maximum width in pixels that the server will return
   ```json
   "maxWidth": 2000
   ```

3. **maxHeight** - Maximum height in pixels that the server will return
   ```json
   "maxHeight": 1500
   ```

4. **maxArea** - Maximum area (width * height) in pixels
   ```json
   "maxArea": 10000000
   ```

#### Technical Properties (API 3.0)

5. **rights** - Rights statement URL (replaces license in some contexts)
   ```json
   "rights": "http://creativecommons.org/licenses/by/4.0/"
   ```

6. **partOf** - Reference to parent collection/manifest
   ```json
   "partOf": [
     {
       "id": "https://example.org/manifest",
       "type": "Manifest"
     }
   ]
   ```

7. **seeAlso** - Related resources
   ```json
   "seeAlso": [
     {
       "id": "https://example.org/metadata.xml",
       "type": "Dataset",
       "format": "text/xml"
     }
   ]
   ```

#### Optional Technical Properties

8. **preferredFormats** - Preferred image formats
   ```json
   "preferredFormats": ["webp", "jpg"]
   ```

9. **extraQualities** - Additional quality parameters supported
   ```json
   "extraQualities": ["bitonal", "gray"]
   ```

10. **extraFeatures** - Additional features supported beyond compliance level
    ```json
    "extraFeatures": [
      "regionByPx",
      "sizeByW",
      "rotationArbitrary"
    ]
    ```

## Recommended Implementation

### Priority 1: Core Image Information (High Priority)

These are commonly used properties that should be added:

1. **Size** class for sizes array
2. **maxWidth** property
3. **maxHeight** property  
4. **maxArea** property

### Priority 2: Rights & Links (Medium Priority)

5. **rights** property
6. **partOf** collection
7. **seeAlso** collection

### Priority 3: Technical Details (Lower Priority)

8. **preferredFormats** array
9. **extraQualities** array
10. **extraFeatures** array

## Implementation Plan

### 1. Create Size Class

Create `Properties/Size/Size.cs` to represent available image sizes:

```csharp
public class Size
{
    [JsonProperty("width")]
    public int Width { get; }
    
    [JsonProperty("height")]
    public int Height { get; }
    
    public Size(int width, int height)
    {
        Width = width;
        Height = height;
    }
}
```

### 2. Extend Service Class

Add new properties to `Properties/Service/Service.cs`:

```csharp
public class Service : BaseItem<Service>, IDimenssionSupport<Service>
{
    // Existing properties...
    
    // New properties for Image API 3.0
    private readonly List<Size> sizes = new List<Size>();
    private readonly List<string> preferredFormats = new List<string>();
    private readonly List<string> extraQualities = new List<string>();
    private readonly List<string> extraFeatures = new List<string>();
    
    [JsonProperty("sizes")]
    public IReadOnlyCollection<Size> Sizes => sizes.AsReadOnly();
    
    [JsonProperty("maxWidth")]
    public int? MaxWidth { get; private set; }
    
    [JsonProperty("maxHeight")]
    public int? MaxHeight { get; private set; }
    
    [JsonProperty("maxArea")]
    public long? MaxArea { get; private set; }
    
    [JsonProperty("rights")]
    public string Rights { get; private set; }
    
    [JsonProperty("preferredFormats")]
    public IReadOnlyCollection<string> PreferredFormats => preferredFormats.AsReadOnly();
    
    [JsonProperty("extraQualities")]
    public IReadOnlyCollection<string> ExtraQualities => extraQualities.AsReadOnly();
    
    [JsonProperty("extraFeatures")]
    public IReadOnlyCollection<string> ExtraFeatures => extraFeatures.AsReadOnly();
    
    // Fluent methods
    public Service AddSize(Size size) => SetPropertyValue(...);
    public Service SetMaxWidth(int maxWidth) => SetPropertyValue(a => a.MaxWidth, maxWidth);
    public Service SetMaxHeight(int maxHeight) => SetPropertyValue(a => a.MaxHeight, maxHeight);
    public Service SetMaxArea(long maxArea) => SetPropertyValue(a => a.MaxArea, maxArea);
    public Service SetRights(string rights) => SetPropertyValue(a => a.Rights, rights);
    public Service AddPreferredFormat(string format) => SetPropertyValue(...);
    public Service AddExtraQuality(string quality) => SetPropertyValue(...);
    public Service AddExtraFeature(string feature) => SetPropertyValue(...);
}
```

### 3. Update ServiceJsonConverter

Update serialization/deserialization logic in `Properties/Service/ServiceJsonConverter.cs` to handle new properties.

## API Version Compatibility

### IIIF Image API 2.x
- Uses `@context`, `@id`, `@type`
- Profile as string or object
- Tiles with scaleFactors

### IIIF Image API 3.0
- Uses `@context`, `id`, `type` (@ prefix optional)
- Profile as string
- Additional properties: sizes, maxWidth, maxHeight, maxArea
- Rights, partOf, seeAlso at image level
- preferredFormats, extraQualities, extraFeatures

### Backward Compatibility Strategy

The Service class should support both API versions:
- Keep existing properties for API 2.x compatibility
- Add new optional properties for API 3.0
- JsonConverter handles both formats during deserialization
- Serialization format determined by @context version

## Example Usage After Implementation

```csharp
// IIIF Image API 3.0 service
var service = new Service(
    "http://iiif.io/api/image/3/context.json",
    "https://example.org/iiif/image1",
    "level2"
)
.SetHeight(4000)
.SetWidth(3000)
.SetMaxWidth(2000)
.SetMaxHeight(1500)
.SetMaxArea(10000000)
.SetRights("http://creativecommons.org/licenses/by/4.0/")
.AddSize(new Size(150, 100))
.AddSize(new Size(600, 400))
.AddSize(new Size(1200, 800))
.AddPreferredFormat("webp")
.AddPreferredFormat("jpg")
.AddExtraQuality("bitonal")
.AddExtraFeature("regionByPx")
.AddExtraFeature("sizeByW");

// Add tiles for deep-zoom
var tile = new Tile().SetWidth(512);
tile.AddScaleFactor(1).AddScaleFactor(2).AddScaleFactor(4).AddScaleFactor(8);
service.AddTile(tile);

// Attach to image resource
imageResource.SetService(service);
```

## Decision

**Recommendation:** Implement Priority 1 properties (Size, maxWidth, maxHeight, maxArea) as these are the most commonly used in IIIF Image API 3.0 implementations.

**Rationale:**
- Most IIIF image servers provide sizes and max dimension constraints
- These properties help clients determine available resolution options
- Backward compatible with existing API 2.x implementations
- Low complexity to implement

**Next Steps:**
1. Create Size class
2. Add properties to Service
3. Update ServiceJsonConverter
4. Add tests for new properties
5. Update documentation
6. Version as 1.2.0 (new features, backward compatible)

