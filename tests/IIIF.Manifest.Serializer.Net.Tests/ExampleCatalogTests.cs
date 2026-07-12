using System.Collections.Generic;
using System.Linq;
using IIIF.Manifests.Serializer.Examples;
using IIIF.Manifests.Serializer.Net.Cookbook;
using IIIF.Manifests.Serializer.Nodes;
using Newtonsoft.Json.Linq;
using ExampleDefinition = IIIF.Manifests.Serializer.Net.Cookbook.ExampleDefinition;

namespace IIIF.Manifests.Serializer.Tests;

/// <summary>
///     Milestone 7 (SDK_VERSIONING_GUIDE.md): every Manifest/Collection example is round-tripped
///     through the version-aware IiifSerializer (both directions, both versions) rather than plain
///     JsonConvert - this is the strongest regression suite in the repo given how much of the model
///     the ~20 cookbook/demo recipes exercise. Layer/AnnotationList examples have no 3.0 concept and
///     therefore no IiifSerializer support by design (see SDK_VERSIONING_GUIDE.md §4/§6); those still
///     fall back to a plain-JSON validity check.
/// </summary>
public class ExampleCatalogTests
{
    public static IEnumerable<object[]> CookbookExamples => CookbookCatalog.GetAll().Select(example => new object[] { example });
    public static IEnumerable<object[]> DemoExamples => DemoCatalog.GetAll().Select(example => new object[] { example });

    [Theory]
    [MemberData(nameof(CookbookExamples))]
    public void Cookbook_examples_should_round_trip_through_IiifSerializer(ExampleDefinition example)
    {
        AssertRoundTrips(example.Title, example.Build());
    }

    [Theory]
    [MemberData(nameof(DemoExamples))]
    public void Demo_examples_should_round_trip_through_IiifSerializer(Examples.ExampleDefinition example)
    {
        AssertRoundTrips(example.Title, example.Build());
    }

    private static void AssertRoundTrips(string title, object value)
    {
        title.Should().NotBeNullOrWhiteSpace();

        switch (value)
        {
            case Manifest manifest:
                AssertManifestRoundTrips(manifest);
                break;
            case Collection collection:
                AssertCollectionRoundTrips(collection);
                break;
            default:
                // Layer/AnnotationList: legacy-only, no 3.0 replacement, no IiifSerializer support.
                AssertSerializesAsValidJson(value);
                break;
        }
    }

    private static void AssertManifestRoundTrips(Manifest manifest)
    {
        var v2Json = IiifSerializer.Serialize(manifest, new IiifSerializerOptions(IiifPresentationVersion.V2_1));
        JToken.Parse(v2Json).Should().NotBeNull();

        var v3Json = IiifSerializer.Serialize(manifest, new IiifSerializerOptions());
        JToken.Parse(v3Json).Should().NotBeNull();

        var fromV2 = IiifSerializer.DeserializeManifest(v2Json);
        IiifSerializer.Serialize(fromV2, new IiifSerializerOptions()).Should().NotBeNullOrWhiteSpace();

        var fromV3 = IiifSerializer.DeserializeManifest(v3Json);
        IiifSerializer.Serialize(fromV3, new IiifSerializerOptions(IiifPresentationVersion.V2_1)).Should().NotBeNullOrWhiteSpace();
    }

    private static void AssertCollectionRoundTrips(Collection collection)
    {
        var v2Json = IiifSerializer.Serialize(collection, new IiifSerializerOptions(IiifPresentationVersion.V2_1));
        JToken.Parse(v2Json).Should().NotBeNull();

        var v3Json = IiifSerializer.Serialize(collection, new IiifSerializerOptions());
        JToken.Parse(v3Json).Should().NotBeNull();

        var fromV2 = IiifSerializer.DeserializeCollection(v2Json);
        IiifSerializer.Serialize(fromV2, new IiifSerializerOptions()).Should().NotBeNullOrWhiteSpace();

        var fromV3 = IiifSerializer.DeserializeCollection(v3Json);
        IiifSerializer.Serialize(fromV3, new IiifSerializerOptions(IiifPresentationVersion.V2_1)).Should().NotBeNullOrWhiteSpace();
    }

    private static void AssertSerializesAsValidJson(object value)
    {
        var json = JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        json.Should().NotBeNullOrWhiteSpace();
        JToken.Parse(json).Should().NotBeNull();
    }
}