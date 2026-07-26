namespace IIIF.Manifests.Serializer.Shared.Trackable;

public class TrackableCollectionChangedEventArgs(
    object? item,
    CollectionChangedType collectionChangedType,
    int index
) : EventArgs
{
    public object? Item { get; } = item;
    public CollectionChangedType CollectionChangedType { get; } = collectionChangedType;
    public int Index { get; } = index;
}

public class TrackableCollectionChangedEventArgs<T>(
    T item,
    CollectionChangedType collectionChangedType,
    int index
) : TrackableCollectionChangedEventArgs(
    item,
    collectionChangedType,
    index
)
{
    public new T Item { get; } = item;
}