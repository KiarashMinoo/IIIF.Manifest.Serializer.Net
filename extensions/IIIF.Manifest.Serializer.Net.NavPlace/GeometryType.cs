using IIIF.Manifests.Serializer.Shared.ValuableItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Extensions;

public class GeometryType : ValuableItem<GeometryType>
{
    private GeometryType(string value) : base(value)
    {
    }

    public static GeometryType Point => new GeometryType(nameof(Point));
    public static GeometryType MultiPoint => new GeometryType(nameof(MultiPoint));
    public static GeometryType LineString => new GeometryType(nameof(LineString));
    public static GeometryType MultiLineString => new GeometryType(nameof(MultiLineString));
    public static GeometryType Polygon => new GeometryType(nameof(Polygon));
    public static GeometryType MultiPolygon => new GeometryType(nameof(MultiPolygon));
    public static GeometryType GeometryCollection => new GeometryType(nameof(GeometryCollection));
}