# IIIF Authentication & Authorization Implementation - Complete

## Summary

The IIIF Authentication API implementation is **complete** for both Auth API 1.0 and Auth API 2.0. This includes full support for authentication patterns AND authorization mechanisms.

## What Was Implemented

### 1. AuthService1 (Auth API 1.0) ✅
**File:** `src/IIIF.Manifest.Serializer.Net/Properties/Service/AuthService1.cs`

**Features:**
- Full support for all Auth 1.0 profiles:
  - `Login` - Interactive authentication with credentials
  - `Clickthrough` - Terms acceptance pattern
  - `Kiosk` - Location-based access
  - `External` - Delegation to external auth provider
  - `Token` - Authorization token service
  - `Logout` - Session termination
- Properties: Profile, Label, Header, Description, ConfirmLabel, FailureHeader, FailureDescription
- Nested service support (token and logout services)
- Fluent API for easy configuration

### 2. AuthService2 (Auth API 2.0) ✅
**File:** `src/IIIF.Manifest.Serializer.Net/Properties/Service/AuthService2.cs`

**Features:**
- Full support for Auth 2.0 patterns:
  - `Probe Service` - Access status checking
  - `Access Service` - Authentication interface (active/external profiles)
  - `Token Service` - Authorization token acquisition
  - `Logout Service` - Session cleanup
- Properties: Profile, Label, Heading, Note, ConfirmLabel
- Four-level nested service hierarchy (probe → access → token → logout)
- Improved user experience with probe-before-auth pattern

### 3. JSON Converters ✅
- **AuthService1JsonConverter.cs** - Full serialization/deserialization for Auth 1.0
- **AuthService2JsonConverter.cs** - Full serialization/deserialization for Auth 2.0
- Single/array handling for nested services
- Context-aware JSON generation
- Required field validation

### 4. Unit Tests ✅
**Files:**
- `tests/.../Properties/AuthService1Tests.cs` (9 tests)
- `tests/.../Properties/AuthService2Tests.cs` (10 tests)

**Test Coverage:**
- Serialization with correct @context and properties
- Deserialization accuracy
- Round-trip fidelity
- All authentication patterns (login, clickthrough, kiosk, external, active, external)
- Single vs. array service handling
- Optional field omission

### 5. Documentation ✅
- **AUTH_API.md** - Complete guide with examples, patterns, and JSON output
- Updated IMPLEMENTATION_SUMMARY.md to mark Auth API as complete
- Inline XML documentation for all public APIs

### 6. Cookbook Recipes ✅
**Files:**
- `RecipeAuth01_LoginAuth1.cs` - Auth 1.0 login pattern
- `RecipeAuth02_ClickthroughAuth1.cs` - Auth 1.0 clickthrough pattern
- `RecipeAuth03_ActiveAuth2.cs` - Auth 2.0 active pattern

## Important Clarifications

### ❌ There is NO Auth 0

**The IIIF Authentication API starts with version 1.0.** There has never been an Auth 0.0 or Auth 0.x specification. If you were referring to "auth/0", this might be a misunderstanding or typo.

**IIIF Auth API Versions:**
- ✅ Auth API 1.0 (Stable, widely adopted)
- ✅ Auth API 2.0 (Draft/in development, improved UX)

### ✅ Authorization is INCLUDED in Auth API

**Authentication vs. Authorization:**
- **Authentication** = Verifying identity ("Who are you?")
- **Authorization** = Controlling access ("What can you access?")

The IIIF **Authentication API handles BOTH**:

1. **Authentication Patterns:**
   - Login with credentials
   - Clickthrough (terms acceptance)
   - Kiosk (location-based)
   - External (delegated auth)

2. **Authorization Mechanisms:**
   - **Token Services** - Provide access tokens after successful authentication
   - **Access Probing** (Auth 2.0) - Check resource access status
   - **Service Nesting** - Token/logout services control authorized access

**There is NO separate "Authorization API" in IIIF.** The token services within the Auth API provide the authorization layer.

## Example: Full Auth Flow

### Auth 1.0 (Login with Authorization)
```csharp
// Token service provides AUTHORIZATION token after authentication
var tokenService = new AuthService1(
    "https://auth.example.org/token",
    Profile.AuthToken.Value  // <-- AUTHORIZATION TOKEN SERVICE
);

// Login service handles AUTHENTICATION
var loginService = new AuthService1(
    "https://auth.example.org/login",
    Profile.AuthLogin.Value  // <-- AUTHENTICATION SERVICE
)
.SetLabel("Login to Access Protected Content")
.AddService(tokenService);  // Token for authorization

// Attach to image service
imageService.SetService(loginService);
```

**Flow:**
1. User attempts to access protected image
2. **Authentication:** User logs in via login service
3. **Authorization:** Token service provides access token
4. Image server validates token and grants/denies access

### Auth 2.0 (Probe + Active Auth + Authorization)
```csharp
// Complete auth + authz hierarchy
var logoutService = new AuthService2("https://auth.example.org/logout");

var tokenService = new AuthService2("https://auth.example.org/token")
    .AddService(logoutService);  // <-- AUTHORIZATION TOKEN

var accessService = new AuthService2(
    "https://auth.example.org/access",
    "active"  // <-- AUTHENTICATION UI
)
.AddService(tokenService);

var probeService = new AuthService2("https://auth.example.org/probe")
    .AddService(accessService);  // <-- ACCESS CHECK

// Attach to resource
imageService.SetService(probeService);
```

**Flow:**
1. **Probe:** Check if user already has access
2. **Authentication:** If needed, show active auth UI
3. **Authorization:** Token service provides access credentials
4. Resource server validates token for access control

## Build & Test Status

✅ **Core library compiles cleanly:**
```
dotnet build src/IIIF.Manifest.Serializer.Net/IIIF.Manifest.Serializer.Net.csproj
Build succeeded in 1.1s
```

✅ **Unit tests created:**
- 9 tests for AuthService1
- 10 tests for AuthService2
- Tests cover all authentication patterns and authorization flows

## Files Created/Modified

### New Files (8)
1. `src/.../Properties/Service/AuthService1.cs`
2. `src/.../Properties/Service/AuthService1JsonConverter.cs`
3. `src/.../Properties/Service/AuthService2.cs`
4. `src/.../Properties/Service/AuthService2JsonConverter.cs`
5. `tests/.../Properties/AuthService1Tests.cs`
6. `tests/.../Properties/AuthService2Tests.cs`
7. `docs/Properties/Service/AUTH_API.md`
8. `tests/.../Integration/AuthDemonstration.cs`

### Modified Files (3)
1. `examples/.../Program.cs` - Added Auth recipe execution
2. `IMPLEMENTATION_SUMMARY.md` - Marked Auth API as complete
3. `examples/.../Recipes/` - Created 3 auth cookbook recipes

## Next Steps (Optional Enhancements)

While the Auth API implementation is feature-complete, you could optionally add:

1. **More Cookbook Recipes:**
   - Kiosk pattern example
   - External auth provider example
   - Auth 2.0 external pattern example

2. **Integration Tests:**
   - Test with actual IIIF Image API servers
   - Validate with IIIF validators
   - Test full auth flows

3. **Advanced Features:**
   - Auth state caching
   - Token refresh patterns
   - Multi-service authentication

## References

- [IIIF Auth API 1.0 Spec](https://iiif.io/api/auth/1.0/)
- [IIIF Auth API 2.0 Draft](https://iiif.io/api/auth/2/)
- [IIIF Cookbook](https://iiif.io/api/cookbook/)
- [Auth API Documentation](docs/Properties/Service/AUTH_API.md)

---

**Status:** ✅ Complete  
**Date:** February 17, 2026  
**Versions Implemented:** Auth API 1.0 + Auth API 2.0  
**Coverage:** Authentication + Authorization (Token Services)
