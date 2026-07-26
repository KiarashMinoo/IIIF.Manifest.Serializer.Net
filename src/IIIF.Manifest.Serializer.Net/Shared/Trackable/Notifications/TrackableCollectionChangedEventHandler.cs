using IIIF.Manifests.Serializer.Shared.Trackable.Collections;

namespace IIIF.Manifests.Serializer.Shared.Trackable.Notifications;

public delegate void TrackableCollectionChangedEventHandler
    (object sender, TrackableCollectionChangedEventArgs e);

public delegate void TrackableCollectionChangedEventHandler<in TTrackableCollection, T>
    (TTrackableCollection sender, TrackableCollectionChangedEventArgs<T> e)
    where TTrackableCollection : TrackableCollection<T>;