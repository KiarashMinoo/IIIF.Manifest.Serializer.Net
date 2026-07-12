using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Extensions;

/// <summary>
///     Represents a geographic location using GeoJSON-LD for IIIF navPlace extension - a
///     FeatureCollection containing Features (RFC 7946). navPlace postdates Presentation 3.0 (no
///     2.x form), so - like Search 2.0/Discovery 1.0/Auth 2.0 (Milestone 9) - it uses unprefixed
///     "id"/"type" rather than BaseItem's "@id"/"@type" (this class is also reused directly as the
///     Georeference extension's Annotation body - see <c>GeoreferenceAnnotation</c> - whose own
///     spec example likewise shows unprefixed id/type).
/// </summary>
public class NavPlace : UnprefixedBaseItem<NavPlace>
{
    public const string NavPlaceJName = "navPlace";
    public const string FeaturesJName = "features";


    /// <summary>
    ///     Create a new NavPlace with the specified features.
    /// </summary>
    public NavPlace(string id) : base(id, "FeatureCollection")
    {
    }

    /// <summary>
    ///     The GeoJSON FeatureCollection containing geographic features.
    /// </summary>
    [JsonProperty(FeaturesJName)]
    public IReadOnlyCollection<Feature> Features
    {
        get => GetElementValue(x => x.Features) ?? [];
        private set => SetElementValue(value);
    }

    public NavPlace SetFeatures(Feature[] features)
    {
        return SetElementValue(a => a.Features, _ => [..features]);
    }

    public NavPlace AddFeature(Feature feature)
    {
        return SetElementValue(a => a.Features, labels => labels.With(feature));
    }

    public NavPlace RemoveFeature(Feature feature)
    {
        return SetElementValue(a => a.Features, labels => labels.Without(feature));
    }
}