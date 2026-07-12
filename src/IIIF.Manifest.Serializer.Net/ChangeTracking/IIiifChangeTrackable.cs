namespace IIIF.Manifests.Serializer.ChangeTracking;

/// <summary>
///     Change-tracking contract implemented generically by every <c>TrackableObject&lt;T&gt;</c> -
///     which is to say, essentially every SDK model object (Manifest, Collection, Canvas, Range,
///     Annotation, AnnotationPage, content resources, services, extension objects, and even simple
///     value types like <c>Label</c>/<c>Description</c>). See <c>docs/CHANGE_TRACKING.md</c> for the
///     full design (pull-based diffing over <c>ElementDescriptors</c>' existing Original/Modified
///     value pair, rather than an explicit parent-attachment/event-bubbling design) and why that
///     design was chosen over the issue's originally-suggested <c>IIiifObjectGraphTrackable</c>
///     (<c>Parent</c>/<c>AttachParent</c>) shape.
/// </summary>
public interface IIiifChangeTrackable
{
    /// <summary>
    ///     <see langword="true" /> if this object or any descendant reachable through its own
    ///     properties/collections has a pending change since the last <see cref="ClearChanges" />
    ///     (or since construction, if never cleared).
    /// </summary>
    bool HasChanges { get; }

    /// <summary>
    ///     All pending changes on this object and its descendants, with paths relative to this
    ///     object as the root (e.g. <c>"items[0].label"</c>).
    /// </summary>
    IReadOnlyCollection<IiifChangeEntry> GetChanges();

    /// <summary>
    ///     Establishes the current state as the new baseline, recursively through every descendant -
    ///     after this call, <see cref="HasChanges" /> is <see langword="false" /> across the whole
    ///     subtree rooted at this object, and future mutations are tracked freshly from here.
    /// </summary>
    void ClearChanges();

    /// <summary>
    ///     Semantic alias for <see cref="ClearChanges" /> (matching EF Core's <c>ChangeTracker.AcceptAllChanges</c>
    ///     naming) - the issue requesting this feature explicitly allowed either an alias or a
    ///     separate-but-equivalent operation; this SDK treats them as identical since there is no
    ///     meaningful distinction here (no separate "pending delete" concept to reconcile).
    /// </summary>
    void AcceptChanges();
}
