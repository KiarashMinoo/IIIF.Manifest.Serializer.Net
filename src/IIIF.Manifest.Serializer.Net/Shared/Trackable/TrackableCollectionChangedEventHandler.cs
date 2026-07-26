namespace IIIF.Manifests.Serializer.Shared.Trackable;

public delegate void TrackableCollectionChangedEventHandler
    (object sender, TrackableCollectionChangedEventArgs e);

public delegate void TrackableCollectionChangedEventHandler<in TTrackableCollection, T>
    (TTrackableCollection sender, TrackableCollectionChangedEventArgs<T> e)
    where TTrackableCollection : TrackableCollection<T>;