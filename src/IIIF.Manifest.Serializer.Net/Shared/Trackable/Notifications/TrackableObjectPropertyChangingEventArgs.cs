using System.ComponentModel;
using IIIF.Manifests.Serializer.Shared.Trackable.Collections;

namespace IIIF.Manifests.Serializer.Shared.Trackable.Notifications;

public sealed class TrackableObjectPropertyChangingEventArgs(
    string propertyName,
    bool isCollection = false
) : PropertyChangingEventArgs(propertyName)
{
    public TrackableObjectPropertyChangingEventArgs(string propertyName, CollectionChangeType changeType)
        : this(propertyName, true)
    {
        ChangeType = changeType;
    }

    public bool IsCollection { get; } = isCollection;
    public CollectionChangeType? ChangeType { get; }
}