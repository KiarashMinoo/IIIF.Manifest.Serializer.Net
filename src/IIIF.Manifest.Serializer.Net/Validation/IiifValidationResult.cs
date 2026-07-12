using System.Collections.Generic;
using System.Linq;

namespace IIIF.Manifests.Serializer.Validation;

/// <summary>
///     The outcome of an <see cref="IiifValidator" /> call. <see cref="IsValid" /> is
///     <see langword="true" /> whenever no <see cref="IiifValidationSeverity.Error" />-severity
///     entry is present - <see cref="IiifValidationSeverity.Warning" />/<see cref="IiifValidationSeverity.Info" />
///     findings never make a document invalid on their own, consistent with this SDK's tolerant
///     parser: validation reports problems, it never blocks import.
/// </summary>
public sealed class IiifValidationResult
{
    internal IiifValidationResult(IReadOnlyCollection<IiifValidationError> errors)
    {
        Errors = errors;
    }

    public IReadOnlyCollection<IiifValidationError> Errors { get; }

    public bool IsValid => Errors.All(x => x.Severity != IiifValidationSeverity.Error);
}
