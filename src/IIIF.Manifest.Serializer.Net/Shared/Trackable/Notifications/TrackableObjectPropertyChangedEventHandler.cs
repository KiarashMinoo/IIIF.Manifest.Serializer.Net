using IIIF.Manifests.Serializer.Shared.Trackable.Objects;

namespace IIIF.Manifests.Serializer.Shared.Trackable.Notifications;

public delegate void TrackableObjectPropertyChangedEventHandler<in TTrackableObject>
    (TTrackableObject sender, TrackableObjectPropertyChangedEventArgs e) where TTrackableObject : TrackableObject<TTrackableObject>;