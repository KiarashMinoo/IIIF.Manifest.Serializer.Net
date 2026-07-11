using System.Linq;
using IIIF.Manifests.Serializer.Properties;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer;

/// <summary>
/// Read/write for the two small "image-shaped" descriptive resources (<see cref="Thumbnail"/>,
/// <see cref="Logo"/>) - both are id/format/height/width plus an optional embedded Image API
/// service, just attached to different parent properties (<c>thumbnail</c> on any BaseNode,
/// <c>logo</c> only on <see cref="Provider"/>).
/// </summary>
public static partial class IiifSerializer
{
    private static JObject WriteV3Thumbnail(Thumbnail thumbnail)
    {
        var obj = new JObject { ["id"] = thumbnail.Id, ["type"] = "Image" };
        if (thumbnail.Format is not null)
        {
            obj["format"] = thumbnail.Format;
        }

        if (thumbnail.Height is not null)
        {
            obj["height"] = thumbnail.Height.Value;
        }

        if (thumbnail.Width is not null)
        {
            obj["width"] = thumbnail.Width.Value;
        }

        var services = thumbnail.Service.Select(WriteV3EmbeddedResourceService).ToList();
        if (services.Count > 0)
        {
            obj["service"] = new JArray(services);
        }

        return obj;
    }

    private static Thumbnail ReadV3Thumbnail(JObject obj)
    {
        var thumbnail = new Thumbnail(ReadRequiredString(obj, "id"));
        if ((string?)obj["format"] is { } format)
        {
            thumbnail.SetFormat(format);
        }

        if ((int?)obj["height"] is { } height)
        {
            thumbnail.SetHeight(height);
        }

        if ((int?)obj["width"] is { } width)
        {
            thumbnail.SetWidth(width);
        }

        foreach (var serviceObj in obj["service"]?.OfType<JObject>() ?? Enumerable.Empty<JObject>())
        {
            if (ReadV3Service(serviceObj) is { } service)
            {
                thumbnail.AddService(service);
            }
        }

        return thumbnail;
    }

    private static JObject WriteV3Logo(Logo logo)
    {
        var obj = new JObject { ["id"] = logo.Id, ["type"] = "Image" };
        if (logo.Format is not null)
        {
            obj["format"] = logo.Format;
        }

        if (logo.Height is not null)
        {
            obj["height"] = logo.Height.Value;
        }

        if (logo.Width is not null)
        {
            obj["width"] = logo.Width.Value;
        }

        var services = logo.Service.Select(WriteV3EmbeddedResourceService).ToList();
        if (services.Count > 0)
        {
            obj["service"] = new JArray(services);
        }

        return obj;
    }

    private static Logo ReadV3Logo(JObject obj)
    {
        var logo = new Logo(ReadRequiredString(obj, "id"));
        if ((string?)obj["format"] is { } format)
        {
            logo.SetFormat(format);
        }

        if ((int?)obj["height"] is { } height)
        {
            logo.SetHeight(height);
        }

        if ((int?)obj["width"] is { } width)
        {
            logo.SetWidth(width);
        }

        foreach (var serviceObj in obj["service"]?.OfType<JObject>() ?? Enumerable.Empty<JObject>())
        {
            if (ReadV3Service(serviceObj) is { } service)
            {
                logo.AddService(service);
            }
        }

        return logo;
    }
}
