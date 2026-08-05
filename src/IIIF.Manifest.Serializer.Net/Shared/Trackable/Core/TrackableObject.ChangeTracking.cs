using System.Collections;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using IIIF.Manifests.Serializer.ChangeTracking;
using IIIF.Manifests.Serializer.Shared.Trackable.Collections;

namespace IIIF.Manifests.Serializer.Shared.Trackable.Core;

/// <summary>
///     Shared pull-based change tracker for both property-backed objects and descriptor-backed
///     collections. Derived collection types override the three recursive core operations; the
///     public API, cycle protection, path composition, and descendant traversal live here once.
/// </summary>
public partial class TrackableObject : IIiifChangeTrackable
{
    public bool HasChanges => HasChangesCore(new HashSet<object>(ReferenceEqualityComparer.Instance));

    public IReadOnlyCollection<IiifChangeEntry> GetChanges()
    {
        var entries = new List<IiifChangeEntry>();
        GetChangesCore(entries, new HashSet<object>(ReferenceEqualityComparer.Instance), DateTimeOffset.UtcNow);
        return entries;
    }

    public void ClearChanges()
    {
        ClearChangesCore(new HashSet<object>(ReferenceEqualityComparer.Instance));
    }

    public void AcceptChanges()
    {
        ClearChanges();
    }

    internal virtual bool HasChangesCore(HashSet<object> visited)
    {
        if (!visited.Add(this)) return false;

        foreach (var descriptor in ElementDescriptors.Values)
        {
            if (IsItemCollection(descriptor.Value))
            {
                if (ItemCollectionDescriptorHasChanges(descriptor, visited)) return true;
                continue;
            }

            if (IsChanged(descriptor.ModificationType) || HasNestedChanges(descriptor.Value, visited)) return true;
        }

        return false;
    }

    internal virtual void GetChangesCore(List<IiifChangeEntry> entries, HashSet<object> visited, DateTimeOffset changedAtUtc)
    {
        if (!visited.Add(this)) return;

        foreach (var (propertyName, descriptor) in ElementDescriptors)
        {
            if (IsItemCollection(descriptor.Value))
            {
                CollectItemCollectionDescriptorChanges(propertyName, descriptor, entries, visited, changedAtUtc);
                continue;
            }

            switch (descriptor.ModificationType)
            {
                case ModificationType.Added:
                    entries.Add(new IiifChangeEntry(propertyName, IiifChangeKind.Added, propertyName, null, descriptor.Value, changedAtUtc));
                    break;
                case ModificationType.Changed:
                    entries.Add(new IiifChangeEntry(propertyName, IiifChangeKind.Modified, propertyName, descriptor.OriginalValue, descriptor.Value, changedAtUtc));
                    break;
                case ModificationType.Removed:
                    entries.Add(new IiifChangeEntry(propertyName, IiifChangeKind.Removed, propertyName, descriptor.OriginalValue, null, changedAtUtc));
                    break;
                default:
                    CollectNestedChanges(descriptor.Value, propertyName, entries, visited, changedAtUtc);
                    break;
            }
        }
    }

    internal virtual void ClearChangesCore(HashSet<object> visited)
    {
        if (!visited.Add(this)) return;

        foreach (var key in ElementDescriptors.Keys.ToList())
        {
            var descriptor = ElementDescriptors[key];
            if (descriptor.ModificationType == ModificationType.Removed)
            {
                ElementDescriptors.Remove(key);
                continue;
            }

            ClearNestedChanges(descriptor.Value, visited);
            ElementDescriptors[key] = new ElementDescriptor(descriptor.Value, descriptor.IsAdditional);
        }
    }

    private static bool ItemCollectionDescriptorHasChanges(IElementDescriptor descriptor, HashSet<object> visited)
    {
        switch (descriptor.ModificationType)
        {
            case ModificationType.Added:
            case ModificationType.Removed:
                return true;
            case ModificationType.Changed:
            {
                var original = ToObjectList(descriptor.OriginalValue);
                var current = ToObjectList(descriptor.Value);
                if (!HaveSameReferences(original, current)) return true;
                return current.Any(item => HasNestedChanges(item, visited));
            }
            default:
                return HasNestedChanges(descriptor.Value, visited);
        }
    }

    private static void CollectItemCollectionDescriptorChanges(
        string propertyName,
        IElementDescriptor descriptor,
        List<IiifChangeEntry> entries,
        HashSet<object> visited,
        DateTimeOffset changedAtUtc)
    {
        if (descriptor.ModificationType is ModificationType.Unchanged or ModificationType.Unknown)
        {
            CollectNestedChanges(descriptor.Value, propertyName, entries, visited, changedAtUtc);
            return;
        }

        var original = descriptor.ModificationType == ModificationType.Added
            ? []
            : ToObjectList(descriptor.OriginalValue);
        var current = descriptor.ModificationType == ModificationType.Removed
            ? []
            : ToObjectList(descriptor.Value);

        if (original.Count == 0 && current.Count == 0)
        {
            var kind = descriptor.ModificationType switch
            {
                ModificationType.Added => IiifChangeKind.Added,
                ModificationType.Removed => IiifChangeKind.Removed,
                _ => IiifChangeKind.Modified
            };
            entries.Add(new IiifChangeEntry(propertyName, kind, propertyName,
                kind == IiifChangeKind.Added ? null : descriptor.OriginalValue,
                kind == IiifChangeKind.Removed ? null : descriptor.Value,
                changedAtUtc));
            return;
        }

        CollectReferenceDiff(propertyName, propertyName, original, current, entries, visited, changedAtUtc);
    }

    internal static bool HasNestedChanges(object? value, HashSet<object> visited)
    {
        switch (value)
        {
            case null:
                return false;
            case TrackableObject trackable:
                return trackable.HasChangesCore(visited);
            case IIiifChangeTrackable externalTrackable:
                return externalTrackable.HasChanges;
            case IEnumerable enumerable and not string:
                foreach (var item in enumerable)
                    if (HasNestedChanges(item, visited))
                        return true;
                return false;
            default:
                return false;
        }
    }

    internal static void CollectNestedChanges(
        object? value,
        string parentPath,
        List<IiifChangeEntry> entries,
        HashSet<object> visited,
        DateTimeOffset changedAtUtc)
    {
        switch (value)
        {
            case TrackableObject trackable:
            {
                var childEntries = new List<IiifChangeEntry>();
                trackable.GetChangesCore(childEntries, visited, changedAtUtc);
                AddPrefixedEntries(parentPath, null, childEntries, entries);
                return;
            }
            case IIiifChangeTrackable externalTrackable:
                AddPrefixedEntries(parentPath, null, externalTrackable.GetChanges(), entries);
                return;
            case IEnumerable enumerable and not string:
            {
                var index = 0;
                foreach (var item in enumerable)
                {
                    CollectNestedChanges(item, $"{parentPath}[{index}]", entries, visited, changedAtUtc);
                    index++;
                }

                return;
            }
        }
    }

    internal static void ClearNestedChanges(object? value, HashSet<object> visited)
    {
        switch (value)
        {
            case TrackableObject trackable:
                trackable.ClearChangesCore(visited);
                return;
            case IIiifChangeTrackable externalTrackable:
                externalTrackable.ClearChanges();
                return;
            case IEnumerable enumerable and not string:
                foreach (var item in enumerable) ClearNestedChanges(item, visited);
                return;
        }
    }

    internal static void AddPrefixedEntries(
        string parentPath,
        string? collectionPropertyName,
        IEnumerable<IiifChangeEntry> childEntries,
        List<IiifChangeEntry> entries)
    {
        foreach (var child in childEntries)
        {
            entries.Add(child with
            {
                Path = CombinePath(parentPath, child.Path),
                PropertyName = child.PropertyName ?? collectionPropertyName
            });
        }
    }

    internal static void CollectReferenceDiff(
        string parentPath,
        string? propertyName,
        IReadOnlyList<object?> original,
        IReadOnlyList<object?> current,
        List<IiifChangeEntry> entries,
        HashSet<object> visited,
        DateTimeOffset changedAtUtc)
    {
        var matchedOriginal = new bool[original.Count];
        var matchedCurrent = new bool[current.Count];

        for (var currentIndex = 0; currentIndex < current.Count; currentIndex++)
        {
            for (var originalIndex = 0; originalIndex < original.Count; originalIndex++)
            {
                if (matchedOriginal[originalIndex] || !ReferenceEquals(original[originalIndex], current[currentIndex])) continue;
                matchedOriginal[originalIndex] = true;
                matchedCurrent[currentIndex] = true;
                break;
            }
        }

        for (var originalIndex = 0; originalIndex < original.Count; originalIndex++)
            if (!matchedOriginal[originalIndex])
                entries.Add(new IiifChangeEntry(
                    $"{parentPath}[{originalIndex}]",
                    IiifChangeKind.CollectionItemRemoved,
                    propertyName,
                    original[originalIndex],
                    null,
                    changedAtUtc));

        for (var currentIndex = 0; currentIndex < current.Count; currentIndex++)
        {
            var item = current[currentIndex];
            if (!matchedCurrent[currentIndex])
            {
                entries.Add(new IiifChangeEntry(
                    $"{parentPath}[{currentIndex}]",
                    IiifChangeKind.CollectionItemAdded,
                    propertyName,
                    null,
                    item,
                    changedAtUtc));
            }
            else
            {
                CollectNestedChanges(item, $"{parentPath}[{currentIndex}]", entries, visited, changedAtUtc);
            }
        }
    }

    internal static bool IsChanged(ModificationType modificationType)
    {
        return modificationType is ModificationType.Added or ModificationType.Changed or ModificationType.Removed;
    }

    private static readonly ConcurrentDictionary<Type, bool> ItemCollectionTypeCache = new();

    private static bool IsItemCollection(object? value)
    {
        if (value is not ITrackableCollection) return false;

        return ItemCollectionTypeCache.GetOrAdd(value.GetType(), static type =>
            type.IsGenericType &&
            type.GetGenericTypeDefinition() == typeof(TrackableCollection<>) &&
            typeof(IBaseItem).IsAssignableFrom(type.GetGenericArguments()[0]));
    }

    private static List<object?> ToObjectList(object? value)
    {
        return value is IEnumerable enumerable and not string
            ? enumerable.Cast<object?>().ToList()
            : [];
    }

    private static bool HaveSameReferences(IReadOnlyList<object?> original, IReadOnlyList<object?> current)
    {
        if (original.Count != current.Count) return false;

        var matched = new bool[original.Count];
        for (var currentIndex = 0; currentIndex < current.Count; currentIndex++)
        {
            var found = false;
            for (var originalIndex = 0; originalIndex < original.Count; originalIndex++)
            {
                if (matched[originalIndex] || !ReferenceEquals(original[originalIndex], current[currentIndex])) continue;
                matched[originalIndex] = true;
                found = true;
                break;
            }

            if (!found) return false;
        }

        return true;
    }

    private static string CombinePath(string parentPath, string childPath)
    {
        if (string.IsNullOrEmpty(parentPath)) return childPath;
        if (string.IsNullOrEmpty(childPath)) return parentPath;
        return childPath[0] == '[' ? $"{parentPath}{childPath}" : $"{parentPath}.{childPath}";
    }

    private sealed class ReferenceEqualityComparer : IEqualityComparer<object>
    {
        internal static readonly ReferenceEqualityComparer Instance = new();

        bool IEqualityComparer<object>.Equals(object? x, object? y) => ReferenceEquals(x, y);
        int IEqualityComparer<object>.GetHashCode(object obj) => RuntimeHelpers.GetHashCode(obj);
    }
}