using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Extensions;

/// <summary>
/// The W3C "SvgSelector" a <see cref="GeoreferenceAnnotation"/> target uses to select a polygonal
/// or rectangular region of the targeted Canvas/Image Service - the preferred selector shape for
/// Georeference Annotations per the extension spec (§3.3.2).
/// </summary>
public class GeoreferenceSvgSelector : TrackableObject<GeoreferenceSvgSelector>
{
    public const string TypeJName = "type";
    public const string ValueJName = "value";

    [GeoreferenceExtension("3.0")]
    [JsonProperty(TypeJName)]
    public string Type
    {
        get => GetElementValue(x => x.Type) ?? "SvgSelector";
        private set => SetElementValue(value);
    }

    [JsonProperty(ValueJName)]
    public string Value
    {
        get => GetElementValue(x => x.Value)!;
        private set => SetElementValue(value);
    }

    [JsonConstructor]
    public GeoreferenceSvgSelector(string value)
    {
        Type = "SvgSelector";
        Value = value;
    }
}
