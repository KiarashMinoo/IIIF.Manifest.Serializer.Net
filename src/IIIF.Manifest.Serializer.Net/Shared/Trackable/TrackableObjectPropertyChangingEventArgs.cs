using System.ComponentModel;

namespace IIIF.Manifests.Serializer.Shared.Trackable;

public class TrackableObjectPropertyChangingEventArgs(
    string propertyName,
    bool isList = false
) : PropertyChangingEventArgs(propertyName)
{
    public TrackableObjectPropertyChangingEventArgs(
        string propertyName,
        CollectionChangedType collectionChangedType
    ) : this(propertyName, true)
    {
        CollectionChangedType = collectionChangedType;
    }

    public bool IsList { get; } = isList;
    public CollectionChangedType? CollectionChangedType { get; }
}