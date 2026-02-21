using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Extensions;

/// <summary>
/// A GeoJSON Geometry object.
/// </summary>
public class Geometry : TrackableObject<Geometry>
{
    [JsonProperty("type")] public string? Type => GetElementValue(x => x.Type);

    [JsonProperty("coordinates")] public Point? Coordinates => GetElementValue(x => x.Coordinates);

    public Geometry()
    {
    }

    public Geometry(string type, Point coordinates)
    {
        SetElementValue(x => x.Type, type);
        SetElementValue(x => x.Coordinates, coordinates);
    }
}