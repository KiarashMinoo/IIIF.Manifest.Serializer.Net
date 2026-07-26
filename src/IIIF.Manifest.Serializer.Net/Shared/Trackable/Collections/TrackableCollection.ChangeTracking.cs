using IIIF.Manifests.Serializer.ChangeTracking;
using IIIF.Manifests.Serializer.Shared.Trackable.Core;

namespace IIIF.Manifests.Serializer.Shared.Trackable.Collections;

public partial class TrackableCollection<T>
{
    internal override bool HasChangesCore(HashSet<object> visited)
    {
        if (!visited.Add(this)) return false;

        foreach (var descriptor in _items)
        {
            if (IsChanged(descriptor.ModificationType)) return true;
            if (descriptor.ModificationType != ModificationType.Removed && HasNestedChanges(descriptor.Value, visited)) return true;
        }

        return false;
    }

    internal override void GetChangesCore(List<IiifChangeEntry> entries, HashSet<object> visited, DateTimeOffset changedAtUtc)
    {
        if (!visited.Add(this)) return;

        var originalIndex = 0;
        var currentIndex = 0;

        foreach (var descriptor in _items)
        {
            switch (descriptor.ModificationType)
            {
                case ModificationType.Added:
                    entries.Add(new IiifChangeEntry(
                        $"[{currentIndex}]",
                        IiifChangeKind.CollectionItemAdded,
                        null,
                        null,
                        descriptor.Value,
                        changedAtUtc));
                    currentIndex++;
                    break;
                case ModificationType.Removed:
                    entries.Add(new IiifChangeEntry(
                        $"[{originalIndex}]",
                        IiifChangeKind.CollectionItemRemoved,
                        null,
                        descriptor.OriginalValue,
                        null,
                        changedAtUtc));
                    originalIndex++;
                    break;
                case ModificationType.Changed:
                    entries.Add(new IiifChangeEntry(
                        $"[{currentIndex}]",
                        IiifChangeKind.Modified,
                        null,
                        descriptor.OriginalValue,
                        descriptor.Value,
                        changedAtUtc));
                    originalIndex++;
                    currentIndex++;
                    break;
                default:
                    CollectNestedChanges(descriptor.Value, $"[{currentIndex}]", entries, visited, changedAtUtc);
                    originalIndex++;
                    currentIndex++;
                    break;
            }
        }
    }

    internal override void ClearChangesCore(HashSet<object> visited)
    {
        if (!visited.Add(this)) return;

        for (var index = _items.Count - 1; index >= 0; index--)
        {
            var descriptor = _items[index];
            if (descriptor.ModificationType == ModificationType.Removed)
            {
                _items.RemoveAt(index);
                continue;
            }

            ClearNestedChanges(descriptor.Value, visited);
            _items[index] = new ElementDescriptor<T>(descriptor.Value);
        }
    }
}