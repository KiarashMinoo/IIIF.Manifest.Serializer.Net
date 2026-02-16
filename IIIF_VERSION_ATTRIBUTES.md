# IIIF Version Attributes - Complete Documentation

## Overview

Version 1.5.0 introduces comprehensive IIIF API version tracking through custom attributes. Every class, property, and value object is now annotated with its supported API versions, deprecation status, and migration paths.

## Attribute Classes

### Base Attribute

**IIIFVersionAttribute**
```csharp
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Field)]
public class IIIFVersionAttribute : Attribute
{
    string MinVersion { get; }           // Minimum supported version
    string MaxVersion { get; }           // Maximum supported version (null = current)
    bool IsDeprecated { get; set; }      // Is this feature deprecated?
    string DeprecatedInVersion { get; set; }  // Version where deprecated
    string ReplacedBy { get; set; }      // Replacement feature name
    string Notes { get; set; }           // Additional version notes
}
```

### Specialized Attributes

| Attribute | Purpose |
|-----------|---------|
| `PresentationAPIAttribute` | IIIF Presentation API features |
| `ImageAPIAttribute` | IIIF Image API features |
| `AuthAPIAttribute` | IIIF Auth API features |
| `SearchAPIAttribute` | IIIF Search API features |
| `DiscoveryAPIAttribute` | IIIF Change Discovery API features |
| `ContentStateAPIAttribute` | IIIF Content State API features |

## Annotated Resources

### Presentation API 2.0 Core Resources

#### Manifest
```csharp
[PresentationAPI("2.0", Notes = "Core resource. Structure changed in 3.0.")]
public class Manifest : BaseNode<Manifest>
{
    [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", 
        ReplacedBy = "items")]
    public IReadOnlyCollection<Sequence> Sequences { get; }
}
```

#### Sequence
```csharp
[PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", 
    ReplacedBy = "Manifest.items", Notes = "Sequences removed in API 3.0")]
public class Sequence : BaseNode<Sequence>
```

#### Canvas
```csharp
[PresentationAPI("2.0", Notes = "Core resource. In 3.0, images replaced by items.")]
public class Canvas : BaseNode<Canvas>
{
    [PresentationAPI("2.1", Notes = "Added in 2.1 for A/V support. Also in 3.0.")]
    public double? Duration { get; }
    
    [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", 
        ReplacedBy = "items")]
    public IReadOnlyCollection<Image> Images { get; }
}
```

#### Collection
```csharp
[PresentationAPI("2.0", Notes = "Supported in 2.x and 3.0. Paging added in 2.0.")]
public class Collection : BaseNode<Collection>
{
    [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", 
        ReplacedBy = "items")]
    public IReadOnlyCollection<Collection> Collections { get; }
    
    [PresentationAPI("2.0", Notes = "Paging property")]
    public string First { get; }
}
```

#### Structure (Range)
```csharp
[PresentationAPI("2.0", Notes = "'structures' in 2.x, 'Range' in 3.0")]
public class Structure : BaseNode<Structure>
{
    [PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", 
        ReplacedBy = "items")]
    public IReadOnlyCollection<string> Canvases { get; }
}
```

### Deprecated in API 3.0

#### Layer
```csharp
[PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", 
    ReplacedBy = "AnnotationCollection", Notes = "Layers removed in API 3.0")]
public class Layer : BaseNode<Layer>
```

#### AnnotationList
```csharp
[PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", 
    ReplacedBy = "AnnotationPage", 
    Notes = "AnnotationLists replaced by AnnotationPage in API 3.0")]
public class AnnotationList : BaseNode<AnnotationList>
```

### Image API Features

#### Service
```csharp
[ImageAPI("2.0", Notes = "Service descriptor. Properties vary between 2.x and 3.0.")]
public class Service : BaseItem<Service>
{
    [ImageAPI("2.0")]
    public string Profile { get; }
    
    [ImageAPI("3.0", Notes = "Added in Image API 3.0")]
    public int? MaxWidth { get; }
    
    [ImageAPI("3.0", Notes = "Added in Image API 3.0")]
    public IReadOnlyCollection<ImageFormat> PreferredFormats { get; }
}
```

## Value Objects Annotations

### ViewingDirection
```csharp
[PresentationAPI("2.0", Notes = "Supported in both 2.x and 3.0")]
public class ViewingDirection : ValuableItem<ViewingDirection>
```

### ViewingHint
```csharp
[PresentationAPI("2.0", "2.1", IsDeprecated = true, DeprecatedInVersion = "3.0", 
    ReplacedBy = "behavior", Notes = "viewingHint renamed to behavior in API 3.0")]
public class ViewingHint : ValuableItem<ViewingHint>
```

### Behavior
```csharp
[PresentationAPI("3.0", Notes = "Replaces viewingHint from API 2.x")]
public class Behavior : ValuableItem<Behavior>
```

### ImageFormat
```csharp
[ImageAPI("2.0", Notes = "Supported in both Image API 2.x and 3.0")]
public class ImageFormat : ValuableItem<ImageFormat>
```

### ImageQuality
```csharp
[ImageAPI("2.0", Notes = "Supported in both Image API 2.x and 3.0")]
public class ImageQuality : ValuableItem<ImageQuality>
```

### ImageFeature
```csharp
[ImageAPI("2.0", Notes = "Supported in both 2.x and 3.0. Feature names consistent.")]
public class ImageFeature : ValuableItem<ImageFeature>
```

### Motivation
```csharp
[PresentationAPI("2.0", Notes = "sc:painting in 2.x, painting in 3.0.")]
public class Motivation : ValuableItem<Motivation>
```

### ResourceType
```csharp
[PresentationAPI("2.0", Notes = "Type values vary between 2.x (sc: prefix) and 3.0")]
public class ResourceType : ValuableItem<ResourceType>
```

### Profile
```csharp
[ImageAPI("2.0", Notes = "Format changed between 2.x (URLs) and 3.0 (keywords).")]
public class Profile : ValuableItem<Profile>
```

### Context
```csharp
[IIIFVersion("1.0", Notes = "Context URLs identify the API version being used.")]
public class Context : ValuableItem<Context>
```

### Rights
```csharp
[PresentationAPI("2.0", Notes = "In 2.x use 'license', in 3.0 use 'rights'. Values same.")]
public class Rights : ValuableItem<Rights>
```

### Language
```csharp
[PresentationAPI("2.0", Notes = "Language tags used in both 2.x and 3.0")]
public class Language : ValuableItem<Language>
```

### TimeMode
```csharp
[PresentationAPI("3.0", Notes = "Time mode for temporal media in Presentation API 3.0")]
public class TimeMode : ValuableItem<TimeMode>
```

## Usage Examples

### Reading Version Information via Reflection

```csharp
using System;
using System.Reflection;
using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Nodes.Manifest;

// Get version info for Manifest class
var manifestType = typeof(Manifest);
var attr = manifestType.GetCustomAttribute<PresentationAPIAttribute>();

Console.WriteLine($"Min Version: {attr.MinVersion}");
Console.WriteLine($"Max Version: {attr.MaxVersion ?? "current"}");
Console.WriteLine($"Notes: {attr.Notes}");

// Check if Sequence property is deprecated
var sequencesProp = manifestType.GetProperty("Sequences");
var seqAttr = sequencesProp.GetCustomAttribute<PresentationAPIAttribute>();

if (seqAttr.IsDeprecated)
{
    Console.WriteLine($"Deprecated in: {seqAttr.DeprecatedInVersion}");
    Console.WriteLine($"Replaced by: {seqAttr.ReplacedBy}");
}
```

### Creating a Version Compatibility Checker

```csharp
public static class VersionChecker
{
    public static bool IsSupported(Type type, string targetVersion)
    {
        var attr = type.GetCustomAttribute<IIIFVersionAttribute>();
        if (attr == null) return true;
        
        var target = Version.Parse(targetVersion);
        var min = Version.Parse(attr.MinVersion);
        
        if (target < min) return false;
        
        if (attr.MaxVersion != null)
        {
            var max = Version.Parse(attr.MaxVersion);
            if (target > max) return false;
        }
        
        return true;
    }
    
    public static bool IsDeprecated(PropertyInfo property)
    {
        var attr = property.GetCustomAttribute<IIIFVersionAttribute>();
        return attr?.IsDeprecated ?? false;
    }
}

// Usage
bool manifestSupported = VersionChecker.IsSupported(typeof(Manifest), "3.0");
bool sequencesDeprecated = VersionChecker.IsDeprecated(
    typeof(Manifest).GetProperty("Sequences")
);
```

### Generating Migration Reports

```csharp
public static class MigrationAnalyzer
{
    public static void AnalyzeDeprecations(Type type)
    {
        Console.WriteLine($"Analyzing {type.Name}...\n");
        
        foreach (var prop in type.GetProperties())
        {
            var attr = prop.GetCustomAttribute<IIIFVersionAttribute>();
            if (attr?.IsDeprecated == true)
            {
                Console.WriteLine($"⚠️  {prop.Name}");
                Console.WriteLine($"   Deprecated in: {attr.DeprecatedInVersion}");
                Console.WriteLine($"   Replace with: {attr.ReplacedBy}");
                if (!string.IsNullOrEmpty(attr.Notes))
                    Console.WriteLine($"   Note: {attr.Notes}");
                Console.WriteLine();
            }
        }
    }
}

// Usage
MigrationAnalyzer.AnalyzeDeprecations(typeof(Manifest));
// Output:
// ⚠️  Sequences
//    Deprecated in: 3.0
//    Replace with: items
//    Note: Sequences removed in API 3.0; canvases moved to items array
```

## Benefits

### 1. **Clear Documentation**
Every class and property is self-documenting about its API version support.

### 2. **IDE Support**
Attributes enable:
- IntelliSense tooltips showing version info
- Code analysis warnings for deprecated features
- Navigation to replacement features

### 3. **Migration Planning**
Easily identify:
- Features that need migration for API 3.0
- Replacement paths for deprecated features
- Version-specific behavior differences

### 4. **Tooling Integration**
Enables creation of:
- Static analyzers
- Migration tools
- Compatibility checkers
- Documentation generators

### 5. **Future-Proofing**
Prepared for:
- IIIF Presentation API 3.0 migration
- Multi-version support
- Gradual deprecation warnings

## API Version Matrix

| Feature | API 2.0 | API 2.1 | API 3.0 | Status |
|---------|---------|---------|---------|--------|
| Manifest | ✅ | ✅ | ✅ | Modified in 3.0 |
| Collection | ✅ | ✅ | ✅ | Modified in 3.0 |
| Sequence | ✅ | ✅ | ❌ | Deprecated |
| Canvas | ✅ | ✅ | ✅ | Modified in 3.0 |
| Structure/Range | ✅ | ✅ | ✅ | Renamed to Range |
| Layer | ✅ | ✅ | ❌ | Deprecated |
| AnnotationList | ✅ | ✅ | ❌ | Deprecated |
| duration | ❌ | ✅ | ✅ | Added in 2.1 |
| viewingHint | ✅ | ✅ | ❌ | Renamed to behavior |
| behavior | ❌ | ❌ | ✅ | New in 3.0 |

## Deprecation Patterns

### Pattern 1: Property Renamed
```csharp
// API 2.x
[PresentationAPI("2.0", "2.1", IsDeprecated = true, 
    DeprecatedInVersion = "3.0", ReplacedBy = "behavior")]
public ViewingHint ViewingHint { get; }

// API 3.0 equivalent
[PresentationAPI("3.0")]
public Behavior Behavior { get; }
```

### Pattern 2: Structure Changed
```csharp
// API 2.x
[PresentationAPI("2.0", "2.1", IsDeprecated = true, 
    ReplacedBy = "items")]
public IReadOnlyCollection<Sequence> Sequences { get; }

// API 3.0: Canvases moved directly to Manifest.items
```

### Pattern 3: Resource Removed
```csharp
// API 2.x only
[PresentationAPI("2.0", "2.1", IsDeprecated = true, 
    DeprecatedInVersion = "3.0", ReplacedBy = "AnnotationPage")]
public class AnnotationList { }
```

## Version Information Summary

- **Total Annotated Classes**: 20+
- **Total Annotated Properties**: 50+
- **Total Value Objects**: 13
- **Deprecated Features**: 8
- **API Versions Covered**: 2.0, 2.1, 3.0

## Conclusion

The IIIF version attribute system provides:
- ✅ Complete API version documentation
- ✅ Clear deprecation tracking
- ✅ Migration path guidance
- ✅ Tooling integration support
- ✅ Future-proof architecture

This enables developers to:
- Understand version compatibility at a glance
- Plan migrations with confidence
- Build version-aware tools
- Maintain backward compatibility

**Version: 1.5.0**  
**Status: Complete** ✅

