using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Nodes.Contents.Annotation;
using IIIF.Manifests.Serializer.Properties;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer;

public static partial class IiifSerializer
{
    private static JObject WriteV3Canvas(Canvas canvas)
    {
        var obj = new JObject
        {
            ["id"] = canvas.Id,
            ["type"] = "Canvas"
        };

        obj["label"] = BuildLabelLanguageMapToken(canvas.Label);

        WriteV3Behavior(canvas, obj);

        if (canvas.Height is not null) obj["height"] = canvas.Height.Value;

        if (canvas.Width is not null) obj["width"] = canvas.Width.Value;

        if (canvas.Duration is not null) obj["duration"] = canvas.Duration.Value;

        if (canvas.PlaceholderCanvas is not null) obj["placeholderCanvas"] = WriteV3Canvas(canvas.PlaceholderCanvas);

        var pages = canvas.Items.OfType<AnnotationPage>().Select(WriteV3AnnotationPage).ToList();
        if (pages.Count > 0) obj["items"] = new JArray(pages);

        var annotationRefs = canvas.Annotations.Select(WriteV3AnnotationPageReference).ToList();
        if (annotationRefs.Count > 0) obj["annotations"] = new JArray(annotationRefs);

        WriteV3NodeExtras(canvas, obj);

        return obj;
    }

    private static Canvas ReadV3Canvas(JObject obj)
    {
        var height = (int?)obj["height"] ?? 1;
        var width = (int?)obj["width"] ?? 1;
        var canvas = new Canvas(ReadRequiredString(obj, "id"), ReadLabels(obj["label"]).FirstOrDefault() ?? new Label("Untitled"), height, width);

        ReadV3Behavior(obj, canvas);

        if ((double?)obj["duration"] is { } duration) canvas.SetDuration(duration);

        if (obj["placeholderCanvas"] is JObject placeholderObj) canvas.SetPlaceholderCanvas(ReadV3Canvas(placeholderObj));

        foreach (var annotationObj in obj["items"]?.OfType<JObject>().SelectMany(x => x["items"]?.OfType<JObject>() ?? Enumerable.Empty<JObject>()) ?? Enumerable.Empty<JObject>())
            if (ReadV3Annotation(canvas, annotationObj) is { } annotation)
                canvas.AddAnnotation(annotation);

        foreach (var annotationsRef in obj["annotations"]?.OfType<JObject>() ?? Enumerable.Empty<JObject>()) canvas.AddAnnotationPageReference(ReadV3AnnotationPageReference(annotationsRef, canvas));

        ReadV3NodeExtras(obj, canvas);

        return canvas;
    }
}