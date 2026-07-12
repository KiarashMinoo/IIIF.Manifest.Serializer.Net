# Service

## Contents
- [Overview](#overview)
- [Files](#files)
- [Types & Members](#types--members)
  - [IBaseService](#ibaseservice)
- [Diagrams](#diagrams)
- [Package Dependencies](#package-dependencies)
- [See Also](#see-also)

## Overview

`IIIF.Manifests.Serializer.Shared.Service` is a single-file, single-type folder holding `IBaseService` — the minimal marker contract that every IIIF service-descriptor type in the SDK implements. It exists purely to break a circular-namespace/attribute problem: the polymorphic `[JsonConverter(typeof(ServiceJsonConverter))]` attribute needs an interface to attach to, and that interface needs to live somewhere both the shared serialization plumbing and the concrete service types (`Service`, `AuthService1`, the Auth2 family, `SearchService`, `AutoCompleteService`, `DiscoveryService`, `ContentStateService` — all under `Properties/Services/`, out of this folder's scope) can reference without a dependency cycle. The concrete service catalog itself lives entirely in `Properties/Services/`; this folder only defines the shape they all share.

## Files

| File | Primary type(s) | LOC (approx) | Responsibility |
| --- | --- | --- | --- |
| `IBaseService.cs` | `IBaseService` | 11 | Marker interface implemented by every concrete IIIF service-descriptor type; carries the `ServiceJsonConverter` attribute so any `IBaseService`-typed property is polymorphically serialized. |

[↑ Back to top](#contents)

## Types & Members

| Type | Kind | Summary | Inherits/Implements | Key Members |
| --- | --- | --- | --- | --- |
| `IBaseService` | interface | Base contract for all IIIF service descriptors (Image API service, Auth1/Auth2 services, Search/AutoComplete/Discovery/ContentState services). | *(none)* | `ProfileJName` (const), `Profile` |

### IBaseService

- **Kind / Namespace**: public interface, `IIIF.Manifests.Serializer.Shared.Service`.
- **Inherits/Implements**: none.
- **Attributes**: `[JsonConverter(typeof(ServiceJsonConverter))]` — declared on `Shared.ServiceJsonConverter` (in `Shared/ServiceJsonConverter.cs`, a sibling file at the `Shared` root, out of this folder's scope but the direct collaborator of this interface). Any property typed `IBaseService` (or a collection thereof) is read/written through that converter, which inspects the JSON to determine which concrete service type (`Service`, `AuthService1`, `AuthAccessService2`, `AuthProbeService2`, `AuthLogoutService2`, `AuthAccessTokenService2`, `SearchService`, `AutoCompleteService`, `DiscoveryService`, `ContentStateService` — all in `Properties/Services/`) to materialize.
- **Constants**: `ProfileJName = "profile"` — the JSON property name for `Profile`, exposed so implementers and the converter share one source of truth rather than duplicating the literal string.
- **Key properties**:
  - `Profile : string` — get-only; every service descriptor carries a `profile` URI/token identifying which service specification it conforms to (e.g. an Image API compliance level, or an Auth service's auth pattern). This is the field the polymorphic converter typically keys off to decide the concrete implementing type.
- **Thread-safety/immutability**: the interface itself carries no state; immutability is a property of each implementer (all of which are `TrackableObject`-derived elsewhere in the SDK).
- **Usage Recipe**:
```csharp
using IIIF.Manifests.Serializer.Shared.Service;
using IIIF.Manifests.Serializer.Properties.Services; // Service, AuthService1, etc. (out of scope here)

// Anywhere a resource exposes a Service collection, elements are held as IBaseService
// and resolved to their concrete type on read via ServiceJsonConverter.
IEnumerable<IBaseService> services = GetServicesFromSomeResource();
foreach (var service in services)
{
    Console.WriteLine(service.Profile);
}
```

[↑ Back to top](#contents)

## Diagrams

*Not applicable — single self-contained type.* `IBaseService` has no in-folder collaborators; its only relationships (to `ServiceJsonConverter` and the concrete `Properties/Services/*` implementers) cross folder boundaries and belong to those folders' own documentation.

[↑ Back to top](#contents)

## Package Dependencies

| Package | Version | Description | Links |
| --- | --- | --- | --- |
| Newtonsoft.Json | 13.0.4 | JSON.NET — this SDK's serialization engine (custom JsonConverters, attribute-driven read/write) | [NuGet](https://www.nuget.org/packages/Newtonsoft.Json/13.0.4) |

[↑ Back to top](#contents)

## See Also

- [`../README.md`](../README.md) — Shared root
- [`../../README.md`](../../README.md) — top-level SDK docs
- [`../../SDK_VERSIONING_GUIDE.md`](../../SDK_VERSIONING_GUIDE.md) — SDK versioning guide

[↑ Back to top](#contents)
