# Value Objects Implementation Summary

## Overview

This document summarizes the value objects created to improve code fluency following the pattern established by `ViewingDirection`. These value objects provide:

1. **Static properties** for common values with proper IIIF specification naming
2. **String constructor** for custom/non-standard values
3. **Automatic JSON serialization** via `ValuableItemJsonConverter`
4. **Full backward compatibility** with string overloads

## Pattern Used

All value objects follow the `ViewingDirection` pattern:

```csharp
[JsonConverter(typeof(ValuableItemJsonConverter<ValueType>))]
public class ValueType : ValuableItem<ValueType>
{
    public ValueType(string value) : base(value) { }
    
    // Static properties for common values
    public static ValueType CommonValue => new ValueType("common-value");
}
```

## Value Objects Created

### 1. ImageFormat (`Properties/ImageFormat.cs`)

IIIF Image API format values:

| Property | Value | Description |
|----------|-------|-------------|
| `Jpg` | `"jpg"` | JPEG format |
| `Png` | `"png"` | PNG format |
| `Gif` | `"gif"` | GIF format |
| `Webp` | `"webp"` | WebP format |
| `Tif` | `"tif"` | TIFF format |
| `Jp2` | `"jp2"` | JPEG 2000 |
| `Pdf` | `"pdf"` | PDF format |
| `Avif` | `"avif"` | AVIF format |
| `Heic` | `"heic"` | HEIC format |

**Usage:**
```csharp
service.AddPreferredFormat(ImageFormat.Webp);
service.AddPreferredFormat(ImageFormat.Jpg);
// Or custom:
service.AddPreferredFormat(new ImageFormat("custom-format"));
// Or string (backward compatible):
service.AddPreferredFormat("webp");
```

### 2. ImageQuality (`Properties/ImageQuality.cs`)

IIIF Image API quality values:

| Property | Value | Description |
|----------|-------|-------------|
| `Default` | `"default"` | Default quality |
| `Color` | `"color"` | Full color |
| `Gray` | `"gray"` | Grayscale |
| `Bitonal` | `"bitonal"` | Black and white |

**Usage:**
```csharp
service.AddExtraQuality(ImageQuality.Bitonal);
service.AddExtraQuality(ImageQuality.Gray);
```

### 3. ImageFeature (`Properties/ImageFeature.cs`)

IIIF Image API feature values:

| Property | Value | Description |
|----------|-------|-------------|
| `RegionByPx` | `"regionByPx"` | Region by pixel |
| `RegionByPct` | `"regionByPct"` | Region by percentage |
| `RegionSquare` | `"regionSquare"` | Square region |
| `SizeByH` | `"sizeByH"` | Size by height |
| `SizeByW` | `"sizeByW"` | Size by width |
| `SizeByPct` | `"sizeByPct"` | Size by percentage |
| `SizeByWh` | `"sizeByWh"` | Size by width and height |
| `SizeByConfinedWh` | `"sizeByConfinedWh"` | Confined dimensions |
| `SizeUpscaling` | `"sizeUpscaling"` | Upscaling allowed |
| `RotationBy90s` | `"rotationBy90s"` | 90° rotation |
| `RotationArbitrary` | `"rotationArbitrary"` | Arbitrary rotation |
| `Mirroring` | `"mirroring"` | Mirroring support |
| `BaseUriRedirect` | `"baseUriRedirect"` | URI redirect |
| `Cors` | `"cors"` | CORS support |
| `JsonldMediaType` | `"jsonldMediaType"` | JSON-LD media type |
| `ProfileLinkHeader` | `"profileLinkHeader"` | Profile link header |
| `CanonicalLinkHeader` | `"canonicalLinkHeader"` | Canonical link |

**Usage:**
```csharp
service.AddExtraFeature(ImageFeature.RegionByPx);
service.AddExtraFeature(ImageFeature.RotationArbitrary);
```

### 4. Behavior (`Properties/Behavior.cs`)

IIIF Presentation API 3.0 behavior values (also applicable as viewingHint in 2.0):

| Property | Value | Description |
|----------|-------|-------------|
| `Paged` | `"paged"` | Separate pages |
| `Continuous` | `"continuous"` | Continuous scroll |
| `Individuals` | `"individuals"` | Individual items |
| `Unordered` | `"unordered"` | Unordered items |
| `FacingPages` | `"facing-pages"` | Side-by-side |
| `NonPaged` | `"non-paged"` | Hidden by default |
| `Sequence` | `"sequence"` | Range as sequence |
| `ThumbnailNav` | `"thumbnail-nav"` | Thumbnail navigation |
| `NoNav` | `"no-nav"` | No navigation |
| `AutoAdvance` | `"auto-advance"` | Auto-advance |
| `NoAutoAdvance` | `"no-auto-advance"` | No auto-advance |
| `Repeat` | `"repeat"` | Repeat playback |
| `NoRepeat` | `"no-repeat"` | No repeat |
| `MultiPart` | `"multi-part"` | Multi-part |
| `Together` | `"together"` | View together |
| `Hidden` | `"hidden"` | Hidden annotation |

### 5. Motivation (`Properties/Motivation.cs`)

IIIF/Web Annotation motivation values:

| Property | Value | Description |
|----------|-------|-------------|
| `Painting` | `"painting"` | IIIF painting |
| `Supplementing` | `"supplementing"` | Supplementing content |
| `Commenting` | `"commenting"` | Comments |
| `Describing` | `"describing"` | Descriptions |
| `Tagging` | `"tagging"` | Tags |
| `Classifying` | `"classifying"` | Classifications |
| `Linking` | `"linking"` | Links |
| `Identifying` | `"identifying"` | Identifications |
| `Bookmarking` | `"bookmarking"` | Bookmarks |
| `Highlighting` | `"highlighting"` | Highlights |
| `Editing` | `"editing"` | Edits |
| `Replying` | `"replying"` | Replies |
| `Assessing` | `"assessing"` | Assessments |
| `Moderating` | `"moderating"` | Moderation |
| `Questioning` | `"questioning"` | Questions |
| `ScPainting` | `"sc:painting"` | IIIF 2.0 painting |

### 6. ResourceType (`Properties/ResourceType.cs`)

IIIF resource type values:

| Property | Value | Description |
|----------|-------|-------------|
| `Collection` | `"Collection"` | IIIF 3.0 Collection |
| `Manifest` | `"Manifest"` | IIIF 3.0 Manifest |
| `Canvas` | `"Canvas"` | IIIF 3.0 Canvas |
| `Range` | `"Range"` | IIIF 3.0 Range |
| `AnnotationPage` | `"AnnotationPage"` | Annotation Page |
| `Annotation` | `"Annotation"` | Web Annotation |
| `AnnotationCollection` | `"AnnotationCollection"` | Collection |
| `Image` | `"Image"` | Image content |
| `Video` | `"Video"` | Video content |
| `Sound` | `"Sound"` | Audio content |
| `Text` | `"Text"` | Text content |
| `Dataset` | `"Dataset"` | Dataset |
| `Model` | `"Model"` | 3D Model |
| `ImageService2` | `"ImageService2"` | Image Service 2 |
| `ImageService3` | `"ImageService3"` | Image Service 3 |
| `SearchService2` | `"SearchService2"` | Search Service 2 |
| `AuthProbeService2` | `"AuthProbeService2"` | Auth Probe 2 |
| `ScCollection` | `"sc:Collection"` | IIIF 2.0 Collection |
| `ScManifest` | `"sc:Manifest"` | IIIF 2.0 Manifest |
| `ScCanvas` | `"sc:Canvas"` | IIIF 2.0 Canvas |
| `OaAnnotation` | `"oa:Annotation"` | OA Annotation |
| `DctypesImage` | `"dctypes:Image"` | DCTypes Image |

### 7. Profile (`Properties/Profile.cs`)

IIIF compliance level profiles:

| Property | Value | Description |
|----------|-------|-------------|
| `Level0` | `"level0"` | Level 0 compliance |
| `Level1` | `"level1"` | Level 1 compliance |
| `Level2` | `"level2"` | Level 2 compliance |
| `ImageApi2Level0` | Full URL | Image API 2.0 Level 0 |
| `ImageApi2Level1` | Full URL | Image API 2.0 Level 1 |
| `ImageApi2Level2` | Full URL | Image API 2.0 Level 2 |
| `AuthLogin` | Full URL | Auth login profile |
| `AuthClickthrough` | Full URL | Auth clickthrough |
| `AuthKiosk` | Full URL | Auth kiosk |
| `AuthExternal` | Full URL | Auth external |
| `AuthToken` | Full URL | Auth token |
| `AuthLogout` | Full URL | Auth logout |

### 8. Context (`Properties/Context.cs`)

IIIF API context URLs:

| Property | Value | Description |
|----------|-------|-------------|
| `Presentation2` | Full URL | Presentation API 2.0 |
| `Presentation3` | Full URL | Presentation API 3.0 |
| `Image2` | Full URL | Image API 2.0 |
| `Image3` | Full URL | Image API 3.0 |
| `Auth1` | Full URL | Auth API 1.0 |
| `Auth2` | Full URL | Auth API 2.0 |
| `Search1` | Full URL | Search API 1.0 |
| `Search2` | Full URL | Search API 2.0 |
| `Discovery1` | Full URL | Discovery API 1.0 |
| `ContentState1` | Full URL | Content State 1.0 |
| `WebAnnotation` | Full URL | W3C Web Annotation |

### 9. Rights (`Properties/Rights.cs`)

Standard rights statement URLs:

| Property | Value | Description |
|----------|-------|-------------|
| `CcBy` | CC BY 4.0 URL | Attribution |
| `CcBySa` | CC BY-SA 4.0 URL | Attribution-ShareAlike |
| `CcByNd` | CC BY-ND 4.0 URL | Attribution-NoDerivs |
| `CcByNc` | CC BY-NC 4.0 URL | Attribution-NonCommercial |
| `CcByNcSa` | CC BY-NC-SA 4.0 URL | Attribution-NonCommercial-ShareAlike |
| `CcByNcNd` | CC BY-NC-ND 4.0 URL | Attribution-NonCommercial-NoDerivs |
| `Cc0` | CC0 URL | Public Domain Dedication |
| `PublicDomain` | PDM URL | Public Domain Mark |
| `InCopyright` | RS InC URL | In Copyright |
| `NoKnownCopyright` | RS NKC URL | No Known Copyright |
| ... | ... | Additional RightsStatements.org values |

### 10. Language (`Properties/Language.cs`)

BCP 47 language tags:

| Property | Value | Description |
|----------|-------|-------------|
| `English` | `"en"` | English |
| `EnglishUs` | `"en-US"` | US English |
| `EnglishGb` | `"en-GB"` | British English |
| `French` | `"fr"` | French |
| `German` | `"de"` | German |
| `Spanish` | `"es"` | Spanish |
| `Italian` | `"it"` | Italian |
| `Portuguese` | `"pt"` | Portuguese |
| `Dutch` | `"nl"` | Dutch |
| `Russian` | `"ru"` | Russian |
| `Chinese` | `"zh"` | Chinese |
| `Japanese` | `"ja"` | Japanese |
| `Korean` | `"ko"` | Korean |
| `Arabic` | `"ar"` | Arabic |
| `Hebrew` | `"he"` | Hebrew |
| `Latin` | `"la"` | Latin |
| `Greek` | `"el"` | Greek |
| `None` | `"none"` | No language |

### 11. TimeMode (`Properties/TimeMode.cs`)

IIIF temporal behavior values:

| Property | Value | Description |
|----------|-------|-------------|
| `Trim` | `"trim"` | Trim to duration |
| `Scale` | `"scale"` | Scale to duration |
| `Loop` | `"loop"` | Loop playback |

## Updated Service Class

The `Service` class has been updated to use value objects with both value object and string overloads:

```csharp
// Value object overloads (recommended)
service.AddPreferredFormat(ImageFormat.Webp);
service.AddExtraQuality(ImageQuality.Bitonal);
service.AddExtraFeature(ImageFeature.RegionByPx);
service.SetRights(Rights.CcBy);

// String overloads (backward compatible, custom values)
service.AddPreferredFormat("custom-format");
service.AddExtraQuality("custom-quality");
service.AddExtraFeature("custom-feature");
service.SetRights("http://example.org/rights");
```

## Benefits

1. **IntelliSense Support**: Static properties appear in IDE autocomplete
2. **Type Safety**: Compile-time verification of common values
3. **Discoverability**: All valid values easily visible
4. **Flexibility**: Custom values supported via string constructor
5. **Backward Compatible**: String overloads still work
6. **JSON Compatible**: Serializes to plain strings
7. **Specification Aligned**: Values match IIIF specifications

## Test Coverage

All value objects have comprehensive tests in `ValueObjectTests.cs`:
- Static property value verification
- Custom value creation
- JSON serialization/deserialization
- Round-trip consistency

## Files Created

| File | Description |
|------|-------------|
| `Properties/ImageFormat.cs` | Image format values |
| `Properties/ImageQuality.cs` | Image quality values |
| `Properties/ImageFeature.cs` | Image API features |
| `Properties/Behavior.cs` | Presentation behaviors |
| `Properties/Motivation.cs` | Annotation motivations |
| `Properties/ResourceType.cs` | IIIF resource types |
| `Properties/Profile.cs` | Compliance profiles |
| `Properties/Context.cs` | API context URLs |
| `Properties/Rights.cs` | Rights statements |
| `Properties/Language.cs` | Language codes |
| `Properties/TimeMode.cs` | Temporal behaviors |

Total: **11 new value object classes**

