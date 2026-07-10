using System.Linq;
using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Nodes.Contents.Annotation;
using IIIF.Manifests.Serializer.Nodes.Contents.Audio;
using IIIF.Manifests.Serializer.Nodes.Contents.Audio.Resource;
using IIIF.Manifests.Serializer.Nodes.Contents.Image;
using IIIF.Manifests.Serializer.Nodes.Contents.Image.Resource;
using IIIF.Manifests.Serializer.Nodes.Contents.Video;
using IIIF.Manifests.Serializer.Nodes.Contents.Video.Resource;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.Content.Resources;
using IIIF.Manifests.Serializer.Shared.Service;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AnnotationNode = IIIF.Manifests.Serializer.Nodes.Contents.Annotation.Annotation;

namespace IIIF.Manifests.Serializer;

/// <summary>
/// Version-aware SDK entry point for IIIF Presentation manifests.
/// </summary>
public static class IiifSerializer
{
    public static string Serialize(Manifest manifest)
    {
        return Serialize(manifest, IiifSerializerOptions.Default);
    }

    public static string Serialize(Manifest manifest, IiifSerializerOptions? options)
    {
        if (manifest is null)
        {
            throw new ArgumentNullException(nameof(manifest));
        }

        options ??= IiifSerializerOptions.Default;

        return options.Version switch
        {
            IiifPresentationVersion.V2_0 or IiifPresentationVersion.V2_1 => JsonConvert.SerializeObject(manifest, TrackableObject.JsonSerializerSettings),
            IiifPresentationVersion.V3_0 => WriteV3Manifest(manifest).ToString(Formatting.Indented),
            _ => throw new NotSupportedException($"Unsupported IIIF Presentation API version: {options.Version}.")
        };
    }

    public static Manifest DeserializeManifest(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            throw new ArgumentException("JSON string cannot be null or whitespace.", nameof(json));
        }

        var version = IiifPresentationVersionDetector.Detect(json);
        return version switch
        {
            IiifPresentationVersion.V3_0 => ReadV3Manifest(JObject.Parse(json)),
            IiifPresentationVersion.V2_0 or IiifPresentationVersion.V2_1 => JsonConvert.DeserializeObject<Manifest>(json, TrackableObject.JsonSerializerSettings)
                ?? throw new JsonSerializationException("Could not deserialize IIIF manifest."),
            _ => throw new JsonSerializationException("Could not detect IIIF Presentation API version.")
        };
    }

    public static string Serialize(Collection collection)
    {
        return Serialize(collection, IiifSerializerOptions.Default);
    }

    public static string Serialize(Collection collection, IiifSerializerOptions? options)
    {
        if (collection is null)
        {
            throw new ArgumentNullException(nameof(collection));
        }

        options ??= IiifSerializerOptions.Default;

        return options.Version switch
        {
            IiifPresentationVersion.V2_0 or IiifPresentationVersion.V2_1 => JsonConvert.SerializeObject(collection, TrackableObject.JsonSerializerSettings),
            IiifPresentationVersion.V3_0 => WriteV3Collection(collection).ToString(Formatting.Indented),
            _ => throw new NotSupportedException($"Unsupported IIIF Presentation API version: {options.Version}.")
        };
    }

    public static Collection DeserializeCollection(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            throw new ArgumentException("JSON string cannot be null or whitespace.", nameof(json));
        }

        var version = IiifPresentationVersionDetector.Detect(json);
        return version switch
        {
            IiifPresentationVersion.V3_0 => ReadV3Collection(JObject.Parse(json)),
            IiifPresentationVersion.V2_0 or IiifPresentationVersion.V2_1 => JsonConvert.DeserializeObject<Collection>(json, TrackableObject.JsonSerializerSettings)
                ?? throw new JsonSerializationException("Could not deserialize IIIF collection."),
            _ => throw new JsonSerializationException("Could not detect IIIF Presentation API version.")
        };
    }

    private static JObject WriteV3Collection(Collection collection)
    {
        var obj = new JObject
        {
            ["@context"] = "http://iiif.io/api/presentation/3/context.json",
            ["id"] = collection.Id,
            ["type"] = "Collection"
        };

        WriteLanguageMap(obj, "label", collection.Label.Select(x => x.Value));

        var behaviorValues = collection.Behavior.Select(x => x.Value).ToList();
#pragma warning disable CS0618
        if (behaviorValues.Count == 0 && collection.ViewingHint is not null)
        {
            behaviorValues.Add(collection.ViewingHint.Value);
        }
#pragma warning restore CS0618

        if (behaviorValues.Count > 0)
        {
            obj["behavior"] = new JArray(behaviorValues);
        }

        if (collection.ViewingDirection is not null)
        {
            obj["viewingDirection"] = collection.ViewingDirection.Value;
        }

        var items = collection.Items.Select(WriteV3CollectionItem).ToList();
        if (items.Count > 0)
        {
            obj["items"] = new JArray(items);
        }

        WriteV3RightsRequiredStatementPartOf(collection, obj);

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
            Collection nested => nested.Label.Select(x => x.Value),
            Manifest manifest => manifest.Label.Select(x => x.Value),
            _ => Enumerable.Empty<string>()
        };

        WriteLanguageMap(itemObj, "label", label);

        return itemObj;
    }

    private static Collection ReadV3Collection(JObject obj)
    {
        var collection = new Collection(ReadRequiredString(obj, "id"), ReadLabels(obj["label"]).FirstOrDefault() ?? new Label("Untitled"));

        foreach (var behavior in ReadStringArray(obj["behavior"]))
        {
            collection.AddBehavior(new Behavior(behavior));
        }

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

        ReadV3RightsRequiredStatementPartOf(obj, collection);

        return collection;
    }

    private static JObject WriteV3Manifest(Manifest manifest)
    {
        var obj = new JObject
        {
            ["@context"] = "http://iiif.io/api/presentation/3/context.json",
            ["id"] = manifest.Id,
            ["type"] = "Manifest"
        };

        WriteLanguageMap(obj, "label", manifest.Label.Select(x => x.Value));

        var behaviorValues = manifest.Behavior.Select(x => x.Value).ToList();
#pragma warning disable CS0618
        if (behaviorValues.Count == 0 && manifest.ViewingHint is not null)
        {
            behaviorValues.Add(manifest.ViewingHint.Value);
        }
#pragma warning restore CS0618

        if (behaviorValues.Count > 0)
        {
            obj["behavior"] = new JArray(behaviorValues);
        }

        if (manifest.ViewingDirection is not null)
        {
            obj["viewingDirection"] = manifest.ViewingDirection.Value;
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

        WriteV3RightsRequiredStatementPartOf(manifest, obj);

        return obj;
    }

    private static void WriteV3RightsRequiredStatementPartOf<TBaseNode>(BaseNode<TBaseNode> node, JObject obj) where TBaseNode : BaseNode<TBaseNode>
    {
        if (node.Rights is not null)
        {
            obj["rights"] = node.Rights.Value;
        }

        if (node.RequiredStatement is not null)
        {
            obj["requiredStatement"] = new JObject
            {
                ["label"] = BuildLanguageMapToken(node.RequiredStatement.Label.Select(x => x.Value)),
                ["value"] = BuildLanguageMapToken(node.RequiredStatement.Value.Select(x => x.Value))
            };
        }

        var partOf = node.PartOf.Select(x => new JObject { ["id"] = x.Id, ["type"] = x.Type }).ToList();
        if (partOf.Count > 0)
        {
            obj["partOf"] = new JArray(partOf);
        }
    }

    private static void ReadV3RightsRequiredStatementPartOf<TBaseNode>(JObject obj, BaseNode<TBaseNode> node) where TBaseNode : BaseNode<TBaseNode>
    {
        if ((string?)obj["rights"] is { } rights)
        {
            node.SetRights(new Rights(rights));
        }

        if (obj["requiredStatement"] is JObject requiredStatementObj)
        {
            var label = ReadLabels(requiredStatementObj["label"]);
            var value = ReadLabels(requiredStatementObj["value"]).Select(x => new Description(x.Value)).ToList();
            node.SetRequiredStatement(new RequiredStatement(label, value));
        }

        foreach (var partOfObj in obj["partOf"]?.OfType<JObject>() ?? Enumerable.Empty<JObject>())
        {
            node.AddPartOf(new PartOf(ReadRequiredString(partOfObj, "id"), (string?)partOfObj["type"] ?? "Manifest"));
        }
    }

    private static JToken BuildLanguageMapToken(IEnumerable<string> values)
    {
        var list = values.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
        return new JObject { ["none"] = new JArray(list) };
    }

    private static JObject WriteV3Service(IBaseService service)
    {
        var token = JObject.FromObject(service, JsonSerializer.Create(TrackableObject.JsonSerializerSettings));
        Rename(token, "@id", "id");
        Rename(token, "@context", "context");
        token.Remove("@type");
        return token;
    }

    private static IEnumerable<Canvas> GetManifestCanvases(Manifest manifest) => manifest.Items.OfType<Canvas>();

    private static JObject WriteV3Range(Structure structure)
    {
        var obj = new JObject
        {
            ["id"] = structure.Id,
            ["type"] = "Range"
        };

        WriteLanguageMap(obj, "label", structure.Label.Select(x => x.Value));

        var items = structure.Items.Select(WriteV3RangeItem).ToList();
        if (items.Count > 0)
        {
            obj["items"] = new JArray(items);
        }

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

    private static JObject WriteV3Canvas(Canvas canvas)
    {
        var obj = new JObject
        {
            ["id"] = canvas.Id,
            ["type"] = "Canvas"
        };

        WriteLanguageMap(obj, "label", canvas.Label.Select(x => x.Value));

        if (canvas.Height is not null)
        {
            obj["height"] = canvas.Height.Value;
        }

        if (canvas.Width is not null)
        {
            obj["width"] = canvas.Width.Value;
        }

        if (canvas.Duration is not null)
        {
            obj["duration"] = canvas.Duration.Value;
        }

        var pages = canvas.Items.OfType<AnnotationPage>().Select(WriteV3AnnotationPage).ToList();
        if (pages.Count > 0)
        {
            obj["items"] = new JArray(pages);
        }

        var annotationRefs = canvas.Annotations.Select(x => new JObject { ["id"] = x.Id, ["type"] = "AnnotationPage" }).ToList();
        if (annotationRefs.Count > 0)
        {
            obj["annotations"] = new JArray(annotationRefs);
        }

        return obj;
    }

    private static JObject WriteV3AnnotationPage(AnnotationPage page)
    {
        return new JObject
        {
            ["id"] = page.Id,
            ["type"] = "AnnotationPage",
            ["items"] = new JArray(page.Items.OfType<AnnotationNode>().Select(WriteV3Annotation))
        };
    }

    private static JObject WriteV3Annotation(AnnotationNode annotation)
    {
        return new JObject
        {
            ["id"] = annotation.Id,
            ["type"] = "Annotation",
            ["motivation"] = NormalizeMotivation(annotation.Motivation),
            ["body"] = WriteV3Resource(annotation.Body),
            ["target"] = annotation.Target
        };
    }

    private static JObject WriteV3Resource(IBaseResource resource)
    {
        var token = JObject.FromObject(resource, JsonSerializer.Create(TrackableObject.JsonSerializerSettings));
        Rename(token, "@id", "id");
        token.Remove("@context");
        token.Remove("@type");
        token["type"] = resource.Type?.Value switch
        {
            "dctypes:Image" => "Image",
            "dctypes:Sound" => "Sound",
            "dctypes:MovingImage" => "Video",
            var type when !string.IsNullOrWhiteSpace(type) => type,
            _ => "ContentResource"
        };

        return token;
    }

    private static Manifest ReadV3Manifest(JObject obj)
    {
        var manifest = new Manifest(ReadRequiredString(obj, "id"), ReadLabels(obj["label"]).FirstOrDefault() ?? new Label("Untitled"));

        foreach (var behavior in ReadStringArray(obj["behavior"]))
        {
            manifest.AddBehavior(new Behavior(behavior));
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

        ReadV3RightsRequiredStatementPartOf(obj, manifest);

        return manifest;
    }

    private static IBaseService? ReadV3Service(JObject obj)
    {
        var normalized = (JObject)obj.DeepClone();
        Rename(normalized, "id", "@id");
        Rename(normalized, "context", "@context");
        Rename(normalized, "type", "@type");
        return normalized.ToObject<IBaseService>();
    }

    private static Structure ReadV3Range(JObject obj)
    {
        var structure = new Structure(ReadRequiredString(obj, "id"), ReadLabels(obj["label"]).FirstOrDefault() ?? new Label("Untitled"));

        foreach (var itemObj in obj["items"]?.OfType<JObject>() ?? Enumerable.Empty<JObject>())
        {
            switch ((string?)itemObj["type"])
            {
                case "Range":
                    structure.AddItem(ReadV3Range(itemObj));
                    break;
                case "Canvas":
                    structure.AddItem(new CanvasReference(ReadRequiredString(itemObj, "id")));
                    break;
            }
        }

        return structure;
    }

    private static Canvas ReadV3Canvas(JObject obj)
    {
        var height = (int?)obj["height"] ?? 1;
        var width = (int?)obj["width"] ?? 1;
        var canvas = new Canvas(ReadRequiredString(obj, "id"), ReadLabels(obj["label"]).FirstOrDefault() ?? new Label("Untitled"), height, width);

        if ((double?)obj["duration"] is { } duration)
        {
            canvas.SetDuration(duration);
        }

        foreach (var annotationObj in obj["items"]?.OfType<JObject>().SelectMany(x => x["items"]?.OfType<JObject>() ?? Enumerable.Empty<JObject>()) ?? Enumerable.Empty<JObject>())
        {
            if (ReadV3Annotation(canvas, annotationObj) is { } annotation)
            {
                canvas.AddAnnotation(annotation);
            }
        }

        foreach (var annotationsRef in obj["annotations"]?.OfType<JObject>() ?? Enumerable.Empty<JObject>())
        {
            canvas.AddAnnotationPageReference(new AnnotationPage(ReadRequiredString(annotationsRef, "id")));
        }

        return canvas;
    }

    private static AnnotationNode? ReadV3Annotation(Canvas canvas, JObject obj)
    {
        if (obj["body"] is not JObject body)
        {
            return null;
        }

        var annotationId = ReadRequiredString(obj, "id");
        var target = (string?)obj["target"] ?? canvas.Id;
        var format = (string?)body["format"] ?? string.Empty;
        var bodyId = ReadRequiredString(body, "id");
        var motivation = (string?)obj["motivation"] ?? "painting";

        IBaseResource? resource = (string?)body["type"] switch
        {
            "Image" => new ImageResource(bodyId, format)
                .SetHeight((int?)body["height"] ?? canvas.Height ?? 1)
                .SetWidth((int?)body["width"] ?? canvas.Width ?? 1),
            "Sound" => (double?)body["duration"] is { } audioDuration
                ? new AudioResource(bodyId, format).SetDuration(audioDuration)
                : new AudioResource(bodyId, format),
            "Video" => BuildVideoResource(bodyId, format, body),
            _ => null
        };

        return resource is null ? null : new AnnotationNode(annotationId, resource, target).SetMotivation(motivation);
    }

    private static VideoResource BuildVideoResource(string bodyId, string format, JObject body)
    {
        var videoResource = new VideoResource(bodyId, format);
        if ((int?)body["height"] is { } videoHeight)
        {
            videoResource.SetHeight(videoHeight);
        }
        if ((int?)body["width"] is { } videoWidth)
        {
            videoResource.SetWidth(videoWidth);
        }
        if ((double?)body["duration"] is { } videoDuration)
        {
            videoResource.SetDuration(videoDuration);
        }
        return videoResource;
    }

    private static void WriteLanguageMap(JObject obj, string name, IEnumerable<string> values)
    {
        var array = new JArray(values.Where(x => !string.IsNullOrWhiteSpace(x)));
        if (array.Count > 0)
        {
            obj[name] = new JObject { ["none"] = array };
        }
    }

    private static IReadOnlyCollection<Label> ReadLabels(JToken? token)
    {
        if (token is JObject languageMap)
        {
            return languageMap.Properties()
                .SelectMany(x => x.Value.Type == JTokenType.Array ? x.Value.Values<string>() : [(string?)x.Value])
                .OfType<string>()
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => new Label(x))
                .ToList();
        }

        if (token is JArray array)
        {
            return array.Values<string>().OfType<string>().Select(x => new Label(x)).ToList();
        }

        var value = (string?)token;
        return string.IsNullOrWhiteSpace(value) ? [] : [new Label(value)];
    }

    private static IEnumerable<string> ReadStringArray(JToken? token)
    {
        return token switch
        {
            JArray array => array.Values<string>().OfType<string>().Where(x => !string.IsNullOrWhiteSpace(x)),
            _ when !string.IsNullOrWhiteSpace((string?)token) => [(string)token!],
            _ => []
        };
    }

    private static string ReadRequiredString(JObject obj, string name)
    {
        return (string?)obj[name] ?? throw new JsonSerializationException($"Required property '{name}' is missing.");
    }

    private static string NormalizeMotivation(string motivation)
    {
        return motivation.StartsWith("sc:", StringComparison.Ordinal) ? motivation.Substring(3) : motivation;
    }

    private static void Rename(JObject obj, string oldName, string newName)
    {
        if (obj[oldName] is not null)
        {
            obj[newName] = obj[oldName];
            obj.Remove(oldName);
        }
    }
}


