# Trackable Collections

## Overview

`IIIF.Manifests.Serializer.Shared.Trackable.Collections` provides mutable collections whose item
descriptors preserve baseline, addition, removal, and nested-child changes.

| File | Public type(s) | Purpose |
| --- | --- | --- |
| `TrackableCollection.cs` | `ITrackableCollection`, `TrackableCollection<T>` | `IList<T>` implementation with changing/changed events |
| `TrackableCollection.ChangeTracking.cs` | `TrackableCollection<T>` (partial) | Recursive descriptor-based change enumeration and acceptance |
| `CollectionChangeType.cs` | `CollectionChangeType` | `None`, `Added`, `Changed`, and `Removed` event states |

Constructor items form the accepted baseline. New items are `Added`; removing a newly added item
cancels that pending change; removing an accepted item records `Removed` until changes are
accepted. Enumeration and `Count` expose only current items.

`Count` is an incrementally maintained field (`_activeCount`), not a per-call `Where(...).Count()`
scan, so building a large collection through repeated `Add` calls (the documented `AddItem` fluent
pattern) stays linear rather than quadratic. `Remove` resolves both the raw list index and the
visible index in a single pass (`TryFindForRemoval`) instead of scanning twice. Each collection
instance owns one `Core.ChangeNotificationSubscription`, constructed once and reused for every
item's `SubscribeItem`/`UnsubscribeItem` call, rather than re-checking the
`ITrackableCollection`/`INotifyPropertyChanging`/`INotifyPropertyChanged` interfaces inline per item.

## Diagrams

```mermaid
classDiagram
    Core.TrackableObject <|-- TrackableCollection~T~
    IList~T~ <|.. TrackableCollection~T~
    ITrackableCollection <|.. TrackableCollection~T~
    TrackableCollection~T~ --> CollectionChangeType
```

## See also

- [Trackable overview](../README.md)
- [Notification event arguments](../Notifications/README.md)
- [Change-tracking semantics](../../../CHANGE_TRACKING.md)
