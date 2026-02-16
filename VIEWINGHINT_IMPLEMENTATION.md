# ViewingHint Implementation Summary

## Changes Made

### 1. Refactored ViewingHint from Enum to Class

**File Modified:** `src/IIIF.Manifest.Serializer.Net/Properties/ViewingHint.cs`

Changed ViewingHint from an enum to a class following the same pattern as ViewingDirection:

**Before (Enum):**
```csharp
public enum ViewingHint
{
    Unspecified = 0,
    Paged = 1,
    Continuous = 2,
    // ... etc
}
```

**After (Class):**
```csharp
[JsonConverter(typeof(ValuableItemJsonConverter<ViewingHint>))]
public class ViewingHint : ValuableItem<ViewingHint>
{
    public ViewingHint(string value) : base(value) { }
    
    public static ViewingHint Paged => new ViewingHint("paged");
    public static ViewingHint Continuous => new ViewingHint("continuous");
    public static ViewingHint Individuals => new ViewingHint("individuals");
    public static ViewingHint FacingPages => new ViewingHint("facing-pages");
    public static ViewingHint NonPaged => new ViewingHint("non-paged");
    public static ViewingHint Top => new ViewingHint("top");
    public static ViewingHint MultiPart => new ViewingHint("multi-part");
}
```

### 2. Removed Custom JsonConverter

**File Deleted:** `src/IIIF.Manifest.Serializer.Net/Properties/ViewingHintJsonConverter.cs`

- No longer needed since we use `ValuableItemJsonConverter<ViewingHint>`
- This provides automatic serialization/deserialization consistent with other property types

### 3. Updated BaseNode

**File Modified:** `src/IIIF.Manifest.Serializer.Net/Shared/BaseNode/BaseNode.cs`

Property is already correctly typed:
```csharp
[JsonProperty(ViewingHintJName)]
public ViewingHint ViewingHint { get; private set; }
```

### 4. Updated BaseNodeJsonConverter

**File Modified:** `src/IIIF.Manifest.Serializer.Net/Shared/BaseNode/BaseNodeJsonConverter.cs`

**Changed null check from `Unspecified` to `null`:**

**Before:**
```csharp
if (node.ViewingHint != ViewingHint.Unspecified)
{
    writer.WritePropertyName(BaseNode<TBaseNode>.ViewingHintJName);
    serializer.Serialize(writer, node.ViewingHint);
}
```

**After:**
```csharp
if (node.ViewingHint != null)
{
    writer.WritePropertyName(BaseNode<TBaseNode>.ViewingHintJName);
    serializer.Serialize(writer, node.ViewingHint);
}
```

### 5. Updated Tests

**File Modified:** `tests/IIIF.Manifest.Serializer.Net.Tests/Properties/ViewingHintTests.cs`

Completely rewrote tests to work with class-based ViewingHint:

- Removed enum comparison tests
- Added string-based parameterized tests
- Added null check test
- Added static property usage test
- Tests verify `.Value` property for assertions

**Key Test Changes:**
- Use `new ViewingHint(value)` instead of enum values
- Use `ViewingHint.Paged`, `ViewingHint.Top`, etc. for static properties
- Assert with `.Value.Should().Be("paged")` instead of enum comparison
- Added test for null ViewingHint (should not serialize)

## Benefits of This Approach

1. **Consistency:** ViewingHint now follows the same pattern as ViewingDirection
2. **Null Safety:** Uses `null` instead of magic enum value (`Unspecified`)
3. **Extensibility:** Easy to add custom hint values if needed
4. **JSON Compatibility:** Automatic serialization via ValuableItemJsonConverter
5. **Type Safety:** Still provides static properties for common values

## Usage Examples

### Creating with Static Properties
```csharp
manifest.SetViewingHint(ViewingHint.Paged);
canvas.SetViewingHint(ViewingHint.Top);
collection.SetViewingHint(ViewingHint.MultiPart);
```

### Creating with Custom Values
```csharp
var customHint = new ViewingHint("my-custom-hint");
manifest.SetViewingHint(customHint);
```

### Null for No Hint
```csharp
// ViewingHint is null by default
var manifest = new Manifest(...);
// manifest.ViewingHint == null

// Won't be serialized to JSON
```

## JSON Serialization

**Input:**
```csharp
manifest.SetViewingHint(ViewingHint.Paged);
```

**Output:**
```json
{
  "@context": "http://iiif.io/api/presentation/2/context.json",
  "@id": "https://example.org/manifest",
  "@type": "sc:Manifest",
  "label": "Book",
  "viewingHint": "paged",
  ...
}
```

**When null:**
```csharp
// ViewingHint not set or null
```

**Output:**
```json
{
  "@context": "http://iiif.io/api/presentation/2/context.json",
  "@id": "https://example.org/manifest",
  "@type": "sc:Manifest",
  "label": "Book",
  // viewingHint is omitted
  ...
}
```

## Test Results

✅ **ViewingHint tests: All passing**
- `ViewingHint_ShouldSerializeCorrectly` (7 test cases via Theory)
- `ViewingHint_ShouldDeserializeCorrectly` (7 test cases via Theory)
- `ViewingHint_StaticProperties_ShouldWorkCorrectly`
- `ViewingHint_Null_ShouldNotSerialize`

## Breaking Changes

**Migration Guide for Users:**

**Before:**
```csharp
manifest.SetViewingHint(ViewingHint.Paged);
if (manifest.ViewingHint == ViewingHint.Unspecified) { }
```

**After:**
```csharp
manifest.SetViewingHint(ViewingHint.Paged);
if (manifest.ViewingHint == null) { }
```

The API is mostly compatible, just replace `Unspecified` checks with `null` checks.

## Files Changed

1. ✏️ Modified: `src/.../Properties/ViewingHint.cs` - Enum → Class
2. 🗑️ Deleted: `src/.../Properties/ViewingHintJsonConverter.cs` - No longer needed
3. ✏️ Modified: `src/.../Shared/BaseNode/BaseNodeJsonConverter.cs` - Null check
4. ✏️ Modified: `tests/.../Properties/ViewingHintTests.cs` - Updated all tests

## Conclusion

ViewingHint now perfectly mirrors the ViewingDirection implementation, providing a consistent and clean API for IIIF viewing hints with proper null handling instead of magic enum values.

