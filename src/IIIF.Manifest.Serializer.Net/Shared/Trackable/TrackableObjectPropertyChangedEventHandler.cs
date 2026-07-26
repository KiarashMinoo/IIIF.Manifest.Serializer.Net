namespace IIIF.Manifests.Serializer.Shared.Trackable;

public delegate void TrackableObjectPropertyChangedEventHandler<in TTrackableObject>
    (TTrackableObject sender, TrackableObjectPropertyChangedEventArgs e) where TTrackableObject : TrackableObject<TTrackableObject>;