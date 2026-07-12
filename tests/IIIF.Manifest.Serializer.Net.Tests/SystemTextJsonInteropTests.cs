using System.Linq;
using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Nodes.Contents.Annotation;
using IIIF.Manifests.Serializer.Nodes.Contents.ContentState;
using IIIF.Manifests.Serializer.Nodes.Contents.Image.Resource;
using Newtonsoft.Json.Linq;
using STJ = System.Text.Json;

namespace IIIF.Manifests.Serializer.Tests;

/// <summary>
/// A developer who serializes/deserializes a Manifest/Collection/AnnotationCollection/ContentState
/// directly with <c>System.Text.Json</c> (e.g. because ASP.NET Core defaults to it, or they just
/// forgot to call <see cref="IiifSerializer"/>) must still get correct IIIF JSON, not whatever
/// System.Text.Json's own reflection-based default would produce against types that only carry
/// Newtonsoft attributes. Each of the 4 top-level document types carries a
/// <c>[System.Text.Json.Serialization.JsonConverter]</c> that bridges to this SDK's existing
/// Newtonsoft-based read/write logic, so no consumer configuration is required.
/// </summary>
public class SystemTextJsonInteropTests
{
    [Fact]
    public void Manifest_Should_SerializeToV3Json_ViaSystemTextJson_WithNoExtraConfiguration()
    {
        var manifest = CreateImageManifest();

        var stjJson = STJ.JsonSerializer.Serialize(manifest);
        var newtonsoftJson = IiifSerializer.Serialize(manifest);

        JToken.Parse(stjJson).Should().BeEquivalentTo(JToken.Parse(newtonsoftJson));
    }

    [Fact]
    public void Manifest_Should_RoundTrip_ViaSystemTextJson()
    {
        var manifest = CreateImageManifest();
        var stjJson = STJ.JsonSerializer.Serialize(manifest);

        var deserialized = STJ.JsonSerializer.Deserialize<Manifest>(stjJson);

        deserialized.Should().NotBeNull();
        deserialized!.Id.Should().Be(manifest.Id);
        deserialized.Items.Should().HaveCount(1);
    }

    [Fact]
    public void Manifest_Should_RoundTrip_FromLegacyV2Json_ViaSystemTextJson()
    {
        var manifest = CreateImageManifest();
        var legacyJson = IiifSerializer.Serialize(manifest, new IiifSerializerOptions(IiifPresentationVersion.V2_1));

        var deserialized = STJ.JsonSerializer.Deserialize<Manifest>(legacyJson);

        deserialized.Should().NotBeNull();
        deserialized!.Id.Should().Be(manifest.Id);
        deserialized.Sequences.Single().Canvases.Should().ContainSingle();
    }

    [Fact]
    public void Collection_Should_SerializeToV3Json_ViaSystemTextJson_WithNoExtraConfiguration()
    {
        var collection = new Collection("https://example.org/collection", new Label("Test Collection"));
        collection.AddItem(new Manifest("https://example.org/manifest", new Label("M")));

        var stjJson = STJ.JsonSerializer.Serialize(collection);
        var newtonsoftJson = IiifSerializer.Serialize(collection);

        JToken.Parse(stjJson).Should().BeEquivalentTo(JToken.Parse(newtonsoftJson));
    }

    [Fact]
    public void Collection_Should_RoundTrip_ViaSystemTextJson()
    {
        var collection = new Collection("https://example.org/collection", new Label("Test Collection"));
        collection.AddItem(new Manifest("https://example.org/manifest", new Label("M")));
        var stjJson = STJ.JsonSerializer.Serialize(collection);

        var deserialized = STJ.JsonSerializer.Deserialize<Collection>(stjJson);

        deserialized.Should().NotBeNull();
        deserialized!.Id.Should().Be(collection.Id);
        deserialized.Items.Should().ContainSingle();
    }

    [Fact]
    public void AnnotationCollection_Should_SerializeAndRoundTrip_ViaSystemTextJson()
    {
        var annotationCollection = new AnnotationCollection("https://example.org/anno_coll.json", new Label("Newspaper layout markup"))
            .SetTotal(8)
            .SetFirst("https://example.org/anno_p1.json")
            .SetLast("https://example.org/anno_p2.json");

        var stjJson = STJ.JsonSerializer.Serialize(annotationCollection);
        var newtonsoftJson = IiifSerializer.Serialize(annotationCollection);
        JToken.Parse(stjJson).Should().BeEquivalentTo(JToken.Parse(newtonsoftJson));

        var deserialized = STJ.JsonSerializer.Deserialize<AnnotationCollection>(stjJson);
        deserialized.Should().NotBeNull();
        deserialized!.Total.Should().Be(8);
        deserialized.First.Should().Be("https://example.org/anno_p1.json");
    }

    [Fact]
    public void ContentState_Should_SerializeAndRoundTrip_ViaSystemTextJson()
    {
        var contentState = new ContentState(new ContentStateTarget("https://example.org/manifest/canvas1", "Canvas"))
            .SetId("https://example.org/state.json");

        var stjJson = STJ.JsonSerializer.Serialize(contentState);
        var newtonsoftJson = contentState.Serialize();
        JToken.Parse(stjJson).Should().BeEquivalentTo(JToken.Parse(newtonsoftJson));

        var deserialized = STJ.JsonSerializer.Deserialize<ContentState>(stjJson);
        deserialized.Should().NotBeNull();
        deserialized!.Id.Should().Be("https://example.org/state.json");
        deserialized.Target.Should().ContainSingle(x => x.ResourceType == "Canvas");
    }

    private static Manifest CreateImageManifest()
    {
        var canvas = new Canvas("https://example.org/canvas/1", new Label("p. 1"), 1000, 800);
        var resource = new ImageResource("https://example.org/image.png", "image/png")
            .SetHeight(1000)
            .SetWidth(800);
        canvas.AddAnnotation(new Annotation("https://example.org/annotation/1", resource, canvas.Id));

        var manifest = new Manifest("https://example.org/manifest", new Label("Single Image Example"));
        manifest.AddItem(canvas);
        manifest.SetSequenceId("https://example.org/sequence/normal");
        return manifest;
    }
}
