using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Extensions;

/// <summary>
/// Properties for a geographic feature.
/// </summary>
public class FeatureProperties
{
    [JsonProperty("label")] public string Label { get; set; }
}