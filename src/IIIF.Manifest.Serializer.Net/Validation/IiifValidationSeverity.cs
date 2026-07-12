namespace IIIF.Manifests.Serializer.Validation;

/// <summary>
///     Severity of a single <see cref="IiifValidationError" />. Only <see cref="Error" /> affects
///     <see cref="IiifValidationResult.IsValid" /> - <see cref="Warning" />/<see cref="Info" />
///     surface real, spec-relevant observations without treating the document as invalid, matching
///     this SDK's tolerant-by-default parsing philosophy (validation is an opt-in, separate layer).
/// </summary>
public enum IiifValidationSeverity
{
    Info,
    Warning,
    Error
}
