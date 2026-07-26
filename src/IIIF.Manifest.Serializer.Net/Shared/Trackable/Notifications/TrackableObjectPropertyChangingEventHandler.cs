using IIIF.Manifests.Serializer.Shared.Trackable.Objects;

namespace IIIF.Manifests.Serializer.Shared.Trackable.Notifications;

public delegate void TrackableObjectPropertyChangingEventHandler<in TTrackableObject>
    (TTrackableObject sender, TrackableObjectPropertyChangingEventArgs args) where TTrackableObject : TrackableObject<TTrackableObject>;