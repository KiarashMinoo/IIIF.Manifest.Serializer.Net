using IIIF.Manifests.Serializer.Shared.BaseItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Extensions;

/// <summary>
/// A GeoJSON Feature representing a geographic area.
/// Supports optional id property per the navPlace extension spec.
/// </summary>
[JsonConverter(typeof(FeatureJsonConverter))]
public class Feature(string id) : BaseItem<Feature>(id, "Feature")
{
    public const string GeometryJName = "geometry";
    public const string PropertiesJName = "properties";

    [JsonProperty(GeometryJName)] public Geometry? Geometry => GetElementValue(x => x.Geometry);

    [JsonProperty(PropertiesJName)] public FeatureProperties? Properties => GetElementValue(x => x.Properties);

    /// <summary>
    /// Set the geometry for this Feature.
    /// </summary>
    public Feature SetGeometry(Geometry geometry) => SetElementValue(x => x.Geometry, geometry);

    /// <summary>
    /// Set the properties for this Feature.
    /// </summary>
    public Feature SetProperties(FeatureProperties properties) => SetElementValue(x => x.Properties, properties);
}