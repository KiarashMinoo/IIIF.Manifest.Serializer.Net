using System;
using System.Collections.Generic;

namespace IIIF.Manifests.Serializer.Helpers
{
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
            => enumerable.Enumerate<IEnumerable<TItem>, TItem>(action);
    }
}