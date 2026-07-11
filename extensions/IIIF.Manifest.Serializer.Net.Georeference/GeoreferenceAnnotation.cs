using System.Collections.Generic;
using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Extensions;

/// <summary>
/// A Georeference Annotation - the actual construct the Georeference extension defines (§3): a
/// W3C Annotation with <c>motivation: "georeferencing"</c>, a <see cref="Target"/> (the Canvas/Image
/// Service being georeferenced, optionally a specific region), and a <see cref="Body"/> (a GeoJSON
/// FeatureCollection of Ground Control Points - reuses the navPlace extension's <c>NavPlace</c>
/// type, since both are the same FeatureCollection shape). Previously only the property-level
/// pieces (<c>transformation</c>/<c>resourceCoords</c>) existed, with no wrapper modeling this
/// top-level construct at all.
/// </summary>
public class GeoreferenceAnnotation : TrackableObject<GeoreferenceAnnotation>
{
    public const string DefaultGeoreferenceContext = "http://iiif.io/api/extension/georef/1/context.json";
    public const string DefaultPresentationContext = "http://iiif.io/api/presentation/3/context.json";
    public const string ContextJName = "@context";
    public const string IdJName = "id";
    public const string TypeJName = "type";
    public const string MotivationJName = "motivation";
    public const string TargetJName = "target";
    public const string BodyJName = "body";

    [JsonProperty(ContextJName)]
    [JsonConverter(typeof(ObjectArrayJsonConverter))]
    public IReadOnlyCollection<string> Context
    {
        get => GetElementValue(x => x.Context) ?? [DefaultGeoreferenceContext, DefaultPresentationContext];
        private set => SetElementValue(value);
    }

    [JsonProperty(IdJName)]
    public string? Id
    {
        get => GetElementValue(x => x.Id);
        private set => SetElementValue(value);
    }

    [JsonProperty(TypeJName)]
    public string Type
    {
        get => GetElementValue(x => x.Type) ?? "Annotation";
        private set => SetElementValue(value);
    }

    [GeoreferenceExtension("3.0")]
    [JsonProperty(MotivationJName)]
    public string Motivation
    {
        get => GetElementValue(x => x.Motivation) ?? "georeferencing";
        private set => SetElementValue(value);
    }

    [JsonProperty(TargetJName)]
    public GeoreferenceTarget Target
    {
        get => GetElementValue(x => x.Target)!;
        private set => SetElementValue(value);
    }

    [JsonProperty(BodyJName)]
    public NavPlace Body
    {
        get => GetElementValue(x => x.Body)!;
        private set => SetElementValue(value);
    }

    public GeoreferenceAnnotation(GeoreferenceTarget target, NavPlace body)
    {
        Context = [DefaultGeoreferenceContext, DefaultPresentationContext];
        Type = "Annotation";
        Motivation = "georeferencing";
        Target = target;
        Body = body;
    }

    public GeoreferenceAnnotation SetId(string id)
    {
        Id = id;
        return this;
    }
}
