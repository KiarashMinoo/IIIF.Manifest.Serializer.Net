using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer;

/// <summary>
/// Detects the IIIF Presentation-family API version of a JSON document from its `@context`
/// (checked first, and wins outright if recognized) and, failing that, from structural cues
/// (`items` vs `sequences`, `id`/`type` vs `@id`/`@type`). Deterministic for ambiguous, invalid,
/// non-object, and empty input - see the priority order documented on <see cref="Detect(JToken)"/>.
/// </summary>
public static class IiifPresentationVersionDetector
{
    public static IiifPresentationVersion Detect(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return IiifPresentationVersion.Unknown;
        }

        try
        {
            return Detect(JToken.Parse(json));
        }
        catch
        {
            return IiifPresentationVersion.Unknown;
        }
    }

    /// <summary>
    /// Priority order when signals conflict (e.g. a document carrying both `items` and
    /// `sequences`): (1) `@context`; (2) legacy 2.x signals (`@id`/`@type`/`sequences`); (3) 3.0
    /// signals (`id`/`type`/`items`); (4) <see cref="IiifPresentationVersion.Unknown"/>. Non-object
    /// JSON (arrays, bare literals, `null`) always returns
    /// <see cref="IiifPresentationVersion.Unknown"/>, regardless of the other rules.
    /// </summary>
    public static IiifPresentationVersion Detect(JToken token)
    {
        if (token is not JObject obj)
        {
            return IiifPresentationVersion.Unknown;
        }

        var contextVersion = DetectFromContext(obj["@context"]);
        if (contextVersion != IiifPresentationVersion.Unknown)
        {
            return contextVersion;
        }

        var type = (string?)obj["type"] ?? (string?)obj["@type"];
        if (!string.IsNullOrWhiteSpace(type))
        {
            if (type is "Manifest" or "Collection" or "Canvas" or "AnnotationPage" or "Annotation")
            {
                return IiifPresentationVersion.V3_0;
            }

            if (type.StartsWith("sc:") || type.StartsWith("oa:") || type.StartsWith("dctypes:"))
            {
                return IiifPresentationVersion.V2_1;
            }
        }

        if (obj["sequences"] is not null || obj["@id"] is not null || obj["@type"] is not null)
        {
            return IiifPresentationVersion.V2_1;
        }

        if (obj["items"] is not null || obj["id"] is not null || obj["type"] is not null)
        {
            return IiifPresentationVersion.V3_0;
        }

        return IiifPresentationVersion.Unknown;
    }

    private static IiifPresentationVersion DetectFromContext(JToken? context)
    {
        if (context is null)
        {
            return IiifPresentationVersion.Unknown;
        }

        if (context.Type == JTokenType.Array)
        {
            foreach (var item in context.Children())
            {
                var version = DetectFromContext(item);
                if (version != IiifPresentationVersion.Unknown)
                {
                    return version;
                }
            }

            return IiifPresentationVersion.Unknown;
        }

        var value = (string?)context;
        if (string.IsNullOrWhiteSpace(value))
        {
            return IiifPresentationVersion.Unknown;
        }

        if (value.Contains("/presentation/4/"))
        {
            return IiifPresentationVersion.V4_0_Rc;
        }

        if (value.Contains("/presentation/3/"))
        {
            return IiifPresentationVersion.V3_0;
        }

        // Presentation 2.0 and 2.1 both use this exact context URL - confirmed against the live
        // spec (iiif.io/api/presentation/2.1/ states its @context is
        // "http://iiif.io/api/presentation/2/context.json", identical to 2.0's, with no version
        // field or structural difference to tell them apart). Real-world 2.x documents are always
        // detected as V2_1 by context, never V2_0.
        if (value.Contains("/presentation/2/"))
        {
            return IiifPresentationVersion.V2_1;
        }

        // Not a real published resource (2.0 never had its own distinct context.json) - kept as a
        // defensive fallback for non-conformant tooling that might emit a URL shaped like this, and
        // as the only way Detect() can ever actually return V2_0 rather than V2_1.
        if (value.Contains("/presentation/2.0/"))
        {
            return IiifPresentationVersion.V2_0;
        }

        // IIIF Metadata API 1.0 ("Shared Canvas"), the predecessor to Presentation 2.0.
        // Structurally near-identical to Presentation 2.x - only reliably distinguished by this
        // context URL.
        if (value.Contains("shared-canvas.org"))
        {
            return IiifPresentationVersion.Metadata_1_0;
        }

        return IiifPresentationVersion.Unknown;
    }
}
