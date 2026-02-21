using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Extensions;

/// <summary>
/// A GeoJSON Feature representing a geographic area.
/// </summary>
public class Feature : TrackableObject<Feature>
{
    public const string TypeJName = "type";
    public const string GeometryJName = "geometry";
    public const string PropertiesJName = "properties";

    [JsonProperty(TypeJName)] public string Type => GetElementValue(x => x.Type)!;

    [JsonProperty(GeometryJName)] public Geometry? Geometry => GetElementValue(x => x.Geometry);

    [JsonProperty(PropertiesJName)] public FeatureProperties? Properties => GetElementValue(x => x.Properties);

    public Feature()
    {
        SetElementValue(x => x.Type, "Feature");
    }

    public Feature(Geometry geometry, string? label = null) : this()
    {
        SetElementValue(x => x.Geometry, geometry);
        if (!string.IsNullOrEmpty(label))
        {
            SetElementValue(x => x.Properties, new FeatureProperties(label));
        }
    }
}