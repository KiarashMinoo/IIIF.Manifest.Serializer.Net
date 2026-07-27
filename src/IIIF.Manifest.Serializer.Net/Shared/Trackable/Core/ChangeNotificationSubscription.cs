using System.ComponentModel;
using IIIF.Manifests.Serializer.Shared.Trackable.Collections;
using IIIF.Manifests.Serializer.Shared.Trackable.Notifications;

namespace IIIF.Manifests.Serializer.Shared.Trackable.Core;

/// <summary>
///     Attaches/detaches the three change-notification interfaces a nested trackable value or
///     collection item may implement (<see cref="ITrackableCollection" />,
///     <see cref="INotifyPropertyChanging" />, <see cref="INotifyPropertyChanged" />), forwarding to a
///     fixed set of callbacks. Shared by <c>TrackableObject&lt;TTrackableObject&gt;</c> (one instance
///     per property, cached so attach/detach always use the same delegate identity) and
///     <c>TrackableCollection&lt;T&gt;</c> (one instance per collection, reused for every item) so the
///     interface-checking mechanics live in exactly one place.
/// </summary>
internal sealed class ChangeNotificationSubscription(
    TrackableCollectionChangingEventHandler onCollectionChanging,
    TrackableCollectionChangedEventHandler onCollectionChanged,
    PropertyChangingEventHandler onItemPropertyChanging,
    PropertyChangedEventHandler onItemPropertyChanged)
{
    public void Attach(object? item)
    {
        if (item is ITrackableCollection trackableCollection)
        {
            trackableCollection.CollectionChanging += onCollectionChanging;
            trackableCollection.CollectionChanged += onCollectionChanged;
        }

        if (item is INotifyPropertyChanging notifyPropertyChanging)
            notifyPropertyChanging.PropertyChanging += onItemPropertyChanging;

        if (item is INotifyPropertyChanged notifyPropertyChanged)
            notifyPropertyChanged.PropertyChanged += onItemPropertyChanged;
    }

    public void Detach(object? item)
    {
        if (item is ITrackableCollection trackableCollection)
        {
            trackableCollection.CollectionChanging -= onCollectionChanging;
            trackableCollection.CollectionChanged -= onCollectionChanged;
        }

        if (item is INotifyPropertyChanging notifyPropertyChanging)
            notifyPropertyChanging.PropertyChanging -= onItemPropertyChanging;

        if (item is INotifyPropertyChanged notifyPropertyChanged)
            notifyPropertyChanged.PropertyChanged -= onItemPropertyChanged;
    }
}
