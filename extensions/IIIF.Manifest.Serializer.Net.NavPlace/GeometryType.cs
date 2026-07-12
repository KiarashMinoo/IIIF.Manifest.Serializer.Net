using IIIF.Manifests.Serializer.Shared.ValuableItem;
using Newtonsoft.Json;

namespace IIIF.Manifests.Serializer.Extensions;

[JsonConverter(typeof(ValuableItemJsonConverter<GeometryType>))]
public class GeometryType : ValuableItem<GeometryType>
{
    private GeometryType(string value) : base(value)
    {
    }

    public static GeometryType Point => new(nameof(Point));
    public static GeometryType MultiPoint => new(nameof(MultiPoint));
    public static GeometryType LineString => new(nameof(LineString));
    public static GeometryType MultiLineString => new(nameof(MultiLineString));
    public static GeometryType Polygon => new(nameof(Polygon));
    public static GeometryType MultiPolygon => new(nameof(MultiPolygon));
    public static GeometryType GeometryCollection => new(nameof(GeometryCollection));
}