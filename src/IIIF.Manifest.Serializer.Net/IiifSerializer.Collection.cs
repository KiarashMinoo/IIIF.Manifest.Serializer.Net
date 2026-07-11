using System.Linq;
using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer;

public static partial class IiifSerializer
{
    private static JObject WriteV3Collection(Collection collection)
    {
        var obj = new JObject
        {
            ["@context"] = "http://iiif.io/api/presentation/3/context.json",
            ["id"] = collection.Id,
            ["type"] = "Collection"
        };

        obj["label"] = BuildLabelLanguageMapToken(collection.Label);

        WriteV3Behavior(collection, obj);

        if (collection.ViewingDirection is not null)
        {
            obj["viewingDirection"] = collection.ViewingDirection.Value;
        }

        var items = collection.Items.Select(WriteV3CollectionItem).ToList();
        if (items.Count > 0)
        {
            obj["items"] = new JArray(items);
        }

        WriteV3NodeExtras(collection, obj);
        WriteV3Provider(collection, obj);

        return obj;
    }

    private static JObject WriteV3CollectionItem(IBaseItem item)
    {
        var itemObj = new JObject
        {
            ["id"] = item.Id,
            ["type"] = item switch
            {
                Collection => "Collection",
                Manifest => "Manifest",
                _ => item.Type
            }
        };

        var label = item switch
        {
            Collection nested => nested.Label,
            Manifest manifest => manifest.Label,
            _ => []
        };

        if (label.Count > 0)
        {
            itemObj["label"] = BuildLabelLanguageMapToken(label);
        }

        return itemObj;
    }

    private static Collection ReadV3Collection(JObject obj)
    {
        var collection = new Collection(ReadRequiredString(obj, "id"), ReadLabels(obj["label"]).FirstOrDefault() ?? new Label("Untitled"));

        ReadV3Behavior(obj, collection);

        foreach (var itemObj in obj["items"]?.OfType<JObject>() ?? Enumerable.Empty<JObject>())
        {
            switch ((string?)itemObj["type"])
            {
                case "Collection":
                    collection.AddItem(new Collection(ReadRequiredString(itemObj, "id"), ReadLabels(itemObj["label"]).FirstOrDefault() ?? new Label("Untitled")));
                    break;
                case "Manifest":
                    var manifestStub = new Manifest(ReadRequiredString(itemObj, "id"));
                    foreach (var label in ReadLabels(itemObj["label"]))
                    {
                        manifestStub.AddLabel(label);
                    }
                    collection.AddItem(manifestStub);
                    break;
            }
        }

        ReadV3NodeExtras(obj, collection);
        ReadV3Provider(obj, collection);

        return collection;
    }
}
