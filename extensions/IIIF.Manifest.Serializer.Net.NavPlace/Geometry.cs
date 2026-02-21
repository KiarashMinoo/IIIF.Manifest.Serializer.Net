using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Extensions;

/// <summary>
/// A GeoJSON Geometry object.
/// </summary>
public class Geometry : TrackableObject<Geometry>
{
    public const string TypeJName = "type";
    public const string CoordinatesJName = "coordinates";

    [JsonProperty(TypeJName)] public string? Type => GetElementValue(x => x.Type);
    [JsonProperty(CoordinatesJName)] public Point? Coordinates => GetElementValue(x => x.Coordinates);

    public Geometry()
    {
    }

    public Geometry(string type, Point coordinates)
    {
        SetElementValue(x => x.Type, type);
        SetElementValue(x => x.Coordinates, coordinates);
    }
}