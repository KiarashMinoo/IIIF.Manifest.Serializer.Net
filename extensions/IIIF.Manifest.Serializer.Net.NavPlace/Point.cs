using System.Collections.Generic;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Extensions;

/// <summary>
/// A geographic point with longitude and latitude, and optional altitude.
/// Serialized as a GeoJSON position array: [longitude, latitude] or [longitude, latitude, altitude].
/// </summary>
[JsonConverter(typeof(PointJsonConverter))]
public class Point : TrackableObject<Point>
{
    [JsonProperty("coordinates")] public IReadOnlyCollection<double>? Coordinates => GetElementValue(x => x.Coordinates);

    public Point(double longitude, double latitude)
    {
        SetElementValue(x => x.Coordinates, [longitude, latitude]);
    }

    /// <summary>
    /// Create a point with longitude, latitude, and altitude.
    /// </summary>
    public Point(double longitude, double latitude, double altitude)
    {
        SetElementValue(x => x.Coordinates, [longitude, latitude, altitude]);
    }
}