# Trackable Objects

## Overview

`IIIF.Manifests.Serializer.Shared.Trackable.Objects` contains the generic
`TrackableObject<TTrackableObject>` partial class used by SDK model types. It layers strongly typed
fluent getters/setters, property notifications, child/collection subscription, and additional JSON
property support over the non-generic core base.

| File | Responsibility |
| --- | --- |
| `TrackableObject.cs` | Events, notification interfaces, descriptor exposure, and the `SetElementValue`/`GetElementValue` engine. Caches a `Core.ChangeNotificationSubscription` per property name (`_changeHandlerSubscriptions`) so attach/detach always use the same delegate identity across separate `SetElementValue` calls - a fresh closure per call would never un-subscribe a handler attached by an earlier call. The reflection used to wrap a raw enumerable into `TrackableCollection<>` is memoized per source `Type` (`TrackableCollectionTypeCache`). The current-value cast for factory-based setters falls back to the same JToken-conversion recovery `GetElementValue` uses, instead of silently discarding an unread additional property's value |
| `TrackableObject.Getters.cs` | Typed descriptor reads |
| `TrackableObject.Setters.cs` | Typed fluent writes and collection normalization |
| `TrackableObject.AdditionalProperties.cs` | `IAdditionalPropertiesSupport<T>` implementation and Newtonsoft.Json extension-data bridge |

## Diagrams

```mermaid
classDiagram
    Core.TrackableObject <|-- TrackableObject~T~
    INotifyPropertyChanging <|.. TrackableObject~T~
    INotifyPropertyChanged <|.. TrackableObject~T~
    IAdditionalPropertiesSupport~T~ <|.. TrackableObject~T~
```

Instances are mutable and are not thread-safe.

## See also

- [Trackable overview](../README.md)
- [Core tracking types](../Core/README.md)
- [Notifications](../Notifications/README.md)
