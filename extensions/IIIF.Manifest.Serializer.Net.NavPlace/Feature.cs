using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.Content.Resources;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Extensions;

/// <summary>
/// A GeoJSON Feature representing a geographic area. navPlace postdates Presentation 3.0 (no 2.x
/// form), so - like Search 2.0/Discovery 1.0/Auth 2.0 (Milestone 9) - it uses unprefixed "id"/"type"
/// rather than BaseItem's "@id"/"@type". Also implements <see cref="IBaseResource"/> so a bare
/// Feature can stand in as an Annotation body directly (cookbook recipe
/// 0139-geolocate-canvas-fragment "tagging" a Canvas region with a GeoJSON location) - distinct from
/// navPlace's usual Manifest/Canvas-level <c>navPlace</c> property. Registers itself with
/// <see cref="ResourceTypeRegistry"/> so core's Annotation-body dispatch (which cannot reference
/// this extension assembly) can still recognize <c>"type":"Feature"</c> bodies.
/// </summary>
public class Feature : UnprefixedBaseItem<Feature>, IBaseResource
{
    public const string GeometryJName = "geometry";
    public const string PropertiesJName = "properties";

    static Feature()
    {
        ResourceTypeRegistry.Register("Feature", obj => obj.ToObject<Feature>(BaseResourceJsonConverter.LeafSerializer)!);
    }

    ResourceType? IBaseResource.Type => new(Type ?? "Feature");

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

    public Feature(string id) : base(id, "Feature")
    {
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
