using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Extensions;

/// <summary>
/// A geographic point with longitude and latitude.
/// </summary>
public class Point
{
    [JsonProperty("coordinates")] public double[] Coordinates { get; set; }

    public Point()
    {
    }

    public Point(double longitude, double latitude)
    {
        Coordinates = new[] { longitude, latitude };
    }
}