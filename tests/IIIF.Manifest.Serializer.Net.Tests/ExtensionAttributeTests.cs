using System.Linq;
using System.Reflection;
using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Extensions;
using IIIF.Manifests.Serializer.Extensions.ResourceCoords;
using IIIF.Manifests.Serializer.Extensions.Transformations;
using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Nodes.Contents.Annotation;
using IIIF.Manifests.Serializer.Nodes.Contents.Textual.Resource;
using IIIF.Manifests.Serializer.Shared.Content.Resources;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Tests;

/// <summary>
///     Milestone 6 (SDK_VERSIONING_GUIDE.md): the navPlace/Georeference/TextGranularity
///     *ExtensionAttribute types were defined but never applied anywhere (confirmed dead code by
///     the initial codebase scan). Now applied to the relevant extension properties/methods/types;
///     these tests confirm both the attributes are actually present via reflection, and that the
///     extensions still round-trip correctly through JSON after the core BaseNode/BaseItem reshape.
/// </summary>
public class ExtensionAttributeTests
{
    [Fact]
    public void NavPlace_Should_RoundTripThroughManifestJson()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Map"));
        manifest.SetNavPlace(new NavPlace("https://example.org/navplace")
            .AddFeature(new Feature("https://example.org/feature/1")
                .SetGeometry(new Geometry(GeometryType.Point).AddCoordinate(new CoordinateItem(-73.9857, 40.7484)))
                .SetProperties(new FeatureProperties().AddLabel(new Label("New York")))));

        var json = JsonConvert.SerializeObject(manifest);
        var deserialized = JsonConvert.DeserializeObject<Manifest>(json)!;

        deserialized.NavPlace.Should().NotBeNull();
        var feature = deserialized.NavPlace!.Features.Single();
        feature.Geometry!.Type.Value.Should().Be("Point");
        feature.Geometry.Coordinates.Single().Longitude.Should().Be(-73.9857);
        feature.Properties!.Label.Single().Value.Should().Be("New York");
    }

    [Fact]
    public void Georeference_Should_RoundTripThroughManifestJson()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Georeferenced Map"));
        manifest.SetTransformation(new ThinPlateSplineTransformation());

        var feature = new Feature("https://example.org/feature/1")
            .SetProperties(new FeatureProperties().SetResourceCoords([100.0, 200.0]));
        manifest.SetNavPlace(new NavPlace("https://example.org/navplace").AddFeature(feature));

        var json = JsonConvert.SerializeObject(manifest);
        var deserialized = JsonConvert.DeserializeObject<Manifest>(json)!;

        deserialized.Transformation.Should().NotBeNull();
        deserialized.Transformation!.Type.Value.Should().Be("thinPlateSpline");

        deserialized.NavPlace!.Features.Single().Properties!.ResourceCoords.Should().BeEquivalentTo([100.0, 200.0]);
    }

    [Fact]
    public void TextGranularity_Should_RoundTripOnAnAnnotationResource()
    {
        var resource = new BaseResource("https://example.org/resource/1", ResourceType.Annotation);
        resource.SetTextGranularity(TextGranularity.Word);

        var json = JsonConvert.SerializeObject(resource);
        var deserialized = JsonConvert.DeserializeObject<BaseResource>(json)!;

        deserialized.TextGranularity.Should().Be(TextGranularity.Word);
    }

    [Fact]
    public void TextGranularity_Should_RejectNonAnnotationResources()
    {
        var resource = new BaseResource("https://example.org/resource/1", ResourceType.Image);

        var act = () => resource.SetTextGranularity(TextGranularity.Word);

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void NavPlace_Should_RoundTripThroughIiifSerializer_OnManifest()
    {
        // Regression guard: IiifSerializer.Serialize/DeserializeManifest use a hand-rolled V3
        // reader/writer that builds its JObject field-by-field rather than going through
        // Newtonsoft's automatic property serialization - it never reached the JsonExtensionData
        // bridge extension properties rely on, so navPlace was silently dropped by the SDK's
        // primary, documented entry point even though it worked via plain JsonConvert (see
        // SDK_VERSIONING_GUIDE.md Round 12).
        var manifest = new Manifest("https://example.org/manifest", new Label("Map"));
        manifest.SetNavPlace(new NavPlace("https://example.org/navplace")
            .AddFeature(new Feature("https://example.org/feature/1")
                .SetGeometry(new Geometry(GeometryType.Point).AddCoordinate(new CoordinateItem(-73.9857, 40.7484)))
                .SetProperties(new FeatureProperties().AddLabel(new Label("New York")))));

        var json = IiifSerializer.Serialize(manifest);
        var obj = JObject.Parse(json);

        obj["navPlace"]!["type"]!.ToString().Should().Be("FeatureCollection");
        obj["navPlace"]!["features"]![0]!["geometry"]!["type"]!.ToString().Should().Be("Point");

        var deserialized = IiifSerializer.DeserializeManifest(json);
        deserialized.NavPlace.Should().NotBeNull();
        var feature = deserialized.NavPlace!.Features.Single();
        feature.Geometry!.Coordinates.Single().Longitude.Should().Be(-73.9857);
        feature.Properties!.Label.Single().Value.Should().Be("New York");
    }

    [Fact]
    public void NavPlace_Should_RoundTripThroughIiifSerializer_OnCanvas()
    {
        var canvas = new Canvas("https://example.org/canvas/1", new Label("p1"), 100, 100);
        canvas.SetNavPlace(new NavPlace("https://example.org/navplace")
            .AddFeature(new Feature("https://example.org/feature/1")
                .SetGeometry(new Geometry(GeometryType.Point).AddCoordinate(new CoordinateItem(10.0, 20.0)))));
        var manifest = new Manifest("https://example.org/manifest", new Label("Test")).AddItem(canvas);

        var json = IiifSerializer.Serialize(manifest);
        var obj = JObject.Parse(json);
        obj["items"]![0]!["navPlace"]!["features"]![0]!["geometry"]!["type"]!.ToString().Should().Be("Point");

        var deserialized = IiifSerializer.DeserializeManifest(json);
        var roundTrippedCanvas = (Canvas)deserialized.Items.Single();
        roundTrippedCanvas.NavPlace!.Features.Single().Geometry!.Coordinates.Single().Longitude.Should().Be(10.0);
    }

    [Fact]
    public void NavPlace_Should_PreserveUnknownGeoJsonProperties_ThroughIiifSerializer()
    {
        // The navPlace object itself is a plain TrackableObject (its own [JsonExtensionData]
        // bridge applies once IiifSerializer hands the whole "navPlace" JToken through), so an
        // unmodeled GeoJSON key nested inside it must survive a full IiifSerializer round-trip.
        const string json = """
                            {
                              "@context": "http://iiif.io/api/presentation/3/context.json",
                              "id": "https://example.org/manifest",
                              "type": "Manifest",
                              "label": { "none": ["Map"] },
                              "navPlace": {
                                "id": "https://example.org/navplace",
                                "type": "FeatureCollection",
                                "features": [
                                  {
                                    "id": "https://example.org/feature/1",
                                    "type": "Feature",
                                    "geometry": { "type": "Point", "coordinates": [10.0, 20.0] },
                                    "bbox": [10.0, 20.0, 10.0, 20.0]
                                  }
                                ]
                              },
                              "items": []
                            }
                            """;

        var manifest = IiifSerializer.DeserializeManifest(json);
        var roundTripped = JObject.Parse(IiifSerializer.Serialize(manifest));

        roundTripped["navPlace"]!["features"]![0]!["bbox"]!.Values<double>().Should().BeEquivalentTo([10.0, 20.0, 10.0, 20.0]);
    }

    [Theory]
    [InlineData("page")]
    [InlineData("block")]
    [InlineData("paragraph")]
    [InlineData("line")]
    [InlineData("word")]
    [InlineData("glyph")]
    public void TextGranularity_Should_RoundTripThroughIiifSerializer_OnAnnotationInsideManifest(string value)
    {
        // Same regression class as navPlace above: WriteV3Annotation/ReadV3Annotation build their
        // JObject by hand and never touched the extension-data bridge, so textGranularity was
        // silently dropped by IiifSerializer even though the bare-resource JsonConvert path
        // (TextGranularity_Should_RoundTripOnAnAnnotationResource, above) already worked.
        var resource = new TextualBody("Example");
        var annotation = new Annotation("https://example.org/anno/1", resource, "https://example.org/canvas/1#xywh=10,10,100,30")
            .SetMotivation("supplementing");
        annotation.SetTextGranularity(TextGranularity.Parse(value));
        var canvas = new Canvas("https://example.org/canvas/1", new Label("p1"), 100, 100).AddAnnotation(annotation);
        var manifest = new Manifest("https://example.org/manifest", new Label("Test")).AddItem(canvas);

        var json = IiifSerializer.Serialize(manifest);
        var obj = JObject.Parse(json);
        obj["items"]![0]!["items"]![0]!["items"]![0]!["textGranularity"]!.ToString().Should().Be(value);

        var deserialized = IiifSerializer.DeserializeManifest(json);
        var roundTrippedAnnotation = ((Canvas)deserialized.Items.Single()).Items
            .OfType<AnnotationPage>().Single().Items.OfType<Annotation>().Single();

        roundTrippedAnnotation.TextGranularity!.Value.Should().Be(value);
    }

    [Fact]
    public void Register_Should_BeIdempotent_ForAllThreeExtensions()
    {
        var act = () =>
        {
            NavPlaceExtensions.Register();
            NavPlaceExtensions.Register();
            GeoreferenceExtensions.Register();
            GeoreferenceExtensions.Register();
            TextGranularityExtensions.Register();
            TextGranularityExtensions.Register();
        };

        act.Should().NotThrow();
    }

    [Theory]
    [InlineData(typeof(NavPlaceExtensions), "NavPlace", typeof(NavPlaceExtensionAttribute))]
    [InlineData(typeof(TransformationExtensions), "Transformation", typeof(GeoreferenceExtensionAttribute))]
    [InlineData(typeof(ResourceCoordExtensions), "ResourceCoords", typeof(GeoreferenceExtensionAttribute))]
    [InlineData(typeof(TextGranularityExtensions), "TextGranularity", typeof(TextGranularityExtensionAttribute))]
    public void ExtensionProperty_Should_CarryItsVersionAttribute(Type extensionClass, string memberName, Type expectedAttribute)
    {
        // C# extension members compile into compiler-synthesized nested types rather than plain
        // static members directly on the declaring class, so search the whole declaring
        // assembly for any member (in any type) whose name matches and carries the attribute.
        var hasAttribute = extensionClass.Assembly.GetTypes()
            .SelectMany(t => t.GetMembers(BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance))
            .Where(m => m.Name.Contains(memberName))
            .Any(m => m.GetCustomAttributes(expectedAttribute, false).Length > 0);

        hasAttribute.Should().BeTrue($"a member named '{memberName}' somewhere in {extensionClass.Assembly.GetName().Name} should carry {expectedAttribute.Name}");
    }
}