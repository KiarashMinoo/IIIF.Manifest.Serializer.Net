using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Shared;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Properties
{
    /// <summary>
    /// Represents georeferencing information for maps using IIIF Georeference extension.
    /// This allows associating geographic coordinates with image pixels.
    /// </summary>
    [JsonConverter(typeof(GeoreferenceJsonConverter))]
    public class Georeference : TrackableObject<Georeference>
    {
        /// <summary>
        /// The type of georeferencing transformation.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; private set; }

        /// <summary>
        /// The coordinate reference system (CRS) used.
        /// </summary>
        [JsonProperty("crs", NullValueHandling = NullValueHandling.Ignore)]
        public string Crs { get; private set; }

        /// <summary>
        /// Ground control points for transformation.
        /// </summary>
        [JsonProperty("gcps", NullValueHandling = NullValueHandling.Ignore)]
        public GroundControlPoint[] Gcps { get; private set; }

        /// <summary>
        /// Transformation parameters.
        /// </summary>
        [JsonProperty("transformation", NullValueHandling = NullValueHandling.Ignore)]
        public Transformation Transformation { get; private set; }

        /// <summary>
        /// Create a new Georeference with transformation type.
        /// </summary>
        public Georeference(string type)
        {
            Type = type;
        }

        /// <summary>
        /// Set the coordinate reference system.
        /// </summary>
        public Georeference SetCrs(string crs)
        {
            Crs = crs;
            return this;
        }

        /// <summary>
        /// Set ground control points.
        /// </summary>
        public Georeference SetGcps(GroundControlPoint[] gcps)
        {
            Gcps = gcps;
            return this;
        }

        /// <summary>
        /// Set transformation parameters.
        /// </summary>
        public Georeference SetTransformation(Transformation transformation)
        {
            Transformation = transformation;
            return this;
        }
    }

    /// <summary>
    /// A ground control point relating image pixels to geographic coordinates.
    /// </summary>
    public class GroundControlPoint
    {
        /// <summary>
        /// Image coordinates (x, y) in pixels.
        /// </summary>
        [JsonProperty("image")]
        public double[] Image { get; set; }

        /// <summary>
        /// Geographic coordinates (longitude, latitude) or (easting, northing).
        /// </summary>
        [JsonProperty("world")]
        public double[] World { get; set; }

        public GroundControlPoint(double imageX, double imageY, double worldX, double worldY)
        {
            Image = new[] { imageX, imageY };
            World = new[] { worldX, worldY };
        }
    }

    /// <summary>
    /// Transformation parameters for georeferencing.
    /// </summary>
    public class Transformation
    {
        /// <summary>
        /// Transformation type (e.g., "polynomial", "helmert").
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Transformation parameters as an array of coefficients.
        /// </summary>
        [JsonProperty("options")]
        public object Options { get; set; }

        public Transformation(string type, object options)
        {
            Type = type;
            Options = options;
        }
    }
}