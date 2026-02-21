# Unhandled JSON Properties Storage Implementation

## Summary

Implemented storage and round-trip preservation of unhandled JSON properties in the IIIF Manifest Serializer library. When deserializing JSON that contains properties not explicitly handled by the converters, those properties are now preserved and re-serialized.

## Changes Made

### 1. TrackableObject.cs
- Added `additionalProperties` dictionary to store unhandled JTokens
- Added public `AdditionalProperties` read-only property for accessing stored properties
- Added `StoreAdditionalProperty(string key, JToken value)` method to store individual properties
- Added `ClearAdditionalProperties()` method for cleanup

**File:** `src/IIIF.Manifest.Serializer.Net/Shared/Trackable/TrackableObject.cs`

```csharp
[JsonIgnore]
private readonly Dictionary<string, JToken> additionalProperties = new Dictionary<string, JToken>();

[JsonIgnore]
public IReadOnlyDictionary<string, JToken> AdditionalProperties => additionalProperties;

internal void StoreAdditionalProperty(string key, JToken value)
{
    if (!string.IsNullOrEmpty(key))
    {
        additionalProperties[key] = value;
    }
}

internal void ClearAdditionalProperties() => additionalProperties.Clear();
```

### 2. TrackableObjectJsonConverter.cs
- Modified `ReadJson` to store all properties when `handled = false`
- Added `WriteAdditionalProperties` method to write stored properties during serialization

**File:** `src/IIIF.Manifest.Serializer.Net/Shared/Trackable/TrackableObjectJsonConverter.cs`

**ReadJson changes:**
```csharp
if (!handled && element is JObject jObject)
{
    // Store all properties from the unhandled JSON object
    foreach (var property in jObject.Properties())
    {
        rtn.StoreAdditionalProperty(property.Name, property.Value);
    }
}
```

**WriteAdditionalProperties method:**
```csharp
protected void WriteAdditionalProperties(JsonWriter writer, TTrackableObject value, JsonSerializer serializer)
{
    if (value?.AdditionalProperties != null && value.AdditionalProperties.Count > 0)
    {
        foreach (var property in value.AdditionalProperties)
        {
            writer.WritePropertyName(property.Key);
            property.Value.WriteTo(writer);
        }
    }
}
```

### 3. BaseItemJsonConverter.cs
- Updated `EnrichWriteJson` to call `WriteAdditionalProperties` before closing the JSON object
- Set `handled = true` in `EnrichReadJson` to indicate successful parsing

**File:** `src/IIIF.Manifest.Serializer.Net/Shared/BaseItem/BaseItemJsonConverter.cs`

```csharp
protected sealed override void EnrichWriteJson(JsonWriter writer, TBaseItem value, JsonSerializer serializer)
{
    // ...existing properties...
    
    EnrichMoreWriteJson(writer, value, serializer);
    
    WriteAdditionalProperties(writer, value, serializer);
    
    writer.WriteEndObject();
}
```

### 4. Fixed All Converter `handled` Parameters
Fixed 20+ JSON converters to properly set the `handled` parameter in their `EnrichReadJson` methods:

- BaseItemJsonConverter.cs
- BaseNodeJsonConverter.cs
- BaseContentJsonConverter.cs
- FormatableItemJsonConverter.cs
- ManifestJsonConverter.cs
- CanvasJsonConverter.cs
- SequenceJsonConverter.cs
- StructureJsonConverter.cs
- CollectionJsonConverter.cs
- LayerJsonConverter.cs
- AnnotationListJsonConverter.cs
- ImageResourceJsonConverter.cs
- AudioResourceJsonConverter.cs
- VideoResourceJsonConverter.cs
- SegmentJsonConverter.cs
- SegmentResourceJsonConverter.cs
- ServiceJsonConverter.cs
- AuthService1JsonConverter.cs
- AuthService2JsonConverter.cs
- SearchServiceJsonConverter.cs
- DiscoveryServiceJsonConverter.cs
- TileJsonConverter.cs
- SizeJsonConverter.cs
- WithinJsonConverter.cs
- DescriptionJsonConverter.cs

## Behavior

### When `handled = true`
The converter successfully processed the JSON. Only known, explicitly handled properties are present in the resulting object.

### When `handled = false`
The converter could not fully deserialize the JSON (e.g., unknown type, unsupported format). All properties from the original JSON are stored in `AdditionalProperties` for later serialization.

### Round-trip Preservation
When an object is deserialized and then re-serialized:
1. Known properties are handled by their specific converters
2. Additional/unknown properties are preserved exactly as they appeared in the input
3. The output JSON maintains both standard and custom properties

## Use Cases

### 1. Custom Extensions
Libraries can extend IIIF manifests with custom properties without losing them during round-trip serialization:

```json
{
  "@context": "http://iiif.io/api/presentation/2/context.json",
  "@type": "sc:Manifest",
  "@id": "https://example.org/manifest",
  "label": "My Manifest",
  "customExtension": {
    "vendor": "MyOrg",
    "data": "custom value"
  }
}
```

### 2. Forward Compatibility
When newer IIIF specifications add properties not yet implemented, those properties are preserved during processing.

### 3. Debugging & Migration
Helps identify and preserve properties during manifest validation and migration workflows.

## Testing

A test file and program were created:
- `test_unhandled_properties.json` - Sample manifest with custom properties
- `TestUnhandledProperties.cs` - Console app to verify round-trip preservation

To test:
```bash
cd C:\Users\Kiarash\RiderProjects\IIIF.Manifest.Serializer.Net
dotnet run --project TestUnhandledProperties.cs
```

## Compatibility

This change is backward compatible:
- Existing code continues to work without modification
- No breaking changes to public API
- Additional properties are opt-in via the `AdditionalProperties` property
- Standard deserialization/serialization behavior unchanged

## Notes

- Additional properties are stored as raw JToken objects, preserving exact structure
- Properties are marked `[JsonIgnore]` to prevent recursion during serialization
- The `WriteAdditionalProperties` method is called from `BaseItemJsonConverter`, so all derived types automatically support this feature
- This implementation aligns with JSON.NET's extension data pattern but uses a custom approach suitable for the library's architecture

