# ServiceJsonConverter.cs - Issues Fixed

**Date:** February 23, 2026  
**File:** `src/IIIF.Manifest.Serializer.Net/Shared/ServiceJsonConverter.cs`

## Summary

Fixed critical compilation errors and improved code quality in the ServiceJsonConverter class that handles IIIF service deserialization.

---

## Critical Issues Fixed

### 1. **Undefined Variable 'serviceToken' (Compilation Error)**
- **Location:** Line 27 in ReadJson method
- **Issue:** Variable `serviceToken` was referenced but never declared
- **Root Cause:** Incomplete refactoring from manual JSON reading to JToken-based approach
- **Fix:** 
  - Changed from manual `JsonReader.Read()` loop to `JToken.Load(reader)`
  - Added proper iteration over JArray elements
  - Declared `serviceToken` variable in foreach loop
- **Impact:** Code now compiles and properly handles service arrays

### 2. **Undefined Variable 'jService' (Compilation Error)**
- **Location:** Line 35 in ReadJson method
- **Issue:** Variable `jService` was used but never declared
- **Fix:** 
  - Renamed to `jToken` for consistency
  - Used pattern matching: `jToken is JObject jObject`
  - Pass `jObject` to `DetectAndDeserializeService`
- **Impact:** Code now compiles and properly handles single service objects

### 3. **Invalid Generic Type Reference (Compilation Error)**
- **Location:** Line 45 in DetectAndDeserializeService method
- **Issue:** `BaseItem<TBaseItem>.TypeJName` referenced non-existent generic parameter `TBaseItem`
- **Root Cause:** Copy-paste error from another converter that works with generic BaseItem
- **Fix:** 
  - Added private const `TypeJName = "@type"`
  - Added private const `ProfileJName = "profile"`
  - Used local constants instead of external reference
- **Impact:** Code now compiles without generic type errors

---

## Code Quality Improvements

### 4. **Empty Catch Blocks (Anti-pattern)**
- **Location:** Lines 84, 104
- **Issue:** Empty `catch` blocks swallow all exceptions silently
- **Fix:** 
  - Changed to catch specific `JsonException` type
  - Added descriptive comments explaining fallback behavior
  - Nested try-catch for Auth service detection with proper fallthrough
- **Impact:** Better error handling and debugging capability

### 5. **Magic Strings**
- **Issue:** Hard-coded strings "@type" and "profile" throughout the code
- **Fix:** Added constants at class level
  ```csharp
  private const string TypeJName = "@type";
  private const string ProfileJName = "profile";
  ```
- **Impact:** Easier maintenance and reduced typo risk

### 6. **Case-Sensitive String Comparisons**
- **Location:** Lines 79-98 in profile matching
- **Issue:** `Contains("auth")` is case-sensitive, could miss "Auth", "AUTH", etc.
- **Fix:** Added `StringComparison.OrdinalIgnoreCase` to all `Contains` calls
  ```csharp
  profileValue.Contains("auth", StringComparison.OrdinalIgnoreCase)
  ```
- **Impact:** More robust profile detection

---

## Documentation Improvements

### 7. **Missing XML Documentation**
- **Issue:** Class and methods lacked comprehensive documentation
- **Fix:** Added full XML documentation comments for:
  - Class-level summary explaining converter purpose and capabilities
  - `CanConvert` method
  - `ReadJson` method with parameter and return value descriptions
  - `DetectAndDeserializeService` method with detection strategy explanation
  - `WriteJson` method
- **Impact:** Better IDE IntelliSense and code maintainability

---

## Code Structure Improvements

### 8. **Improved ReadJson Logic**
- **Before:** Mixed token types and manual reading
- **After:** Clean pattern matching with JToken types
  ```csharp
  var jToken = JToken.Load(reader);
  
  if (jToken is JArray jArray) { ... }
  else if (jToken is JObject jObject) { ... }
  ```
- **Impact:** More readable and maintainable

### 9. **Consistent Exception Handling**
- **Before:** Mixed catch-all and specific exceptions
- **After:** Consistent `JsonException` catching throughout
- **Impact:** Predictable error behavior

---

## Service Type Support

The converter now properly supports detection for:

### By @type Field:
- **Image API:** `ImageService2`, `ImageService3`
- **Auth API v1:** `AuthCookieService1`, `AuthTokenService1`, `AuthLogoutService1`
- **Auth API v2:** `AuthProbeService2`, `AuthAccessService2`, `AuthAccessTokenService2`, `AuthLogoutService2`
- **Search API:** `SearchService2`
- **Auto-complete:** `AutoCompleteService2`
- **Discovery:** `OrderedCollection`
- **Content State:** `ContentStateService`

### By profile Field (case-insensitive):
- **auth** → AuthService1 or AuthService2
- **search** → SearchService
- **discovery** → DiscoveryService
- **content-state** → ContentStateService
- **image** → Service (ImageService)

### Fallback:
- Defaults to `Service` (ImageService) if detection fails

---

## Testing Recommendations

1. **Test array handling:** Service arrays with multiple types
2. **Test single object:** Single service object deserialization
3. **Test type detection:** Each @type value
4. **Test profile detection:** Each profile pattern (case variations)
5. **Test fallback:** Unknown service types
6. **Test case sensitivity:** Mixed-case profile values
7. **Test malformed JSON:** Invalid service structures

---

## Related Files

- `IBaseService.cs` - Base interface for all services
- `Service.cs` - Image service implementation
- `AuthService1.cs` / `AuthService2.cs` - Auth service implementations
- `SearchService.cs` - Search service implementation
- `DiscoveryService.cs` - Discovery service implementation
- `ContentStateService.cs` - Content state service implementation

---

## Migration Notes

All changes are backward compatible. No breaking changes to public API or serialization format.

The improved case-insensitive profile matching may now successfully deserialize services that previously failed, which is an improvement rather than a breaking change.

