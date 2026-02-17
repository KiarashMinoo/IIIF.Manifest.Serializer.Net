# IIIF Extension Libraries - Modular Architecture

This document demonstrates how the IIIF approved extensions can be extracted into separate, independent libraries while maintaining compatibility with the main IIIF.Manifest.Serializer.Net library.

## Implementation Status

### ✅ **Phase 1 Complete: Extension Libraries Created**
- **Text Granularity Extension 1.0**: Full implementation with classes, converters, tests, and cookbook recipes
- **navPlace Extension 1.0**: Geographic location support with GeoJSON-LD, custom converters, and tests
- **Georeference Extension 1.0**: Map georeferencing with Ground Control Points, transformation algorithms, and tests
- **Project Structure**: Extensions organized under `src/extensions/` folder with proper dependencies
- **Build Status**: All 7 projects (core + 3 extensions + tests + examples + cookbook) build successfully
- **Test Coverage**: 165 tests pass, confirming functionality after reorganization

### 🔄 **Phase 2 In Progress: Core Library Refactoring**
- Extension registration system design
- Conditional compilation setup
- Build system updates for modular extensions

### 📋 **Phase 3 Planned: Documentation & Communication**
- Migration guides for users
- NuGet packaging strategy
- Deprecation notices for monolithic usage

## Architecture Overview

### Current State (Monolithic)
```
IIIF.Manifest.Serializer.Net (Main Library)
├── Core IIIF Classes (Manifest, Canvas, etc.)
├── Text Granularity Extension (integrated)
├── navPlace Extension (integrated)
└── Georeference Extension (integrated)
```

### Proposed State (Modular)
```
src/
├── IIIF.Manifest.Serializer.Net/ (Core Library)
│   ├── Core IIIF Classes (Manifest, Canvas, etc.)
│   └── Extension Points (interfaces/hooks)
└── extensions/
    ├── IIIF.Manifest.Serializer.Net.NavPlace/ (Extension Library)
    │   ├── NavPlace classes
    │   ├── NavPlaceJsonConverter
    │   └── NavPlaceExtensionAttribute
    ├── IIIF.Manifest.Serializer.Net.TextGranularity/ (Extension Library)
    │   ├── TextGranularity classes
    │   ├── TextGranularityJsonConverter
    │   └── TextGranularityExtensionAttribute
    └── IIIF.Manifest.Serializer.Net.Georeference/ (Extension Library)
        ├── Georeference classes
        ├── GeoreferenceJsonConverter
        └── GeoreferenceExtensionAttribute
```

## Benefits of Modular Architecture

### ✅ **Selective Dependencies**
Users can include only the extensions they need:
```xml
<!-- Only need geographic features -->
<PackageReference Include="IIIF.Manifest.Serializer.Net" Version="1.0.0" />
<PackageReference Include="IIIF.Manifest.Serializer.Net.NavPlace" Version="1.0.0" />
<PackageReference Include="IIIF.Manifest.Serializer.Net.Georeference" Version="1.0.0" />
```

### ✅ **Independent Versioning**
Each extension can be versioned and released independently:
- Core library: v2.1.0
- navPlace: v1.1.0 (with new features)
- TextGranularity: v1.0.0 (stable)

### ✅ **Smaller Footprint**
Applications not using extensions have a smaller dependency:
- Core library: ~50KB
- Each extension: ~10-20KB

### ✅ **Team Autonomy**
Different teams can maintain different extensions without affecting others.

### ✅ **Future-Proof**
Easy to add new extensions without modifying the core library.

## Implementation Strategy

### 1. Extension Registration Pattern

The core library provides extension points that extension libraries can register with:

```csharp
// In core library
public interface IIIFExtension
{
    void Register(JsonSerializer serializer);
    string ExtensionName { get; }
    string Version { get; }
}

// Extension libraries implement this
public class NavPlaceExtension : IIIFExtension
{
    public void Register(JsonSerializer serializer)
    {
        // Register custom converters, etc.
    }
    public string ExtensionName => "navPlace";
    public string Version => "1.0";
}
```

### 2. Conditional Compilation

Use conditional compilation for extension features:

```csharp
public class BaseNode<TBaseNode> : BaseItem<TBaseNode>
{
    // Core properties always available
    [JsonProperty(LabelJName)]
    public IReadOnlyCollection<Label> Label => labels.AsReadOnly();

#if NAVPLACE_EXTENSION
    [NavPlaceExtension("1.0")]
    [JsonProperty(NavPlaceJName)]
    public NavPlace NavPlace { get; private set; }
#endif

#if GEOREFERENCE_EXTENSION
    [GeoreferenceExtension("1.0")]
    [JsonProperty(GeoreferenceJName)]
    public Georeference Georeference { get; private set; }
#endif
}
```

### 3. NuGet Package Configuration

Each extension library is published as a separate NuGet package:

```xml
<!-- IIIF.Manifest.Serializer.Net.NavPlace.nuspec -->
<package>
  <metadata>
    <id>IIIF.Manifest.Serializer.Net.NavPlace</id>
    <version>1.0.0</version>
    <title>IIIF navPlace Extension</title>
    <description>Geographic location support for IIIF manifests</description>
    <dependencies>
      <dependency id="IIIF.Manifest.Serializer.Net" version="[1.0.0,2.0.0)" />
      <dependency id="Newtonsoft.Json" version="13.0.1" />
    </dependencies>
  </metadata>
</package>
```

## Migration Path

### Phase 1: Create Extension Libraries (✅ Complete)
- Extract extension code into separate projects under `src/extensions/` (✅ Done)
- Organize extension libraries in dedicated folder structure (✅ Done)
- Publish as separate NuGet packages
- Maintain backward compatibility

### Phase 2: Core Library Refactoring
- Add extension registration system
- Make extension properties conditional
- Update build system for conditional compilation

### Phase 3: Documentation & Communication (🔄 In Progress)
- Update documentation for modular approach (✅ Updated folder structure)
- Provide migration guides
- Announce deprecation of monolithic extensions

## Usage Examples

### With Extensions (Modular)
```csharp
// Install: dotnet add package IIIF.Manifest.Serializer.Net.NavPlace

using IIIF.Manifests.Serializer.Net.NavPlace;

var canvas = new Canvas("https://example.org/canvas/1", new Label("Map"))
    .SetNavPlace(NavPlace.FromPoint(9.938, 51.533, "Göttingen"));
```

### Without Extensions (Core Only)
```csharp
// Install: dotnet add package IIIF.Manifest.Serializer.Net

var canvas = new Canvas("https://example.org/canvas/1", new Label("Image"));
// navPlace property not available - no compilation error
```

## Build Configuration

### Conditional Compilation Symbols
```xml
<!-- In extension library .csproj -->
<PropertyGroup Condition="'$(IncludeNavPlace)' == 'true'">
  <DefineConstants>$(DefineConstants);NAVPLACE_EXTENSION</DefineConstants>
</PropertyGroup>
```

### MSBuild Integration
```xml
<!-- Directory.Build.props -->
<PropertyGroup>
  <IncludeNavPlace Condition="'$(IncludeNavPlace)' != 'false'">true</IncludeNavPlace>
  <IncludeGeoreference Condition="'$(IncludeGeoreference)' != 'false'">true</IncludeGeoreference>
</PropertyGroup>
```

## Testing Strategy

### Extension-Specific Tests
Each extension library includes its own test suite:

```
tests/
├── IIIF.Manifest.Serializer.Net.Tests/          # Core tests
├── IIIF.Manifest.Serializer.Net.NavPlace.Tests/ # navPlace tests
├── IIIF.Manifest.Serializer.Net.TextGranularity.Tests/
└── IIIF.Manifest.Serializer.Net.Georeference.Tests/
```

### Integration Tests
Combined tests that verify extension interoperability with core library.

## Decision Factors

### When to Use Modular Extensions

**Recommended for:**
- Large applications with specific extension needs
- Library authors building on IIIF.Serializer
- Teams with different extension maintenance schedules
- Applications concerned about bundle size

**Current Monolithic Approach is Better for:**
- Simple applications using all/most extensions
- Prototyping and development
- Applications where bundle size is not critical
- Teams preferring single-dependency approach

## Conclusion

Yes, it is absolutely possible and beneficial to move the IIIF approved extensions to separate libraries. The modular approach provides:

- **Flexibility** for users to include only needed extensions
- **Maintainability** with independent versioning
- **Scalability** for future extension development
- **Compatibility** with existing monolithic usage

The demonstration projects created show the structure and approach for implementing this modular architecture.