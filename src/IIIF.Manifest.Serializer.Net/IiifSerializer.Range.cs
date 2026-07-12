using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer;

public static partial class IiifSerializer
{
    private static JObject WriteV3Range(Structure structure)
    {
        var obj = new JObject
        {
            ["id"] = structure.Id,
            ["type"] = "Range"
        };

        obj["label"] = BuildLabelLanguageMapToken(structure.Label);

        WriteV3Behavior(structure, obj);

        var items = structure.Items.Select(WriteV3RangeItem).ToList();
        if (items.Count > 0) obj["items"] = new JArray(items);

        WriteV3NodeExtras(structure, obj);

        return obj;
    }

    private static JObject WriteV3RangeItem(IBaseItem item)
    {
        return item switch
        {
            Structure nested => WriteV3Range(nested),
            _ => new JObject { ["id"] = item.Id, ["type"] = item.Type }
        };
    }

    private static Structure ReadV3Range(JObject obj)
    {
        var structure = new Structure(ReadRequiredString(obj, "id"), ReadLabels(obj["label"]).FirstOrDefault() ?? new Label("Untitled"));

        ReadV3Behavior(obj, structure);

        foreach (var itemObj in obj["items"]?.OfType<JObject>() ?? Enumerable.Empty<JObject>())
            switch ((string?)itemObj["type"])
            {
                case "Range":
                    structure.AddItem(ReadV3Range(itemObj));
                    break;
                case "Canvas":
                    structure.AddItem(new CanvasReference(ReadRequiredString(itemObj, "id")));
                    break;
            }

        ReadV3NodeExtras(obj, structure);

        return structure;
    }
}