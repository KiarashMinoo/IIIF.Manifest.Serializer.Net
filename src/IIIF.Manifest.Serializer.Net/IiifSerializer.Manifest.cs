using System.Linq;
using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Nodes.Contents.Annotation;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer;

public static partial class IiifSerializer
{
    private static JObject WriteV3Manifest(Manifest manifest)
    {
        var obj = new JObject
        {
            ["@context"] = "http://iiif.io/api/presentation/3/context.json",
            ["id"] = manifest.Id,
            ["type"] = "Manifest"
        };

        obj["label"] = BuildLabelLanguageMapToken(manifest.Label);

        WriteV3Behavior(manifest, obj);

        if (manifest.ViewingDirection is not null)
        {
            obj["viewingDirection"] = manifest.ViewingDirection.Value;
        }

        if (manifest.Start is not null)
        {
            obj["start"] = JToken.FromObject(manifest.Start, JsonSerializer.Create(TrackableObject.JsonSerializerSettings));
        }

        if (manifest.PlaceholderCanvas is not null)
        {
            obj["placeholderCanvas"] = WriteV3Canvas(manifest.PlaceholderCanvas);
        }

        var canvases = GetManifestCanvases(manifest).Select(WriteV3Canvas).ToList();
        if (canvases.Count > 0)
        {
            obj["items"] = new JArray(canvases);
        }

        var structures = manifest.Structures.Select(WriteV3Range).ToList();
        if (structures.Count > 0)
        {
            obj["structures"] = new JArray(structures);
        }

        var services = manifest.Services.Select(WriteV3Service).ToList();
        if (services.Count > 0)
        {
            obj["services"] = new JArray(services);
        }

        WriteV3NodeExtras(manifest, obj);
        WriteV3Provider(manifest, obj);

        return obj;
    }

    private static Manifest ReadV3Manifest(JObject obj)
    {
        var manifest = new Manifest(ReadRequiredString(obj, "id"), ReadLabels(obj["label"]).FirstOrDefault() ?? new Label("Untitled"));

        ReadV3Behavior(obj, manifest);

        if (obj["start"] is { } startToken)
        {
            manifest.SetStart(startToken.ToObject<AnnotationTarget>()!);
        }

        if (obj["placeholderCanvas"] is JObject placeholderObj)
        {
            manifest.SetPlaceholderCanvas(ReadV3Canvas(placeholderObj));
        }

        foreach (var canvasObj in obj["items"]?.OfType<JObject>() ?? Enumerable.Empty<JObject>())
        {
            manifest.AddItem(ReadV3Canvas(canvasObj));
        }

        foreach (var structureObj in obj["structures"]?.OfType<JObject>() ?? Enumerable.Empty<JObject>())
        {
            manifest.AddStructure(ReadV3Range(structureObj));
        }

        foreach (var serviceObj in obj["services"]?.OfType<JObject>() ?? Enumerable.Empty<JObject>())
        {
            if (ReadV3Service(serviceObj) is { } service)
            {
                manifest.AddTopLevelService(service);
            }
        }

        ReadV3NodeExtras(obj, manifest);
        ReadV3Provider(obj, manifest);

        return manifest;
    }

    private static IEnumerable<Canvas> GetManifestCanvases(Manifest manifest) => manifest.Items.OfType<Canvas>();
}
