using IIIF.Manifests.Serializer.Shared.Trackable.Collections;

namespace IIIF.Manifests.Serializer.Shared.Trackable.Notifications;

public abstract class TrackableCollectionChangeEventArgs(
    object? item,
    CollectionChangeType changeType,
    int index
) : EventArgs
{
    public object? Item { get; } = item;
    public CollectionChangeType ChangeType { get; } = changeType;
    public int Index { get; } = index;
}