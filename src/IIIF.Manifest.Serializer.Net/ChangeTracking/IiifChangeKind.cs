namespace IIIF.Manifests.Serializer.ChangeTracking;

/// <summary>
///     The nature of a single <see cref="IiifChangeEntry" />. <see cref="ChildChanged" /> is
///     reserved for future use by callers building their own change-summarization logic - this
///     SDK's own <see cref="IIiifChangeTrackable.GetChanges" /> implementation re-emits a changed
///     descendant's own concrete entries (e.g. <see cref="Modified" />) with a prefixed path rather
///     than wrapping them in a generic "something changed below here" entry, since the concrete
///     kind is strictly more useful to a caller than a generic wrapper.
/// </summary>
public enum IiifChangeKind
{
    Added,
    Modified,
    Removed,
    CollectionItemAdded,
    CollectionItemRemoved,
    CollectionCleared,
    ChildChanged
}
