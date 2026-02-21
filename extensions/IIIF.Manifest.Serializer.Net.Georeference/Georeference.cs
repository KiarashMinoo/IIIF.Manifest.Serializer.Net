using System.Collections.Generic;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Extensions
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
        public string Type => GetElementValue(x => x.Type)!;

        /// <summary>
        /// The coordinate reference system (CRS) used.
        /// </summary>
        [JsonProperty("crs", NullValueHandling = NullValueHandling.Ignore)]
        public string? Crs => GetElementValue(x => x.Crs);

        /// <summary>
        /// Ground control points for transformation.
        /// </summary>
        [JsonProperty("gcps", NullValueHandling = NullValueHandling.Ignore)]
        public IReadOnlyCollection<GroundControlPoint> Gcps => GetElementValue(x => x.Gcps) ?? [];

        /// <summary>
        /// Transformation parameters.
        /// </summary>
        [JsonProperty("transformation", NullValueHandling = NullValueHandling.Ignore)]
        public Transformation? Transformation => GetElementValue(x => x.Transformation);

        /// <summary>
        /// Create a new Georeference with transformation type.
        /// </summary>
        public Georeference(string type)
        {
            SetElementValue(x => x.Type, type);
        }

        /// <summary>
        /// Set the coordinate reference system.
        /// </summary>
        public Georeference SetCrs(string crs) => SetElementValue(x => x.Crs, crs);

        /// <summary>
        /// Set ground control points.
        /// </summary>
        public Georeference SetGcps(GroundControlPoint[] gcps) => SetElementValue(x => x.Gcps, gcps);

        /// <summary>
        /// Set transformation parameters.
        /// </summary>
        public Georeference SetTransformation(Transformation transformation) => SetElementValue(x => x.Transformation, transformation);
    }
}