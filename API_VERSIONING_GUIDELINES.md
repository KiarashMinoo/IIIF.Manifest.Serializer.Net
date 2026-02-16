# API Versioning Analysis - SemVer 2.0.0 & IIIF Guidelines

## Semantic Versioning 2.0.0 Summary

Based on https://semver.org/spec/v2.0.0.html

### Version Format: MAJOR.MINOR.PATCH

Given a version number **MAJOR.MINOR.PATCH**, increment the:

1. **MAJOR** version when you make incompatible API changes (breaking changes)
2. **MINOR** version when you add functionality in a backward compatible manner
3. **PATCH** version when you make backward compatible bug fixes

### Key Rules from SemVer 2.0.0:

1. **Version 0.y.z is for initial development** - Anything may change at any time. The public API should not be considered stable.

2. **Version 1.0.0 defines the public API** - The way in which the version number is incremented after this release is dependent on this public API and how it changes.

3. **PATCH version Z (x.y.Z)** - MUST be incremented if only backward compatible bug fixes are introduced. A bug fix is defined as an internal change that fixes incorrect behavior.

4. **MINOR version Y (x.Y.z)** - MUST be incremented if new, backward compatible functionality is introduced to the public API. It MUST be incremented if any public API functionality is marked as deprecated. It MAY be incremented if substantial new functionality or improvements are introduced within the private code. It MAY include patch level changes. Patch version MUST be reset to 0 when minor version is incremented.

5. **MAJOR version X (X.y.z)** - MUST be incremented if any backward incompatible changes are introduced to the public API. It MAY also include minor and patch level changes. Patch and minor versions MUST be reset to 0 when major version is incremented.

6. **Pre-release versions** - MAY be denoted by appending a hyphen and a series of dot separated identifiers immediately following the patch version (e.g., 1.0.0-alpha, 1.0.0-alpha.1, 1.0.0-0.3.7, 1.0.0-x.7.z.92).

7. **Build metadata** - MAY be denoted by appending a plus sign and a series of dot separated identifiers immediately following the patch or pre-release version (e.g., 1.0.0-alpha+001, 1.0.0+20130313144700).

8. **Precedence** - Determined by the first difference when comparing each identifier from left to right: MAJOR, MINOR, and PATCH. Pre-release versions have lower precedence than the associated normal version.

### Breaking vs Non-Breaking Changes

**Breaking Changes (MAJOR):**
- Removing fields, properties, or methods
- Renaming fields, properties, or methods
- Changing data types
- Changing required vs optional status
- Changing default values that affect behavior
- Removing support for a feature
- Changing semantics of existing functionality

**Non-Breaking Changes (MINOR):**
- Adding new optional fields/properties
- Adding new methods/endpoints
- Adding new features
- Deprecating (but not removing) functionality
- Internal optimizations that don't affect API

**Bug Fixes (PATCH):**
- Fixing incorrect behavior
- Security patches
- Documentation corrections
- Internal refactoring with no API impact

## IIIF-Specific Versioning Guidelines

Based on https://iiif.io/api/annex/notes/semver/ and IIIF practices:

### IIIF API Versions

IIIF APIs follow Semantic Versioning with additional guidelines:

1. **API Context URLs include version**: 
   - Presentation API 2.0: `http://iiif.io/api/presentation/2/context.json`
   - Presentation API 3.0: `http://iiif.io/api/presentation/3/context.json`
   - Image API 2.1: `http://iiif.io/api/image/2/context.json`
   - Image API 3.0: `http://iiif.io/api/image/3/context.json`

2. **Major versions represent incompatible changes**:
   - Presentation API 2.x → 3.0 changed data model significantly
   - Changed from `sequences/canvases` to `items` (AnnotationPage)
   - Changed property names and structure

3. **Minor versions add features**:
   - Can add new optional properties
   - Can add new resource types
   - Must remain backward compatible

4. **Patch versions for corrections**:
   - Clarifications to specification
   - Bug fixes in reference implementations
   - Documentation improvements

### IIIF Versioning Best Practices

1. **Clients should support version negotiation** - Check @context to determine API version

2. **Servers should be explicit about versions** - Always include @context with full version URL

3. **Deprecated features** - Marked clearly in documentation, remain available for at least one minor version cycle

4. **Breaking changes** - Require new major version, old versions remain available

5. **Version stability** - Once published, a version specification does not change (except for editorial clarifications)

## Application to IIIF.Manifest.Serializer.Net

### Current Status
- **Current Version**: 1.0.0
- **Target API**: IIIF Presentation API 2.0
- **Framework**: .NET Standard 2.0, .NET Framework 4.5.1

### Versioning Strategy

Based on SemVer 2.0.0 and IIIF guidelines, the library should version as follows:

#### When to increment MAJOR (X.0.0):
- Breaking changes to public API
- Removing public methods or properties
- Changing method signatures
- Renaming classes or namespaces
- Targeting different IIIF Presentation API major version (2.x → 3.x)
- Changing serialization format in incompatible ways
- Dropping framework support (e.g., removing .NET 4.5.1)

#### When to increment MINOR (1.X.0):
- Adding new IIIF resource types (e.g., Collection - already done in 1.1.0)
- Adding new properties or methods (backward compatible)
- Adding support for additional IIIF features
- Deprecating (but not removing) functionality
- Adding new framework targets (e.g., adding .NET 8.0)
- Performance improvements with no API changes
- New helper methods or extension methods

#### When to increment PATCH (1.0.X):
- Bug fixes in serialization/deserialization
- Correcting incorrect IIIF spec compliance
- Performance optimizations (no API changes)
- Documentation updates
- Internal refactoring
- Dependency updates (for security)

### Recommended Next Version

Given the changes made:
- ✅ Added Collection resource type (new feature)
- ✅ Added ViewingHint enum/class (API enhancement)
- ✅ Created comprehensive tests
- ✅ Created examples

**Recommended version: 1.1.0**

Reasoning:
- Backward compatible additions (new Collection type)
- Enhanced ViewingHint implementation (improved but compatible)
- No breaking changes to existing API
- Follows MINOR version increment for new features

### Version Update Recommendation

Update these files:

1. **Directory.Build.props**:
   ```xml
   <PropertyGroup Label="Versioning">
     <Version>1.1.0</Version>
   </PropertyGroup>
   ```

2. **Package metadata** (if creating NuGet package):
   - PackageVersion: 1.1.0
   - Release notes: "Added Collection support, enhanced ViewingHint implementation, comprehensive tests and examples"

3. **CHANGELOG.md** (create if doesn't exist):
   ```markdown
   ## [1.1.0] - 2026-02-16
   
   ### Added
   - Collection resource type for organizing manifests
   - ViewingHint class-based implementation (replacing enum)
   - Comprehensive unit test project with xUnit
   - Example project with 5 practical scenarios
   - Full IIIF Presentation API 2.0 compliance
   
   ### Changed
   - ViewingHint now uses null instead of Unspecified value
   - Improved consistency across property types
   
   ### Fixed
   - Various serialization/deserialization improvements
   ```

## Compatibility Matrix

| Library Version | IIIF Presentation API | .NET Standard | .NET Framework | Status |
|----------------|----------------------|---------------|----------------|---------|
| 1.0.0          | 2.0                  | 2.0           | 4.5.1          | Stable  |
| 1.1.0          | 2.0                  | 2.0, 2.1      | 4.5.1          | Current |
| 2.0.0 (future) | 3.0                  | 2.1           | -              | Planned |

## References

- **Semantic Versioning 2.0.0**: https://semver.org/spec/v2.0.0.html
- **IIIF SemVer Notes**: https://iiif.io/api/annex/notes/semver/
- **IIIF Presentation API 2.0**: https://iiif.io/api/presentation/2.0/
- **IIIF Presentation API 3.0**: https://iiif.io/api/presentation/3.0/

## Conclusion

The library should be versioned as **1.1.0** for the current changes, following SemVer 2.0.0 guidelines. All new features are backward compatible, no breaking changes have been introduced, and the library maintains full IIIF Presentation API 2.0 compliance.

