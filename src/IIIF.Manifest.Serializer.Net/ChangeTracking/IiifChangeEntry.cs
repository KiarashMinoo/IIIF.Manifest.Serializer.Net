namespace IIIF.Manifests.Serializer.ChangeTracking;

/// <summary>
///     A single tracked change. <paramref name="Path" /> is an SDK-style dotted/bracketed path from
///     the root object (e.g. <c>"items[0].label"</c>, matching this SDK's own convention rather than
///     JSON Pointer or JSONPath - see <c>docs/CHANGE_TRACKING.md</c> for the rationale).
///     <paramref name="PropertyName" /> is the raw C# member name that changed (the last path
///     segment, without any collection index). <paramref name="OriginalValue" />/
///     <paramref name="CurrentValue" /> are the raw values captured at tracking time - for
///     <see cref="IiifChangeKind.CollectionItemAdded" />/<see cref="IiifChangeKind.CollectionItemRemoved" />
///     entries only one of the two is populated (the added or removed item respectively).
/// </summary>
public sealed record IiifChangeEntry(
    string Path,
    IiifChangeKind Kind,
    string? PropertyName,
    object? OriginalValue,
    object? CurrentValue,
    DateTimeOffset ChangedAtUtc);
