using System.Collections.Generic;
using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Extensions
{
    /// <summary>
    /// Represents a geographic location using GeoJSON-LD for IIIF navPlace extension.
    /// This is a simplified implementation compatible with Presentation API 2.0.
    /// </summary>
    public class NavPlace : BaseItem<NavPlace>
    {
        public const string NavPlaceJName = "navPlace";
        public const string FeaturesJName = "features";

        /// <summary>
        /// The GeoJSON FeatureCollection containing geographic features.
        /// </summary>
        [JsonProperty(FeaturesJName)]
        public IReadOnlyCollection<Feature> Features
        {
            get => GetElementValue(x => x.Features) ?? [];
            private set => SetElementValue(value);
        }


        /// <summary>
        /// Create a new NavPlace with the specified features.
        /// </summary>
        public NavPlace(string id) : base(id, "FeatureCollection")
        {
        }

        public NavPlace SetFeatures(Feature[] features) => SetElementValue(a => a.Features, _ => [..features]);
        public NavPlace AddFeature(Feature feature) => SetElementValue(a => a.Features, labels => labels.With(feature));
        public NavPlace RemoveFeature(Feature feature) => SetElementValue(a => a.Features, labels => labels.Without(feature));
    }
}