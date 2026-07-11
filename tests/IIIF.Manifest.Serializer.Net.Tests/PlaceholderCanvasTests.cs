using System.Linq;
using IIIF.Manifests.Serializer.Nodes;
using IIIF.Manifests.Serializer.Nodes.Contents.Annotation;
using IIIF.Manifests.Serializer.Nodes.Contents.Image.Resource;
using IIIF.Manifests.Serializer.Nodes.Contents.Video.Resource;
using Newtonsoft.Json.Linq;

namespace IIIF.Manifests.Serializer.Tests;

/// <summary>
/// Cookbook Group E: recipe 0013-placeholderCanvas - a full embedded Canvas shown before an A/V
/// Canvas's own content is available (e.g. a poster-frame thumbnail). Fixed a pre-existing defect
/// along the way: <see cref="Manifest.PlaceholderCanvas"/> previously modeled this as a bare
/// string tagged 2.0, but the property is 3.0-only and always a full Canvas object per spec §5.4.2.
/// </summary>
public class PlaceholderCanvasTests
{
    private static Canvas BuildPlaceholder()
    {
        var image = new ImageResource("https://fixtures.iiif.io/video/act1-thumbnail.png", "image/png").SetHeight(360).SetWidth(640);
        var annotation = new Annotation("https://example.org/canvas/donizetti/placeholder/1-image", image,
            "https://example.org/canvas/donizetti/placeholder");
        return new Canvas("https://example.org/canvas/donizetti/placeholder", new Label("placeholder"), 360, 640)
            .AddAnnotation(annotation);
    }

    [Fact]
    public void Canvas_Should_RoundTripPlaceholderCanvasThroughIiifSerializer()
    {
        var video = new VideoResource("https://fixtures.iiif.io/video/act1.mp4", "video/mp4").SetDuration(7278.466).SetHeight(360).SetWidth(640);
        var mainAnnotation = new Annotation("https://example.org/annotation/1", video, "https://example.org/canvas/donizetti");
        var canvas = new Canvas("https://example.org/canvas/donizetti", new Label("Donizetti"), 360, 640)
            .SetDuration(7278.466)
            .AddAnnotation(mainAnnotation)
            .SetPlaceholderCanvas(BuildPlaceholder());

        var manifest = new Manifest("https://example.org/manifest", new Label("Test")).AddItem(canvas);

        var json = IiifSerializer.Serialize(manifest);
        var obj = JObject.Parse(json);
        var canvasObj = obj["items"]![0]!;

        canvasObj["placeholderCanvas"]!["type"]!.ToString().Should().Be("Canvas");
        canvasObj["placeholderCanvas"]!["id"]!.ToString().Should().Be("https://example.org/canvas/donizetti/placeholder");
        canvasObj["placeholderCanvas"]!["items"]![0]!["items"]![0]!["body"]!["type"]!.ToString().Should().Be("Image");

        var deserialized = IiifSerializer.DeserializeManifest(json);
        var roundTrippedCanvas = (Canvas)deserialized.Items.Single();

        roundTrippedCanvas.PlaceholderCanvas.Should().NotBeNull();
        roundTrippedCanvas.PlaceholderCanvas!.Id.Should().Be("https://example.org/canvas/donizetti/placeholder");
        roundTrippedCanvas.PlaceholderCanvas.Items.OfType<AnnotationPage>().Single()
            .Items.OfType<Annotation>().Single().Body.Should().BeOfType<ImageResource>();
    }

    [Fact]
    public void Manifest_Should_RoundTripPlaceholderCanvasThroughIiifSerializer()
    {
        var manifest = new Manifest("https://example.org/manifest", new Label("Test")).SetPlaceholderCanvas(BuildPlaceholder());

        var json = IiifSerializer.Serialize(manifest);
        var obj = JObject.Parse(json);

        obj["placeholderCanvas"]!["type"]!.ToString().Should().Be("Canvas");

        var deserialized = IiifSerializer.DeserializeManifest(json);
        deserialized.PlaceholderCanvas.Should().NotBeNull();
        deserialized.PlaceholderCanvas!.Id.Should().Be("https://example.org/canvas/donizetti/placeholder");
    }

    [Fact]
    public void Canvas_Should_OmitPlaceholderCanvasWhenNotSet()
    {
        var canvas = new Canvas("https://example.org/canvas/1", new Label("c1"), 100, 100);
        var manifest = new Manifest("https://example.org/manifest", new Label("Test")).AddItem(canvas);

        var json = IiifSerializer.Serialize(manifest);
        var obj = JObject.Parse(json);

        obj["items"]![0]!["placeholderCanvas"].Should().BeNull();
    }
}
