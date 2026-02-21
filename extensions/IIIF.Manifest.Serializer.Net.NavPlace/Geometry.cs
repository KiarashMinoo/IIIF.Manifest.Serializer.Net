using System.Collections.Generic;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Extensions;

/// <summary>
/// A GeoJSON Geometry object supporting all geometry types:
/// Point, MultiPoint, LineString, MultiLineString, Polygon, MultiPolygon, GeometryCollection.
/// Per RFC 7946, coordinates structure varies by type.
/// </summary>
[JsonConverter(typeof(GeometryJsonConverter))]
public class Geometry : TrackableObject<Geometry>
{
    public const string TypeJName = "type";
    public const string CoordinatesJName = "coordinates";
    public const string GeometriesJName = "geometries";

    [JsonProperty(TypeJName)] public string? Type => GetElementValue(x => x.Type);

    /// <summary>
    /// Raw coordinates token. Structure depends on Type:
    /// Point: [lng, lat] or [lng, lat, alt]
    /// MultiPoint: [[lng,lat], ...]
    /// LineString: [[lng,lat], ...]
    /// MultiLineString: [[[lng,lat], ...], ...]
    /// Polygon: [[[lng,lat], ...], ...]
    /// MultiPolygon: [[[[lng,lat], ...], ...], ...]
    /// </summary>
    [JsonProperty(CoordinatesJName)]
    public JToken? Coordinates => GetElementValue(x => x.Coordinates);

    /// <summary>
    /// For GeometryCollection type only: array of child Geometry objects.
    /// </summary>
    [JsonProperty(GeometriesJName)]
    public IReadOnlyCollection<Geometry>? Geometries => GetElementValue(x => x.Geometries);

    public Geometry()
    {
    }

    /// <summary>
    /// Create a Geometry with specified type (for deserialization or GeometryCollection).
    /// </summary>
    public Geometry(string type)
    {
        SetElementValue(x => x.Type, type);
    }

    /// <summary>
    /// Create a Point geometry from a Point value object.
    /// </summary>
    public Geometry(string type, Point coordinates)
    {
        SetElementValue(x => x.Type, type);
        SetElementValue(x => x.Coordinates, JToken.FromObject(coordinates));
    }

    /// <summary>
    /// Create a Geometry with raw coordinates token.
    /// </summary>
    public Geometry(string type, JToken coordinates)
    {
        SetElementValue(x => x.Type, type);
        SetElementValue(x => x.Coordinates, coordinates);
    }

    /// <summary>
    /// Set the raw coordinates token.
    /// </summary>
    public Geometry SetCoordinates(JToken coordinates) => SetElementValue(x => x.Coordinates, coordinates);

    /// <summary>
    /// Set the geometries array (for GeometryCollection only).
    /// </summary>
    public Geometry SetGeometries(Geometry[] geometries) => SetElementValue(x => x.Geometries, (IReadOnlyCollection<Geometry>)geometries);

    /// <summary>
    /// Create a Point geometry.
    /// </summary>
    public static Geometry CreatePoint(double longitude, double latitude)
        => new("Point", new Point(longitude, latitude));

    /// <summary>
    /// Create a Point geometry with altitude.
    /// </summary>
    public static Geometry CreatePoint(double longitude, double latitude, double altitude)
        => new("Point", new Point(longitude, latitude, altitude));

    /// <summary>
    /// Create a Polygon geometry from coordinate arrays.
    /// </summary>
    public static Geometry CreatePolygon(JArray coordinates)
        => new("Polygon", coordinates);

    /// <summary>
    /// Create a LineString geometry from coordinate arrays.
    /// </summary>
    public static Geometry CreateLineString(JArray coordinates)
        => new("LineString", coordinates);

    /// <summary>
    /// Create a GeometryCollection from child geometries.
    /// </summary>
    public static Geometry CreateGeometryCollection(params Geometry[] geometries)
    {
        var gc = new Geometry("GeometryCollection");
        gc.SetGeometries(geometries);
        return gc;
    }
}