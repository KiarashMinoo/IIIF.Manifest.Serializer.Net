namespace IIIF.Manifests.Serializer.Validation;

/// <summary>
///     Options controlling <see cref="IiifValidator" /> behavior. <paramref name="Version" />
///     makes some rules version-aware (e.g. flagging 3.0-only data that would be silently dropped
///     if written as <see cref="IiifPresentationVersion.V2_0" />/<see cref="IiifPresentationVersion.V2_1" /> -
///     see <c>docs/README.md</c>'s "Downgrade limitations" table for the same set of lossy
///     properties this cross-checks against). <paramref name="Strict" /> enables additional
///     pedantic checks (e.g. well-formed URI checks on recommended-but-not-required properties)
///     that are useful for authoring new documents but too noisy to run by default against
///     tolerantly-imported real-world data.
/// </summary>
public sealed record IiifValidationOptions(IiifPresentationVersion Version = IiifPresentationVersion.V3_0, bool Strict = false)
{
    public static IiifValidationOptions Default { get; } = new();
}
