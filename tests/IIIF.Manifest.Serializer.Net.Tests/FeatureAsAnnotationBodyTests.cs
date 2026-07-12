using System.Linq;
using IIIF.Manifests.Serializer.Extensions;
using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Nodes.Contents.Annotation;
using IIIF.Manifests.Serializer.Shared.Selectors;
using IIIF.Manifests.Serializer.Shared.Trackable;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Tests;

/// <summary>
///     Cookbook Group H: recipe 0139-geolocate-canvas-fragment embeds a bare GeoJSON
///     <see cref="Feature" /> directly as a "tagging" Annotation body targeting a Canvas fragment -
///     distinct from navPlace's usual Manifest/Canvas-level <c>navPlace</c> property. Required making
///     <see cref="Feature" /> implement <see cref="Shared.Content.Resources.IBaseResource" /> and
///     self-register with <see cref="Shared.Content.Resources.ResourceTypeRegistry" /> so core's
///     Annotation-body dispatch (which cannot reference the navPlace extension assembly) still
///     recognizes it.
/// </summary>
public class FeatureAsAnnotationBodyTests
{
    private static Annotation BuildGeoTaggingAnnotation()
    {
        var feature = new Feature("https://example.org/geo.json")
            .SetGeometry(new Geometry(GeometryType.Polygon)
                .AddCoordinate(new CoordinateItem(-77.019853, 38.913101))
                .AddCoordinate(new CoordinateItem(-77.110013, 38.843254)))
            .SetProperties(new FeatureProperties().AddLabel(new Label("Targeted Map")));

        var target = new AnnotationTarget("https://example.org/canvas.json")
            .SetSelector(FragmentSelector.ForRegion(920, 3600, 1510, 3000));

        return new Annotation("https://example.org/geoAnno.json", feature, target).SetMotivation("tagging");
    }

    [Fact]
    public void Canvas_Should_RoundTripFeatureAnnotationBodyThroughIiifSerializer()
    {
        // The recipe puts the geo-tagging Annotation on an external "annotations" AnnotationPage,
        // but the SDK's hand-rolled reader only resolves painting Annotations from a Canvas's own
        // "items"; exercise the body dispatch directly the same way ReadV3AnnotationResource would.
        var geoAnnotation = BuildGeoTaggingAnnotation();
        var canvas = new Canvas("https://example.org/canvas.json", new Label("Pamphlet"), 7072, 5212).AddAnnotation(geoAnnotation);
        var manifest = new Manifest("https://example.org/manifest", new Label("Test")).AddItem(canvas);

        var json = IiifSerializer.Serialize(manifest);
        var obj = JObject.Parse(json);
        var bodyObj = obj["items"]![0]!["items"]![0]!["items"]![0]!["body"]!;

        bodyObj["type"]!.ToString().Should().Be("Feature");
        bodyObj["id"]!.ToString().Should().Be("https://example.org/geo.json");
        bodyObj["geometry"]!["type"]!.ToString().Should().Be("Polygon");
        bodyObj["@context"].Should().BeNull();

        var deserialized = IiifSerializer.DeserializeManifest(json);
        var roundTrippedBody = ((Canvas)deserialized.Items.Single()).Items
            .OfType<AnnotationPage>().Single().Items.OfType<Annotation>().Single().Body;

        roundTrippedBody.Should().BeOfType<Feature>();
        var roundTrippedFeature = (Feature)roundTrippedBody;
        roundTrippedFeature.Id.Should().Be("https://example.org/geo.json");
        roundTrippedFeature.Geometry!.Type.Should().Be(GeometryType.Polygon);
    }

    [Fact]
    public void Annotation_Should_RoundTripFeatureBodyThroughPlainJsonConvert()
    {
        var annotation = BuildGeoTaggingAnnotation();

        var deserialized = TrackableObject.Parse<Annotation>(annotation.Serialize());

        deserialized.Body.Should().BeOfType<Feature>();
        ((Feature)deserialized.Body).Id.Should().Be("https://example.org/geo.json");
    }
}