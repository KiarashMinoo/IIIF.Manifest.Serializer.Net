using IIIF.Manifests.Serializer.Shared.Trackable.Collections;

namespace IIIF.Manifests.Serializer.Helpers;

public static class CollectionHelper
{
    public static TCollection Attach<TCollection, TItem>(this TCollection collection, TItem item) where TCollection : ICollection<TItem>
    {
        collection.Add(item);
        return collection;
    }


    public static TCollection AttachRange<TCollection, TItem>(this TCollection collection, IEnumerable<TItem> items) where TCollection : ICollection<TItem>
    {
        foreach (var item in items)
            collection.Add(item);

        return collection;
    }

    public static TCollection Detach<TCollection, TItem>(this TCollection collection, TItem item) where TCollection : ICollection<TItem>
    {
        collection.Remove(item);
        return collection;
    }

    public static TEnumerable Enumerate<TEnumerable, TItem>(this TEnumerable enumerable, Action<TItem> action) where TEnumerable : IEnumerable<TItem>
    {
        foreach (var item in enumerable)
            action.Invoke(item);

        return enumerable;
    }

    public static IEnumerable<TItem> Enumerate<TItem>(this IEnumerable<TItem> enumerable, Action<TItem> action)
    {
        return enumerable.Enumerate<IEnumerable<TItem>, TItem>(action);
    }

    extension<TItem>(IReadOnlyCollection<TItem>? collection) where TItem : notnull
    {
        /// <summary>
        ///     Creates a new IReadOnlyCollection with the specified item added.
        /// </summary>
        public IReadOnlyCollection<TItem> With(TItem item)
        {
            if (collection is ITrackableCollection<TItem> trackableCollection)
            {
                trackableCollection.Add(item);
                return trackableCollection;
            }

            var hashSet = new HashSet<TItem>();

            if (collection != null)
                foreach (var tItem in collection)
                    hashSet.Add(tItem);

            hashSet.Add(item);

            return hashSet;
        }

        /// <summary>
        ///     Creates a new IReadOnlyCollection with the specified item removed.
        /// </summary>
        public IReadOnlyCollection<TItem> Without(TItem item)
        {
            if (collection is ITrackableCollection<TItem> trackableCollection)
            {
                trackableCollection.Remove(item);
                return trackableCollection;
            }

            var hashSet = new HashSet<TItem>();

            if (collection != null)
                foreach (var tItem in collection)
                    hashSet.Add(tItem);

            hashSet.Remove(item);

            return hashSet;
        }
    }
}
