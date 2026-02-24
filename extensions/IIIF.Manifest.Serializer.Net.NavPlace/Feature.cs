using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Extensions;

/// <summary>
/// A GeoJSON Feature representing a geographic area.
/// Supports optional id property per the navPlace extension spec.
/// </summary>
public class Feature(string id) : BaseItem<Feature>(id, "Feature")
{
    public const string GeometryJName = "geometry";
    public const string PropertiesJName = "properties";

    [JsonProperty(GeometryJName)]
    public Geometry? Geometry
    {
        get => GetElementValue(x => x.Geometry);
        private set => SetElementValue(value);
    }

    [JsonProperty(PropertiesJName)]
    public FeatureProperties? Properties
    {
        get => GetElementValue(x => x.Properties);
        private set => SetElementValue(value);
    }

    /// <summary>
    /// Set the geometry for this Feature.
    /// </summary>
    public Feature SetGeometry(Geometry geometry) => SetElementValue(x => x.Geometry, geometry);

    /// <summary>
    /// Set the properties for this Feature.
    /// </summary>
    public Feature SetProperties(FeatureProperties properties) => SetElementValue(x => x.Properties, properties);
}