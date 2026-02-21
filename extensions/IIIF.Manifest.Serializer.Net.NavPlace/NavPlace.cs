using System.Collections.Generic;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Properties;
using IIIF.Manifests.Serializer.Shared.BaseItem;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Extensions
{
    /// <summary>
    /// Represents a geographic location using GeoJSON-LD for IIIF navPlace extension.
    /// This is a simplified implementation compatible with Presentation API 2.0.
    /// </summary>
    [JsonConverter(typeof(NavPlaceJsonConverter))]
    public class NavPlace : BaseItem<NavPlace>
    {
        public const string NavPlaceJName = "navPlace";
        public const string FeaturesJName = "features";

        /// <summary>
        /// The GeoJSON FeatureCollection containing geographic features.
        /// </summary>
        [JsonProperty(FeaturesJName)]
        public IReadOnlyCollection<Feature> Features => GetElementValue(x => x.Features) ?? [];


        /// <summary>
        /// Create a new NavPlace with the specified features.
        /// </summary>
        public NavPlace(string id) : base(id, "FeatureCollection")
        {
        }

        public NavPlace SetFeatures(Feature[] features) => SetElementValue(a => a.Features, _ => [..features]);
        public NavPlace AddFeature(Feature feature) => SetElementValue(a => a.Features, labels => labels.With(feature));
        public NavPlace RemoveFeature(Feature feature) => SetElementValue(a => a.Features, labels => labels.Without(feature));

        /// <summary>
        /// Create a new NavPlace with a single point feature.
        /// </summary>
        public static NavPlace FromPoint(double longitude, double latitude, string? label = null)
        {
            var point = new Point(longitude, latitude);
            var geometry = new Geometry("Point", point);
            var feature = new Feature("http://example.com/feature/1");
            feature.SetGeometry(geometry);

            if (!string.IsNullOrWhiteSpace(label))
            {
                var properties = new FeatureProperties();
                var lbl = new Label(label);
                properties.AddLabel(lbl);
                feature.SetProperties(properties);
            }

            var rtn = new NavPlace("http://example.com/feature-collection/1");
            rtn.AddFeature(feature);
            return rtn;
        }
    }
}