# IIIF Authentication API Implementation

## Overview

This library implements support for **IIIF Authentication API 1.0** and **IIIF Authentication API 2.0**, providing complete authentication and authorization patterns for controlled access to IIIF resources.

## Implementation Status

| API Version | Status | Classes | Profiles Supported |
|-------------|--------|---------|-------------------|
| **Auth 1.0** | ✅ Complete | `AuthService1` | Login, Clickthrough, Kiosk, External, Token, Logout |
| **Auth 2.0** | ✅ Complete | `AuthService2` | Active, External, Probe, Access, Token, Logout |

**Note:** There is no Auth API 0.x in IIIF specifications. The Authentication API begins with version 1.0.

## Architecture

### Auth API 1.0

Auth 1.0 uses a hierarchical service structure where the main authentication service (login/clickthrough/kiosk/external) contains nested token and logout services.

**Service Hierarchy:**
```
AuthService (login/clickthrough/kiosk/external)
├── Token Service
└── Logout Service
```

**Profiles:**
- `AuthLogin` - Interactive login with credentials
- `AuthClickthrough` - Terms acceptance pattern (no credentials)
- `AuthKiosk` - Physical location-based access
- `AuthExternal` - Delegation to external authentication provider
- `AuthToken` - Token acquisition service
- `AuthLogout` - Session termination service

### Auth API 2.0

Auth 2.0 introduces a probe-based pattern for checking access status before presenting authentication UI, improving user experience.

**Service Hierarchy:**
```
Probe Service
└── Access Service (active/external)
    └── Token Service
        └── Logout Service
```

**Service Types:**
- `AuthProbeService2` - Entry point for access checking
- `AuthAccessService2` - Authentication interface (active or external)
- `AuthAccessTokenService2` - Token acquisition
- `AuthLogoutService2` - Session termination

## Usage Examples

### Auth 1.0 - Login Pattern

```csharp
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.Service;

// Create token service
var tokenService = new AuthService1(
    "https://auth.example.org/token",
    Profile.AuthToken.Value
);

// Create logout service
var logoutService = new AuthService1(
    "https://auth.example.org/logout",
    Profile.AuthLogout.Value
).SetLabel("Logout");

// Create login service with nested services
var loginService = new AuthService1(
    "https://auth.example.org/login",
    Profile.AuthLogin.Value
)
.SetLabel("Login to View Content")
.SetHeader("Authentication Required")
.SetDescription("Please log in with your credentials.")
.SetConfirmLabel("Login")
.SetFailureHeader("Login Failed")
.SetFailureDescription("Authentication unsuccessful.")
.AddService(tokenService)
.AddService(logoutService);

// Attach to image service
imageService.SetService(loginService);
```

### Auth 1.0 - Clickthrough Pattern

```csharp
var tokenService = new AuthService1(
    "https://auth.example.org/clickthrough/token",
    Profile.AuthToken.Value
);

var clickthroughService = new AuthService1(
    "https://auth.example.org/clickthrough",
    Profile.AuthClickthrough.Value
)
.SetLabel("Terms of Use")
.SetDescription("By clicking Accept, you agree to the terms.")
.SetConfirmLabel("Accept")
.AddService(tokenService);
```

### Auth 2.0 - Active Pattern

```csharp
// Create nested service structure
var logoutService = new AuthService2("https://auth.example.org/logout");

var tokenService = new AuthService2("https://auth.example.org/token")
    .AddService(logoutService);

var accessService = new AuthService2(
    "https://auth.example.org/access",
    "active" // profile
)
.SetLabel("Login Required")
.SetHeading("Secure Content")
.SetNote("Authentication required to access this content.")
.SetConfirmLabel("Login")
.AddService(tokenService);

var probeService = new AuthService2("https://auth.example.org/probe")
    .AddService(accessService);

// Attach to image service
imageService.SetService(probeService);
```

### Auth 2.0 - External Pattern

```csharp
var accessService = new AuthService2(
    "https://auth.example.org/access",
    "external" // profile for external auth provider
)
.SetLabel("External Authentication")
.SetHeading("Third-Party Login")
.SetNote("You will be redirected to an external authentication provider.");
```

## JSON Serialization

### Auth 1.0 Output

```json
{
  "@context": "http://iiif.io/api/auth/1/context.json",
  "@id": "https://auth.example.org/login",
  "profile": "http://iiif.io/api/auth/1/login",
  "label": "Login to View Content",
  "header": "Authentication Required",
  "description": "Please log in with your credentials.",
  "confirmLabel": "Login",
  "failureHeader": "Login Failed",
  "failureDescription": "Authentication unsuccessful.",
  "service": [
    {
      "@context": "http://iiif.io/api/auth/1/context.json",
      "@id": "https://auth.example.org/token",
      "profile": "http://iiif.io/api/auth/1/token"
    },
    {
      "@context": "http://iiif.io/api/auth/1/context.json",
      "@id": "https://auth.example.org/logout",
      "profile": "http://iiif.io/api/auth/1/logout",
      "label": "Logout"
    }
  ]
}
```

### Auth 2.0 Output

```json
{
  "@context": "http://iiif.io/api/auth/2/context.json",
  "@id": "https://auth.example.org/probe",
  "service": {
    "@context": "http://iiif.io/api/auth/2/context.json",
    "@id": "https://auth.example.org/access",
    "profile": "active",
    "label": "Login Required",
    "heading": "Secure Content",
    "note": "Authentication required to access this content.",
    "confirmLabel": "Login",
    "service": {
      "@context": "http://iiif.io/api/auth/2/context.json",
      "@id": "https://auth.example.org/token",
      "service": {
        "@context": "http://iiif.io/api/auth/2/context.json",
        "@id": "https://auth.example.org/logout"
      }
    }
  }
}
```

## Properties

### AuthService1 (Auth 1.0)

| Property | Type | Required | Description |
|----------|------|----------|-------------|
| `Id` | string | ✅ | Service endpoint URL |
| `Profile` | string | ✅ | Auth profile (login/clickthrough/kiosk/external/token/logout) |
| `Label` | string | ❌ | User-facing service label |
| `Header` | string | ❌ | Authentication dialog header |
| `Description` | string | ❌ | Explanatory text |
| `ConfirmLabel` | string | ❌ | Confirmation button label |
| `FailureHeader` | string | ❌ | Failure dialog header |
| `FailureDescription` | string | ❌ | Failure explanatory text |
| `Services` | collection | ❌ | Nested services (token, logout) |

### AuthService2 (Auth 2.0)

| Property | Type | Required | Description |
|----------|------|----------|-------------|
| `Id` | string | ✅ | Service endpoint URL |
| `Profile` | string | ❌ | Auth profile (active/external for access services) |
| `Label` | string | ❌ | User-facing service label |
| `Heading` | string | ❌ | Authentication interface heading |
| `Note` | string | ❌ | Explanatory note |
| `ConfirmLabel` | string | ❌ | Confirmation button label |
| `Services` | collection | ❌ | Nested services (access, token, logout) |

## Fluent API

Both `AuthService1` and `AuthService2` support fluent method chaining:

```csharp
var service = new AuthService1(id, profile)
    .SetLabel("Login")
    .SetHeader("Header Text")
    .SetDescription("Description Text")
    .AddService(tokenService)
    .AddService(logoutService);
```

## Testing

Comprehensive unit tests are provided in:
- `AuthService1Tests.cs` - 9 tests covering all Auth 1.0 patterns
- `AuthService2Tests.cs` - 10 tests covering all Auth 2.0 patterns

Tests verify:
- Serialization correctness
- Deserialization accuracy
- Round-trip fidelity
- Single vs. array service handling
- All authentication patterns (login, clickthrough, kiosk, external, active)

## Authorization vs Authentication

The IIIF **Authentication API** handles both:
1. **Authentication** - Verifying user identity (login, clickthrough, kiosk, external patterns)
2. **Authorization** - Controlling resource access (token services, access probing)

There is no separate Authorization API in IIIF. Token services provide authorization tokens that grant access to protected resources after successful authentication.

## References

- [IIIF Authentication API 1.0 Specification](https://iiif.io/api/auth/1.0/)
- [IIIF Authentication API 2.0 (Draft)](https://iiif.io/api/auth/2/)
- [IIIF Cookbook - Authentication Recipes](https://iiif.io/api/cookbook/recipe/)

## See Also

- [Service.cs](../../src/IIIF.Manifest.Serializer.Net/Properties/Service/Service.cs) - Image API service implementation
- [Profile.cs](../../src/IIIF.Manifest.Serializer.Net/Properties/Profile.cs) - Profile constants for Auth patterns
- [BaseItem.cs](../../src/IIIF.Manifest.Serializer.Net/Shared/BaseItem/BaseItem.cs) - Base class for service objects
