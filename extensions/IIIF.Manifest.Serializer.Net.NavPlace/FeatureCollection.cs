using System.Collections.Generic;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Extensions;

/// <summary>
/// A GeoJSON FeatureCollection containing geographic features.
/// </summary>
public class FeatureCollection : TrackableObject<FeatureCollection>
{
    public const string TypeJName = "type";
    public const string FeaturesJName = "features";

    [JsonProperty(TypeJName)] public string Type => GetElementValue(x => x.Type)!;

    [JsonProperty(FeaturesJName)] public IReadOnlyCollection<Feature> Features => GetElementValue(x => x.Features) ?? [];

    public FeatureCollection(Feature[] features)
    {
        SetElementValue(x => x.Type, "FeatureCollection");
        SetElementValue(x => x.Features, features);
    }
}