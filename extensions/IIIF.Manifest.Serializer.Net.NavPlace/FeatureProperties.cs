using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Extensions;

/// <summary>
/// Properties for a geographic feature.
/// </summary>
public class FeatureProperties : TrackableObject<FeatureProperties>
{
    [JsonProperty("label")] public string Label => GetElementValue(x => x.Label)!;

    public FeatureProperties(string label)
    {
        SetElementValue(x => x.Label, label);
    }
}