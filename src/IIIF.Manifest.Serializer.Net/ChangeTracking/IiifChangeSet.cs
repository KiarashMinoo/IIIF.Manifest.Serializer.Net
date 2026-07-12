namespace IIIF.Manifests.Serializer.ChangeTracking;

/// <summary>
///     A delta envelope: the full, precise list of <see cref="Changes" /> (including removals,
///     which cannot be represented in a pure partial-document shape - see
///     <c>docs/CHANGE_TRACKING.md</c>) plus, where the changes can be expressed as a valid partial
///     IIIF document, a best-effort reconstruction in <see cref="ChangedManifest" />. Prefer this
///     over the bare <c>GetChangedManifest()</c> convenience whenever a consumer needs to reliably
///     detect and apply removals, since those never appear in <see cref="ChangedManifest" />.
/// </summary>
public sealed class IiifChangeSet
{
    public IiifChangeSet(string rootId, string rootType, DateTimeOffset createdAtUtc, IReadOnlyCollection<IiifChangeEntry> changes, Nodes.Manifest? changedManifest)
    {
        RootId = rootId;
        RootType = rootType;
        CreatedAtUtc = createdAtUtc;
        Changes = changes;
        ChangedManifest = changedManifest;
    }

    public string RootId { get; }
    public string RootType { get; }
    public DateTimeOffset CreatedAtUtc { get; }
    public IReadOnlyCollection<IiifChangeEntry> Changes { get; }
    public Nodes.Manifest? ChangedManifest { get; }
}
