using System.Linq;
using IIIF.Manifests.Serializer.Nodes.Contents.Annotation;
using IIIF.Manifests.Serializer.Properties;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer;

public static partial class IiifSerializer
{
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
}
