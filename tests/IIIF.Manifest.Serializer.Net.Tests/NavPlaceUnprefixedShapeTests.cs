using System.Linq;
using IIIF.Manifests.Serializer.Extensions;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Tests;

/// <summary>
/// Round 2 (SDK_VERSIONING_GUIDE.md §11, Milestone 23): navPlace's <see cref="NavPlace"/> and
/// <see cref="Feature"/> previously inherited <c>BaseItem</c> (<c>@id</c>/<c>@type</c>), but
/// navPlace postdates Presentation 3.0 (no 2.x form) and its own spec examples use unprefixed
/// <c>id</c>/<c>type</c> throughout - matching the fix already applied to Search 2.0/Discovery
/// 1.0/Auth 2.0 in Milestone 9.
/// </summary>
public class NavPlaceUnprefixedShapeTests
{
    [Fact]
    public void NavPlace_Should_WriteUnprefixedIdAndType()
    {
        var navPlace = new NavPlace("https://example.org/feature-collection/1");

        var obj = JObject.Parse(navPlace.Serialize());

        obj["id"]!.ToString().Should().Be("https://example.org/feature-collection/1");
        obj["type"]!.ToString().Should().Be("FeatureCollection");
        obj["@id"].Should().BeNull();
        obj["@type"].Should().BeNull();
    }

    [Fact]
    public void Feature_Should_WriteUnprefixedIdAndType()
    {
        var feature = new Feature("https://example.org/feature/1");

        var obj = JObject.Parse(feature.Serialize());

        obj["id"]!.ToString().Should().Be("https://example.org/feature/1");
        obj["type"]!.ToString().Should().Be("Feature");
        obj["@id"].Should().BeNull();
        obj["@type"].Should().BeNull();
    }

    [Fact]
    public void NavPlace_Should_RoundTripFeaturesThroughUnprefixedShape()
    {
        var navPlace = new NavPlace("https://example.org/feature-collection/1")
            .AddFeature(new Feature("https://example.org/feature/1")
                .SetGeometry(new Geometry(GeometryType.Point).AddCoordinate(new CoordinateItem(-73.9857, 40.7484)))
                .SetProperties(new FeatureProperties().AddLabel(new Properties.Label("New York"))));

        var deserialized = TrackableObject.Parse<NavPlace>(navPlace.Serialize());

        deserialized.Id.Should().Be("https://example.org/feature-collection/1");
        deserialized.Type.Should().Be("FeatureCollection");
        var feature = deserialized.Features.Single();
        feature.Id.Should().Be("https://example.org/feature/1");
        feature.Type.Should().Be("Feature");
        feature.Geometry!.Coordinates.Single().Longitude.Should().Be(-73.9857);
    }
}
