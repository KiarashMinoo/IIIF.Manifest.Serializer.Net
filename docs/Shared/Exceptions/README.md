# Exceptions

## Contents
- [Overview](#overview)
- [Files](#files)
- [Types & Members](#types--members)
- [Examples](#examples)
- [See Also](#see-also)

## Overview

The Exceptions folder provides custom exception types for JSON validation errors during IIIF Presentation API 2.0 deserialization. JsonNodeRequiredException<T> indicates a required JSON property is missing (@id, @type, etc.), JsonObjectMustBeJArray<T> signals a field expected to be an array is a single object, and JsonObjectMustBeJObject<T> indicates a field expected to be an object is an array or primitive. All exceptions are generic over the target type for clear error messages using Newtonsoft.Json parsing context.

## Files

| File | Primary type(s) | LOC (approx) | Responsibility |
|------|-----------------|--------------|----------------|
| [JsonNodeRequiredException.cs](../../../src/IIIF.Manifest.Serializer.Net/Shared/Exceptions/JsonNodeRequiredException.cs) | `JsonNodeRequiredException<T>` | 15 | Thrown when required JSON property missing (e.g., @id, @type, label) |
| [JsonObjectMustBeJArray.cs](../../../src/IIIF.Manifest.Serializer.Net/Shared/Exceptions/JsonObjectMustBeJArray.cs) | `JsonObjectMustBeJArray<T>` | 15 | Thrown when array expected but single object provided (e.g., sequences, canvases) |
| [JsonObjectMustBeJObject.cs](../../../src/IIIF.Manifest.Serializer.Net/Shared/Exceptions/JsonObjectMustBeJObject.cs) | `JsonObjectMustBeJObject<T>` | 15 | Thrown when object expected but array or primitive provided |

[↑ Back to top](#contents)

## Types & Members

| Type | Kind | Summary | Inherits/Implements | Key Members |
|------|------|---------|---------------------|-------------|
| `JsonNodeRequiredException<T>` | Class | Exception for missing required JSON property | `Exception` | Constructor(propertyName) |
| `JsonObjectMustBeJArray<T>` | Class | Exception for non-array when array expected | `Exception` | Constructor(propertyName) |
| `JsonObjectMustBeJObject<T>` | Class | Exception for non-object when object expected | `Exception` | Constructor(propertyName) |

[↑ Back to top](#contents)

## Examples

```csharp
// JsonNodeRequiredException - missing @id
// Input: { "@type": "sc:Manifest", "label": "Book" }
// Throws: JsonNodeRequiredException<Manifest> ("@id")

// JsonObjectMustBeJArray - single object instead of array
// Input: "sequences": { "@id": "...", "canvases": [...] }
// Expected: "sequences": [{ "@id": "...", "canvases": [...] }]
// Throws: JsonObjectMustBeJArray<Manifest> ("sequences")

// JsonObjectMustBeJObject - array instead of object
// Input: "service": [{ "@id": "...", "profile": "..." }]
// Expected: "service": { "@id": "...", "profile": "..." }
// Throws: JsonObjectMustBeJObject<Canvas> ("service")
```

[↑ Back to top](#contents)

## See Also

- [../BaseItem/README.md](../BaseItem/README.md) – Throws JsonNodeRequiredException for @id/@type
- [../README.md](../README.md) – Parent Shared folder

[↑ Back to top](#contents)
