using System.Collections;
using System.ComponentModel;
using System.Linq;

namespace IIIF.Manifests.Serializer.Shared.Trackable;

public interface ITrackableCollection
{
    event TrackableCollectionChangingEventHandler? CollectionChanging;
    event TrackableCollectionChangedEventHandler? CollectionChanged;

    void Add(object item);
}

public class TrackableCollection<T>() :
    ICollection<T>,
    IReadOnlyCollection<T>,
    ITrackableCollection
{
    private readonly bool _initialized = true;
    private readonly IList<T> _items = [];

    public event TrackableCollectionChangingEventHandler? CollectionChanging;
    public event TrackableCollectionChangedEventHandler? CollectionChanged;

    public int Count => _items.Count;
    public bool IsReadOnly => _items.IsReadOnly;


    public TrackableCollection(IEnumerable<T> items) : this()
    {
        foreach (var item in items)
        {
            _initialized = false;
            Add(item);
            _initialized = true;
        }
    }

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
        var trackableObject = (T)sender;
        var index = _items.IndexOf(trackableObject);
        OnCollectionChanging(trackableObject, CollectionChangedType.Modify, index);
    }

    protected virtual void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        var trackableObject = (T)sender;
        var index = _items.IndexOf(trackableObject);
        OnCollectionChanged(trackableObject, CollectionChangedType.Modify, index);
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

    private int AddCore(T trackableObject)
    {
        if (IsReadOnly)
        {
            throw new InvalidOperationException("Cannot add items to read-only collection");
        }

        if (_initialized)
            OnCollectionChanging(trackableObject, CollectionChangedType.Add, _items.Count);

        SubscribeItem(trackableObject);

        _items.Add(trackableObject);
        var index = _items.Count - 1;

        if (_initialized)
            OnCollectionChanged(trackableObject, CollectionChangedType.Add, index);

        return index;
    }

    public int Add(T trackableObject)
    {
        return AddCore(trackableObject);
    }

    void ITrackableCollection.Add(object item)
    {
        if (item is not T trackableObject)
            throw new ArgumentException($"Cannot add items to trackable collection");

        AddCore(trackableObject);
    }

    void ICollection<T>.Add(T trackableObject)
    {
        AddCore(trackableObject);
    }

    public bool Remove(T item)
    {
        if (IsReadOnly)
        {
            return false;
        }

        var index = _items.IndexOf(item);
        if (index < 0)
        {
            return false;
        }

        var trackableObject = _items[index];

        OnCollectionChanging(trackableObject, CollectionChangedType.Remove, index);

        UnsubscribeItem(trackableObject);

        _items.RemoveAt(index);

        OnCollectionChanged(trackableObject, CollectionChangedType.Remove, index);

        return true;
    }

    public void Clear()
    {
        foreach (var trackableObject in _items.ToList())
        {
            Remove(trackableObject);
        }
    }

    public bool Contains(T item)
    {
        return _items.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        _items.CopyTo(array, arrayIndex);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}