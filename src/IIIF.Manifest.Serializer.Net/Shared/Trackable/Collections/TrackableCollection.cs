using System.Collections;
using System.ComponentModel;
using IIIF.Manifests.Serializer.Shared.Trackable.Core;
using IIIF.Manifests.Serializer.Shared.Trackable.Notifications;

namespace IIIF.Manifests.Serializer.Shared.Trackable.Collections;

public interface ITrackableCollection
{
    event TrackableCollectionChangingEventHandler? CollectionChanging;
    event TrackableCollectionChangedEventHandler? CollectionChanged;

    void Add(object item);
}

public partial class TrackableCollection<T> :
    Core.TrackableObject,
    ICollection<T>,
    IReadOnlyCollection<T>,
    ITrackableCollection
{
    private readonly List<ElementDescriptor<T>> _items = [];
    private readonly ChangeNotificationSubscription _itemSubscription;
    private int _activeCount;

    public TrackableCollection()
    {
        _itemSubscription = new ChangeNotificationSubscription(
            OnNestedCollectionChanging, OnNestedCollectionChanged, OnItemPropertyChanging, OnItemPropertyChanged);
    }

    public TrackableCollection(IEnumerable<T> items) : this()
    {
        if (items is null) throw new ArgumentNullException(nameof(items));

        foreach (var item in items)
        {
            SubscribeItem(item);
            _items.Add(new ElementDescriptor<T>(item));
            _activeCount++;
        }
    }

    public event TrackableCollectionChangingEventHandler? CollectionChanging;
    public event TrackableCollectionChangedEventHandler? CollectionChanged;

    public int Count => _activeCount;
    public bool IsReadOnly => false;

    protected virtual void OnCollectionChanging(T item, CollectionChangeType changeType, int index)
    {
        var args = new TrackableCollectionChangingEventArgs<T>(item, changeType, index);
        CollectionChanging?.Invoke(this, args);
    }

    protected virtual void OnCollectionChanged(T item, CollectionChangeType changeType, int index)
    {
        var args = new TrackableCollectionChangedEventArgs<T>(item, changeType, index);
        CollectionChanged?.Invoke(this, args);
    }

    protected virtual void OnItemPropertyChanging(object sender, PropertyChangingEventArgs e)
    {
        var item = (T)sender;
        var index = FindItemIndex(item);
        OnCollectionChanging(item, CollectionChangeType.Changed, index);
    }

    protected virtual void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        var item = (T)sender;
        var index = FindItemIndex(item);
        OnCollectionChanged(item, CollectionChangeType.Changed, index);
    }

    private void OnNestedCollectionChanging(object sender, TrackableCollectionChangingEventArgs e)
    {
        var item = (T)sender;
        OnCollectionChanging(item, e.ChangeType, FindItemIndex(item));
    }

    private void OnNestedCollectionChanged(object sender, TrackableCollectionChangedEventArgs e)
    {
        var item = (T)sender;
        OnCollectionChanged(item, e.ChangeType, FindItemIndex(item));
    }

    private void SubscribeItem(T item)
    {
        _itemSubscription.Attach(item);
    }

    private void UnsubscribeItem(T item)
    {
        _itemSubscription.Detach(item);
    }

    private int FindItemIndex(T item)
    {
        var visibleIndex = 0;
        foreach (var descriptor in _items)
        {
            if (descriptor.ModificationType == ModificationType.Removed) continue;
            if (EqualityComparer<T>.Default.Equals(descriptor.Value, item)) return visibleIndex;
            visibleIndex++;
        }

        return -1;
    }

    private int FindDescriptorIndex(T item)
    {
        return _items.FindIndex(x =>
            x.ModificationType != ModificationType.Removed &&
            EqualityComparer<T>.Default.Equals(x.Value, item));
    }

    private bool TryFindForRemoval(T item, out int descriptorIndex, out int visibleIndex)
    {
        var scannedVisibleIndex = 0;
        for (var i = 0; i < _items.Count; i++)
        {
            if (_items[i].ModificationType == ModificationType.Removed) continue;

            if (EqualityComparer<T>.Default.Equals(_items[i].Value, item))
            {
                descriptorIndex = i;
                visibleIndex = scannedVisibleIndex;
                return true;
            }

            scannedVisibleIndex++;
        }

        descriptorIndex = -1;
        visibleIndex = -1;
        return false;
    }

    private int AddCore(T item)
    {
        if (IsReadOnly) throw new InvalidOperationException("Cannot add items to read-only collection");

        var index = Count;
        OnCollectionChanging(item, CollectionChangeType.Added, index);
        SubscribeItem(item);

        var descriptor = new ElementDescriptor<T>(item);
        descriptor.SetModificationType(ModificationType.Added);
        _items.Add(descriptor);
        _activeCount++;

        OnCollectionChanged(item, CollectionChangeType.Added, index);
        return index;
    }

    public int Add(T item)
    {
        return AddCore(item);
    }

    void ITrackableCollection.Add(object item)
    {
        if (item is not T typedItem) throw new ArgumentException("Cannot add item to trackable collection", nameof(item));
        AddCore(typedItem);
    }

    void ICollection<T>.Add(T item)
    {
        AddCore(item);
    }

    public bool Remove(T item)
    {
        if (IsReadOnly) return false;

        if (!TryFindForRemoval(item, out var descriptorIndex, out var visibleIndex)) return false;

        var descriptor = _items[descriptorIndex];
        OnCollectionChanging(descriptor.Value, CollectionChangeType.Removed, visibleIndex);
        UnsubscribeItem(descriptor.Value);

        if (descriptor.ModificationType == ModificationType.Added)
            _items.RemoveAt(descriptorIndex);
        else
            descriptor.SetModificationType(ModificationType.Removed);

        _activeCount--;
        OnCollectionChanged(descriptor.Value, CollectionChangeType.Removed, visibleIndex);
        return true;
    }

    public void Clear()
    {
        foreach (var item in _items.Where(x => x.ModificationType != ModificationType.Removed).Select(x => x.Value).ToList()) Remove(item);
    }

    public bool Contains(T item)
    {
        return FindDescriptorIndex(item) >= 0;
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        _items.Where(x => x.ModificationType != ModificationType.Removed).Select(x => x.Value).ToArray().CopyTo(array, arrayIndex);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _items
            .Where(x => x.ModificationType != ModificationType.Removed)
            .Select(x => x.Value)
            .GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

}