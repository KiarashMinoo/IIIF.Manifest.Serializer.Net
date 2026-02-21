using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Extensions
{
    /// <summary>
    /// Represents a geographic location using GeoJSON-LD for IIIF navPlace extension.
    /// This is a simplified implementation compatible with Presentation API 2.0.
    /// </summary>
    [JsonConverter(typeof(NavPlaceJsonConverter))]
    public class NavPlace : TrackableObject<NavPlace>
    {
        public const string NavPlaceJName = "navPlace";
        public const string CoordinatesJName = "coordinates";

        /// <summary>
        /// The GeoJSON FeatureCollection containing geographic features.
        /// </summary>
        [JsonProperty(CoordinatesJName)]
        public FeatureCollection Features => GetElementValue(x => x.Features)!;

        /// <summary>
        /// Create a new NavPlace with the specified features.
        /// </summary>
        public NavPlace(FeatureCollection features)
        {
            SetElementValue(x => x.Features, features);
        }

        /// <summary>
        /// Create a new NavPlace with a single point feature.
        /// </summary>
        public static NavPlace FromPoint(double longitude, double latitude, string? label = null)
        {
            var point = new Point(longitude, latitude);
            var geometry = new Geometry("Point", point);
            var feature = new Feature(geometry, label);
            var features = new FeatureCollection([feature]);
            return new NavPlace(features);
        }
    }
}