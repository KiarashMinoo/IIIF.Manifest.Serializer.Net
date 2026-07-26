using System.Collections;
using System.ComponentModel;

namespace IIIF.Manifests.Serializer.Shared.Trackable;

public interface ITrackableCollection
{
    event TrackableCollectionChangingEventHandler? CollectionChanging;
    event TrackableCollectionChangedEventHandler? CollectionChanged;

    void Add(object item);
}

public partial class TrackableCollection<T> :
    TrackableObject,
    ICollection<T>,
    IReadOnlyCollection<T>,
    ITrackableCollection
{
    private readonly List<ElementDescriptor<T>> _items = [];

    public TrackableCollection()
    {
    }

    public TrackableCollection(IEnumerable<T> items)
    {
        if (items is null) throw new ArgumentNullException(nameof(items));

        foreach (var item in items)
        {
            SubscribeItem(item);
            _items.Add(new ElementDescriptor<T>(item));
        }
    }

    public event TrackableCollectionChangingEventHandler? CollectionChanging;
    public event TrackableCollectionChangedEventHandler? CollectionChanged;

    public int Count => _items.Count(x => x.ModificationType != ModificationType.Removed);
    public bool IsReadOnly => false;

    protected virtual void OnCollectionChanging(T item, CollectionChangedType collectionChangedType, int index)
    {
        var args = new TrackableCollectionChangingEventArgs<T>(item, collectionChangedType, index);
        CollectionChanging?.Invoke(this, args);
    }

    protected virtual void OnCollectionChanged(T item, CollectionChangedType collectionChangedType, int index)
    {
        var args = new TrackableCollectionChangedEventArgs<T>(item, collectionChangedType, index);
        CollectionChanged?.Invoke(this, args);
    }

    protected virtual void OnItemPropertyChanging(object sender, PropertyChangingEventArgs e)
    {
        var item = (T)sender;
        var index = FindItemIndex(item);
        OnCollectionChanging(item, CollectionChangedType.Modify, index);
    }

    protected virtual void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        var item = (T)sender;
        var index = FindItemIndex(item);
        OnCollectionChanged(item, CollectionChangedType.Modify, index);
    }

    private void OnPrivateCollectionChanging(object sender, TrackableCollectionChangingEventArgs e)
    {
        OnItemPropertyChanging(sender, new PropertyChangingEventArgs("N/A"));
    }

    private void OnPrivateCollectionChanged(object sender, TrackableCollectionChangedEventArgs e)
    {
        OnItemPropertyChanged(sender, new PropertyChangedEventArgs("N/A"));
    }

    private void SubscribeItem(T item)
    {
        if (item is ITrackableCollection trackableCollection)
        {
            trackableCollection.CollectionChanging += OnPrivateCollectionChanging;
            trackableCollection.CollectionChanged += OnPrivateCollectionChanged;
        }

        if (item is INotifyPropertyChanging notifyPropertyChanging)
            notifyPropertyChanging.PropertyChanging += OnItemPropertyChanging;

        if (item is INotifyPropertyChanged notifyPropertyChanged)
            notifyPropertyChanged.PropertyChanged += OnItemPropertyChanged;
    }

    private void UnsubscribeItem(T item)
    {
        if (item is ITrackableCollection trackableCollection)
        {
            trackableCollection.CollectionChanging -= OnPrivateCollectionChanging;
            trackableCollection.CollectionChanged -= OnPrivateCollectionChanged;
        }

        if (item is INotifyPropertyChanging notifyPropertyChanging)
            notifyPropertyChanging.PropertyChanging -= OnItemPropertyChanging;

        if (item is INotifyPropertyChanged notifyPropertyChanged)
            notifyPropertyChanged.PropertyChanged -= OnItemPropertyChanged;
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

    private int AddCore(T item)
    {
        if (IsReadOnly) throw new InvalidOperationException("Cannot add items to read-only collection");

        var index = Count;
        OnCollectionChanging(item, CollectionChangedType.Add, index);
        SubscribeItem(item);

        var descriptor = new ElementDescriptor<T>(item);
        descriptor.SetModificationType(ModificationType.Added);
        _items.Add(descriptor);

        OnCollectionChanged(item, CollectionChangedType.Add, index);
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

        var descriptorIndex = FindDescriptorIndex(item);
        if (descriptorIndex < 0) return false;

        var descriptor = _items[descriptorIndex];
        var visibleIndex = FindItemIndex(item);
        OnCollectionChanging(descriptor.Value, CollectionChangedType.Remove, visibleIndex);
        UnsubscribeItem(descriptor.Value);

        if (descriptor.ModificationType == ModificationType.Added)
            _items.RemoveAt(descriptorIndex);
        else
            descriptor.SetModificationType(ModificationType.Removed);

        OnCollectionChanged(descriptor.Value, CollectionChangedType.Remove, visibleIndex);
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