# Shared Trackable Infrastructure

`Shared/Trackable` is the storage, notification, and change-tracking foundation used by the IIIF model graph. Model properties are backed by `ElementDescriptor` instances rather than individual fields, while collection items are backed by `ElementDescriptor<T>` instances.

## Source layout

```text
Shared/Trackable/
├── Core/
│   ├── ElementDescriptor.cs
│   ├── ModificationType.cs
│   ├── TrackableObject.cs
│   └── TrackableObject.ChangeTracking.cs
├── Objects/
│   ├── TrackableObject.cs
│   ├── TrackableObject.Getters.cs
│   ├── TrackableObject.Setters.cs
│   └── TrackableObject.AdditionalProperties.cs
├── Collections/
│   ├── CollectionChangeType.cs
│   ├── TrackableCollection.cs
│   └── TrackableCollection.ChangeTracking.cs
├── Notifications/
│   ├── TrackableCollection*EventArgs.cs
│   ├── TrackableCollection*EventHandler.cs
│   ├── TrackableObjectProperty*EventArgs.cs
│   └── TrackableObjectProperty*EventHandler.cs
└── AdditionalProperties/
    └── IAdditionalPropertiesSupport.cs
```

The folders and namespaces mirror the same concern boundaries: `Core`, `Objects`, `Collections`, `Notifications`, and `AdditionalProperties`. This keeps generic object behavior separate from the reusable non-generic tracking base while making dependencies explicit.
## Core types

### `TrackableObject`

The non-generic base owns:

- the `ElementDescriptors` property store;
- Newtonsoft.Json `Serialize`, `Parse<T>`, and `TryParse<T>` helpers;
- the recursive `IIiifChangeTrackable` implementation;
- cycle detection, path composition, descendant traversal, and acceptance.

### `TrackableObject<TTrackableObject>`

The CRTP-derived type adds the strongly typed fluent-property engine and notifications:

- `GetElementValue<TValue>` and `SetElementValue<TValue>`;
- `INotifyPropertyChanging` and `INotifyPropertyChanged`;
- strongly typed TrackableObject events;
- collection and child notification attachment;
- additional-property integration.

Shared state belongs to the non-generic base so every closed generic object and `TrackableCollection<T>` use the same tracking machinery.

### `ElementDescriptor<TValue>`

Each descriptor stores:

- `OriginalValue` — the accepted baseline;
- `ModifiedValue` — the replacement value when one was supplied;
- `Value` — the current effective value;
- `ModificationType` — `Unchanged`, `Added`, `Changed`, `Removed`, or `Unknown`;
- `IsAdditional` — whether the descriptor represents extension data.

An explicit modified-value flag is used internally so value types such as `0`, coordinates, and enum defaults are never confused with “no modified value.”

### `TrackableCollection<T>`

`TrackableCollection<T>` derives from `TrackableObject` and stores each item in an `ElementDescriptor<T>`.

- constructor items form an unchanged baseline;
- new items are marked `Added`;
- removing an added item cancels it immediately;
- accepted items are marked `Removed` until `AcceptChanges` or `ClearChanges`;
- enumeration, `Count`, `Contains`, and `CopyTo` expose only current items;
- removed item subscriptions are detached immediately;
- collection changes and nested item changes bubble through notification events.

The item descriptors are the structural source of truth. No parallel collection-baseline dictionary is maintained.

## Change and notification naming

`CollectionChangeType` is the canonical collection event state:

- `None`
- `Added`
- `Changed`
- `Removed`

Collection event arguments expose `ChangeType`. TrackableObject property event arguments expose `IsCollection` and nullable `ChangeType`.

`CollectionChangeType`, `ChangeType`, and `IsCollection` replace the ambiguous older names `CollectionChangedType`, `CollectionChangedType`, and `IsList`.

## Property update flow

A fluent setter follows this sequence:

1. Resolve the member name.
2. Read the current descriptor value.
3. Produce the requested value through the setter factory.
4. Raise the changing notification.
5. Normalize a non-trackable enumerable into `TrackableCollection<T>`.
6. Attach child and collection handlers.
7. Update the descriptor state.
8. Raise the changed notification.

Existing `TrackableCollection<T>` values are not rewrapped. In-place collection operations therefore preserve their item descriptors and change states.

## Additional properties

Unknown JSON properties are stored in the same `ElementDescriptors` dictionary with `IsAdditional = true`. The `AdditionalProperties` partial provides the public support interface, while the `[JsonExtensionData]` adapter in `TrackableObject<T>` bridges these descriptors to Newtonsoft.Json.

## Thread safety

Trackable objects and collections are mutable and are not thread-safe. Descriptor dictionaries/lists and synchronous event handlers must not be mutated concurrently without external synchronization.

## Related documentation

- [Change tracking](../../CHANGE_TRACKING.md)
- [Core tracking types](Core/README.md)
- [Generic trackable objects](Objects/README.md)
- [Trackable collections](Collections/README.md)
- [Notification events](Notifications/README.md)
- [Additional-property contract](AdditionalProperties/README.md)
- [Helpers](../../Helpers/README.md)
- [SDK versioning guide](../../SDK_VERSIONING_GUIDE.md)
