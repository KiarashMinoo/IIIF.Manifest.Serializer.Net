using IIIF.Manifests.Serializer.Shared.Trackable.Collections;

namespace IIIF.Manifests.Serializer.Shared.Trackable.Notifications;

public class TrackableCollectionChangedEventArgs(
    object? item,
    CollectionChangeType changeType,
    int index
) : TrackableCollectionChangeEventArgs(item, changeType, index);

public sealed class TrackableCollectionChangedEventArgs<T>(
    T item,
    CollectionChangeType changeType,
    int index
) : TrackableCollectionChangedEventArgs(item, changeType, index)
{
    public new T Item { get; } = item;
}