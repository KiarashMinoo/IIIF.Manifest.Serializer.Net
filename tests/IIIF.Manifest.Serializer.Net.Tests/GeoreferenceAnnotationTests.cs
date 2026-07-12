using IIIF.Manifests.Serializer.Extensions;
using IIIF.Manifests.Serializer.Extensions.ResourceCoords;
using IIIF.Manifests.Serializer.Extensions.Transformations;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Tests;

/// <summary>
///     Milestone 17 (SDK_VERSIONING_GUIDE.md §10, finding 9): the Georeference extension only modeled
///     the property-level pieces (transformation/resourceCoords), with no wrapper for the actual
///     construct the spec defines - a full W3C Annotation with motivation "georeferencing", a
///     target (Canvas/Image Service, optionally a specific region via an SvgSelector), and a body
///     (a GeoJSON FeatureCollection of Ground Control Points).
/// </summary>
public class GeoreferenceAnnotationTests
{
    private static Feature MakeGcp(double resourceX, double resourceY, double lon, double lat)
    {
        return new Feature("").SetGeometry(new Geometry(GeometryType.Point).SetCoordinates([new CoordinateItem(lon, lat)]))
            .SetProperties(new FeatureProperties().SetResourceCoords([resourceX, resourceY]));
    }

    [Fact]
    public void GeoreferenceAnnotation_Should_WriteRequiredContextTypeAndMotivation()
    {
        var body = new NavPlace("https://example.org/feature-collection.json")
            .SetTransformation(new Transformation(TransformationType.Polynomial))
            .AddFeature(MakeGcp(5085, 782, 4.4885839, 51.9101828));
        var annotation = new GeoreferenceAnnotation(new GeoreferenceTarget("https://example.org/canvas.json"), body)
            .SetId("https://example.org/canvas-annotation.json");

        var obj = JObject.Parse(annotation.Serialize());

        obj["type"]!.ToString().Should().Be("Annotation");
        obj["motivation"]!.ToString().Should().Be("georeferencing");
        obj["target"]!.ToString().Should().Be("https://example.org/canvas.json");
        // NavPlace (the body FeatureCollection) now uses unprefixed id/type (round 2, Milestone 23),
        // matching the spec's own Georeference Annotation body example.
        obj["body"]!["type"]!.ToString().Should().Be("FeatureCollection");
        obj["@context"]!.Should().HaveCount(2);
    }

    [Fact]
    public void GeoreferenceAnnotation_Should_RoundTripFullResourceTargetAndTransformation()
    {
        var body = new NavPlace("https://example.org/feature-collection.json")
            .SetTransformation(new Transformation(TransformationType.ThinPlateSpline))
            .AddFeature(MakeGcp(5085, 782, 4.4885839, 51.9101828))
            .AddFeature(MakeGcp(5467, 1338, 4.5011785, 51.901595));
        var target = new GeoreferenceTarget("https://example.org/canvas.json", "Canvas").SetSourceDimensions(2514, 5965);
        var annotation = new GeoreferenceAnnotation(target, body).SetId("https://example.org/canvas-annotation.json");

        var deserialized = TrackableObject.Parse<GeoreferenceAnnotation>(annotation.Serialize());

        deserialized.Motivation.Should().Be("georeferencing");
        deserialized.Target.SourceId.Should().Be("https://example.org/canvas.json");
        deserialized.Target.SourceType.Should().Be("Canvas");
        deserialized.Target.SourceHeight.Should().Be(2514);
        deserialized.Target.SourceWidth.Should().Be(5965);
        deserialized.Body.Transformation!.Type.Value.Should().Be("thinPlateSpline");
        deserialized.Body.Features.Should().HaveCount(2);
    }

    [Fact]
    public void GeoreferenceTarget_Should_WriteSpecificResourceWithSvgSelector()
    {
        var target = new GeoreferenceTarget("https://example.org/canvas2.json", "Canvas")
            .SetSourceDimensions(2514, 5965)
            .SetSelector(new GeoreferenceSvgSelector("<svg><polygon points=\"59,84 44,2329 5932,2353 5920,103 \" /></svg>"))
            .SetSpecificResourceId("https://example.org/canvas-specific-resource.json");

        var obj = JObject.Parse(target.Serialize());

        obj["id"]!.ToString().Should().Be("https://example.org/canvas-specific-resource.json");
        obj["type"]!.ToString().Should().Be("SpecificResource");
        obj["source"]!["id"]!.ToString().Should().Be("https://example.org/canvas2.json");
        obj["source"]!["height"]!.Value<int>().Should().Be(2514);
        obj["selector"]!["type"]!.ToString().Should().Be("SvgSelector");
    }

    [Fact]
    public void GeoreferenceTarget_Should_RoundTripSpecificResourceWithSvgSelector()
    {
        var target = new GeoreferenceTarget("https://example.org/canvas2.json", "Canvas")
            .SetSelector(new GeoreferenceSvgSelector("<svg><rect x=\"0\" y=\"0\" width=\"10\" height=\"10\"/></svg>"));

        var deserialized = TrackableObject.Parse<GeoreferenceTarget>(target.Serialize());

        deserialized.SourceId.Should().Be("https://example.org/canvas2.json");
        deserialized.SourceType.Should().Be("Canvas");
        deserialized.Selector!.Value.Should().Contain("<rect");
    }

    [Fact]
    public void Transformation_Should_BeSettableDirectlyOnTheFeatureCollectionBody()
    {
        // Milestone 17's cross-cutting fix: transformation was only reachable via a BaseNode
        // constraint (Manifest/Canvas/etc.), so it could never be set on the FeatureCollection
        // body (NavPlace is a BaseItem, not a BaseNode) where the spec actually places it.
        var body = new NavPlace("https://example.org/feature-collection.json")
            .SetTransformation(new Transformation(TransformationType.Polynomial));

        body.Transformation!.Type.Value.Should().Be("polynomial");

        var obj = JObject.Parse(body.Serialize());
        obj["transformation"]!["type"]!.ToString().Should().Be("polynomial");
    }
}