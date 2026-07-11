using System.Linq;
using IIIF.Manifests.Serializer.Properties;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer;

/// <summary>
/// Read/write for the three small "link-shaped" descriptive resources - <see cref="Rendering"/>,
/// <see cref="Homepage"/>, <see cref="SeeAlso"/> - each an id/label/format, with SeeAlso alone
/// adding a profile and a settable type (recipes 0053/0068/0234 use "Dataset").
/// </summary>
public static partial class IiifSerializer
{
    private static JObject WriteV3Rendering(Rendering rendering)
    {
        var obj = new JObject { ["id"] = rendering.Id, ["type"] = "Text" };
        WriteLanguageMap(obj, "label", [rendering.Label]);
        if (rendering.Format is not null)
        {
            obj["format"] = rendering.Format;
        }

        return obj;
    }

    private static Rendering ReadV3Rendering(JObject obj)
    {
        var label = ReadLabels(obj["label"]).FirstOrDefault()?.Value ?? string.Empty;
        var rendering = new Rendering(ReadRequiredString(obj, "id"), label);
        if ((string?)obj["format"] is { } format)
        {
            rendering.SetFormat(format);
        }

        return rendering;
    }

    private static JObject WriteV3Homepage(Homepage homepage)
    {
        var obj = new JObject { ["id"] = homepage.Id, ["type"] = "Text" };
        if (homepage.Label is not null)
        {
            WriteLanguageMap(obj, "label", [homepage.Label]);
        }

        if (homepage.Format is not null)
        {
            obj["format"] = homepage.Format;
        }

        return obj;
    }

    private static Homepage ReadV3Homepage(JObject obj)
    {
        var id = ReadRequiredString(obj, "id");
        var label = ReadLabels(obj["label"]).FirstOrDefault()?.Value;
        var homepage = label is not null ? new Homepage(id, label) : new Homepage(id);
        if ((string?)obj["format"] is { } format)
        {
            homepage.SetFormat(format);
        }

        return homepage;
    }

    private static JObject WriteV3SeeAlso(SeeAlso seeAlso)
    {
        var obj = new JObject { ["id"] = seeAlso.Id, ["type"] = seeAlso.Type ?? "Dataset" };
        if (seeAlso.Label is not null)
        {
            WriteLanguageMap(obj, "label", [seeAlso.Label]);
        }

        if (seeAlso.Format is not null)
        {
            obj["format"] = seeAlso.Format;
        }

        if (seeAlso.Profile is not null)
        {
            obj["profile"] = seeAlso.Profile;
        }

        return obj;
    }

    private static SeeAlso ReadV3SeeAlso(JObject obj)
    {
        var seeAlso = new SeeAlso(ReadRequiredString(obj, "id"));
        if ((string?)obj["type"] is { } type)
        {
            seeAlso.SetType(type);
        }

        if ((string?)obj["format"] is { } format)
        {
            seeAlso.SetFormat(format);
        }

        if ((string?)obj["profile"] is { } profile)
        {
            seeAlso.SetProfile(profile);
        }

        if (ReadLabels(obj["label"]).FirstOrDefault()?.Value is { } label)
        {
            seeAlso.SetLabel(label);
        }

        return seeAlso;
    }
}
