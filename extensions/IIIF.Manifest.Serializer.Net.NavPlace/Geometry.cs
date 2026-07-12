using IIIF.Manifests.Serializer.Helpers;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Extensions;

/// <summary>
///     A GeoJSON Geometry object supporting all geometry types:
///     Point, MultiPoint, LineString, MultiLineString, Polygon, MultiPolygon, GeometryCollection.
///     Per RFC 7946, coordinates structure varies by type.
/// </summary>
public class Geometry : TrackableObject<Geometry>, ICoordinateItemSupport<Geometry>
{
    public const string TypeJName = "type";
    public const string CoordinatesJName = "coordinates";
    public const string GeometriesJName = "geometries";

    /// <summary>
    ///     Create a Geometry with specified type (for deserialization or GeometryCollection).
    /// </summary>
    public Geometry(GeometryType type)
    {
        Type = type;
    }

    [JsonProperty(TypeJName)]
    public GeometryType Type
    {
        get => GetElementValue(x => x.Type)!;
        private set => SetElementValue(value);
    }

    /// <summary>
    ///     Only meaningful when <see cref="Type" /> is <see cref="GeometryType.GeometryCollection" /> -
    ///     per RFC 7946 §3.1.8, a GeometryCollection has no "coordinates" member of its own; it wraps
    ///     other complete Geometry objects (which may themselves be further GeometryCollections)
    ///     instead.
    /// </summary>
    [JsonProperty(GeometriesJName)]
    public IReadOnlyCollection<Geometry> Geometries
    {
        get => GetElementValue(x => x.Geometries) ?? [];
        private set => SetElementValue(value);
    }

    [JsonProperty(CoordinatesJName)]
    public IReadOnlyCollection<CoordinateItem> Coordinates
    {
        get => GetElementValue(x => x.Coordinates) ?? [];
        private set => SetElementValue(value);
    }

    public Geometry SetCoordinates(IReadOnlyCollection<CoordinateItem> coordinates)
    {
        Coordinates = coordinates;
        return this;
    }

    public Geometry AddCoordinate(CoordinateItem coordinate)
    {
        Coordinates = Coordinates.With(coordinate);
        return this;
    }

    public Geometry RemoveAddCoordinate(CoordinateItem coordinate)
    {
        Coordinates = Coordinates.Without(coordinate);
        return this;
    }

    public Geometry SetGeometries(IReadOnlyCollection<Geometry> geometries)
    {
        Geometries = geometries;
        return this;
    }

    public Geometry AddGeometry(Geometry geometry)
    {
        Geometries = Geometries.With(geometry);
        return this;
    }

    public Geometry RemoveGeometry(Geometry geometry)
    {
        Geometries = Geometries.Without(geometry);
        return this;
    }
}