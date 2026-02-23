using System.ComponentModel;

namespace IIIF.Manifests.Serializer.Shared.Trackable;

public class TrackableObjectPropertyChangingEventArgs(
    string propertyName,
    bool isList = false
) : PropertyChangingEventArgs(propertyName)
{
    public bool IsList { get; } = isList;
}