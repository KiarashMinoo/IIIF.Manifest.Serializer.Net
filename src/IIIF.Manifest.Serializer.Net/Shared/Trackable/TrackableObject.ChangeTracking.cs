using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using IIIF.Manifests.Serializer.ChangeTracking;

namespace IIIF.Manifests.Serializer.Shared.Trackable;

/// <summary>
///     Non-generic dispatch surface letting one closed <see cref="TrackableObject{T}" /> instantiation
///     (e.g. <c>TrackableObject&lt;Canvas&gt;</c>) invoke another's (e.g. <c>TrackableObject&lt;Manifest&gt;</c>)
///     visited-set-threaded core walk. Each closed generic instantiation is a genuinely distinct
///     runtime type, so a <c>private</c> method on one can't be called from the other directly even
///     though both come from the same template - this internal, explicitly-implemented interface is
///     the dispatch point instead. Never implemented by anything outside this file.
/// </summary>
internal interface IChangeTrackingCoreAccess
{
    bool HasChangesCore(HashSet<object> visited);
    void GetChangesCore(List<IiifChangeEntry> entries, HashSet<object> visited);
    void ClearChangesCore(HashSet<object> visited);
}

/// <summary>
///     Generic <see cref="IIiifChangeTrackable" /> implementation shared by every
///     <see cref="TrackableObject{TTrackableObject}" />-derived type - which is to say, essentially
///     the entire SDK object graph (Manifest, Collection, Canvas, Range, Annotation, AnnotationPage,
///     content resources, services, extension objects, and value types like Label/Description). See
///     <c>docs/CHANGE_TRACKING.md</c> for the full design rationale.
///     <para>
///         This is a <b>pull-based</b> diff, not a push/event-bubbling design with explicit parent
///         attachment - deliberately different from the issue's originally-suggested
///         <c>IIiifObjectGraphTrackable</c> (<c>Parent</c>/<c>AttachParent</c>) shape. A child mutated
///         through a reference held outside its parent (e.g. <c>canvas.SetHeight(...)</c> after
///         <c>manifest.AddItem(canvas)</c>) is still correctly detected by <c>manifest.HasChanges</c>,
///         because the check walks the object graph fresh on every call rather than relying on a
///         wire-up that could go stale or be missed at some call site.
///     </para>
///     <para>
///         Scalar/single-reference properties reuse <c>ElementDescriptors</c>' pre-existing
///         Original/Modified value pair (already maintained by <c>SetElementValue</c>, independent
///         of this feature) directly. <see cref="IBaseItem" />-typed collections (<c>Items</c>,
///         <c>Structures</c>, ...) need their own separate baseline snapshot
///         (<see cref="_collectionBaselines" />) instead: an <c>ElementDescriptor</c>'s own
///         Original/Modified pair only reflects "the value at first-ever assignment" vs. "now," which
///         would silently miss the very first item added to a collection that had never been touched
///         before the last <see cref="ClearChanges" /> (e.g. a freshly-constructed
///         <c>Manifest</c>'s empty <c>Items</c>) - a real gap found by actually running the tests
///         for this feature's own canonical example, not assumed correct from the design alone.
///     </para>
/// </summary>
public partial class TrackableObject<TTrackableObject> : IIiifChangeTrackable, IChangeTrackingCoreAccess
    where TTrackableObject : TrackableObject<TTrackableObject>
{
    private Dictionary<string, List<object>>? _collectionBaselines;

    /// <summary>
    ///     The set of <c>ElementDescriptors</c> keys that existed at the last <see cref="ClearChanges" />
    ///     call (or <see langword="null" /> if never cleared - meaning we're still in "initial
    ///     construction" phase, where nothing counts as a change yet, no matter what). Needed
    ///     because an <see cref="ElementDescriptor" />'s own Original/Modified pair treats a
    ///     property's very first-ever assignment as "the baseline, not a change" - correct for
    ///     properties set during construction, but wrong for a property set for the first time
    ///     *after* an explicit <see cref="ClearChanges" /> (e.g. <c>manifest.ClearChanges();
    ///     manifest.SetRights(...)</c> on a Manifest that never had Rights before) - another real
    ///     gap found by running this feature's own tests, not assumed correct from the design alone.
    /// </summary>
    private HashSet<string>? _keysAtLastClear;

    public bool HasChanges => HasChangesCore(new HashSet<object>(ReferenceEqualityComparer.Instance));

    public IReadOnlyCollection<IiifChangeEntry> GetChanges()
    {
        var entries = new List<IiifChangeEntry>();
        GetChangesCore(entries, new HashSet<object>(ReferenceEqualityComparer.Instance));
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

    bool IChangeTrackingCoreAccess.HasChangesCore(HashSet<object> visited)
    {
        return HasChangesCore(visited);
    }

    void IChangeTrackingCoreAccess.GetChangesCore(List<IiifChangeEntry> entries, HashSet<object> visited)
    {
        GetChangesCore(entries, visited);
    }

    void IChangeTrackingCoreAccess.ClearChangesCore(HashSet<object> visited)
    {
        ClearChangesCore(visited);
    }

    private bool HasChangesCore(HashSet<object> visited)
    {
        // A cycle (or a shared/re-visited reference within the same graph walk) contributes no
        // *further* changes from this revisit - whatever changes it has were already accounted for
        // (or will be) at its first visit.
        if (!visited.Add(this)) return false;

        foreach (var (propertyName, descriptor) in ElementDescriptors)
        {
            if (TryGetTrackableCollectionElementType(descriptor.Value, out _))
            {
                if (CollectionHasChanges(propertyName, descriptor, visited)) return true;
            }
            else
            {
                if (descriptor.IsModified || IsNewSinceLastClear(propertyName)) return true;
                if (HasNestedChanges(descriptor.Value, visited)) return true;
            }
        }

        return false;
    }

    private void GetChangesCore(List<IiifChangeEntry> entries, HashSet<object> visited)
    {
        if (!visited.Add(this)) return;

        var now = DateTimeOffset.UtcNow;

        foreach (var (propertyName, descriptor) in ElementDescriptors)
        {
            if (TryGetTrackableCollectionElementType(descriptor.Value, out _))
            {
                CollectCollectionChanges(propertyName, descriptor, entries, visited, now);
            }
            else if (descriptor.IsModified)
            {
                entries.Add(new IiifChangeEntry(propertyName, IiifChangeKind.Modified, propertyName, descriptor.OriginalValue, descriptor.ModifiedValue, now));
            }
            else if (IsNewSinceLastClear(propertyName))
            {
                // First-ever assignment to this key, but it happened after an explicit
                // ClearChanges() established a baseline that didn't include it - there was
                // nothing here before, so OriginalValue is genuinely null (not descriptor.OriginalValue,
                // which for a never-modified descriptor just holds the same current value).
                entries.Add(new IiifChangeEntry(propertyName, IiifChangeKind.Modified, propertyName, null, descriptor.Value, now));
            }
            else if (descriptor.Value is IIiifChangeTrackable trackable)
                PropagateChildChanges(propertyName, trackable, entries, visited);
        }
    }

    private void ClearChangesCore(HashSet<object> visited)
    {
        if (!visited.Add(this)) return;

        _keysAtLastClear = new HashSet<string>(ElementDescriptors.Keys);

        foreach (var key in ElementDescriptors.Keys.ToList())
        {
            var descriptor = ElementDescriptors[key];
            ElementDescriptors[key] = new ElementDescriptor(descriptor.Value, descriptor.IsAdditional);

            if (TryGetTrackableCollectionElementType(descriptor.Value, out _))
            {
                _collectionBaselines ??= new Dictionary<string, List<object>>();
                _collectionBaselines[key] = ToList(descriptor.Value);
            }

            ClearNestedChanges(descriptor.Value, visited);
        }
    }

    private bool IsNewSinceLastClear(string propertyName)
    {
        return _keysAtLastClear is not null && !_keysAtLastClear.Contains(propertyName);
    }

    private List<object> GetCollectionBaseline(string propertyName)
    {
        return _collectionBaselines is not null && _collectionBaselines.TryGetValue(propertyName, out var baseline) ? baseline : [];
    }

    private bool CollectionHasChanges(string propertyName, ElementDescriptor descriptor, HashSet<object> visited)
    {
        var original = GetCollectionBaseline(propertyName);
        var current = ToList(descriptor.Value);

        if (original.Count != current.Count) return true;
        if (original.Where((t, i) => !ReferenceEquals(t, current[i])).Any()) return true;

        return current.Any(item => item is IIiifChangeTrackable childTrackable && InvokeHasChangesCore(childTrackable, visited));
    }

    private static void PropagateChildChanges(string parentPath, IIiifChangeTrackable trackable, List<IiifChangeEntry> entries, HashSet<object> visited)
    {
        var childEntries = new List<IiifChangeEntry>();
        InvokeGetChangesCore(trackable, childEntries, visited);
        foreach (var child in childEntries) entries.Add(child with { Path = $"{parentPath}.{child.Path}" });
    }

    private void CollectCollectionChanges(string propertyName, ElementDescriptor descriptor, List<IiifChangeEntry> entries, HashSet<object> visited, DateTimeOffset now)
    {
        var original = GetCollectionBaseline(propertyName);
        var current = ToList(descriptor.Value);

        foreach (var item in original)
            if (!current.Any(x => ReferenceEquals(x, item)))
            {
                var index = original.IndexOf(item);
                entries.Add(new IiifChangeEntry($"{propertyName}[{index}]", IiifChangeKind.CollectionItemRemoved, propertyName, item, null, now));
            }

        for (var i = 0; i < current.Count; i++)
        {
            var item = current[i];
            var wasAlreadyPresent = original.Any(x => ReferenceEquals(x, item));

            if (!wasAlreadyPresent)
                entries.Add(new IiifChangeEntry($"{propertyName}[{i}]", IiifChangeKind.CollectionItemAdded, propertyName, null, item, now));
            else if (item is IIiifChangeTrackable childTrackable) PropagateChildChanges($"{propertyName}[{i}]", childTrackable, entries, visited);
        }
    }

    private static bool HasNestedChanges(object? value, HashSet<object> visited)
    {
        switch (value)
        {
            case null:
                return false;
            case IIiifChangeTrackable trackable:
                return InvokeHasChangesCore(trackable, visited);
            case IEnumerable enumerable and not string:
            {
                foreach (var item in enumerable)
                    if (item is IIiifChangeTrackable itemTrackable && InvokeHasChangesCore(itemTrackable, visited))
                        return true;
                return false;
            }
            default:
                return false;
        }
    }

    private static void ClearNestedChanges(object? value, HashSet<object> visited)
    {
        switch (value)
        {
            case IIiifChangeTrackable trackable:
                InvokeClearChangesCore(trackable, visited);
                return;
            case IEnumerable enumerable and not string:
            {
                foreach (var item in enumerable)
                    if (item is IIiifChangeTrackable itemTrackable)
                        InvokeClearChangesCore(itemTrackable, visited);
                return;
            }
        }
    }

    private static bool InvokeHasChangesCore(IIiifChangeTrackable trackable, HashSet<object> visited)
    {
        return trackable is IChangeTrackingCoreAccess access ? access.HasChangesCore(visited) : trackable.HasChanges;
    }

    private static void InvokeGetChangesCore(IIiifChangeTrackable trackable, List<IiifChangeEntry> entries, HashSet<object> visited)
    {
        if (trackable is IChangeTrackingCoreAccess access) access.GetChangesCore(entries, visited);
        else foreach (var entry in trackable.GetChanges()) entries.Add(entry);
    }

    private static void InvokeClearChangesCore(IIiifChangeTrackable trackable, HashSet<object> visited)
    {
        if (trackable is IChangeTrackingCoreAccess access) access.ClearChangesCore(visited);
        else trackable.ClearChanges();
    }

    private static bool TryGetTrackableCollectionElementType(object? value, out Type elementType)
    {
        elementType = typeof(object);
        if (value is null or string) return false;

        var type = value.GetType();
        if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(BindingList<>)) return false;

        elementType = type.GetGenericArguments()[0];
        return typeof(IBaseItem).IsAssignableFrom(elementType);
    }

    private static List<object> ToList(object? value)
    {
        return value is IEnumerable enumerable and not string ? enumerable.Cast<object>().ToList() : [];
    }

    private sealed class ReferenceEqualityComparer : IEqualityComparer<object>
    {
        public static readonly ReferenceEqualityComparer Instance = new();

        bool IEqualityComparer<object>.Equals(object? x, object? y)
        {
            return ReferenceEquals(x, y);
        }

        int IEqualityComparer<object>.GetHashCode(object obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }
}
