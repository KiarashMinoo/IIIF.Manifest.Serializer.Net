using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Extensions;

/// <summary>
/// A GeoJSON Feature representing a geographic area.
/// </summary>
public class Feature : TrackableObject<Feature>
{
    [JsonProperty("type")] public string Type => GetElementValue(x => x.Type)!;

    [JsonProperty("geometry")] public Geometry? Geometry => GetElementValue(x => x.Geometry);

    [JsonProperty("properties", NullValueHandling = NullValueHandling.Ignore)]
    public FeatureProperties? Properties => GetElementValue(x => x.Properties);

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