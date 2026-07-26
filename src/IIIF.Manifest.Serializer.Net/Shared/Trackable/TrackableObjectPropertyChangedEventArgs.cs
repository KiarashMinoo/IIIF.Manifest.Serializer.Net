using System.ComponentModel;

namespace IIIF.Manifests.Serializer.Shared.Trackable;

public class TrackableObjectPropertyChangedEventArgs(
    string propertyName,
    bool isList = false
) : PropertyChangedEventArgs(propertyName)
{
    public TrackableObjectPropertyChangedEventArgs(
        string propertyName,
        CollectionChangedType collectionChangedType
    ) : this(propertyName, true)
    {
        CollectionChangedType = collectionChangedType;
    }

    public bool IsList { get; } = isList;
    public CollectionChangedType? CollectionChangedType { get; } 
}