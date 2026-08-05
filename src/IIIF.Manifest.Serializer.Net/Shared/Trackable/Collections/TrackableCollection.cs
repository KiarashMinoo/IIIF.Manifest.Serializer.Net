using System.Collections;
using System.ComponentModel;
using IIIF.Manifests.Serializer.ChangeTracking;
using IIIF.Manifests.Serializer.Shared.Trackable.Core;
using IIIF.Manifests.Serializer.Shared.Trackable.Notifications;

namespace IIIF.Manifests.Serializer.Shared.Trackable.Collections;

public interface ITrackableCollection :
    ICollection,
    IList
{
    event TrackableCollectionChangingEventHandler? CollectionChanging;
    event TrackableCollectionChangedEventHandler? CollectionChanged;
}

public interface ITrackableCollection<T> :
    ITrackableCollection,
    ICollection<T>,
    IList<T>,
    IReadOnlyCollection<T>;

public partial class TrackableCollection :
    Core.TrackableObject,
    ITrackableCollection,
    ICollection,
    IList
{
    private readonly List<IElementDescriptor> _items = [];

    /// <summary>
    ///     Tombstones for pre-existing items removed this session, each paired with the index it held
    ///     in the last-accepted baseline - resolved once at removal time via <see cref="ResolveOriginalIndex" />
    ///     since the item itself is no longer in <see cref="_items" /> to read a position back from.
    /// </summary>
    private readonly List<(IElementDescriptor Descriptor, int OriginalIndex)> _removedItems = [];

    /// <summary>Mirrors <see cref="_removedItems" />'s original indices for O(1) membership checks in <see cref="ResolveOriginalIndex" />.</summary>
    private readonly HashSet<int> _removedOriginalIndices = [];

    private readonly ChangeNotificationSubscription _itemSubscription;
    private readonly Func<object?, IElementDescriptor> _descriptorFactory;
    private readonly IEqualityComparer _equalityComparer;
    private int _baselineCount;

    public event TrackableCollectionChangingEventHandler? CollectionChanging;
    public event TrackableCollectionChangedEventHandler? CollectionChanged;

    bool ICollection.IsSynchronized => false;
    object ICollection.SyncRoot => this;

    bool IList.IsFixedSize => false;

    object? IList.this[int index]
    {
        get => GetAtIndex(index).Value;
        set => throw new NotImplementedException();
    }

    public int Count => _items.Count;
    public bool IsReadOnly => false;

    internal TrackableCollection(Func<object?, IElementDescriptor> descriptorFactory, IEqualityComparer equalityComparer)
    {
        _descriptorFactory = descriptorFactory;
        _equalityComparer = equalityComparer;

        _itemSubscription = new ChangeNotificationSubscription(
            OnNestedCollectionChanging, OnNestedCollectionChanged, OnItemPropertyChanging, OnItemPropertyChanged);
    }

    internal TrackableCollection(Func<object?, IElementDescriptor> descriptorFactory, IEnumerable items, IEqualityComparer equalityComparer) : this(descriptorFactory, equalityComparer)
    {
        if (items is null) throw new ArgumentNullException(nameof(items));

        foreach (var item in items)
        {
            AddFast(item);
        }

        _baselineCount = _items.Count;
    }

    public TrackableCollection() : this(item => new ElementDescriptor(item), EqualityComparer<object?>.Default)
    {
    }


    protected virtual void OnCollectionChanging(object? item, CollectionChangeType changeType, int index)
    {
        var args = new TrackableCollectionChangingEventArgs(item, changeType, index);
        CollectionChanging?.Invoke(this, args);
    }

    protected virtual void OnCollectionChanged(object? item, CollectionChangeType changeType, int index)
    {
        var args = new TrackableCollectionChangedEventArgs(item, changeType, index);
        CollectionChanged?.Invoke(this, args);
    }

    private void OnNestedCollectionChanging(object? sender, TrackableCollectionChangingEventArgs e)
    {
        OnCollectionChanging(sender, e.ChangeType, FindIndex(sender));
    }

    private void OnNestedCollectionChanged(object? sender, TrackableCollectionChangedEventArgs e)
    {
        OnCollectionChanged(sender, e.ChangeType, FindIndex(sender));
    }

    protected virtual void OnItemPropertyChanging(object? sender, PropertyChangingEventArgs e)
    {
        OnCollectionChanging(sender, CollectionChangeType.Changed, FindIndex(sender));
    }

    protected virtual void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnCollectionChanged(sender, CollectionChangeType.Changed, FindIndex(sender));
    }

    protected internal void SubscribeItem(object? item)
    {
        _itemSubscription.Attach(item);
    }

    protected internal void UnsubscribeItem(object? item)
    {
        _itemSubscription.Detach(item);
    }

    protected internal virtual IElementDescriptor GetAtIndex(int index)
    {
        return _items[index];
    }

    protected internal virtual int FindIndex(object? item)
    {
        // Hand-written loop instead of List<T>.FindIndex(predicate) - this runs on every add/remove
        // and on every property change of every item already in the collection (see
        // OnItemPropertyChanging/Changed below), so avoiding a per-call closure allocation matters.
        for (var i = 0; i < _items.Count; i++)
            if (_equalityComparer.Equals(_items[i].Value, item))
                return i;

        return -1;
    }

    protected internal virtual IElementDescriptor AddFast(object? item)
    {
        SubscribeItem(item);
        var descriptor = _descriptorFactory(item);
        _items.Add(descriptor);
        return descriptor;
    }

    protected internal virtual int AddCore(object? item)
    {
        if (IsReadOnly) throw new InvalidOperationException("Cannot add items to read-only collection");

        var index = Count;
        OnCollectionChanging(item, CollectionChangeType.Added, index);

        AddFast(item).SetModificationType(ModificationType.Added);

        OnCollectionChanged(item, CollectionChangeType.Added, index);
        return index;
    }

    int IList.Add(object? item) => AddCore(item!);

    protected internal virtual bool ClearCore()
    {
        // Walk backwards by index rather than delegating to RemoveCore(item) per element - that
        // would re-run FindIndex's linear scan for every item, turning a bulk clear into O(n^2).
        for (var index = _items.Count - 1; index >= 0; index--)
            RemoveAtCore(index);

        return true;
    }

    void IList.Clear() => ClearCore();

    bool IList.Contains(object? item) => FindIndex(item) >= 0;

    int IList.IndexOf(object? item) => FindIndex(item);

    protected internal virtual void InsertCore(int index, object? item) => _items.Insert(index, _descriptorFactory(item));

    void IList.Insert(int index, object? item) => InsertCore(index, item);

    protected internal virtual bool RemoveCore(object? item)
    {
        if (IsReadOnly) return false;

        var index = FindIndex(item);
        return index >= 0 && RemoveAtCore(index);
    }

    protected internal virtual bool RemoveAtCore(int index)
    {
        var descriptor = GetAtIndex(index);
        OnCollectionChanging(descriptor.Value, CollectionChangeType.Removed, index);
        UnsubscribeItem(descriptor.Value);

        if (descriptor.ModificationType != ModificationType.Added)
        {
            var liveRank = 0;
            for (var i = 0; i < index; i++)
                if (_items[i].ModificationType != ModificationType.Added) liveRank++;

            descriptor.SetModificationType(ModificationType.Removed);
            var originalIndex = ResolveOriginalIndex(liveRank);
            _removedItems.Add((descriptor, originalIndex));
            _removedOriginalIndices.Add(originalIndex);
        }

        _items.RemoveAt(index);

        OnCollectionChanged(descriptor.Value, CollectionChangeType.Removed, index);
        return true;
    }

    /// <summary>
    ///     Recovers the baseline index a pre-existing item held before any of this session's removals.
    ///     A removed item's rank among the survivors still in <see cref="_items" /> (<paramref name="liveRank" />)
    ///     isn't its true original index once earlier removals have pulled other items out from before
    ///     it - so this walks the baseline's index space (<c>0.._baselineCount</c>), skips indices
    ///     already claimed by earlier tombstones, and returns the <paramref name="liveRank" />-th one
    ///     still unclaimed.
    /// </summary>
    private int ResolveOriginalIndex(int liveRank)
    {
        var seen = 0;
        for (var candidate = 0; candidate < _baselineCount; candidate++)
        {
            if (_removedOriginalIndices.Contains(candidate)) continue;
            if (seen == liveRank) return candidate;
            seen++;
        }

        throw new InvalidOperationException("Trackable collection is in an inconsistent state: could not resolve the original index of a removed item.");
    }

    void IList.Remove(object? item) => RemoveCore(item);

    void IList.RemoveAt(int index) => RemoveAtCore(index);

    protected internal virtual void CopyToCore(Array array, int arrayIndex)
    {
        for (var i = 0; i < _items.Count; i++)
            array.SetValue(_items[i].Value, arrayIndex + i);
    }

    void ICollection.CopyTo(Array array, int arrayIndex) => CopyToCore(array, arrayIndex);

    protected internal virtual IEnumerator<T> GetEnumerator<T>()
    {
        foreach (var descriptor in _items)
            if (descriptor.Value is T typedValue)
                yield return typedValue;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator<object>();
    }
}

public partial class TrackableCollection<T> :
    Core.TrackableObject,
    ITrackableCollection<T>,
    ICollection,
    ICollection<T>,
    IList,
    IList<T>,
    IReadOnlyCollection<T>
{
    private static readonly Func<object?, IElementDescriptor> DescriptorFactory = item => new ElementDescriptor<T>(TypedItem(item));

    private readonly TrackableCollection _trackableCollection;

    public event TrackableCollectionChangingEventHandler? CollectionChanging
    {
        add => _trackableCollection.CollectionChanging += value;
        remove => _trackableCollection.CollectionChanging -= value;
    }

    public event TrackableCollectionChangedEventHandler? CollectionChanged
    {
        add => _trackableCollection.CollectionChanged += value;
        remove => _trackableCollection.CollectionChanged -= value;
    }

    // Change-tracking state lives on the wrapped _trackableCollection, not on this wrapper (which
    // has no ElementDescriptors of its own) - delegate rather than let the TrackableObject base
    // walk an always-empty dictionary here.
    internal override bool HasChangesCore(HashSet<object> visited) => _trackableCollection.HasChangesCore(visited);

    internal override void GetChangesCore(List<IiifChangeEntry> entries, HashSet<object> visited, DateTimeOffset changedAtUtc)
        => _trackableCollection.GetChangesCore(entries, visited, changedAtUtc);

    internal override void ClearChangesCore(HashSet<object> visited) => _trackableCollection.ClearChangesCore(visited);

    bool ICollection.IsSynchronized => ((ICollection)_trackableCollection).IsSynchronized;
    object ICollection.SyncRoot => ((ICollection)_trackableCollection).SyncRoot;

    bool IList.IsFixedSize => ((IList)_trackableCollection).IsFixedSize;

    object? IList.this[int index]
    {
        get => _trackableCollection.GetAtIndex(index).Value;
        set => throw new NotImplementedException();
    }

    T IList<T>.this[int index]
    {
        get => TypedItem(_trackableCollection.GetAtIndex(index).Value);
        set => throw new NotImplementedException();
    }

    public int Count => _trackableCollection.Count;
    public bool IsReadOnly => _trackableCollection.IsReadOnly;

    public TrackableCollection()
    {
        _trackableCollection = new TrackableCollection(DescriptorFactory, EqualityComparer<T>.Default);
    }

    public TrackableCollection(IEnumerable<T> items)
    {
        _trackableCollection = new TrackableCollection(DescriptorFactory, items, EqualityComparer<T>.Default);
    }

    private static T TypedItem(object? item)
    {
        return item is not T typedItem
            ? throw new ArgumentException("Cannot add item to trackable collection", nameof(item))
            : typedItem;
    }

    int IList.Add(object? item) => _trackableCollection.AddCore(TypedItem(item));
    void ICollection<T>.Add(T item) => _trackableCollection.AddCore(item);

    void IList.Clear() => _trackableCollection.ClearCore();
    void ICollection<T>.Clear() => _trackableCollection.ClearCore();

    bool IList.Contains(object? item) => _trackableCollection.FindIndex(TypedItem(item)) >= 0;
    bool ICollection<T>.Contains(T item) => _trackableCollection.FindIndex(item) >= 0;

    int IList.IndexOf(object? item) => _trackableCollection.FindIndex(item);
    int IList<T>.IndexOf(T item) => _trackableCollection.FindIndex(item);

    void IList.Insert(int index, object? item) => _trackableCollection.InsertCore(index, TypedItem(item));
    void IList<T>.Insert(int index, T item) => _trackableCollection.InsertCore(index, item);

    void IList.Remove(object? item) => _trackableCollection.RemoveCore(TypedItem(item));
    bool ICollection<T>.Remove(T item) => _trackableCollection.RemoveCore(item);

    void IList.RemoveAt(int index) => _trackableCollection.RemoveAtCore(index);
    void IList<T>.RemoveAt(int index) => _trackableCollection.RemoveAtCore(index);

    void ICollection.CopyTo(Array array, int arrayIndex) => _trackableCollection.CopyToCore(array, arrayIndex);
    void ICollection<T>.CopyTo(T[] array, int arrayIndex) => _trackableCollection.CopyToCore(array, arrayIndex);

    IEnumerator IEnumerable.GetEnumerator() => _trackableCollection.GetEnumerator<object>();
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => _trackableCollection.GetEnumerator<T>();
}