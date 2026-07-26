using System.ComponentModel;
using IIIF.Manifests.Serializer.Shared.Trackable.Collections;

namespace IIIF.Manifests.Serializer.Shared.Trackable.Notifications;

public sealed class TrackableObjectPropertyChangedEventArgs(
    string propertyName,
    bool isCollection = false
) : PropertyChangedEventArgs(propertyName)
{
    public TrackableObjectPropertyChangedEventArgs(string propertyName, CollectionChangeType changeType)
        : this(propertyName, true)
    {
        ChangeType = changeType;
    }

    public bool IsCollection { get; } = isCollection;
    public CollectionChangeType? ChangeType { get; }
}