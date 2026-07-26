namespace IIIF.Manifests.Serializer.Shared.Trackable;

public delegate void TrackableObjectPropertyChangingEventHandler<in TTrackableObject>
    (TTrackableObject sender, TrackableObjectPropertyChangingEventArgs args) where TTrackableObject : TrackableObject<TTrackableObject>;