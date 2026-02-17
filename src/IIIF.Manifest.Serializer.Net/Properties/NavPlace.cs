using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties
{
    /// <summary>
    /// Represents a geographic location using GeoJSON-LD for IIIF navPlace extension.
    /// This is a simplified implementation compatible with Presentation API 2.0.
    /// </summary>
    [JsonConverter(typeof(NavPlaceJsonConverter))]
    public class NavPlace : TrackableObject<NavPlace>
    {
        /// <summary>
        /// The GeoJSON FeatureCollection containing geographic features.
        /// </summary>
        [JsonProperty("features")]
        public FeatureCollection Features { get; private set; }

        /// <summary>
        /// Create a new NavPlace with the specified features.
        /// </summary>
        public NavPlace(FeatureCollection features)
        {
            Features = features;
        }

        /// <summary>
        /// Create a new NavPlace with a single point feature.
        /// </summary>
        public static NavPlace FromPoint(double longitude, double latitude, string label = null)
        {
            var point = new Point(longitude, latitude);
            var geometry = new Geometry("Point", point);
            var feature = new Feature(geometry, label);
            var features = new FeatureCollection(new[] { feature });
            return new NavPlace(features);
        }
    }

    /// <summary>
    /// A GeoJSON FeatureCollection containing geographic features.
    /// </summary>
    public class FeatureCollection
    {
        [JsonProperty("type")]
        public string Type => "FeatureCollection";

        [JsonProperty("features")]
        public Feature[] Features { get; }

        public FeatureCollection(Feature[] features)
        {
            Features = features ?? new Feature[0];
        }
    }

    /// <summary>
    /// A GeoJSON Feature representing a geographic area.
    /// </summary>
    public class Feature
    {
        [JsonProperty("type")]
        public string Type => "Feature";

        [JsonProperty("geometry")]
        public Geometry Geometry { get; set; }

        [JsonProperty("properties", NullValueHandling = NullValueHandling.Ignore)]
        public FeatureProperties Properties { get; set; }

        public Feature()
        {
        }

        public Feature(Geometry geometry, string label = null)
        {
            Geometry = geometry;
            if (!string.IsNullOrEmpty(label))
            {
                Properties = new FeatureProperties { Label = label };
            }
        }
    }

    /// <summary>
    /// Properties of a GeoJSON Feature.
    /// </summary>
    public class FeatureProperties
    {
        [JsonProperty("label", NullValueHandling = NullValueHandling.Ignore)]
        public string Label { get; set; }
    }

    /// <summary>
    /// A GeoJSON Geometry object.
    /// </summary>
    public class Geometry
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("coordinates")]
        public Point Coordinates { get; set; }

        public Geometry()
        {
        }

        public Geometry(string type, Point coordinates)
        {
            Type = type;
            Coordinates = coordinates;
        }
    }

    /// <summary>
    /// A geographic point with longitude and latitude.
    /// </summary>
    public class Point
    {
        [JsonProperty("coordinates")]
        public double[] Coordinates { get; set; }

        public Point()
        {
        }

        public Point(double longitude, double latitude)
        {
            Coordinates = new[] { longitude, latitude };
        }
    }
}