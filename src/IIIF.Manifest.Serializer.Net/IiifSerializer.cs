using System.Linq;
using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Nodes.Contents.Audio;
using IIIF.Manifests.Serializer.Nodes.Contents.Audio.Resource;
using IIIF.Manifests.Serializer.Nodes.Contents.Image;
using IIIF.Manifests.Serializer.Nodes.Contents.Image.Resource;
using IIIF.Manifests.Serializer.Nodes.Contents.Video;
using IIIF.Manifests.Serializer.Nodes.Contents.Video.Resource;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.Content.Resources;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        return obj;
    }

    private static IEnumerable<Canvas> GetManifestCanvases(Manifest manifest)
    {
        if (manifest.Items.OfType<Canvas>().Any())
        {
            return manifest.Items.OfType<Canvas>();
        }

        return manifest.Sequences.SelectMany(x => x.Canvases);
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

        var annotations = canvas.Images.Select(x => WriteV3Annotation(x.Id, x.Motivation, x.Resource, x.On))
            .Concat(canvas.Audios.Select(x => WriteV3Annotation(x.Id, x.Motivation, x.Resource, x.On)))
            .Concat(canvas.Videos.Select(x => WriteV3Annotation(x.Id, x.Motivation, x.Resource, x.On)))
            .ToList();

        if (annotations.Count > 0)
        {
            obj["items"] = new JArray
            {
                new JObject
                {
                    ["id"] = $"{canvas.Id}/page",
                    ["type"] = "AnnotationPage",
                    ["items"] = new JArray(annotations)
                }
            };
        }

        return obj;
    }

    private static JObject WriteV3Annotation(string id, string motivation, IBaseResource resource, string target)
    {
        return new JObject
        {
            ["id"] = id,
            ["type"] = "Annotation",
            ["motivation"] = NormalizeMotivation(motivation),
            ["body"] = WriteV3Resource(resource),
            ["target"] = target
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

        var sequence = new Sequence($"{manifest.Id}/sequence/normal");
        foreach (var canvasObj in obj["items"]?.OfType<JObject>() ?? Enumerable.Empty<JObject>())
        {
            sequence.AddCanvas(ReadV3Canvas(canvasObj));
        }

        if (sequence.Canvases.Count > 0)
        {
            manifest.AddSequence(sequence);
        }

        return manifest;
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
            ReadV3Annotation(canvas, annotationObj);
        }

        return canvas;
    }

    private static void ReadV3Annotation(Canvas canvas, JObject obj)
    {
        if (obj["body"] is not JObject body)
        {
            return;
        }

        var annotationId = ReadRequiredString(obj, "id");
        var target = (string?)obj["target"] ?? canvas.Id;
        var format = (string?)body["format"] ?? string.Empty;
        var bodyId = ReadRequiredString(body, "id");

        switch ((string?)body["type"])
        {
            case "Image":
                var image = new Image(annotationId, new ImageResource(bodyId, format).SetHeight((int?)body["height"] ?? canvas.Height ?? 1).SetWidth((int?)body["width"] ?? canvas.Width ?? 1), target);
                canvas.AddImage(image);
                break;
            case "Sound":
                var audioResource = new AudioResource(bodyId, format);
                if ((double?)body["duration"] is { } audioDuration)
                {
                    audioResource.SetDuration(audioDuration);
                }
                canvas.AddAudio(new Audio(annotationId, audioResource, target));
                break;
            case "Video":
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
                canvas.AddVideo(new Video(annotationId, videoResource, target));
                break;
        }
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


