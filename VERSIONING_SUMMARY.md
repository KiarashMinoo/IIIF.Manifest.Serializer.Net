# API Versioning Analysis - Complete Summary

## Executive Summary

Successfully analyzed Semantic Versioning 2.0.0 (SemVer) and IIIF-specific versioning guidelines, then applied them to the IIIF.Manifest.Serializer.Net library.

**Key Outcome:** Updated library version from **1.0.0** to **1.1.0** following SemVer best practices.

---

## Semantic Versioning 2.0.0 - Core Principles

Source: https://semver.org/spec/v2.0.0.html

### Version Format: MAJOR.MINOR.PATCH

| Component | When to Increment | Example Changes |
|-----------|------------------|-----------------|
| **MAJOR** | Incompatible API changes | Removing methods, renaming properties, changing types |
| **MINOR** | Backward-compatible new features | Adding optional properties, new classes, new methods |
| **PATCH** | Backward-compatible bug fixes | Fixing bugs, security patches, documentation |

### Critical Rules

1. ✅ **Version 1.0.0** defines the public API
2. ✅ **Once published**, a version never changes (immutable)
3. ✅ **MAJOR = 0** means initial development (unstable)
4. ✅ **Pre-release**: append `-alpha`, `-beta`, `-rc.1`
5. ✅ **Build metadata**: append `+20130313144700`

---

## IIIF Versioning Guidelines

Source: https://iiif.io/api/annex/notes/semver/

### IIIF-Specific Practices

1. **Version in Context URLs**
   ```json
   "@context": "http://iiif.io/api/presentation/2/context.json"
   "@context": "http://iiif.io/api/presentation/3/context.json"
   ```

2. **Major Versions = Breaking Changes**
   - Presentation 2.x → 3.0: Changed data model (`sequences` → `items`)
   - Image API 2.x → 3.0: Updated compliance levels

3. **Specification Stability**
   - Published specs don't change (only editorial clarifications)
   - Old versions remain available indefinitely
   - Servers should support version negotiation

4. **Deprecation Policy**
   - Features marked deprecated in MINOR version
   - Removed in next MAJOR version
   - Minimum one cycle warning period

---

## Application to IIIF.Manifest.Serializer.Net

### Changes Analysis

| Change | Type | Version Impact |
|--------|------|----------------|
| Added Collection resource type | New feature (backward compatible) | MINOR |
| Refactored ViewingHint to class | API enhancement (compatible) | MINOR |
| Created test project | Development infrastructure | MINOR |
| Created example project | Documentation | MINOR |
| Changed null handling | Internal improvement (compatible) | MINOR |

### Version Decision: 1.0.0 → 1.1.0 ✅

**Rationale:**
- ✅ All changes are backward compatible
- ✅ Added new functionality (Collection type)
- ✅ Enhanced existing features (ViewingHint)
- ✅ No breaking changes to existing API
- ✅ Existing code continues to work unchanged

**NOT a PATCH (1.0.1) because:**
- Added significant new features (not just bug fixes)
- New resource type (Collection)
- Enhanced API capabilities

**NOT a MAJOR (2.0.0) because:**
- No breaking changes
- Existing manifests still work
- ViewingHint change is compatible (null vs Unspecified)
- Still targets IIIF Presentation API 2.0

---

## Files Updated

### 1. Directory.Build.props
```xml
<PropertyGroup Label="Versioning">
  <Version>1.1.0</Version>  <!-- Changed from 1.0.0 -->
</PropertyGroup>
```

### 2. CHANGELOG.md (Created)
- Comprehensive version history
- Detailed list of changes
- Upgrade guide
- Future roadmap

### 3. API_VERSIONING_GUIDELINES.md (Created)
- Complete SemVer analysis
- IIIF versioning practices
- Decision matrix for future versions
- Compatibility matrix

---

## Versioning Decision Matrix

Use this guide for future version increments:

### Increment MAJOR (2.0.0, 3.0.0, ...)

- ❌ Remove public methods/properties
- ❌ Rename classes or namespaces
- ❌ Change method signatures (parameters, return types)
- ❌ Target different IIIF API major version (2.x → 3.x)
- ❌ Drop framework support (.NET 4.5.1)
- ❌ Change serialization format (incompatible)

### Increment MINOR (1.2.0, 1.3.0, ...)

- ✅ Add new resource types (like Collection)
- ✅ Add new optional properties/methods
- ✅ Add IIIF features (backward compatible)
- ✅ Deprecate (not remove) functionality
- ✅ Add framework targets (.NET 8.0)
- ✅ Performance improvements (no API change)
- ✅ New helper methods/extensions

### Increment PATCH (1.1.1, 1.1.2, ...)

- 🐛 Bug fixes in serialization
- 🐛 Correct IIIF spec compliance issues
- 🐛 Security patches
- 🐛 Documentation fixes
- 🐛 Internal refactoring (no API impact)
- 🐛 Dependency updates (critical security)

---

## Version Compatibility

| Library Version | IIIF Presentation API | .NET Standard | .NET Framework | Status |
|----------------|----------------------|---------------|----------------|---------|
| **1.1.0** ⭐    | 2.0                  | 2.0, 2.1      | 4.5.1          | **Current** |
| 1.0.0          | 2.0                  | 2.0           | 4.5.1          | Stable  |
| 2.0.0 (future) | 3.0                  | 2.1+          | -              | Planned |

---

## Build Verification

✅ **All builds successful:**
```bash
dotnet build IIIF.Manifest.Serializer.Net.sln
dotnet build src/IIIF.Manifest.Serializer.Net/IIIF.Manifest.Serializer.Net.csproj -c Release
```

✅ **Version propagated:**
- Directory.Build.props: 1.1.0
- All projects inherit version
- Assembly version: 1.1.0
- File version: 1.1.0

---

## Next Steps Recommendations

### For Library Maintainers

1. **Tag Release in Git:**
   ```bash
   git tag -a v1.1.0 -m "Version 1.1.0 - Collection support and comprehensive tests"
   git push origin v1.1.0
   ```

2. **Create GitHub Release:**
   - Title: "v1.1.0 - Collection Support & Enhanced ViewingHint"
   - Copy changelog content
   - Attach compiled binaries

3. **Publish to NuGet:**
   ```bash
   dotnet pack src/IIIF.Manifest.Serializer.Net -c Release
   dotnet nuget push bin/Release/IIIF.Manifest.Serializer.Net.1.1.0.nupkg
   ```

4. **Update Documentation:**
   - Update main README with new version
   - Publish API documentation
   - Update examples in docs

### For Users

**Upgrading from 1.0.0 to 1.1.0:**

No code changes required! All existing code continues to work.

**Optional improvements:**
```csharp
// Old way (still works)
if (manifest.ViewingHint == ViewingHint.Unspecified) { }

// New way (recommended)
if (manifest.ViewingHint == null) { }

// New feature: Collections
var collection = new Collection("https://example.org/collection", 
                               new Label("My Collection"));
collection.AddManifest("https://example.org/manifest/1");
```

---

## Future Version Planning

### Version 1.2.0 (Next Minor)
Potential features:
- Additional IIIF 2.0 optional properties
- More helper methods
- Performance optimizations
- Additional framework targets

### Version 2.0.0 (Next Major)
Breaking changes:
- IIIF Presentation API 3.0 support
- New data model (items, AnnotationPage)
- Migration utilities from 2.0 to 3.0
- Drop .NET Framework 4.5.1 support
- Modern C# language features

---

## References & Resources

### Specifications
- 📘 **Semantic Versioning 2.0.0**: https://semver.org/spec/v2.0.0.html
- 📘 **IIIF SemVer Notes**: https://iiif.io/api/annex/notes/semver/
- 📘 **IIIF Presentation 2.0**: https://iiif.io/api/presentation/2.0/
- 📘 **IIIF Presentation 3.0**: https://iiif.io/api/presentation/3.0/

### Best Practices
- 📗 **Keep a Changelog**: https://keepachangelog.com/
- 📗 **Conventional Commits**: https://www.conventionalcommits.org/
- 📗 **NuGet Versioning**: https://learn.microsoft.com/en-us/nuget/concepts/package-versioning

### Documentation Created
- ✅ `CHANGELOG.md` - Version history and upgrade guide
- ✅ `API_VERSIONING_GUIDELINES.md` - Complete versioning analysis
- ✅ `VIEWINGHINT_IMPLEMENTATION.md` - ViewingHint refactoring details
- ✅ `IMPLEMENTATION_SUMMARY.md` - Overall implementation summary

---

## Conclusion

✅ **Successfully applied Semantic Versioning 2.0.0 and IIIF versioning guidelines**

The library is now versioned as **1.1.0**, reflecting:
- Backward-compatible new features (Collection)
- Enhanced API (ViewingHint refactoring)
- Comprehensive testing and examples
- Full IIIF Presentation API 2.0 compliance

The version number accurately communicates the scope and impact of changes to users, following industry best practices and IIIF community standards.

**Version: 1.1.0 ✅**
**Date: February 16, 2026**
**Status: Ready for Release**

