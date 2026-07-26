namespace IIIF.Manifests.Serializer.Shared.Trackable;

public class TrackableCollectionChangingEventArgs(
    object? item,
    CollectionChangedType collectionChangedType,
    int index
) : EventArgs
{
    public object? Item { get; } = item;
    public CollectionChangedType CollectionChangedType { get; } = collectionChangedType;
    public int Index { get; } = index;
}

public class TrackableCollectionChangingEventArgs<T>(
    T item,
    CollectionChangedType collectionChangedType,
    int index
) : TrackableCollectionChangingEventArgs(
    item,
    collectionChangedType,
    index
)
{
    public new T Item { get; } = item;
}