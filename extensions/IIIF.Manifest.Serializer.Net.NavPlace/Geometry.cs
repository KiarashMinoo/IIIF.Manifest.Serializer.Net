using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Extensions;

/// <summary>
/// A GeoJSON Geometry object.
/// </summary>
public class Geometry
{
    [JsonProperty("type")] public string Type { get; set; }

    [JsonProperty("coordinates")] public Point Coordinates { get; set; }

    public Geometry()
    {
    }

    public Geometry(string type, Point coordinates)
    {
        Type = type;
        Coordinates = coordinates;
    }
}