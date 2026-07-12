using System.Linq;
using System.Reflection;
using IIIF.Manifests.Serializer.Attributes;
using IIIF.Manifests.Serializer.Extensions;
using IIIF.Manifests.Serializer.Extensions.ResourceCoords;
using IIIF.Manifests.Serializer.Extensions.Transformations;
using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Shared.Content.Resources;

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