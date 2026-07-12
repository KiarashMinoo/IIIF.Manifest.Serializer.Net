namespace IIIF.Manifests.Serializer.Validation;

/// <summary>
///     A single validation finding. <paramref name="RuleId" /> is a stable, documented identifier
///     (e.g. <c>"canvas-dimensions-required"</c>) a caller can filter/suppress on without depending
///     on the human-readable <paramref name="Message" /> text. <paramref name="Path" /> is a
///     simplified JSONPath-like pointer (e.g. <c>"$.items[2].behavior"</c>) to the offending
///     location, best-effort rather than a full JSONPath implementation.
/// </summary>
public sealed record IiifValidationError(string RuleId, IiifValidationSeverity Severity, string Message, string Path);
