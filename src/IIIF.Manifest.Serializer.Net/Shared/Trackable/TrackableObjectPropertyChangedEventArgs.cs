using System.ComponentModel;

namespace IIIF.Manifests.Serializer.Shared.Trackable;

public class TrackableObjectPropertyChangedEventArgs : PropertyChangedEventArgs
{
    public TrackableObjectPropertyChangedEventArgs(string propertyName) : base(propertyName)
    {
    }

    public TrackableObjectPropertyChangedEventArgs(string propertyName, ListChangedType listChangedType) : this(propertyName)
    {
        IsList = true;
        ListChangedType = listChangedType;
    }

    public bool IsList { get; }

    public ListChangedType? ListChangedType { get; }
}