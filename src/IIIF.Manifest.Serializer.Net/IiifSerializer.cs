using System.Linq;
using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Nodes.Contents.Annotation;
using IIIF.Manifests.Serializer.Nodes.Contents.Audio;
using IIIF.Manifests.Serializer.Nodes.Contents.Audio.Resource;
using IIIF.Manifests.Serializer.Nodes.Contents.Choice;
using IIIF.Manifests.Serializer.Nodes.Contents.Image;
using IIIF.Manifests.Serializer.Nodes.Contents.Image.Resource;
using IIIF.Manifests.Serializer.Nodes.Contents.Textual.Resource;
using IIIF.Manifests.Serializer.Nodes.Contents.Video;
using IIIF.Manifests.Serializer.Nodes.Contents.Video.Resource;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Properties.MetadataProperty;
using IIIF.Manifests.Serializer.Properties.MetadataProperty.MetadataValue;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.Content.Resources;
using IIIF.Manifests.Serializer.Shared.Selectors;
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

    public static string Serialize(AnnotationCollection annotationCollection)
    {
        return Serialize(annotationCollection, IiifSerializerOptions.Default);
    }

    public static string Serialize(AnnotationCollection annotationCollection, IiifSerializerOptions? options)
    {
        if (annotationCollection is null)
        {
            throw new ArgumentNullException(nameof(annotationCollection));
        }

        options ??= IiifSerializerOptions.Default;

        return options.Version switch
        {
            IiifPresentationVersion.V3_0 => WriteV3AnnotationCollection(annotationCollection).ToString(Formatting.Indented),
            _ => throw new NotSupportedException($"Unsupported IIIF Presentation API version: {options.Version}. AnnotationCollection is a 3.0-only concept.")
        };
    }

    public static AnnotationCollection DeserializeAnnotationCollection(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            throw new ArgumentException("JSON string cannot be null or whitespace.", nameof(json));
        }

        return ReadV3AnnotationCollection(JObject.Parse(json));
    }

    private static JObject WriteV3AnnotationCollection(AnnotationCollection annotationCollection)
    {
        var obj = new JObject
        {
            ["@context"] = "http://iiif.io/api/presentation/3/context.json",
            ["id"] = annotationCollection.Id,
            ["type"] = "AnnotationCollection"
        };

        obj["label"] = BuildLabelLanguageMapToken(annotationCollection.Label);

        if (annotationCollection.Total is not null)
        {
            obj["total"] = annotationCollection.Total.Value;
        }

        if (annotationCollection.First is not null)
        {
            obj["first"] = annotationCollection.First;
        }

        if (annotationCollection.Last is not null)
        {
            obj["last"] = annotationCollection.Last;
        }

        return obj;
    }

    private static AnnotationCollection ReadV3AnnotationCollection(JObject obj)
    {
        var annotationCollection = new AnnotationCollection(ReadRequiredString(obj, "id"), ReadLabels(obj["label"]).FirstOrDefault() ?? new Label("Untitled"));

        if ((int?)obj["total"] is { } total)
        {
            annotationCollection.SetTotal(total);
        }

        if ((string?)obj["first"] is { } first)
        {
            annotationCollection.SetFirst(first);
        }

        if ((string?)obj["last"] is { } last)
        {
            annotationCollection.SetLast(last);
        }

        return annotationCollection;
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

    /// <summary>
    /// Writes every BaseNode-generic descriptive/linking property valid on Manifest, Collection,
    /// Canvas, and Range alike: rights, requiredStatement, partOf, summary, metadata, thumbnail,
    /// rendering, homepage, seeAlso. <see cref="Provider"/> is deliberately excluded - per spec it's
    /// only valid on Manifest/Collection, so it's written separately by <see cref="WriteV3Provider"/>.
    /// </summary>
    private static void WriteV3NodeExtras<TBaseNode>(BaseNode<TBaseNode> node, JObject obj) where TBaseNode : BaseNode<TBaseNode>
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

        if (node.Summary.Count > 0)
        {
            obj["summary"] = BuildDescriptionLanguageMapToken(node.Summary);
        }

        var metadata = node.Metadata.Select(WriteV3Metadata).ToList();
        if (metadata.Count > 0)
        {
            obj["metadata"] = new JArray(metadata);
        }

        if (node.Thumbnail is not null)
        {
            obj["thumbnail"] = new JArray(WriteV3Thumbnail(node.Thumbnail));
        }

        var rendering = node.Rendering.Select(WriteV3Rendering).ToList();
        if (rendering.Count > 0)
        {
            obj["rendering"] = new JArray(rendering);
        }

        var homepage = node.Homepage.Select(WriteV3Homepage).ToList();
        if (homepage.Count > 0)
        {
            obj["homepage"] = new JArray(homepage);
        }

        var seeAlso = node.SeeAlso.Select(WriteV3SeeAlso).ToList();
        if (seeAlso.Count > 0)
        {
            obj["seeAlso"] = new JArray(seeAlso);
        }
    }

    private static void ReadV3NodeExtras<TBaseNode>(JObject obj, BaseNode<TBaseNode> node) where TBaseNode : BaseNode<TBaseNode>
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

        if (obj["summary"] is { } summaryToken)
        {
            node.SetSummary(ReadDescriptions(summaryToken));
        }

        foreach (var metadataObj in obj["metadata"]?.OfType<JObject>() ?? Enumerable.Empty<JObject>())
        {
            node.AddMetadata(ReadV3Metadata(metadataObj));
        }

        if (obj["thumbnail"]?.OfType<JObject>().FirstOrDefault() is { } thumbnailObj)
        {
            node.SetThumbnail(ReadV3Thumbnail(thumbnailObj));
        }

        foreach (var renderingObj in obj["rendering"]?.OfType<JObject>() ?? Enumerable.Empty<JObject>())
        {
            node.AddRendering(ReadV3Rendering(renderingObj));
        }

        foreach (var homepageObj in obj["homepage"]?.OfType<JObject>() ?? Enumerable.Empty<JObject>())
        {
            node.AddHomepage(ReadV3Homepage(homepageObj));
        }

        foreach (var seeAlsoObj in obj["seeAlso"]?.OfType<JObject>() ?? Enumerable.Empty<JObject>())
        {
            node.AddSeeAlso(ReadV3SeeAlso(seeAlsoObj));
        }
    }

    private static JObject WriteV3Metadata(Metadata metadata)
    {
        return new JObject
        {
            ["label"] = BuildLanguageMapToken([metadata.Label]),
            ["value"] = BuildLanguageMapValueToken(metadata.Value)
        };
    }

    private static JToken BuildLanguageMapValueToken(IEnumerable<MetadataValue> values)
    {
        var map = new JObject();
        foreach (var group in values.GroupBy(x => x.Language ?? "none"))
        {
            map[group.Key] = new JArray(group.Select(x => x.Value));
        }

        return map;
    }

    private static JToken BuildDescriptionLanguageMapToken(IEnumerable<Description> descriptions)
    {
        var map = new JObject();
        foreach (var group in descriptions.GroupBy(x => x.Language ?? "none"))
        {
            map[group.Key] = new JArray(group.Select(x => x.Value));
        }

        return map;
    }

    private static JToken BuildLabelLanguageMapToken(IEnumerable<Label> labels)
    {
        var map = new JObject();
        foreach (var group in labels.GroupBy(x => x.Language ?? "none"))
        {
            map[group.Key] = new JArray(group.Select(x => x.Value));
        }

        return map;
    }

    private static List<Description> ReadDescriptions(JToken? token)
    {
        if (token is JObject languageMap)
        {
            return languageMap.Properties()
                .SelectMany(prop => (prop.Value.Type == JTokenType.Array ? prop.Value.Values<string>() : [(string?)prop.Value])
                    .OfType<string>()
                    .Select(value => prop.Name == "none" ? new Description(value) : new Description(value).SetLanguage(prop.Name)))
                .ToList();
        }

        if (token is JArray array)
        {
            return array.Values<string>().OfType<string>().Select(x => new Description(x)).ToList();
        }

        var stringValue = (string?)token;
        return string.IsNullOrWhiteSpace(stringValue) ? [] : [new Description(stringValue)];
    }

    private static Metadata ReadV3Metadata(JObject obj)
    {
        var label = ReadLabels(obj["label"]).FirstOrDefault()?.Value ?? string.Empty;
        var metadata = new Metadata(label, string.Empty);
        var values = ReadMetadataValues(obj["value"]);
        if (values.Count > 0)
        {
            metadata.ResetValue(values[0]);
            foreach (var value in values.Skip(1))
            {
                metadata.AddValue(value);
            }
        }

        return metadata;
    }

    private static List<MetadataValue> ReadMetadataValues(JToken? token)
    {
        if (token is JObject languageMap)
        {
            return languageMap.Properties()
                .SelectMany(prop => (prop.Value.Type == JTokenType.Array ? prop.Value.Values<string>() : [(string?)prop.Value])
                    .OfType<string>()
                    .Select(value => prop.Name == "none" ? new MetadataValue(value) : new MetadataValue(value, prop.Name)))
                .ToList();
        }

        var value = (string?)token;
        return string.IsNullOrWhiteSpace(value) ? [] : [new MetadataValue(value)];
    }

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

    /// <summary>
    /// Per spec, <c>provider</c> is only valid on Manifest and Collection (unlike the other
    /// BaseNode-generic extras in <see cref="WriteV3NodeExtras{TBaseNode}"/>), so it's written/read
    /// by its own explicit call sites rather than the shared Canvas/Range-inclusive helper.
    /// </summary>
    private static void WriteV3Provider<TBaseNode>(BaseNode<TBaseNode> node, JObject obj) where TBaseNode : BaseNode<TBaseNode>
    {
        var providers = node.Provider.Select(WriteV3ProviderEntry).ToList();
        if (providers.Count > 0)
        {
            obj["provider"] = new JArray(providers);
        }
    }

    private static void ReadV3Provider<TBaseNode>(JObject obj, BaseNode<TBaseNode> node) where TBaseNode : BaseNode<TBaseNode>
    {
        foreach (var providerObj in obj["provider"]?.OfType<JObject>() ?? Enumerable.Empty<JObject>())
        {
            node.AddProvider(ReadV3ProviderEntry(providerObj));
        }
    }

    private static JObject WriteV3ProviderEntry(Provider provider)
    {
        var obj = new JObject { ["id"] = provider.Id, ["type"] = "Agent" };
        obj["label"] = BuildLabelLanguageMapToken(provider.Label);

        var homepages = provider.Homepage.Select(WriteV3Homepage).ToList();
        if (homepages.Count > 0)
        {
            obj["homepage"] = new JArray(homepages);
        }

        var logos = provider.Logo.Select(WriteV3Logo).ToList();
        if (logos.Count > 0)
        {
            obj["logo"] = new JArray(logos);
        }

        var seeAlsos = provider.SeeAlso.Select(WriteV3SeeAlso).ToList();
        if (seeAlsos.Count > 0)
        {
            obj["seeAlso"] = new JArray(seeAlsos);
        }

        return obj;
    }

    private static Provider ReadV3ProviderEntry(JObject obj)
    {
        var provider = new Provider(ReadRequiredString(obj, "id"));
        var labels = ReadLabels(obj["label"]);
        if (labels.Count > 0)
        {
            provider.SetLabel(labels);
        }

        foreach (var homepageObj in obj["homepage"]?.OfType<JObject>() ?? Enumerable.Empty<JObject>())
        {
            provider.AddHomepage(ReadV3Homepage(homepageObj));
        }

        foreach (var logoObj in obj["logo"]?.OfType<JObject>() ?? Enumerable.Empty<JObject>())
        {
            provider.AddLogo(ReadV3Logo(logoObj));
        }

        foreach (var seeAlsoObj in obj["seeAlso"]?.OfType<JObject>() ?? Enumerable.Empty<JObject>())
        {
            provider.AddSeeAlso(ReadV3SeeAlso(seeAlsoObj));
        }

        return provider;
    }

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

    private static void WriteV3Behavior<TBaseNode>(BaseNode<TBaseNode> node, JObject obj) where TBaseNode : BaseNode<TBaseNode>
    {
        var behaviorValues = node.Behavior.Select(x => x.Value).ToList();
#pragma warning disable CS0618
        if (behaviorValues.Count == 0 && node.ViewingHint is not null)
        {
            behaviorValues.Add(node.ViewingHint.Value);
        }
#pragma warning restore CS0618

        if (behaviorValues.Count > 0)
        {
            obj["behavior"] = new JArray(behaviorValues);
        }
    }

    private static void ReadV3Behavior<TBaseNode>(JObject obj, BaseNode<TBaseNode> node) where TBaseNode : BaseNode<TBaseNode>
    {
        foreach (var behavior in ReadStringArray(obj["behavior"]))
        {
            node.AddBehavior(new Behavior(behavior));
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
        Rename(token, "@type", "type");
        return token;
    }

    /// <summary>
    /// Same as <see cref="WriteV3Service"/> but additionally strips "@context" - unlike a
    /// top-level Manifest.Services entry (which may legitimately declare its own context per
    /// Milestone 9), a service embedded inline on a content resource (e.g. an Image API service on
    /// a painting body) never carries one in any real cookbook recipe.
    /// </summary>
    private static JObject WriteV3EmbeddedResourceService(IBaseService service)
    {
        var token = WriteV3Service(service);
        token.Remove("@context");
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

        obj["label"] = BuildLabelLanguageMapToken(structure.Label);

        WriteV3Behavior(structure, obj);

        var items = structure.Items.Select(WriteV3RangeItem).ToList();
        if (items.Count > 0)
        {
            obj["items"] = new JArray(items);
        }

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

    private static JObject WriteV3Canvas(Canvas canvas)
    {
        var obj = new JObject
        {
            ["id"] = canvas.Id,
            ["type"] = "Canvas"
        };

        obj["label"] = BuildLabelLanguageMapToken(canvas.Label);

        WriteV3Behavior(canvas, obj);

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

        if (canvas.PlaceholderCanvas is not null)
        {
            obj["placeholderCanvas"] = WriteV3Canvas(canvas.PlaceholderCanvas);
        }

        var pages = canvas.Items.OfType<AnnotationPage>().Select(WriteV3AnnotationPage).ToList();
        if (pages.Count > 0)
        {
            obj["items"] = new JArray(pages);
        }

        var annotationRefs = canvas.Annotations.Select(WriteV3AnnotationPageReference).ToList();
        if (annotationRefs.Count > 0)
        {
            obj["annotations"] = new JArray(annotationRefs);
        }

        WriteV3NodeExtras(canvas, obj);

        return obj;
    }

    private static JObject WriteV3AnnotationPageReference(AnnotationPage page)
    {
        // Per spec, a Canvas's "annotations" entries may be either a bare {id,type} external
        // reference OR a fully embedded AnnotationPage - most cookbook recipes with a secondary
        // commenting/tagging/supplementing page (0019/0021/0045/0074/0103/0135/0139/0219/0258/
        // 0261/0266/0326/0346/0377/0464, among others) embed it directly; only a few (0269/0306)
        // deliberately model the external-reference form (no items on the AnnotationPage object).
        var hasItems = page.Items.OfType<AnnotationNode>().Any();
        var obj = hasItems ? WriteV3AnnotationPage(page) : new JObject { ["id"] = page.Id, ["type"] = "AnnotationPage" };

        var partOf = page.PartOf.Select(x => new JObject { ["id"] = x.Id, ["type"] = x.Type }).ToList();
        if (partOf.Count > 0)
        {
            obj["partOf"] = new JArray(partOf);
        }

        if (page.Next is not null)
        {
            obj["next"] = page.Next;
        }

        if (page.Prev is not null)
        {
            obj["prev"] = page.Prev;
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
        var obj = new JObject
        {
            ["id"] = annotation.Id,
            ["type"] = "Annotation",
            ["motivation"] = NormalizeMotivation(annotation.Motivation),
            ["body"] = annotation.Bodies.Count == 1
                ? WriteV3Resource(annotation.Body)
                : new JArray(annotation.Bodies.Select(WriteV3Resource)),
            ["target"] = JToken.FromObject(annotation.Targets.Count == 1 ? annotation.Target : annotation.Targets, JsonSerializer.Create(TrackableObject.JsonSerializerSettings))
        };

        if (annotation.Stylesheet is not null)
        {
            obj["stylesheet"] = annotation.Stylesheet;
        }

        if (annotation.TimeMode is not null)
        {
            obj["timeMode"] = annotation.TimeMode.Value;
        }

        return obj;
    }

    private static JObject WriteV3Resource(IBaseResource resource)
    {
        if (resource is SpecificResource specificResource)
        {
            // Recurse so the nested Source gets the same @id/@type-stripping treatment (e.g. an
            // embedded ImageResource, which is still BaseItem-shaped/@-prefixed internally) -
            // JObject.FromObject below only ever normalizes the outermost resource.
            var specificObj = new JObject { ["type"] = "SpecificResource" };
            if (specificResource.Id is not null)
            {
                specificObj["id"] = specificResource.Id;
            }

            specificObj["source"] = WriteV3Resource(specificResource.Source);
            if (specificResource.StyleClass is not null)
            {
                specificObj["styleClass"] = specificResource.StyleClass;
            }

            if (specificResource.Selector is not null)
            {
                specificObj["selector"] = JToken.FromObject(specificResource.Selector, JsonSerializer.Create(TrackableObject.JsonSerializerSettings));
            }

            return specificObj;
        }

        if (resource is Choice choice)
        {
            // Same reasoning as SpecificResource above: recurse per item so a BaseItem-shaped
            // alternative (e.g. an embedded ImageResource) gets its @id/@type stripped too.
            return new JObject
            {
                ["type"] = "Choice",
                ["items"] = new JArray(choice.Items.Select(WriteV3Resource))
            };
        }

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

        // BaseResource<T>.Label goes through ObjectArrayJsonConverter + Label's own
        // ValuableItemJsonConverter (bare-string shape) via the generic JObject.FromObject above,
        // never the language-map shape the V3 spec requires - rebuild it the same way
        // WriteV3Manifest/WriteV3Canvas do for their own labels (cookbook recipes 0033-choice/
        // 0434-choice-av put a label on each Choice item, e.g. "Natural Light" vs "X-Ray").
        if (token["label"] is { } labelToken && labelToken.Type != JTokenType.Null)
        {
            var values = labelToken is JArray labelArray ? labelArray.Select(v => v.ToString()) : [labelToken.ToString()];
            token.Remove("label");
            WriteLanguageMap(token, "label", values);
        }

        // Same reasoning as label above: BaseItem.Service goes through ObjectArrayJsonConverter,
        // which collapses a single service to a bare object and leaves its @id/@context/@type
        // unnormalized - but per spec, an embedded Image API "service" is always an array of
        // clean id/type objects (seen in nearly every recipe with an IIIF Image service).
        token.Remove("service");
        var resourceServices = resource.Service.Select(WriteV3EmbeddedResourceService).ToList();
        if (resourceServices.Count > 0)
        {
            token["service"] = new JArray(resourceServices);
        }

        return token;
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

    private static IBaseService? ReadV3Service(JObject obj)
    {
        // A V3 manifest's "services" array always writes id/type unprefixed (see WriteV3Service);
        // ServiceJsonConverter normalizes to whichever shape the detected leaf class needs, so no
        // renaming is required here.
        return obj.ToObject<IBaseService>();
    }

    private static Structure ReadV3Range(JObject obj)
    {
        var structure = new Structure(ReadRequiredString(obj, "id"), ReadLabels(obj["label"]).FirstOrDefault() ?? new Label("Untitled"));

        ReadV3Behavior(obj, structure);

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

        ReadV3NodeExtras(obj, structure);

        return structure;
    }

    private static Canvas ReadV3Canvas(JObject obj)
    {
        var height = (int?)obj["height"] ?? 1;
        var width = (int?)obj["width"] ?? 1;
        var canvas = new Canvas(ReadRequiredString(obj, "id"), ReadLabels(obj["label"]).FirstOrDefault() ?? new Label("Untitled"), height, width);

        ReadV3Behavior(obj, canvas);

        if ((double?)obj["duration"] is { } duration)
        {
            canvas.SetDuration(duration);
        }

        if (obj["placeholderCanvas"] is JObject placeholderObj)
        {
            canvas.SetPlaceholderCanvas(ReadV3Canvas(placeholderObj));
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
            canvas.AddAnnotationPageReference(ReadV3AnnotationPageReference(annotationsRef, canvas));
        }

        ReadV3NodeExtras(obj, canvas);

        return canvas;
    }

    private static AnnotationPage ReadV3AnnotationPageReference(JObject annotationsRef, Canvas canvas)
    {
        var page = new AnnotationPage(ReadRequiredString(annotationsRef, "id"));

        foreach (var itemObj in annotationsRef["items"]?.OfType<JObject>() ?? Enumerable.Empty<JObject>())
        {
            if (ReadV3Annotation(canvas, itemObj) is { } annotation)
            {
                page.AddItem(annotation);
            }
        }

        foreach (var partOfObj in annotationsRef["partOf"]?.OfType<JObject>() ?? Enumerable.Empty<JObject>())
        {
            page.AddPartOf(new PartOf(ReadRequiredString(partOfObj, "id"), (string?)partOfObj["type"] ?? "AnnotationCollection"));
        }

        if ((string?)annotationsRef["next"] is { } next)
        {
            page.SetNext(next);
        }

        if ((string?)annotationsRef["prev"] is { } prev)
        {
            page.SetPrev(prev);
        }

        return page;
    }

    private static AnnotationNode? ReadV3Annotation(Canvas canvas, JObject obj)
    {
        var bodyToken = obj["body"];
        var bodyObjects = bodyToken is JArray bodyArray ? bodyArray.OfType<JObject>().ToList() : bodyToken is JObject singleBody ? [singleBody] : [];
        if (bodyObjects.Count == 0)
        {
            return null;
        }

        var annotationId = ReadRequiredString(obj, "id");
        var motivation = (string?)obj["motivation"] ?? "painting";

        var resources = bodyObjects.Select(b => ReadV3AnnotationResource(b, canvas)).Where(r => r is not null).Select(r => r!).ToList();
        if (resources.Count == 0)
        {
            return null;
        }

        var targetToken = obj["target"];
        var targets = targetToken is JArray targetArray
            ? targetArray.Select(t => t.ToObject<AnnotationTarget>()!).ToList()
            : [targetToken?.ToObject<AnnotationTarget>() ?? new AnnotationTarget(canvas.Id)];

        var annotation = new AnnotationNode(annotationId, resources[0], targets[0]).SetMotivation(motivation);
        foreach (var extraBody in resources.Skip(1))
        {
            annotation.AddBody(extraBody);
        }

        foreach (var extraTarget in targets.Skip(1))
        {
            annotation.AddTarget(extraTarget);
        }

        if ((string?)obj["stylesheet"] is { } stylesheet)
        {
            annotation.SetStylesheet(stylesheet);
        }

        if ((string?)obj["timeMode"] is { } timeMode)
        {
            annotation.SetTimeMode(new TimeMode(timeMode));
        }

        return annotation;
    }

    private static IBaseResource? ReadV3AnnotationResource(JObject body, Canvas canvas)
    {
        if ((string?)body["type"] == "SpecificResource")
        {
            if (body["source"] is not JObject sourceObj || ReadV3AnnotationResource(sourceObj, canvas) is not { } source)
            {
                return null;
            }

            var specificResource = new SpecificResource(source);
            if ((string?)body["id"] is { } specificId)
            {
                specificResource.SetId(specificId);
            }

            if ((string?)body["styleClass"] is { } styleClass)
            {
                specificResource.SetStyleClass(styleClass);
            }

            if (body["selector"] is { } selectorToken)
            {
                specificResource.SetSelector(selectorToken.ToObject<ISelector>()!);
            }

            return specificResource;
        }

        if ((string?)body["type"] == "TextualBody")
        {
            return BuildTextualBody(body);
        }

        if ((string?)body["type"] == "Choice")
        {
            return BuildChoice(body, canvas);
        }

        var format = (string?)body["format"] ?? string.Empty;
        var bodyId = (string?)body["id"] ?? string.Empty;
        var labels = ReadLabels(body["label"]);

        var services = body["service"]?.OfType<JObject>().Select(ReadV3Service).Where(x => x is not null).Select(x => x!).ToList() ?? [];

        return (string?)body["type"] switch
        {
            "Image" => new ImageResource(bodyId, format)
                .SetHeight((int?)body["height"] ?? canvas.Height ?? 1)
                .SetWidth((int?)body["width"] ?? canvas.Width ?? 1)
                .SetLabel(labels)
                .AddServices(services),
            "Sound" => ((double?)body["duration"] is { } audioDuration
                ? new AudioResource(bodyId, format).SetDuration(audioDuration)
                : new AudioResource(bodyId, format)).SetLabel(labels).AddServices(services),
            "Video" => BuildVideoResource(bodyId, format, body).SetLabel(labels).AddServices(services),
            var type when type is not null => ResourceTypeRegistry.TryCreate(type, body)
                // A SpecificResource's source (or a sibling body) can be a bare, non-embeddable
                // reference to another resource (e.g. a Canvas/Manifest - cookbook recipe
                // 0022-linking-with-a-hotspot links to a whole Canvas, not an image/text body).
                ?? new BaseResource(bodyId, type),
            _ => null
        };
    }

    private static TResource AddServices<TResource>(this TResource resource, IEnumerable<IBaseService> services) where TResource : BaseItem<TResource>
    {
        foreach (var service in services)
        {
            resource.AddService(service);
        }

        return resource;
    }

    private static Choice BuildChoice(JObject body, Canvas canvas)
    {
        var items = body["items"]?.OfType<JObject>().Select(item => ReadV3AnnotationResource(item, canvas)).Where(x => x is not null).Select(x => x!).ToList()
                    ?? new List<IBaseResource>();
        return new Choice(items);
    }

    private static TextualBody BuildTextualBody(JObject body)
    {
        var textualBody = new TextualBody((string?)body["value"] ?? string.Empty);
        if ((string?)body["format"] is { } bodyFormat)
        {
            textualBody.SetFormat(bodyFormat);
        }

        if ((string?)body["language"] is { } bodyLanguage)
        {
            textualBody.SetLanguage(bodyLanguage);
        }

        return textualBody;
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
                .SelectMany(prop => (prop.Value.Type == JTokenType.Array ? prop.Value.Values<string>() : [(string?)prop.Value])
                    .OfType<string>()
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => prop.Name == "none" ? new Label(x) : new Label(x, prop.Name)))
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


