using IIIF.Manifests.Serializer.Shared.Trackable.Collections;

namespace IIIF.Manifests.Serializer.Shared.Trackable.Notifications;

public class TrackableCollectionChangingEventArgs(
    object? item,
    CollectionChangeType changeType,
    int index
) : TrackableCollectionChangeEventArgs(item, changeType, index);

public sealed class TrackableCollectionChangingEventArgs<T>(
    T item,
    CollectionChangeType changeType,
    int index
) : TrackableCollectionChangingEventArgs(item, changeType, index)
{
    public new T Item { get; } = item;
}