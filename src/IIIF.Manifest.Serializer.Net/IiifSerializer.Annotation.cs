using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Nodes.Contents.Annotation;
using IIIF.Manifests.Serializer.Nodes.Contents.Audio.Resource;
using IIIF.Manifests.Serializer.Nodes.Contents.Choice;
using IIIF.Manifests.Serializer.Nodes.Contents.Image.Resource;
using IIIF.Manifests.Serializer.Nodes.Contents.Textual.Resource;
using IIIF.Manifests.Serializer.Nodes.Contents.Video.Resource;
using IIIF.Manifests.Serializer.Properties;
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
///     Write/read logic for AnnotationPage, Annotation, and the polymorphic Annotation-body resource
///     dispatch (Image/Sound/Video/TextualBody/Choice/SpecificResource/extension types via
///     <see cref="ResourceTypeRegistry" />).
/// </summary>
public static partial class IiifSerializer
{
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
        if (partOf.Count > 0) obj["partOf"] = new JArray(partOf);

        if (page.Next is not null) obj["next"] = page.Next;

        if (page.Prev is not null) obj["prev"] = page.Prev;

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

        if (annotation.Stylesheet is not null) obj["stylesheet"] = annotation.Stylesheet;

        if (annotation.TimeMode is not null) obj["timeMode"] = annotation.TimeMode.Value;

        WriteV3AdditionalProperties(annotation, obj);

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
            if (specificResource.Id is not null) specificObj["id"] = specificResource.Id;

            specificObj["source"] = WriteV3Resource(specificResource.Source);
            if (specificResource.StyleClass is not null) specificObj["styleClass"] = specificResource.StyleClass;

            if (specificResource.Selector is not null) specificObj["selector"] = JToken.FromObject(specificResource.Selector, JsonSerializer.Create(TrackableObject.JsonSerializerSettings));

            return specificObj;
        }

        if (resource is Choice choice)
            // Same reasoning as SpecificResource above: recurse per item so a BaseItem-shaped
            // alternative (e.g. an embedded ImageResource) gets its @id/@type stripped too.
            return new JObject
            {
                ["type"] = "Choice",
                ["items"] = new JArray(choice.Items.Select(WriteV3Resource))
            };

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
        if (resourceServices.Count > 0) token["service"] = new JArray(resourceServices);

        return token;
    }

    private static AnnotationPage ReadV3AnnotationPageReference(JObject annotationsRef, Canvas canvas)
    {
        var page = new AnnotationPage(ReadRequiredString(annotationsRef, "id"));

        foreach (var itemObj in annotationsRef["items"]?.OfType<JObject>() ?? Enumerable.Empty<JObject>())
            if (ReadV3Annotation(canvas, itemObj) is { } annotation)
                page.AddItem(annotation);

        foreach (var partOfObj in annotationsRef["partOf"]?.OfType<JObject>() ?? Enumerable.Empty<JObject>()) page.AddPartOf(new PartOf(ReadRequiredString(partOfObj, "id"), (string?)partOfObj["type"] ?? "AnnotationCollection"));

        if ((string?)annotationsRef["next"] is { } next) page.SetNext(next);

        if ((string?)annotationsRef["prev"] is { } prev) page.SetPrev(prev);

        return page;
    }

    private static AnnotationNode? ReadV3Annotation(Canvas canvas, JObject obj)
    {
        var bodyToken = obj["body"];
        var bodyObjects = bodyToken is JArray bodyArray ? bodyArray.OfType<JObject>().ToList() : bodyToken is JObject singleBody ? [singleBody] : [];
        if (bodyObjects.Count == 0) return null;

        var annotationId = ReadRequiredString(obj, "id");
        var motivation = (string?)obj["motivation"] ?? "painting";

        var resources = bodyObjects.Select(b => ReadV3AnnotationResource(b, canvas)).Where(r => r is not null).Select(r => r!).ToList();
        if (resources.Count == 0) return null;

        var targetToken = obj["target"];
        var targets = targetToken is JArray targetArray
            ? targetArray.Select(t => t.ToObject<AnnotationTarget>()!).ToList()
            : [targetToken?.ToObject<AnnotationTarget>() ?? new AnnotationTarget(canvas.Id)];

        var annotation = new AnnotationNode(annotationId, resources[0], targets[0]).SetMotivation(motivation);
        foreach (var extraBody in resources.Skip(1)) annotation.AddBody(extraBody);

        foreach (var extraTarget in targets.Skip(1)) annotation.AddTarget(extraTarget);

        if ((string?)obj["stylesheet"] is { } stylesheet) annotation.SetStylesheet(stylesheet);

        if ((string?)obj["timeMode"] is { } timeMode) annotation.SetTimeMode(new TimeMode(timeMode));

        // textGranularity (extensions/IIIF.Manifest.Serializer.Net.TextGranularity) - see
        // WriteV3AdditionalProperties/ReadV3AdditionalProperty's doc comments in
        // IiifSerializer.Helpers.cs for why the key is hardcoded here rather than referenced from
        // the extension assembly.
        ReadV3AdditionalProperty(obj, annotation, "textGranularity");

        return annotation;
    }

    private static IBaseResource? ReadV3AnnotationResource(JObject body, Canvas canvas)
    {
        if ((string?)body["type"] == "SpecificResource")
        {
            if (body["source"] is not JObject sourceObj || ReadV3AnnotationResource(sourceObj, canvas) is not { } source) return null;

            var specificResource = new SpecificResource(source);
            if ((string?)body["id"] is { } specificId) specificResource.SetId(specificId);

            if ((string?)body["styleClass"] is { } styleClass) specificResource.SetStyleClass(styleClass);

            if (body["selector"] is { } selectorToken) specificResource.SetSelector(selectorToken.ToObject<ISelector>()!);

            return specificResource;
        }

        if ((string?)body["type"] == "TextualBody") return BuildTextualBody(body);

        if ((string?)body["type"] == "Choice") return BuildChoice(body, canvas);

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
        foreach (var service in services) resource.AddService(service);

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
        if ((string?)body["format"] is { } bodyFormat) textualBody.SetFormat(bodyFormat);

        if ((string?)body["language"] is { } bodyLanguage) textualBody.SetLanguage(bodyLanguage);

        return textualBody;
    }

    private static VideoResource BuildVideoResource(string bodyId, string format, JObject body)
    {
        var videoResource = new VideoResource(bodyId, format);
        if ((int?)body["height"] is { } videoHeight) videoResource.SetHeight(videoHeight);
        if ((int?)body["width"] is { } videoWidth) videoResource.SetWidth(videoWidth);
        if ((double?)body["duration"] is { } videoDuration) videoResource.SetDuration(videoDuration);
        return videoResource;
    }

    private static string NormalizeMotivation(string motivation)
    {
        return motivation.StartsWith("sc:", StringComparison.Ordinal) ? motivation.Substring(3) : motivation;
    }
}