using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Extensions;

/// <summary>
/// A GeoJSON Feature representing a geographic area.
/// </summary>
public class Feature
{
    [JsonProperty("type")] public string Type => "Feature";

    [JsonProperty("geometry")] public Geometry Geometry { get; set; }

    [JsonProperty("properties", NullValueHandling = NullValueHandling.Ignore)]
    public FeatureProperties Properties { get; set; }

    public Feature()
    {
    }

    public Feature(Geometry geometry, string label = null)
    {
        Geometry = geometry;
        if (!string.IsNullOrEmpty(label))
        {
            Properties = new FeatureProperties { Label = label };
        }
    }
}