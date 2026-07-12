using System;
using System.Collections.Generic;
using System.Linq;
using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Validation;

/// <summary>
///     Opt-in, version-aware validation layer - separate from parsing, which stays tolerant per
///     this SDK's core design (invalid-but-readable documents still deserialize; validation reports
///     problems rather than blocking import). Callers decide whether/when to check a document
///     against these rules; nothing here is invoked implicitly by <see cref="IiifSerializer" />.
/// </summary>
public static class IiifValidator
{
    // Presentation API 3.0 §5.4.3 "Behavior" - which behavior values are valid on which resource
    // type, restricted to the 4 BaseNode-derived types this SDK models with AddBehavior (Manifest,
    // Collection, Canvas, Range). "hidden" (valid on AnnotationCollection/AnnotationPage/Annotation/
    // SpecificResource/Choice) is out of scope here since none of those are BaseNode-derived.
    private static readonly HashSet<string> ManifestBehaviors =
    [
        "auto-advance", "no-auto-advance", "repeat", "no-repeat", "unordered", "individuals", "continuous", "paged"
    ];

    private static readonly HashSet<string> CollectionBehaviors =
    [
        "auto-advance", "no-auto-advance", "repeat", "no-repeat", "unordered", "individuals", "continuous", "paged", "multi-part", "together"
    ];

    private static readonly HashSet<string> CanvasBehaviors = ["auto-advance", "no-auto-advance", "facing-pages", "non-paged"];

    private static readonly HashSet<string> RangeBehaviors =
    [
        "auto-advance", "no-auto-advance", "unordered", "individuals", "continuous", "paged", "sequence", "thumbnail-nav", "no-nav"
    ];

    public static IiifValidationResult ValidateManifest(Manifest manifest, IiifValidationOptions? options = null)
    {
        if (manifest is null) throw new ArgumentNullException(nameof(manifest));

        options ??= IiifValidationOptions.Default;
        var errors = new List<IiifValidationError>();

        ValidateBaseNode(manifest, "$", ManifestBehaviors, options, errors);

        if (manifest.Label.Count == 0)
            errors.Add(new IiifValidationError("manifest-label-required", IiifValidationSeverity.Error, "Manifest requires a non-empty label.", "$.label"));

        var canvases = manifest.Items.OfType<Canvas>().ToList();
        if (canvases.Count == 0)
            errors.Add(new IiifValidationError("manifest-items-empty", IiifValidationSeverity.Warning, "Manifest has no canvases (items is empty).", "$.items"));

        for (var i = 0; i < canvases.Count; i++) ValidateCanvas(canvases[i], $"$.items[{i}]", options, errors);

        if (options.Version is IiifPresentationVersion.V2_0 or IiifPresentationVersion.V2_1) ValidateManifestDowngradeLimitations(manifest, errors);

        return new IiifValidationResult(errors);
    }

    public static IiifValidationResult ValidateCollection(Collection collection, IiifValidationOptions? options = null)
    {
        if (collection is null) throw new ArgumentNullException(nameof(collection));

        options ??= IiifValidationOptions.Default;
        var errors = new List<IiifValidationError>();

        ValidateBaseNode(collection, "$", CollectionBehaviors, options, errors);

        if (collection.Label.Count == 0)
            errors.Add(new IiifValidationError("collection-label-required", IiifValidationSeverity.Error, "Collection requires a non-empty label.", "$.label"));

        if (collection.Items.Count == 0)
            errors.Add(new IiifValidationError("collection-items-empty", IiifValidationSeverity.Warning, "Collection has no items.", "$.items"));

        return new IiifValidationResult(errors);
    }

    /// <summary>
    ///     Parses <paramref name="json" /> via <see cref="IiifSerializer.DeserializeManifest" /> and
    ///     validates the result. A JSON document that cannot even be parsed is reported as a single
    ///     <see cref="IiifValidationSeverity.Error" /> finding rather than throwing - consistent with
    ///     validation being a report-problems layer, not a parser gate.
    /// </summary>
    public static IiifValidationResult ValidateJson(string json, IiifValidationOptions? options = null)
    {
        if (string.IsNullOrWhiteSpace(json)) throw new ArgumentException("JSON string cannot be null or whitespace.", nameof(json));

        options ??= IiifValidationOptions.Default;

        try
        {
            var manifest = IiifSerializer.DeserializeManifest(json);
            return ValidateManifest(manifest, options);
        }
        catch (JsonException ex)
        {
            return new IiifValidationResult([new IiifValidationError("json-parse-error", IiifValidationSeverity.Error, ex.Message, "$")]);
        }
    }

    private static void ValidateCanvas(Canvas canvas, string path, IiifValidationOptions options, List<IiifValidationError> errors)
    {
        ValidateBaseNode(canvas, path, CanvasBehaviors, options, errors);

        var hasDimensions = canvas.Height is > 0 && canvas.Width is > 0;
        var hasDuration = canvas.Duration is > 0;
        if (!hasDimensions && !hasDuration)
            errors.Add(new IiifValidationError("canvas-dimensions-required", IiifValidationSeverity.Error,
                "Canvas must have height and width (for visual content), duration (for time-based content), or both.", path));
    }

    private static void ValidateBaseNode<TBaseNode>(BaseNode<TBaseNode> node, string path, HashSet<string> validBehaviors, IiifValidationOptions options, List<IiifValidationError> errors)
        where TBaseNode : BaseNode<TBaseNode>
    {
        foreach (var behavior in node.Behavior)
            if (!validBehaviors.Contains(behavior.Value))
                errors.Add(new IiifValidationError("behavior-not-valid-for-resource-type", IiifValidationSeverity.Warning,
                    $"Behavior '{behavior.Value}' is not listed as valid for this resource type in the Presentation API 3.0 behavior table.", $"{path}.behavior"));

        if (node.RequiredStatement is not null)
        {
            if (node.RequiredStatement.Label.Count == 0)
                errors.Add(new IiifValidationError("requiredstatement-label-required", IiifValidationSeverity.Error, "requiredStatement.label must be non-empty when requiredStatement is present.",
                    $"{path}.requiredStatement.label"));

            if (node.RequiredStatement.Value.Count == 0)
                errors.Add(new IiifValidationError("requiredstatement-value-required", IiifValidationSeverity.Error, "requiredStatement.value must be non-empty when requiredStatement is present.",
                    $"{path}.requiredStatement.value"));
        }

        if (options.Strict && node.Rights is not null && !Uri.IsWellFormedUriString(node.Rights.Value, UriKind.Absolute))
            errors.Add(new IiifValidationError("rights-should-be-uri", IiifValidationSeverity.Warning, "rights should be a well-formed absolute URI.", $"{path}.rights"));
    }

    private static void ValidateManifestDowngradeLimitations(Manifest manifest, List<IiifValidationError> errors)
    {
        // Mirrors docs/README.md's "Downgrade limitations" table (SDK_VERSIONING_GUIDE.md Round 10):
        // these 3.0-only properties have no 2.x equivalent shape at all and are silently omitted by
        // IiifSerializer's legacy writer - surfacing that here, before the caller writes/publishes
        // legacy output, is the whole point of a version-aware validator.
        if (manifest.PlaceholderCanvas is not null)
            errors.Add(new IiifValidationError("v2-downgrade-loses-placeholdercanvas", IiifValidationSeverity.Warning,
                "placeholderCanvas has no Presentation 2.x equivalent and will be omitted when writing this version.", "$.placeholderCanvas"));

        if (manifest.Start is not null)
            errors.Add(new IiifValidationError("v2-downgrade-loses-start", IiifValidationSeverity.Warning, "start has no Presentation 2.x equivalent and will be omitted when writing this version.",
                "$.start"));

        if (manifest.Services.Count > 0)
            errors.Add(new IiifValidationError("v2-downgrade-loses-top-level-services", IiifValidationSeverity.Warning,
                "Top-level services (the centralized 3.0-only services array) has no Presentation 2.x equivalent and will be omitted when writing this version.", "$.services"));
    }
}
