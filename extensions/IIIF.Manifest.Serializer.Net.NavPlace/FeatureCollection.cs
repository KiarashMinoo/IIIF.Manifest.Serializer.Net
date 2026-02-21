using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Extensions;

/// <summary>
/// A GeoJSON FeatureCollection containing geographic features.
/// </summary>
public class FeatureCollection
{
    [JsonProperty("type")] public string Type => "FeatureCollection";

    [JsonProperty("features")] public Feature[] Features { get; }

    public FeatureCollection(Feature[] features)
    {
        Features = features ?? new Feature[0];
    }
}