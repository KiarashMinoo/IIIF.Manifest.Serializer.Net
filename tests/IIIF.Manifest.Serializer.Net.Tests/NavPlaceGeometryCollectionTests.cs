using System.Linq;
using IIIF.Manifests.Serializer.Extensions;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Tests;

/// <summary>
///     Milestone 16 (SDK_VERSIONING_GUIDE.md §10, finding 8): navPlace's <see cref="Geometry" /> had no
///     way to represent a GeoJSON "GeometryCollection" (RFC 7946 §3.1.8) - an array of nested Geometry
///     objects under "geometries", distinct from (and mutually exclusive with) the "coordinates" member
///     every other geometry type uses.
/// </summary>
public class NavPlaceGeometryCollectionTests
{
    [Fact]
    public void GeometryCollection_Should_WriteGeometriesArray_NotCoordinates()
    {
        var point = new Geometry(GeometryType.Point).SetCoordinates([new CoordinateItem(-73.9, 40.7)]);
        var lineString = new Geometry(GeometryType.LineString).SetCoordinates([new CoordinateItem(-73.9, 40.7), new CoordinateItem(-73.8, 40.8)]);
        var collection = new Geometry(GeometryType.GeometryCollection).AddGeometry(point).AddGeometry(lineString);

        var obj = JObject.Parse(collection.Serialize());

        obj["type"]!.ToString().Should().Be("GeometryCollection");
        obj["coordinates"].Should().BeNull();
        obj["geometries"]!.Should().HaveCount(2);
    }

    [Fact]
    public void GeometryCollection_Should_RoundTripNestedGeometries()
    {
        var point = new Geometry(GeometryType.Point).SetCoordinates([new CoordinateItem(-73.9, 40.7)]);
        var polygon = new Geometry(GeometryType.Polygon).SetCoordinates([
            new CoordinateItem([new CoordinateItem(-73.9, 40.7), new CoordinateItem(-73.8, 40.7), new CoordinateItem(-73.8, 40.8)])
        ]);
        var collection = new Geometry(GeometryType.GeometryCollection).AddGeometry(point).AddGeometry(polygon);

        var deserialized = TrackableObject.Parse<Geometry>(collection.Serialize());

        deserialized.Type.Value.Should().Be("GeometryCollection");
        deserialized.Geometries.Should().HaveCount(2);
        deserialized.Geometries.Select(g => g.Type.Value).Should().Contain(["Point", "Polygon"]);
        deserialized.Geometries.Single(g => g.Type.Value == "Point").Coordinates.Single().Longitude.Should().Be(-73.9);
    }

    [Fact]
    public void NonCollectionGeometry_Should_NotWriteAGeometriesField()
    {
        var point = new Geometry(GeometryType.Point).SetCoordinates([new CoordinateItem(-73.9, 40.7)]);

        var obj = JObject.Parse(point.Serialize());

        obj["geometries"].Should().BeNull();
    }
}