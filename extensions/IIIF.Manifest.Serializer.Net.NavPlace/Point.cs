using System.Collections.Generic;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Extensions;

/// <summary>
/// A geographic point with longitude and latitude.
/// </summary>
public class Point : TrackableObject<Point>
{
    [JsonProperty("coordinates")] public IReadOnlyCollection<double>? Coordinates => GetElementValue(x => x.Coordinates);

    public Point()
    {
    }

    public Point(double longitude, double latitude)
    {
        SetElementValue(x => x.Coordinates, [longitude, latitude]);
    }
}