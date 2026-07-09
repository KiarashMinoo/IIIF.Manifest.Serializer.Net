using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer;

/// <summary>
/// Detects IIIF Presentation API version from JSON-LD context and resource shape.
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

        if (value.Contains("/presentation/3/"))
        {
            return IiifPresentationVersion.V3_0;
        }

        if (value.Contains("/presentation/2/"))
        {
            return IiifPresentationVersion.V2_1;
        }

        if (value.Contains("/presentation/2.0/"))
        {
            return IiifPresentationVersion.V2_0;
        }

        return IiifPresentationVersion.Unknown;
    }
}
