using IIIF.Manifests.Serializer.ChangeTracking;
using IIIF.Manifests.Serializer.Shared.Trackable.Core;

namespace IIIF.Manifests.Serializer.Shared.Trackable.Collections;

public partial class TrackableCollection
{
    internal override bool HasChangesCore(HashSet<object> visited)
    {
        if (!visited.Add(this)) return false;

        if (_removedItems.Count > 0) return true;

        foreach (var descriptor in _items)
        {
            if (IsChanged(descriptor.ModificationType)) return true;
            if (HasNestedChanges(descriptor.Value, visited)) return true;
        }

        return false;
    }

    internal override void GetChangesCore(List<IiifChangeEntry> entries, HashSet<object> visited, DateTimeOffset changedAtUtc)
    {
        if (!visited.Add(this)) return;

        for (var index = 0; index < _items.Count; index++)
        {
            var descriptor = _items[index];
            switch (descriptor.ModificationType)
            {
                case ModificationType.Added:
                    entries.Add(new IiifChangeEntry(
                        $"[{index}]",
                        IiifChangeKind.CollectionItemAdded,
                        null,
                        null,
                        descriptor.Value,
                        changedAtUtc));
                    break;
                case ModificationType.Changed:
                    entries.Add(new IiifChangeEntry(
                        $"[{index}]",
                        IiifChangeKind.Modified,
                        null,
                        descriptor.OriginalValue,
                        descriptor.Value,
                        changedAtUtc));
                    break;
                default:
                    CollectNestedChanges(descriptor.Value, $"[{index}]", entries, visited, changedAtUtc);
                    break;
            }
        }

        foreach (var (descriptor, originalIndex) in _removedItems)
        {
            entries.Add(new IiifChangeEntry(
                $"[{originalIndex}]",
                IiifChangeKind.CollectionItemRemoved,
                null,
                descriptor.OriginalValue,
                null,
                changedAtUtc));
        }
    }

    internal override void ClearChangesCore(HashSet<object> visited)
    {
        if (!visited.Add(this)) return;

        _removedItems.Clear();
        _removedOriginalIndices.Clear();

        for (var index = 0; index < _items.Count; index++)
        {
            var descriptor = _items[index];
            ClearNestedChanges(descriptor.Value, visited);
            _items[index] = _descriptorFactory(descriptor.Value);
        }

        _baselineCount = _items.Count;
    }
}
